using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CoffeBags : MonoBehaviour
{
    [SerializeField] public GameObject coffeeBags;
    [SerializeField] private float minTime = 2f;
    [SerializeField] private float maxTime = 4f;
    [SerializeField] private int maxBags = 10;

    private bool canSpawn = false;

    private int currentBags = 0;

    public void ActivateSpawn()
    {
        if (!canSpawn)
        {
            canSpawn = true;
            Spawn();
        }
    }
    private void Spawn()
    {
        if(!canSpawn && currentBags >= maxBags)
        {
            return;
        }

        Debug.Log("Spawning coffee bag");

        Instantiate(coffeeBags, transform.position, Quaternion.identity);

        currentBags++;
        Invoke(nameof(Spawn), Random.Range(minTime, maxTime));

    }

    
}
