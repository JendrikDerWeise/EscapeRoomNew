using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSinglePlayerClicked : MonoBehaviour, IInputClickHandler
{

    public void OnInputClicked(InputClickedEventData eventData)
    {
        NetworkHelper._Instance.CmdSendClientReady();
        eventData.Use();
    }
}
