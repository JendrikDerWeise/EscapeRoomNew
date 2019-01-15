﻿using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Receiver : InteractionReceiver
{
    public InventoryController_Fuse fuseInv;
    private GameController gameController;
    private HoloToolkit.Unity.SharingWithUNET.PlayerController player;
    private MultiplayerBox multiplayerBox;

    void Start()
    {
        gameController = GameController.instance;
        player = HoloToolkit.Unity.SharingWithUNET.PlayerController._Instance;
        multiplayerBox = MultiplayerBox._Instance;
    }

    protected override void InputDown(GameObject obj, InputEventData eventData)
    {
        switch (obj.tag)
        {
            case "Fuse":
                fuseInv.UpdateFuseUI();
                AudioManager_Fuse.instance.Play("PickupSFX");
                
                player.PickUpFuse(multiplayerBox.GetIndexOfClickedFuse(obj));
                break;

            case "FuseBox":
                //GameObject.FindGameObjectWithTag("FuseBox").GetComponent<FuseController>().CheckFuseBox();
                if (fuseInv.inventoryFuses <= 0)
                {
                    player.PuzzleFail();
                    return;
                }

                player.PutFuseIntoFuseBox();
                break;

            case "Switch":
                //obj.GetComponent<ClickButtonObject_Electric1>().OnMouseDown();
                int num = obj.GetComponent<ClickButtonObject_Electric1>().clickIndex;
                player.SwitchingSwitchButton(num);
                break;

            case "ButtonBoard":
                //obj.GetComponent<ClickButtonObject>().OnMouseDown();
                int numBtn = obj.GetComponent<ClickButtonObject>().clickIndex;
                player.PushBtnPuzzle(numBtn);
                break;

            case "ButtonBoardReset":
                //obj.GetComponent<ResetButton>().OnMouseDown();
                player.PushResetBtn();
                break;

            case "ElectricWave":
                //obj.GetComponent<ElectricWave_ClickObjectWheel>().OnMouseDown();
                int waveBtn = GetClickIndexFromWaveBtn(obj);
                player.OnWaveBtnDown(waveBtn);
                break;


            default:
                break;
        }
    }

    protected override void InputUp(GameObject obj, InputEventData eventData)
    {
        switch (obj.tag)
        {
            case "ElectricWave":
                //obj.GetComponent<ElectricWave_ClickObjectWheel>().OnMouseUp();
                int waveBtn = GetClickIndexFromWaveBtn(obj);
                player.OnWaveBtnUp(waveBtn);
                break;

            default:
                break;
        }
    }

    protected override void FocusEnter(GameObject obj, PointerSpecificEventData eventData)
    {
        switch (obj.tag)
        {
            case "ElectricWave":
                //obj.GetComponent<ElectricWave_ClickObjectWheel>().OnFocusEnter();
                int waveBtn = GetClickIndexFromWaveBtn(obj);
                player.OnFocusEnter(waveBtn);
                break;

            default:
                break;
        }
    }

    int GetClickIndexFromWaveBtn(GameObject obj)
    {
        return obj.GetComponent<ElectricWave_ClickObjectWheel>().clickIndex;
    }
}
