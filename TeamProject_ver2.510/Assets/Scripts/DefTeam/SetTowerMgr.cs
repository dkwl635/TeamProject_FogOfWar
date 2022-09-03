using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SetTowerMgr : MonoBehaviour
{
    public static SetTowerMgr inst;

    //타워 모델 프리팹
    public List<GameObject> towerModelList = new List<GameObject>();
    public Sprite[] towerSprite;
    //현재 들고 있는 타워 오브젝트
    SetTower dragobj = null;
    //위치 계산용
    private Vector3 pos;

    //전체 타워 UI
    public GameObject setTowerUI;

    [Header("UI")]
    public Text countTxt;
    public Text costTxt;

    [Header("LogBox Info")]
    public GameObject InfoPanel;
    public Image towerImg;
    public Text statusTxt;
    public Text infoTxt;

    //타워 설치 위치를 담을 배열
    GameObject[] spawnPoint;

    //타워 타입별 위치 딕셔너리
    static Dictionary<TowerType, List<int>> DictowerPos = new Dictionary<TowerType, List<int>>();

    //타워 설치용 UI 
    [Header("TowerNode UI")]
    public Transform content = null;
    public GameObject nodeObj = null;

    int maxTower = 15;   //최대 설치 가능 타워
    int maxCost = 99;    //최대 설치 가능 코스트
    int curTowerCount = 0; //현재 설치 된 타워 갯수
    int curTowerCost = 0; //현재 설치 된 타워 코스트
    TowerType curTowerType;   //현재 설치 중인 타워 타입

    //UI에 등록된 설치용 노드 입니다.
    Dictionary<TowerType, TowerNode> towerNodeDic = new Dictionary<TowerType, TowerNode>();

    public bool bRetry = false; //재설치 중인지

    public Button saveBtn;
    public Button exitBtn;

    private string UpdateMyDefSetUrl = "http://pmaker.dothome.co.kr/pMaker_7Gi/UpdateMyDefSet.php";

    private void Awake()
    {
        inst = this;
        InitTower();

        spawnPoint = GameObject.FindGameObjectsWithTag("TowerPos");
    }

    void InitTower()
    {
        towerSprite = Resources.LoadAll<Sprite>("DefTeam/TowerImg");

        //DBContainer.towerInfoList; -> 타워 DB 상의 모든 정보
        //GlobalValue.TowerLvDic; -> 내가 가진 타워 Lv
        foreach (TowerInfo info in DBContainer.towerInfoList)
        {
            if (!GlobalValue.myInfo.TowerLvDic.ContainsKey(info.towerType))
            {
                GlobalValue.myInfo.TowerLvDic.Add(info.towerType, 0);
            }
            //보유 타워의 레벨과 타입이 일치하는 경우에
            //UI 노드를 인스턴스화 하고 노드를 딕셔너리에 추가한다.
            //UI 노드 자체에 TowerInfo를 넣어서 바로 접근 가능.
            if (GlobalValue.myInfo.TowerLvDic[info.towerType] == info.lv)
            {
                GameObject node = Instantiate(nodeObj, content);
                TowerNode towerNode = node.GetComponent<TowerNode>();

                towerNode.SetNode(info);
                towerNodeDic.Add((TowerType)info.towerType, towerNode);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadDefDeck();

        //버튼 초기화
        if (saveBtn != null)
            saveBtn.onClick.AddListener(() =>
            {
                GlobalValue.myInfo.mydefset = JsonMgr.DefDeckToStr(DictowerPos);
                UpdateMyDefSet();
            });

        if (exitBtn != null)
            exitBtn.onClick.AddListener(() => { LoadingManager.Instance.LoadScene("Lobby"); });

        SoundManager.Instance.PlayBGM("SetDefBGM");

        SetText();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ReferenceEquals(dragobj, null))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//현재 마우스 위치 값을 입력 받습니다.
            RaycastHit rayhit;
            if (Physics.Raycast(ray, out rayhit, Mathf.Infinity))
            {
                if (Input.GetMouseButton(0))
                {
                    if (rayhit.collider.CompareTag("TowerPos"))
                    {
                        pos = rayhit.collider.transform.position;
                        dragobj.transform.position = pos;
                    }
                    else
                    {
                        pos = rayhit.point;
                        pos.y += 1.0f;
                        dragobj.transform.position = pos;
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (rayhit.collider.CompareTag("TowerPos") && (curTowerCost + towerNodeDic[curTowerType].twInfo.cost <= maxCost))
                    {
                        SettingTower(rayhit.collider.gameObject);
                    }
                    else
                    {
                        if (bRetry)
                        {
                            curTowerCount--;
                            towerNodeDic[curTowerType].SetTowerCount(-1);
                            curTowerCost -= towerNodeDic[curTowerType].twInfo.cost;

                            DictowerPos[curTowerType].Remove(dragobj.pointIdx);
                            if (DictowerPos[curTowerType].Count <= 0)
                                DictowerPos.Remove(curTowerType);

                            bRetry = false;
                        }

                        //기존 레이어 돌려 놓기
                        spawnPoint[dragobj.pointIdx].layer = 0;

                        //들고 있는 타워 삭제
                        Destroy(dragobj.gameObject);
                        dragobj = null;
                        OnTowerUI();

                        SetText();
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//현재 마우스 위치 값을 입력 받습니다.
                RaycastHit rayhit;
                if (Physics.Raycast(ray, out rayhit, Mathf.Infinity))
                {
                    if (rayhit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("TOWERS")))
                    {
                        RetryTowerSet(rayhit.collider.gameObject);
                    }
                }
            }
        }
    }

    public void SpawnTowerModel(TowerType towertype)
    {
        if (curTowerCount >= maxTower) //최대 설치 넣음
        {
            Debug.Log("최대 건설 갯수를 초과 했습니다.");
            return;
        }

        dragobj = Instantiate(towerModelList[(int)towertype]).GetComponent<SetTower>();

        dragobj.gameObject.layer = 2;
        curTowerType = towertype;

        dragobj.GetComponent<SetTower>().towertype = towertype;
        dragobj.arOn();
        OffTowerUI();
    }

    void RetryTowerSet(GameObject obj)
    {
        dragobj = obj.GetComponent<SetTower>();
        dragobj.gameObject.layer = 2;
        OffTowerUI();

        dragobj.arOn();
        spawnPoint[dragobj.pointIdx].gameObject.layer = 0;
        curTowerType = dragobj.towertype;
        bRetry = true;

        SetText();
    }

    void SettingTower(GameObject posObj)
    {
        pos = posObj.transform.position;
        pos.y += 1.5f;
        dragobj.transform.position = pos;
        dragobj.gameObject.layer = LayerMask.NameToLayer("TOWERS");

        int towerPosIdx = System.Array.IndexOf(spawnPoint, posObj);

        if (!bRetry) //현재 다시 설치하는게 아니라면 (위치 변경이 아니라면)
        {
            //새로 추가 하기
            if (!DictowerPos.ContainsKey(curTowerType))
            {
                List<int> pos = new List<int>();
                pos.Add(towerPosIdx);
                DictowerPos.Add(curTowerType, pos);
            }
            else
                DictowerPos[curTowerType].Add(towerPosIdx);

            dragobj.pointIdx = towerPosIdx;

            curTowerCount++;
            curTowerCost += towerNodeDic[curTowerType].twInfo.cost;
            towerNodeDic[curTowerType].SetTowerCount(1);

        }
        else
        {
            //기존 위치 변경
            for (int i = 0; i < DictowerPos[curTowerType].Count; i++)
            {
                if (DictowerPos[curTowerType][i].Equals(dragobj.pointIdx))
                {
                    DictowerPos[curTowerType][i] = towerPosIdx;
                    dragobj.pointIdx = towerPosIdx;
                }
            }

            bRetry = false;
        }

        posObj.layer = 2;
        dragobj.arOff();
        dragobj = null;
        OnTowerUI();
        SetText();
    }

    void OnTowerUI()
    {
        setTowerUI.SetActive(true);
        InfoPanel.SetActive(false);
    }

    void OffTowerUI()
    {
        setTowerUI.SetActive(false);
    }

    public void SetInfoBox(TowerInfo info)
    {
        InfoPanel.SetActive(true);

        statusTxt.text = Enum.GetDescription((TowerType)info.towerType) + "\n\n";
        statusTxt.text += "공격 : " + info.towerdamage + "\n";
        statusTxt.text += "공격속도 : " + info.attackcycle + "\n";
        statusTxt.text += "체력 : " + info.towerHP + "\n";
        towerImg.sprite = towerSprite[info.towerType];

        switch((TowerType)info.towerType)
        {
            case TowerType.Rocket:
                infoTxt.text = "로켓을 발사하는 가장 기본적인 타워입니다";
                break;
            case TowerType.MG:
                infoTxt.text = "공격력은 약하지만 공격 속도가 빠른 타워입니다";
                break;
            case TowerType.Laser:
                infoTxt.text = "빠른 공격속도의 레이저를 발사하는 타워입니다";
                break;
            case TowerType.Fire:
                infoTxt.text = "넓은 범위에 화염을 발사하는 타워입니다";
                break;
            case TowerType.Buff:
                infoTxt.text = "주변 타워의 공격력을 상승시키는 타워입니다";
                break;
            case TowerType.Super:
                infoTxt.text = "많은 미사일을 발사하는 가장 강력한 타워입니다";
                break;
        }
    }
    public void OffInfoBox()
    {
        InfoPanel.SetActive(false);
    }

    void SetText()
    {
        countTxt.text = "설치 타워 : " + curTowerCount + " / " + maxTower;
        costTxt.text = "설치 코스트 : " + curTowerCost + " / " + maxCost;
    }

    void LoadDefDeck()
    {
        DictowerPos.Clear();
        DictowerPos = JsonMgr.DefDeckToDic(GlobalValue.myInfo.mydefset);
        // 각 위치에 타워 설치
        foreach (var item in DictowerPos)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                //타입별 프리팹 생성
                SetTower setTower = Instantiate(towerModelList[(int)item.Key]).GetComponent<SetTower>();

                //타입 지정
                setTower.towertype = item.Key;
                //저장된 포인트에서 오브젝트 위치 지정
                pos = spawnPoint[item.Value[i]].transform.position;
                pos.y += 1.0f;
                setTower.gameObject.transform.position = pos;
                setTower.gameObject.layer = LayerMask.NameToLayer("TOWERS");

                setTower.pointIdx = item.Value[i];

                //사거리 표시 끄기
                setTower.arOff();

                //카운트 해주기
                curTowerCount++;
                curTowerCost += towerNodeDic[item.Key].twInfo.cost;

                towerNodeDic[item.Key].SetTowerCount(1);

                //타워 발판 레이어 변경
                spawnPoint[item.Value[i]].gameObject.layer = 2;
            }
        }
        //텍스트 갱신
        SetText();
    }

    void UpdateMyDefSet()
    {
        StartCoroutine(UpdateMyDefSetCo());
    }

    IEnumerator UpdateMyDefSetCo()
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_id", GlobalValue.myInfo.userID, System.Text.Encoding.UTF8);
        form.AddField("Input_mydefset", GlobalValue.myInfo.mydefset);

        UnityWebRequest a_www = UnityWebRequest.Post(UpdateMyDefSetUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sReturn = enc.GetString(a_www.downloadHandler.data);
            if (sReturn.Contains("OK_"))
            {
                LoadingManager.Instance.LoadScene("Lobby");
            }
        }
        else
        {
            Debug.Log(a_www.error);
        }
    }
}