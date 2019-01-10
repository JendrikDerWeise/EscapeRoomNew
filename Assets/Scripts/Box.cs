using HoloToolkit.Sharing.SyncModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour {
    public static Box instance;
    public GameObject[] puzzles;
    public Text number;
    [SyncData]
    private SyncString numberText;

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


    public void SpawnNextPuzzle()
    {
        foreach (GameObject go in puzzles)
            if (!go.activeSelf)
            {
                go.SetActive(true);
                return;
            }
    }

    public void ShowNextNumber(int num)
    {
        //public Canvas canvas;
        //irgendeinem Objekt unter der Box die Nummer zuweisen
        numberText.Value = numberText.Value + num;
        number.text = numberText.Value;
    }
}
