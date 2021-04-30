using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public Camera cam1;
    public Camera cam2;

    private void Start() {
        cam1.enabled = true;
        cam2.enabled = false;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.C)) {
            SwitchCamera();
        }
    }

    private void SwitchCamera() {
        if (cam1.enabled) {
            cam1.enabled = false;
            cam2.enabled = true;
        } else {
            cam1.enabled = true;
            cam2.enabled = false;
        }
    }
}
