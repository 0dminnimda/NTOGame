using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ReactorAnimator : MonoBehaviour
{
    public Transform core;
    public float turnSpeed = 1f;

    public float rotationSpeed = 20f;
    
    private Quaternion curQuaternion;
    private Quaternion newQuaternion;

    private float countdown;
    public float timeBetweenTurns = 2f;

    [SerializeField]
    private bool turning;
    [SerializeField]
    private bool initialized;
    
    private void Awake()
    {
        curQuaternion = RandomQuaternion();
        countdown = timeBetweenTurns;
        turning = false;
        initialized = false;
    }

    private void Update()
    {
        if (turning)
        {
            if (!initialized)
            {
                newQuaternion = RandomQuaternion();
                initialized = true;
            }
            else if (newQuaternion == curQuaternion)
            {
                turning = false;
                initialized = false;
            }
            else
            {
                curQuaternion = Quaternion.Lerp(curQuaternion, newQuaternion, Time.deltaTime * turnSpeed);
            }
        }
        else if (countdown <= 0)
        {
            turning = true;
            countdown = timeBetweenTurns;
        }
        else
            countdown -= Time.deltaTime;
        
        core.rotation *= curQuaternion;
    }

    Quaternion RandomQuaternion()
    {
        var v = new Vector3(
            Random.Range(0.0f, 360.0f), 
            Random.Range(0.0f, 360.0f), 
            Random.Range(0.0f, 360.0f));

        v.Normalize();
        return Quaternion.Euler(v * rotationSpeed);
        //return Random.rotation;
        //Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));
    }
}
