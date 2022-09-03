using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProfileScrollView : MonoBehaviour
{
    public bool isSelected = false;
    Vector3 selPos = Vector3.zero;
    Vector3 deselPos = new Vector3(-1260, 0, 0);
    float speed = 12600f;

    public GameObject ProfileNodePrefab;
    public GameObject Content;

    // Start is called before the first frame update
    void Start()
    {
        if (ProfileNodePrefab != null && Content != null)
        {
            Sprite[] ProfileImgs = Resources.LoadAll<Sprite>("ProfileImage");
            foreach (Sprite ProfileImg in ProfileImgs)
            {
                int Idx = int.Parse(ProfileImg.name.Replace("Character", ""));
                GameObject ProfileNode = Instantiate(ProfileNodePrefab);
                ProfileNode.GetComponent<ProfileListNodeCtrl>().Init(Idx, ProfileImg);
                ProfileNode.name = string.Format("ProfileNode_{0}", Idx.ToString("00"));
                ProfileNode.transform.SetParent(Content.transform);
                ProfileNode.transform.localScale = new Vector3(1f, 1f, 1f); // Scale이 왜 1.5로 변하는지 모르겠다.
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveUIObject();
    }

    void MoveUIObject()
    {    
        if (!isSelected)
        {
            if (transform.localPosition.x > deselPos.x)
            {
                transform.localPosition =
                    Vector3.MoveTowards(transform.localPosition,
                                        deselPos, speed * Time.deltaTime);
            }
        }
        else
        {
            if (selPos.x > transform.localPosition.x)
            {
                transform.localPosition =
                    Vector3.MoveTowards(transform.localPosition,
                                        selPos, speed * Time.deltaTime);
            }
        }
    }
}
