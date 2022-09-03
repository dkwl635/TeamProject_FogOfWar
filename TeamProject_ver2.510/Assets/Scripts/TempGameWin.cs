using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGameWin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Unit"))
        {
            GameManager.Inst.PlayerWinLose(true);
        }
    }
}
