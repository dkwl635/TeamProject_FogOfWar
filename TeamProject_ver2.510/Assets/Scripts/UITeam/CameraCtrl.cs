using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//InGame 씬의 카메라 위치를 조작하는 스크립트
public class CameraCtrl : MonoBehaviour
{
    float h = 0f;
    float v = 0f;

    float moveSpeed = 40f;

    float cameraSize = 0f; // 현재 카메라 사이즈
    float zoomSpeed = 5f; // 줌인, 줌아웃 속도

    float maxSize = 75f; // 화면을 최대한 축소했을 때 카메라 사이즈
    float minSize = 30f; // 화면을 최대한 확대했을 때 카메라 사이즈

    public Transform viewRect = null; // 미니맵 카메라 뷰포트 이미지(흰색 사각형)의 transform
    Vector3 viewRectSize = Vector3.zero; //미니맵 카메라 뷰포트 이미지의 오리지널 사이즈

    Vector3 camPos = Vector3.zero; //카메라의 위치를 계산할 변수
    Vector3 maxPos = new Vector3(134, 100, 60); //맵의 우측 상단 끝 좌표
    Vector3 minPos = new Vector3(-241, 100, -235); //맵의 좌측 하단 끝 좌표

    Vector3 minVPos = Vector3.zero; //뷰포트 좌하단 좌표
    Vector3 maxVPos = Vector3.zero; //뷰포트 우상단 좌표

    Vector3 maxLimit = Vector3.zero; // 카메라 중심이 위치할 수 있는 우측 상단 끝 좌표
    Vector3 minLimit = Vector3.zero; // 카메라 중심이 위치할 수 있는 좌측 하단 끝 좌표

    //모바일용 변수

    void Start()
    {
        viewRectSize = viewRect.localScale;
    }

    void LateUpdate()
    {
        //DragMove와 MoveLimit가 연산된 후에 transform.position이 결정된다.
        camPos = transform.position;
        MouseDragMove();
        CamMoveLimit();
        transform.position = camPos;

        CameraZoom();
    }

    //마우스 드래그로 카메라를 이동
    void MouseDragMove()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // 마우스 축 방향에 따라 이동할 값을 계산
        // 마우스 좌측 클릭 시
        if(Input.GetMouseButton(1))
        {
            h += Input.GetAxis("Mouse X") / moveSpeed;
            v += Input.GetAxis("Mouse Y") / moveSpeed;

            camPos += new Vector3(h, 0f, v);
        }
        else
        {
            h = 0f;
            v = 0f;
        }
#elif UNITY_IOS || UNITY_ANDROID
        
#endif
        // 계산된 위치로 이동
        
    }

    //맵 범위 바깥으로 카메라가 나가는 것을 방지
    void CamMoveLimit()
    {
        //뷰포트 양 끝단의 월드 좌표를 받아온다.
        minVPos = Camera.main.ViewportToWorldPoint(Vector3.zero);
        maxVPos = Camera.main.ViewportToWorldPoint(Vector3.one);

        //좌표가 도달할 수 있는 최대 위치를 계산한다.
        maxLimit.x = maxPos.x - (maxVPos.x - minVPos.x) / 2f;//우측
        maxLimit.z = maxPos.z - (maxVPos.z - minVPos.z) / 2f;//상단
        minLimit.x = minPos.x + (maxVPos.x - minVPos.x) / 2f;//좌측
        minLimit.z = minPos.z + (maxVPos.z - minVPos.z) / 2f;//하단

        if (camPos.x > maxLimit.x)
            camPos.x = maxLimit.x;
        else if (camPos.x < minLimit.x)
            camPos.x = minLimit.x;

        if (camPos.z > maxLimit.z)
            camPos.z = maxLimit.z;
        else if (camPos.z < minLimit.z)
            camPos.z = minLimit.z;
    }

    //카메라 줌인 줌아웃(동시에 미니맵에 표시될 뷰포트 사각형의 스케일 조절)
    void CameraZoom()
    {
        cameraSize = Camera.main.orthographicSize;

#if UNITY_EDITOR || UNITY_STANDALONE
        //마우스 휠을 아래로 스크롤하면
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && cameraSize < maxSize)
        {
            cameraSize += zoomSpeed; //카메라 사이즈가 커진다 == 줌아웃
        }
        //마우스 휠을 위로 스크롤하면
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && cameraSize > minSize)
        {
            cameraSize -= zoomSpeed; //카메라 사이즈가 작아진다 == 줌인
        }
#elif UNITY_IOS || UNITY_ANDROID
        
#endif
        //카메라 사이즈 변경
        Camera.main.orthographicSize = cameraSize;

        //미니맵 카메라 뷰포트 이미지(흰색 사각형)의 스케일 변경
        viewRect.localScale = new Vector3(viewRectSize.x * cameraSize / 50,
                                          viewRectSize.y * cameraSize / 50, 1f);
        viewRect.position = new Vector3(transform.position.x, transform.position.y - 40, transform.position.z + 68);
    }
}