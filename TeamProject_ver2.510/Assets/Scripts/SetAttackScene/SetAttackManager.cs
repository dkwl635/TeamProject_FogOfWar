using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SelectNode
{
    public RawImage Img;
    public SlotType Type;
    public int TypeIdx;
}

public class SetAttackManager : MonoBehaviour
{
    private const string LoadMyAttSet = "http://pmaker.dothome.co.kr/pMaker_7Gi/LoadMyAttSet.php";
    private const string SaveMyAttSet = "http://pmaker.dothome.co.kr/pMaker_7Gi/SaveMyAttSet.php";

    public static SetAttackManager inst;
    public SelectNode m_SelectNode = new SelectNode();

    public Texture[] tankTex;
    public Texture[] skillTex;

    public Canvas Canvas;
    public Text infoNameText;
    public Text infoStatText;
    public Text infoHelpText;

    [Header("Select Item Prefab")]
    public GameObject Skill_PF;
    public GameObject Unit_PF;

    [Header("Selected Item Slot")]
    public Transform[] SkillSlots = new Transform[4];
    public Transform[] UnitSlots = new Transform[4];

    [Header("Select Item")]
    public GameObject SelectUnitPanel;
    public GameObject SelectSkillPanel;
    public GameObject Skill_SvContent;
    public GameObject Unit_SvContent;
    public Button ChangeSelectPanelBtn;
    float tabSpeed = 600f;
    Vector3 UnitMvPos;
    Vector3 SkillMvPos;

    [Header("Etc Prefab")]
    public GameObject ConfigBox;
    public GameObject DlgBox;

    [Header("Etc Btn")]
    public Button StartBtn;
    public Button ConfigBtn;
    public Button ExitBtn;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadSetAttack();

        SkillMvPos = SelectSkillPanel.transform.position;
        UnitMvPos = SelectUnitPanel.transform.position;

        if (!ReferenceEquals(ChangeSelectPanelBtn, null))
            ChangeSelectPanelBtn.onClick.AddListener(ChangeSelectPanel);

        if (!ReferenceEquals(StartBtn, null))
            StartBtn.onClick.AddListener(StartBtn_Click);

        if (!ReferenceEquals(ConfigBtn, null))
            ConfigBtn.onClick.AddListener(ConfigBtnClick);

        if (!ReferenceEquals(ExitBtn, null))
            ExitBtn.onClick.AddListener(() => 
            {
                SelectEnemyInfo.SetData();
                LoadingManager.Instance.LoadScene("Lobby"); 
            });

        tankTex = Resources.LoadAll<Texture>("TankImg");

        foreach (TankInfo info in DBContainer.tankInfoList)
        {
            if(GlobalValue.TankLvDic[info.tankType] == info.lv)
            {
                GameObject TankNode = Instantiate(Unit_PF, Unit_SvContent.transform);
                SelectNodeCtrl node = TankNode.GetComponent<SelectNodeCtrl>();

                node.Img.texture = tankTex[info.tankType];
                node.lvText.text = "Lv." + info.lv.ToString();
                node.MyItem.TypeIdx = info.tankType;
                node.MyItem.Type = SlotType.Unit;

                node.infoNameStr = Enum.GetDescription((TankType)info.tankType);
                node.infoStatStr = "공격 : " + info.damage + "\n체력 : " + info.maxHP + "\n공격주기 : " + info.atkCool;
                switch((TankType)info.tankType)
                {
                    case TankType.Normal:
                        node.infoHelpStr = "특징이 없는 기본 탱크입니다";
                        break;
                    case TankType.Missile:
                        node.infoHelpStr = "먼 거리에서 미사일을 발사하는 탱크입니다";
                        break;
                    case TankType.Fire:
                        node.infoHelpStr = "적을 불태우는 화염을 발사하는 탱크입니다";
                        break;
                    case TankType.MG:
                        node.infoHelpStr = "기관총을 발사하는 탱크입니다";
                        break;
                    case TankType.Turret:
                        node.infoHelpStr = "여러개의 포탑을 가지고 있는 탱크입니다";
                        break;
                    case TankType.Scout:
                        node.infoHelpStr = "이동속도가 빠르고 파괴시 자폭하는 탱크입니다";
                        break;
                    case TankType.Stealth:
                        node.infoHelpStr = "주기적으로 투명화 스킬을 사용하는 탱크입니다";
                        break;
                    case TankType.Buff:
                        node.infoHelpStr = "일시적으로 주변 탱크의 공격력과 이동속도를 강화하는 탱크입니다";
                        break;
                }
            }
        }

        skillTex = Resources.LoadAll<Texture>("SkillImg");
        foreach(var node in GlobalValue.SkillLvDic)
        {
            if (node.Value <= 0)
                continue;

            GameObject SkillNode = Instantiate(Skill_PF, Skill_SvContent.transform);
            SelectNodeCtrl snc = SkillNode.GetComponent<SelectNodeCtrl>();
            snc.Img.texture = skillTex[(int)node.Key];
            snc.MyItem.TypeIdx = (int)node.Key;
            snc.MyItem.Type = SlotType.Skill;

            snc.infoNameStr = Enum.GetDescription(node.Key);
            switch(node.Key)
            {
                case SkillType.Plane:
                    snc.infoHelpStr = "폭격기를 보내 타워를 공격하는 스킬입니다";
                    break;
                case SkillType.Shield:
                    snc.infoHelpStr = "타워의 공격을 방어하는 보호막을 설치하는 스킬입니다";
                    break;
                case SkillType.Flare:
                    snc.infoHelpStr = "어둠으로 둘러싸인 영역을 밝히는 스킬입니다";
                    break;
            }
        }

