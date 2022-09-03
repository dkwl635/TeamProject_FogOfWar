using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTowerController : TowerController
{
    //----- 타워 공격 관련 변수
    [Header("타워 공격 관련 변수")]
    public ParticleSystem LazerPtc = null;
    public GameObject Lazertr = null;

    [HideInInspector] public List<GameObject> TargetList = new List<GameObject>();


    protected override void TurretAI()
    {
        //타워가 공격 상태가 아닐 때 포탑이 회전한다.
        if (TwCond == towerCondition.idle && towerHead != null)
        {
            Effectoff();
            towerHead.transform.eulerAngles = new Vector3(0, towerHead.transform.eulerAngles.y, 0);
            towerHead.transform.Rotate(0, 60f * Time.deltaTime, 0);
        }
        //타워가 공격상태일 때
        else if (TwCond == towerCondition.attack)
        {
            if (!target)
            {
                //리스트 내 가장 먼저 들어온 유닛을 타겟으로 지정(null 이면 삭제)
                if (enemyList.Count != 0)
                {
                    for (int i = 0; i < enemyList.Count;)
                    {
                        if (enemyList[i].Equals(null) || enemyList[i].gameObject.activeSelf.Equals(false))
                            enemyList.RemoveAt(i);
                        else
                            i++;
                    }
                }
                else
                {
                    TwCond = towerCondition.idle;
                    return;
                }

                if (enemyList.Count >= 1)
                    target = enemyList[0];

            }
            else
            {
                dic = target.transform.position;
                Lazertr.transform.LookAt(dic);
                dic.y = towerHead.transform.position.y;
                towerHead.transform.LookAt(dic);
                

                //dic2 = target.transform.position;
                //

                //공격 범위 내에 유닛이 없으면 -> 다시 idle 상태로 바꾼다.
                if (enemyList.Count <= 0)
                {
                    TwCond = towerCondition.idle;
                    return;
                }

                delta += Time.deltaTime;
                if (delta >= twInfo.attackcycle)
                {
                    Attack();
                    delta = 0f;
                }
            }
        }
    }


    protected override void Attack()
    {
        LazerPtc.Play();
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

        SoundManager.Instance.PlayEffSound("LaserTower", this.transform.position);
    }

    protected override void Effectoff()
    {
        LazerPtc.Stop();
    }
}
