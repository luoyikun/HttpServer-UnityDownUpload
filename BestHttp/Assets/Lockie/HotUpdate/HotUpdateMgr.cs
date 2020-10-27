using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class HotUpdateMgr : MonoBehaviour {
    private List<string> downloadFiles = new List<string>();


    /// <summary>
    /// 初始化游戏管理器
    /// </summary>
    //void Awake()
    //{
    //    Init();
    //}

    ///// <summary>
    ///// 初始化
    ///// </summary>
    //void Init()
    //{
    //    //DontDestroyOnLoad(gameObject);  //防止销毁自己
    //    //		Facade.Instance.AddManager<PanelManager>(ManagerName.Panel);
    //    //		Facade.Instance.AddManager<MusicManager>(ManagerName.Music);
    //    //		Facade.Instance.AddManager<TimerManager>(ManagerName.Timer);
    //    ////			AppFacade.Instance.AddManager<NetworkManager>(ManagerName.Network);
    //    //		Facade.Instance.AddManager<ResourceManager>(ManagerName.Resource);
    //    //Facade.Instance.AddManager<ThreadManager>("ThreadManager");

    //    CheckExtractResource(); //释放资源
    //    Screen.sleepTimeout = SleepTimeout.NeverSleep;
    //    Application.targetFrameRate = AppConst.GameFrameRate;

    //}

    private void Start()
    {
        //NotificationCenter.Get().ObjAddEventListener(KEventKey.m_evDownFinish, gameObject, OnEvDownFinish);
        CheckExtractResource(); //释放资源
    }

    private void OnDestroy()
    {
        //NotificationCenter.Get().ObjRemoveEventListener(KEventKey.m_evDownFinish, gameObject);
    }

    void OnEvDownFinish(Notification noti)
    {
        Destroy(gameObject);
    }
    /// <summary>
    /// 释放资源
    /// </summary>
    public void CheckExtractResource()
    {
        StartCoroutine(OnUpdateResource());
    }

    IEnumerator OnExtractResource()
    {
        string dataPath = AppConst.DataPath;  //数据目录
        string resPath = AppConst.AppContentPath(); //游戏包资源目录

        if (Directory.Exists(dataPath)) Directory.Delete(dataPath, true);
        Directory.CreateDirectory(dataPath);

        string infile = resPath + "/AbBag/files.txt";
        string outfile = dataPath + "/AbBag/files.txt";
        if (File.Exists(outfile)) File.Delete(outfile);

        string message = "正在解包文件:>files.txt";
        Debug.Log(infile);
        Debug.Log(outfile);
        if (Application.platform == RuntimePlatform.Android)
        {
            WWW www = new WWW(infile);
            yield return www;

            if (www.isDone)
            {
                File.WriteAllBytes(outfile, www.bytes);
            }
            yield return 0;
        }
        else File.Copy(infile, outfile, true);
        yield return new WaitForEndOfFrame();

        //释放所有文件到数据目录
        string[] files = File.ReadAllLines(outfile);
        foreach (var file in files)
        {
            string[] fs = file.Split('|');
            infile = resPath + fs[0];  //
            outfile = dataPath + fs[0];

            message = "正在解包文件:>" + fs[0];
            Debug.Log("正在解包文件:>" + infile);
            //                facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);

            string dir = Path.GetDirectoryName(outfile);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if (Application.platform == RuntimePlatform.Android)
            {
                WWW www = new WWW(infile);
                yield return www;

                if (www.isDone)
                {
                    File.WriteAllBytes(outfile, www.bytes);
                }
                yield return 0;
            }
            else
            {
                if (File.Exists(outfile))
                {
                    File.Delete(outfile);
                }
                File.Copy(infile, outfile, true);
            }
            yield return new WaitForEndOfFrame();
        }
        message = "解包完成!!!";
        //            facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);
        yield return new WaitForSeconds(0.1f);

        message = string.Empty;
        //释放完成，开始启动更新资源
        StartCoroutine(OnUpdateResource());
    }

    /// <summary> 
    /// 启动更新下载，这里只是个思路演示，此处可启动线程下载更新
    /// </summary>
    IEnumerator OnUpdateResource()
    {
        if (!AppConst.UpdateMode)
        {
            OnResourceInited();
            yield break;
        }
        string dataPath = AppConst.DataPath + "/AbBag";  //数据目录，根据不同的软件
        string url = AppConst.WebUrl + "/" + DataMgr.m_softId.ToString();
        string message = string.Empty;
        //string random = DateTime.Now.ToString("yyyymmddhhmmss");
        //string listUrl = url + "files.txt?v=" + random;
        string listUrl = url + "/" + DataMgr.m_softId.ToString().ToLower() + "/files.txt";
        Debug.LogWarning("LoadUpdate---->>>" + listUrl);

        WWW www = new WWW(listUrl); yield return www;
        if (www.error != null)
        {
            OnUpdateFailed(string.Empty);
            yield break;
        }

        //文件夹不能删除，而是通过文件列表对比
        if (!Directory.Exists(dataPath + "/" + DataMgr.m_softId.ToString().ToLower()))
        {
            Directory.CreateDirectory(dataPath + "/" + DataMgr.m_softId.ToString().ToLower());
        }
        File.WriteAllBytes(dataPath+"/"+ DataMgr.m_softId.ToString().ToLower() +"/" + "files.txt", www.bytes);
        string filesText = www.text;
        string[] files = filesText.Split('\n');

        Dictionary<string, string> dicTotalDown = new Dictionary<string, string>();
        for (int i = 0; i < files.Length; i++)
        {
            if (string.IsNullOrEmpty(files[i])) continue;
            string[] keyValue = files[i].Split('|');
            string f = keyValue[0];
            string localfile = (dataPath + "/" + f).Trim();
            string path = Path.GetDirectoryName(localfile);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //string fileUrl = url + f + "?v=" + random;
            string fileUrl = url +"/" +f;
            bool canUpdate = !File.Exists(localfile);
            if (!canUpdate)
            {
                string remoteMd5 = keyValue[1].Trim();
                string localMd5 = Util.md5file(localfile);
                canUpdate = !remoteMd5.Equals(localMd5);
                if (canUpdate) File.Delete(localfile);
            }
            if (canUpdate)
            {   //本地缺少文件
                if (DataMgr.m_isTotalDown == false)
                {
                    Debug.Log("开始下载：" + fileUrl);
                    //这里都是资源文件，用线程下载
                    BeginDownload(fileUrl, localfile);
                    while (!(IsDownOK(localfile))) { yield return new WaitForEndOfFrame(); }
                }
                else
                {
                    dicTotalDown[fileUrl] = localfile;
                }
            }
        }
        if (DataMgr.m_isTotalDown == true)
        {
            long countLength = 0;
            int cnt = 0;
            foreach (var it in dicTotalDown)
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(it.Key);
                request.Method = "HEAD";
                countLength += request.GetResponse().ContentLength;
                cnt++;
            }
            float dTotalLength = (float)countLength * 1.0f / (1024 * 1024);
            DataMgr.m_downTotal = countLength;
            Debug.Log("总下载量:" + countLength);
            Debug.Log("下载个数:" + cnt);

            if (cnt > 0)
            {
                //UiMgr.Instance.CreateScreenUi("Lockie/SliderMgr");
                //UiMgr.Instance.m_dicPanel["SliderMgr"].GetComponent<SliderMgr>().m_textTop.text = "正在下载";
            }

            foreach (var it in dicTotalDown)
            {
                Debug.Log("Down:" + it.Key);
                BeginDownload(it.Key, it.Value);
                while (!(IsDownOK(it.Value))) { yield return new WaitForEndOfFrame(); }
            }
        }

        yield return new WaitForEndOfFrame();

        Debug.Log("更新完成");
        OnResourceInited();
    }

    void OnUpdateFailed(string file)
    {
        string message = "更新失败!>" + file;
        Debug.Log(message);
        DataMgr.m_downCur = 0;
        DataMgr.m_downTotal = 0;
        NotificationCenter.Get().ObjDispatchEvent(KEventKey.m_evDownFinish);
        Destroy(gameObject);
        //  facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);
    }

    /// <summary>
    /// 是否下载完成
    /// </summary>
    bool IsDownOK(string file)
    {
        return downloadFiles.Contains(file);
    }

    /// <summary>
    /// 线程下载
    /// </summary>
    void BeginDownload(string url, string file)
    {     //线程下载
        object[] param = new object[2] { url, file };

        ThreadEvent ev = new ThreadEvent();
        ev.Key = NotiConst.UPDATE_DOWNLOAD;
        ev.evParams.AddRange(param);
        ThreadManager.Instance.AddEvent(ev, OnThreadCompleted);   //线程下载
    }

    /// <summary>
    /// 线程完成
    /// </summary>
    /// <param name="data"></param>
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
    /// 资源初始化结束
    /// </summary>
    public void OnResourceInited()
    {
        DataMgr.m_downCur = 0;
        DataMgr.m_downTotal = 0;
        NotificationCenter.Get().ObjDispatchEvent(KEventKey.m_evDownFinish);
        Destroy(gameObject);
        //当更新结束时，执行的操作
    }
}
