using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundChanger : MonoBehaviour
{
    public BackgroundSwitcher backgroundManager;

    public string playerTag = "Player";

    public bool activatesBackgroundB = true;

    private void OnTriggerEnter2D(Collider2D other) 

    {

        if (other.CompareTag(playerTag) && backgroundManager != null)

        {

            if (activatesBackgroundB)

            {

                backgroundManager.SwitchToBackgroundB();

            }

            else

            {


                backgroundManager.SwitchToBackgroundA();

            }

        }

    }

}
