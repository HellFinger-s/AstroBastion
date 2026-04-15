using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class InfoBase : MonoBehaviour
{
    [SerializeField]
    private TMP_Text counter;
    [SerializeField]
    private List<GameObject> descriptions = new List<GameObject> { };
    [SerializeField]
    private GameObject infoBase;

    private int index = 0;
    private bool previousVisible;

    private void OnEnable()
    {
        UpdateCounter();
    }

    public void CloseBase()
    {
        Cursor.lockState = CursorLockMode.Confined;
        infoBase.SetActive(false);
        if (!previousVisible)
        {
            Cursor.visible = false;
            Time.timeScale = 1f;
            LUbus.GetInstance().SwitchToShip();
            LUbus.GetInstance().FreeControl();
        }
    }

    public void OpenBase()
    {
        previousVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        infoBase.SetActive(true);
        Time.timeScale = 0f;
        LUbus.GetInstance().LockControl();
        counter.text = string.Format("{0}/{1}", index + 1, descriptions.Count);
        UpdateCounter();
    }

    public void SwitchToRight()
    {
        descriptions[index].SetActive(false);
        index++;
        if (index == descriptions.Count)
        {
            index = 0;
        }
        descriptions[index].SetActive(true);
        UpdateCounter();
    }

    public void SwitchToLeft()
    {
        descriptions[index].SetActive(false);
        index--;
        if (index == -1)
        {
            index = descriptions.Count - 1;
        }
        descriptions[index].SetActive(true);
        UpdateCounter();
    }

    private void UpdateCounter()
    {
        counter.text = string.Format("{0}/{1}", index + 1, descriptions.Count);
    }
}
