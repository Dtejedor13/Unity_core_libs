using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TP_RotateToCamera : MonoBehaviour
{
    private float Speed = 10.0f;
    private bool isRotating = false;

    void Update()
    {
        //Set your input right here to start the rotation
        if (Input.GetKeyDown(KeyCode.Space))
            isRotating = !isRotating; //Starts the rotation

        if (isRotating) //Check if your game object is currently rotating
            SetRotate(this.gameObject, Camera.main.gameObject);

        //When your child game object and your camera have the same rotation.y value, it stops the rotation
        if (transform.rotation.eulerAngles.y == Camera.main.transform.rotation.eulerAngles.y)
            isRotating = !isRotating;
    }

    void SetRotate(GameObject toRotate, GameObject camera)
    {
        //You can call this function for any game object and any camera, just change the parameters when you call this function
        transform.rotation = Quaternion.Lerp(toRotate.transform.rotation, camera.transform.rotation, Speed * Time.deltaTime);
    }
}
