using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensor : MonoBehaviour {
    [SerializeField] private float sensorRange = 20f;

    [HideInInspector] public float sensorA, sensorB, sensorC;

    // these points are only for use in the OnDrawGizmos method
    private Vector3 hitPointA, hitPointB, hitPointC;

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
            hitPointA = hit.point;
            Debug.DrawLine(transform.position, hit.point, Color.green);
        } else {
            hitPointA = Vector3.zero;
            Debug.DrawLine(transform.position, transform.position + a * sensorRange, Color.red);
        }

        ray.direction = b;

        if (Physics.Raycast(ray, out hit, sensorRange)) {
            sensorB = hit.distance / 20f;
            hitPointB = hit.point;
            Debug.DrawLine(transform.position, hit.point, Color.green);
        } else {
            hitPointB = Vector3.zero;
            Debug.DrawLine(transform.position, transform.position + b * sensorRange, Color.red);
        }

        ray.direction = c;

        if (Physics.Raycast(ray, out hit, sensorRange)) {
            sensorC = hit.distance / 20f;
            hitPointC = hit.point;
            Debug.DrawLine(transform.position, hit.point, Color.green);
        } else {
            hitPointC = Vector3.zero;
            Debug.DrawLine(transform.position, transform.position + c * sensorRange, Color.red);
        }

        print("A: " + sensorA + "\tB: " + sensorB + "\tC: " + sensorC);
    }

    private void OnDrawGizmos() {
        float sphereRadius = 0.3f;
        Gizmos.color = Color.green;

        if (hitPointA.magnitude > 0f)
            Gizmos.DrawWireSphere(hitPointA, sphereRadius);
        if (hitPointB.magnitude > 0f)
            Gizmos.DrawWireSphere(hitPointB, sphereRadius);
        if (hitPointC.magnitude > 0f)
            Gizmos.DrawWireSphere(hitPointC, sphereRadius);

    }
}
