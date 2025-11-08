using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineActivator : MonoBehaviour
{

    [SerializeField] private GameObject npc;


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            npc.SetActive(true);

        }
    }

}
