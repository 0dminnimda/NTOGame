using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();    
    }

    public void MoveToLocation(Vector3 targetPoint)
    {
        agent.destination = targetPoint;
        // agent.isStopped = false;
    }

    private void Update()
    {
        if (agent.isOnNavMesh)
            if (agent.remainingDistance <= 0.5f)
                Destroy(gameObject);
    }

    // public void StartMovingTowards
}
