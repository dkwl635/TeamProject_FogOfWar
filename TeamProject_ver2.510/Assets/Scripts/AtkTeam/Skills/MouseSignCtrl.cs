using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSignCtrl : MonoBehaviour
{    
    //----- 마우스 위치를 따라가도록 하기위한 변수
    Ray MousePos;
    RaycastHit hitInfo;
    //----- 마우스 위치를 따라가도록 하기위한 변수   

    public GameObject TargetMark;
    // Update is called once per frame
    void Update()
    {
        if (!GameManager.IsPointerOverUIObject())     //스킬 버튼 UI쪽 아래로 표식이 갈수 있도록 하려고했는데, 현재 안됨
        {
            MousePos = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(MousePos, out hitInfo, Mathf.Infinity))
            {
                Vector3 hitPos = hitInfo.point;
                hitPos.y += 3f;                                                   //지형때문에 표식을 5정도 위로 올림
                transform.position = hitPos;                                   //레이저 쏜 위치로 표식위치 설정
            }
        }

        if (Input.GetMouseButtonDown(0))        //마우스 왼쪽버튼 누르면 스킬동작
        {
            GameObject go = Instantiate(TargetMark,        //표식 재생성(바닥에) 
                              transform.position, Quaternion.Euler(90, 0, 0));
            go.transform.localScale = new Vector3(5, 5, 1);
            go.GetComponent<MouseSignCtrl>().enabled = false;
            go.GetComponent<LandSignCtrl>().enabled = true;//마우스 따라다니는 표식일 때의 동작 안하도록 스크립트 정지
            if (gameObject.name.Contains("Shield") != true)  //실드 스킬이 아닐때만
                Destroy(go, 4f);
            else
                Destroy(go, 0.1f);              //실드스킬일 때

            Destroy(gameObject);                                                       //마우스 따라다니는 표식 삭제            
        }
        else if (Input.GetMouseButtonDown(1))   //마우스 오른쪽버튼 누르면 취소
        {
            Destroy(gameObject);
        }
    }
}