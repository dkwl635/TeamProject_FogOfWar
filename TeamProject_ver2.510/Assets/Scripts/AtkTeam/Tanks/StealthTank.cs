using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthTank : UnitCtrl
{
    [Header("스텔스용 기능")]
    public GameObject TankMesh;
    MeshRenderer[] TankMeshRenderer;

    float stealthTime = 0f;
    bool isStealth = true;

    public override void SetStat(TankInfo info)
    {     
        base.SetStat(info);

        nvAgent.speed = 12f;

        TankMeshRenderer = TankMesh.GetComponentsInChildren<MeshRenderer>();
    }

    protected override void UniqueSystem()
    {
        stealthTime -= Time.deltaTime;
        if (stealthTime <= 0f)
        {
            isStealth = !isStealth;
            stealthTime = (isStealth) ? 5f : 3f;
            StealthOnOff(isStealth);
        }
    }

    void StealthOnOff(bool isStealth)
    {
        if(!isStealth)
        {
            for (int a = 0; a < TankMeshRenderer.Length; a++)
            {
                TankMeshRenderer[a].material.shader = Shader.Find("Unlit/BlurShader");
            }
        }
        else
        {
            for (int a = 0; a < TankMeshRenderer.Length; a++)
            {
                TankMeshRenderer[a].material.shader = Shader.Find("Standard");
            }
        }

        TankMesh.GetComponent<BoxCollider>().enabled = isStealth;
    }
}