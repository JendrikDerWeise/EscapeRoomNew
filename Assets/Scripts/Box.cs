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


    public void SpawnNextPuzzle()
    {
        foreach (GameObject go in puzzles)
            if (!go.activeSelf)
            {
                go.SetActive(true);
                return;
            }
    }

    [ClientRpc]
    void RpcActivateNextPuzzle(GameObject go)
    {
        go.SetActive(true);
    }

    public void ShowNextNumber(int num)
    {
        //public Canvas canvas;
        //irgendeinem Objekt unter der Box die Nummer zuweisen
        numberText = numberText + num;
        number.text = numberText;
        
  //   int number = Random.Range(0, 10);
//    return number;
    }
}
