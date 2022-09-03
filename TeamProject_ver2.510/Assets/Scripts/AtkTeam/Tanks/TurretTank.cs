using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTank : UnitCtrl
{
    public Transform mainFirePos = null;        //메인 총구(탱크 몸체에 붙어있는 부분)
    public GameObject mainBullet = null;        //메인 총알
    Vector3 tgDir = Vector3.zero;               //적을 바라보기 위한 방향벡터
    float tgDiv = 0.0f;                         //적과의 거리        
    float mainAtkdelta = 0.0f;                  //공격 충전시간   

    public override void SetStat(TankInfo info)
    {
        base.SetStat(info);
        nvAgent.speed = 6f;
    }

    protected override void UniqueSystem()
    {
        if (tankState != TankState.Attack)
            return;

        if (target == null)
            return;

        Vector3 tgVec = target.transform.position;
        tgVec.y = mainFirePos.position.y;
        Vector3 calcVec = tgVec - transform.position;                             //몸체와 변경위치 사이의 벡터
        tgDir = calcVec.normalized;                                                 //방향벡터
        tgDiv = calcVec.magnitude;
        Quaternion rot = Quaternion.LookRotation(calcVec);

        if (30.0f < tgDiv && tgDiv <= 35.0)                          //적과의 거리 비교
        {
            nvAgent.isStopped = true;                                //네비매쉬 멈춤
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1.0f * Time.deltaTime);            //적타워를 향해 몸체 회전
            transform.position = Vector3.Slerp(transform.position, tgVec, 0.06f * Time.deltaTime);            //적타워를 향해 조금씩 이동
        }
        else if (tgDiv <= 30.0f)                    //메인 총알 발사할수 있는 거리
        {
            if (target == null)
                return;

            mainAtkdelta += Time.deltaTime;                                                                      //공격시간 충전
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1.0f * Time.deltaTime);             //회전해야될 값이 남아있거나 처음부터 적과 마주할경우 회전만   

            if (mainAtkdelta < 0.25)                 //0.25초마다 발사
                return;

            GameObject go = Instantiate(mainBullet);
            go.transform.position = mainFirePos.position;

            BulletCtrl refBullet = go.GetComponent<BulletCtrl>();
            refBullet.damage = (stat.damage / 8);                               //탱크데미지의 / 8 (속도가 빠르므로 약하게)
            refBullet.BulletSpawn(mainFirePos.transform, tgDir, target);

            mainAtkdelta = 0.0f;
        }
    }
}
