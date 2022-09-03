using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTower : MonoBehaviour
{
    public TowerType towertype;
    public GameObject attackRange;
    public int pointIdx;


    public void arOn()
    {
        attackRange.SetActive(true);
    }

    public void arOff()
    {
        attackRange.SetActive(false);
    }

}
