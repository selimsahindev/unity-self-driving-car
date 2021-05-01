using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTracker : MonoBehaviour {
    RaycastHit hit;
    Ray ray;

    private void Start() {
        ray = new Ray(transform.position, -transform.up);
    }

    private void FixedUpdate() {
        if (Physics.Raycast(ray, out hit, 2f)) {
            Debug.Log(hit.transform.tag);
        }
    }
}
