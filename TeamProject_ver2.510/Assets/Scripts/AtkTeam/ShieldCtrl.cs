using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCtrl : MonoBehaviour
{
    //public GameObject shieldEffect = null;                          //실드 이펙트 담긴 변수
    [HideInInspector] public bool isShieldOn = false;              //현재 이 탱크가 실드스킬에 타겟되고있는지 확인하는 변수

    MeshRenderer[] tankmesh;                          //실드스킬 일때 타겟된 탱크의 MeshRenderer를 저장하기위한 변수
    Color32[] tankmeshColor;                          //탱크의 기본색깔을 저장하기위한 변수    

    // Start is called before the first frame update
    void Start()
    {
        tankmesh = gameObject.GetComponentsInChildren<MeshRenderer>();         //하위 오브젝트의 MeshRenderer찾기
        tankmeshColor = new Color32[tankmesh.Length];                          //MeshRenderer의 수 만큼 Color32 배열 생성
        for (int i = 0; i < tankmeshColor.Length; i++)
            tankmeshColor[i] = tankmesh[i].material.GetColor("_Color");    //탱크의 기본 색상 가져오기
    }

    // Update is called once per frame
    void Update()
    {        
        if (isShieldOn)
        {
            for (int i = 0; i < tankmesh.Length; i++)
                if (tankmesh[i].material.color != new Color32(0, 0, 255, 255))                //이 색상이 아니라면
                    tankmesh[i].material.SetColor("_Color", new Color32(0, 0, 255, 255));  //쉴드스킬의 타겟이 되고있는 탱크의 색상 변경                       
        }
        else
        {
            for (int i = 0; i < tankmesh.Length; i++)
                if (tankmesh[i].material.color != tankmeshColor[i])                         //탱크의 기본색상이 아니라면
                    tankmesh[i].material.SetColor("_Color", tankmeshColor[i]);          //쉴드스킬의 타겟이 아닐때 기본색상으로 변경
        }        
    }
}