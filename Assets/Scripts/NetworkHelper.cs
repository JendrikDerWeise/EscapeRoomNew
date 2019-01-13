using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkHelper : NetworkBehaviour {
    [HideInInspector]
    public GameObject box;
    [HideInInspector]
    public GameObject panel;

    public GameObject gameController;

    [Command]
    public void CmdStartGame()
    {
        StartCoroutine("StartingWithWAitingtime");
    }

    IEnumerator StartingWithWAitingtime()
    {
        yield return new WaitForSeconds(10f);
        RpcClientStart();
    }

    [ClientRpc]
    void RpcClientStart() { 
        panel.GetComponent<TapToPlace>().enabled = false;
        box.GetComponent<TapToPlace>().enabled = false;
        panel.GetComponent<BoxCollider>().enabled = false;
        box.GetComponent<BoxCollider>().enabled = false;
        gameController.GetComponent<GameController>().box = box;
        gameController.GetComponent<GameController>().doorlock = panel;
        gameController.SetActive(true);
    }
}
