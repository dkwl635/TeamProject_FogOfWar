using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpTileCtrl : MonoBehaviour
{
    public GameObject tile = null;
    private void OnTriggerEnter(Collider other)
    {
        if (tile.layer == LayerMask.NameToLayer("SPAWN"))
            return;

        if(other.CompareTag("Unit"))
        {
            tile.layer = LayerMask.NameToLayer("SPAWN");
        }
    }
}