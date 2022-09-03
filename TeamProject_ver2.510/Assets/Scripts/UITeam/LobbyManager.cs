using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using System.Linq;
using UnityEngine.EventSystems;

public class LobbyManager : MonoBehaviour
{
    public Canvas Canvas;
    public Button LogoutBtn;
    public Button ConfigBtn;
    public Button DefSetBtn;
    public Button StoreBtn;

    public Button MatchRefreshBtn;

    public GameObject ConfigBox;

    private string LoadMyRankUrl = "http://pmaker.dothome.co.kr/pMaker_7Gi/LoadMyRank.php";
    private string GetRankListUrl = "http://pmaker.dothome.co.kr/pMaker_7Gi/FindMatchPlayer.php";
    List<UserInfo> MatchPlayerList = new List<UserInfo>();

    public Text MatchListRefreshTimeTxt;
    private float MatchListRefreshTime = 181.0f;

    public GameObject ScrollView;
    private PlayerListNodeCtrl[] ArrPlayerNode;

    [Header("MyInfo")]
    public Text myInfoText;
    public Text userGoldText;
    public Text rankText;
    public Text rank_pointText;
    public Image rankImg;
    public Button profileChangeBtn;
    public GameObject profileSV;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f; //일시정지 된 상황에서 넘어오면 다시 원상복구 해야 함

        if (LogoutBtn != null)
            LogoutBtn.onClick.AddListener(() =>
            {
                GlobalValue.myInfo.ClearUserInfo();

                GlobalValue.userGold = 0;
                GlobalValue.TankLvDic.Clear();
                GlobalValue.SkillLvDic.Clear();
                LoadingManager.Instance.LoadScene("Title");
            });

        if (DefSetBtn != null)
            DefSetBtn.onClick.AddListener(() =>
            {
                LoadingManager.Instance.LoadScene("SetDefence");
            });

        if (StoreBtn != null)
            StoreBtn.onClick.AddListener(() =>
            {
                LoadingManager.Instance.LoadScene("Store");
            });

        if (ConfigBtn != null)
            ConfigBtn.onClick.AddListener(ConfigBtnClick);

        if (MatchRefreshBtn != null)
            MatchRefreshBtn.onClick.AddListener(GetLeaderboard);

        if (ScrollView != null)
        {
            if (ArrPlayerNode == null || ArrPlayerNode.Length <= 0)
                ArrPlayerNode = ScrollView.GetComponentsInChildren<PlayerListNodeCtrl>();
        }

        //프로필 이미지 교체 버튼 클릭 시
        if (profileChangeBtn != null)
        {
            profileChangeBtn.transform.Find("ProfileImg").GetComponent<Image>().sprite = Resources.Load<Sprite>(string.Format("ProfileImage/Character{0:00}", GlobalValue.myInfo.profileIdx));
            EventTrigger eventTrigger = profileChangeBtn.gameObject.GetComponent<EventTrigger>();
            EventTrigger.Entry entry_PointerDown = new EventTrigger.Entry();
            entry_PointerDown.eventID = EventTriggerType.PointerDown;
            entry_PointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
            eventTrigger.triggers.Add(entry_PointerDown);
        }

        SoundManager.Instance.PlayBGM("LobbyBGM");

