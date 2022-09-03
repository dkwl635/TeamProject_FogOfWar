using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGTank : UnitCtrl
{
    protected override void Fire(Vector3 targetPos)
    {
        targetPos.y -= 10;
        base.Fire(targetPos);
        SoundManager.Instance.PlayEffSound("MGSound", transform.position);
    }
}
