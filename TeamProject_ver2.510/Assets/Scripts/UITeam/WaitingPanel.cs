using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingPanel : G_Singleton<WaitingPanel>
{
    GameObject WaitingCanvas;
    GameObject watingManager;

    protected override void Init()
    {
        base.Init();
        WaitingCanvas = Resources.Load("WatingCanvas") as GameObject;
        watingManager = Instantiate(WaitingCanvas);

        DontDestroyOnLoad(watingManager);
    }

    public void SetActive(bool isActive)
    {
        watingManager.SetActive(isActive);
    }
}
