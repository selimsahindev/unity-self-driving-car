using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTracker : MonoBehaviour {
    RaycastHit hit;
    
    private void FixedUpdate() {
        Debug.DrawRay(transform.position, Vector3.down);

        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out hit, 2f)) {
            if (!hit.transform.CompareTag("Road")) {
                Debug.Log("Out Of Track. Restarting...");
            }
        }
    }
}
