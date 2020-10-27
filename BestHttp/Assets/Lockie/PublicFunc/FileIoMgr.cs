
using UnityEngine;
using System.Collections;
using System.IO;

public class FileIoMgr : MonoBehaviour {

    static public void SaveString(string JsonString, string path)    
    {
        if (File.Exists(path) == true)
        {
            File.Delete(path);
        }
        FileInfo file = new FileInfo(path);   
        StreamWriter writer = file.CreateText();
        writer.Write(JsonString);
        writer.Close();
        writer.Dispose();
    }

    static public string GetString(string path)   
    {
        StreamReader reader = new StreamReader(path);
        string jsonData = reader.ReadToEnd();
        reader.Close();
        reader.Dispose();
        return jsonData;
    }
}
