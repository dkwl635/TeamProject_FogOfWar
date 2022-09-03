using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListNodeCtrl : MonoBehaviour
{
    UserInfo EnemyInfo;

    private Button ThisBtn;

    public Text NickTxt;
    public Text WinCntTxt;
    public Text LoseCntTxt;
    public Text RankTxt;
    public RawImage ProfileImg;
    public Image RankImg;

    // Start is called before the first frame update
    void Start()
    {
        ThisBtn = this.GetComponent<Button>();
        if (!ReferenceEquals(ThisBtn, null))
            ThisBtn.onClick.AddListener(ThisBtnClick);
    }

    public void InitNode(UserInfo a_User)
    {
        EnemyInfo = a_User;

        NickTxt.text = string.Format($"닉네임 : {a_User.nickName}");
        WinCntTxt.text = string.Format($"Win : {a_User.winCnt}");
        LoseCntTxt.text = string.Format($"Lose : {a_User.loseCnt}");
        RankTxt.text = string.Format($"랭킹 : {a_User.ranking}");
        ProfileImg.texture = Resources.Load<Sprite>(string.Format("ProfileImage/Character{0:00}", a_User.profileIdx)).texture;
        int a_Tier = a_User.GetTier(a_User.score);
        RankImg.sprite = Resources.Load<Sprite>(string.Format("TierImage/Tier_{0}", a_Tier));
    }

    void ThisBtnClick()
    {
        SelectEnemyInfo.SetData(EnemyInfo);
        LoadingManager.Instance.LoadScene("SetAttack");
    }
}
