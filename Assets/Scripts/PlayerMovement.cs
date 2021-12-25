using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [Flags]
// public enum MovementDirection
// {
//     Up,
//     Down,
//     Left,
//     Right,
//     UpLeft = Up & Left,
//     UpRight = Up & Right,
//     DownLeft = Down & Left,
//     DownRight = Down & Right,
// } 

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    public float movementSpeed = 5f;
    public float rotationSpeed = 5f;

    public Transform model;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void UpdatePlayer()
    {
        // Input.GetAxis("Mouse X")
        // transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * Time.deltaTime * rotationSpeed);

        // Input.GetAxis("Horizontal")
        //Store user input as a movement vector
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        movement = transform.TransformDirection(movement);

        //Apply the movement vector to the current position, which is
        //multiplied by deltaTime and speed for a smooth MovePosition
        Vector3 newPos = transform.position + movement * Time.fixedDeltaTime * movementSpeed;
        model.LookAt(newPos);
        rb.MovePosition(newPos);
    }
}
