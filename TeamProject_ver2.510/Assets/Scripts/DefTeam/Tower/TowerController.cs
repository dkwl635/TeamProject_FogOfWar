using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum towerCondition
{
    idle,
    attack
}

public class TowerController : MonoBehaviour
{
    [Header("UI관련 변수")]
    public GameObject UICanvas = null;
    public Image HPBar = null;
    [HideInInspector]public GameObject MainCamera;

    [Header("타워 모델링 관련 변수")]
    public GameObject towerHead = null;
    public GameObject SpawnPoint = null;

    [Header("사망시 나올 이펙트")]
    public GameObject bombObj;

    //----- 타워 정보 관련 변수
    protected towerCondition TwCond = towerCondition.idle;
    protected float delta = 0;
    [HideInInspector]public float TowerHP = 0;
    //----- 타워 정보 관련 변수

    //List 자료구조를 통해 공격범위에 들어온 순서대로 공격한다
    protected List<GameObject> enemyList = new List<GameObject>();
    protected GameObject target;

    //----- 타워 공격 관련 변수
    public TowerInfo twInfo = new TowerInfo();
    protected Vector3 dic = Vector3.zero; //타워해드 몸 방향 계산용

    // Start is called before the first frame update
    void Start()
    {
        UICanvas.gameObject.SetActive(false);
    }

    public void SetStat(TowerInfo info)
    {
        twInfo = info;
        TowerHP = twInfo.towerHP;
    }

    // Update is called once per frame
    void Update()
    {
        TurretAI();
    }
   
    //포탑의 AI
    protected virtual void TurretAI()
    {
        //타워가 공격 상태가 아닐 때 포탑이 회전한다.
        if (TwCond == towerCondition.idle && towerHead != null)
        {
            Effectoff();
            towerHead.transform.Rotate(0, 60f*Time.deltaTime, 0);
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
                    towerHead.transform.rotation = Quaternion.identity;
                    return;
                }

                if (enemyList.Count >= 1)
                    target = enemyList[0];
              
            }
            else
            {
                dic = target.transform.position;
                towerHead.transform.LookAt(dic);

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

    protected virtual void Attack()
    {
        //공격
        //추가적으로 적용이 필요하면 사용

    }

    protected virtual void Effectoff()
    {
        //사용 이펙트 및 효과 끄기
        //추가적으로 적용이 필요하면 사용
    }

    protected virtual void Dead()
    {
        //죽음
        //추가적으로 적용이 필요하면 사용
    }

    public void TakeDamage(float value = 10)
    {
        TowerHP -= value;
        HPBar.fillAmount = TowerHP / twInfo.towerHP;

        UICanvas.gameObject.SetActive(true);

        if (TowerHP <= 0)
        {
            GameObject BombEffect = Instantiate(bombObj);
            BombEffect.transform.position = transform.position;

            Dead();

            // 타워 오브젝트를 구성하는 가장 상위 오브젝트를 destroy한다
            Destroy(gameObject);
            Destroy(BombEffect.gameObject, 2.0f);

            GameManager.Inst.AddGold(twInfo.gold);
        }
    }

    //공격 범위 감지
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Unit"))
        {
            //공격 범위 내에 적대 유닛이 들어오면 리스트에 추가하고 공격 상태로 전환
            if (!enemyList.Contains(other.gameObject))  //중복 체크
            {
                enemyList.Add(other.gameObject);
                TwCond = towerCondition.attack;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (enemyList.Contains(other.gameObject))  //중복 체크
        { 
            enemyList.Remove(other.gameObject);
            target = null;
        }
    }
}
