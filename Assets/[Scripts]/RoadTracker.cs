using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTracker : MonoBehaviour {
    [SerializeField] private List<Transform> trackers;

    private RaycastHit hit;
    
    public bool IsTrackersOnRoad() {
        bool isOnRoad = true;

        trackers.ForEach(tracker => {
            Debug.DrawRay(tracker.position, Vector3.down);

            Ray ray = new Ray(tracker.position, Vector3.down);

            if (Physics.Raycast(ray, out hit, 2f)) {
                if (!hit.transform.CompareTag("Road")) {
                    isOnRoad = false;
                }
            }
        });

        return isOnRoad;
    }
}
