using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FusePickUpNetwork : NetworkBehaviour {
    [SyncVar(hook = "OnTap")]
    public bool isactive;

    void Start()
    {
        isactive = true;
    }

    [Command]
    public void CmdPickUp()
    {
        isactive = false;
        print("pickupServer");
    }

    void OnTap(bool enable)
    {
        gameObject.SetActive(enable);
        if(isServer)
            print("pickupServer");

        if (isClient)
            print("pickupClient");

        if (isLocalPlayer)
            print("pickupLocal");
    }
}
