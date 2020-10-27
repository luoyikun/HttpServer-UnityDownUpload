using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class UiMgr : MonoBehaviour {
    public Dictionary<string, GameObject> m_dicPanel = new Dictionary<string, GameObject>();
    static UiMgr m_instance = null;
    public string m_curPanel;
    public Transform m_canvasScreen;
    public Transform m_canvasWorld;
    public delegate void OnLoadUiFinish(GameObject obj);
    static public UiMgr Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject obj = PublicFunc.CreateObjFromRes("Lockie/UiMgr");
                obj.transform.localPosition = Vector3.zero;
                m_instance = obj.GetComponent<UiMgr>();
            }
            return m_instance;
        }
    }

    private void OnDestroy()
    {
        m_dicPanel.Clear();
        m_instance = null;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CreateWorldUi(string name,Transform par = null, OnLoadUiFinish finish = null)
    {
        DeleteNull();
        GameObject obj = PublicFunc.CreateObjFromRes(name);
        if (par != null)
        {
            obj.transform.parent = par;
        }
        else
        {
            obj.transform.parent = m_canvasWorld;
        }
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localEulerAngles = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        //Transform tran = obj.transform.GetChild(0);
       //GameObject winObj = GameObject.Instantiate<GameObject>(tran.gameObject);
        name = PublicFunc.GetFileNameByLine(name);
        m_dicPanel[name] = obj;

        if (finish != null)
        {
            finish(obj);
            Debug.Log(obj.name);
        }
    }

    public void CreateScreenUi(string name,Transform par = null)
    {
        DeleteNull();
        GameObject obj = PublicFunc.CreateObjFromRes(name);
        if (par != null)
        {
            obj.transform.parent = par;
        }
        else
        {
            obj.transform.parent = m_canvasScreen;
        }
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localEulerAngles = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        name = PublicFunc.GetFileNameByLine(name);
        m_dicPanel[name] = obj;
    }

    public void HideUi(string name)
    {
        DeleteNull();
        if (!m_dicPanel.ContainsKey(name))
            return;
        m_dicPanel[name].SetActive(false);
    }

    public void DestroyUi(string name)
    {
        DeleteNull();
        if (!m_dicPanel.ContainsKey(name))
            return;
        Destroy(m_dicPanel[name]);
    }

    public void ShowUi(string name)
    {
        DeleteNull();
        if (!m_dicPanel.ContainsKey(name))
            return;
        m_dicPanel[name].SetActive(true);
    }

    void DeleteNull()
    {
        List<string> listRemoveKey = new List<string>();
        foreach (var it in m_dicPanel)
        {
            if (it.Value == null)
            {
                listRemoveKey.Add(it.Key);
            }
        }

        for (int i = 0; i < listRemoveKey.Count; i++)
        {
            m_dicPanel.Remove(listRemoveKey[i]);
        }
    }

    public bool IsExist(string name)
    {
        if (m_dicPanel.ContainsKey(name))
        {
            return true;
        }
        return false;
    }

    public void Del()
    {
        foreach (var it in m_dicPanel)
        {
            if (it.Value != null)
            {
                Destroy(it.Value);
            }
        }
    }
}
