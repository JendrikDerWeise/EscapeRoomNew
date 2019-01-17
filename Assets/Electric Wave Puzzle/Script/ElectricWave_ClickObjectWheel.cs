﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricWave_ClickObjectWheel : MonoBehaviour
{
    public ElectricWavePuzzle ewp;
    //public Camera cam;

    public int clickIndex = 0;

    private int saveRotation = 0;
    private int currentRotation = 0;
    private float dragAngle = 0;
    private int lastSavePoint = 0;

    public AudioClip tickSound;
    private AudioSource audioSource;
    private int rotation = 1;

    private bool inputDown;

    private float time = 0.0f;
    public float interpolationPeriod = 0.2f;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnFocusEnter()
    {
        rotation *= -1;
    }

    public void OnMouseDown()
    {
        inputDown = true;
    }
    

    void Update()
    {
        time += Time.deltaTime;
        if (time >= interpolationPeriod)
        {
            time = time - interpolationPeriod;
            if (inputDown)
                MoveWave();
        }
    }

    public void MoveWave()//OnMouseDrag()
    {
        if (!ewp.gamePause)
        {
            /*Vector3 pos = cam.WorldToScreenPoint(transform.position);
            pos = Input.mousePosition - pos;
            int rotation = -(int)(((Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg) - dragAngle));
            */
            //currentRotation = (rotation / 18) + saveRotation;
            currentRotation += rotation;
            transform.localRotation = Quaternion.Euler(0, 0, currentRotation * 18);

            if (clickIndex == 1)
            {
                //if (currentRotation > lastSavePoint && currentRotation - lastSavePoint == 1)
                if(rotation == 1)
                {
                    ewp.AMP_UP();
                    PlayTickSound();
                }
                //if (currentRotation < lastSavePoint && lastSavePoint - currentRotation == 1)
                if (rotation == -1)
                {
                    ewp.AMP_DOWN();
                    PlayTickSound();
                }
            }
            if (clickIndex == 2)
            {
                //if (currentRotation > lastSavePoint && currentRotation - lastSavePoint == 1)
                if (rotation == 1)
                {
                    ewp.FRE_UP();
                    PlayTickSound();
                }
                //if (currentRotation < lastSavePoint && lastSavePoint - currentRotation == 1)
                if (rotation == -1)
                {
                    ewp.FRE_DOWN();
                    PlayTickSound();
                }
            }

            lastSavePoint = currentRotation;
            
        }
    }

    private void PlayTickSound ()
    {
        audioSource.PlayOneShot(tickSound);
    }

    public void OnMouseUp()
    {
        inputDown = false;
        /*if (!ewp.gamePause)
        {
            saveRotation = currentRotation;
            transform.localRotation = Quaternion.Euler(0, 0, currentRotation * 18);
        }*/
    }
}
