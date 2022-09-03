using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using SimpleJSON;

public class TitleManager : MonoBehaviour
{
    EventSystem eventSystem;

    [Header("LoginPanel")]
    public GameObject loginPanel;
    public InputField idIFd;
    public InputField pwIFd;
    public Button loginBtn = null;
    public Button creAccOpenBtn = null;

    [Header("CreateAccountPanel")]
    public GameObject createAccPanel;
    public InputField new_idIFd;
    public InputField new_pwIFd;
    public InputField new_nickIFd;
    public Button createAccBtn;
    public Button cancelBtn;

    [Header("Normal")]
    public Text messageText;
    float messageTimer = 0.0f;
    public Button configBtn;
    public Canvas canvas;
    public GameObject configBox;

    string loginURL = "http://pmaker.dothome.co.kr/pMaker_7Gi/Login.php";
    string createURL = "http://pmaker.dothome.co.kr/pMaker_7Gi/CreateAccount.php";

    string defDBURL = "http://pmaker.dothome.co.kr/pMaker_7Gi/DefContainer.php";
    string attDBURL = "http://pmaker.dothome.co.kr/pMaker_7Gi/AttContainer.php";

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = EventSystem.current;

        if (DBContainer.towerInfoList.Count <= 0)
        {
            StartCoroutine(TowerDBCo());
        }
        if (DBContainer.tankInfoList.Count <= 0)
        {
            StartCoroutine(TankDBCo());
        }

        if (loginBtn != null)
            loginBtn.onClick.AddListener(LoginBtn);
        if (creAccOpenBtn != null)
            creAccOpenBtn.onClick.AddListener(CreateAccPanelOn);
        if (cancelBtn != null)
            cancelBtn.onClick.AddListener(LoginPanelOn);
        if (createAccBtn != null)
            createAccBtn.onClick.AddListener(CreateAccBtn);

        if (configBtn != null)
            configBtn.onClick.AddListener(() =>
            {
                GameObject cfgBox = Instantiate(configBox);
                cfgBox.transform.SetParent(canvas.transform, false);
            });

        SoundManager.Instance.PlayBGM("TitleBGM");
        LoadingManager.Instance.InitData();
        WaitingPanel.Instance.SetActive(false);

