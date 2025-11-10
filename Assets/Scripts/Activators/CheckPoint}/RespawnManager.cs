using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.SearchService;
using UnityEditorInternal;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager instance;

    public Vector2 lastRespawnPosition;

    public bool isCameraEffectActive = true;
    public bool effectRemovalItemCollected = false;
    public bool itemAInteracted = false;
    private GrampaGuideMovement grampaGuide;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            lastRespawnPosition = Vector2.zero;
            isCameraEffectActive = true;

        }
        else
        {
            Destroy(gameObject);
        }

        grampaGuide = FindObjectOfType<GrampaGuideMovement>();

        
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void SoftRespawn(PlayerController player)
    {
        player.gameObject.SetActive(false);
        player.transform.position = lastRespawnPosition;

        player.health = player.maxHealth;

        player.rb.velocity = Vector2.zero;
        player.horizontal = 0f;

        player.gameObject.SetActive(true);

        if (grampaGuide != null)
        {
            grampaGuide.RespawnToNearestWaypoint(lastRespawnPosition);
        }
    }


    public void SetCheckpoint(Vector2 newPosition)
    {
        lastRespawnPosition = newPosition;
    }

    public void RespawnPlayer(GameObject player)
    {
        player.transform.position = lastRespawnPosition;

        if (grampaGuide != null)
        {
            // Usar la misma posición de respawn para reubicar la guía
            grampaGuide.RespawnToNearestWaypoint(lastRespawnPosition);
        }
    }

    public void SetCameraEffectState(bool state)
    {
        isCameraEffectActive = state;
    }

    public void CollectEffectRemovalItem()
    {
        effectRemovalItemCollected = true;
        SetCameraEffectState(false);
    }

    public void SetItemAInteracted()
    {
        itemAInteracted = true;

        CollectEffectRemovalItem(); 
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Solo reposicionamos si la escena es un nivel jugable y no el menú
        if (scene.name != "Menu" && lastRespawnPosition != Vector2.zero)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                // Mueve directamente al jugador a la posición que el MenuUI guardó.
                playerObject.transform.position = lastRespawnPosition;

                // Opcional: Si el jugador aparece en un estado muerto, 
                // llama a la función de reinicio suave para restaurar la vida.
                // PlayerController playerController = playerObject.GetComponent<PlayerController>();
                // if (playerController != null)
                // {
                //     playerController.SoftRespawn(lastRespawnPosition);
                // }
            }
        }
    }
}
