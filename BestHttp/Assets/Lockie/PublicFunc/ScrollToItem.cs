using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public enum ScrollLayoutType
{
    Grid,
    Vertical
}

//只能竖直方向的滚动
public class ScrollToItem : MonoBehaviour {

    public ScrollLayoutType m_type = ScrollLayoutType.Grid;
    public RectTransform m_content;
    public RectTransform m_view;
    [SerializeField] float m_cellY;
    private void Awake()
    {
        //以后通知订阅放awake
        NotificationCenter.Get().ObjAddEventListener(KEventKey.m_evScrollToItem, gameObject, OnEventScroll);
    }

    private void OnDestroy()
    {
        NotificationCenter.Get().ObjRemoveEventListener(KEventKey.m_evScrollToItem, gameObject);
    }
    public void OnEventScroll(Notification noti)
    {
        int idx = (int)noti.param;

        switch (m_type)
        {
            case ScrollLayoutType.Grid:
                ScrolllToIdx(idx);
                break;
            case ScrollLayoutType.Vertical:
                ScrollToIdxVertical(idx);
                break;
            default:
                break;
        }
    }
    
    void ScrolllToIdx(int idx)
    {
        float length = idx * (m_content.GetComponent<GridLayoutGroup>().cellSize.y + m_content.GetComponent<GridLayoutGroup>().spacing.y);
        float ViewProtH = m_content.childCount * (m_content.GetComponent<GridLayoutGroup>().cellSize.y + m_content.GetComponent<GridLayoutGroup>().spacing.y) - m_view.sizeDelta.y - m_content.GetComponent<GridLayoutGroup>().spacing.y;
        if (length > ViewProtH)
        {
            length = ViewProtH;
        }
        DOTween.To(() => m_content.localPosition, x => m_content.localPosition = x, new Vector3(0, length), 0.5f);
    }

    void ScrollToIdxVertical(int idx)
    {
        float length = idx * (m_cellY + m_content.GetComponent<VerticalLayoutGroup>().spacing);
        float ViewProtH = m_content.childCount * (m_cellY + m_content.GetComponent<VerticalLayoutGroup>().spacing) - m_view.sizeDelta.y - m_content.GetComponent<VerticalLayoutGroup>().spacing ;
        if (length > ViewProtH)
        {
            length = ViewProtH;
        }
        DOTween.To(() => m_content.localPosition, x => m_content.localPosition = x, new Vector3(0,length,0), 0.5f);
    }
}
