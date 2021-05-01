using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float followTime = 0.3f;

    public Transform target;
    public Transform cameraPosition;

    private Vector3 velocity = Vector3.zero;

    private void FixedUpdate() {
        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, cameraPosition.position, ref velocity, followTime);

        transform.LookAt(target);
    }
}
