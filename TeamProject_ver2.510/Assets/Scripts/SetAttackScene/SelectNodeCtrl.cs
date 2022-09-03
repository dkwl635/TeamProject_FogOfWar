using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectNodeCtrl : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler,IPointerEnterHandler,IPointerExitHandler
{
    public RawImage Img;
    public Text lvText;
    private Rect RectSR;

    private GameObject Canvas; // Instantiate 한 Obj 부모 위치를 위해
    private ScrollRect ParentSR; // ScrollView 동작을 위해

    bool IsSelect = false; // SelectItem을 Instantiate 헀는지 여부
    public SelectNode MyItem = new SelectNode();

    public string infoNameStr;
    public string infoStatStr;
    public string infoHelpStr;

    // Start is called before the first frame update
    void Start()
    {
        MyItem.Img = Img;
        Canvas = GameObject.Find("Canvas");
        ParentSR = transform.parent.parent.parent.GetComponent<ScrollRect>();
        Vector2 Size = ParentSR.GetComponent<RectTransform>().sizeDelta;
        Vector2 LTPos = new Vector2(ParentSR.transform.position.x - Size.x / 2f, ParentSR.transform.position.y - Size.y / 2f);
        RectSR = new Rect(LTPos, Size);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // ScrollView도 같이 동작시키기 위해사
        ParentSR.OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // ScrollView도 같이 동작시키기 위해사
        ParentSR.OnEndDrag(eventData);

        if (IsSelect)
        {
            // OnDrop EventTrigger 를 위해
            SetAttackManager.inst.m_SelectNode.Img.raycastTarget = true;
            IsSelect = false;
        }

        // 여기서 Drag용 Obj 삭제
        if (SetAttackManager.inst.m_SelectNode.Img != null)
            Destroy(SetAttackManager.inst.m_SelectNode.Img.gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!RectSR.Contains(eventData.position))
        {
            if (IsSelect == false)
            {
                IsSelect = true;
                SetAttackManager.inst.m_SelectNode.Img = GameObject.Instantiate<RawImage>(MyItem.Img, Canvas.transform);
                SetAttackManager.inst.m_SelectNode.Img.name = "SelectItem"; // Slot에서 Child 이름으로 찾으려고 변경
                SetAttackManager.inst.m_SelectNode.Type = MyItem.Type;
                SetAttackManager.inst.m_SelectNode.TypeIdx = MyItem.TypeIdx;
                // OnDrop EventTrigger 를 위해
                SetAttackManager.inst.m_SelectNode.Img.raycastTarget = false;
            }

            // Item을 Drag 할 경우 ScrollView 동작은 중단하기 위해
            ParentSR.OnEndDrag(eventData);
        }

        // Drag 따라가기
        if (IsSelect)
            SetAttackManager.inst.m_SelectNode.Img.transform.position = eventData.position;
       
        // ScrollView도 같이 동작시키기 위해사
        ParentSR.OnDrag(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetAttackManager.inst.SetInfo(infoNameStr, infoStatStr, infoHelpStr);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetAttackManager.inst.EnemyInfo();
    }
}
