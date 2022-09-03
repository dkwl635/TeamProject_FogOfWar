using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonMgr
{
    public static string DefDeckToStr(Dictionary<TowerType, List<int>> DictowerPos)
    {
        string Return = string.Empty;
        try
        {
            var jsonAllTower = new JSONObject();
            var jsonKeyType = new JSONArray();

            foreach (var item in DictowerPos)
            {
                var jsonTower = new JSONObject();

                var json = new JSONArray();
                for (int i = 0; i < item.Value.Count; i++)
                {
                    json.Add(item.Value[i]);
                }
                jsonTower.Add("P",json);
                jsonKeyType.Add((int)item.Key); //enum -> int -> string
                jsonAllTower.Add(((int)item.Key).ToString(), jsonTower);
            }

            jsonAllTower.Add("K", jsonKeyType);

            Return = jsonAllTower.ToString();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        return Return;
    }

    public static Dictionary<TowerType, List<int>> DefDeckToDic(string str)
    {
        Dictionary<TowerType, List<int>> DictowerPos = new Dictionary<TowerType, List<int>>();
        try
        {
            if (string.IsNullOrEmpty(str) || str == "")
                return DictowerPos;

            DictowerPos.Clear(); // 한번 초기화

            var json = JSON.Parse(str);
            var key = json["K"];
            for (int i = 0; i < key.Count; i++)
            {
                List<int> pos = new List<int>();
                int keyCount = json[key[i].ToString()]["P"].Count;
                int keyint = key[i].AsInt;
                string keystr = key[i].ToString();
                for (int ii = 0; ii < keyCount; ii++)
                    pos.Add(json[keystr]["P"][ii]);

                DictowerPos.Add((TowerType)keyint, pos);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        return DictowerPos;
    }

    public static string AttDeckToStr(int[] arr)
    {
        string Return = string.Empty;
        try
        {
            var json = new JSONArray();
            for (int i = 0; i < arr.Length; i++)
                json.Add(arr[i]);

            Return = json.ToString();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        return Return;
    }

    public static int[] AttDeckToArr(SlotType type, string str)
    {
        int[] ArrReturn = new int[4] { -1, -1, -1, -1 };
        try
        {
            if (string.IsNullOrEmpty(str) || str == "")
                return ArrReturn;

            var json = JSON.Parse(str);
            var value = json[(int)type];
            for (int i = 0; i < value.Count; i++)
                ArrReturn[i] = value[i].AsInt;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        return ArrReturn;
    }
}
