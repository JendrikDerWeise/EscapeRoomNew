using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkHelper : NetworkBehaviour {
    public static NetworkHelper _Instance;
    [HideInInspector]
    public GameObject box;
    [HideInInspector]
    public GameObject panel;
    public GameObject spawnMenu;
    public GameObject waitingThing;

    public GameObject gameController;
    [SyncVar(hook = "OnValueChange")]
    private int anchorsEstablished = 0;


    void Awake()
    {
        if (_Instance == null)
            _Instance = this;

        else if (_Instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        spawnMenu.SetActive(false);
    }

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
        if (panel == null)
            panel = MultiplayerPanel._Instance.gameObject;

        if (box == null)
            box = MultiplayerBox._Instance.gameObject;

        panel.GetComponent<TapToPlace>().enabled = false;
        box.GetComponent<TapToPlace>().enabled = false;
        panel.GetComponent<BoxCollider>().enabled = false;
        box.GetComponent<BoxCollider>().enabled = false;
        gameController.GetComponent<GameController>().box = box;
        gameController.GetComponent<GameController>().doorlock = panel;
        gameController.GetComponent<GameController>().CmdStartOnServer();
    }

    [Command]
    public void CmdSendClientReady()
    {
        anchorsEstablished++;  
    }

    public void OnValueChange(int value)
    {
        anchorsEstablished = value;
        print("anchors established nwhelper: " + anchorsEstablished);

        if(anchorsEstablished >= 1)
        {
            if (isServer)
                spawnMenu.SetActive(true);

            waitingThing.SetActive(false);
            //RpcHookReplacement();
        }
    }

    /// <summary>
    /// Weil der Hook auf dem Client nicht feuert, hier eine Funktion um das auszugleichen.
    /// UNET ist wirklich ätzend.
    /// </summary>
    void RpcHookReplacement()
    {
        if(!isServer)
            waitingThing.SetActive(false);
    }
}
