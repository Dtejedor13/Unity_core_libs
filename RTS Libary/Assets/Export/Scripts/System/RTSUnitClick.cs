using UnityEngine;

public class RTSUnitClick : MonoBehaviour
{
    private LayerMask clickable;

    private void Start()
    {
        clickable = RTSGameManager.Instance.Clickeble;
    }

    private void Update()
    {
        // check if left mousebutton is down
        if (Input.GetMouseButtonDown(0)) {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable)) {

                if (Input.GetKey(KeyCode.LeftShift))
                    RTSUnitSelections.Instance.ShiftClickSelect(hit.collider.gameObject);
                else
                    RTSUnitSelections.Instance.ClickSelect(hit.collider.gameObject);
            }
            else if (!Input.GetKey(KeyCode.LeftShift))
                RTSUnitSelections.Instance.DeselectAll();
        }
    }
}
