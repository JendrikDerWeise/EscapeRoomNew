using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPuzzle : Puzzle {
    public Electric1 switchPuzzle;

	// Use this for initialization
	void Start () {
        switchPuzzle.onComplete += OnComplete;
        switchPuzzle.onFail += OnFail;
        OpenSwitches();
        puzzleID = 2;
    }
    
    private void OpenSwitches()
    {
        Box.instance.GetComponent<Animation>().Play("OpenSwitches");
        Box.instance.GetComponent<AudioSource>().clip = openSound;
        Box.instance.GetComponent<AudioSource>().Play();
    }

    private void OnComplete()
    {
        PuzzleSolved(puzzleID);
    }

    private void OnFail()
    {
        GameController.instance.PuzzleFail();
    }
}
