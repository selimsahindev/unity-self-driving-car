using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensor : MonoBehaviour {
    [HideInInspector] public float sensorA, sensorB, sensorC;

    private void SensorInputs() {
        Vector3 a = transform.forward - transform.right;
        Vector3 b = transform.forward;
        Vector3 c = transform.forward + transform.right;

        RaycastHit hit;
        Ray ray = new Ray(transform.position, a);

        if (Physics.Raycast(ray, out hit)) {
            sensorA = hit.distance / 20f;
        }

        ray.direction = b;

        if (Physics.Raycast(ray, out hit)) {
            sensorB = hit.distance / 20f;
        }

        ray.direction = c;

        if (Physics.Raycast(ray, out hit)) {
            sensorC = hit.distance / 20f;
        }

        print("Sensor A: " + sensorA + "Sensor B: " + sensorB + "Sensor C: " + sensorC);
    }
}
