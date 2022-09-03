using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public struct LoadingData
{
    public Sprite backImgSprite;
    public string helpTextStr;
}

public class LoadingManager : MonoBehaviour
{
    #region Singleton
    protected static LoadingManager instance;
    public static LoadingManager Instance 
    { 
        get 
        { 
            if (instance == null) 
            { 
                var obj = FindObjectOfType<LoadingManager>(); 
                if (obj != null) 
                { 
                    instance = obj; 
                } 
                else 
                { 
                    instance = Create(); 
                } 
            } 
            return instance; 
        } 
        private set 
        { 
            instance = value; 
        } 
    }
    public static LoadingManager Create()
    {
        var SceneLoaderPrefab = Resources.Load<LoadingManager>("SceneLoader");
        return Instantiate(SceneLoaderPrefab);
    }
    #endregion

    Dictionary<int, LoadingData> loadingDic = new Dictionary<int, LoadingData>();

    [SerializeField] CanvasGroup sceneLoaderCanvasGroup;
    [SerializeField] Image progressBar;
    [SerializeField] Image background;
    [SerializeField] Text helpText;

    private string loadSceneName; 
    
    private void Awake() 
    { 
        if (Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        } 
        DontDestroyOnLoad(gameObject);

        InitData();
    }

    public void InitData()
    {
        if (loadingDic.Count > 0)
            return;

        LoadingData node = new LoadingData();

        TextAsset txt = Resources.Load("LoadUIData/LoadingText", typeof(TextAsset)) as TextAsset;
        Dictionary<int, Sprite> ArrImg = Resources.LoadAll<Sprite>("LoadUIData").ToDictionary(k => int.Parse(k.name.Split('_')[1]));

        string txtSource = txt.text;
        string[] txtValues = txtSource.Split('\n');

        foreach (string txtLine in txtValues)
        {
            int key = int.Parse(txtLine.Split(',')[0]);
            string txtvalue = txtLine.Split(',')[1];

            if (!ArrImg.ContainsKey(key))
                continue;

            node.backImgSprite = ArrImg[key];
            node.helpTextStr = txtvalue;
            loadingDic.Add(key, node);
        }

        gameObject.SetActive(false);
    }

    public void LoadScene(string sceneName) 
    {    
        //로딩 배경화면과 텍스트를 랜덤하게 출력
        int idx = Random.Range(1, loadingDic.Count);
        if(loadingDic.ContainsKey(idx))
        {
            background.sprite = loadingDic[idx].backImgSprite;
            helpText.text = loadingDic[idx].helpTextStr;
        }

        if (SceneManager.GetActiveScene().name.Contains("Title") && sceneName == "Lobby")
        {
            background.sprite = loadingDic[0].backImgSprite;
            helpText.text = loadingDic[0].helpTextStr;
        }


        gameObject.SetActive(true); 
        SceneManager.sceneLoaded += LoadSceneEnd; 
        loadSceneName = sceneName; 
        StartCoroutine(Load(sceneName));
    }
    private IEnumerator Load(string sceneName) 
    { 
        progressBar.fillAmount = 0f;
        sceneLoaderCanvasGroup.alpha = 1f;

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName); 
        op.allowSceneActivation = false; 
        float timer = 0.0f; 
        while (!op.isDone) 
        { 
            yield return null; 
            timer += Time.unscaledDeltaTime; 
            if (op.progress < 0.9f) 
            { 
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer); 
                if (progressBar.fillAmount >= op.progress) 
                { 
                    timer = 0f; 
                } 
            } 
            else 
            { 
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer); 
                if (progressBar.fillAmount == 1.0f) 
                { 
                    op.allowSceneActivation = true; 
                    yield break; 
                } 
            } 
        } 
    }
    private void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode) 
    {
        //InGame씬 전용 연출
        if (scene.name == loadSceneName && scene.name == "InGame") 
        { 
            StartCoroutine(InGame()); 
            SceneManager.sceneLoaded -= LoadSceneEnd; 
        }
        else if(scene.name == loadSceneName)
        {
            gameObject.SetActive(false);
            SceneManager.sceneLoaded -= LoadSceneEnd;
        }
    }

    //InGame씬 전용 연출(fog of war 적용 시간동안 대기)
    private IEnumerator InGame() 
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        SoundManager.Instance.PlayBGM("InGameBGM");
    }
}
