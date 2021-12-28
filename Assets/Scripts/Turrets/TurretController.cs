using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TurretMovementController
public class TurretController : MonoBehaviour
{
    // Angular speed in radians per sec.
    public float turningSpeed = 1.0f;
    public Transform point;

    [Header("Vertical")]
    public Transform verticalMovingPart;
    public float minVerticalTurnAngle = 0.02f;
    
    private float verticalAngle;
    private Vector3 originalVerticalEulerAngles;

    [Header("Horisontal")] public Transform horisontalMovingPart;
    public float minHorisontalTurnAngle = 0.02f;

    private float horisontalAngle;
    private Vector3 originalHorisontalEulerAngles;

    private void Awake()
    {
        RecordAngles();
    }

    private void Update()
    {
        PointAt(point.position);
    }

    public void RecordAngles()
    {
        originalVerticalEulerAngles = verticalMovingPart.localRotation.eulerAngles;
        originalHorisontalEulerAngles = horisontalMovingPart.localRotation.eulerAngles;
    }

    private void TurnOneThing(
        Vector3 point, Transform transform, float turningSpeed, float minTurnAngle, Vector3 normal)
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = transform.parent.InverseTransformDirection(
            Vector3.ProjectOnPlane(point - transform.position, normal));
        Vector3 forward =         transform.parent.InverseTransformDirection(
            Vector3.ProjectOnPlane(transform.forward,          normal));
        
        // The step size is equal to speed times frame time.
        float singleStep = turningSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(
            forward, targetDirection, singleStep, minTurnAngle);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.localRotation = Quaternion.LookRotation(newDirection);
    }
    
    public void PointAt(Vector3 point)
    {
        TurnOneThing(point, verticalMovingPart, turningSpeed, minVerticalTurnAngle,
            verticalMovingPart.parent.right);

        TurnOneThing(point, horisontalMovingPart, turningSpeed, minHorisontalTurnAngle,
               horisontalMovingPart.parent.up);
    }
}