using HoloToolkit.Unity.SharingWithUNET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MultiplayerPanel : NetworkBehaviour {

    public static MultiplayerPanel _Instance;
    private Transform sharedWorldAnchorTransform;


    private bool receivedAuthority;
    
    [SyncVar]
    public NetworkInstanceId nid;

    [SyncVar]
    private Vector3 localPosition;

    [SyncVar]
    private Quaternion localRotation;

    void Awake()
    {
        if (_Instance == null)
            _Instance = this;

        else if (_Instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        nid = netId;

        sharedWorldAnchorTransform = SharedCollection.Instance.gameObject.transform;
        transform.SetParent(sharedWorldAnchorTransform, false);
    }

    void Update()
    {
        if (receivedAuthority)
        {
            Vector3 objDir = transform.forward;
            Vector3 objPos = transform.position + objDir * .01f;

            localPosition = sharedWorldAnchorTransform.InverseTransformPoint(objPos);
            localRotation = transform.localRotation;
            CmdSetNewPos(localPosition, transform.localRotation);


        }
        else if (!receivedAuthority)
        {

            transform.localPosition = localPosition;
            transform.localRotation = localRotation;
            
        }
    }

    [Command]
    public void CmdSetAuth(NetworkInstanceId objectId, NetworkIdentity player)
    {
        var iObject = NetworkServer.FindLocalObject(objectId);
        var networkIdentity = iObject.GetComponent<NetworkIdentity>();
        var otherOwner = networkIdentity.clientAuthorityOwner;

        if (otherOwner == player.connectionToClient)
        {
            return;
        }
        else
        {
            if (otherOwner != null)
            {
                networkIdentity.RemoveClientAuthority(otherOwner);
            }
            networkIdentity.AssignClientAuthority(player.connectionToClient);
        }
    }

    [Command]
    public void CmdSetNewPos(Vector3 pos, Quaternion rot)
    {
        //!islocalplayer?
        localPosition = pos;
        localRotation = rot;
    }

    public override void OnStartAuthority()
    {
        receivedAuthority = true;
    }

    public override void OnStopAuthority()
    {
        receivedAuthority = false;
    }

    public void PressPanelBtn(string panelBtn)
    {
        GameObject pinCodeControl = PinCodeControl._Instance.gameObject;

        foreach (Transform btn in pinCodeControl.transform)
        {
            if (btn.tag.Equals("PinLockButton"))
            {
                if (btn.GetComponentInChildren<ButtonAction>().ButtonValue.Equals(panelBtn))
                    btn.GetComponentInChildren<ButtonAction>().OnMouseDown();
            }
        }
    }
}
