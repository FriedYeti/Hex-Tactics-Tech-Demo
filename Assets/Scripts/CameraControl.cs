using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    private Vector3 oldMousePosition;
    public float cameraRotationSpeed = 100f; // degrees per second to rotate
    public float cameraMovementSpeed = 5f; // units per second to move
    public float cameraDragSpeed = 2f;
    public bool inverseCameraDrag = false; // true drags the map, false drags the camera

    private void Start() {
        if(inverseCameraDrag) {
            cameraDragSpeed = cameraDragSpeed * -1;
        }
    }

    void Update () {
        gameObject.transform.Rotate(Input.GetAxisRaw("Rotation") * new Vector3(0,1,0) * Time.deltaTime * cameraRotationSpeed);
        gameObject.transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * Time.deltaTime * cameraMovementSpeed);
        
        // allow user to drag with middle mouse button to translate
        if(Input.GetMouseButton(2)) {
            Vector3 movementAmount = (Input.mousePosition - oldMousePosition) * Time.deltaTime * cameraDragSpeed;
            gameObject.transform.Translate(new Vector3(movementAmount.x, 0, movementAmount.y));
        }
        oldMousePosition = Input.mousePosition;

        // TODO camera zoom in and out
    }
}
