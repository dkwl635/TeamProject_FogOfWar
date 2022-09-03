using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneCtrl : MonoBehaviour
{    
    Vector3 MoveDir = Vector3.zero;                  //비행기와 가상표식 사이의 벡터를 담을 변수
    float MoveSpeed = 0.0f;                          //비행기 이동속도    

    //------- 폭격 관련
    Vector3 calcPos = Vector3.zero;
    Vector3 boomPos;             //폭격 이미지 생성할 위치변수        
    float boomTime = 0.02f;                         //폭격 간격

    //------- 폭격 관련        
    LandSignCtrl LSignCtrl;
    public GameObject BoomPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        LSignCtrl = FindObjectOfType<LandSignCtrl>();
        MoveDir = LSignCtrl.TargetPos - transform.position;
        MoveDir.y = 0.0f;
        MoveSpeed = MoveDir.magnitude / 1.5f;       //총쏘는 시간 2초에 맞춰 비행기 속도 조절(표식위치에 닿기 전 속도)
        boomPos = LSignCtrl.TargetPos;
        Destroy(gameObject, 4.0f);                  //4초뒤 비행기 오브젝트 삭제
    }

    // Update is called once per frame
    void Update()
    {
        calcPos = LSignCtrl.TargetPos - transform.position;
        calcPos.y = 0.0f;
        if (calcPos.magnitude <= 3f)
        {
            boomTime -= Time.deltaTime;                     //폭격 발생 간격
            if (boomTime <= 0.0f)         //폭격 최대 9번
            {
                Debug.Log("Bomb!");
                GameObject a_Boom = Instantiate(BoomPrefab, boomPos, Quaternion.identity);
                Destroy(a_Boom, 3f);

                boomTime = 4f;
            }
        }

        Vector3 a_StepVec = (MoveDir.normalized * Time.deltaTime * MoveSpeed);      
        transform.Translate(a_StepVec, Space.World);            //이동속도에 따라 비행기 이동
    }
}
