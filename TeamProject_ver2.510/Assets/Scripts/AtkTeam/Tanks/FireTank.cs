using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTank : UnitCtrl
{
    protected override void UniqueSystem()
    {
        if (tankState == TankState.Attack)
            firePos.GetChild(0).gameObject.SetActive(true);
        else
            firePos.GetChild(0).gameObject.SetActive(false);
    }

    protected override void Fire(Vector3 targetPos)
    {
        base.Fire(targetPos);
        SoundManager.Instance.PlayEffSound("FireSound", transform.position);
    }
}
