using HoloToolkit.Sharing.Spawning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles spawning sync objects on call by ObjectPlacer.
/// </summary>
public class MySyncObjectSpawner : MonoBehaviour {

    [SerializeField]
    private MyPrefabSpawnManager spawnManager = null;

    [SerializeField]
    [Tooltip("Optional transform target, for when you want to spawn the object on a specific parent.  If this value is not set, then the spawned objects will be spawned on this game object.")]
    private Transform spawnParentTransform;

    private void Awake()
    {
        if (spawnManager == null)
        {
            Debug.LogError("You need to reference the spawn manager on SyncObjectSpawner.");
        }

        // If we don't have a spawn parent transform, then spawn the object on this transform.
        if (spawnParentTransform == null)
        {
            spawnParentTransform = transform;
        }
    }

    //METHODEN ZUM BOX UND PANEL SPAWNING

    public GameObject SpawnBox(Vector3 position, Quaternion rotation)
    {
        var spawnedObject = new SyncSpawnedObject();

        GameObject spawned = spawnManager.Spawn(spawnedObject, position, rotation, spawnParentTransform.gameObject, "SpawnedObject", false);

        return spawned;
    }

}