        idIFd.Select();
    }

    // Update is called once per frame
    void Update()
    {
        //시간이 지나면 메시지 텍스트가 꺼지게 하기
        if (messageTimer > 0f)
        {
            messageTimer -= Time.deltaTime;

            if (messageTimer <= 0f)
            {
                messageTimer = 0f;
                MessageOnOff();
            }
        }

        //tab 키를 눌러서 다음 인풋필드로 넘어가기
        //inputfield의 navigation을 설정해야 한다.
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = eventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next != null)
                next.Select();
        }

        //엔터 키를 눌러서 로그인/계정생성 동작 실행
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (createAccPanel.activeSelf) //계정 생성 패널이 열려 있을 때는 계정 생성 버튼 동작
                createAccBtn.onClick.Invoke();
            else if (loginPanel.activeSelf) //로그인 패널은 항상 열려 있게 되어 있으므로
                loginBtn.onClick.Invoke();
        }
    }

    //패널이 열리고 닫힐 때 인풋필드를 전부 초기화 시키기
    void InputFieldClear()
    {
        idIFd.text = "";
        pwIFd.text = "";
        new_idIFd.text = "";
        new_pwIFd.text = "";
        new_nickIFd.text = "";
    }

    #region 버튼 클릭 시 호출되는 함수들
    //로그인 버튼
    void LoginBtn()
    {
        GlobalValue.myInfo.userID = "";
        string idStr = idIFd.text.Trim();
        string pwStr = pwIFd.text.Trim();

        if (idStr == "" || pwStr == "")
        {
            MessageOnOff("빈 칸 없이 입력해 주셔야 합니다.");
            return;
        }

        if (!(3 <= idStr.Length && idStr.Length <= 10))
        {
            MessageOnOff("ID는 3글자 이상 10글자 이하로 작성해 주세요.");
            return;
        }

        if (!(4 <= pwStr.Length && pwStr.Length <= 15))
        {
            MessageOnOff("비밀번호는 4글자 이상 15글자 이하로 작성해 주세요.");
            return;
        }
        StartCoroutine(LoginCo(idStr, pwStr));
    }
    //계정생성 패널 열기 버튼
    void CreateAccPanelOn()
    {
        InputFieldClear();
        createAccPanel.SetActive(true);

        new_idIFd.Select();
    }
    //계정 생성 닫기
    void LoginPanelOn()
    {
        InputFieldClear();
        createAccPanel.SetActive(false);

        idIFd.Select();
    }
    //계정생성 버튼
    void CreateAccBtn()
    {
        string idStr = new_idIFd.text.Trim();
        string pwStr = new_pwIFd.text.Trim();
        string nickStr = new_nickIFd.text.Trim();

        if (idStr == "" || pwStr == "" || nickStr == "")
        {
            MessageOnOff("빈 칸 없이 입력해 주셔야 합니다.");
            return;
        }

        if (!(3 <= idStr.Length && idStr.Length <= 10))
        {
            MessageOnOff("ID는 3글자 이상 10글자 이하로 작성해 주세요.");
            return;
        }

        if (!(4 <= pwStr.Length && pwStr.Length <= 15))
        {
            MessageOnOff("비밀번호는 4글자 이상 15글자 이하로 작성해 주세요.");
            return;
        }

        if (!(2 <= nickStr.Length && nickStr.Length <= 6))
        {
            MessageOnOff("별명은 2글자 이상 6글자 이하로 작성해 주세요.");
            return;
        }

        StartCoroutine(CreateCo(idStr, pwStr, nickStr));
    }
    #endregion

    #region 네트워크 코루틴
    IEnumerator LoginCo(string idStr, string pwStr)
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_user", idStr, System.Text.Encoding.UTF8);
        form.AddField("Input_pass", pwStr, System.Text.Encoding.UTF8);

        UnityWebRequest request = UnityWebRequest.Post(loginURL, form);
        yield return request.SendWebRequest();

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string sz = enc.GetString(request.downloadHandler.data);

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
            Debug.Log("네트워크 에러");
            ErrorMessage(request.error);
            yield break;
        }
        else if (!sz.Contains("Login Success") || !sz.Contains("{\""))
        {
            Debug.Log(sz);
            ErrorMessage(sz);
            yield break;
        }
        else
        {
            var N = JSON.Parse(sz);
            if (N == null)
            {
                Debug.Log("N = null");
                yield break;
            }

            GlobalValue.myInfo.userID = idStr;
            if (N["NickName"] != null)
                GlobalValue.myInfo.nickName = N["NickName"];
            if (N["Win"] != null)
                GlobalValue.myInfo.winCnt = N["Win"].AsInt;
            if (N["Lose"] != null)
                GlobalValue.myInfo.loseCnt = N["Lose"].AsInt;
            if (N["UserGold"] != null)
                GlobalValue.userGold = N["UserGold"].AsInt;
            if (N["UserScore"] != null)
                GlobalValue.myInfo.score = N["UserScore"].AsInt;
            if (N["ProfileImgIdx"] != null)
                GlobalValue.myInfo.profileIdx = N["ProfileImgIdx"].AsInt;
            if (N["MyDefSet"] != null)
                GlobalValue.myInfo.mydefset = N["MyDefSet"];
            if (N["TowerLv"] != null)
            {
                string jsonStr = N["TowerLv"];
                if (jsonStr != "" && jsonStr.Contains("TowerLv"))
                {
                    var N2 = JSON.Parse(jsonStr);
                    for (int i = 0; i < N2["TowerLv"].Count; i++)
                    {
                        GlobalValue.myInfo.TowerLvDic.Add(i, N2["TowerLv"][i]);
                    }
                }
            }
            if (N["TankLv"] != null)
            {
                string jsonStr = N["TankLv"];
                if (jsonStr != "" && jsonStr.Contains("TankLv"))
                {
                    var N2 = JSON.Parse(jsonStr);
                    for (int i = 0; i < N2["TankLv"].Count; i++)
                    {
                        GlobalValue.TankLvDic.Add(i, N2["TankLv"][i]);
                    }
                }
            }
            if (N["SkillLv"] != null)
            {
                string jsonStr = N["SkillLv"];
                if (jsonStr != "" && jsonStr.Contains("SkillLv"))
                {
                    var N2 = JSON.Parse(jsonStr);
                    for (int i = 0; i < N2["SkillLv"].Count; i++)
                    {
                        GlobalValue.SkillLvDic.Add((SkillType)i, N2["SkillLv"][i]);
                    }
                }
            }

            LoadingManager.Instance.LoadScene("Lobby");
        }
    }
    IEnumerator CreateCo(string idStr, string pwStr, string nickStr)
    {
        WaitingPanel.Instance.SetActive(true);

        WWWForm form = new WWWForm();
        form.AddField("Input_user", idStr, System.Text.Encoding.UTF8);
        form.AddField("Input_pass", pwStr, System.Text.Encoding.UTF8);
        form.AddField("Input_nick", nickStr, System.Text.Encoding.UTF8);

        UnityWebRequest request = UnityWebRequest.Post(createURL, form);
        yield return request.SendWebRequest();

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string sz = enc.GetString(request.downloadHandler.data);

        if (request.isNetworkError || request.isHttpError)
        {
            //Debug.Log(request.error);
            ErrorMessage(request.error);
        }
        else if (sz.Contains("Create Success"))
        {
            Debug.Log(sz);
            ErrorMessage(request.downloadHandler.text);
            LoginPanelOn();
        }
        else
        {
            Debug.Log(sz);
            ErrorMessage(request.downloadHandler.text);
        }

        WaitingPanel.Instance.SetActive(false);
    }

    IEnumerator TowerDBCo()
    {
        UnityWebRequest request = UnityWebRequest.Get(defDBURL);
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            Debug.Log(request.error);
            yield break;
        }

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string sz = enc.GetString(request.downloadHandler.data);

        var N = JSON.Parse(sz);

        for (int i = 0; i < N.Count; i++)
        {
            TowerInfo node = new TowerInfo();
            node.towerType = N[i]["TowerType"].AsInt;
            node.lv = N[i]["Level"].AsInt;
            node.towerdamage = N[i]["Damage"].AsFloat;
            node.attackcycle = N[i]["AttCycle"].AsFloat;
            node.towerHP = N[i]["MaxHP"].AsFloat;
            node.price = N[i]["StoreGold"].AsInt;
            node.gold = N[i]["DestroyGold"].AsInt;
            node.cost = N[i]["Cost"].AsInt;
            DBContainer.towerInfoList.Add(node);
        }
    }

    IEnumerator TankDBCo()
    {
        UnityWebRequest request = UnityWebRequest.Get(attDBURL);
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            Debug.Log(request.error);
            yield break;
        }

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string sz = enc.GetString(request.downloadHandler.data);

        var N = JSON.Parse(sz);

        for (int i = 0; i < N.Count; i++)
        {
            TankInfo node = new TankInfo();
            node.tankType = N[i]["TankType"].AsInt;
            node.lv = N[i]["Level"].AsInt;
            node.damage = N[i]["Damage"].AsInt;
            node.atkCool = N[i]["AttCycle"].AsFloat;
            node.maxHP = N[i]["MaxHP"].AsFloat;
            node.price = N[i]["Gold"].AsInt;
            node.spawnCount = N[i]["SpawnCount"].AsInt;
            DBContainer.tankInfoList.Add(node);
        }
    }
    #endregion

    void MessageOnOff(string message = "", bool isOn = true)
    {
        if (messageText == null)
            return;

        if (isOn)
        {
            messageText.text = message;
            messageText.gameObject.SetActive(true);
            messageTimer = 5.0f;
        }
        else
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false);
        }
    }
    void ErrorMessage(string message)
    {
        if (message.Contains("ID does not exist"))
            MessageOnOff("ID가 존재하지 않습니다");
        else if (message.Contains("Pass does not Match"))
            MessageOnOff("패스워드가 일치하지 않습니다");
        else if (message.Contains("ID exists."))
            MessageOnOff("중복된 ID가 존재합니다");
        else if (message.Contains("Nickname exists."))
            MessageOnOff("중복된 닉네임이 존재합니다");
        else
            MessageOnOff(message);
    }
}