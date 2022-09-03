using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopDlgBox : MonoBehaviour
{
    public delegate void DLT_Response();
    DLT_Response DLTMethod;

    public Button okBtn;
    public Button closeBtn;
    public Button cancelBtn;
    public Text contentsText;

    // Start is called before the first frame update
    void Start()
    {
        if (okBtn != null)
            okBtn.onClick.AddListener(() =>
            {
                if (DLTMethod != null)
                    DLTMethod();
                Destroy(gameObject);
            });
        if (closeBtn != null)
            closeBtn.onClick.AddListener(() =>
            {
                Destroy(gameObject);
            });
        if (cancelBtn != null)
            cancelBtn.onClick.AddListener(() =>
            {
                Destroy(gameObject);
            });
    }

    public void SetMessage(string mess, DLT_Response dltMtd = null)
    {
        contentsText.text = mess;
        DLTMethod = dltMtd;
    }
}
