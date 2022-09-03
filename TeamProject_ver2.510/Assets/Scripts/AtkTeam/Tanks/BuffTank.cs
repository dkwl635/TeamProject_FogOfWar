using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTank : UnitCtrl
{
    float SkillTime = 5.0f;             //버프 지속시간
    float SkillCool = 5.0f;             //버프 쿨타임
    bool BuffOnOff = true;             //버프 온오프체크  //탱크가 필드에 등장했을때는 버프 제공

    public override void SetStat(TankInfo info)
    {
        base.SetStat(info);

        nvAgent.speed = 6f;
    }

    protected override void UniqueSystem()
    {
        if (SkillTime > 0.0f)           //스킬 지속시간
        {
            SkillTime -= Time.deltaTime;
            if (SkillTime <= 0.0f)      //지속시간 끝나면
            {
                SkillTime = 0.0f;
                SkillCool = 5.0f;      //쿨타임 적용                
                BuffOnOff = false;
            }
        }
        else if (SkillTime <= 0.0f)
        {
            SkillCool -= Time.deltaTime;
            if (SkillCool <= 0.0f)
            {
                SkillTime = 5.0f;           //스킬 재적용
                SkillCool = 0.0f;
                BuffOnOff = true;
                SoundManager.Instance.PlayEffSound("BuffSound", transform.position);
            }
        }

        buffEffect.SetActive(BuffOnOff);      //내 이펙트 표시
    }

    protected override void Fire(Vector3 targetPos)
    {

    }

    public List<UnitCtrl> BuffTankList = new List<UnitCtrl>();
    void OnTriggerStay(Collider other)      //버프의 온오프와 관계없이 충돌범위 안에 있을 때 계속 측정
    {
        //충돌 시 우리팀일 경우
        if (other.CompareTag("Unit") && other.gameObject.name.Contains("Buff") != true)      //우리팀이고 버프탱크가 아니면....
        {
            UnitCtrl a_UC = other.GetComponentInParent<UnitCtrl>();
    

            if (BuffOnOff)                      //버프 지속시간 중이면
                a_UC.takeSkill = TakeSkill.BuffOn;      //충돌대상을 버프받는 상태로 바꿔주기

            else if (BuffOnOff == false || tankState == TankState.Die)
            {
                a_UC.takeSkill = TakeSkill.BuffOff;
            }//버프 쿨타임이면  
                     //충돌대상을 버프꺼진 상태로 바꿔주기
        }
    }

    void OnTriggerExit(Collider other)      //버프 온오프 상관없이 충돌범위 안에있다가 나가면 버프 꺼진 상태되도록...
    {
        //충돌 끝날 때 우리팀일 경우        
        if (other.CompareTag("Unit") && other.gameObject.name.Contains("Buff") != true)      //우리팀이고 버프탱크가 아니면....
        {
            UnitCtrl a_UC = other.GetComponentInParent<UnitCtrl>();
            a_UC.takeSkill = TakeSkill.BuffOff;     //충돌대상을 버프꺼진 상태로 바꿔주기
            //BuffTankList.Remove(a_UC);
        }
    }
}