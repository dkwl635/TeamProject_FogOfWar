using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPosMgr : MonoBehaviour
{
    public GameObject[] TowerPrefabs;
    GameObject[] TowerPos;

    // Start is called before the first frame update
    void Start()
    {
        TowerPos = GameObject.FindGameObjectsWithTag("TowerPos");
        foreach(var tower in SelectEnemyInfo.DicTowerPos)
        {
            foreach (int pos in tower.Value)
            {
                GameObject obj = Instantiate(TowerPrefabs[(int)tower.Key], TowerPos[pos].transform.position, Quaternion.identity);
                obj.GetComponent<TowerController>().SetStat(SelectEnemyInfo.DicTowerInfo[tower.Key]);
            }
        }
    }
}
