using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTowersDispatcher : MonoBehaviour
{
    private List<PortalTower> connectedPortals = new List<PortalTower> { };
    private PortalTower freePortal;
    public GameObject connectionVisualizer;
    public int poolCapacity = 10;

    private Queue<GameObject> pool = new Queue<GameObject> { };

    private void Awake()
    {
        FillPool();
    }

    private void ExpandPool()
    {
        poolCapacity *= 2;
        FillPool();
    }

    public void SetFreePortal(PortalTower tower)
    {
        freePortal = tower;
    }

    public PortalTower GetFreePortal()
    {
        return freePortal;
    }

    public void AddPare(PortalTower tower1, PortalTower tower2)
    {
        if (pool.Count == 0)
        {
            ExpandPool();
        }
        tower1.SetConnectionVisualizer(pool.Peek());
        tower2.SetConnectionVisualizer(pool.Peek());
        pool.Peek().transform.position = tower1.transform.position;
        Vector3 dir = tower2.transform.position - tower1.transform.position;
        Debug.DrawLine(tower1.transform.position, tower2.transform.position, Color.red, 10f);
        pool.Peek().transform.LookAt(tower2.transform.position);
        pool.Peek().transform.localScale = new Vector3(0.2f, 0.2f, dir.magnitude / 2f);
        pool.Dequeue().SetActive(true);
        connectedPortals.Add(tower1);
        connectedPortals.Add(tower2);
    }

    private void FillPool()
    {
        for (int i = 0; i < poolCapacity; i++)
        {
            CreateVisualizer();
        }
    }

    private void CreateVisualizer()
    {
        GameObject instance = Instantiate(connectionVisualizer);
        instance.SetActive(false);
        pool.Enqueue(instance);
    }

    public void ReleaseVisualizer(GameObject visualizer)
    {
        visualizer.transform.localScale = Vector3.one;
        visualizer.SetActive(false);
        pool.Enqueue(visualizer);
    }
}
