using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private float _healthPoints;
    public float healthPoints { get => _healthPoints; }

    void TakeDamage(float damage)
    {
        _healthPoints -= damage;
    }
}
