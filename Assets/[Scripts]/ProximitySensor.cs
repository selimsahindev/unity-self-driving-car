using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensor : MonoBehaviour {
    [SerializeField] private float sensorRange = 20f;

    [HideInInspector] public float sensorA, sensorB, sensorC;

    private void FixedUpdate() {
        InputSensors();
    }

    private void InputSensors() {
        Vector3 a = (transform.forward - transform.right).normalized;
        Vector3 b = (transform.forward).normalized;
        Vector3 c = (transform.forward + transform.right).normalized;

        RaycastHit hit;
        Ray ray = new Ray(transform.position, a);

        if (Physics.Raycast(ray, out hit, sensorRange)) {
            sensorA = hit.distance / 20f;
            Debug.DrawLine(transform.position, transform.position + a * sensorRange, Color.green);
        } else {
            Debug.DrawLine(transform.position, transform.position + a * sensorRange, Color.red);
        }

        ray.direction = b;

        if (Physics.Raycast(ray, out hit, sensorRange)) {
            sensorB = hit.distance / 20f;
            Debug.DrawLine(transform.position, transform.position + b * sensorRange, Color.green);
        } else {
            Debug.DrawLine(transform.position, transform.position + b * sensorRange, Color.red);
        }

        ray.direction = c;

        if (Physics.Raycast(ray, out hit, sensorRange)) {
            sensorC = hit.distance / 20f;
            Debug.DrawLine(transform.position, transform.position + c * sensorRange, Color.green);
        } else {
            Debug.DrawLine(transform.position, transform.position + c * sensorRange, Color.red);
        }

        print("A: " + sensorA + "\tB: " + sensorB + "\tC: " + sensorC);
    }
}
