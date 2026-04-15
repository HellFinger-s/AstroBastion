using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDetector : MonoBehaviour
{
    public MonoBehaviour obj;

    private IReceiveTowers towersReceiver = null;

    private void Awake()
    {
        CheckReceiver();
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckReceiver();
        if (other.TryGetComponent<BaseTower>(out BaseTower tower))
        {
            Debug.LogError(towersReceiver is null);
            towersReceiver.AddTower(tower);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CheckReceiver();
        if (other.TryGetComponent<BaseTower>(out BaseTower tower))
        {
            towersReceiver.RemoveTower(tower);
        }
    }

    private void CheckReceiver()
    {
        if (towersReceiver is null)
        {
            towersReceiver = obj as IReceiveTowers;
            //Debug.LogError("HERE " + towersReceiver == null);
        }
    }
}
