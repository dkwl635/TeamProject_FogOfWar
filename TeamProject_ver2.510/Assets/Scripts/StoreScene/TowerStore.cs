using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class TowerStore : MonoBehaviour
{
    public Transform UICanvas;
    public GameObject itemSVContent;
    public GameObject itemNodePrefab;

    protected ShopNode[] nodeList;
    protected int buyType;
    protected int myGoldToServer = 0;
    protected string jsonStr = "";

    protected bool isNetworkLock = false;
    protected string buyReqURL = "";

    protected List<int> setLv = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        buyReqURL = "http://pmaker.dothome.co.kr/pMaker_7Gi/TowerBuy.php";

        //DBContainer.towerInfoList; -> 타워 DB 상의 모든 정보
        //GlobalValue.myInfo.TowerLvDic; -> 내가 가진 타워 Lv
        foreach (var info in DBContainer.towerInfoList)
        {
            //타워 타입과 일치하는 키가 없으면
            if (!GlobalValue.myInfo.TowerLvDic.ContainsKey(info.towerType))
            {
                //레벨 0으로 해당 타입 딕셔너리 등록
                GlobalValue.myInfo.TowerLvDic.Add(info.towerType, 0);
            }
            //보유한 타워의 타입과 레벨이 일치하는 경우
            if (GlobalValue.myInfo.TowerLvDic[info.towerType] == info.lv - 1)
            {
                //UI 노드를 인스턴스화
                GameObject obj = Instantiate(itemNodePrefab);
                ShopNode node = obj.GetComponent<ShopNode>();
                obj.transform.SetParent(itemSVContent.transform, false);
                //UI 노드 자체에 타워 정보를 심어서 바로 접근
                node.InitData(info);
            }
        }

        //내 정보(닉네임, 골드) 불러오기
        StoreManager.Inst.RefreshMyInfo();
        RefreshItemList();
    }

    //타워 레벨에 따라 UI 노드 상태를 바꿔주는 메서드
    protected void RefreshItemList()
    {
        if (nodeList == null || nodeList.Length <= 0)
            nodeList = itemSVContent.GetComponentsInChildren<ShopNode>();

        foreach (var node in nodeList)
        {
            node.itemState = (node.lv > 0) ? ItemState.Active : ItemState.BeforeBuy;
            if (node.lv >= 5)//만렙이면
                node.itemState = ItemState.MaxLv;
            node.SetState(node.itemState);
        }
    }

    public virtual void BuyItem(int type)
    {
        if (isNetworkLock)
        {
            return;
        }

        if (type < 0 || nodeList.Length <= type)
            return;

        buyType = type;

        string mess = "";
        ItemState state = ItemState.BeforeBuy;
        bool isBuyOK = false;
        TowerInfo itemInfo = nodeList[type].twInfo;

        if (nodeList != null && buyType < nodeList.Length)
        {
            state = nodeList[buyType].itemState;
        }
        switch (state)
        {
            case ItemState.BeforeBuy:
                if (GlobalValue.userGold < itemInfo.price)
                {
                    mess = "보유 골드가 모자랍니다";
                }
                else
                {
                    mess = "정말 구매 하시겠습니까";
                    isBuyOK = true;
                }
                break;
            case ItemState.Active:
                int cost = itemInfo.price;
                if (5 < itemInfo.lv)
                {
                    mess = "최고 레벨입니다";
                }
                else if (GlobalValue.userGold < cost)
                {
                    mess = "레벨업에 필요한 보유 골드가 모자랍니다";
                }
                else
                {
                    mess = "정말 업그레이드 하시겠습니까";
                    isBuyOK = true;
                }
                break;
        }

        if (isBuyOK)
            ShowDlgBox(mess, TryBuyItem);
        else
            ShowDlgBox(mess);
    }

    public virtual void TryBuyItem()
    {
        bool isBuyOk = false;
        TowerInfo info = null;
        setLv.Clear();

        //상점 노드 전체를 순회
        foreach (var node in nodeList)
        {
            info = node.twInfo;
            //현재 타워 레벨은 상점 노드의 타워 레벨보다 1 낮으므로
            setLv.Add(info.lv - 1);
            
            //구매할 타워와 타입이 다르거나 최대 레벨을 넘어갈 경우
            if (info.towerType != buyType || 5 < info.lv)
                continue;

            int cost = info.price;

            //가진 골드가 가격보다 적을 경우
            if (GlobalValue.userGold < cost)
                continue;

            myGoldToServer = GlobalValue.userGold;
            myGoldToServer -= cost;
            setLv[info.towerType]++;
            isBuyOk = true;
        }

        if (isBuyOk)
        {
            if (setLv.Count <= 0)
                return;

            if (setLv.Count != nodeList.Length)
                return;

            JSONObject json = new JSONObject();
            JSONArray jArray = new JSONArray();
            for (int i = 0; i < setLv.Count; i++)
            {
                jArray.Add(setLv[i]);
            }
            json.Add("TowerLv", jArray);
            jsonStr = json.ToString();

            StartCoroutine(TowerStoreCo());
        }
    }

    protected virtual IEnumerator TowerStoreCo()
    {
        if (string.IsNullOrEmpty(jsonStr))
            yield break;

        if (string.IsNullOrEmpty(GlobalValue.myInfo.userID))
            yield break;

        WWWForm form = new WWWForm();
        form.AddField("Input_user", GlobalValue.myInfo.userID, System.Text.Encoding.UTF8);
        form.AddField("myGold", myGoldToServer);
        form.AddField("TowerLv", jsonStr, System.Text.Encoding.UTF8);
        isNetworkLock = true;

        UnityWebRequest request = UnityWebRequest.Post(buyReqURL, form);
        yield return request.SendWebRequest();

        if (request.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(request.downloadHandler.data);

            if (sz.Contains("Success"))
                RefreshMyInfoCo();
        }
        else
        {
            Debug.Log(request.error);
        }

        isNetworkLock = false;
    }

    protected virtual void RefreshMyInfoCo()
    {
        if (!GlobalValue.myInfo.TowerLvDic.ContainsKey(buyType))
            return;

        GlobalValue.userGold = myGoldToServer;
        GlobalValue.myInfo.TowerLvDic[buyType]++;

        foreach (var info in DBContainer.towerInfoList)
        {
            if (GlobalValue.myInfo.TowerLvDic[info.towerType] == info.lv - 1)
            {
                nodeList[info.towerType].InitData(info);
            }
        }

        RefreshItemList();
        StoreManager.Inst.RefreshMyInfo();
    }

    protected void ShowDlgBox(string message, ShopDlgBox.DLT_Response DLTMTD = null)
    {
        if (string.IsNullOrEmpty(message))
            return;

        GameObject dlgResource = Resources.Load("DlgBox") as GameObject;
        GameObject obj = Instantiate(dlgResource);
        obj.transform.SetParent(UICanvas, false);

        ShopDlgBox dlgCtrl = obj.GetComponent<ShopDlgBox>();
        if (dlgCtrl != null)
        {
            dlgCtrl.SetMessage(message, DLTMTD);
        }
    }
}
