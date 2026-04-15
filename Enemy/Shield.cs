using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public int maxValue;
    public float shutdownRegenerationTime = 2f;
    public float regenerationSpeed;

    public bool regeneration = false;
    private float regenAccumulator = 0f;
    public int currentValue;
    private Coroutine coroutine;

    private void Update()
    {
        if (regeneration)
        {
            regenAccumulator += regenerationSpeed * Time.deltaTime;

            int amountToAdd = Mathf.FloorToInt(regenAccumulator);
            if (amountToAdd > 0)
            {
                currentValue += amountToAdd;
                regenAccumulator -= amountToAdd;
            }
            if (currentValue > maxValue)
            {
                currentValue = maxValue;
                regeneration = false;
            }
        }
    }

    public void ResetShield()
    {
        currentValue = maxValue;
        if (coroutine is not null)
        {
            StopCoroutine(coroutine);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Damage>(out Damage damage))
        {
            if (coroutine is not null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(ToggleRegen(shutdownRegenerationTime));
            ReceiveDamage(damage, other);
        }
    }


    private void ReceiveDamage(Damage damageComponent, Collider collider)
    {
        currentValue -= damageComponent.value;
        DamageVisualizer.GetInstance().ShowDamage(collider.ClosestPoint(transform.position),
                damageComponent.gameObject.transform.position - transform.position,
                damageComponent.value * damageComponent.multiplier,
                DamageVisType.ShieldedDamage,
                damageComponent.multiplier > 1);
        if (currentValue <= 0)
        {
            currentValue = 0;
            if (coroutine is not null)
            {
                StopCoroutine(coroutine);
            }
            regeneration = false;
            gameObject.SetActive(false);
        }
    }


    private IEnumerator ToggleRegen(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        regeneration = true;
    }
}
