using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTarget : MonoBehaviour
{
    LaserTowerController lc;

    private void Start()
    {
        lc = GetComponentInParent<LaserTowerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Unit"))
        {
            if (lc.TargetList.Contains(other.gameObject))
                return;

            lc.TargetList.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(lc.TargetList.Contains(other.gameObject))
        {
            lc.TargetList.Remove(other.gameObject);
        }
    }

}
