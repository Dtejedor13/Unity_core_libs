using UnityEngine;

public class RTSGroundMarker : MonoBehaviour
{
    public void EnableObject(Vector3 point)
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);

        gameObject.SetActive(true);
        transform.position = point;
    }

    public void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
