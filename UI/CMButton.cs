using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Serialization;

[System.Serializable]
public class CMButton : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    public List<Image> upgradeMarkers;
    public Image buttonIcon;
    public Image selectedBorder;
    public Image background;
    public GameObject spaceIcon;

    public TMP_Text costText;

    private int upgradeLevel = 0;
    
    public BuildableTypes attachedBuildableKey;
    public IconTypes iconType;
    public TargetPolicy attachedPolicy;
    public IButtonTextTemplate textTemplate;

    public string operationCost = "";

    [FormerlySerializedAs("controllMatrix")]
    public ControlMatrix controlMatrix;

    private Color transparentColor = new Color(0, 0, 0, 0);

    private Sprite normalIcon;
    private Sprite hoveredIcon = null;


    public void Call()
    {
        DeSelected();
        controlMatrix.ButtonAction(this);
        //RefreshTooltip();
    }

    public void ChangeIcon(Sprite newIcon)
    {
        buttonIcon.sprite = newIcon;
        normalIcon = newIcon;
    }

    public void IncreaseUpgradeLevel(int currentLevel)
    {
        upgradeMarkers[currentLevel - 1].color = Color.green;
    }

    public void SetActiveMarkers(int markersCount)
    {
        for (int i = 0; i < upgradeMarkers.Count; i++)
        {
            if (i < markersCount)
                upgradeMarkers[i].color = Color.green;
            else
                upgradeMarkers[i].color = transparentColor;
        }
    }

    public void ChangeIconVisibility(bool visibility)
    {
        buttonIcon.enabled = visibility;
    }

    public void SetIconType(IconTypes newIconType)
    {
        iconType = newIconType;
    }

    public void SetBuildableKey(BuildableTypes key)
    {
        attachedBuildableKey = key;
    }

    public void SetAttachedPolicy(TargetPolicy policy)
    {
        attachedPolicy = policy;
    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    RefreshTooltip();
    //}


    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    HideTooltip();        
    //}

    public void RefreshTooltip()
    {
        if (textTemplate is not null)
        {
        controlMatrix.ShowButtonText(textTemplate.ReturnText(this));
            if (operationCost != "")
            {
                buttonIcon.sprite = hoveredIcon;
                costText.text = operationCost;
                background.enabled = true;
            }
        }
    }

    public void HideTooltip()
    {
        controlMatrix.HideButtonText();
        buttonIcon.sprite = normalIcon;
        background.enabled = false;
        costText.text = "";
    }

    public void Selected()
    {
        selectedBorder.enabled = true;
        //background.enabled = true;
        if (iconType != IconTypes.Hidden)
        {
            spaceIcon.SetActive(true);
            RefreshTooltip();
        }
    }

    public void DeSelected()
    {
        selectedBorder.enabled = false;
        HideTooltip();
        spaceIcon.SetActive(false);
    }
}
