using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricWPuzzle : Puzzle {
    public ElectricWavePuzzle electricWavePuzzle;

	// Use this for initialization
	void Start () {
        electricWavePuzzle.onComplete += OnComplete;
        OpenElectricWaveBox();
        puzzleID = 4;
    }
	
    public void OnComplete()
    {
        PuzzleSolved(puzzleID);
    }

    private void OpenElectricWaveBox()
    {
        Box.instance.GetComponent<Animation>().Play("OpenElectricWave");
        Box.instance.GetComponent<AudioSource>().clip = openSound;
        Box.instance.GetComponent<AudioSource>().Play();
    }
}
