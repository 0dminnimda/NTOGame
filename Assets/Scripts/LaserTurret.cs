using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTurret : MonoBehaviour
{
    public GameObject movingPart;
    public float radius;
    
    private void Update()
    {
        // Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        // foreach (var hitCollider in hitColliders)
        // {
        //     hitCollider.SendMessage("AddDamage");
        // }
    }
}
