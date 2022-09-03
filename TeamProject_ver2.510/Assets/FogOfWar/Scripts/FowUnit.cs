using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FowUnit : MonoBehaviour
{
    //xz 시야 범위
    public float sightRange = 5;
    void OnEnable()
    {
        FowManager.AddUnit(this);
    }
    void OnDisable()
    {
        FowManager.RemoveUnit(this);
    }
    void OnDestroy()
    {
        FowManager.RemoveUnit(this);
    }
}