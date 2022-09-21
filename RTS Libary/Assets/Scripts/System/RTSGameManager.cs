using UnityEngine;

[RequireComponent(typeof(RTSUnitClick))]
[RequireComponent(typeof(RTSUnitDrag))]
[RequireComponent(typeof(RTSUnitSelections))]
public class RTSGameManager : MonoBehaviour
{
    [SerializeField] GameObject RTSUnitPrefab;
    [SerializeField] Transform unitAreaTransform;
    public int PlayerFactionId;
    public LayerMask Ground, Clickeble;
    [SerializeField] GameObject groundMarker;
    public static RTSGameManager Instance { get { return _instance; } }
    private static RTSGameManager _instance;

    public bool ConstructionMode;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        // spawn rts units
        int unitCount = 0;
        for(int row = 0; row < 3; row++)
            for(int colum = 0; colum < 3; colum++) {
                Vector3 spawnPoint = new Vector3(0 + (1.5f * colum), 1.5f, 1.5f - (1.5f * row));
                GameObject go = Instantiate(RTSUnitPrefab, spawnPoint, Quaternion.identity, unitAreaTransform);
                go.GetComponent<RTSCombat>().factionID = PlayerFactionId;
                go.name = "RTS Unit Id " + unitCount;
                unitCount++;
            }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool enabled = true;

            if (enabled) // action declaration
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, Clickeble))
                {
                    RTSCombat rtsCombat;
                    hit.collider.gameObject.TryGetComponent(out rtsCombat);
                    if (rtsCombat != null && rtsCombat.factionID != PlayerFactionId)
                    {
                        foreach (GameObject select in RTSUnitSelections.Instance.UnitSelected)
                        {
                            select.GetComponent<RTSUnitController>().DeclareAction(hit.collider.gameObject);
                        }
                        enabled = false;
                    }
                }
            }

            if (enabled) // groundmarker
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, Ground))
                    groundMarker.GetComponent<RTSGroundMarker>().EnableObject(hit.point);
            }
        }
    }
}
