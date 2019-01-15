using System;
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
    [SerializeField] public GameObject[] solvedPuzzles = new GameObject[0];
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
    private MultiplayerBox multiplayerBox;
    public int ownGeneratedID;
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
        multiplayerBox = MultiplayerBox._Instance;
    }

    public override void OnStartClient()
    {
        print("clientonstart");
        multiplayerBox = MultiplayerBox._Instance;
        ownGeneratedID = UnityEngine.Random.Range(0, 10000);
    }

    // Use this for initialization
    void Start () {
        
    }

    [ClientRpc]
    public void RpcStart()
    {
        sc = GetComponent<SoundController>();
        //currentTime = startingTime;
        bStart = win = pause = false;

        keywords.Add("Next", () => {
            CmdStartNextPuzzle(3); //PaperPuzzle
            print("Er hatr next gesagt!");
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
        RpcStart();
        gameStarted = true;
        RpcStartNextPuzzle(0);
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
    public void CmdStartNextPuzzle(int puzzleID)
    {
        RpcStartNextPuzzle(puzzleID);
    }    

    [ClientRpc]
    public void RpcStartNextPuzzle(int puzzleID)//hiermit starten
    {
        if (!win)//if (!CheckWinCondition())
        {
            sc.PlayClip(sc.raise);
            box.GetComponentInChildren<Box>().SpawnNextPuzzle(puzzleID);
            ShowNumber(puzzleID);
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
        string solutionString = "";
        solution = new int[5];
        for (int i = 0; i < solution.Length; i++)
        {
            solution[i] = UnityEngine.Random.Range(0, 9);
            solutionString += solution[i];
        }
        
        RpcSendSolutionToClients(solution, solutionString);
    }

    [ClientRpc]
    void RpcSendSolutionToClients(int[] sol, string solutionString)
    {
        solution = sol;
        doorlock.GetComponentInChildren<PinCodeControl>().SetCode(solutionString);
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

    private void ShowNumber(int nextNumber)
    {
        if (nextNumber == 0)
            return;

        string visibleCode = "";

        for (int i = 0; i < nextNumber; i++)
            visibleCode += "" + solution[i];

        box.GetComponentInChildren<Box>().ShowNextNumber(visibleCode);
        /*
        foreach (GameObject go in solvedPuzzles)
        {
            if (go.activeSelf)
            {
                int nextNumber = Array.FindIndex(solvedPuzzles, x => x.Equals(go));
                box.GetComponentInChildren<Box>().ShowNextNumber(solution[nextNumber]);
                //box.GetComponentInChildren<Box>().ShowNextNumber(UnityEngine.Random.Range(0, 10));
            }
            //box.GetComponentInChildren<Box>().ShowNextNumber(UnityEngine.Random.Range(0, 10));
        }*/
    }

    public void PuzzleFail()
    {
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
    public void CmdPutFuseInFusebox(int senderID)
    {
        RpcPutFuseInFuseBox(senderID);
    }

    [ClientRpc]
    void RpcPutFuseInFuseBox(int senderID)
    {
        bool placeFuseFromInventory = false;

        if (senderID == ownGeneratedID)
            placeFuseFromInventory = true;

        print("senderID: " + senderID + " ownID: " + ownGeneratedID);
        GameObject.FindGameObjectWithTag("FuseBox").GetComponent<FuseController>().CheckFuseBox(placeFuseFromInventory);
    }

    [Command]
    public void CmdDisableObject(int index)
    { 
        if (multiplayerBox == null)
            multiplayerBox = MultiplayerBox._Instance;
        multiplayerBox.RpcPickUpFuse(index);
        multiplayerBox.CmdPickUpFuse(index);
    }
    
    [ClientRpc]
    public void RpcSetButtonPowerOnSwitchPuzzle(int[] array)
    {
        Electric1 sp = multiplayerBox.switchPuzzle.GetComponent<SwitchPuzzle>().switchPuzzle;
        sp.buttonPower = array;
    }

    [Command]
    public void CmdSwitchTheSwitchBox(int num)
    {
        RpcSwitchTheSwitchBox(num);
    }

    [ClientRpc]
    void RpcSwitchTheSwitchBox(int num)
    {
        if (multiplayerBox == null)
            multiplayerBox = MultiplayerBox._Instance;
        multiplayerBox.GetComponentInChildren<Electric1>().ClickButton(num);
    }

    [Command]
    public void CmdOnWaveBtnDown(int num)
    {
        RpcOnWaveBtnDown(num);
    }

    [ClientRpc]
    void RpcOnWaveBtnDown(int num)
    {
        if (multiplayerBox == null)
            multiplayerBox = MultiplayerBox._Instance;
        multiplayerBox.GetComponentInChildren<ElectricWavePuzzle>().btns[num - 1].GetComponent<ElectricWave_ClickObjectWheel>().OnMouseDown();
    }

    [Command]
    public void CmdOnWaveBtnUp(int num)
    {
        RpcOnWaveBtnUp(num);
    }

    [ClientRpc]
    void RpcOnWaveBtnUp(int num)
    {
        if (multiplayerBox == null)
            multiplayerBox = MultiplayerBox._Instance;
        multiplayerBox.GetComponentInChildren<ElectricWavePuzzle>().btns[num - 1].GetComponent<ElectricWave_ClickObjectWheel>().OnMouseUp();
    }

    [Command]
    public void CmdOnFocusEnter(int num)
    {
        RpcOnFocusEnter(num);
    }

    [ClientRpc]
    void RpcOnFocusEnter(int num)
    {
        if (multiplayerBox == null)
            multiplayerBox = MultiplayerBox._Instance;
        multiplayerBox.GetComponentInChildren<ElectricWavePuzzle>().btns[num - 1].GetComponent<ElectricWave_ClickObjectWheel>().OnFocusEnter();
    }

    [Command]
    public void CmdPushBtnPuzzle(int num)
    {
        RpcPushBtnPuzzle(num);
    }

    [ClientRpc]
    void RpcPushBtnPuzzle(int num)
    {
        if (multiplayerBox == null)
            multiplayerBox = MultiplayerBox._Instance;
        multiplayerBox.GetComponentInChildren<ButtonPuzzle>().ClickButton(num);
    }

    [Command]
    public void CmdPushResetBtn()
    {
        RpcPushResetBtn();
    }

    [ClientRpc]
    void RpcPushResetBtn()
    {
        if (multiplayerBox == null)
            multiplayerBox = MultiplayerBox._Instance;
        multiplayerBox.GetComponentInChildren<ButtonPuzzle>().Setup();
        PuzzleFail();
    }

    [Command]
    public void CmdSetWaves(float m1_point1, float m1_point2, float m2_point1, float m2_point2)
    {
        RpcSetWaves(m1_point1, m1_point2, m2_point1, m2_point2);
    }

    [ClientRpc]
    void RpcSetWaves(float m1_point1, float m1_point2, float m2_point1, float m2_point2)
    {
        ElectricWavePuzzle wp = multiplayerBox.wavePuzzle.GetComponent<ElectricWavePuzzle>();
        wp.m1_point1 = m1_point1;
        wp.m1_point2 = m1_point2;
        wp.m2_point1 = m2_point1;
        wp.m2_point2 = m2_point2;
    }
}
