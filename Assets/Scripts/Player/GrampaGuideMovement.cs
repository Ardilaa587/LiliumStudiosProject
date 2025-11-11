using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrampaGuideMovement : MonoBehaviour
{
    [Header("Ruta y Componentes")]
    [SerializeField] private Transform[] waypoints; 
    [SerializeField] private Transform player; 

    [Header("Configuración de Movimiento")]
    [SerializeField] private float movementSpeed = 5f;  
    [Range(0.1f, 10f)]
    private float lerpFactor = 3f; 
    private float proximityThreshold = 0.5f; 

    [Header("Configuración de Espera")]
    [SerializeField] private float waitDistance = 5f;  
    [SerializeField] private string waitPointTag = "WaitPoint"; 

    private int currentWaypointIndex = 0;
    private bool isWaiting = false;

    void Start()
    {
        if (player == null)
        {
            enabled = false;
        }
    }

    void Update()
    {
        if (currentWaypointIndex >= waypoints.Length)
        {
            return;
        }

        CheckForWaiting();

        if (!isWaiting)
        {
            MoveTowardsWaypoint();
        }
    }

    private void CheckForWaiting()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        if (targetWaypoint.CompareTag(waitPointTag))
        {
            float distToPlayer = Vector2.Distance(transform.position, player.position);

            if (distToPlayer > waitDistance)
            {
                isWaiting = true;
            }
            else
            {
                isWaiting = false;
            }
        }
        else
        {
            isWaiting = false;
        }
    }

    private void MoveTowardsWaypoint()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        transform.position = Vector2.Lerp(
            transform.position,
            targetWaypoint.position,
            lerpFactor * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetWaypoint.position) < proximityThreshold)
        {
            currentWaypointIndex++;
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }

    public void RespawnToNearestWaypoint(Vector2 checkpointPosition)
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("GrampaGuide no tiene waypoints asignados. No puede reubicarse.");
            return;
        }

        int nearestIndex = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            float distance = Vector2.Distance(checkpointPosition, waypoints[i].position);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestIndex = i;
            }
        }

        transform.position = waypoints[nearestIndex].position;

        currentWaypointIndex = nearestIndex;

        isWaiting = false;
    }
}
