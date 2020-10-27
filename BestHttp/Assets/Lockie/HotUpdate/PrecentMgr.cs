using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrecentData
{
    public float precent;
    public string speed;
    public string total;
}

public class PrecentMgr : MonoBehaviour {
    [SerializeField] Image m_imgPrecent;
    [SerializeField] Text m_textSpeed;
    [SerializeField] Text m_textPrecent;
	// Use this for initialization
	void Start () {
        NotificationCenter.Get().ObjAddEventListener(KEventKey.m_evDownload, gameObject, OnEventDown);
	}

    private void OnDestroy()
    {
        NotificationCenter.Get().ObjRemoveEventListener(KEventKey.m_evDownload, gameObject);
    }
    // Update is called once per frame
    void Update () {
		
	}

    void OnEventDown(Notification noti)
    {
        PrecentData data = (PrecentData)noti.param;
        m_imgPrecent.fillAmount = data.precent;
        m_textSpeed.text = data.speed;
        m_textPrecent.text = data.total;
        //if (data.precent >= 1.0f)
        //{
        //    Destroy(gameObject);
        //}
    }
}
