using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandSignCtrl : MonoBehaviour
{
    public GameObject SkillPrefab;
    RectTransform CanvasRect;

    //캔버스의 네 꼭지점 좌표를 월드좌표로 바꾸기위한 변수들
    Vector3[] v = new Vector3[4];                                 //캔버스의 네 꼭지점의 스크린좌표를 담을 변수                           
    protected Ray ray;
    protected RaycastHit hitinfo;
    //캔버스의 네 꼭지점 좌표를 월드좌표로 바꾸기위한 변수들
    
    [HideInInspector] public Vector3 HalfSize = Vector3.zero;     //지형에 표시된 표식의 크기를 가져오기 위한 변수    
    [HideInInspector] public Vector3 TargetPos = Vector3.zero;    //식의 위치를 담을 변수
        
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        TargetPos = transform.position;                           //가상의 표식 위치를 담을 변수        

        CanvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();
        CanvasRect.GetWorldCorners(v);       //캔버스의 네 꼭지점 스크린 좌표
        int a_Rnd = Random.Range(0, 4);                                 //네 꼭지점 중 랜덤 선택                                   

        ray = Camera.main.ScreenPointToRay(v[a_Rnd]);                   //랜덤 선택된 위치로 부터 쏘는 광선

        Vector3 CanvasPos = Vector3.zero;                               //비행기 생성위치(캔버스의 네 꼭지점 좌표를 월드좌표로 담을 변수)
        if (Physics.Raycast(ray, out hitinfo, Mathf.Infinity))
        {
            Vector3 a_Vec = hitinfo.point;
            CanvasPos = a_Vec;
        }
        CanvasPos.y = 50f;

        Vector3 a_Dir = (TargetPos - CanvasPos).normalized;            //가상의 표식과 비행기 사이의 방향벡터
        Vector3 a_Rot = Quaternion.LookRotation(a_Dir).eulerAngles;    //비행기를 회전시킬 각도를 담기
        a_Rot.y += 90.0f;                                              //비행기 프리팹 자체가 -90도 돌아가 있어서 보정해주기

        Instantiate(SkillPrefab, CanvasPos, Quaternion.Euler(a_Rot));   //표식을 바라보도록 비행기 생성
    }
}
