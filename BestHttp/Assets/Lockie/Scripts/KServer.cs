using UnityEngine;
using System.Collections;
using HttpServer;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

public class KServer : MonoBehaviour {
    public Text m_text;
    int m_port = 10089;
	// Use this for initialization
	void Start () {
        //ExamServer server = new ExamServer(PublicFunc.GetIp(), m_port);
        //server.SetRoot(@"D:\KServer");
        //server.Start();
        ////m_text.text = "启动服务器成功，ip地址：" + PublicFunc.GetIp();
        ////Debug.Log(Network.player.ipAddress);
    }
	
	// Update is called once per frame
	void Update () {
	
	} 

    public void Send()
    {
        //StartCoroutine(Post());

        //UnityEvent onEvDownOk = new UnityEvent();
        //onEvDownOk.AddListener(() => { DownForNone(); });
        DownMgr.Instance.DownRes("http://" + PublicFunc.GetIp() + ":" + m_port + "/", AppConst.DataPath, "UnitySetup-Android-Support-for-Editor-2017.4.40c1.exe");

    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 80, 50), "Get"))
        {
            //StartCoroutine(Post());
            //StartCoroutine(SendFile());
            //TraditionPost();

        }

        if (GUI.Button(new Rect(0, 50, 80, 50), "Unload"))
        {
            //using (WebClient client = new WebClient())
            //{

            //    //client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            //    client.UploadFileAsync(new System.Uri("http://" + PublicFunc.GetIp() + ":" + m_port + "/ShuZhuang.rar"), Application.streamingAssetsPath + "/ShuZhuang.rar");
            //}

            using (WebClient client = new WebClient())
            {
                try
                {
                    //同步上传不容易报错
                    byte[] data = client.UploadFile(new Uri("http://" + PublicFunc.GetIp() + ":" + m_port + "/ShuZhuang.rar"), "POST", Application.streamingAssetsPath + "/ShuZhuang.rar");
                    string reply = Encoding.UTF8.GetString(data);


                    
                    //client.UploadFileAsync(new Uri(url), "POST", path);
                    //client.UploadFileCompleted += delegate(object sender, UploadFileCompletedEventArgs e)
                    //{
                    //    byte[] data = (byte[])e.Result;
                    //    string reply = Encoding.UTF8.GetString(data);
                    //};
                }
                catch (Exception e)
                {
                    
                    throw new Exception("文件上传失败:" + e.Message );
                }
            }
        }
    }

    void TraditionPost()
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic["num1"] = "1";
        dic["num2"] = "2";
        StartCoroutine(Post("http://" + PublicFunc.GetIp() + ":" + m_port + "/", dic));
        
    }
    IEnumerator Post()
    {
        WWWForm form = new WWWForm();
        form.AddField("num1", 12);
        form.AddField("num2", 23);
        WWW www = new WWW("http://" + PublicFunc.GetIp() + ":" + m_port + "/", form);
        Debug.Log(www.url);
        yield return www;
        Debug.Log(www.text);
    }

    IEnumerator SendGet(string _url)
    {
        WWW getData = new WWW(_url);
        yield return getData;
        if (getData.error != null)
        {
            Debug.Log(getData.error);
        }
        else
        {
            Debug.Log(getData.text);
        }
    }

    IEnumerator Post(string url, Dictionary<string, string> dic)
    {
        string result = "";
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.Method = "POST";
        req.ContentType = "application/x-www-form-urlencoded";
        //#region 添加Post 参数
        StringBuilder builder = new StringBuilder();
        int i = 0;
        foreach (var item in dic)
        {
            if (i > 0)
                builder.Append("&");
            builder.AppendFormat("{0}={1}", item.Key, item.Value);
            i++;
        }
        byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
        //req.ContentLength = data.Length;
        //using (Stream reqStream = req.GetRequestStream())
        //{
        //    reqStream.Write(data, 0, data.Length);
        //    reqStream.Close();
        //}
        //#endregion
        //HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
        //Stream stream = resp.GetResponseStream();
        ////获取响应内容
        //using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        //{
        //    result = reader.ReadToEnd();
        //}
        //Debug.Log(result);

        WWW www = new WWW("http://" + PublicFunc.GetIp() + ":" + m_port + "/", data);
        Debug.Log(www.url);
        yield return www;
        Debug.Log(www.text);

        //return result;
    }

    IEnumerator SendFile()
    {
        WWWForm form = new WWWForm();
        form.AddField("num1", 12);
        string fileName = @"D:\1.txt";
        byte[] data = FileContent(fileName);
        form.AddBinaryData("1.txt", data);

        WWW www = new WWW("http://" + PublicFunc.GetIp() + ":" + m_port + "/", form);
        Debug.Log(www.url);
        yield return www;
        Debug.Log(www.text);

    }

    private byte[] FileContent(string fileName)
    {
        using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            try
            {
                byte[] buffur = new byte[fs.Length];
                fs.Read(buffur, 0, (int)fs.Length);
                return buffur;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}

public class TestData
{
    public int num1;
    public int num2;
}
