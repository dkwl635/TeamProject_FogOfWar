using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public GameObject skillNodePrefab;
    public Texture[] skillIconImg;

    // Start is called before the first frame update
    void Start()
    {
        skillIconImg = Resources.LoadAll<Texture>("SkillImg");
        foreach(var type in GlobalValue.SkillSet)
        {
            if (type < 0)
                continue;

            GameObject node = Instantiate(skillNodePrefab, gameObject.transform);
            SkillCtrl sc = node.GetComponent<SkillCtrl>();
            sc.iconImg.texture = skillIconImg[type];
            sc.type = (SkillType)type;
        }
    }
}
