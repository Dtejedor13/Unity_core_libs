using System;
using System.Collections.Generic;
using UnityEngine;

public class RTSUnitSelections : MonoBehaviour
{
    public List<GameObject> UnitList = new List<GameObject>();
    public List<GameObject> UnitSelected = new List<GameObject>();

    private static RTSUnitSelections _instance;
    public static RTSUnitSelections Instance { get { return _instance; } }

    private int playerFactionId;
    
    private void Awake()
    {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }
    }

    private void Start()
    {
        playerFactionId = RTSGameManager.Instance.PlayerFactionId;
    }

    private void Update()
    {
        // Declare movment destination to selected objects
        if (Input.GetMouseButtonDown(1) && UnitSelected.Count > 0)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, RTSGameManager.Instance.Ground))
            {
                if (UnitSelected.Count > 1)
                {
                    // ToDo: group movment
                    List<Vector3> targetPositionList = GetPositionListAround(hit.point, new float[] { 2f, 4f, 5f, 6f, 10f }, new int[] { 5, 10, 20, 30, 40 });

                    int targetPositionListIndex = 0;
                    foreach(GameObject go in UnitSelected) {
                        go.GetComponent<RTSUnitController>().SetMoveDestination(targetPositionList[targetPositionListIndex]);
                        targetPositionListIndex = (targetPositionListIndex + 1) % targetPositionList.Count;
                    }
                }
                else
                {
                    foreach (GameObject go in UnitSelected)
                        go.GetComponent<RTSUnitController>().SetMoveDestination(hit.point);
                }
            }               
        }
    }

    public void ClickSelect(GameObject clickedObject)
    {
        // select unit or inspect enemy or set attack target
        RTSUnitController unitController;
        clickedObject.TryGetComponent(out unitController);

        if (unitController != null && clickedObject.GetComponent<RTSCombat>().factionID != playerFactionId) {
            // enemy clicked
            if (UnitSelected.Count == 0) {
                Debug.Log("inspection mode");
            }
        }

        // is friendly unit
        if (unitController != null && clickedObject.GetComponent<RTSCombat>().factionID == playerFactionId) {
            DeselectAll();
            UnitSelected.Add(clickedObject);
            unitController.IsSelectedMarker.gameObject.SetActive(true);
            unitController.EnableMovement = true;
        }

    }

    public void ShiftClickSelect(GameObject clickedObject)
    {
        RTSUnitController unitController;
        clickedObject.TryGetComponent(out unitController);

        if (unitController != null && clickedObject.GetComponent<RTSCombat>().factionID == playerFactionId && !UnitSelected.Contains(clickedObject))
        {
            UnitSelected.Add(clickedObject);
            unitController.IsSelectedMarker.gameObject.SetActive(true);
            unitController.EnableMovement = true;
        }
        else if (unitController != null && clickedObject.GetComponent<RTSCombat>().factionID == playerFactionId && UnitSelected.Contains(clickedObject))
        {
            unitController.IsSelectedMarker.gameObject.SetActive(false);
            unitController.EnableMovement = false;
            UnitSelected.Remove(clickedObject);
        }
    }

    public void DragSelect(GameObject unitToAdd)
    {
        RTSUnitController unitController;
        unitToAdd.TryGetComponent(out unitController);

        if (unitController != null && unitToAdd.GetComponent<RTSCombat>().factionID == playerFactionId && !UnitSelected.Contains(unitToAdd))
        {
            UnitSelected.Add(unitToAdd);
            unitToAdd.GetComponent<RTSUnitController>().IsSelectedMarker.gameObject.SetActive(true);
            unitToAdd.GetComponent<RTSUnitController>().EnableMovement = true;
        }
    }

    public void DeselectAll()
    {
        foreach (var unit in UnitList) {
            unit.GetComponent<RTSUnitController>().IsSelectedMarker.gameObject.SetActive(false);
            unit.GetComponent<RTSUnitController>().EnableMovement = false;
        }

        UnitSelected.Clear();
    }

    public void Deselect(GameObject unitToDeSelect)
    {
        UnitSelected.Remove(unitToDeSelect);
    }

    private List<Vector3> GetPositionListAround(Vector3 startPosition, float[] ringDistanceArray, int[] ringPositionCountArray)
    {
        List<Vector3> positionList = new List<Vector3>();
        positionList.Add(startPosition);
        for (int i = 0; i < ringDistanceArray.Length; i++)
        {
            positionList.AddRange(GetPositionListAround(startPosition, ringDistanceArray[i], ringPositionCountArray[i]));
        }
        return positionList;
    }

    private List<Vector3> GetPositionListAround(Vector3 startPosition, float distance, int positionCount)
    {
        List<Vector3> positionList = new List<Vector3>();
        for (int i = 0; i < positionCount; i++)
        {
            float angle = i * (360f / positionCount);
            Vector3 dir = ApplayRotationToVector(new Vector3(1, 0), angle);
            Vector3 position = startPosition + dir * distance;
            positionList.Add(position);
        }

        return positionList;
    }

    private Vector3 ApplayRotationToVector(Vector3 vec, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * vec;
    }
}
