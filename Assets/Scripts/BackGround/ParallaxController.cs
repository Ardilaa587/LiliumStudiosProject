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
        // Si no hay cámara o no hay capas, salir
        if (cam == null || Background.Length == 0) return;

        // 1. Calcular la distancia horizontal que se ha movido la cámara
        distance = cam.position.x - camStartPos.x;

        // 2. Mover el controlador raíz del parallax horizontalmente con la cámara
        // Esto asegura que el fondo fijo (velocidad 0.0) se quede estático en la pantalla.
        transform.position = new Vector3(cam.position.x, transform.position.y, transform.position.z);

        // 3. Aplicar el offset de textura a cada material
        for (int i = 0; i < Background.Length; i++)
        {
            if (mat[i] == null) continue; // Saltar si el material no existe

            // La velocidad es el factor serializado (layerSpeeds[i]) * el factor global (ParallaxSpeed)
            float speedFactor = layerSpeeds[i];
            float speed = speedFactor * ParallaxSpeed;

            // Si speedFactor es 0.0, el offset será 0, y la textura no se moverá.
            mat[i].SetTextureOffset("_MainTex", new Vector2(distance * speed, 0));
        }
    }
}


