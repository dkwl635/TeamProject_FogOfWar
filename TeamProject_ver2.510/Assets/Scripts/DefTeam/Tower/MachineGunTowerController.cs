using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunTowerController : TowerController
{
    //----- 타워 공격 관련 변수
    [Header("타워 공격 관련 변수")]
    public ParticleSystem Particle = null;

    protected override void Attack()
    {
        //이펙트
        Particle.Play();
        target.GetComponent<UnitDamage>().TakeDamage(twInfo.towerdamage);
        SoundManager.Instance.PlayEffSound("MachineGunTower", this.transform.position);
    }

    protected override void Effectoff()
    {
        Particle.Stop();
    }
}
