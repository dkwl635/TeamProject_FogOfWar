using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public enum TakeSkill           //탱크가 받고 있는 스킬(버프)
{
    Normal,
    BuffOn,
    BuffOff,
}

public enum TankState
{
    Idle,
    Move,
    Attack,
    Die,
}

public class UnitCtrl : MonoBehaviour
{
    [HideInInspector] public TankState tankState = TankState.Idle;
    [HideInInspector] public TakeSkill takeSkill = TakeSkill.Normal;    //스킬(버프)받는 상태 체크용

    //버프
    [Header("---버프---")]
    public GameObject buffEffect = null; //탱크 자체에 오브젝트로 들어있는 버프이펙트 컨트롤 하기위한 변수

    //----- 탱크 스탯
    float atkDelta = 0f;
    protected TankInfo stat = new TankInfo();
    float hp;
    protected NavMeshAgent nvAgent;
    //----- 탱크 스탯

    [Header("---공격---")]
    public GameObject bulletPrefab;
    public Transform firePos;
    [HideInInspector] public Vector3 targetDir = Vector3.zero;

    //공격 범위 감지
    Queue<GameObject> enemyList = new Queue<GameObject>();
    protected GameObject target = null;

    //탱크 연출(터렛)
    [Header("---연출---")]
    public Image hpBar;
    public GameObject TankHead;

    public GameObject expEff;
    Vector3 effPos = Vector3.zero;

    GameObject destination; // 네비메쉬 목적지

    public virtual void SetStat(TankInfo info)
    {
        stat = info;
        nvAgent = GetComponent<NavMeshAgent>();
        hp = stat.maxHP;
        hpBar.fillAmount = hp / stat.maxHP;

        atkDelta = stat.atkCool - 0.1f;

        destination = GameObject.Find("EndPoint");
    }

    // Update is called once per frame
    void Update()
    {
        UnitAI();

        UniqueSystem();
        StatBuff();
    }

    protected virtual void UniqueSystem()
    {
        //각종 특수 동작(버프, 스텔스 등등)
    }

    //protected virtual void DieEvent()
    //{
    //    //사망 시 특수 동작
    //}

    //탱크 유닛의 AI
    void UnitAI()
    {
        switch (tankState)
        {
            case TankState.Idle: // 다시 목적지를 찾아 움직이게 한다
                atkDelta = stat.atkCool - 0.1f;
                if (destination != null)
                {
                    nvAgent.SetDestination(destination.transform.position);
                    tankState = TankState.Move;
                }
                break;
            case TankState.Move:
                nvAgent.isStopped = false;

                if (TankHead != null)
                    TankHead.transform.rotation = Quaternion.Slerp(TankHead.transform.rotation, transform.rotation, 3.0f * Time.deltaTime);
                break;
            case TankState.Attack:
                //공격 범위 내에 유닛이 없으면 -> 다시 idle 상태로 바꾼다.
                if (enemyList.Count <= 0)
                {
                    tankState = TankState.Idle;
                }
                //큐 리스트 내에 유닛이 존재하지만 타겟이 지정되지 않았으면
                else if (target == null)
                {
                    //리스트 내 가장 먼저 들어온 유닛을 타겟으로 지정(null 이면 삭제)
                    if (enemyList.Peek() == null)
                    {
                        enemyList.Dequeue();
                    }
                    if (enemyList.Count >= 1)
                        target = enemyList.Peek();
                }
                else
                {
                    if (TankHead != null)
                    {
                        Vector3 targetVec = target.transform.position;
                        targetVec.y /= 2f;
                        Vector3 tarDir = (targetVec - firePos.position).normalized;
                        Quaternion rot = Quaternion.LookRotation(tarDir);
                        TankHead.transform.rotation = Quaternion.Slerp(TankHead.transform.rotation, rot, 3.0f * Time.deltaTime);

                    }

                    nvAgent.isStopped = true; //공격 범위 내에 타겟이 존재하면 멈추고

                    atkDelta += Time.deltaTime;
                    if (atkDelta >= stat.atkCool)
                    {
                        Fire(target.transform.position); //공격 쿨타임에 맞춰서 공격

                        //쿨타임 초기화
                        atkDelta = 0f;
                    }
                }
                break;
        }
    }

    //공격 범위 안에 들어 오면
    void OnTriggerEnter(Collider other)
    {
        //서로 팀이 다르면
        if (other.CompareTag("Tower"))
        {
            //공격 리스트 내에 넣기
            enemyList.Enqueue(other.gameObject);
            tankState = TankState.Attack;
        }
    }

    //포탄 발사
    protected virtual void Fire(Vector3 targetPos)
    {
        // 상속 받은 class에서 구현
        if (target == null)
            return;

        GameObject go = Instantiate(bulletPrefab);
        go.transform.position = firePos.position;

        targetDir = (targetPos - transform.position).normalized;

        BulletCtrl refBullet = go.GetComponent<BulletCtrl>();
        refBullet.damage = stat.damage;
        refBullet.BulletSpawn(firePos, targetDir, target);
    }

    public void TakeDamage(float damage = 10)
    {
        if (hp <= 0f)
            return;

        hp -= damage;
        hpBar.fillAmount = hp / stat.maxHP;

        if (hp <= 0f)
        {
            hp = 0f;
            //DieEvent();
            tankState = TankState.Die;

            GameObject eff = Instantiate(expEff);
            effPos = transform.position;
            effPos.y += 3f;
            eff.transform.position = effPos;

            Destroy(eff, 0.4f);
            Destroy(gameObject, .4f);
        }
    }

    void StatBuff()        //받고있는 스킬에 따라서 얻어지는 효과
    {
        if (takeSkill == TakeSkill.Normal)       //아무상태 아닐경우(맨처음 생성시)      
            return;

        else if (takeSkill == TakeSkill.BuffOn)   //버프스킬을 받고 있을 경우
        {
            if (buffEffect.activeSelf == true)  //무한적용 막기
                return;

            buffEffect.SetActive(true);        //이펙트 온
            stat.damage *= 2;        //공격력 2배(임시)
            nvAgent.speed *= 2.0f;     //이동속도 2배(임시)     
        }

        else if (takeSkill == TakeSkill.BuffOff)   //버프스킬이 끝난 경우
        {
            if (buffEffect.activeSelf == false)    //무한적용 막기
                return;

            buffEffect.SetActive(false);      //이펙트 오프
            stat.damage /= 2;        //공격력 원래대로
            nvAgent.speed /= 2.0f;     //이동속도 원래대로
        }
    }
}
