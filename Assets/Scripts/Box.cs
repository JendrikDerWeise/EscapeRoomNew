using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Box : NetworkBehaviour {
    public static Box instance;
    public GameObject[] puzzles;
    public Text number;
    private string numberText;

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


    public void SpawnNextPuzzle(int puzzleID)
    {
        if(puzzleID == puzzles.Length)
            return;
        
        puzzles[puzzleID].SetActive(true);
            
    }

    public void ShowNextNumber(string num)
    {
        //public Canvas canvas;
        //irgendeinem Objekt unter der Box die Nummer zuweisen
        numberText = num; //numberText + num;
        number.text = numberText;

        print("shownextnumber");
  //   int number = Random.Range(0, 10);
//    return number;
    }
}
