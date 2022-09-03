using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDamage : MonoBehaviour
{
    UnitCtrl uc;
    ShieldCtrl sc;
    // Start is called before the first frame update
    void Start()
    {
        uc = GetComponentInParent<UnitCtrl>();
        sc = GetComponentInParent<ShieldCtrl>();
    }

    public void TakeDamage(float damage = 10f)
    {
        if (sc.isShieldOn)
        {
            return;
        }
        else   
            uc.TakeDamage(damage);
    }
}
