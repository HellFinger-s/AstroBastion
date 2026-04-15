using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine.Serialization;


public enum CMStatesKeys
{
    BasicBuyState,
    BasicControlState,
    BasicLevel3ControlState,
    Level4ControlState,
    SolutionState,
    HiddenState
}


public class ControlMatrix : MonoBehaviour
{
    [System.Serializable]
    public class InnerImageList
    {
        public List<Image> innerList;
    }

    [FormerlySerializedAs("controllMatrixButtons")]
    public List<InnerList<CMButton>> controlMatrixButtons;

    public SO_Icons iconsDB;


    [SerializedDictionary]
    public SerializedDictionary<CMStatesKeys, BaseState> states;

    //[SerializedDictionary]
    private Dictionary<ButtonTextTemplates, IButtonTextTemplate> buttonTextTemplates = new();


    public GameObject buttonTextPanel;
    public TMP_Text buttonText;
    public GameObject raycastBlocker;


    private BaseState previousState;
    private BaseState currentState;

    private int selectedButtonRow = 0;
    private int selectedButtonColumn = 0;
    private int currentMaxSelectedIndex = 0;

    public void Awake()
    {
        SwitchState(CMStatesKeys.HiddenState);
        buttonTextTemplates.Add(ButtonTextTemplates.Buildable, new BuildableTextTemplate());
        buttonTextTemplates.Add(ButtonTextTemplates.CubeDestroy, new CubeDestroyTextTemplate());
        buttonTextTemplates.Add(ButtonTextTemplates.LeftUpgrade, new LeftUpgradeTextTemplate());
        buttonTextTemplates.Add(ButtonTextTemplates.RightUpgrade, new RightUpgradeTextTemplate());
        buttonTextTemplates.Add(ButtonTextTemplates.LevelUp, new LevelUpTextTemplate());
        buttonTextTemplates.Add(ButtonTextTemplates.TowerDestroy, new TowerDestroyTextTemplate());
        buttonTextTemplates.Add(ButtonTextTemplates.TowerRepair, new TowerRepairTextTemplate());
        buttonTextTemplates.Add(ButtonTextTemplates.Policy, new PolicyTextTemplate());
    }

    public void SwitchState(CMStatesKeys stateKey)
    {
        if (currentState is not null)
        {
            currentState.Exit();
            previousState = currentState;
            if (previousState.GetType() != states[stateKey].GetType())
            {
                controlMatrixButtons[selectedButtonRow].innerList[selectedButtonColumn].DeSelected();
                selectedButtonColumn = 0;
                selectedButtonRow = 0;
            }
        }
        currentState = states[stateKey];
        //currentMaxSelectedIndex = states[stateKey].maxSelectedIndex;
        SetupButtons(states[stateKey]);
        states[stateKey].Enter();
        controlMatrixButtons[selectedButtonRow].innerList[selectedButtonColumn].Selected();

    }

    public void SwitchState(BaseState newState)
    {
        if (currentState is not null)
        {
            currentState.Exit();
            previousState = currentState;
            if (previousState.GetType() != newState.GetType())
            {
                controlMatrixButtons[selectedButtonRow].innerList[selectedButtonColumn].DeSelected();
                selectedButtonColumn = 0;
                selectedButtonRow = 0;
            }
        }
        currentState = newState;
        SetupButtons(newState);
        newState.Enter();
        controlMatrixButtons[selectedButtonRow].innerList[selectedButtonColumn].Selected();
    }

    public void GetBack()
    {
        SwitchState(previousState);
    }


