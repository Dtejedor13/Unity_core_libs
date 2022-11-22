using UnityEngine;

public class RTSCameraController : MonoBehaviour
{
    public static RTSCameraController Instance { get { return _instance; } }
    private static RTSCameraController _instance;

    [SerializeField] bool RTSMode = false;
    [SerializeField] Transform cameraTransform;
    [SerializeField] private float normalSpeed = .5f;
    [SerializeField] private float fastSpeed = 3f;
    [SerializeField] private float movementTime = 5f;
    [SerializeField] private float rotationAmount = 1f;
    [SerializeField] private Vector3 zoomAmount = new Vector3(0, -10, 10);
    [Header("Zooming")]
    [SerializeField] float minYCameraPosition = 20f;
    [SerializeField] float maxYCameraPosition = 80f;

    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 rotateStartPosition;
    private Vector3 rotateCurrentPosition;
    private Vector3 newZoom;
    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;
    private float movementSpeed;

    void Start()
    {
        _instance = this;
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!RTSMode)
        {
            // for RTS games not recomended
            HandleMouseInput();
        }

        HandleMouseRotation();
        HandleMovmentInput();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMouseInput()
    {
        // if left mousebutton is pressed
        if (Input.GetMouseButtonDown(0)) {
            float entry;
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out entry)) {
                dragStartPosition = ray.GetPoint(entry);
            }
        }

        // if left mousebutton is holding
        if (Input.GetMouseButton(0)) {
            float entry;
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);

                newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }
    }

    private void HandleMouseRotation()
    {
        // middle mousebutton is pressed
        if (Input.GetMouseButtonDown(2)) {
            rotateStartPosition = Input.mousePosition;
        }

        // middle mousebutton is holding
        if (Input.GetMouseButton(2)) {
            rotateCurrentPosition = Input.mousePosition;
            Vector3 difference = rotateStartPosition - rotateCurrentPosition;
            // reset for nex trame
            rotateStartPosition = rotateCurrentPosition;

            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
        }
    }

    private void HandleMovmentInput()
    {
        // fast or normalspeed
        movementSpeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : normalSpeed;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            newPosition += (transform.forward * movementSpeed);
        }
        
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            newPosition += (transform.forward *  -movementSpeed);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            newPosition += (transform.right * movementSpeed);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            newPosition += (transform.right * -movementSpeed);
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
    }

    private void HandleRotation()
    {
        if (RTSGameManager.Instance.ConstructionMode) return;
        // rotation
        if (Input.GetKey(KeyCode.Q)) {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }

        if (Input.GetKey(KeyCode.E)) {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
    }

    private void HandleZoom()
    {
        // zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && newZoom.y + zoomAmount.y >= minYCameraPosition) {
            newZoom += zoomAmount;
        }

        // zoom out
        if (Input.GetAxis("Mouse ScrollWheel") < 0f && newZoom.y - zoomAmount.y <= maxYCameraPosition) {
            newZoom -= zoomAmount;
        }

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
