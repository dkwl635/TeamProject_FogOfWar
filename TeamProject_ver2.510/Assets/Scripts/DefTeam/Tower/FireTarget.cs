using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTarget : MonoBehaviour
{ 
    FireTowerController fc;
    private void Start()
    {
        fc = GetComponentInParent<FireTowerController>();
    }

    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("Unit"))
        {
            if (fc.TargetList.Contains(other.gameObject))
                return;

            fc.TargetList.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (fc.TargetList.Contains(other.gameObject))
        {
            fc.TargetList.Remove(other.gameObject);
        }
    }
}