    public void ButtonAction(CMButton button)
    {
        switch (button.iconType)
        {
            case IconTypes.LeftUpgrade:
                if (Builder.GetInstance().selectedTower.leftUpgradeLevel < 3)
                {
                    int upgradeLevel = Builder.GetInstance().IncreaseTowerLeftUpgradeLevel();
                    if (upgradeLevel == -1) 
                    {
                        break;
                    }
                    button.IncreaseUpgradeLevel(upgradeLevel);
                    if (Builder.GetInstance().selectedTower.leftUpgradeLevel == 3)
                        button.operationCost = "";
                }
                else
                {
                    button.operationCost = "";
                }
                SetupButtons(currentState);
                break;

            case IconTypes.RightUpgrade:
                if (Builder.GetInstance().selectedTower.rightUpgradeLevel < 3)
                {
                    int upgradeLevel = Builder.GetInstance().IncreaseTowerRightUpgradeLevel();
                    if (upgradeLevel == -1)
                    {
                        break;
                    }
                    button.IncreaseUpgradeLevel(upgradeLevel);
                    if (Builder.GetInstance().selectedTower.rightUpgradeLevel == 3)
                        button.operationCost = "";
                }
                else
                {
                    button.operationCost = "";
                }
                SetupButtons(currentState);
                break;

            case IconTypes.LeftLevel4Tower:
                if (Builder.GetInstance().EnoughResourcesForBuild(button.attachedBuildableKey))
                {
                    Builder.GetInstance().ClickedOnBuildable(button.attachedBuildableKey);
                    Builder.GetInstance().VisualizeNewBuildable();
                    SwitchState(CMStatesKeys.SolutionState);
                }
                break;

            case IconTypes.RightLevel4Tower:
                if (Builder.GetInstance().EnoughResourcesForBuild(button.attachedBuildableKey))
                {
                    Builder.GetInstance().ClickedOnBuildable(button.attachedBuildableKey);
                    Builder.GetInstance().VisualizeNewBuildable();
                    SwitchState(CMStatesKeys.SolutionState);
                }
                break;

            case IconTypes.Repair:
                Builder.GetInstance().RepairTower();
                break;

            case IconTypes.Reject:
                Builder.GetInstance().HideNewBuildable();
                GetBack();
                break;

            case IconTypes.DestroyCube:
                Builder.GetInstance().DestroyCube();
                SwitchState(CMStatesKeys.HiddenState);
                break;

            case IconTypes.DestroyObject:
                Builder.GetInstance().Destroy();
                SwitchState(CMStatesKeys.HiddenState);
                break;

            case IconTypes.IncreaseLevel:
                if (Builder.GetInstance().selectedTower.currentLevel < 3 && Builder.GetInstance().EnoughResourcesForBuild(button.attachedBuildableKey))
                {
                    Builder.GetInstance().IncreaseTowerLevel();
                    if (Builder.GetInstance().selectedTower.currentLevel == 3)
                    {
                        SwitchState(CMStatesKeys.BasicLevel3ControlState);
                    }

                    SetupButtons(currentState);
                }
                break;

            case IconTypes.Policy:
                Builder.GetInstance().ChangeTowerPolicy(EnumUtils.GetNext<TargetPolicy>(button.attachedPolicy));
                SetupButtons(currentState);
                break;

            case IconTypes.BuildableIcon:
                if (Builder.GetInstance().EnoughResourcesForBuild(button.attachedBuildableKey))
                {
                    Builder.GetInstance().ClickedOnBuildable(button.attachedBuildableKey);
                    Builder.GetInstance().VisualizeNewBuildable();
                    SwitchState(CMStatesKeys.SolutionState);
                }
                break;

            case IconTypes.Confirm:
                Builder.GetInstance().Build();
                if (previousState == states[CMStatesKeys.BasicBuyState])
                {
                    if (Builder.GetInstance().GetClickedBuildableKey() == BuildableTypes.Cube)
                    {
                        SwitchState(CMStatesKeys.HiddenState);
                    }
                    else
                    {
                        SwitchState(CMStatesKeys.BasicControlState);
                    }
                }
                if (previousState == states[CMStatesKeys.BasicLevel3ControlState])
                {
                    SwitchState(CMStatesKeys.Level4ControlState);
                }
                break;

            default:
                Debug.LogWarning("Unknown iconType");
                break;
        }

    }

