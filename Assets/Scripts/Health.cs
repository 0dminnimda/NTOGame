using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private float _healthPoints;
    public float healthPoints { get => _healthPoints; }

    public void TakeDamage(float damage)
    {
        _healthPoints -= damage;

        if (_healthPoints <= 0)
        {
            Destroy(gameObject);
            Debug.Log("Died");
        }
    }
}
