using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrampaGuideMovement : MonoBehaviour
{
    [Header("Ruta y Componentes")]
    [SerializeField] private Transform[] waypoints; // Arreglo de puntos de la ruta (GameObjects vacíos)
    [SerializeField] private Transform player;      // Referencia al Transform del jugador

    [Header("Configuración de Movimiento")]
    [SerializeField] private float movementSpeed = 5f;  // Velocidad general (afecta la rapidez del Lerp)
    [Range(0.1f, 10f)]
    private float lerpFactor = 3f;     // Factor para la fluidez del Lerp (más bajo = más suave)
    private float proximityThreshold = 0.5f; // Distancia para considerar que llegó al waypoint

    [Header("Configuración de Espera")]
    [SerializeField] private float waitDistance = 5f;   // Distancia máxima antes de que el guía espere
    [SerializeField] private string waitPointTag = "WaitPoint"; // Tag para los waypoints donde debe esperar

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
            // Ruta completada. Puedes desactivar el script o destruir el objeto.
            return;
        }

        // 1. Lógica de Espera (Prioridad: Verifica si debe esperar)
        CheckForWaiting();

        // 2. Lógica de Movimiento
        if (!isWaiting)
        {
            MoveTowardsWaypoint();
        }
    }

    private void CheckForWaiting()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Solo verifica la distancia al jugador si el waypoint actual tiene el tag de espera
        if (targetWaypoint.CompareTag(waitPointTag))
        {
            float distToPlayer = Vector2.Distance(transform.position, player.position);

            if (distToPlayer > waitDistance)
            {
                isWaiting = true;
                // Debug.Log("Guía esperando al jugador.");
            }
            else
            {
                // Jugador cerca, reanuda el movimiento
                isWaiting = false;
            }
        }
        else
        {
            // El punto actual no es un punto de espera, el guía siempre se mueve
            isWaiting = false;
        }
    }

    private void MoveTowardsWaypoint()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Movimiento muy fluido usando Lerp. 
        // El movimiento es independiente del framerate gracias a Time.deltaTime.
        transform.position = Vector2.Lerp(
            transform.position,
            targetWaypoint.position,
            lerpFactor * Time.deltaTime
        );

        // Comprobar si ha llegado lo suficientemente cerca
        if (Vector2.Distance(transform.position, targetWaypoint.position) < proximityThreshold)
        {
            // Avanzar al siguiente punto en la ruta
            currentWaypointIndex++;
        }
    }

    // Opcional: Dibuja la ruta en el editor para visualización
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
}
