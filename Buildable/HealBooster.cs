using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBooster : MonoBehaviour
{
    public MonoBehaviour parentTower;
    public int healValue;

    private IHealer parent;

    private void Awake()
    {
        parent = parentTower as IHealer;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerControl>(out PlayerControl player))
        {
            parent.ReloadBooster();
            player.ReceiveHeal(healValue);
            gameObject.SetActive(false);
        }
    }
}
