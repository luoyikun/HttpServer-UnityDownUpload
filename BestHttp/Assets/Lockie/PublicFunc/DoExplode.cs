using UnityEngine;
using System.Collections;
using DG.Tweening;
public class DoExplode : MonoBehaviour
{
    public float m_explodeTime = 1.0f;
    public float m_resetTime = 1.0f;
    public Vector3 m_oriPos;
    public Vector3 m_oriRotate = Vector3.zero;
    public Vector3 m_oriScale = new Vector3(1, 1, 1);

    public Vector3 m_toPos;
    public Vector3 m_toRotate = Vector3.zero;
    public Vector3 m_toScale = new Vector3(1, 1, 1);
    // Use this for initialization
    void Start()
    {
        NotificationCenter.Get().ObjAddEventListener(KEventKey.m_evExplode, gameObject, OnEventExplode);
        NotificationCenter.Get().ObjAddEventListener(KEventKey.m_evReset, gameObject, OnEventReset);
    }

    private void OnDestroy()
    {
        NotificationCenter.Get().ObjRemoveEventListener(KEventKey.m_evExplode, gameObject);
        NotificationCenter.Get().ObjRemoveEventListener(KEventKey.m_evReset, gameObject);
    }

    void OnEventReset(Notification notific)
    {
        DoReset();
    }
    void OnEventExplode(Notification notific)
    {
        DoOpen();
    }

    public void DoOpen()
    {
        transform.DOLocalMove(m_toPos, m_explodeTime);
        transform.DOLocalRotate(m_toRotate, m_explodeTime);
        transform.DOScale(m_toScale, m_explodeTime);
    }

    public void DoReset()
    {
        transform.DOLocalMove(m_oriPos, m_resetTime);
        transform.DOLocalRotate(m_oriRotate, m_resetTime);
        transform.DOScale(m_oriScale, m_resetTime);
    }
}
