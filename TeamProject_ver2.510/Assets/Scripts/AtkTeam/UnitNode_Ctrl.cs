using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UNodeState
{
    NORMAL, //사용가능 & 선택X
    DISABLE, //사용 불가
    SELECT, //선택 중
    COOL, //쿨타임 중
}

public class UnitNode_Ctrl : MonoBehaviour
{
    Button btn;

    public Image UnitImg;
    public Text CountText;
    public Image UnitSelect;
    public Image CoolImg;

    public int Count;
    float CoolTimer = 1;

    public UNodeState uNodeState = UNodeState.NORMAL;
    public TankType tankType = TankType.Normal;

    // Start is called before the first frame update
    void Start()
    {
        btn = GetComponent<Button>();

        if (btn != null)
            btn.onClick.AddListener(() =>
            {
                if (SpawnCtrl.Inst.isSel)
                    return;

                if (uNodeState != UNodeState.NORMAL)
                    return;

                uNodeState = UNodeState.SELECT;
                SpawnCtrl.Inst.isSel = true;
                UnitSelect.gameObject.SetActive(true);
                SpawnCtrl.Inst.SelUnitInfo(this);
            });
    }

    // Update is called once per frame
    void Update()
    {
        if (Count <= 0)
            uNodeState = UNodeState.DISABLE;

        switch(uNodeState)
        {
            case UNodeState.NORMAL:
                UnitSelect.gameObject.SetActive(false);
                CoolTimer = 1;
                CoolImg.fillAmount = 1;
                break;
            case UNodeState.DISABLE:
                CoolImg.gameObject.SetActive(true);
                UnitSelect.gameObject.SetActive(false);
                break;
            case UNodeState.COOL:
                UnitSelect.gameObject.SetActive(false);
                CoolImg.gameObject.SetActive(true);

                CoolTimer -= Time.deltaTime;
                CoolImg.fillAmount -= Time.deltaTime;
                if (CoolTimer <= 0.0f)
                {
                    CoolImg.gameObject.SetActive(false);

                    if (Count <= 0)
                        uNodeState = UNodeState.DISABLE;

                    uNodeState = UNodeState.NORMAL;
                }
                break;
        }
    }
}
