using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTowerController : TowerController
{
    //----- 타워 공격 관련 변수
    [Header("타워 공격 관련 변수")]
    public ParticleSystem FirePtc = null;

    [HideInInspector] public List<GameObject> TargetList = new List<GameObject>();
    protected override void Attack()
    {      
        FirePtc.Play();
        for (int i = 0; i < TargetList.Count;)
        {
            if (TargetList[i] == null)
            {
                TargetList.RemoveAt(i);
                continue;
            }

            TargetList[i].GetComponent<UnitDamage>().TakeDamage(twInfo.towerdamage);
            i++;
        }

        SoundManager.Instance.PlayEffSound("FireTower", this.transform.position);
    }

    protected override void Effectoff()
    {
        FirePtc.Stop();
    }
}
