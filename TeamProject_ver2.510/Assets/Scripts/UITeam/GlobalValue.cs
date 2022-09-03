using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalValue
{
    public static UserInfo myInfo = new UserInfo();

    public static Dictionary<int, int> TankLvDic = new Dictionary<int, int>();
    public static Dictionary<SkillType, int> SkillLvDic = new Dictionary<SkillType, int>();
    public static int userGold = 0;

    public static int[] UnitSet = new int[4] { -1, -1, -1, -1 };
    public static int[] SkillSet = new int[4] { -1, -1, -1, -1 };
}

public class ConfigValue
{
    public static int UseBgmSound = 1; // 0 : 사용 안함 , 1 : 사용함
    public static int UseEffSound = 1;
    public static float BgmSdVolume = 1f; // 0.0f ~ 1.0f
    public static float EffSdVolume = 1f;
}

public class UserInfo
{
    public string userID;
    public string nickName;
    public int winCnt;
    public int loseCnt;
    public int score;
    public int ranking;
    public int profileIdx;
    public string mydefset;

    public Dictionary<int, int> TowerLvDic = new Dictionary<int, int>();

    public bool IsShow; // 서버에서 유저 정보를 가져와 갱신한 후 한번이라도 매칭에 보여졌으면? 

    //티어 계산
    public int GetTier(int score)
    {
        int Tier = 0;

        if (1100 <= score && 1200 > score)
            Tier = 0;
        else if (1200 <= score && 1350 > score)
            Tier = 1;
        else if (1350 <= score && 1500 > score)
            Tier = 2;
        else if (1500 <= score && 1650 > score)
            Tier = 3;
        else if (1650 <= score && 1800 > score)
            Tier = 4;
        else if (1800 <= score && 2000 > score)
            Tier = 5;
        else if (2000 <= score && 2200 > score)
            Tier = 6;
        else if (2200 <= score && 2400 > score)
            Tier = 7;
        else if (2400 <= score && 2500 > score)
            Tier = 8;
        else if (2500 <= score)
            Tier = 9;

        return Tier;
    }

    public void ClearUserInfo()
    {
        userID = "";
        nickName = "";
        winCnt = 0;
        loseCnt = 0;
        score = 0;
        ranking = 0;
        profileIdx = 0;
        mydefset = "";

        IsShow = false;
        
        TowerLvDic.Clear();
    }
}

public class SelectEnemyInfo
{
    public static UserInfo EnemyInfo = new UserInfo();
    public static Dictionary<TowerType, List<int>> DicTowerPos = new Dictionary<TowerType, List<int>>();
    public static Dictionary<TowerType, TowerInfo> DicTowerInfo = new Dictionary<TowerType, TowerInfo>();
    public static void SetData(UserInfo info = null)
    {
        if (info != null)
        {
            EnemyInfo.userID = info.userID;
            EnemyInfo.nickName = info.nickName;
            EnemyInfo.winCnt = info.winCnt;
            EnemyInfo.loseCnt = info.loseCnt;
            EnemyInfo.score = info.score;
            EnemyInfo.ranking = info.ranking;

            EnemyInfo.IsShow = info.IsShow;
            DicTowerPos = JsonMgr.DefDeckToDic(info.mydefset);

            EnemyInfo.TowerLvDic = info.TowerLvDic;
            foreach(var twInfo in DBContainer.towerInfoList)
            {
                if (EnemyInfo.TowerLvDic[twInfo.towerType] == twInfo.lv)
                {
                    TowerInfo node = new TowerInfo();
                    node.towerType = twInfo.towerType;
                    node.lv = twInfo.lv;
                    node.towerdamage = twInfo.towerdamage;
                    node.attackcycle = twInfo.attackcycle;
                    node.towerHP = twInfo.towerHP;
                    node.gold = twInfo.gold;
                    DicTowerInfo.Add((TowerType)node.towerType, node);
                }
            }
        }
        else
        {
            EnemyInfo.ClearUserInfo();

            DicTowerPos.Clear();
            DicTowerInfo.Clear();
        }
    }

    public static UserInfo GetData()
    {
        return EnemyInfo;
    }
}

public class ScoreInfo
{
    public const float k = 20f;

    //일반화한 함수
    public static int GetPoint(bool isWin, int myPoint, int elsePoint)
    {
        float p = (elsePoint - myPoint) / 400;
        float we = 1 / (Mathf.Pow(10, p) + 1);

        float w = (isWin) ? 1f : 0f;

        return Mathf.RoundToInt(k * (w - we));
    }
}