    public void SetupButtons(BaseState state)
    {
        int basicIndex = 0;
        for (int row = 0; row < state.controlMatrix.Count; row++)
        {
            for (int column = 0; column < state.controlMatrix[0].innerList.Count; column++)
            {
                controlMatrixButtons[row].innerList[column].SetIconType(state.controlMatrix[row].innerList[column]);
                controlMatrixButtons[row].innerList[column].SetActiveMarkers(0);
                if (state.controlMatrix[row].innerList[column] != IconTypes.Hidden)
                {
                    controlMatrixButtons[row].innerList[column].ChangeIconVisibility(true);
                    BaseTower selectedTower = Builder.GetInstance().selectedTower;
                    controlMatrixButtons[row].innerList[column].textTemplate = null;
                    controlMatrixButtons[row].innerList[column].operationCost = "";
                    switch (state.controlMatrix[row].innerList[column])
                    {
                        case IconTypes.LeftUpgrade:
                            controlMatrixButtons[row].innerList[column].ChangeIcon(selectedTower.leftUpgradeIcon);
                            controlMatrixButtons[row].innerList[column].SetActiveMarkers(selectedTower.leftUpgradeLevel);
                            controlMatrixButtons[row].innerList[column].textTemplate = buttonTextTemplates[ButtonTextTemplates.LeftUpgrade];
                            if (selectedTower.leftUpgradeLevel < 3)
                            {
                                controlMatrixButtons[row].innerList[column].operationCost = "-" + selectedTower.leftUpgradeCost[selectedTower.leftUpgradeLevel];
                            }
                    break;

                        case IconTypes.RightUpgrade:
                            controlMatrixButtons[row].innerList[column].ChangeIcon(selectedTower.rightUpgradeIcon);
                            controlMatrixButtons[row].innerList[column].SetActiveMarkers(selectedTower.rightUpgradeLevel);
                            controlMatrixButtons[row].innerList[column].textTemplate = buttonTextTemplates[ButtonTextTemplates.RightUpgrade];
                            if (selectedTower.rightUpgradeLevel < 3)
                            {
                                controlMatrixButtons[row].innerList[column].operationCost = "-" + selectedTower.rightUpgradeCost[selectedTower.rightUpgradeLevel];
                            }
                            break;

                        case IconTypes.LeftLevel4Tower:
                            controlMatrixButtons[row].innerList[column].ChangeIcon(
                                Builder.GetInstance().GetBuildableInfo(selectedTower.leftLevel4Tower).buildable.selfIcon);
                            controlMatrixButtons[row].innerList[column].SetBuildableKey(selectedTower.leftLevel4Tower);
                            controlMatrixButtons[row].innerList[column].textTemplate = buttonTextTemplates[ButtonTextTemplates.Buildable];
                            controlMatrixButtons[row].innerList[column].operationCost = "-" + Builder.GetInstance().GetBuildableInfo(selectedTower.leftLevel4Tower).buildable.cost;
                            break;

                        case IconTypes.RightLevel4Tower:
                            controlMatrixButtons[row].innerList[column].ChangeIcon(
                                Builder.GetInstance().GetBuildableInfo(selectedTower.rightLevel4Tower).buildable.selfIcon);
                            controlMatrixButtons[row].innerList[column].SetBuildableKey(selectedTower.rightLevel4Tower);
                            controlMatrixButtons[row].innerList[column].textTemplate = buttonTextTemplates[ButtonTextTemplates.Buildable];
                            controlMatrixButtons[row].innerList[column].operationCost = "-" + Builder.GetInstance().GetBuildableInfo(selectedTower.rightLevel4Tower).buildable.cost;
                            break;

                        case IconTypes.Repair:
                            controlMatrixButtons[row].innerList[column].ChangeIcon(
                                iconsDB.database[IconsKeys.Repair]);
                            controlMatrixButtons[row].innerList[column].textTemplate = buttonTextTemplates[ButtonTextTemplates.TowerRepair];
                            controlMatrixButtons[row].innerList[column].operationCost = "-" + ((1 - (selectedTower.currentHealth / selectedTower.maxHealth)) * selectedTower.cost).ToString();
                            break;

                        case IconTypes.Policy:
                            if (Builder.GetInstance().selectedTower.usingTargetPolicy)
                            {
                                controlMatrixButtons[row].innerList[column].ChangeIcon(
                                iconsDB.GetPolicyIcon(selectedTower.policy));
                                controlMatrixButtons[row].innerList[column].textTemplate = buttonTextTemplates[ButtonTextTemplates.Policy];
                                controlMatrixButtons[row].innerList[column].SetAttachedPolicy(selectedTower.policy);
                            }
                            else
                            {
                                controlMatrixButtons[row].innerList[column].ChangeIconVisibility(false);
                                controlMatrixButtons[row].innerList[column].iconType = IconTypes.Hidden;
                            }
                            break;

                        case IconTypes.DestroyObject:
                            controlMatrixButtons[row].innerList[column].ChangeIcon(
                                iconsDB.database[IconsKeys.DestroyObject]);
                            controlMatrixButtons[row].innerList[column].textTemplate = buttonTextTemplates[ButtonTextTemplates.TowerDestroy];
                            controlMatrixButtons[row].innerList[column].operationCost = "+" + selectedTower.GetPlayerDestroyCashback();
                            break;

                        case IconTypes.IncreaseLevel:
                            controlMatrixButtons[row].innerList[column].ChangeIcon(
                                iconsDB.GetLevelIcon(selectedTower.currentLevel));
                            if (selectedTower.currentLevel < 3)
                            {
                                controlMatrixButtons[row].innerList[column].textTemplate = buttonTextTemplates[ButtonTextTemplates.LevelUp];
                                controlMatrixButtons[row].innerList[column].operationCost = "-" + selectedTower.nextLevelCost;
                                controlMatrixButtons[row].innerList[column].SetBuildableKey(selectedTower.nextTowerKey);
                            }
                            break;

                        case IconTypes.Confirm:
                            controlMatrixButtons[row].innerList[column].ChangeIcon(
                                iconsDB.database[IconsKeys.Confirm]);
                            break;

                        case IconTypes.Reject:
                            controlMatrixButtons[row].innerList[column].ChangeIcon(
                                iconsDB.database[IconsKeys.Reject]);
                            break;

                        case IconTypes.BuildableIcon:
                            Buildable buildable = LUbus.GetInstance().buildableDB.GetBasic(basicIndex);
                            Debug.Log(buildable.keyName);
                            controlMatrixButtons[row].innerList[column].ChangeIcon(buildable.selfIcon);
                            controlMatrixButtons[row].innerList[column].SetBuildableKey(buildable.keyName);
                            controlMatrixButtons[row].innerList[column].textTemplate = buttonTextTemplates[ButtonTextTemplates.Buildable];
                            controlMatrixButtons[row].innerList[column].operationCost = "-" + buildable.cost;
                            basicIndex++;
                            break;

                        case IconTypes.DestroyCube:
                            if (Builder.GetInstance().selectedCube.isDestroyable)
                            {
                                Buildable cube = Builder.GetInstance().GetBuildableInfo(BuildableTypes.Cube).buildable;
                                controlMatrixButtons[row].innerList[column].ChangeIcon(
                                    iconsDB.database[IconsKeys.DestroyCube]);
                                controlMatrixButtons[row].innerList[column].textTemplate = buttonTextTemplates[ButtonTextTemplates.CubeDestroy];
                                controlMatrixButtons[row].innerList[column].operationCost = "+" + (cube.cost * cube.cashbackPercent);
                            }
                            else
                            {
                                controlMatrixButtons[row].innerList[column].ChangeIconVisibility(false);
                                controlMatrixButtons[row].innerList[column].iconType = IconTypes.Hidden;
                            }
                            break;

                        default:
                            Debug.LogWarning("Incorrect IconType");
                            break;
                    }
                }
                else
                {
                    controlMatrixButtons[row].innerList[column].DeSelected();
                    controlMatrixButtons[row].innerList[column].ChangeIconVisibility(false);
                }
            }
        }
    }


