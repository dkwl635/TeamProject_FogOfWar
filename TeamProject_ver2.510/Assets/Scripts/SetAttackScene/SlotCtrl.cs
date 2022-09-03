using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SlotType
{
    Skill,
    Unit
}

public class SlotCtrl : MonoBehaviour, IDropHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public SlotType Type;
    public int MySlotIdx = 0;
    private GameObject Canvas;
    public SelectNode InMyItem = new SelectNode();

    public RawImage img;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InMyItem.Img != null)
        {
            SetAttackManager.inst.m_SelectNode.Img = GameObject.Instantiate<RawImage>(InMyItem.Img, Canvas.transform);
            SetAttackManager.inst.m_SelectNode.Img.name = "SelectItem"; // Slot에서 Child 이름으로 찾으려고 변경
            SetAttackManager.inst.m_SelectNode.Type = InMyItem.Type;
            SetAttackManager.inst.m_SelectNode.TypeIdx = InMyItem.TypeIdx;

            if (Type == SlotType.Skill)
                GlobalValue.SkillSet[MySlotIdx] = -1;
            else
                GlobalValue.UnitSet[MySlotIdx] = -1;
            // OnDrop EventTrigger 를 위해
            SetAttackManager.inst.m_SelectNode.Img.raycastTarget = false;
            Destroy(InMyItem.Img.gameObject);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (SetAttackManager.inst.m_SelectNode.Img != null)
            SetAttackManager.inst.m_SelectNode.Img.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerEnter.name == this.name)
        {
            InMyItem.Img = GameObject.Instantiate<RawImage>(SetAttackManager.inst.m_SelectNode.Img, Canvas.transform);
            InMyItem.Img.name = "SelectItem";
            InMyItem.Type = Type;
            InMyItem.TypeIdx = SetAttackManager.inst.m_SelectNode.TypeIdx;

            if (Type == SlotType.Skill)
                GlobalValue.SkillSet[MySlotIdx] = InMyItem.TypeIdx;
            else
                GlobalValue.UnitSet[MySlotIdx] = InMyItem.TypeIdx;

            InMyItem.Img.transform.SetParent(this.transform);
            InMyItem.Img.transform.localPosition = Vector3.zero;
        }

        if(SetAttackManager.inst.m_SelectNode != null)
            Destroy(SetAttackManager.inst.m_SelectNode.Img.gameObject);
    }

    public void OnDrop(PointerEventData eventData)
    {
        // 타입과 맞지 않은 Slot에 Item을 넣으려 한다면 현재 선택중인 Item 삭제
        if (Type != SetAttackManager.inst.m_SelectNode.Type)
        {
            Destroy(SetAttackManager.inst.m_SelectNode.Img);
            return;
        }

        // Item이 있는데 또 올리려한다면 기존 Item 삭제
        if (!ReferenceEquals(InMyItem, null) &&
            !ReferenceEquals(this.transform.Find("SelectItem"), null))
            Destroy(this.transform.Find("SelectItem").gameObject);

        if (SetAttackManager.inst.m_SelectNode.Img != null)
        {
            // 여기서 새로 생성하여 넣고, 기존 생성된 SelectItem은 SelectNodeCtrl의 EndDrag에서 삭제
            // OnDrop이 발생하지 않는 다른 Point에서 EndDrag시 삭제해주기 위해서
            InMyItem.Img = GameObject.Instantiate<RawImage>(SetAttackManager.inst.m_SelectNode.Img, Canvas.transform);
            InMyItem.Img.name = "SelectItem";
            InMyItem.Type = Type;
            InMyItem.TypeIdx = SetAttackManager.inst.m_SelectNode.TypeIdx;

            if (Type == SlotType.Skill)
                GlobalValue.SkillSet[MySlotIdx] = InMyItem.TypeIdx;
            else
                GlobalValue.UnitSet[MySlotIdx] = InMyItem.TypeIdx;

            InMyItem.Img.transform.SetParent(this.transform);
            InMyItem.Img.transform.localPosition = Vector3.zero;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Canvas = GameObject.Find("Canvas");

        InMyItem.Img = img;
    }
}
