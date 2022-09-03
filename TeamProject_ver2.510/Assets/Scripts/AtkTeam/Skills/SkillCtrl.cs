using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCtrl : MonoBehaviour
{
    public GameObject[] targetMark;
    public SkillType type = SkillType.Plane;
    public RawImage iconImg;

    // Start is called before the first frame update
    void Start()
    {
        iconImg = GetComponentInChildren<RawImage>();
        GetComponent<Button>().onClick.AddListener(TargetMarkOn);
    }

    protected virtual void TargetMarkOn()
    {
        GameObject go = Instantiate(targetMark[(int)type]);
        go.transform.localScale = new Vector3(5, 5, 1);
    }
}
