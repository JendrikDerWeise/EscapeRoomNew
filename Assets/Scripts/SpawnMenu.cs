using HoloToolkit.Unity.SharingWithUNET;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnMenu : NetworkBehaviour {
    /// <summary>
    /// The transform of the shared world anchor.
    /// </summary>
    public Transform sharedWorldAnchorTransform;
    private bool colorChanged;

    private GameObject spawnedBox;
    private GameObject spawnedPanel;

    public bool AnchorEstablished;
    public TextMesh text;
    public GameObject background;
    public GameObject boxPrefab;
    public GameObject panelPrefab;
    public NetworkHelper networkHelper;
	
    void Start()
    {
        sharedWorldAnchorTransform = SharedCollection.Instance.gameObject.transform;
        transform.SetParent(sharedWorldAnchorTransform);
    }

	
    public void SpawnBox()
    {
        CmdSpawnObject(boxPrefab);
    }

    public void SpawnPanel()
    {
        CmdSpawnObject(panelPrefab);
    }

    public void MoveBox()
    {
        TakeAuthBox(spawnedBox);
        spawnedBox.GetComponent<TapToPlace>().enabled = true;
    }

    public void MovePanel()
    {
        TakeAuthPanel(spawnedPanel);
        spawnedPanel.GetComponent<TapToPlace>().enabled = true;
        networkHelper.CmdStartGame();
        gameObject.SetActive(false);  
    }

    /// <summary>
    /// This will 'spawn' the box on all clients, including the 
    /// client on the host.
    /// </summary>
    [Command]
    void CmdSpawnObject(GameObject prefab)
    {
        HoloToolkit.Unity.SharingWithUNET.PlayerController player = HoloToolkit.Unity.SharingWithUNET.PlayerController._Instance;
        Vector3 playerPos = Camera.main.transform.position;
        Vector3 playerDirection = Camera.main.transform.forward;
        Quaternion playerRotation = Camera.main.transform.rotation;
        float spawnDistance = 1.8f;

        Vector3 spawnPos = playerPos + playerDirection * spawnDistance;

        if (prefab.name.Equals(boxPrefab.name))
        {
            // The box needs to be transformed relative to the shared anchor.
            spawnedBox = (GameObject)Instantiate(prefab, sharedWorldAnchorTransform.InverseTransformPoint(spawnPos), Quaternion.Euler(playerDirection));
            NetworkServer.SpawnWithClientAuthority(spawnedBox, player.connectionToClient);
            networkHelper.box = spawnedBox;
            RpcSyncBox(spawnedBox);
        }

        else if (prefab.name.Equals(panelPrefab.name))
        {
            // The box needs to be transformed relative to the shared anchor.
            spawnedPanel = (GameObject)Instantiate(prefab, sharedWorldAnchorTransform.InverseTransformPoint(spawnPos), Quaternion.Euler(playerDirection));
            NetworkServer.SpawnWithClientAuthority(spawnedPanel, player.connectionToClient);
            networkHelper.panel = spawnedPanel;
            RpcSyncPanel(spawnedPanel);
        }
    }

    [ClientRpc]
    void RpcSyncBox(GameObject go)
    {
        spawnedBox = go;
        networkHelper.box = spawnedBox;
    }

    [ClientRpc]
    void RpcSyncPanel(GameObject go)
    {
        spawnedPanel = go;
        networkHelper.panel = spawnedPanel;
    }

    private void TakeAuthBox(GameObject go)
    {
        HoloToolkit.Unity.SharingWithUNET.PlayerController player = HoloToolkit.Unity.SharingWithUNET.PlayerController._Instance;
        NetworkIdentity playerID = player.GetComponent<NetworkIdentity>();
        go.GetComponent<MultiplayerBox>().CmdSetAuth(netId, playerID);
    }

    private void TakeAuthPanel(GameObject go)
    {
        HoloToolkit.Unity.SharingWithUNET.PlayerController player = HoloToolkit.Unity.SharingWithUNET.PlayerController._Instance;
        NetworkIdentity playerID = player.GetComponent<NetworkIdentity>();
        go.GetComponent<MultiplayerPanel>().CmdSetAuth(netId, playerID);
    }
}
