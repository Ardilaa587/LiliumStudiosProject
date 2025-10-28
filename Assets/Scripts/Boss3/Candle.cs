using UnityEngine;

public class Candle : MonoBehaviour, InteractableI
{
    [Header("Flama (hijo del objeto)")]
    public GameObject flame; // arrastra aquí el objeto hijo de la vela que representa el fuego

    public bool IsLit { get; private set; } = true;

    private void Reset()
    {
        // busca flama automáticamente si no se asignó
        if (flame == null && transform.childCount > 0)
            flame = transform.GetChild(0).gameObject;
    }

    public void Extinguish()
    {
        if (!IsLit) return;

        IsLit = false;
        if (flame != null)
            flame.SetActive(false);

        // opcional: cambia color al apagarse
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.gray;
    }

    public void Relight() // para debug
    {
        IsLit = true;
        if (flame != null)
            flame.SetActive(true);

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.white;
    }

    // ==========================
    // 🔥 Integración con el sistema de interacción del jugador
    // ==========================

    public void Interact()
    {
        if (!IsLit)
        {
            Relight();
            Debug.Log($"🕯️ {name} encendida por el jugador.");
        }
    }

    public bool canInteract()
    {
        // solo se puede interactuar si está apagada
        return !IsLit;
    }
}
