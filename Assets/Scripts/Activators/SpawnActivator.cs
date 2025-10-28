using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnActivator : MonoBehaviour
{
    [SerializeField] private CoffeBags spawner;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spawner.ActivateSpawn();
        }
    }
}
