using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCtrl : MonoBehaviour
{
    public static SpawnCtrl Inst;
    public GameObject SelMapMark;
    GameObject Mark;

    public GameObject unitNodePrefab;

    public Sprite[] tankIconImg;

    public bool isSel;
    [HideInInspector] public TankType type;

    Ray MousePos;
    RaycastHit hitInfo;

    Vector3 SpawnPos;
    public GameObject[] tankPrefabs;
    UnitNode_Ctrl refNodeCtrl;

    bool isPossible;

    Dictionary<int, TankInfo> tankInfoDic = new Dictionary<int, TankInfo>();

    private void Awake()
    {
        Inst = this;
        tankIconImg = Resources.LoadAll<Sprite>("TankImg");
    }

    // Start is called before the first frame update
    void Start()
    {
        Mark = Instantiate(SelMapMark);
        Mark.gameObject.SetActive(false);

        foreach(var info in DBContainer.tankInfoList)
        {
            if(GlobalValue.TankLvDic[info.tankType] == info.lv)
            {
                TankInfo node = new TankInfo();

                node.tankType = info.tankType;
                node.lv = info.lv;
                node.damage = info.damage;
                node.atkCool = info.atkCool;
                node.maxHP = info.maxHP;
                node.spawnCount = info.spawnCount;

                tankInfoDic.Add(node.tankType, node);
            }
        }

        foreach(var type in GlobalValue.UnitSet)
        {
            if (type < 0)
                continue;

            GameObject node = Instantiate(unitNodePrefab, gameObject.transform);
            UnitNode_Ctrl uc = node.GetComponent<UnitNode_Ctrl>();
            uc.UnitImg.sprite = tankIconImg[type];
            uc.tankType = (TankType)type;
            uc.Count = tankInfoDic[type].spawnCount;
            uc.CountText.text = uc.Count + "/" + tankInfoDic[(int)uc.tankType].spawnCount;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.IsPointerOverUIObject() && isSel)     
        {
            MousePos = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(MousePos, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("SPAWN")))
            {
                Mark.gameObject.SetActive(true);
                SpawnPos = hitInfo.point;
                SpawnPos.y += 3f;
                Mark.transform.position = SpawnPos;

                isPossible = true;
            }

            if (Input.GetMouseButtonDown(0) && isPossible)        
            {
                Mark.SetActive(false);

                GameObject tank = Instantiate(tankPrefabs[(int)refNodeCtrl.tankType], SpawnPos, Quaternion.identity);
                tank.GetComponent<UnitCtrl>().SetStat(tankInfoDic[(int)refNodeCtrl.tankType]);

                refNodeCtrl.uNodeState = UNodeState.COOL;
                refNodeCtrl.Count--;
                refNodeCtrl.CountText.text = refNodeCtrl.Count + "/" + tankInfoDic[(int)refNodeCtrl.tankType].spawnCount;

                refNodeCtrl = null;
                isSel = false;
            }
        }
    }

    public void SelUnitInfo(UnitNode_Ctrl node)
    {
        refNodeCtrl = node;
    }
}