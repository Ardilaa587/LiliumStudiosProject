using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInteractable : MonoBehaviour, InteractableI
{

    public Transform playerMountPoint;
    public Transform[] movementPoints;
    public float speed = 5f;
    public float detectionRadius = 0.5f;

    [SerializeField] private Transform actualObjective;
    [SerializeField] private Rigidbody2D rb;
    private bool isMoving = false;
    private Vector2 movement;
    private MonoBehaviour playerControllerScript;

    private bool isFinished = false;

    private const string PlayerControllerScriptName = "PlayerController";

    public void Interact()
    {
        if (!canInteract())
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerControllerScript = playerObject.GetComponent(PlayerControllerScriptName) as MonoBehaviour;
            

            if (playerControllerScript != null)
            {
                playerControllerScript.enabled = false;
            }

            playerObject.transform.position = playerMountPoint.position;

            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.velocity = Vector2.zero;
                movement = Vector2.zero;
            }

            isMoving = true;
        }

    }

    public bool canInteract()
    {
        return playerMountPoint != null && !isMoving && !isFinished;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (movementPoints.Length < 2)
        {
            this.enabled = false;
            return;
        }

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Static;
            rb.velocity = Vector2.zero;
        }

        actualObjective = movementPoints[0];
        isMoving = false;
    }

    void FixedUpdate()
    {
        if (!isMoving) // Solo comprueba la bandera de movimiento
        {
            return;
        }

        float distanceToObjective = Vector2.Distance(transform.position, actualObjective.position);

        if (distanceToObjective < detectionRadius)
        {
            if (actualObjective == movementPoints[0])
            {
                actualObjective = movementPoints[1];
            }
            else if (actualObjective == movementPoints[1])
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    if (playerControllerScript == null)
                    {
                        playerControllerScript = playerObject.GetComponent(PlayerControllerScriptName) as MonoBehaviour;
                    }

                    if (playerControllerScript != null)
                    {
                        playerControllerScript.enabled = true;
                        Debug.Log("Script de control del jugador re-habilitado.");
                    }

                    playerObject.transform.position = transform.position + new Vector3(1f, 0f, 0f);
                }

                rb.velocity = Vector2.zero;
                isMoving = false;
                isFinished = true;

                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Static;
                }

                this.enabled = false;

                return;
            }
        }

        Vector2 direction = (actualObjective.position - transform.position).normalized;
        rb.velocity = direction * speed;
    }
}






