using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class AbResCtrl : MonoBehaviour
{
    public static int DebugLevel = 0; //0:Release 1:Debug 2:Verbose
    static GameObject m_obj;
    static bool m_isHaveInit = false;
    public static bool IsApplePlatform
    {
        get
        {
            return Application.platform == RuntimePlatform.IPhonePlayer ||
                   Application.platform == RuntimePlatform.OSXEditor ||
                   Application.platform == RuntimePlatform.OSXPlayer;
        }
    }

    public GameObject ObjCreateFromAsset(string bundleName,Vector3 pos,Vector3 angle, Vector3 scale, Transform par = null,string assetName = "")
    {
        string asName = "";
        if (assetName == "")
        {
            asName = bundleName;
        }
        else
        {
            asName = assetName;
        }
        var prefab = AbResCtrl.Instance.LoadAsset(bundleName, asName);

        var go = GameObject.Instantiate(prefab);
        go.name = assetName;
        go.transform.parent = par;
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.transform.localEulerAngles = angle;
        go.SetActive(true);
        AbResCtrl.Instance.UnloadAssetBundle(bundleName, false, false);
        return go;
    }

    public string GetText(string bundleName, string assetName,string path = "")
    {
        TextAsset prefab = (TextAsset)AbResCtrl.Instance.LoadAsset(bundleName, assetName, "UnityEngine.TextAsset",path);
        if (prefab == null)
        {
            Debug.Log("文字错误" + bundleName + ".." + assetName);
            return null;
        }
        return (prefab.text);
    }
    
    //asset名与bundle名同名，全小写
    public GameObject ObjCreateFromAssetAndPath(string name, string path,Vector3 pos ,Vector3 angle,Vector3 scale,Transform par = null)
    {
        string assetName = name;
        var prefab = AbResCtrl.Instance.LoadAssetAndPath(name, assetName, path);                                                    

        var go = GameObject.Instantiate(prefab);
        go.name = assetName;
        go.transform.parent = par;
        go.transform.localPosition = pos;
        go.transform.localEulerAngles = angle;
        go.transform.localScale = scale;
        go.SetActive(true);
        AbResCtrl.Instance.UnloadAssetBundle(name, false, false);
        return go;
    }

    public GameObject LoadAssetAndPath(string abname, string assetname, string path = "")
    {
        if (DebugLevel > 1)
            Debug.LogWarning("LoadAsset " + abname + " " + assetname);

        //abname = abname.ToLower();
        AssetBundle bundle = LoadAssetBundle(abname, path);
        if (bundle == null)
        {
            Debug.LogError(String.Format("Load Asset {0} Failed From Bundle {1}", assetname, abname));
            return null;
        }

        var asset = bundle.LoadAsset<GameObject>(assetname);
        if (asset == null)
        {
            Debug.LogError(String.Format("Load Asset {0} Failed From Bundle {1} (Asset Not Found)", assetname, abname));
        }

        return asset;
    }

    private AssetBundle LoadAssetBundle(string abname, string abPath = "")
    {
        if (DebugLevel > 1)
            Debug.LogWarning("LoadAssetBundle " + abname);

        if (!abname.EndsWith(".assetbundle"))
        {
            abname += ".assetbundle";
        }

        AssetBundle bundle = null;
        if (!bundles.ContainsKey(abname))
        {
            byte[] stream = null;
            string uri;
            if (abPath != "")
            {
                uri = abPath + "/" + abname;
            }
            else
            {
                uri = AppConst.DataPath + "/AbBag/"+ abname;
            }
            //Debug.Log("Loading AssetBundle: " + uri);

            if (!File.Exists(uri))
            {
                Debug.LogError(String.Format("AssetBundle {0} Not Found", uri));
                return null;
            }

            stream = File.ReadAllBytes(uri);
            PublicFunc.Encypt(ref stream);

            bundle = AssetBundle.LoadFromMemory(stream);
            stream = null;
            bundles.Add(abname, bundle);

            if (bundleDependencies != null)
            {
                for (int i = 0; i < bundleDependencies[abname].Length; i++)
                {
                    LoadAssetBundle(bundleDependencies[abname][i]);
                }
            }
        }
        else
        {
            bundles.TryGetValue(abname, out bundle);
        }
        return bundle;
    }

    public Sprite GetSprite(string bundleName, string assetName,string path = "")
    {
        Sprite prefab  = (Sprite)AbResCtrl.Instance.LoadAsset(bundleName, assetName, "UnityEngine.Sprite",path);
        if (prefab == null)
        {
            Debug.Log("找不到图" + bundleName + ".." + assetName);
            return null;
        }
        return (prefab);
    }

    public static string StreamingAssetsPath
    {
        get
        {
            string contentPath;
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    contentPath = "jar:file://" + Application.dataPath + "!/assets/";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    contentPath = Application.dataPath + "/Raw/";
                    break;
                default:
                    contentPath = Application.dataPath + "/StreamingAssets/";
                    break;
            }
            return contentPath;
        }
    }


    public static void WriteAllText(string path, string text)
    {
        var fs = File.Open(path, FileMode.Create);
        var sw = new StreamWriter(fs);
        sw.Write(text);
        sw.Close();
        fs.Close();
    }

    public static string ReadAllText(string path)
    {
        var fs = File.Open(path, FileMode.Open);
        var sr = new StreamReader(fs);
        var text = sr.ReadToEnd();
        sr.Close();
        fs.Close();

        return text;
    }

    public static AbResCtrl m_instance;

    private AssetBundleManifest manifest;
    private Dictionary<string, AssetBundle> bundles;// = new Dictionary<string, AssetBundle>();
    private Dictionary<string, string[]> bundleDependencies;// = new Dictionary<string, string[]>();

    static public AbResCtrl Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_obj = new GameObject();
                m_obj.name = "AbResCtrl";
                m_instance = m_obj.AddComponent<AbResCtrl>();
                DontDestroyOnLoad(m_obj);
                m_instance.InitBundleDependencies();
            }
            return m_instance;
        }
    }
    public void InitBundleDependencies()
    {
        bundles = new Dictionary<string, AssetBundle>();
        //bundles.Clear();
        //var uri = DataPath + "StreamingAssets";
        var uri = Application.streamingAssetsPath + "/AbBag";

        if (!File.Exists(uri)) return;

        var stream = File.ReadAllBytes(uri);
        PublicFunc.Encypt(ref stream);

        var assetbundle = AssetBundle.LoadFromMemory(stream);
        manifest = assetbundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        bundleDependencies = new Dictionary<string, string[]>();
        //bundleDependencies.Clear();
        foreach (var b in manifest.GetAllAssetBundles())
        {
            bundleDependencies[b] = manifest.GetAllDependencies(b);
        }

        assetbundle.Unload(false);
    }

    public bool UnloadAssetBundle(string abname, bool unloadAssets, bool unloadDepends)
    {
        if (abname == "fonts.assetbundle")
            return true;

        //abname = abname.ToLower();

        if (!abname.EndsWith(".assetbundle"))
        {
            abname += ".assetbundle";
        }

        if (DebugLevel > 1)
            Debug.LogWarning("UnloadAssetBundle " + abname);

        if (bundles.ContainsKey(abname))
        {
            bundles[abname].Unload(unloadAssets);

            bundles.Remove(abname);

            if (unloadDepends)
            {
                for (int i = 0; i < bundleDependencies[abname].Length; i++)
                {
                    UnloadAssetBundle(bundleDependencies[abname][i], unloadAssets, unloadDepends);
                }
            }
        }

        return false;
    }

    public GameObject LoadAsset(string abname, string assetname)
    {
        if (DebugLevel > 1)
            Debug.LogWarning("LoadAsset " + abname + " " + assetname);

        //abname = abname.ToLower();
        AssetBundle bundle = LoadAssetBundle(abname);
        if (bundle == null)
        {
            Debug.LogError(String.Format("Load Asset {0} Failed From Bundle {1}", assetname, abname));
            return null;
        }

        var asset = bundle.LoadAsset<GameObject>(assetname);
        if (asset == null)
        {
            Debug.LogError(String.Format("Load Asset {0} Failed From Bundle {1} (Asset Not Found)", assetname, abname));
        }

        return asset;
    }

    public UnityEngine.Object LoadAsset(string abname, string assetname, string assettype,string path = "")
    {
        if (DebugLevel > 1)
            Debug.LogWarning("LoadAsset " + abname + " " + assetname);

        abname = abname.ToLower();
        AssetBundle bundle = LoadAssetBundle(abname,path);

        return bundle.LoadAsset(assetname, GetType(assettype));
    }

    public static Type GetType(string TypeName)
    {
        //var type = Type.GetType(TypeName);
        //if (type != null)
        //    return type;

        //var type = Types.GetType(TypeName, "Assembly-CSharp");
        var type = System.Reflection.Assembly.Load("Assembly-CSharp").GetType(TypeName);
        if (type != null)
            return type;

        var assemblyName = TypeName;
        while (assemblyName.LastIndexOf('.') != -1)
        {
            assemblyName = assemblyName.Substring(0, assemblyName.LastIndexOf('.'));
            //type = Types.GetType(TypeName, assemblyName);
            type = System.Reflection.Assembly.Load(assemblyName).GetType(TypeName);
            if (type != null)
                return type;
        }
        return null;
    }


}