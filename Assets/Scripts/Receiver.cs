using HoloToolkit.Unity.InputModule;
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

    void Start()
    {
        gameController = GameController.instance;
        player = HoloToolkit.Unity.SharingWithUNET.PlayerController._Instance;
    }

    protected override void InputDown(GameObject obj, InputEventData eventData)
    {
        switch (obj.tag)
        {
            case "Fuse":
                fuseInv.UpdateFuseUI();
                AudioManager_Fuse.instance.Play("PickupSFX");
                //obj.SetActive(false);
                //NetworkIdentity objNetId = gameController.GetComponent<NetworkIdentity>();        // get the object's network ID
                //objNetId.AssignClientAuthority(player.connectionToClient);
                //gameController.CmdDisableObject(obj);
                //objNetId.RemoveClientAuthority(player.connectionToClient);
                //obj.GetComponent<FusePickUpNetwork>().CmdPickUp();
                player.PickUpFuse(obj);
                break;

            case "FuseBox":
                //GameObject.FindGameObjectWithTag("FuseBox").GetComponent<FuseController>().CheckFuseBox();
                gameController.CmdPutFuseInFusebox();
                break;

            case "Switch":
                obj.GetComponent<ClickButtonObject_Electric1>().OnMouseDown();
                break;

            case "ButtonBoard":
                obj.GetComponent<ClickButtonObject>().OnMouseDown();
                break;

            case "ButtonBoardReset":
                obj.GetComponent<ResetButton>().OnMouseDown();
                break;

            case "ElectricWave":
                obj.GetComponent<ElectricWave_ClickObjectWheel>().OnMouseDown();
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
                obj.GetComponent<ElectricWave_ClickObjectWheel>().OnMouseUp();
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
                obj.GetComponent<ElectricWave_ClickObjectWheel>().OnFocusEnter();
                break;

            default:
                break;
        }
    }
}
