using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBrain : MonoBehaviour {
    [Range(-1f, 1f)]
    public float v, h;

    public ProximitySensor sensor;
    public RoadTracker roadTracker;
    public float timeSinceStart;

    // Coefficents of the speed and the distance traveled by the car in the fitness function
    // Staying in the middle of the road is also important, so we have a sensor multiplier
    [Header("Fitness")]
    public float overallFitness;
    public float distanceMultiplier = 1.4f;
    public float averageSpeedMultiplier = 0.2f;
    public float sensorMultiplier = 0.1f;

    private float averageSpeed;
    private float totalDistanceTraveled;
    private Vector3 startPosition;
    private Vector3 startRotation;
    private Vector3 lastPosition;
    private CarControllerV2 car;

    private void Awake() {
        car = GetComponent<CarControllerV2>();
        startPosition = transform.position;
        startRotation = transform.rotation.eulerAngles;
    }
    
    private void FixedUpdate() {
        // Check if the car is off the road, reset if it does;
        if (!roadTracker.IsTrackersOnRoad())
            Reset();

        lastPosition = transform.position;
        timeSinceStart += Time.fixedDeltaTime;
        totalDistanceTraveled += Vector3.Distance(transform.position, lastPosition);

        // Neural Network Code Here
        car.Drive(v, h);

        CalculateFitness();
    }

    private void CalculateFitness() {
        if (timeSinceStart > 0f)
            averageSpeed = totalDistanceTraveled / timeSinceStart;

        float traveledDistanceFactor = totalDistanceTraveled * distanceMultiplier;
        float averageSpeedFactor = averageSpeed * averageSpeedMultiplier;
        float proximityFactor = ((sensor.sensorA + sensor.sensorB + sensor.sensorC) / 3f) * sensorMultiplier;

        overallFitness = traveledDistanceFactor + averageSpeedFactor + proximityFactor;
    }

    public void Reset() {
        timeSinceStart = 0f;
        totalDistanceTraveled = 0f;
        averageSpeed = 0f;
        lastPosition = startPosition;
        overallFitness = 0f;
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
        car.Reset();
    }

}
