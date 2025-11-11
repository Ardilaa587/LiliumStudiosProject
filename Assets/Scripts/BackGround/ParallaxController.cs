using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    Transform cam; 
    Vector3 camStartPos; 
    float distance; 

    GameObject[] Background;
    Material[] mat;

    [Header("Configuración de Parallax")]
    [SerializeField] private float[] layerSpeeds;

    [Range(0.01f, 0.5f)]
    public float ParallaxSpeed = 0.05f; 

    void Start()
    {
        cam = Camera.main.transform;

        camStartPos = cam.position;

        int backCount = transform.childCount;
        mat = new Material[backCount];
        Background = new GameObject[backCount];

        if (layerSpeeds == null || layerSpeeds.Length != backCount)
        {
            layerSpeeds = new float[backCount];
            for (int i = 0; i < backCount; i++) layerSpeeds[i] = 1f;
        }

        for (int i = 0; i < backCount; i++)
        {
            Background[i] = transform.GetChild(i).gameObject;
            Renderer renderer = Background[i].GetComponent<Renderer>();

            if (renderer != null)
            {
                mat[i] = renderer.material;
            }

        }
    }

    void LateUpdate()
    {
        if(cam == null || Background.Length == 0) return;

        float horizontalDistance = cam.position.x - camStartPos.x;

        transform.position = new Vector3(cam.position.x, cam.position.y, transform.position.z);

        for (int i = 0; i < Background.Length; i++)
        {
            if (mat[i] == null) continue;

            float speedFactor = layerSpeeds[i];
            float speed = speedFactor * ParallaxSpeed;

            mat[i].SetTextureOffset("_MainTex", new Vector2(horizontalDistance * speed, 0));
        }
    }
}


