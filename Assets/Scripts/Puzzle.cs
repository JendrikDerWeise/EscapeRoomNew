using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Puzzle : NetworkBehaviour {
    
    public AudioClip openSound;
    public GameObject solved;

    private void PuzzleFail()
    {
        GameController.instance.PuzzleFail();
    }

    public void PuzzleSolved()
    {
        solved.SetActive(true);
        GameController.instance.CmdStartNextPuzzle();
    }
    
}
