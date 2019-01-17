using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiting : MonoBehaviour {

    public static Waiting _Instance;
    
    void Awake()
    {
        if (_Instance == null)
            _Instance = this;

        else if (_Instance != this)
            Destroy(gameObject);
    }

}
