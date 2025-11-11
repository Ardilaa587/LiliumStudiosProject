using UnityEngine;

public class Candle : MonoBehaviour, InteractableI
{
    [Header("Flama (hijo del objeto)")]
    public GameObject flame; 

    public bool IsLit { get; private set; } = true;

    private void Reset()
    {
        if (flame == null && transform.childCount > 0)
            flame = transform.GetChild(0).gameObject;
    }

    public void Extinguish()
    {
        if (!IsLit) return;

        IsLit = false;
        if (flame != null)
            flame.SetActive(false);

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.gray;
    }

    public void Relight() 
    {
        IsLit = true;
        if (flame != null)
            flame.SetActive(true);

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.white;
    }

    public void Interact(GameObject user)
    {
        if (!IsLit)
        {
            Relight();
            Debug.Log($"🕯️ {name} encendida por el jugador.");
        }
    }

    public bool canInteract()
    {
        return !IsLit;
    }
}
