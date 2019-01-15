using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Puzzle : NetworkBehaviour {
    
    public AudioClip openSound;
    public GameObject solved;
    public int puzzleID;

    private void PuzzleFail()
    {
        GameController.instance.PuzzleFail();
    }

    public void PuzzleSolved(int id)
    {
        solved.SetActive(true);
        GameController.instance.CmdStartNextPuzzle(id);
    }
}
