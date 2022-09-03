using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTank : UnitCtrl
{
    public override void SetStat(TankInfo info)
    {
        base.SetStat(info);
        nvAgent.speed = 8f;
    }

    protected override void Fire(Vector3 targetPos)
    {
        base.Fire(targetPos);
        SoundManager.Instance.PlayEffSound("MissileSound", transform.position);
    }
}