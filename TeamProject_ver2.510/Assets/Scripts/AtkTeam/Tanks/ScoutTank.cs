using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutTank : UnitCtrl
{
    public override void SetStat(TankInfo info)
    {
        base.SetStat(info);

        nvAgent.speed = 25f;
    }

    //protected override void DieEvent()
    //{
    //    GameObject go = Instantiate(bulletPrefab);
    //    go.transform.position = transform.position;
    //    go.GetComponent<SphereCollider>().center = Vector3.zero;
    //    SoundManager.Instance.PlayEffSound("TankFireSound", transform.position);
    //}

    protected override void Fire(Vector3 targetPos)
    {
        GameObject go = Instantiate(bulletPrefab);
        go.transform.position = transform.position;
        go.GetComponent<SphereCollider>().center = Vector3.zero;
        SoundManager.Instance.PlayEffSound("TankFireSound", transform.position);
        base.TakeDamage(1000);
    }
}