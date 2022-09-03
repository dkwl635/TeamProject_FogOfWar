using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public enum GameState
{
    Ready,
    Playing,
    Pause,
    End,
    Exit
}

public class GameManager : MonoBehaviour
{
    public static GameManager Inst;

    public GameObject ConfigBox;
    public GameObject DlgBox;
    public Canvas Canvas;

    [Header("UI")]
    public Button pauseBtn;
    public Text timerText;

    float remainTime = 181f; //제한 시간

    [Header("WinLosePanel")] //승패 조건을 만족할 경우 UI
    public GameObject winlosePanel;
    public Text winloseText;
    public Button goLobbyBtn;
    public Text ratingText;
    public Text goldText;

    [Header("GamePause")] //일시 정지 상태일 때
    public GameObject pausePanel;
    public Button restartBtn;
    public Button exitBtn;
    public Button ConfigBtn;

    [Header("SkillTabGroup")]
    public Button tabBtn;
    public Transform skillGroup;
    public Transform tankSpawnGroup;
    bool isSkillGroupOn = false;
    Vector3 tabOnPos = new Vector3(480, 110, 0);
    Vector3 tabOffPos = new Vector3(500, 140, 0);
    float tabSpeed = 600f;

    public static GameState gameState = GameState.Ready;

    int tempGold = 0;
    public Text tempGoldText;

    string UpdateGameResultUrl = "http://pmaker.dothome.co.kr/pMaker_7Gi/UpdateGameResult.php";
    void Awake()
    {
        Inst = this;
        //로딩이 다 될 때까지 gameState는 Ready로
        gameState = GameState.Ready;
        //혹시 다른 신에서 timeScale 값을 조정했다면 다시 1f로 돌려야 함
        Time.timeScale = 1f;
    }

    // Start is called before the first frame update
    void Start()
    {
        //로딩이 완료되면 gameState를 Playing으로
        gameState = GameState.Playing;

        if (pauseBtn != null)
            pauseBtn.onClick.AddListener(PauseBtnFunc);

        if (goLobbyBtn != null)
            goLobbyBtn.onClick.AddListener(() =>
            {
                LoadingManager.Instance.LoadScene("Lobby");
            });

        //pausePanel에 달린 버튼 처리
        if (restartBtn != null)
            restartBtn.onClick.AddListener(() =>
            {
                gameState = GameState.Playing;
                Time.timeScale = 1f; //일시 정지 해제
                pausePanel.SetActive(false); // 일시정지 패널 끄기
            });
        if (exitBtn != null)
            exitBtn.onClick.AddListener(() =>
            {
                string msg = "나가기를 누를경우 패배로 처리됩니다.\n그래도 나가시겠습니까?";
                DlgBoxCtrl.OK_Act ActFunc = DlgPlayerLose;
                GameObject a_DlgBoxObj = (GameObject)Instantiate(DlgBox);
                a_DlgBoxObj.GetComponent<DlgBoxCtrl>().InitData("경고!", msg, ActFunc);
                a_DlgBoxObj.transform.SetParent(Canvas.transform, false);
            });

        if (ConfigBtn != null)
            ConfigBtn.onClick.AddListener(ConfigBtnClick);

        if (tabBtn != null)
            tabBtn.onClick.AddListener(TabChange);
    }

    // Update is called once per frame
    void Update()
    {
        //gameState가 Playing이 아니면 프레임마다 연산해야 할 기능이 없다. (생기면 그 때 가서 수정)
        if (gameState != GameState.Playing)
            return;

        remainTime -= Time.deltaTime; //남은 시간이 점점 줄어든다
        timerText.text = System.TimeSpan.FromSeconds(remainTime).ToString(@"mm\:ss"); // 남은 시간을 timerText에 표시한다.(시간 표기는 분:초 로 표기할 것)

        //남은 시간이 0이 되면
        if (remainTime <= 0f)
        {
            remainTime = 0f;
            PlayerWinLose(false); //패배 처리를 한다.
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            TabChange();
        }

        TabMoveCtrl();
    }

    #region Skill/TankTabGroup
    void TabChange()
    {
        isSkillGroupOn = !isSkillGroupOn;
        int s = skillGroup.GetSiblingIndex();
        int t = tankSpawnGroup.GetSiblingIndex();
        skillGroup.SetSiblingIndex(t);
        tankSpawnGroup.SetSiblingIndex(s);
    }

    void TabMoveCtrl()
    {
        if (isSkillGroupOn)
        {
            if (tankSpawnGroup.localPosition.y < tabOffPos.y)
                tankSpawnGroup.localPosition = Vector3.MoveTowards(tankSpawnGroup.localPosition, tabOffPos, tabSpeed * Time.deltaTime);
            if (skillGroup.localPosition.y > tabOnPos.y)
                skillGroup.localPosition = Vector3.MoveTowards(skillGroup.localPosition, tabOnPos, tabSpeed * Time.deltaTime);
        }
        else
        {
            if (tankSpawnGroup.localPosition.y > tabOnPos.y)
                tankSpawnGroup.localPosition = Vector3.MoveTowards(tankSpawnGroup.localPosition, tabOnPos, tabSpeed * Time.deltaTime);
            if (skillGroup.localPosition.y < tabOffPos.y)
                skillGroup.localPosition = Vector3.MoveTowards(skillGroup.localPosition, tabOffPos, tabSpeed * Time.deltaTime);
        }
    }
    #endregion

