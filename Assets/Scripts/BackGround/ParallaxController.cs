using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    Transform cam; // Referencia al transform de la cámara principal
    Vector3 camStartPos; // Posición inicial de la cámara
    float distance; // Distancia recorrida por la cámara desde el inicio

    GameObject[] Background;
    Material[] mat;

    [Header("Configuración de Parallax")]
    [Tooltip("El número de elementos debe coincidir con el número de capas hijas. Usa 0.0 para una capa fija.")]
    [SerializeField] private float[] layerSpeeds; // Velocidades de parallax (ajustables en el Inspector)

    [Range(0.01f, 0.5f)]
    public float ParallaxSpeed = 0.05f; // Factor de velocidad global

    void Start()
    {
        // 1. Inicializar Cámara y Posición Inicial
        cam = Camera.main.transform;

        camStartPos = cam.position;

        int backCount = transform.childCount;
        mat = new Material[backCount];
        Background = new GameObject[backCount];

        //  VERIFICACIÓN: Comprueba que el array de velocidades coincida con el número de capas.
        if (layerSpeeds == null || layerSpeeds.Length != backCount)
        {
            Debug.LogError("El número de 'layerSpeeds' NO coincide con el número de capas hijas (" + backCount + "). Por favor, ajusta el tamaño del array en el Inspector.");
            // Inicialización de emergencia: asigna velocidad 1.0 a todas las capas para que se muevan.
            layerSpeeds = new float[backCount];
            for (int i = 0; i < backCount; i++) layerSpeeds[i] = 1f;
        }

        // 2. Obtener fondos y materiales
        for (int i = 0; i < backCount; i++)
        {
            Background[i] = transform.GetChild(i).gameObject;
            Renderer renderer = Background[i].GetComponent<Renderer>();

            if (renderer != null)
            {
                mat[i] = renderer.material;
            }
            else
            {
                Debug.LogError($"La capa hija {i} no tiene un componente Renderer. El parallax no funcionará en esa capa.");
            }
        }
    }

    void LateUpdate()
    {
        if(cam == null || Background.Length == 0) return;

        // 1. Calcular distancia SOLO en X (para el paralaje horizontal/tiling)
        float horizontalDistance = cam.position.x - camStartPos.x;

        // 2. Mover el controlador raíz (Padre) para seguir la posición de la cámara en X y Y.
        // ESTE ES EL CAMBIO CLAVE para que la escena avance.
        transform.position = new Vector3(cam.position.x, cam.position.y, transform.position.z);

        // 3. Aplicar el offset de textura SOLO en horizontal (X)
        for (int i = 0; i < Background.Length; i++)
        {
            if (mat[i] == null) continue;

            float speedFactor = layerSpeeds[i];
            float speed = speedFactor * ParallaxSpeed;

            // El offset se aplica solo en X. El componente Y es 0.
            mat[i].SetTextureOffset("_MainTex", new Vector2(horizontalDistance * speed, 0));
        }
    }
}


