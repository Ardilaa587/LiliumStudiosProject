using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEditorInternal;
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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
        }
        else
        {
            Destroy(gameObject);
        }

        lastRespawnPosition = Vector2.zero;
        isCameraEffectActive = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CameraEffects cameraEffects = Camera.main?.GetComponent<CameraEffects>();

        PickUps[] pickUpsInScene = FindObjectsOfType<PickUps>(true);

    }

    public void SetCheckpoint(Vector2 newPosition)
    {
        lastRespawnPosition = newPosition;
    }

    public void RespawnPlayer(GameObject player)
    {
        player.transform.position = lastRespawnPosition;
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
}
