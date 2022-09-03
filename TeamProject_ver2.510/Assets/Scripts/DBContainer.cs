using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using System.Reflection;

public enum TowerType
{
    [Description("로켓 타워")]
    Rocket,
    [Description("머신건 타워")]
    MG,
    [Description("파이어 타워")]
    Fire,
    [Description("레이저 타워")]
    Laser,
    [Description("버프 타워")]
    Buff,
    [Description("슈퍼 타워")]
    Super,
    TowerCount
}
public enum TankType
{
    [Description("노멀 탱크")]
    Normal,
    [Description("미사일 탱크")]
    Missile,
    [Description("파이어 탱크")]
    Fire,
    [Description("터렛 탱크")]
    Turret,
    [Description("머신건 탱크")]
    MG,
    [Description("스카웃 탱크")]
    Scout,
    [Description("스텔스 탱크")]
    Stealth,
    [Description("버프 탱크")]
    Buff,
    TankCount
}
public enum SkillType
{
    [Description("폭격")]
    Plane,
    [Description("보호막")]
    Shield,
    [Description("스캔")]
    Flare,
    SkillCount
}

public static class Enum
{
    public static string GetDescription<T>(this T source)
    {
        FieldInfo fi = source.GetType().GetField(source.ToString());

        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.
            GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attributes != null && attributes.Length > 0) return attributes[0].Description;
        else return source.ToString();
    }
}

public class TankInfo
{
    public int tankType = 0;
    public int lv = 0;
    public int price = 0;

    public int damage = 0;              //공격력
    public float atkCool = 0.0f;        //공격 쿨타임
    public float maxHP = 0.0f;         //탱크 MaxHP
    public int spawnCount = 0;
}

public class TowerInfo
{
    public int towerType = 0;
    public int lv = 0;
    public int price = 0;

    public float towerdamage = 0;               //타워 공격력
    public float attackcycle = 0;               //공격 주기
    public float towerHP = 0;                   //타워 체력
    public int gold = 10;
    public int cost = 1;
}

public class DBContainer
{
    public static List<TowerInfo> towerInfoList = new List<TowerInfo>();
    public static List<TankInfo> tankInfoList = new List<TankInfo>();
}