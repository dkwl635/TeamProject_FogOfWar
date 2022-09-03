using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.ComponentModel;

public enum ItemState
{
    BeforeBuy,
    Active,
    MaxLv
}

public class ShopNode : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    [HideInInspector] public int type = 0;
    [HideInInspector] public int lv = 0;
    [HideInInspector] public ItemState itemState = ItemState.BeforeBuy;
    [HideInInspector] public TowerInfo twInfo;
    [HideInInspector] public TankInfo tkInfo;

    public Text lvText;
    public Image iconImg;
    public Text nameText;
    public Text buyText;

    // Start is called before the first frame update
    void Start()
    {
        Button btn = GetComponentInChildren<Button>();
        if(btn != null)
        {
            btn.onClick.AddListener(() =>
            {
                TowerStore sm = FindObjectOfType<TowerStore>();
                if (sm != null)
                    sm.BuyItem(type);
            });
        }
    }

    public void InitData(TowerInfo info)
    {
        twInfo = info;
        type = info.towerType;
        lv = info.lv - 1;
        nameText.text = Enum.GetDescription((TowerType)info.towerType);
        lvText.text = info.lv.ToString();
        buyText.text = info.price.ToString();

        if (StoreManager.Inst.towerSprite.Length > info.towerType)
            iconImg.sprite = StoreManager.Inst.towerSprite[info.towerType];
    }

    public void InitData(TankInfo info)
    {
        tkInfo = info;
        type = info.tankType;
        lv = info.lv - 1;
        nameText.text = Enum.GetDescription((TankType)info.tankType);

        lvText.text = info.lv.ToString();
        buyText.text = info.price.ToString();

        if (StoreManager.Inst.tankSprite.Length > info.tankType)
            iconImg.sprite = StoreManager.Inst.tankSprite[info.tankType];
    }

    public void InitData(SkillType type, int lv)
    {
        this.type = (int)type;
        this.lv = lv;
        nameText.text = Enum.GetDescription(type);
        lvText.text = (lv + 1).ToString();
        buyText.text = (500 + (lv * 50)).ToString();

        if (StoreManager.Inst.skillSprite.Length > (int)type)
            iconImg.sprite = StoreManager.Inst.skillSprite[(int)type];
    }

    public void SetState(ItemState state)
    {
        itemState = state;
        switch(state)
        {
            case ItemState.BeforeBuy:
                lvText.color = new Color32(110, 110, 110, 255);
                iconImg.color = new Color32(255, 255, 255, 120);
                nameText.color = new Color32(180, 180, 180, 255);
                break;
            case ItemState.Active:
                lvText.color = new Color32(0, 0, 0, 255);
                iconImg.color = new Color32(255, 255, 255, 255);
                nameText.color = new Color32(0, 0, 0, 255);
                break;
            case ItemState.MaxLv:
                lvText.text = "Max";
                buyText.text = "Max";
                lvText.color = new Color32(0, 0, 0, 255);
                iconImg.color = new Color32(255, 255, 255, 255);
                nameText.color = new Color32(0, 0, 0, 255);
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (twInfo != null)
            StoreManager.Inst.SetInfo(twInfo);
        else if (tkInfo != null)
            StoreManager.Inst.SetInfo(tkInfo);
        else
            StoreManager.Inst.SetInfo((SkillType)type);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StoreManager.Inst.infoPanel.SetActive(false);
    }

}
