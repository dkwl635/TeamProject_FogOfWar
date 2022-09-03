using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TowerNode : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public TowerInfo twInfo;

    public Text towerName;
    public Image towerImg;
    [HideInInspector] public int maxCount = 5;
    [HideInInspector] public int count = 0;

    public Text countTxt;

    // Start is called before the first frame update
    void Start()
    {
        countTxt.text = count + " / " + maxCount;
    }

    public void SetNode(TowerInfo info)
    {
        twInfo = info;

        towerImg.sprite = SetTowerMgr.inst.towerSprite[info.towerType];
        towerName.text = "Lv." + info.lv +" " + Enum.GetDescription((TowerType)info.towerType);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (count >= maxCount)
            return;

        SetTowerMgr.inst.SpawnTowerModel((TowerType)twInfo.towerType);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetTowerMgr.inst.SetInfoBox(twInfo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetTowerMgr.inst.OffInfoBox();
    }

    public void SetTowerCount(int add)
    {
        count += add;
        countTxt.text = count + " / " + maxCount;
    }
}
