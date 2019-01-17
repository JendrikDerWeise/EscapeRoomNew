using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiverPinPad : InteractionReceiver
{
    private GameController gameController;
    private HoloToolkit.Unity.SharingWithUNET.PlayerController player;
    private MultiplayerPanel multiplayerPanel;

    void Start()
    {
        gameController = GameController.instance;
        player = HoloToolkit.Unity.SharingWithUNET.PlayerController._Instance;
        multiplayerPanel = MultiplayerPanel._Instance;
    }

    protected override void InputDown(GameObject obj, InputEventData eventData)
    {
        switch (obj.tag)
        {
            case "PinLockButton":
                //obj.GetComponent<ButtonAction>().OnMouseDown();
                string btnValue = obj.GetComponent<ButtonAction>().ButtonValue;
                player.PressPanelBtn(btnValue);
                break;

            default:
                break;
        }
    }
}
