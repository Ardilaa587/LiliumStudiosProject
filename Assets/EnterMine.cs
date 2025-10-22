using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterMine : MonoBehaviour
{
    private GameObject platform;
    PickUps pickUps;

    public void OnDestroy()
    {
        pickUps.isPickedUp = true;

        gameObject.SetActive(false);
    }

}
