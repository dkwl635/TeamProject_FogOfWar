using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldLand : LandSignCtrl
{
    protected override void Init()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitinfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("SPAWN")) ||
            Physics.Raycast(ray, out hitinfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("TERRAIN")))
        {
            TargetPos = hitinfo.point;
        }
        Instantiate(SkillPrefab, TargetPos, Quaternion.identity);
    }
}