        GetLeaderboard(); 
    }

    // Update is called once per frame
    void Update()
    {
        MatchListRefreshTime -= Time.deltaTime;
        MatchListRefreshTimeTxt.text = string.Format($"<color=#0000FF>리스트 갱신까지</color> {System.TimeSpan.FromSeconds(MatchListRefreshTime).ToString(@"mm\:ss")}");
        if (MatchListRefreshTime <= 0.0f)
        {
            GetLeaderboard();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.currentSelectedGameObject == null || 
                !EventSystem.current.currentSelectedGameObject.name.Contains("Profile"))
                profileSV.GetComponent<ProfileScrollView>().isSelected = false;
        }
    }

    void ConfigBtnClick()
    {
        GameObject a_ConfigBox = Instantiate(ConfigBox);
        a_ConfigBox.transform.SetParent(Canvas.transform, false);
    }

    void GetLeaderboard()  //순위 불러오기
    {
        // 현재 가지고있는 리스트를 서버에서 반복적으로 가져오는 것을 방지하기위해
        // 현재 플레이어의 Win / Lose를 비교하여 위로 10, 아래로 10명 총 20명의 리스트를 들고 온다
        // 20명중 5명을 랜덤으로 ScrollView에 표시
        // 이후 Refresh시 보여줬던 인원을 제외한 나머지 인원 5명을 변경 (Sql서버 연결을 최소화 하기위해)
        // 현재 가지고 있는 MatchPlayerList에서 안보여줬던 인원 5명 미만일 경우 다시 서버에서 가져온다.
        int CheckShowCnt = MatchPlayerList.Where(info => info.IsShow == false).Count();
        if (CheckShowCnt < 5)
            StartCoroutine(GetMatchPlayerListCo());
        else
            RefreshMatchPlayerNode();

        MatchListRefreshTime = 181.0f; // 갱신 후 Timer는 초기화
    }

    IEnumerator GetMatchPlayerListCo()
    {
        WaitingPanel.Instance.SetActive(true);

        WWWForm form = new WWWForm();
        form.AddField("Input_id", GlobalValue.myInfo.userID, System.Text.Encoding.UTF8);

        UnityWebRequest a_www = UnityWebRequest.Post(GetRankListUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sReturn = enc.GetString(a_www.downloadHandler.data);
            if (sReturn.Contains("OK_"))
                RecMatchPlayerList(sReturn);
        }
        else
        {
            Debug.Log(a_www.error);
        }

        WaitingPanel.Instance.SetActive(false);
    }

    void RecMatchPlayerList(string strJsonData)
    {
        if (strJsonData.Contains("RkList") == false)
            return;

        MatchPlayerList.Clear();

        var N = JSON.Parse(strJsonData);

        UserInfo a_UserNd;
        for (int i = 0; i < N["RkList"].Count; i++)
        {
            string UserID = N["RkList"][i]["User_ID"];
            string NickName = N["RkList"][i]["NickName"];
            int WinCnt = N["RkList"][i]["Win"].AsInt;
            int LoseCnt = N["RkList"][i]["Lose"].AsInt;
            int Ranking = N["RkList"][i]["Rank"].AsInt;
            int Score = N["RkList"][i]["UserScore"].AsInt;
            int ProfileIdx = N["RkList"][i]["ProfileImgIdx"].AsInt;
            string defSet = N["RkList"][i]["MyDefSet"];
            string jsonStr = N["RkList"][i]["TowerLv"];
            
            a_UserNd = new UserInfo();
            a_UserNd.userID = UserID;
            a_UserNd.nickName = NickName;
            a_UserNd.winCnt = WinCnt;
            a_UserNd.loseCnt = LoseCnt;
            a_UserNd.score = Score;
            a_UserNd.ranking = Ranking;
            a_UserNd.profileIdx = ProfileIdx;
            a_UserNd.mydefset = defSet;

            if (!string.IsNullOrEmpty(jsonStr) && jsonStr.Contains("TowerLv"))
            {
                var N2 = JSON.Parse(jsonStr);
                for (int j = 0; j < N2["TowerLv"].Count; j++)
                {
                    a_UserNd.TowerLvDic.Add(j, N2["TowerLv"][j]);
                }
            }
            MatchPlayerList.Add(a_UserNd);
        }

        RefreshMyInfo();
        RefreshMatchPlayerNode();
    }

    void RefreshMatchPlayerNode()
    {
        int ShowCnt = 0;
        while(ShowCnt < ArrPlayerNode.Length)
        {
            int RandIdx = Random.Range(0, MatchPlayerList.Count);
            if (MatchPlayerList[RandIdx].IsShow == false)
            {
                MatchPlayerList[RandIdx].IsShow = true;
                ArrPlayerNode[ShowCnt].InitNode(MatchPlayerList[RandIdx]);
                ShowCnt++;
            }

            if (ShowCnt >= MatchPlayerList.Count)
                break;
        }
    }

    //접속자의 닉네임, 승패, 골드 등의 정보 갱신
    void RefreshMyInfo()
    {
        myInfoText.text = GlobalValue.myInfo.nickName + " : " + GlobalValue.myInfo.winCnt.ToString() + "승 " +
                          GlobalValue.myInfo.loseCnt + "패";
        userGoldText.text = "x" + GlobalValue.userGold.ToString();
        GetMyRanking();
    }
    void GetMyRanking()  //순위 불러오기
    {
        StartCoroutine(GetMyRankingCo());
    }

    IEnumerator GetMyRankingCo()
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_id", GlobalValue.myInfo.userID, System.Text.Encoding.UTF8);

        UnityWebRequest a_www = UnityWebRequest.Post(LoadMyRankUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sReturn = enc.GetString(a_www.downloadHandler.data);
            if (sReturn.Contains("OK_"))
            {
                GlobalValue.myInfo.ranking = int.Parse(sReturn.Replace("OK_", string.Empty));

                rankText.text = GlobalValue.myInfo.ranking + "등";
                rank_pointText.text = string.Format($"({GlobalValue.myInfo.score}P)");
                int a_Tier = GlobalValue.myInfo.GetTier(GlobalValue.myInfo.score);
                
                rankImg.sprite = Resources.Load<Sprite>(string.Format("TierImage/Tier_{0}", a_Tier));
            }
        }
        else
        {
            Debug.Log(a_www.error);
        }
    }
    private void OnPointerDown(PointerEventData data)
    {
        profileSV.GetComponent<ProfileScrollView>().isSelected = true;
    }
}
 