using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RTSStructure : MonoBehaviour
{
    [SerializeField] Material materialOk, materialFail;
    [SerializeField] GameObject finalBuilding;
    [SerializeField] LayerMask ground;
    [SerializeField] private List<GameObject> colliderCollection = new List<GameObject>();

    public bool IsBuildable = false;
    public Renderer rend;
    public int FactionId;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        RTSGameManager.Instance.ConstructionMode = true;
    }

    private void Update()
    {
        HandleRotation();

        if (Input.GetMouseButton(1) && IsBuildable)
        {
            GameObject go = Instantiate(finalBuilding, transform.parent.position, transform.parent.rotation);
            go.transform.GetChild(0).GetComponent<RTSCombat>().factionID = FactionId;
            RTSSpawnStructuresSystem.Instance.ClearBluePrintReferance();
            Destroy(gameObject);
            RTSGameManager.Instance.ConstructionMode = false;
        }
        else if (Input.GetMouseButton(0)) {
            // cancel building
            RTSGameManager.Instance.ConstructionMode = false;
            Destroy(gameObject);
        }
    }

    private void HandleRotation()
    {
        // rotation
        Quaternion newRotation = transform.parent.rotation;
        if (Input.GetKey(KeyCode.Q)) {
            newRotation *= Quaternion.Euler(Vector3.up * 10f);
        }

        if (Input.GetKey(KeyCode.E)) {
            newRotation *= Quaternion.Euler(Vector3.up * -10f);
        }

        transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, newRotation, Time.deltaTime * 15f);
    }

    private void FixedUpdate()
    {
        GetMousePosition();
        if (colliderCollection.Any(x => x.gameObject.isStatic == false)) {
            rend.material = materialFail;
            IsBuildable = false;
        }
        else {
            rend.material = materialOk;
            IsBuildable = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        colliderCollection.Add(collision.collider.gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        colliderCollection.Remove(collision.collider.gameObject);
    }

    private void GetMousePosition()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // hit everything
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
        {
            transform.parent.position = hit.point;
        }
    }
}
