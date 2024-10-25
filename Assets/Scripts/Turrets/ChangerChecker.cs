using UnityEngine;

public class ChangerChecker: MonoBehaviour
{
    void Update()
    {
        if (transform.hasChanged)
        {
            print($"The transform has changed on {gameObject.name}");
            transform.hasChanged = false;
        }
    }

}