using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfileListNodeCtrl : MonoBehaviour
{
    public int Idx = 0; // 프로필 인덱스
    private Sprite Img;

    private string UpdateProfileUrl = "http://pmaker.dothome.co.kr/pMaker_7Gi/UpdateProfile.php";

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(OnClickBtn);
    }

    public void Init(int a_Idx, Sprite a_Img)
    {
        Idx = a_Idx;
        Img = a_Img;

        RawImage RawImg = this.gameObject.GetComponentInChildren<RawImage>();
        RawImg.texture = a_Img.texture;
    }

    void OnClickBtn()
    {
        GameObject a_MyProfile = GameObject.Find("ProfileChangeBtn");
        if (a_MyProfile != null)
            a_MyProfile.transform.Find("ProfileImg").GetComponent<Image>().sprite = Img;

        GlobalValue.myInfo.profileIdx = Idx;
        ProfileScrollView test = this.transform.GetComponentInParent<ProfileScrollView>();
        test.isSelected = false;
        UpdateProfile();
    }

    void UpdateProfile()
    {
        StartCoroutine(UpdateProfileCo());
    }

    IEnumerator UpdateProfileCo()
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_id", GlobalValue.myInfo.userID, System.Text.Encoding.UTF8);
        form.AddField("Input_profileidx", GlobalValue.myInfo.profileIdx);

        UnityWebRequest a_www = UnityWebRequest.Post(UpdateProfileUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sReturn = enc.GetString(a_www.downloadHandler.data);
            if (sReturn.Contains("OK_"))
                Debug.Log("OK_");
        }
        else
        {
            Debug.Log(a_www.error);
        }
    }
}
