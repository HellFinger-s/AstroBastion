using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stealth : MonoBehaviour
{
    private bool active = true;
    private bool initiallyEnabled = false;

    [SerializeField] private float rebootTime = 4f;
    [SerializeField] private MeshRenderer visualization;

    private Coroutine reboot;

    public void Enable()
    {
        if (reboot is not null)
        {
            StopCoroutine(reboot);
            reboot = null;
        }
        active = true;
        initiallyEnabled = true;
        visualization.enabled = true;
    }

    public void Disable()
    {
        active = false;
        visualization.enabled = false;
        if (reboot is not null)
        {
            StopCoroutine(reboot);
            reboot = null;
        }
        if (gameObject.activeInHierarchy)
        {
            reboot = StartCoroutine(Reboot());
        }
    }

    public bool IsActive()
    {
        return active;
    }

    private IEnumerator Reboot()
    {
        yield return new WaitForSeconds(rebootTime);
        Enable();
    }

    public bool IsInitiallyEnabled()
    {
        return initiallyEnabled;
    }
}
