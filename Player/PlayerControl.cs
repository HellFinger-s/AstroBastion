using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour, IReleaseProjectile
{
    public bool controlLocked = false;

    public Transform movableTransform;

    public GameObject camParent;
    public Camera playerCam;
    public GameObject playerShip;
    public ParticleSystem warpParticles;

    public Warlord warlord;

    public float maxSpeed;
    public float rollSpeed = 10f;
    public float rotationSpeed = 10f;
    public float speedModifier = 1f;
    public float boostModifier = 2f;
    public int overheatDamage = 10;
    public float engineTempChangeSpeed = 1f;
    public float rotationSensitivity = 3f;
    public float camRotationLag = 5f;

    private int currentHP = 100;
    public int maxHP = 100;

    public float engineTemp = 0f; //[0, 1]

    public float selectRaycastDistance = 1000f;
    public LayerMask raycastLayerMask = ~0;

    public LayerMask enemyInfoLayerMask = ~0;
    public float enemyInfoRaycastDistance = 1000f;

    public LayerMask waveInfoLayerMask = ~0;
    public float waveInfoRaycastDistance = 1000f;

    public BaseProjectile projectile;
    public int poolCapacity;
    public float aimDistance = 2000f;
    public List<Transform> muzzles;
    public float reloadSeconds;

    private bool builderMode = false;
    private bool boosted = false;

    private float enemyInfoTimer = 0f;
    private float enemyInfoUpdateInterval = 0.2f;
    private float shootTimer = 0f;

    //input
    private float moveX;
    private float moveY;
    private float moveZ;
    private float roll;
    private float mouseX;
    private float mouseY;

    private float compForward = 0;

    private Vector2 mouseStart;
    private Vector2 mousePosition;

    private Ray interactableRay;

    private Platform highlightedPlatform;
    private Platform selectedPlatform;
    private BaseTower highlightedTower;

    private Quaternion targetRotation;

    private Coroutine receiveDamageByOverheat = null;

    private Queue<BaseProjectile> pool = new Queue<BaseProjectile> { };

    private float scrollAccumulator = 0f;

    private void Awake()
    {
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        mousePosition = new Vector2(Screen.width * 4 / 5, Screen.height / 4);
        FillPool();
    }   

    private void FillPool()
    {
        for (int i = pool.Count; i < poolCapacity; i++)
        {
            BaseProjectile temp = Instantiate(projectile);
            temp.gameObject.SetActive(false);
            temp.parentPool = this;
            pool.Enqueue(temp);
        }
    }

    void Update()
    {
        enemyInfoTimer += Time.deltaTime;
        shootTimer += Time.deltaTime;

        DoInteractRaycast();
        DoWaveInfoRaycast();

        if (!controlLocked)
        {
            moveX = Input.GetAxisRaw("Horizontal");
            moveY = Input.GetAxisRaw("Vertical");
            moveZ = Input.GetKey(KeyCode.Z) ? 1f : (Input.GetKey(KeyCode.X) ? -1f : 0f);
            roll = Input.GetKey(KeyCode.Q) ? 1f : (Input.GetKey(KeyCode.E) ? -1f : 0);
            mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

            targetRotation = transform.rotation;

            compForward = moveY * speedModifier * maxSpeed;

            movableTransform.position += moveX * -movableTransform.right * speedModifier * maxSpeed * Time.deltaTime;
            movableTransform.position += moveZ * -movableTransform.up * speedModifier * maxSpeed * Time.deltaTime;
            movableTransform.position += moveY * -movableTransform.forward * speedModifier * maxSpeed * Time.deltaTime;

            movableTransform.Rotate(-Vector3.forward, roll * rollSpeed * Time.deltaTime, Space.Self);
            //if (!builderMode)
            //{
                movableTransform.Rotate(mouseY, mouseX, 0f, Space.Self);
            //}

            CameraLag();

            if (/*!builderMode && */Input.GetMouseButtonDown(1))
            {
                DoRaycast();
            }

            //if (Input.GetMouseButtonDown(1))
            //{
            //    SwitchMode();
            //}

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Boost();
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                boosted = false;
                speedModifier /= boostModifier;
            }

            if (Input.GetKeyDown(KeyCode.Return) && warlord.preparation)
            {
                warlord.InterruptPreparation();
            }

            if (shootTimer > reloadSeconds && Input.GetMouseButton(0))
            {
                Shoot();
            }

            scrollAccumulator += Input.mouseScrollDelta.y;

            if (scrollAccumulator >= 1f)
            {
                scrollAccumulator -= 1f;
                LUbus.GetInstance().SwitchSelectedToRight();
            }
            else if (scrollAccumulator <= -1f)
            {
                scrollAccumulator += 1f;
                LUbus.GetInstance().SwitchSelectedToLeft();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                LUbus.GetInstance().PressSelectedButton();
            }

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LUbus.GetInstance().ToggleEscPanel();
        }

        
        EngineTempUpdate();
        if (enemyInfoTimer > enemyInfoUpdateInterval)
        {
            DoEnemyInfoRaycast();
            enemyInfoTimer = 0f;
        }
    }

    public void SwitchToShip()
    {
        builderMode = false;
        Cursor.visible = false;
        Mouse.current.WarpCursorPosition(mousePosition);
        mousePosition = Input.mousePosition;
        LUbus.GetInstance().EnableRaycastBlocker();
    }

    void DoRaycast()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = playerCam.ScreenPointToRay(screenCenter);

        if (Builder.GetInstance().isFree() && Physics.Raycast(ray, out RaycastHit hit, selectRaycastDistance, raycastLayerMask))
        {
            if (hit.collider.gameObject.TryGetComponent<Platform>(out Platform platform) && platform.isFree())
            {
                Tutorials.GetInstance().ShowTutorial(TutorialKeys.BuildMenu);
                if (platform != selectedPlatform)
                {
                    if (selectedPlatform is not null)
                    {
                        selectedPlatform.SetMaterial(MaterialKeys.normal);
                    }
                    if (highlightedPlatform is not null)
                    {
                        selectedPlatform = highlightedPlatform;
                        highlightedPlatform = null;
                    }
                }
                
                Builder.GetInstance().SetSelectedPlatform(platform);
                Builder.GetInstance().ClearSelectedTower();
                LUbus.GetInstance().SwitchState(CMStatesKeys.BasicBuyState);
                //SwitchToBuilder();
            }
            if (hit.collider.gameObject.TryGetComponent<BaseTower>(out BaseTower tower))
            {
                Builder.GetInstance().ClearSelectedTower();
                Builder.GetInstance().ClearSelectedPlatform();
                Builder.GetInstance().SetSelectedTowerAndPlatform(tower);
                //SwitchToBuilder();
                SwitchCMState(tower);
            }
        }
        else
        {
            Debug.Log("Raycast from center: no hit");
        }
    }


    private void DoInteractRaycast()
    {
        interactableRay = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Builder.GetInstance().isFree() && Physics.Raycast(interactableRay, out RaycastHit hit, selectRaycastDistance, raycastLayerMask))
        {
            if (hit.collider.TryGetComponent<Platform>(out Platform platform))
            {
                
                DisableTowerHighlight();
                if (platform != highlightedPlatform)
                {
                    if (highlightedPlatform is not null)
                    {
                        highlightedPlatform.SetMaterial(MaterialKeys.normal);
                    }
                    if (platform != selectedPlatform)
                    {
                        highlightedPlatform = platform;
                    }
                    platform.SetMaterial(MaterialKeys.interactable);
                }
            }
            else if (hit.collider.TryGetComponent<BaseTower>(out BaseTower baseTower) && baseTower != highlightedTower)
            {
                DisablePlatformHighlight();
                if (baseTower != highlightedTower)
                {
                    if (highlightedTower is not null)
                    {
                        highlightedTower.SetMaterial(MaterialKeys.normal);
                    }
                    highlightedTower = baseTower;
                    baseTower.SetMaterial(MaterialKeys.interactable);
                }
            }
        }
        else
        {
            DisablePlatformHighlight();
            DisableTowerHighlight();
        }
    }

    private void DoEnemyInfoRaycast()
    {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, enemyInfoRaycastDistance, enemyInfoLayerMask))
        {
            if (hit.collider.TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
            {
                LUbus.GetInstance().ShowEnemyInfo(enemy);
            }
        }
        else
        {
            LUbus.GetInstance().DisableEnemyInfo();
        }
    }

    private void DoWaveInfoRaycast()
    {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, waveInfoRaycastDistance, waveInfoLayerMask))
        {
            if (warlord.preparation && hit.collider.TryGetComponent<Portal>(out Portal portal))
            {
                portal.ShowWaveInfo(playerCam.WorldToScreenPoint(hit.point));
            }
        }
    }


    private void DisablePlatformHighlight()
    {
        if (highlightedPlatform)
        {
            highlightedPlatform.SetMaterial(MaterialKeys.normal);
            highlightedPlatform = null;
        }
    }

    private void DisableTowerHighlight()
    {
        if (highlightedTower)
        {
            highlightedTower.SetMaterial(MaterialKeys.normal);
            highlightedTower = null;
        }
    }

    private void CameraLag()
    {
        if (camParent != null)
        {
            camParent.transform.position = transform.position;

            camParent.transform.rotation = Quaternion.Slerp(
                camParent.transform.rotation,
                targetRotation,
                camRotationLag * Time.deltaTime
            );
        }
    }

    private void SwitchCMState(BaseTower tower)
    {
        if (tower.currentLevel < 3)
        {
            LUbus.GetInstance().SwitchState(CMStatesKeys.BasicControlState);
        }
        else if (tower.currentLevel == 3)
        {
            LUbus.GetInstance().SwitchState(CMStatesKeys.BasicLevel3ControlState);
        }
        else
        {
            LUbus.GetInstance().SwitchState(CMStatesKeys.Level4ControlState);
        }
    }

    public void ReceiveDamage(Damage damageComponent)
    {
        currentHP -= damageComponent.value;
        LUbus.GetInstance().UpdateHealthBarValue(currentHP, maxHP);
        if (currentHP < 0)
        {
            StartCoroutine(Destroy());
        }
    }

    public void ReceiveDamage(int value)
    {
        currentHP -= value;
        LUbus.GetInstance().UpdateHealthBarValue(currentHP, maxHP);
        if (currentHP < 0)
        {
            StartCoroutine(Destroy());
        }
    }

    private void Boost()
    {
        if (!boosted)
        {
            speedModifier *= boostModifier;
            boosted = true;
        }
    }

    private void EngineTempUpdate()
    {
        if (1f - engineTemp < 1e-3 && receiveDamageByOverheat is null)
        {
            receiveDamageByOverheat = StartCoroutine(ReceiveDamageByOverheat(1f));
        }

        if (boosted && engineTemp < 1f)
        {
            engineTemp += engineTempChangeSpeed * Time.deltaTime;
            if (engineTemp > 0.99f)
            {
                engineTemp = 1f;
            }
            LUbus.GetInstance().UpdateTempBarValue(engineTemp, 1f);
        }
        if (!boosted && engineTemp > 0)
        {
            engineTemp -= engineTempChangeSpeed * Time.deltaTime;
            if (engineTemp < 1e-2)
            {
                engineTemp = 0f;
            }
            LUbus.GetInstance().UpdateTempBarValue(engineTemp, 1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Platform" ||
            LayerMask.LayerToName(other.gameObject.layer) == "Tower" ||
            LayerMask.LayerToName(other.gameObject.layer) == "Enemy" ||
            LayerMask.LayerToName(other.gameObject.layer) == "Protectable")
        {
            StartCoroutine(Destroy());
        }

        if (other.TryGetComponent<Damage>(out Damage damageComponent))
        {
            ReceiveDamage(damageComponent);
        }
    }

    private IEnumerator ReceiveDamageByOverheat(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ReceiveDamage(overheatDamage);
        receiveDamageByOverheat = null;
    }

    public void ReceiveHeal(int value)
    {
        currentHP += value;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        LUbus.GetInstance().UpdateHealthBarValue(currentHP, maxHP);
    }

    public void FlushInput()
    {
        boosted = false;
        speedModifier = 1;
    }

    private IEnumerator Destroy()
    {
        FlushInput();
        transform.position = new Vector3(-260f, 0f, -1400f);
        playerShip.SetActive(false);
        CameraLag();
        controlLocked = true;
        LUbus.GetInstance().StartRespawnCount(5f);
        yield return new WaitForSeconds(5f);
        playerShip.SetActive(true);
        controlLocked = false;
        currentHP = maxHP;
        LUbus.GetInstance().UpdateHealthBarValue(currentHP, maxHP);
        engineTemp = 0f;
        LUbus.GetInstance().UpdateTempBarValue(engineTemp, 1f);
    }

    private void Shoot()
    {
        
        Ray aimRay = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 aimPoint = transform.position + aimRay.direction * aimDistance;
        for (int i = 0; i < muzzles.Count; i++)
        {
            Vector3 dir = (aimPoint - muzzles[i].position).normalized;
            if (pool.Count == 0)
            {
                poolCapacity *= 2;
                FillPool();
            }
            
            BaseProjectile projectile = pool.Dequeue();
            projectile.Setup(compForward);
            projectile.transform.position = muzzles[i].position;
            projectile.transform.rotation = Quaternion.LookRotation(dir);
            projectile.gameObject.SetActive(true);
        }
        shootTimer = 0f;
    }

    public void ReleaseProjectile(BaseProjectile projectile)
    {
        pool.Enqueue(projectile);
    }

    public void PlayWarpParticles()
    {
        warpParticles.Play();
    }
}
