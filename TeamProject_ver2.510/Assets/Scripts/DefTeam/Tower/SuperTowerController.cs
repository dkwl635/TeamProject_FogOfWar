using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperTowerController : TowerController
{
    //----- 타워 공격 관련 변수
    [Header("타워 공격 관련 변수")]
    public GameObject SuperRocket = null;
    public GameObject[] ShotPoint = null;

    protected override void Attack()
    {
        GameObject go = Instantiate(SuperRocket);
        go.transform.position = ShotPoint[Random.Range(0, 5)].transform.position;
        go.transform.eulerAngles = towerHead.transform.eulerAngles + new Vector3(-90, Random.Range(-120, 121), 0);

        SuperRocketController SRC = go.GetComponent<SuperRocketController>();
        SRC.Target = enemyList[Random.Range(0, enemyList.Count)];
        SRC.RocketDamage = twInfo.towerdamage;

        SoundManager.Instance.PlayEffSound("SuperTower", this.transform.position);
    }
}
