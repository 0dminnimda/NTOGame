using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretGuide : MonoBehaviour
{
    public TurretController turretController;
    private Transform enemyPosition;
    private bool inLoopUp;
    public float loopUpDelay = 0.1f;
    private GameObject lastEnemy;

    void Start()
    {
        turretController.updateItself = false;
    }

    void Update()
    {
        if (!inLoopUp)
            StartCoroutine(LoopUpNewEnemy());
        if (lastEnemy != null)
            turretController.UpdateItself();
    }

    IEnumerator LoopUpNewEnemy()
    {
        inLoopUp = true;

        while (true)
        {
            lastEnemy = GameManager.Instance.spawner.GetClosestEnemy(transform.position);
            if (lastEnemy != null)
                turretController.point = lastEnemy.transform;
            yield return new WaitForSeconds(loopUpDelay);
        }

        inLoopUp = false;
    }
}
