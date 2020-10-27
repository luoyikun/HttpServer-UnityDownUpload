using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class DownMgr : MonoBehaviour
{
    static public DownMgr m_instance = null;
    static GameObject m_obj;
    public static bool DownBool = false;

    //public UnityEvent onEvDownOk = new UnityEvent();
    static public DownMgr Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_obj = new GameObject();
                m_obj.name = "DownMgr";
                m_instance = m_obj.AddComponent<DownMgr>();
                DontDestroyOnLoad(m_obj);
            }
            return m_instance;
        }
    }

    private List<string> downloadFiles = new List<string>();
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DownRes(string uri,string toPath,string fileName,UnityEvent onEvDownOk = null)
    {
        StartCoroutine(YieldDownRes(uri,toPath,fileName, onEvDownOk));    //下载
    }

    IEnumerator YieldDownRes(string uri,string toPath,string fileName, UnityEvent onEvDownOk)
    {
        if (!Directory.Exists(toPath))
        {
            Directory.CreateDirectory(toPath);
        }

        uri += "/" + fileName;
        toPath += "/" + fileName;
        //WWW www = new WWW(uri); yield return www;
        //if (www.error != null)
        //{
        //    OnUpdateFailed(string.Empty);
        //    yield break;
        //}



        //File.WriteAllBytes(toPath, www.bytes);
        Debug.Log("toPath:" + toPath);
        BeginDownload(uri, toPath);
        while (!(IsDownOK(toPath))) { yield return new WaitForEndOfFrame(); }

        yield return new WaitForEndOfFrame();

        // facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);
        Debug.Log("更新完成");
        //OnResourceInited(onEvDownOk);

        if (onEvDownOk != null)
            onEvDownOk.Invoke();
    }

    public void OnResourceInited(UnityEvent onEvDownOk)
    {
        //当更新结束时，执行的操作
        if (onEvDownOk != null)
        onEvDownOk.Invoke();
        //JsonMgr.SaveJsonString();
    }

    void OnUpdateFailed(string file)
    {
        string message = "更新失败!>" + file;
        Debug.Log(message);
    }

    void BeginDownload(string url, string file)
    {     //线程下载
        object[] param = new object[2] { url, file };

        ThreadEvent ev = new ThreadEvent();
        ev.Key = NotiConst.UPDATE_DOWNLOAD;
        ev.evParams.AddRange(param);
        //Facade.Instance.ThreadManager.AddEvent(ev, OnThreadCompleted);   //线程下载
        ThreadManager.Instance.AddEvent(ev, OnThreadCompleted);   //线程下载
    }

    void OnThreadCompleted(NotiData data)
    {
        switch (data.evName)
        {
            case NotiConst.UPDATE_EXTRACT:  //解压一个完成
                                            //
                break;
            case NotiConst.UPDATE_DOWNLOAD: //下载一个完成
                downloadFiles.Add(data.evParam.ToString());
                break;
        }
    }

    /// <summary>
    /// 是否下载完成
    /// </summary>
    bool IsDownOK(string file)
    {
        return downloadFiles.Contains(file);
    }

}