        EnemyInfo();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ChangeSelectPanel();

        TabMoveCtrl();
    }

    void StartBtn_Click()
    {
        SaveSetAttack();
        string msg = "확인버튼을 누를 경우 게임이 시작됩니다..\n시작 하겠습니까?";
        DlgBoxCtrl.OK_Act ActFunc = GameStart;
        GameObject a_DlgBoxObj = (GameObject)Instantiate(DlgBox);
        a_DlgBoxObj.GetComponent<DlgBoxCtrl>().InitData("정보", msg, ActFunc);
        a_DlgBoxObj.transform.SetParent(Canvas.transform, false);
    }

    public void GameStart()
    {
        LoadingManager.Instance.LoadScene("InGame");
    }

    // Node쪽에 마우스가 올려져 있을 경우 정보내용을 우측에 보여주기 위해 넘겨주어야할 함수
    public void SelectItemInfoView(string Msg)
    {
        infoHelpText.text = Msg;
    }

    void ConfigBtnClick()
    {
        GameObject a_ConfigBox = Instantiate(ConfigBox);
        a_ConfigBox.transform.SetParent(Canvas.transform, false);
    }

    void LoadSetAttack()
    {
        StartCoroutine(LoadSetAttackCo());
    }

    void SaveSetAttack()
    {
        StartCoroutine(SaveSetAttackCo());
    }

    IEnumerator LoadSetAttackCo()
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_id", GlobalValue.myInfo.userID, System.Text.Encoding.UTF8);

        UnityWebRequest a_www = UnityWebRequest.Post(LoadMyAttSet, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sReturn = enc.GetString(a_www.downloadHandler.data);
            if (sReturn.Contains("OK_"))
            {
                GlobalValue.UnitSet = JsonMgr.AttDeckToArr(SlotType.Unit, sReturn);
                GlobalValue.SkillSet = JsonMgr.AttDeckToArr(SlotType.Skill, sReturn);

                GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
                GameObject[] skills = GameObject.FindGameObjectsWithTag("Skill");

                for(int i=0; i< GlobalValue.UnitSet.Length;i++)
                {
                    if (GlobalValue.UnitSet[i] < 0)
                        continue;

                    SlotCtrl sc = units[i].GetComponent<SlotCtrl>();

                    sc.img.texture = tankTex[GlobalValue.UnitSet[i]];
                    sc.img.color = Color.white;

                    sc.InMyItem.Type = SlotType.Unit;
                    sc.InMyItem.TypeIdx = GlobalValue.UnitSet[i];
                    //Debug.Log(sc.InMyItem.Type);
                }
                for(int i=0; i< GlobalValue.SkillSet.Length;i++)
                {
                    if (GlobalValue.SkillSet[i] < 0)
                        continue;

                    SlotCtrl sc = skills[i].GetComponent<SlotCtrl>();

                    sc.img.texture = skillTex[GlobalValue.SkillSet[i]];
                    sc.img.color = Color.white;

                    sc.InMyItem.Type = SlotType.Skill;
                    sc.InMyItem.TypeIdx = GlobalValue.SkillSet[i];
                    //Debug.Log(sc.InMyItem.Type);
                }
            }
        }
        else
        {
            Debug.Log(a_www.error);
        }
    }

    IEnumerator SaveSetAttackCo()
    {
        WWWForm form = new WWWForm();
        string MyAttSet =  "{"+JsonMgr.AttDeckToStr(GlobalValue.SkillSet) + "," +
                               JsonMgr.AttDeckToStr(GlobalValue.UnitSet) +"}";
        form.AddField("Input_id", GlobalValue.myInfo.userID, System.Text.Encoding.UTF8);
        form.AddField("Input_myattset", MyAttSet, System.Text.Encoding.UTF8);

        UnityWebRequest a_www = UnityWebRequest.Post(SaveMyAttSet, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sReturn = enc.GetString(a_www.downloadHandler.data);
            if(sReturn.Contains("OK"))
            {
                Debug.Log("SaveSuccess");
            }
        }
        else
        {
            Debug.Log(a_www.error);
        }
    }

    public void SetInfo(string name,string stat, string helpStr)
    {
        infoNameText.text = name;
        infoStatText.text = stat;
        infoHelpText.text = helpStr;
    }

    public void EnemyInfo()
    {
        string HelpStr = "";
        foreach (var pair in SelectEnemyInfo.DicTowerPos)
        {
            HelpStr += string.Format($"{Enum.GetDescription(pair.Key)} : {pair.Value.Count}\n");
        }
        infoNameText.text = "적 정보";
        infoStatText.text = "\n"+ HelpStr;
        infoHelpText.text = "";
    }

    #region Skill/TankTabGroup
    void TabMoveCtrl()
    {
        SelectUnitPanel.transform.position = Vector3.MoveTowards(SelectUnitPanel.transform.position, UnitMvPos, tabSpeed * Time.deltaTime);
        SelectSkillPanel.transform.position = Vector3.MoveTowards(SelectSkillPanel.transform.position, SkillMvPos, tabSpeed * Time.deltaTime);
    }

    void ChangeSelectPanel()
    {
        Vector3 temp = UnitMvPos;
        UnitMvPos = SkillMvPos;
        SkillMvPos = temp;
        int s = SelectSkillPanel.transform.GetSiblingIndex();
        int u = SelectUnitPanel.transform.GetSiblingIndex();

        SelectSkillPanel.transform.SetSiblingIndex(u);
        SelectUnitPanel.transform.SetSiblingIndex(s);
    }
    #endregion
}
