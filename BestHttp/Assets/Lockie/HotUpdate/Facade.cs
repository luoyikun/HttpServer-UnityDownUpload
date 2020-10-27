using System;
using System.Collections.Generic;
using UnityEngine;

//	Facade.cs
//	Author: Jxw
//	2015-10-16

public class Facade {
 //   protected IController m_controller;
    static GameObject m_GameManager;
    static Dictionary<string, object> m_Managers = new Dictionary<string, object>();
	private static Facade _instance;

	//private  LuaScriptMgr m_LuaMgr;
	//private  ResourceManager m_ResMgr;
	//private  MusicManager m_MusicMgr;
	//private  TimerManager m_TimerMgr;
	private ThreadManager m_ThreadMgr;

	GameObject AppGameManager {
        get {
            if (m_GameManager == null) {
                m_GameManager = GameObject.Find("GameMgr");
                //m_GameManager = new GameObject();
                //m_GameManager.name = "GameManager";
                //m_GameManager.AddComponent<GameManager>();
            }
            return m_GameManager;
        }
    }

	public static Facade Instance
	{
		get{
			if (_instance == null) {
				_instance = new Facade();
			}
			return _instance;
		}
	}
	
    /// <summary>
    /// 添加管理器
    /// </summary>
    public void AddManager(string typeName, object obj) {
        if (!m_Managers.ContainsKey(typeName)) {
            m_Managers.Add(typeName, obj);
        }
    }

    /// <summary>
    /// 添加Unity对象
    /// </summary>
    public T AddManager<T>(string typeName) where T : Component {
        object result = null;
        if (m_Managers.ContainsKey(typeName))
        {
            return null;
        }
        m_Managers.TryGetValue(typeName, out result);
        if (result != null) {
            return (T)result;
        }
        Component c = AppGameManager.AddComponent<T>();
        m_Managers.Add(typeName, c);
        return default(T);
    }

    /// <summary>
    /// 获取系统管理器
    /// </summary>
    public  T GetManager<T>(string typeName) where T : class {
        if (!m_Managers.ContainsKey(typeName)) {
            return default(T);
        }
        object manager = null;
        m_Managers.TryGetValue(typeName, out manager);
        return (T)manager;
    }

    /// <summary>
    /// 删除管理器
    /// </summary>
    public void RemoveManager(string typeName) {
        if (!m_Managers.ContainsKey(typeName)) {
            return;
        }
        object manager = null;
        m_Managers.TryGetValue(typeName, out manager);
        Type type = manager.GetType();
        if (type.IsSubclassOf(typeof(MonoBehaviour))) {
            GameObject.Destroy((Component)manager);
        }
        m_Managers.Remove(typeName);
    }
	
	public  ThreadManager ThreadManager {
		get {
			if (m_ThreadMgr == null) {
				m_ThreadMgr = GetManager<ThreadManager>("ThreadManager");
			}
			return m_ThreadMgr;
		}
	}

}
