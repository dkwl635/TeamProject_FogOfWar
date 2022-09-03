using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDamage : MonoBehaviour
{
    TowerController tc;
    // Start is called before the first frame update
    void Start()
    {
        tc = GetComponentInParent<TowerController>();
    }

    //타워가 공격 받으면 호출
    public void TakeDamage(float value = 10)
    {
        tc.TakeDamage(value);
    }

}
