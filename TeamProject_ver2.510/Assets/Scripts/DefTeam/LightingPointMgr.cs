using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightingPointMgr : MonoBehaviour
{
    public SpriteRenderer Img;
    Color color;
    string str;
    int num;
    float delta;

    // Start is called before the first frame update
    void Start()
    {
        color = Img.color;
        num = this.transform.GetSiblingIndex();

    }

    // Update is called once per frame
    void Update()
    {
        delta += Time.deltaTime * 10;
        color.a = ((delta-num) / 50) % 1;
        
        Img.color = color;
    }
}
