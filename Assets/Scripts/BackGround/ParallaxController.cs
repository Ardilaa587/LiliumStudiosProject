using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    // VARIABLES
    Transform cam;
    Vector3 camStartPos;
    float distance;

    GameObject[] Background;
    Material[] mat;
    float[] backSpeed; // Almacena el factor de velocidad relativo (e.g., 1.0, 0.7, 0.4)

    [Range(0.01f, 0.5f)] // Aumentado el rango para más control sobre la velocidad general
    public float ParallaxSpeed = 0.05f;

    void Start()
    {
        // 1. Inicializar Cámara y Posición Inicial
        cam = Camera.main.transform;
        camStartPos = cam.position;

        int backCount = transform.childCount;
        mat = new Material[backCount];
        backSpeed = new float[backCount];
        Background = new GameObject[backCount];

        // 2. Obtener fondos y materiales
        for (int i = 0; i < backCount; i++)
        {
            Background[i] = transform.GetChild(i).gameObject;
            // Asegúrate de que los GameObjects tengan un componente Renderer
            mat[i] = Background[i].GetComponent<Renderer>().material;
        }

        // 3. Calcular las velocidades relativas
        BackSpeedCalculate(backCount);
    }

    void BackSpeedCalculate(int backCount)
    {
        // El factor de atenuación controla la diferencia de velocidad entre las capas.
        float attenuationFactor = 0.7f;

        for (int i = 0; i < backCount; i++)
        {
            // La capa más cercana (i=0) tiene la velocidad más alta (1.0).
            // La velocidad se reduce gradualmente para las capas más lejanas.
            backSpeed[i] = 1.0f - (i * attenuationFactor / backCount);
        }
    }

    void LateUpdate()
    {
        // 1. Calcular la distancia horizontal que se ha movido la cámara
        distance = cam.position.x - camStartPos.x;

        // 2. Opcional: Mover el controlador raíz del parallax horizontalmente con la cámara
        transform.position = new Vector3(cam.position.x, transform.position.y, transform.position.z);

        // 3. Aplicar el offset de textura a cada material
        for (int i = 0; i < Background.Length; i++)
        {
            // 'speed' es el factor relativo (backSpeed[i]) multiplicado por la velocidad general (ParallaxSpeed)
            float speed = backSpeed[i] * ParallaxSpeed;

            // SetTextureOffset utiliza la distancia total recorrida * la velocidad calculada
            mat[i].SetTextureOffset("_MainTex", new Vector2(distance * speed, 0));
        }
    }
}


