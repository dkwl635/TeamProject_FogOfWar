using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//싱글 톤 패턴
//어느 씬에서든 로딩할 수 있는 구조로 작성

//MonoBehaviour를 상속받는 게 아니라 MonoSingleton의 G_Singleton 클래스를 상속받는다.
public class SoundManager : G_Singleton<SoundManager>
{
    // Bgm + UI AudioSource
    [HideInInspector] public AudioSource audioSrc = null;
    // 모든 AudioClip 
    Dictionary<string, AudioClip> audioClipList = new Dictionary<string, AudioClip>();

    [HideInInspector] public bool soundOnOff = true;

    //효과음 버퍼
    int effSdCount = 10; //최대 10번 플레이
    int iSdCount = 0;
    List<GameObject> sdObjList = new List<GameObject>();

    // Effect AudioSource
    List<AudioSource> sdSrcList = new List<AudioSource>();

    //awake 대신 사용
    protected override void Init()
    {
        base.Init();

        LoadChildGameObj();
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioClip clip = null;
        object[] temp = Resources.LoadAll("Sound");
        for (int i = 0; i < temp.Length; i++)
        {
            clip = temp[i] as AudioClip;
            if (audioClipList.ContainsKey(clip.name))
                continue;
            audioClipList.Add(clip.name, clip);
        }

        ConfigValue.UseBgmSound = PlayerPrefs.GetInt("SoundOnOff_Bgm", 1);
        bool a_soundOnOff = (ConfigValue.UseBgmSound == 1);
        SoundOnOff_Bgm(a_soundOnOff);

        ConfigValue.UseEffSound = PlayerPrefs.GetInt("SoundOnOff_Eff", 1);
        a_soundOnOff = (ConfigValue.UseEffSound == 1);
        SoundOnOff_Eff(a_soundOnOff);

        ConfigValue.BgmSdVolume = PlayerPrefs.GetFloat("SoundVolume_Bgm", 1f);
        ConfigValue.EffSdVolume = PlayerPrefs.GetFloat("SoundVolume_Eff", 1f);
        SoundVolume_Bgm(ConfigValue.BgmSdVolume);
        SoundVolume_Eff(ConfigValue.EffSdVolume);
    }

    void LoadChildGameObj()
    {
        if (this == null)
            return;

        audioSrc = gameObject.AddComponent<AudioSource>();

        for (int i = 0; i < effSdCount; i++)
        {
            GameObject newSdObj = new GameObject();
            newSdObj.transform.SetParent(transform);
            newSdObj.transform.localPosition = Vector3.zero;
            AudioSource audioSource = newSdObj.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            newSdObj.name = "SoundEffObj";

            sdSrcList.Add(audioSource);
            sdObjList.Add(newSdObj);
        }
    }

    public void PlayBGM(string fileName)
    {
        if (!soundOnOff)
            return;

        if (!audioClipList.ContainsKey(fileName))
            audioClipList.Add(fileName, Resources.Load("Sound/" + fileName) as AudioClip);
        if (audioSrc == null)
            return;

        if (audioSrc.clip != null && audioSrc.clip.name == fileName)
            return;

        audioSrc.clip = audioClipList[fileName];
        audioSrc.loop = true;
        audioSrc.Play();
    }

    public void PlayEffSound(string fileName, Vector3 OhterPos)
    {
        if (!soundOnOff)
            return;

        float DistVol = GetEffVolume(OhterPos);
        if (DistVol <= 0f)
            return;

        if (!audioClipList.ContainsKey(fileName))
            audioClipList.Add(fileName, Resources.Load("Sound/" + fileName) as AudioClip);

        if (audioClipList[fileName] != null && sdSrcList[iSdCount] != null)
        {
            sdSrcList[iSdCount].clip = audioClipList[fileName];
            sdSrcList[iSdCount].loop = false;
            sdSrcList[iSdCount].volume = DistVol * ConfigValue.EffSdVolume / 1f;
            sdSrcList[iSdCount].Play();

            iSdCount++;
            if (effSdCount <= iSdCount)
                iSdCount = 0;
        }
    }

    public float GetEffVolume(Vector3 OhterPos)
    {
        float volume = 1f;
        Vector3 zeroVec = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 CamPos = Camera.main.GetComponent<CameraCtrl>().viewRect.position;
        CamPos.y = 0f;
        OhterPos.y = 0f;

        float MaxDist = (CamPos - zeroVec).magnitude;
        float CurDist = (CamPos - OhterPos).magnitude;

        if (MaxDist <= CurDist)
            return 0f;

        volume = 1f - (CurDist / MaxDist);

        return volume;
    }

    public void PlayUISound(string fileName, float volume = 0.2f)
    {
        if (!soundOnOff)
            return;

        if (!audioClipList.ContainsKey(fileName))
            audioClipList.Add(fileName, Resources.Load("Sound/" + fileName) as AudioClip);

        if (audioSrc == null)
            return;

        audioSrc.PlayOneShot(audioClipList[fileName], volume * ConfigValue.BgmSdVolume);
    }

    public void SoundOnOff_Bgm(bool soundOn = true)
    {
        if (audioSrc != null)
        {
            audioSrc.mute = !soundOn;
        }
    }

    public void SoundOnOff_Eff(bool soundOn = true)
    {
        for (int i = 0; i < sdSrcList.Count; i++)
        {
            if (sdSrcList[i] != null)
            {
                sdSrcList[i].mute = !soundOn;
            }
        }
    }

    public void SoundVolume_Bgm(float volume)
    {
        if (audioSrc != null)
        {
            audioSrc.volume = volume;
        }
    }

    public void SoundVolume_Eff(float volume)
    {
        for (int i = 0; i < sdSrcList.Count; i++)
        {
            sdSrcList[i].volume = volume;
        }
    }
}