using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public static StoreManager Inst;
    public Sprite[] tankSprite;
    public Sprite[] towerSprite;
    public Sprite[] skillSprite;

    public Button returnBtn;
    public Text userNickText = null;
    public Text userGoldText = null;

    public Button defTabBtn;
    public Button attTabBtn;
    public Button skillTabBtn;

    public GameObject defSV;
    public GameObject attSV;
    public GameObject skillSV;

    [Header("InfoPanel")]
    public GameObject infoPanel;
    public Image infoIconImg;
    public Text infoNameText;
    public Text infoStatText;
    public Text infoHelpText;

    private void Awake()
    {
        Inst = this;

        tankSprite = Resources.LoadAll<Sprite>("TankImg"); 
        towerSprite = Resources.LoadAll<Sprite>("DefTeam/TowerImg");
        skillSprite = Resources.LoadAll<Sprite>("SkillImg");
    }
    // Start is called before the first frame update
    void Start()
    {
        if (returnBtn != null)
            returnBtn.onClick.AddListener(() =>
            {
                LoadingManager.Instance.LoadScene("Lobby");
            });

        if (defTabBtn != null)
            defTabBtn.onClick.AddListener(() =>
            {
                defSV.SetActive(true);
                attSV.SetActive(false);
                skillSV.SetActive(false);
            });
        if (attTabBtn != null)
            attTabBtn.onClick.AddListener(() =>
            {
                defSV.SetActive(false);
                attSV.SetActive(true);
                skillSV.SetActive(false);
            });
        if (skillTabBtn != null)
            skillTabBtn.onClick.AddListener(() =>
            {
                defSV.SetActive(false);
                attSV.SetActive(false);
                skillSV.SetActive(true);
            });

        SoundManager.Instance.PlayBGM("ShopBGM");
    }

    public void RefreshMyInfo()
    {
        userNickText.text = GlobalValue.myInfo.nickName;
        userGoldText.text = GlobalValue.userGold.ToString();
    }

    public void SetInfo(TowerInfo info)
    {
        infoNameText.text = Enum.GetDescription((TowerType)info.towerType);
        infoStatText.text = "공격 : " + info.towerdamage + "\n";
        infoStatText.text += "공격속도 : " + info.attackcycle + "\n";
        infoStatText.text += "체력 : " + info.towerHP + "\n";

        if (towerSprite.Length > info.towerType)
            infoIconImg.sprite = towerSprite[info.towerType];

        switch((TowerType)info.towerType)
        {
            case TowerType.Rocket:
                infoHelpText.text = "로켓을 발사하는 가장 기본적인 타워입니다";
                break;
            case TowerType.MG:
                infoHelpText.text = "공격력은 약하지만 공격 속도가 빠른 타워입니다";
                break;
            case TowerType.Laser:
                infoHelpText.text = "빠른 공격속도의 레이저를 발사하는 타워입니다";
                break;
            case TowerType.Fire:
                infoHelpText.text = "넓은 범위에 화염을 발사하는 타워입니다";
                break;
            case TowerType.Buff:
                infoHelpText.text = "주변 타워의 공격력을 상승시키는 타워입니다";
                break;
            case TowerType.Super:
                infoHelpText.text = "많은 미사일을 발사하는 가장 강력한 타워입니다";
                break;
        }

        infoPanel.SetActive(true);
    }
    public void SetInfo(TankInfo info)
    {
        infoNameText.text = Enum.GetDescription((TankType)info.tankType);
        infoStatText.text = "공격 : " + info.damage + "\n";
        infoStatText.text += "공격속도 : " + info.atkCool + "\n";
        infoStatText.text += "체력 : " + info.maxHP + "\n";

        if (tankSprite.Length > info.tankType)
            infoIconImg.sprite = tankSprite[info.tankType];

        switch((TankType)info.tankType)
        {
            case TankType.Normal:
                infoHelpText.text = "특징이 없는 기본 탱크입니다";
                break;
            case TankType.Missile:
                infoHelpText.text = "먼 거리에서 미사일을 발사하는 탱크입니다";
                break;
            case TankType.Fire:
                infoHelpText.text = "적을 불태우는 화염을 발사하는 탱크입니다";
                break;
            case TankType.MG:
                infoHelpText.text = "기관총을 발사하는 탱크입니다";
                break;
            case TankType.Turret:
                infoHelpText.text = "여러개의 포탑을 가지고 있는 탱크입니다";
                break;
            case TankType.Scout:
                infoHelpText.text = "이동속도가 빠르고 파괴시 자폭하는 탱크입니다";
                break;
            case TankType.Stealth:
                infoHelpText.text = "주기적으로 투명화 스킬을 사용하는 탱크입니다";
                break;
            case TankType.Buff:
                infoHelpText.text = "일시적으로 주변 탱크의 공격력과 이동속도를 강화하는 탱크입니다";
                break;
        }

        infoPanel.SetActive(true);
    }
    public void SetInfo(SkillType type)
    {
        infoNameText.text = "";
        infoStatText.text = Enum.GetDescription(type);

        if (skillSprite.Length > (int)type)
            infoIconImg.sprite = skillSprite[(int)type];

        switch(type)
        {
            case SkillType.Plane:
                infoHelpText.text = "폭격기를 보내 타워를 공격하는 스킬입니다";
                break;
            case SkillType.Shield:
                infoHelpText.text = "타워의 공격을 방어하는 보호막을 설치하는 스킬입니다";
                break;
            case SkillType.Flare:
                infoHelpText.text = "어둠으로 둘러싸인 영역을 밝히는 스킬입니다";
                break;
        }

        infoPanel.SetActive(true);
    }
}