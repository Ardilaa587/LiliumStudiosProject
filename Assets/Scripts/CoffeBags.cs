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

    private int currentBags = 0;

    // Start is called before the first frame update
    private void Start()
    {
        Spawn();
        
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    private void Spawn()
    {
        if(currentBags >= maxBags)
        {
            return;
        }

        Instantiate(coffeeBags, transform.position, Quaternion.identity);

        currentBags++;
        Invoke(nameof(Spawn), Random.Range(minTime, maxTime));

    }

    
}