    public void ShowButtonText(string text)
    {
        buttonTextPanel.SetActive(true);
        buttonText.text = text;
    }

    public void HideButtonText()
    {
        buttonTextPanel.SetActive(false);
    }

    public void EnableRaycastBlocker()
    {
        raycastBlocker.SetActive(true);
    }

    public void DisableRaycastBlocker()
    {
        raycastBlocker.SetActive(false);
    }

    public void SwitchSelectedToRight()
    {
        controlMatrixButtons[selectedButtonRow].innerList[selectedButtonColumn].DeSelected();
        selectedButtonColumn++;
        if (selectedButtonColumn == controlMatrixButtons[0].innerList.Count)
        {
            selectedButtonColumn = 0;
            selectedButtonRow++;
            if (selectedButtonRow == controlMatrixButtons.Count)
            {
                selectedButtonRow = 0;
            }
        }
        controlMatrixButtons[selectedButtonRow].innerList[selectedButtonColumn].Selected();
    }

    public void SwitchSelectedToLeft()
    {
        controlMatrixButtons[selectedButtonRow].innerList[selectedButtonColumn].DeSelected();
        selectedButtonColumn--;
        if (selectedButtonColumn == -1)
        {
            selectedButtonColumn = controlMatrixButtons[0].innerList.Count - 1;
            selectedButtonRow--;
            if (selectedButtonRow == -1)
            {
                selectedButtonRow = controlMatrixButtons.Count - 1;
            }
        }
        controlMatrixButtons[selectedButtonRow].innerList[selectedButtonColumn].Selected();
    }

    public void PressSelectedButton()
    {
        if (controlMatrixButtons[selectedButtonRow].innerList[selectedButtonColumn].iconType != IconTypes.Hidden)
        {
            controlMatrixButtons[selectedButtonRow].innerList[selectedButtonColumn].Call();
            controlMatrixButtons[selectedButtonRow].innerList[selectedButtonColumn].Selected();
        }
    }
}
