using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class TankStore : TowerStore
{
    protected override void Init()
    {
        buyReqURL = "http://pmaker.dothome.co.kr/pMaker_7Gi/TankBuy.php";
        foreach (var info in DBContainer.tankInfoList)
        {
            if (!GlobalValue.TankLvDic.ContainsKey(info.tankType))
            {
                GlobalValue.TankLvDic.Add(info.tankType, 0);
            }
            if (GlobalValue.TankLvDic[info.tankType] == info.lv - 1)
            {
                GameObject obj = Instantiate(itemNodePrefab);
                ShopNode node = obj.GetComponent<ShopNode>();
                obj.transform.SetParent(itemSVContent.transform, false);
                node.InitData(info);
            }
        }

        StoreManager.Inst.RefreshMyInfo();
        RefreshItemList();
    }

    public override void BuyItem(int type)
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
        TankInfo itemInfo = nodeList[type].tkInfo;

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

    public override void TryBuyItem()
    {
        bool isBuyOk = false;
        TankInfo info = null;
        setLv.Clear();

        foreach (var node in nodeList)
        {
            info = node.tkInfo;
            setLv.Add(info.lv - 1);

            if (info.tankType != buyType || 5 < info.lv)
                continue;

            int cost = info.price;
            if (GlobalValue.userGold < cost)
                continue;

            myGoldToServer = GlobalValue.userGold;
            myGoldToServer -= cost;
            setLv[info.tankType]++;
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
            json.Add("TankLv", jArray);
            jsonStr = json.ToString();

            StartCoroutine(TowerStoreCo());
        }
    }

    protected override IEnumerator TowerStoreCo()
    {
        if (string.IsNullOrEmpty(jsonStr))
            yield break;

        if (string.IsNullOrEmpty(GlobalValue.myInfo.userID))
            yield break;

        WWWForm form = new WWWForm();
        form.AddField("Input_user", GlobalValue.myInfo.userID, System.Text.Encoding.UTF8);
        form.AddField("myGold", myGoldToServer);
        form.AddField("TankLv", jsonStr, System.Text.Encoding.UTF8);
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

    protected override void RefreshMyInfoCo()
    {
        if (!GlobalValue.TankLvDic.ContainsKey(buyType))
            return;

        GlobalValue.userGold = myGoldToServer;
        GlobalValue.TankLvDic[buyType]++;

        foreach (var info in DBContainer.tankInfoList)
        {
            if (GlobalValue.TankLvDic[info.tankType] == info.lv - 1)
            {
                nodeList[info.tankType].InitData(info);
            }
        }

        RefreshItemList();
        StoreManager.Inst.RefreshMyInfo();
    }
}
