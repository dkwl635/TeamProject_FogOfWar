using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    // HUD_Canvas가 언제나 카메라를 바라보게 한다
    void LateUpdate()
    { 
        transform.forward = Camera.main.transform.forward;  
    }
}
