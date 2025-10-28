using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    private Material material;
    [SerializeField]private Shader shader;
    public bool effectActive = true;

    // Start is called before the first frame update
    void Start()
    {
        material = new Material (shader);
        if (RespawnManager.instance != null)
        {
            effectActive = RespawnManager.instance.isCameraEffectActive;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (effectActive == true)
        {
            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }

    }
}
