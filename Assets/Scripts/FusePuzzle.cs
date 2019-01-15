using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusePuzzle : Puzzle{
    
    public GameObject[] fuses;
    public GameObject fuseBox;
    public AudioClip openFuseBox;
    public AudioClip shootFuseSound;
    public float fuseSpeed;
    public GameObject fuseOverlay;
    public List<GameObject> fuseList;

    void Start () {
        StartCoroutine(OpenHoles());
        fuseOverlay.SetActive(true);
        puzzleID = 1;

        fuseList = new List<GameObject>();
        fuseList.Add(fuses[0]);
        fuseList.Add(fuses[1]);
        fuseList.Add(fuses[2]);
        fuseList.Add(fuses[3]);
    }
	
    IEnumerator OpenHoles()
    {
        Box.instance.GetComponent<Animation>().Play("OpenFuseHoles");
        Box.instance.GetComponent<AudioSource>().clip = openSound;
        Box.instance.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(2);
        StartCoroutine(ShootFuses());
    }

    IEnumerator ShootFuses()
    {
        foreach(GameObject fuse in fuses)
        {
            fuse.GetComponent<Rigidbody>().velocity = fuse.transform.forward * fuseSpeed;
            Box.instance.GetComponent<AudioSource>().clip = shootFuseSound;
            Box.instance.GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(0.1f);
            fuse.GetComponent<Rigidbody>().useGravity = true;
            fuse.GetComponent<MeshCollider>().enabled = true;
        }
        StartCoroutine(OpenFuseBox());
    }

    IEnumerator OpenFuseBox()
    {
        yield return new WaitForSeconds(3);
        Box.instance.GetComponent<Animation>().Play("OpenFuseBox");
        Box.instance.GetComponent<AudioSource>().clip = openFuseBox;
        Box.instance.GetComponent<AudioSource>().Play();
    }

    public void PowerOn()
    {
        PuzzleSolved(puzzleID);
        fuseOverlay.SetActive(false);
    }

    public void PickUpFuse(int index)
    {
        fuseList[index].SetActive(false);
    }

    public int GetIndexOfFuse(GameObject fuse)
    {
        return fuseList.IndexOf(fuse);
    }

}
