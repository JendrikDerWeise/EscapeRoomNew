﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour {
    public static GameController instance;
    [SyncVar]
    public bool gameStarted;
    int round = 0;
    #region GameObjects to control
    [SerializeField] public GameObject doorlock;
    [SerializeField] public GameObject box;
    [SerializeField] public GameObject[] solvedPuzzles;
    #endregion

    #region Time configuration
    [SerializeField] public float startingTime = 900f;
    [SerializeField] public float timeToDecrease = 600f;
    #endregion

    #region private fields
    [HideInInspector]
    [SerializeField] private int[] solution;

    [SyncVar]
    private float currentTime = 0f;
    private int tmpSec = 999;
    private bool bStart;
    [SyncVar]
    private bool win;
    private SoundController sc;
    private bool pause;
    #endregion

    KeywordRecognizer keywordRecognizer;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartServer()
    {
        print("serveronstart");
        MakeSolution();
        currentTime = startingTime;
    }

    public override void OnStartClient()
    {
        print("clientonstart");
    }

    // Use this for initialization
    void Start () {
        if (isServer)
        {
            //MakeSolution(); SYNCHRONISIEREN!!!!!!!!!!!
           
            print("server");
        }

        if (isLocalPlayer)
            print("localPlayer");

        if (isClient)
            print("cvlient");
    }

    [ClientRpc]
    public void RpcStart()
    {
        
        //VoiceOver Ansage 1
        //SpawnBox();
        //Warten auf Box
        //SpawnLock();
        //Warten auf Lock
        //VoiceOver Ansage 2

        //StartPauseTimer();
        sc = GetComponent<SoundController>();
        //currentTime = startingTime;
        bStart = win = pause = false;

        keywords.Add("Next", () => {
            CmdStartNextPuzzle(); //&&solvedPuzzles.Length == 2
        });

        keywords.Add("Pause", () => {
            if (!pause)
            {
                StartPauseTimer();
                pause = !pause;
            }
        });

        keywords.Add("Anti Pause", () => {
            if (pause)
            {
                StartPauseTimer();
                pause = !pause;
            }
        });

        keywords.Add("Move box", () => {
            box.GetComponentInParent<TapToPlace>().enabled = true;
            box.GetComponentInParent<BoxCollider>().enabled = true;
        });

        keywords.Add("Stop moving box", () => {
            box.GetComponentInParent<TapToPlace>().enabled = false;
            box.GetComponentInParent<BoxCollider>().enabled = false;
        });

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordReconizeOnPhraseReconized;
        keywordRecognizer.Start();

        
        StartPauseTimer();
    }

    [Command]
    public void CmdStartOnServer()
    {
        gameStarted = true;
        RpcStartNextPuzzle();
    }

    void KeywordReconizeOnPhraseReconized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;

        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

    // Update is called once per frame
    void Update () {
        if (!gameStarted)
            return;
        
        if(isServer)
            win = CheckWinCondition();

        CheckTimer();

	}

    private string getTime()
    {
        float min = currentTime / 60;
        int tmp = (int)min;
        int sec = (int)((currentTime) % 60);
        if (sec != tmpSec && bStart)
        {
            doorlock.GetComponentInChildren<PinCodeControl>().PlaySecBeep();
            tmpSec = sec;
        }
        if (sec > 9)
        {
            return tmp + "min " + sec + "s";
        }
        else
        {
            return tmp + "min 0" + sec + "s";
        }
    }

    public void StartPauseTimer()
    {
        doorlock.GetComponentInChildren<PinCodeControl>().UpdateText(getTime());
        bStart = !bStart;
        sc.PlayClip(sc.start);
    }

    private void GameOver()
    {
        //TODO GameOver Screen, Optionen zum Neustart
    }


    [Command]
    public void CmdStartNextPuzzle()
    {
        RpcStartNextPuzzle();
        //box.GetComponentInChildren<Box>().SpawnNextPuzzle();
    }  

    [ClientRpc]
    public void RpcStartNextPuzzle()//hiermit starten
    {
        if (!win)//if (!CheckWinCondition())
        {
            sc.PlayClip(sc.raise);
            box.GetComponentInChildren<Box>().SpawnNextPuzzle();
            ShowNumber();
            /*
            if (round > 0)
            {
                box.GetComponentInChildren<Box>().ShowNextNumber(UnityEngine.Random.Range(0, 10));

            }
            round++;*/
        }
        else
            sc.PlayClip(sc.win);//WinScreen
    }

    

    private void DecreaseTimer()
    {
        CmdDecreaseTimer();
        sc.PlayClip(sc.failure);
    }

    [Command]
    void CmdDecreaseTimer()
    {
        currentTime -= timeToDecrease;
    }

    private void MakeSolution()
    {
        solution = new int[solvedPuzzles.Length];
        for(int i = 0; i < solution.Length; i++) 
            solution[i] = UnityEngine.Random.Range(0, 9);
    }

    [ClientRpc]
    void RpcSendSolutionToClients(int[] sol)
    {
        solution = sol;
    }

    private void CheckTimer()
    {
        if (bStart && !win)
        {
            if (currentTime >= 0)
            {
                currentTime -= 1 * Time.deltaTime;
                doorlock.GetComponentInChildren<PinCodeControl>().UpdateText(getTime());
            }
            else
            {
                doorlock.GetComponentInChildren<PinCodeControl>().UpdateText("Game Over");
                GameOver();
                sc.PlayClip(sc.gameOver);
            }
        }
    }

    private void ShowNumber()
    {
        foreach(GameObject go in solvedPuzzles)
        {
            if (go.activeSelf)
            {
                int nextNumber = Array.FindIndex(solvedPuzzles, x => x.Equals(go));
                box.GetComponentInChildren<Box>().ShowNextNumber(solution[nextNumber]);
                //box.GetComponentInChildren<Box>().ShowNextNumber(UnityEngine.Random.Range(0, 10));
            }
            //box.GetComponentInChildren<Box>().ShowNextNumber(UnityEngine.Random.Range(0, 10));
        }
    }

    public void PuzzleFail()
    {
        //Optisch verdeutlichen?
        sc.PlayClip(sc.failure);
        DecreaseTimer();
    }
    
    private bool CheckWinCondition()
    {
        int solved = 0;
        foreach (GameObject go in solvedPuzzles)
            if (go.activeSelf)
                solved++;

        return solved == box.GetComponentInChildren<Box>().puzzles.Length;
    }

    /// <summary>
    /// Receiver Callbacks, weil Network IDs bei ChildObjekten nicht funktionieren...
    /// ...oder ich zu blöd bin...
    /// </summary>

    [Command]
    public void CmdPutFuseInFusebox()
    {
        //GameObject.FindGameObjectWithTag("FuseBox").GetComponent<FuseController>().CheckFuseBox();
        RpcPutFuseInFuseBox();
    }

    [ClientRpc]
    void RpcPutFuseInFuseBox()
    {
        GameObject.FindGameObjectWithTag("FuseBox").GetComponent<FuseController>().CheckFuseBox();
    }

    [Command]
    public void CmdDisableObject(GameObject obj)
    {
        NetworkIdentity objNetId = box.GetComponent<NetworkIdentity>();
        CmdSetAuth(box.GetComponent<NetworkIdentity>().netId, HoloToolkit.Unity.SharingWithUNET.PlayerController._Instance.GetComponent<NetworkIdentity>());
        //objNetId.AssignClientAuthority(connectionToClient);
        RpcDisableObject(obj);
        obj.SetActive(false);
        //obj.GetComponent<FusePickUpNetwork>().isactive = false;
        objNetId.RemoveClientAuthority(connectionToClient);
    }

    [ClientRpc]
    void RpcDisableObject(GameObject obj)
    {

        if (obj != null)
            obj.SetActive(false);
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
}
