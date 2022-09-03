using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTowerController : TowerController
{
    List<GameObject> BuffList = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tower"))
        {
            other.gameObject.GetComponentInParent<TowerController>().twInfo.towerdamage *= 1.5f;
            Debug.Log(other.gameObject.name);
            BuffList.Add(other.gameObject);
        }
    }


    protected override void Dead()
    {
        UnBuff();
    }

     void UnBuff()
    {
        for(int i = 0; i < BuffList.Count;)
        {
            if (BuffList[i].gameObject == null)
                continue;

            BuffList[i].gameObject.GetComponentInParent<TowerController>().twInfo.towerdamage /= 1.5f;
            Debug.Log("Unbuffed");

            i++;
        }
    }
}
