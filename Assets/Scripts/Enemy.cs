using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, .05f);
    }
}
