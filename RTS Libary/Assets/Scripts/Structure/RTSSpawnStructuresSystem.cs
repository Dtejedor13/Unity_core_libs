using System.Collections.Generic;
using UnityEngine;

public class RTSSpawnStructuresSystem : MonoBehaviour
{
    public static RTSSpawnStructuresSystem Instance { get { return _instance; } }
    private static RTSSpawnStructuresSystem _instance;
    
    [Header("List of building blueprints")]
    [SerializeField] List<GameObject> structurePrefabs = new List<GameObject>();
    [SerializeField] int buildingFactionId;

    private GameObject spawnedBluePrint;

    private void Awake()
    {
        _instance = this;
    }

    public void SpawnStructure(int index)
    {
        if (index < structurePrefabs.Count && spawnedBluePrint == null) {
            spawnedBluePrint = Instantiate(structurePrefabs[index]);
            spawnedBluePrint.transform.GetChild(0).GetComponent<RTSStructure>().FactionId = buildingFactionId + 1;
        }
    }

    public void ClearBluePrintReferance()
    {
        spawnedBluePrint = null;
    }
}
