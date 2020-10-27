using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Net;
using System;
using UnityEngine;

//	ThreadManager.cs
//	Author: Jxw
//	2015-10-16

public class ThreadEvent {
    public string Key;
    public List<object> evParams = new List<object>();
}

public class NotiData {
    public string evName;
    public object evParam;

    public NotiData(string name, object param) {
        this.evName = name;
        this.evParam = param;
    }
}

/// <summary>
/// 当前线程管理器，同时只能做一个任务
/// </summary>
public class ThreadManager : MonoBehaviour {
    private Thread thread;
    private Action<NotiData> func;
    private Stopwatch sw = new Stopwatch();
    private string currDownFile = string.Empty;

    static readonly object m_lockObj = new object();
    static Queue<ThreadEvent> events = new Queue<ThreadEvent>();

    delegate void ThreadSyncEvent(NotiData data);
    private ThreadSyncEvent m_SyncEvent;

    long m_lastDown = 0;
    static ThreadManager m_instance = null;
    static public ThreadManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject obj = new GameObject("ThreadManager");
                m_instance = obj.AddComponent<ThreadManager>();
            }
            return m_instance;
        }

    }
    void Awake()
    {
        m_SyncEvent = OnSyncEvent;
        //thread = new Thread(OnUpdate);

        Loom.RunAsync(
             () =>
             {
                 thread = new Thread(OnUpdate);
                 thread.Start();
             }
             );


    }

        // Use this for initialization
        void Start() {
        
        //thread.Start();
    }

        /// <summary>
        /// 添加到事件队列
        /// </summary>
        public void AddEvent(ThreadEvent ev, Action<NotiData> func) {
            lock (m_lockObj) {
                this.func = func;
                events.Enqueue(ev);
            }
        }

        /// <summary>
        /// 通知事件
        /// </summary>
        /// <param name="state"></param>
        private void OnSyncEvent(NotiData data) {
            if (this.func != null) func(data);  //回调逻辑层
            //facade.SendMessageCommand(data.evName, data.evParam); //通知View层
        }

        // Update is called once per frame
        void OnUpdate() {
            while (true) {
                lock (m_lockObj) {
                    if (events.Count > 0) {
                        ThreadEvent e = events.Dequeue();
                        try {
                            switch (e.Key) {
                                case NotiConst.UPDATE_EXTRACT: {     //解压文件
                                    OnExtractFile(e.evParams);
                                }
                                break;
                                case NotiConst.UPDATE_DOWNLOAD: {    //下载文件
                                    OnDownloadFile(e.evParams);
                                }
                                break;
                            }
                        } catch (System.Exception ex) {
                            UnityEngine.Debug.LogError(ex.Message);
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        void OnDownloadFile(List<object> evParams) {
            string url = evParams[0].ToString();    
            currDownFile = evParams[1].ToString();


            

        

        using (WebClient client = new WebClient()) {
                    sw.Start();
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    client.DownloadFileAsync(new System.Uri(url), currDownFile);
                }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
        //UnityEngine.Debug.Log(e.ProgressPercentage);

        //UnityEngine.Debug.Log(string.Format("{0} MB's / {1} MB's",
        //    (e.BytesReceived / 1024d / 1024d).ToString("0.00"),
        //    (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00")));

        DataMgr.m_downCur += (e.BytesReceived - m_lastDown);
        //UnityEngine.Debug.Log("DataMgr.m_downCur:" + DataMgr.m_downCur);
        m_lastDown = e.BytesReceived;

        string value = string.Format("{0} kb/s", (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00"));

        if (DataMgr.m_isTotalDown == false)
        {
            //下载的总量
            PrecentData preData = new PrecentData();
            preData.total = string.Format("{0} MB / {1} MB", (e.BytesReceived / 1024d / 1024d).ToString("0.00"), (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"));
            preData.precent = (float)e.BytesReceived / (float)e.TotalBytesToReceive;


            
            //UnityEngine.Debug.Log(value);
            preData.speed = value;

            Loom.QueueOnMainThread((param) =>
            {
                NotificationCenter.Get().ObjDispatchEvent(KEventKey.m_evDownload, preData);
                UnityEngine.Debug.Log(e.ProgressPercentage);
            }, null);
        }
        else
        {

            Loom.QueueOnMainThread((param) =>
            {
                //if (UiMgr.Instance.IsExist("SliderMgr"))
                //{
                //    UiMgr.Instance.m_dicPanel["SliderMgr"].GetComponent<SliderMgr>().SetSliderValue(value);
                //}
            }, null);
        }

        //NotiData data = new NotiData(NotiConst.UPDATE_PROGRESS, value);
        //if (m_SyncEvent != null) m_SyncEvent(data);

        if (e.ProgressPercentage == 100 && e.BytesReceived == e.TotalBytesToReceive) {
                sw.Reset();
            m_lastDown = 0;
            NotiData data = new NotiData(NotiConst.UPDATE_DOWNLOAD, currDownFile);
                if (m_SyncEvent != null) m_SyncEvent(data);
        }
        }

        /// <summary>
        /// 调用方法
        /// </summary>
        void OnExtractFile(List<object> evParams) {
            

            ///------------------通知更新面板解压完成--------------------
            NotiData data = new NotiData(NotiConst.UPDATE_DOWNLOAD, null);
            if (m_SyncEvent != null) m_SyncEvent(data);
        }

        /// <summary>
        /// 应用程序退出
        /// </summary>
        void OnDestroy() {
            thread.Abort();
        }
    }