    //타워 파괴 시 골드 추가
    public void AddGold(int value)
    {
        tempGold += value;
        tempGoldText.text = tempGold.ToString();
    }

    #region GamePause
    //일시 정지 버튼을 눌렀을 때 호출되는 메서드
    void PauseBtnFunc()
    {
        //일시 정지 상태로 변경
        gameState = GameState.Pause;
        Time.timeScale = 0f;

        //메시지 창을 띄운다.(Instantiate or SetActive())
        pausePanel.SetActive(true);
        //메시지 창에서는 게임 종료, 환경설정, 일시 정지 해제 등의 기능을 제공한다.
    }
    public void DlgPlayerLose()
    {
        PlayerWinLose(false); //강제 패배 호출
        pausePanel.SetActive(false); //일시정지 패널 끄기
    }

    //환경설정
    void ConfigBtnClick()
    {
        GameObject a_ConfigBox = Instantiate(ConfigBox);
        a_ConfigBox.transform.SetParent(Canvas.transform, false);
    }
    #endregion

    #region GameEnd
    //플레이어가 승리하면 true, 패배하면 false
    public void PlayerWinLose(bool isWin)
    {
        Time.timeScale = 0f;
        //패배 시의 UI를 띄운다.
        winlosePanel.SetActive(true);
        winloseText.text = isWin ? "YOU Win!!!" : "YOU LOSE...";
        //네트워크를 통해 패배 처리를 한다.
        if (!isWin)
            tempGold /= 2;
        StartCoroutine(UpdateGameResultCo(isWin));
    }
    //DB서버로 플레이어와 상대방의 승패 결과를 송신
    IEnumerator UpdateGameResultCo(bool isWin)
    {
        WaitingPanel.Instance.SetActive(true);

        //Instantiate
        WWWForm form = new WWWForm();
        form.AddField("Input_id", GlobalValue.myInfo.userID, System.Text.Encoding.UTF8);
        form.AddField("Input_win", isWin ? 1 : 0);
        form.AddField("Input_lose", isWin ? 0 : 1);
        form.AddField("Input_score", ScoreInfo.GetPoint(isWin, GlobalValue.myInfo.score, SelectEnemyInfo.EnemyInfo.score));
        form.AddField("Input_gold", tempGold);

        form.AddField("Enemy_id", SelectEnemyInfo.EnemyInfo.userID, System.Text.Encoding.UTF8);
        form.AddField("Enemy_win", isWin ? 0 : 1);
        form.AddField("Enemy_lose", isWin ? 1 : 0);
        form.AddField("Enemy_score", ScoreInfo.GetPoint(!isWin, SelectEnemyInfo.EnemyInfo.score, GlobalValue.myInfo.score));

        UnityWebRequest a_www = UnityWebRequest.Post(UpdateGameResultUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sReturn = enc.GetString(a_www.downloadHandler.data);
            if (sReturn.Contains("OK_"))
            {
                Debug.Log("OK_");

                ratingText.text = "score : " + sReturn.Split('_')[1] + "p(";

                if (isWin)
                    ratingText.text += "+";

                ratingText.text += ScoreInfo.GetPoint(isWin, GlobalValue.myInfo.score, SelectEnemyInfo.EnemyInfo.score).ToString() + "p)";

                goldText.text = "x " + tempGold + " 획득";

                int.TryParse(sReturn.Split('_')[1], out GlobalValue.myInfo.score);
                GlobalValue.userGold += tempGold;
            }
        }
        else
        {
            Debug.Log(a_www.error);
        }

        WaitingPanel.Instance.SetActive(false);
        gameState = GameState.End;
        SelectEnemyInfo.SetData();
    }
    #endregion

    #region pointerOverUI
    public static bool IsPointerOverUIObject() //UGUI의 UI들이 먼저 피킹되는지 확인하는 함수
    {
        PointerEventData a_EDCurPos = new PointerEventData(EventSystem.current);

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
       List<RaycastResult> results = new List<RaycastResult>();
       for (int i = 0; i < Input.touchCount; ++i)
       {
            a_EDCurPos.position = Input.GetTouch(i).position;  
            results.Clear();
            if (EventSystem.current == null)
                return false;
            EventSystem.current.RaycastAll(a_EDCurPos, results);
            if (0 < results.Count)
                return true;
       }
       return false;
#else
        a_EDCurPos.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        if (EventSystem.current == null)
            return false;
        EventSystem.current.RaycastAll(a_EDCurPos, results);
        return (0 < results.Count);
#endif
    }
    #endregion
}
