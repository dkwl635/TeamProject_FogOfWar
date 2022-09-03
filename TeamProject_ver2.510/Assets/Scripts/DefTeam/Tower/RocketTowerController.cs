using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTowerController : TowerController
{
    //----- 타워 공격 관련 변수
    [Header("타워 공격 관련 변수")]
    public GameObject RocketPrefab = null;

    protected override void Attack()
    {
        GameObject go = Instantiate(RocketPrefab);
        go.transform.position = SpawnPoint.transform.position;
        go.transform.LookAt(target.transform);

        RocketController rocketController = go.GetComponent<RocketController>();
        rocketController.Target = target;
        rocketController.RocketDamage = twInfo.towerdamage;

        SoundManager.Instance.PlayEffSound("RocketTower", this.transform.position);
    }    
}
