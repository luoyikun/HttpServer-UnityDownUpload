using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System;
using LitJson;
using UnityEngine.Events;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public class PublicFunc : MonoBehaviour
{

    static PublicFunc m_instance = null;
    static GameObject m_publicObj = null;
    static Coroutine m_corUiPlaneMove = null;
    static Coroutine m_corCreateFromRes = null;//异步创建obj
    public static PublicFunc Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_publicObj = new GameObject();
                m_publicObj.name = "PublicFunc";
                m_instance = m_publicObj.AddComponent<PublicFunc>();
            }
            return m_instance;
        }
    }

    public static void AddMeshCollider(GameObject obj)
    {
        foreach (var render in obj.transform.GetComponentsInChildren<MeshRenderer>())
        {
            if (render.gameObject.GetComponent<MeshCollider>() == null)
            render.gameObject.AddComponent<MeshCollider>();
        }

        foreach (var skinRender in obj.transform.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (skinRender.gameObject.GetComponent<MeshCollider>() == null)
                skinRender.gameObject.AddComponent<MeshCollider>();
        }
    }


    public static void RemoveMeshCollider(GameObject obj)
    {
        foreach (var render in obj.transform.GetComponentsInChildren<MeshCollider>())
        {
            Destroy(render);
        }
    }
    public static void ChangeTag(GameObject obj, string sTag)
    {
        //给未赋值的放tag
        if (obj.gameObject.tag == DataMgr.m_tagUntagged)
            obj.tag = sTag;
        foreach (var trans in obj.transform.GetComponentsInChildren<Transform>())
        {
            if (trans.gameObject.tag == DataMgr.m_tagUntagged)
                trans.gameObject.tag = sTag;
        }
    }

    public static void ChangeTagUntagged(GameObject obj)
    {
        obj.tag = DataMgr.m_tagUntagged;
        foreach (var trans in obj.transform.GetComponentsInChildren<Transform>())
        {
                trans.gameObject.tag = DataMgr.m_tagUntagged;
        }
    }

    public static void ChangeTagForce(GameObject obj, string sTag)
    {
        foreach (var trans in obj.transform.GetComponentsInChildren<Transform>())
        {
            trans.gameObject.tag = sTag;
        }
    }
    public static void ObjSetDrag(GameObject obj)
    {
        ChangeTag(obj, DataMgr.m_tagDrag);
        AddMeshCollider(obj);
    }
    public static void SaveOriPos(GameObject obj)
    {
        DataMgr.m_listHideObj.Clear();
        DataMgr.m_dicOriPos.Clear();
        DataMgr.m_dicOriQua.Clear();
        DataMgr.m_dicOriLocalPos.Clear();
        DataMgr.m_dicOriLocalQua.Clear();
        DataMgr.m_dicOriLocalScale.Clear();

        DataMgr.m_dicOriPos[obj] = obj.transform.position;
        DataMgr.m_dicOriQua[obj] = obj.transform.rotation;
        DataMgr.m_dicOriLocalScale[obj] = obj.transform.localScale;
        DataMgr.m_dicOriLocalPos[obj] = obj.transform.localPosition;
        DataMgr.m_dicOriLocalQua[obj] = obj.transform.localRotation;

        foreach (var trans in obj.transform.GetComponentsInChildren<Transform>())
        {

            DataMgr.m_dicOriPos[trans.gameObject] = trans.position;
            DataMgr.m_dicOriQua[trans.gameObject] = trans.rotation;

            DataMgr.m_dicOriLocalPos[trans.gameObject] = trans.localPosition;
            DataMgr.m_dicOriLocalQua[trans.gameObject] = trans.localRotation;
            DataMgr.m_dicOriLocalScale[trans.gameObject] = trans.localScale;
        }
    }

    public static void SaveOriPosDotween(GameObject obj)
    {
        DataMgr.m_listHideObj.Clear();

        DataMgr.m_dicOriLocalPos.Clear();
        DataMgr.m_dicOriLocalEuler.Clear();
        DataMgr.m_dicOriLocalScale.Clear();
        DataMgr.m_dicOriLocalQua.Clear();

        DataMgr.m_dicOriPos.Clear();
        DataMgr.m_dicOriEuler.Clear();
        DataMgr.m_dicOriScale.Clear();
        DataMgr.m_dicOriQua.Clear();
        //DataMgr.m_dicOriPos[obj] = obj.transform.position;
        //DataMgr.m_dicOriQua[obj] = obj.transform.rotation;
        //DataMgr.m_dicOriLocalScale[obj] = obj.transform.localScale;
        //DataMgr.m_dicOriLocalPos[obj] = obj.transform.localPosition;
        //DataMgr.m_dicOriLocalQua[obj] = obj.transform.localRotation;

        DataMgr.m_dicOriLocalPos[obj] = obj.transform.localPosition;
        DataMgr.m_dicOriLocalEuler[obj] = obj.transform.localEulerAngles;
        DataMgr.m_dicOriLocalScale[obj] = obj.transform.localScale;
        DataMgr.m_dicOriLocalQua[obj] = obj.transform.localRotation;

        DataMgr.m_dicOriPos[obj] = obj.transform.position;
        DataMgr.m_dicOriEuler[obj] = obj.transform.eulerAngles;
        DataMgr.m_dicOriScale[obj] = obj.transform.localScale;
        DataMgr.m_dicOriQua[obj] = obj.transform.rotation;

        foreach (var trans in obj.transform.GetComponentsInChildren<Transform>())
        {

            //DataMgr.m_dicOriPos[trans.gameObject] = trans.position;
            //DataMgr.m_dicOriQua[trans.gameObject] = trans.rotation;

            DataMgr.m_dicOriLocalPos[trans.gameObject] = trans.localPosition;
            DataMgr.m_dicOriLocalEuler[trans.gameObject] = trans.localEulerAngles;
            DataMgr.m_dicOriLocalScale[trans.gameObject] = trans.localScale;
            DataMgr.m_dicOriLocalQua[trans.gameObject] = trans.localRotation;

            DataMgr.m_dicOriPos[trans.gameObject] = trans.position;
            DataMgr.m_dicOriEuler[trans.gameObject] = trans.eulerAngles;
            DataMgr.m_dicOriScale[trans.gameObject] = trans.localScale;
            DataMgr.m_dicOriQua[trans.gameObject] = trans.rotation;
        }
    }

    //全体差值还原，包括一开始的位置，方向，缩放
    public static void RestoreOriPos(GameObject obj, float fTime = 0.01f)
    {
        //AllChildSetActive(obj);
        //Instance.StopCoroutine()
        for (int i = 0; i < DataMgr.m_listHideObj.Count; i++)
        {
            DataMgr.m_listHideObj[i].SetActive(true);
        }
        Instance.StopCoroutine("YieldRestoreOriPos");
        Instance.StartCoroutine("YieldRestoreOriPos", obj);//
        //Instance.StartCoroutine (Instance.YieldRestoreOriPos (obj));
        //StartEnableToPpt(fTime*101);
        DataMgr.m_isEnableDrag = false;
        DataMgr.m_isEnableToPpt = false;
        DataMgr.m_listHideObj.Clear();
    }

    public static void RestoreOriPosDotween(GameObject obj, float time = 1.0f)
    {
        for (int i = 0; i < DataMgr.m_listHideObj.Count; i++)
        {
            DataMgr.m_listHideObj[i].SetActive(true);
        }
        obj.transform.DOScale(DataMgr.m_oriScale, time);
        foreach (var trans in obj.transform.GetComponentsInChildren<Transform>())
        {
            trans.DOMove(DataMgr.m_dicOriPos[obj], time);
            trans.DORotate(DataMgr.m_dicOriEuler[obj], time);
        }
    }

    //所有的子物体返回位置
    public static void RestoreAllChildOriLocalPos(GameObject obj)
    {
        Instance.StopCoroutine("YieldAllChildRestoreOriLocalPos");
        Instance.StartCoroutine("YieldAllChildRestoreOriLocalPos", obj);
    }


    public static void RestoreOriLocalPos(GameObject obj)
    {
        Instance.StopCoroutine("YieldRestoreOriLocalPos");
        Instance.StartCoroutine("YieldRestoreOriLocalPos", obj);
        //DataMgr.m_isMoveOri = true;
        //if (DataMgr.m_coMoveOri!= null)
        //{
        //    Instance.StopCoroutine(DataMgr.m_coMoveOri);
        //}
        //DataMgr.m_coMoveOri = Instance.StartCoroutine(Instance.YieldRestoreOriLocalPos(obj));
    }

    public static void RestoreOriPosImmediate(GameObject obj)
    {
        obj.transform.position = DataMgr.m_dicOriPos[obj];
        obj.transform.rotation = DataMgr.m_dicOriQua[obj];
        //obj.transform.localScale = DataMgr.m_dicOriLocalScale
        foreach (var trans in obj.transform.GetComponentsInChildren<Transform>())
        {
            trans.position = DataMgr.m_dicOriPos[trans.gameObject];
            trans.rotation = DataMgr.m_dicOriQua[trans.gameObject];
        }
    }

    public static void RestoreAllChildOriPosImmediate(GameObject obj)
    {
        foreach (var trans in obj.transform.GetComponentsInChildren<Transform>())
        {
            trans.position = DataMgr.m_dicOriPos[trans.gameObject];
            trans.rotation = DataMgr.m_dicOriQua[trans.gameObject];
        }
    }


    public static void RestoreAllChildOriLocalPosImmediate(GameObject obj)
    {
        obj.transform.position = DataMgr.m_dicOriPos[obj];
        obj.transform.rotation = DataMgr.m_dicOriQua[obj];
        obj.transform.localScale = DataMgr.m_oriScale;
        foreach (var it in DataMgr.m_dicOriLocalPos)
        {
            it.Key.transform.localPosition = it.Value;
            if (DataMgr.m_dicOriLocalQua.ContainsKey(it.Key))
            {
                it.Key.transform.localRotation = DataMgr.m_dicOriLocalQua[it.Key];
            }
        }
        //foreach (var trans in obj.transform.GetComponentsInChildren<Transform>())
        //{
        //    trans.localPosition = DataMgr.m_dicOriLocalPos[trans.gameObject];
        //    trans.localRotation = DataMgr.m_dicOriLocalQua[trans.gameObject];
        //}
    }

    IEnumerator YieldRestoreOriPos(GameObject obj)
    {
        int i = 0;
        float t = Time.time;
        while (i <= 100)
        {
            //obj.transform.position = Vector3.Lerp (obj.transform.position, DataMgr.m_dicOriPos [obj], (float)i / 100.0f);
            //obj.transform.rotation = Quaternion.Lerp (obj.transform.rotation, DataMgr.m_dicOriQua [obj], (float)i / 100.0f);
            //obj.transform.localScale = Vector3.Lerp (obj.transform.localScale, DataMgr.m_oriScale, (float)i / 100.0f);
            //foreach (var trans in obj.transform.GetComponentsInChildren<Transform>())
            //{
            //    if (DataMgr.m_dicOriLocalPos.ContainsKey(trans.gameObject) && trans.localPosition != DataMgr.m_dicOriLocalPos[trans.gameObject])
            //    {
            //        trans.localPosition = Vector3.Lerp(trans.localPosition, DataMgr.m_dicOriLocalPos[trans.gameObject], (float)i / 100.0f);
            //    }
            //    if (DataMgr.m_dicOriLocalQua.ContainsKey(trans.gameObject) && trans.localRotation != DataMgr.m_dicOriLocalQua[trans.gameObject])
            //    {
            //        trans.localRotation = Quaternion.Lerp(trans.localRotation, DataMgr.m_dicOriLocalQua[trans.gameObject], (float)i / 100.0f);
            //    }
            //}
            foreach (var it in DataMgr.m_dicOriLocalPos)
            {
                it.Key.transform.localPosition = Vector3.Lerp(it.Key.transform.localPosition, it.Value, (float)i / 100.0f);
                it.Key.transform.localRotation = Quaternion.Lerp(it.Key.transform.localRotation, DataMgr.m_dicOriLocalQua[it.Key], (float)i / 100.0f);
                it.Key.transform.localScale = Vector3.Lerp(it.Key.transform.localScale, DataMgr.m_dicOriLocalScale[it.Key], (float)i / 100.0f);
            }
            i += 2;
            //yield return new WaitForSeconds (fTime);
            yield return null;
        }
        float timeEnd = Time.time;
        float timeTotal = timeEnd - t;
        DataMgr.m_isEnableDrag = true;
        DataMgr.m_isEnableToPpt = true;
    }

    IEnumerator YieldAllChildRestoreOriLocalPos(GameObject obj)
    {
        int i = 0;
        while (i <= 100/* && DataMgr.m_isMoveOri == true*/)
        {
            foreach (var trans in obj.transform.GetComponentsInChildren<Transform>())
            {
                trans.position = Vector3.Lerp(trans.position, DataMgr.m_dicOriPos[trans.gameObject], (float)i / 100.0f);
                trans.rotation = Quaternion.Lerp(trans.rotation, DataMgr.m_dicOriQua[trans.gameObject], (float)i / 100.0f);
            }
            i += 2;
            yield return new WaitForSeconds(0.001f);
        }
    }

    IEnumerator YieldRestoreOriLocalPos(GameObject obj)
    {
        int i = 0;
        while (i <= 100 && obj != null)
        {
            if (DataMgr.m_dicOriLocalPos.ContainsKey(obj))
                obj.transform.localPosition = Vector3.Lerp(obj.transform.localPosition, DataMgr.m_dicOriLocalPos[obj], (float)i / 100.0f);
            if (DataMgr.m_dicOriLocalQua.ContainsKey(obj))
                obj.transform.localRotation = Quaternion.Lerp(obj.transform.localRotation, DataMgr.m_dicOriLocalQua[obj], (float)i / 100.0f);
            if (DataMgr.m_dicOriLocalScale.ContainsKey(obj))
                obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, DataMgr.m_dicOriLocalScale[obj], (float)i / 100.0f);
            i +=2;
            yield return new WaitForSeconds(0.001f);
        }
    }

    static public void StopYieldRestoreOriLocalPos()
    {
        Instance.StopCoroutine("YieldRestoreOriLocalPos");
    }

    public static void CreateClone(GameObject obj, bool isHaveOri = true)
    {
        DeleteClone();
        GameObject clone = Instantiate(obj);
        clone.transform.parent = obj.transform.parent;
        if (isHaveOri == true)
        {
            clone.transform.localScale = DataMgr.m_dicOriLocalScale[obj.gameObject];
            clone.transform.localPosition = DataMgr.m_dicOriLocalPos[obj.gameObject];
            clone.transform.localRotation = DataMgr.m_dicOriLocalQua[obj.gameObject];
        }
        else
        {
            clone.transform.localScale = obj.transform.localScale;
            clone.transform.localPosition = obj.transform.localPosition;
            clone.transform.localRotation = obj.transform.localRotation;
        }
        if (clone.GetComponent<MeshCollider>() != null)
        {
            Destroy(clone.GetComponent<MeshCollider>());
        }
        clone.transform.tag = DataMgr.m_tagUnuse;

        List<GameObject> lineTip = new List<GameObject>();//获取标签页
        foreach(var it in clone.transform.GetComponentsInChildren<Transform>())
        {
            if (it.transform.name == "LineTip")
            {
                lineTip.Add(it.gameObject);
            }

            if (it.GetComponent<Canvas>() != null)
            {
                lineTip.Add(it.gameObject);
            }
        }

        for (int i = 0; i < lineTip.Count; i++)
        {
            //lineTip.RemoveAt(i);
            Destroy(lineTip[i]);
        }

        foreach (var trans in clone.transform.GetComponentsInChildren<Renderer>())
        {
            Material mat = new Material(Shader.Find("Custom/TwoSideAlpha"));
            Material[] newBufMat = new Material[trans.transform.GetComponent<Renderer>().materials.Length];
            for (int i = 0; i < trans.transform.GetComponent<Renderer>().materials.Length; i++)
            {
                newBufMat[i] = mat;
            }
            //for (int i = 0; i < trans.transform.GetComponent<Renderer>().materials.Length; i++)
            //{
            //    //trans.transform.GetComponent<Renderer>().materials[i].shader = Shader.Find("Custom/TwoSideAlpha");
            //    trans.transform.GetComponent<Renderer>().materials[i] = PenMgr.mInstance.m_alphaMat;
            //}

            trans.transform.GetComponent<Renderer>().materials = newBufMat;
        }

        //if (clone.GetComponent<SkinnedMeshRenderer>() != null)
        //{
        //    for (int i = 0; i < clone.GetComponent<SkinnedMeshRenderer>().materials.Length; i++)
        //    {
        //        clone.GetComponent<SkinnedMeshRenderer>().materials[i].shader = Shader.Find("Custom/TwoSideAlpha");
        //    }
        //}
        //else
        //{
        //    foreach (var trans in clone.transform.GetComponentsInChildren<SkinnedMeshRenderer>())
        //    {
        //        for (int i = 0; i < trans.transform.GetComponent<SkinnedMeshRenderer>().materials.Length; i++)
        //        {
        //            trans.transform.GetComponent<SkinnedMeshRenderer>().materials[i].shader = Shader.Find("Custom/TwoSideAlpha");
        //        }
        //    }
        //}
        DataMgr.m_curAdsorbObj = clone;
    }

    static public void TransChangeAlphaShader(GameObject obj)
    {
        Material mat = new Material(Shader.Find("Custom/TwoSideAlpha"));
        mat.SetColor("_ColorWithAlpha_Front", new Color(1,1,1,0.04f));
        mat.SetColor("_ColorWithAlpha_Back", new Color(1, 1, 1, 0.04f));
        foreach (var trans in obj.transform.GetComponentsInChildren<Renderer>())
        {
            Material[] newBufMat = new Material[trans.transform.GetComponent<Renderer>().materials.Length];
            for (int i = 0; i < trans.transform.GetComponent<Renderer>().materials.Length; i++)
            {
                newBufMat[i] = mat;
            }
            trans.transform.GetComponent<Renderer>().materials = newBufMat;
        }
    }

    static public void TransOneChangeAlphaShader(GameObject obj)
    {
        Material mat = new Material(Shader.Find("Custom/TwoSideAlpha"));
        mat.SetColor("_ColorWithAlpha_Front", new Color(1, 1, 1, 0.04f));
        mat.SetColor("_ColorWithAlpha_Back", new Color(1, 1, 1, 0.04f));
        
        Material[] newBufMat = new Material[obj.transform.GetComponent<Renderer>().materials.Length];
        for (int i = 0; i < obj.transform.GetComponent<Renderer>().materials.Length; i++)
        {
            newBufMat[i] = mat;
        }

        obj.transform.GetComponent<Renderer>().materials = newBufMat;
    }

    public static void DeleteClone()
    {
        if (DataMgr.m_curAdsorbObj != null)
        {
            Destroy(DataMgr.m_curAdsorbObj);
            DataMgr.m_curAdsorbObj = null;
        }
    }

    public static void CloneSetFalse()
    {
        if (DataMgr.m_curAdsorbObj != null)
        {
            DataMgr.m_curAdsorbObj.SetActive(false);
        }
    }


    static public bool IsAdsorb(GameObject objCur, GameObject objClone)
    {
        bool isAdsorb = false;

        {
            float dis = Vector3.Distance(objCur.transform.position, objClone.transform.position);
            float angle = GetAngle(objCur.transform.forward, objClone.transform.forward);
            //if (dis <= 0.02f && angle <= 30.0f)
            if (dis <= 0.03f)
            {
                isAdsorb = true;
            }
        }
        return isAdsorb;
    }

    public static float GetAngle(Vector3 from_, Vector3 to_)
    {
        Vector3 v3 = Vector3.Cross(from_, to_);
        return Vector3.Angle(from_, to_);
    }

    static public void AdsorbDeal()
    {
        if (DataMgr.m_isPenMidPress == false && DataMgr.m_curSelectObj != null && DataMgr.m_curAdsorbObj != null)
        {
            bool isAdsorb = IsAdsorb(DataMgr.m_curSelectObj, DataMgr.m_curAdsorbObj);
            if (isAdsorb == true)
            {
                //DataMgr.m_curSelectObj.transform.localPosition = DataMgr.m_dicOriLocalPos[DataMgr.m_curSelectObj];
                //DataMgr.m_curSelectObj.transform.localRotation = DataMgr.m_dicOriLocalQua[DataMgr.m_curSelectObj];
                RestoreOriLocalPos(DataMgr.m_curSelectObj);
                DeleteClone();
            }
        }
    }

    //中键松开手后还原到原来的位置
    static public void MidUpReturn(GameObject obj)
    {
        obj.transform.DOMove(DataMgr.m_curAdsorbObj.transform.position, 0.5f);
        obj.transform.DORotate(DataMgr.m_curAdsorbObj.transform.eulerAngles, 0.5f);
    }

    static public void MovePos()
    {
        bool isAdsorb = IsAdsorb(DataMgr.m_moveObj, DataMgr.m_moveToObj);
        if (isAdsorb == true)
        {
            Instance.StartCoroutine(Instance.YieldMoveToPos(0.01f, DataMgr.m_moveObj, DataMgr.m_moveToObj));
        }
    }

    static public void MovePosClose()
    {
        bool isAdsorb = IsAdsorb(DataMgr.m_moveObj, DataMgr.m_moveToObj);
        if (isAdsorb == true)
        {
            Instance.StartCoroutine(Instance.YieldMoveToPos(0.01f, DataMgr.m_moveObj, DataMgr.m_moveToObj, isclose: true));
        }
    }

    IEnumerator YieldMoveToPos(float fTime, GameObject obj, GameObject abObj, bool isclose = false)
    {
        int i = 0;
        obj.transform.parent = abObj.transform.parent;
        while (i <= 100 && obj != null)
        {
            obj.transform.position = Vector3.Lerp(obj.transform.position, abObj.transform.position, (float)i / 100.0f);
            obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, abObj.transform.rotation, (float)i / 100.0f);
            obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, abObj.transform.localScale, (float)i / 100.0f);
            i++;
            if (i == 50)
            {
                //abObj.SetActive(false);
                //吸附完成了,发出通知
                if (isclose)
                {
                    obj.GetComponent<BoxCollider>().enabled = false;
                }
                NotificationCenter.Get().ObjDispatchEvent(KEventKey.m_evAbEnd);
            }
            yield return new WaitForSeconds(fTime);
        }
    }

    static public void AllChildSetActive(GameObject obj)
    {
        obj.SetActive(true);
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            AllChildSetActive(obj.transform.GetChild(i).gameObject);
        }
    }

    public static void RemoveFromChild(Transform trans)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            Destroy(trans.GetChild(i).gameObject);
        }
    }

    public static void ShaderChangeTrans(GameObject obj)
    {
        foreach (var mesh in obj.transform.GetComponentsInChildren<MeshRenderer>())
        {
            for (int i = 0; i < mesh.materials.Length; i++)
            {
                mesh.materials[i].shader = Shader.Find("Custom/TwoSideAlpha");
            }
        }
    }

    public static void RemoveBoxCollider(GameObject obj)
    {
        foreach (var trans in obj.transform.GetComponentsInChildren<Transform>())
        {
            if (trans.GetComponent<BoxCollider>() != null)
            {
                Destroy(trans.GetComponent<BoxCollider>());
            }
        }
    }

    public static void CommonShowObj(GameObject obj)
    {
        AddMeshCollider(obj);
        ChangeTag(obj, DataMgr.m_tagDrag);
        DataMgr.m_curShowObj = obj;
        SaveOriPos(obj);
        DataMgr.m_oriScale = obj.transform.localScale;

        DoExplode doEx = obj.GetComponent<DoExplode>();
        if (doEx != null)
        {
            doEx.m_oriPos = doEx.m_toPos = obj.transform.localPosition;
            doEx.m_oriRotate = doEx.m_toRotate = obj.transform.localEulerAngles;
            doEx.m_oriScale = doEx.m_toScale = obj.transform.localScale;
        }
    }

    public static void DelayChageTransShader(float fTime, GameObject obj, float r, float g, float b, float a)
    {
        Instance.StartCoroutine(Instance.YieldChageTransShader(fTime, obj, r, g, b, a));
    }

    IEnumerator YieldChageTransShader(float fTime, GameObject obj, float r, float g, float b, float a)
    {
        yield return new WaitForSeconds(fTime);
        ShaderChangeTransColor(obj, r, g, b, a);
    }

    //shader为标准模式
    public static void ShaderReturn(GameObject obj)
    {
        if (obj != null)
        {
            foreach (var mesh in obj.transform.GetComponentsInChildren<MeshRenderer>())
            {
                for (int i = 0; i < mesh.materials.Length; i++)
                {
                    mesh.materials[i].shader = Shader.Find("Standard");
                }
            }
        }
    }

    public static void ShaderChangeTransColor(GameObject obj, float r, float g, float b, float a)
    {
        if (obj != null)
        {
            foreach (var mesh in obj.transform.GetComponentsInChildren<MeshRenderer>())
            {
                for (int i = 0; i < mesh.materials.Length; i++)
                {
                    mesh.materials[i].shader = Shader.Find("Custom/TwoSideAlpha");
                    mesh.materials[i].SetVector("_ColorWithAlpha_Front", new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f));
                    mesh.materials[i].SetVector("_ColorWithAlpha_Back", new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f));
                }
            }
        }
    }

    public static void RightUiUpdate(int leftIdx, int rightIdx)
    {
        //GameObject par = GameObject.Find("TmpUi");
        //GameObject m_rightPar = par.transform.Find("RightPar").gameObject;
        //GameObject btnLast = m_rightPar.transform.Find("BtnLast").gameObject;
        //GameObject btnNext = m_rightPar.transform.Find("BtnNext").gameObject;
        //btnNext.SetActive(true);
        //btnLast.SetActive(true);
        //string sceneName = SceneManager.GetActiveScene().name;
        //string sChapter = sceneName + leftIdx;
        //Text textTitle = m_rightPar.transform.Find("TextTitle").GetComponent<Text>();
        //textTitle.text = dataManage.Instance(sChapter).get(rightIdx, "Title");

        //string sType = dataManage.Instance(sChapter).get(rightIdx, "Type");
        //GameObject objText = m_rightPar.transform.Find("TextContent").gameObject;
        //GameObject objSpr = m_rightPar.transform.Find("ImgPar").gameObject;

        //Text textIdx = m_rightPar.transform.Find("TextIdx").GetComponent<Text>();
        //textIdx.text = (rightIdx + 1).ToString() + "/" + dataManage.Instance(sChapter).num().ToString();
        //if (sType == "0")
        //{
        //    objText.SetActive(true);
        //    objSpr.SetActive(false);
        //    objText.GetComponent<Text>().text = dataManage.Instance(sChapter).get(rightIdx, "Text");
        //    TypeWriter writer = objText.GetComponent<TypeWriter>();
        //    writer.SetContent();
        //}
        //else if (sType == "1")
        //{
        //    objText.SetActive(false);
        //    objSpr.SetActive(true);
        //    objSpr.transform.Find("Image");

        //    Image sprView = objSpr.transform.Find("Image").GetComponent<Image>();
        //    Sprite tem = new Sprite();
        //    sprView.sprite = Resources.Load(dataManage.Instance(sChapter).get(rightIdx, "Sprite"), tem.GetType()) as Sprite;

        //    objSpr.transform.Find("Text").GetComponent<Text>().text = dataManage.Instance(sChapter).get(rightIdx, "Text");
        //    TypeWriter writer = objSpr.transform.Find("Text").GetComponent<TypeWriter>();
        //    writer.SetContent();
        //}
        //int max = dataManage.Instance(sChapter).num();
        //if (max == 1)
        //{
        //    btnLast.SetActive(false);
        //    btnNext.SetActive(false);
        //}

        //if (rightIdx == 0)
        //{
        //    btnLast.SetActive(false);
        //}

        //if (rightIdx == max - 1)
        //{
        //    btnNext.SetActive(false);
        //}
    }

    public static void GoToScene(string name)
    {
        PlayerPrefs.SetString(DataMgr.m_gotoScene, name);
        PlayerPrefs.SetString(DataMgr.m_reScene, SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("LoadScene");
    }


    public static void GoToSceneDirect(string name)
    {
        PlayerPrefs.SetString(DataMgr.m_gotoScene, name);
        PlayerPrefs.SetString(DataMgr.m_reScene, SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(name);
    }

    public static void GotoReScene()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString(DataMgr.m_reScene));
    }

    //从资源文件夹中加载
    public static Sprite GetSprite(string name)
    {
        return Resources.Load<Sprite>(name);
    }

    public static string GetCurSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    static public void ExplodeObj(GameObject obj)
    {
        float j = 0;
        Instance.StopCoroutine("YieldRestoreOriPos");
        StartEnableDrag(1);
        StartEnableToPpt(1);
        foreach (var it in DataMgr.m_dicOriLocalPos)
        {
            float x = DataMgr.m_radius * Mathf.Cos(j);
            float y = DataMgr.m_radius * Mathf.Sin(j);
            //trans.DOMove(new Vector3(x + m_dicTransLocalInfo[trans.gameObject].m_pos.x, y + m_dicTransLocalInfo[trans.gameObject].m_pos.y, trans.localPosition.z), 2);
            it.Key.transform.DOMove(new Vector3(x + DataMgr.m_dicOriLocalPos[it.Key.transform.gameObject].x, y + DataMgr.m_dicOriLocalPos[it.Key.transform.gameObject].y, it.Key.transform.localPosition.z), 1);
            j += DataMgr.m_changeRate;
        }
        //foreach (Transform trans in obj.transform.GetComponentsInChildren<Transform>())
        //{
        //    float x = DataMgr.m_radius * Mathf.Cos(j);
        //    float y = DataMgr.m_radius * Mathf.Sin(j);
        //    //trans.DOMove(new Vector3(x + m_dicTransLocalInfo[trans.gameObject].m_pos.x, y + m_dicTransLocalInfo[trans.gameObject].m_pos.y, trans.localPosition.z), 2);
        //    trans.DOMove(new Vector3(x + DataMgr.m_dicOriLocalPos[trans.gameObject].x, y + DataMgr.m_dicOriLocalPos[trans.gameObject].y, trans.localPosition.z), 1);
        //    j += DataMgr.m_changeRate;
        //}
    }

    //static public void FlashObj(GameObject obj, float time, float delayClose)
    //{
    //    if (obj.GetComponent<HighlightableObject>() == null)
    //    {
    //        obj.AddComponent<HighlightableObject>();
    //    }
    //    obj.GetComponent<HighlightableObject>().FlashingOn(Color.red, Color.yellow, time);
    //    Instance.StartCoroutine(Instance.YieldFlashClose(delayClose, obj));
    //}

    //static public void FlashObjAllTime(GameObject obj, float freq)
    //{
    //    if (obj.GetComponent<HighlightableObject>() == null)
    //    {
    //        obj.AddComponent<HighlightableObject>();
    //    }
    //    obj.GetComponent<HighlightableObject>().FlashingOn(Color.red, Color.yellow, freq);
    //}

    //static public void FlashOff(GameObject obj)
    //{
    //    if (obj.GetComponent<HighlightableObject>() == null)
    //    {
    //        obj.GetComponent<HighlightableObject>().FlashingOff();
    //    }
    //}
    //IEnumerator YieldFlashClose(float delayClose, GameObject obj)
    //{
    //    yield return new WaitForSeconds(delayClose);
    //    obj.GetComponent<HighlightableObject>().FlashingOff();
    //}


    static public void MoveAToB(GameObject a, GameObject b, float time = 0.01f)
    {
        Instance.StartCoroutine(Instance.YieldMoveToPos(time, a, b));
    }


    static public void MoveOnlyPosAToB(GameObject a, GameObject b, float time = 0.01f)
    {
        Instance.StartCoroutine(Instance.YieldOnlyPosMoveToPos(time, a, b));
    }

    IEnumerator YieldOnlyPosMoveToPos(float fTime, GameObject obj, GameObject abObj, bool isclose = false)
    {
        int i = 0;
        while (i <= 100)
        {
            obj.transform.position = Vector3.Lerp(obj.transform.position, abObj.transform.position, (float)i / 100.0f);
            i++;
            yield return new WaitForSeconds(fTime);
        }
    }

    static public void ForbidDrag(GameObject obj)
    {
        ChangeTag(obj, DataMgr.m_tagUnuse);
        RemoveMeshCollider(obj);
    }

    public static void SaveLabel(GameObject obj)
    {
        DataMgr.m_listLabelCanvas.Clear();
        foreach (Canvas trans in obj.transform.GetComponentsInChildren<Canvas>())
        {
            DataMgr.m_listLabelCanvas.Add(trans);
        }
    }
    public static void LabelSet(bool isActive)
    {
        for (int i = 0; i < DataMgr.m_listLabelCanvas.Count; i++)
        {
            DataMgr.m_listLabelCanvas[i].gameObject.SetActive(isActive);
        }
    }
    //static public void AutoAddTip(GameObject obj)
    //{
    //    for (int i = 0; i < obj.transform.childCount; i++)
    //    {
    //        for (int j = 0; j < obj.transform.GetChild(i).transform.childCount; j++)
    //        {
    //            Transform trans = obj.transform.GetChild(i).transform.GetChild(j);
    //            if (trans.GetComponent<Tip>() == null)
    //            {
    //                trans.gameObject.AddComponent<Tip>();
    //            }
    //            string sTip = "";
    //            switch (trans.parent.name)
    //            {
    //                case "ChuanDongZhuangZi":
    //                    sTip = "传动装置";
    //                    break;
    //                case "DianJi":
    //                    sTip = "电机";
    //                    break;
    //                case "ErXiXuanGua":
    //                    sTip = "二系悬挂";
    //                    break;
    //                case "GouJia":
    //                    sTip = "构架";
    //                    break;
    //                case "LingBuJian":
    //                    sTip = "零部件";
    //                    break;
    //                case "LunDuiZhouXiang":
    //                    sTip = "轮对轴箱";
    //                    break;
    //                case "YiXiXuanGua":
    //                    sTip = "一系悬挂";
    //                    break;
    //                case "ZhongYangQianYingZhuangZi":
    //                    sTip = "轴箱牵引装置";
    //                    break;
    //                case "ZiDongZhuangZhi":
    //                    sTip = "制动装置";
    //                    break;
    //            }
    //            trans.GetComponent<Tip>().m_tip = sTip;
    //        }
    //    }
    //}

    static public void StartEnableToPpt(float delay)
    {
        DataMgr.m_isEnableToPpt = false;
        Instance.StopCoroutine("YieldStartEnableToPpt");
        Instance.StartCoroutine("YieldStartEnableToPpt", delay);
    }

    static public void StopCor()
    {
        Instance.StopAllCoroutines();
    }
    IEnumerator YieldStartEnableToPpt(float delay)
    {
        yield return new WaitForSeconds(delay);
        DataMgr.m_isEnableToPpt = true;
    }

    static public void StartEnableDrag(float delay)
    {
        DataMgr.m_isEnableDrag = false;
        Instance.StopCoroutine("YieldStartEnableDrag");
        Instance.StartCoroutine("YieldStartEnableDrag", delay);
    }

    IEnumerator YieldStartEnableDrag(float delay)
    {
        yield return new WaitForSeconds(delay);
        DataMgr.m_isEnableDrag = true;
    }

    static public void StartEnableRightBigMenu(float delay)
    {
        DataMgr.m_isEnableRightBigMenu = false;
        Instance.StopCoroutine("YieldStartEnableRightBigMenu");
        Instance.StartCoroutine("YieldStartEnableRightBigMenu", delay);
    }

    IEnumerator YieldStartEnableRightBigMenu(float delay)
    {
        yield return new WaitForSeconds(delay);
        DataMgr.m_isEnableRightBigMenu = true;
    }

    static public void ShaderFlash(float freq, float times, GameObject obj, float r, float g, float b, float a)
    {
        Instance.StartCoroutine(Instance.YieldShaderFlash(freq, times, obj, r, g, b, a));
    }

    static public void ShaderFlashZaku(float freq, float times, GameObject obj, float r, float g, float b, float a)
    {
        Instance.StartCoroutine(Instance.YieldShaderFlashZaku(freq, times, obj, r, g, b, a));
    }

    IEnumerator YieldShaderFlashZaku(float freq, float times, GameObject obj, float r, float g, float b, float a)
    {
        int i = 0;
        while (i <= times)
        {
            if (i % 2 == 0)
            {
                ShaderChangeTransColorZaku(obj, r, g, b, a);
            }
            else
            {
                ShaderReturn(obj);
            }
            i++;
            yield return new WaitForSeconds(freq);
        }
        ShaderReturn(obj);
    }

    public static void ShaderChangeTransColorZaku(GameObject obj, float r, float g, float b, float a)
    {
        if (obj != null)
        {
            foreach (var mesh in obj.transform.GetComponentsInChildren<MeshRenderer>())
            {
                for (int i = 0; i < mesh.materials.Length; i++)
                {
                    Texture main = mesh.materials[i].mainTexture;
                    Texture nomal = mesh.materials[i].GetTexture("_BumpMap");
                    Texture metal = mesh.materials[i].GetTexture("_MetallicGlossMap");
                    mesh.materials[i].shader = Shader.Find("Custom/Flash_S");
                    mesh.materials[i].SetTexture("_Albedo", main);
                    mesh.materials[i].SetTexture("_Metallic", metal);
                    mesh.materials[i].SetTexture("_Normal", nomal);
                    //mesh.materials[i].mainTexture = main;fdfddsfds
                    //mesh.materials[i].
                    //mesh.materials[i].SetVector("_ColorWithAlpha_Front", new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f));
                    //mesh.materials[i].SetVector("_ColorWithAlpha_Back", new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f));
                }
            }
        }
    }

    IEnumerator YieldShaderFlash(float freq, float times, GameObject obj, float r, float g, float b, float a)
    {
        int i = 0;
        while (i <= times)
        {
            if (i % 2 == 0)
            {
                ShaderChangeTransColor(obj, r, g, b, a);
            }
            else
            {
                ShaderReturn(obj);
            }
            i++;
            yield return new WaitForSeconds(freq);
        }
        ShaderReturn(obj);
    }

    public static void DoMoveY(GameObject obj, float y, float time)
    {
        foreach (MeshRenderer trans in obj.transform.GetComponentsInChildren<MeshRenderer>())
        {
            trans.transform.DOLocalMoveY(y, time);
        }
    }

    static public void ShaderFlashFrame(int clip, int total, GameObject obj, float r, float g, float b, float a)
    {
        Instance.StartCoroutine(Instance.YieldShaderFlashFrame(clip, total, obj, r, g, b, a));
    }
    IEnumerator YieldShaderFlashFrame(int clip, int total, GameObject obj, float r, float g, float b, float a)
    {
        int i = 0;
        int cnt = 0;
        bool isFalsh = false;
        while (i < total && obj != null)
        {
            cnt++;
            if (cnt >= clip)
            {
                cnt = 0;
                isFalsh = !isFalsh;
            }

            if (isFalsh)
            {
                ShaderChangeTransColor(obj, r, g, b, a);
            }
            else
            {
                ShaderReturn(obj);
            }
            i++;
            yield return null;
        }
        ShaderReturn(obj);
    }

    /// <summary>
    /// 以IO方式进行加载
    /// </summary>
    static public Sprite LoadSprByIO(string sPath, Image img, int width, int height)
    {
        //double startTime = (double)Time.time;
        //创建文件读取流
        FileStream fileStream = new FileStream(sPath, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        //创建文件长度缓冲区
        byte[] bytes = new byte[fileStream.Length];
        //读取文件
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        //释放文件读取流
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        //创建Texture
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(bytes);

        //创建Sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        img.sprite = sprite;

        //startTime = (double)Time.time - startTime;
        //Debug.Log("IO加载用时:" + startTime);
        return sprite;
    }

    static public Sprite GetSprByByte(byte[] bufByte,int width,int height)
    {
        //创建Texture
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(bufByte);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }

    static public byte[] GetSprByIoRetByte(string sPath, Image img, int width, int height)
    {
        //double startTime = (double)Time.time;
        //创建文件读取流
        FileStream fileStream = new FileStream(sPath, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        //创建文件长度缓冲区
        byte[] bytes = new byte[fileStream.Length];
        //读取文件
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        //释放文件读取流
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        //创建Texture
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(bytes);

        //创建Sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        img.sprite = sprite;

        //startTime = (double)Time.time - startTime;
        //Debug.Log("IO加载用时:" + startTime);
        return bytes;
    }

    static public Sprite GetSprByIO(string sPath, int width, int height)
    {
        //double startTime = (double)Time.time;
        //创建文件读取流
        FileStream fileStream = new FileStream(sPath, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        //创建文件长度缓冲区
        byte[] bytes = new byte[fileStream.Length];
        //读取文件
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        //释放文件读取流
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        //创建Texture
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(bytes);

        //创建Sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        //startTime = (double)Time.time - startTime;
        //Debug.Log("IO加载用时:" + startTime);
        return sprite;
    }

    //一个btnpar下的统一按钮样式,
    static public void BtnChangeState(int idx, GameObject btnPar, Sprite sprN, Sprite sprP, Color colN, Color colP)
    {
        for (int i = 0; i < btnPar.transform.childCount; i++)
        {
            if (idx == i)
            {
                btnPar.transform.GetChild(i).GetComponent<Image>().sprite = sprP;
                btnPar.transform.GetChild(i).Find("Text").GetComponent<Text>().color = colP;
            }
            else
            {
                btnPar.transform.GetChild(i).GetComponent<Image>().sprite = sprN;
                btnPar.transform.GetChild(i).Find("Text").GetComponent<Text>().color = colN;
            }
        }
    }

    static public void BtnChangeStateList(int idx, GameObject btnPar, List<Sprite> listSpr)
    {
        for (int i = 0; i < btnPar.transform.childCount; i++)
        {
            if (idx == i)
            {
                btnPar.transform.GetChild(i).GetComponent<Image>().sprite = listSpr[i + btnPar.transform.childCount];
            }
            else
            {
                btnPar.transform.GetChild(i).GetComponent<Image>().sprite = listSpr[i];
            }
        }
    }

    static public GameObject CreateTmp(GameObject tmp, GameObject par, Vector3 localPos, Vector3 angles, Vector3 scale)
    {
        GameObject obj = Instantiate(tmp);
        obj.SetActive(true);
        obj.transform.parent = par.transform;
        obj.transform.localPosition = localPos;
        obj.transform.localScale = scale;
        obj.transform.localEulerAngles = angles;

        BigParDoExplodeInit(obj);
        return obj;
    }


    static public void BigParDoExplodeInit(GameObject obj)
    {
        DoExplode doEx = obj.GetComponent<DoExplode>();
        if (doEx != null)
        {
            doEx.m_oriPos = doEx.m_toPos = obj.transform.localPosition;
            doEx.m_oriRotate = doEx.m_toRotate = obj.transform.localEulerAngles;
            doEx.m_oriScale = doEx.m_toScale = obj.transform.localScale;
        }
    } 
    static public GameObject CreateObjFromRes(string sName)
    {
        GameObject tmp = Resources.Load<GameObject>(sName);
        if (tmp != null)
        {
            GameObject obj = Instantiate(tmp);
            obj.SetActive(true);
            return obj;
        }
        else
        {
            UnityEngine.Debug.Log("CreateFromResFailure:" + sName);
            return null;
        }
    }

    static public void CreateObjFromResAsync(string sName,GameObject par,Vector3 pos,Vector3 angle,Vector3 scale, bool isShowLoad = false,bool isAutoDestory = false,string name = "",bool isAtuoHide = false,bool isPreCnt = false)
    {
        //if (m_corCreateFromRes != null)
        //{
        //    Instance.StopCoroutine(m_corCreateFromRes);
        //}
        m_corCreateFromRes = Instance.StartCoroutine(Instance.YieldCreateFromRes(sName,par,pos,angle,scale,isShowLoad, isAutoDestory,name,isAtuoHide,isPreCnt));
    }

    IEnumerator YieldCreateFromRes(string sName, GameObject par, Vector3 pos, Vector3 angle, Vector3 scale,bool isShowLoad,bool isAutoDestory = false,string name = "",bool isAutoHide = false,bool isPreCnt = false)
    {
        //ResourceRequest resourceRequest = Resources.LoadAsync<Texture2D>("Characters/Textures/CostumePartyCharacters" + (i < 2 ? "" : "" + i));
        //while (!resourceRequest.isDone)
        //{
        //    yield return 0;
        //}
        //material.mainTexture = resourceRequest.asset as Texture2D;
        GameObject load = null;
        if (isShowLoad == true)
        {
            load = PublicFunc.CreateObjFromRes("Lockie/PreLoadMgr");
        }
        ResourceRequest resourceRequest = Resources.LoadAsync<GameObject>(sName);
        while (!resourceRequest.isDone)
        {
            yield return 0;
        }


        GameObject tmp = resourceRequest.asset as GameObject;
        //GameObject tmp = (GameObject)Resources.LoadAsync<GameObject>(sName);
        if (tmp != null)
        {
            GameObject obj = Instantiate(tmp);
            obj.SetActive(true);
            if (obj != null && par != null)
            {
                obj.transform.parent = par.transform;
                obj.transform.localPosition = pos;
                obj.transform.localEulerAngles = angle;
                obj.transform.localScale = scale;
            }

            if (name != "")
            {
                obj.name = name;
            }

            if (isAutoHide == true)
            {
                obj.SetActive(false);
            }

            if (isPreCnt == true)
            {
                NotificationCenter.Get().ObjDispatchEvent(KEventKey.m_evPreLoadCnt);
            }
            if (isAutoDestory == true)
            {
                Destroy(obj);
            }
            //var rotation = Quaternion.identity;
            //rotation.eulerAngles = angle;
            //GameObject obj = ObjPoolMgr.SpawnPrefab(tmp, pos, rotation, scale, par.transform, transform);
        }
        else
        {
            UnityEngine.Debug.LogError("CreateFromResFailure:" + sName);
        }

        if (isShowLoad == true)
        {
            if (load != null)
            {
                Destroy(load);
            }
        }
        yield return null;
    }
    static public bool IsAnimtorFinish(Animator anim)
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        // 判断动画是否播放完成
        if (info.normalizedTime >= 1.0f)
        {
            return true;
        }
        return false;
    }

    public static float ClampAngle(float angle)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return angle;
    }

    public static float ClampAngleLimit(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    //static public void UiPlaneMove(UiPlane ui, float[] bufTo,float fTime)
    //{
    //    //Instance.StopAllCoroutines();
    //    if (m_corUiPlaneMove != null)
    //    {
    //        Instance.StopCoroutine(m_corUiPlaneMove);
    //    }
    //    m_corUiPlaneMove = Instance.StartCoroutine(Instance.YieldUiPlaneMove(ui,bufTo,fTime/200.0f));
    //}

    //IEnumerator YieldUiPlaneMove(UiPlane ui,float[] bufTo, float fTime)
    //{
    //    int i = 1;
    //    while (i <= 100)
    //    {
    //        ui.m_xOffset = Mathf.Lerp(ui.m_xOffset, bufTo[0], (float)i / 100.0f);
    //        ui.m_yOffset = Mathf.Lerp(ui.m_yOffset, bufTo[1], (float)i / 100.0f);
    //        ui.m_zOffset = Mathf.Lerp(ui.m_zOffset, bufTo[2], (float)i / 100.0f);
    //        i+=2;
    //        yield return new WaitForSeconds(fTime);
    //    }
    //}

    public static string GetOpenFilePathOne(string filter)
    {
        string path = "";
        OpenFileDialog ofd = new OpenFileDialog();

        ofd.Multiselect = false;
        //ofd.Filter =  "图片文件 |*.jpg;*.png;*.jpeg";
        ofd.Filter = filter;
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            path = ofd.FileName;
        }
        return path;
    }

    public static List<string> GetOpenFilePathMulti(string filter)
    {
        List<string> listPath = new List<string>();
        OpenFileDialog ofd = new OpenFileDialog();

        ofd.Multiselect = true;
        //ofd.Filter = "图片文件 |*.jpg;*.png;*.jpeg";
        ofd.Filter = filter;
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            for (int i = 0; i < ofd.FileNames.Length; i++)
            {
                listPath.Add(ofd.FileNames[i]);
            }
        }
        listPath.Sort(new PathCompareTwoSprit());
        return listPath;
    }

    static public void CopyFileToFolder(string oriPath, string toPath)
    {
        string name = GetFileNameWithFormat(oriPath);

    }

    static public void CopyFileOldName(string oriPath, string toPath)
    {
        string name = GetFileNameWithFormat(oriPath);
        string toAllPath = toPath + "/" + name;

        if (!Directory.Exists(toPath))
        {
            Directory.CreateDirectory(toPath);
        }

        if (File.Exists(toAllPath))
        {
            File.Copy(oriPath, toAllPath, true);
        }
        else
        {
            File.Copy(oriPath, toAllPath);
        }
    }

    static public void CopyFileNewName(string oriPath, string toPath,string name)
    {
        string format = GetFileFormat(oriPath);
        string toAllPath = toPath + "/" + name + "." + format;
        
        if (!Directory.Exists(toPath))
        {
            Directory.CreateDirectory(toPath);
        }

        if (File.Exists(toAllPath))
        {
            File.Copy(oriPath, toAllPath, true);
        }
        else
        {
            File.Copy(oriPath, toAllPath);
        }
    }

    static public string GetFileNameWithFormat(string path)
    {
        string[] bufName = path.Split('\\');
        string name = bufName[bufName.Length - 1];
        return name;
    }

    static public string GetFileFormat(string path)
    {
        string[] bufFormat = path.Split('.');
        return bufFormat[bufFormat.Length - 1];
    }

    static public void SavePptDataToOther(int id,string toPath)
    {

        string toPar = toPath + "/" + id.ToString();

        if (!Directory.Exists(toPar))
        {
            Directory.CreateDirectory(toPar);
        }

        string toJsonPath = toPar + "/PptEdit";
        string toImgPath = toPar + "/PptImg";

        if (!Directory.Exists(toJsonPath))
        {
            Directory.CreateDirectory(toJsonPath);
        }

        if (!Directory.Exists(toImgPath))
        {
            Directory.CreateDirectory(toImgPath);
        }

        string imgOriPath = UnityEngine.Application.streamingAssetsPath + "/PptImg/" + id.ToString();
        string jsonOriPathFile = UnityEngine.Application.streamingAssetsPath + "/PptEdit/" + id.ToString() + ".json";
        File.Copy(jsonOriPathFile, toJsonPath + "/" + id.ToString() + ".json", true);

        string[] listImg = Directory.GetFiles(imgOriPath);
        for (int i = 0; i < listImg.Length; i++)
        {
            string[] bufImg = listImg[i].Split('\\');
            string toImgPathFile = toImgPath + "/"  + bufImg[bufImg.Length - 1];
            File.Copy(listImg[i], toImgPathFile, true);
        }

        //寻找子图文件夹
        string[] listChildImgFolder = Directory.GetDirectories(imgOriPath);
        for ( int i = 0; i < listChildImgFolder.Length; i++)
        {
            string[] listChildImg = Directory.GetFiles(listChildImgFolder[i]);

            for ( int j = 0; j < listChildImg.Length; j++)
            {
                string[] bufImg = listChildImg[j].Split('\\');
                string toImgPathFile = toImgPath + "/" + bufImg[bufImg.Length - 2] + "/" + bufImg[bufImg.Length - 1];
                string toImgPathFloder = toImgPath + "/" + bufImg[bufImg.Length - 2];
                if (!Directory.Exists(toImgPathFloder))
                {
                    Directory.CreateDirectory(toImgPathFloder);
                }

                File.Copy(listChildImg[j], toImgPathFile, true);
            }
        }
    }

    static public void ExportPptData(Dictionary<int,int> dic,string path)
    {
        foreach(var it in dic)
        {
            string oriId = it.Key.ToString();
            string toId = it.Value.ToString();
            string oriJosnPathFile = path + "/" + oriId + "/PptEdit/" + oriId + ".json";
            string toJsonPathFile = UnityEngine.Application.streamingAssetsPath + "/PptEdit/" + toId + ".json";
            File.Copy(oriJosnPathFile, toJsonPathFile, true);

            string oriImgPath = path + "/" + oriId + "/PptImg";

            string toImgPath = UnityEngine.Application.streamingAssetsPath + "/PptImg/" + toId;
            if (!Directory.Exists(toImgPath))
            {
                Directory.CreateDirectory(toImgPath);
            }

            string[] listImg = Directory.GetFiles(oriImgPath);
            for (int i = 0; i < listImg.Length; i++)
            {
                string[] bufImg = listImg[i].Split('\\');
                string toImgPathFile = toImgPath + "/" + bufImg[bufImg.Length - 1];
                File.Copy(listImg[i], toImgPathFile, true);
            }

            //寻找子图文件夹
            string[] listChildImgFolder = Directory.GetDirectories(oriImgPath);
            for (int i = 0; i < listChildImgFolder.Length; i++)
            {
                string[] listChildImg = Directory.GetFiles(listChildImgFolder[i]);

                for (int j = 0; j < listChildImg.Length; j++)
                {
                    string[] bufImg = listChildImg[j].Split('\\');
                    string toImgPathFile = UnityEngine.Application.streamingAssetsPath + "/PptImg/" + toId + "/" + bufImg[bufImg.Length - 2] + "/" + bufImg[bufImg.Length - 1];
                    string toImgPathFloder = UnityEngine.Application.streamingAssetsPath + "/PptImg/" + toId + "/" + bufImg[bufImg.Length - 2];
                    if (!Directory.Exists(toImgPathFloder))
                    {
                        Directory.CreateDirectory(toImgPathFloder);
                    }

                    File.Copy(listChildImg[j], toImgPathFile, true);
                }
            }

        }
    }

    public static Process StartProcess(string fileName,string args)
    {
        try
        {
            Process myProcess = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo(fileName, args);
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardInput = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            myProcess.StartInfo = startInfo;
            return myProcess;
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log("出错原因：" + ex.Message);
        }
        return null;
    }

    public static bool IsChild(Transform child, Transform par)
    {
        bool isChild = false;
        foreach (var it in par.GetComponentsInChildren<Transform>())
        {
            if (it.GetInstanceID() == child.GetInstanceID())
            {
                isChild = true;
                break;
            }
        }
        return isChild;
    }

    //public static OnePpt GetOnePptData(int id)
    //{
    //    OnePpt one = new OnePpt();
    //    //string path = UnityEngine.Application.streamingAssetsPath + "/PptEdit/" + DataMgr.m_softId.ToString();
    //    string filePath = UnityEngine.Application.streamingAssetsPath + "/PptEdit/" + DataMgr.m_softId.ToString() + "/"+id.ToString() + ".json";
    //    if (File.Exists(filePath))
    //    {
    //        string json = JsonMgr.GetJsonString(filePath);
    //        one = JsonMapper.ToObject<OnePpt>(json);
    //    }
    //    return one;
    //}

    //public static OnePpt GetOnePptDataMyClass(int id)
    //{
    //    OnePpt one = new OnePpt();
    //    //string path = UnityEngine.Application.streamingAssetsPath + "/PptEdit/" + DataMgr.m_softId.ToString();
    //    string filePath = UnityEngine.Application.streamingAssetsPath + "/PptEdit/" + id.ToString() + ".json";
    //    if (File.Exists(filePath))
    //    {
    //        string json = JsonMgr.GetJsonString(filePath);
    //        one = JsonMapper.ToObject<OnePpt>(json);
    //    }
    //    return one;
    //}

    //public static void LightHigh(Transform trans)
    //{
    //    if (trans.transform.gameObject.GetComponent<HighlightableObject>() == null)
    //    {
    //        trans.transform.gameObject.AddComponent<HighlightableObject>();
    //    }
    //    trans.transform.gameObject.GetComponent<HighlightableObject>().ConstantOn(Color.yellow);
    //}

    public static void Encypt(ref Byte[] targetData)
    {
        int dataLength = targetData.Length;
        for (int i = 0; i < dataLength; ++i)
        {
            targetData[i] = (byte)(targetData[i] ^ AppConst.m_abKey);
        }
    }

    public static string GetFileNameByLine(string name)
    {
        string[] file = name.Split('/');
        if (file.Length == 0)
        {
            return name;
        }
        else
        {
            return file[file.Length - 1];
        }
    }

    public static string GetFileWithFormatByPath(string path)
    {
        string[] bufPath = path.Split('/');
        string name = bufPath[bufPath.Length - 1];
        //string abPath = info.m_prefabName.Replace("/" + abName, "");
        //string[] bufAbName = abName.Split('.');
        return name;
    }

    public static string GetFileNoFormatByPath(string path)
    {
        string[] bufPath = path.Split('/');
        string name = bufPath[bufPath.Length - 1];
        string[] bufName = name.Split('/');
        //string abPath = info.m_prefabName.Replace("/" + abName, "");
        //string[] bufAbName = abName.Split('.');
        return bufName[0];
    }

    public static string GetOnlyPath(string path)
    {
        string[] bufPath = path.Split('/');
        string name = bufPath[bufPath.Length - 1];
        string onlyPath = path.Replace(name,"");
        //string abPath = info.m_prefabName.Replace("/" + abName, "");
        //string[] bufAbName = abName.Split('.');
        return onlyPath;
    }

    public static T StringToEnum<T>(string str)
    {
        return (T)Enum.Parse(typeof(T), str);
    }

    public static string GetIp()
    {
        string ip = "";
        var strHostName = System.Net.Dns.GetHostName();


        var ipEntry = System.Net.Dns.GetHostEntry(strHostName);


        var addr = ipEntry.AddressList;
        


        return addr[1].ToString();
   
    }
  
}

public class PathCompareTwoSprit : IComparer<string>
{
    public int Compare(string x, string y)
    {
        string[] aBuf = x.Split('\\');
        string[] bBuf = y.Split('\\');
        string[] aNameBuf = aBuf[aBuf.Length - 1].Split('.');
        string[] bNameBuf = bBuf[bBuf.Length - 1].Split('.');
        if (aNameBuf[0].Length > bNameBuf[0].Length)
        {
            return 1;
        }
        else if (aNameBuf[0].Length < bNameBuf[0].Length)
        {
            return -1;
        }
        else if (aNameBuf[0].Length == bNameBuf[0].Length)
        {
            return aNameBuf[0].CompareTo(bNameBuf[0]);
        }
        return 0;
    }
}

public class PathCompareOneSprit:IComparer<string>
{
    public int Compare(string x, string y)
    {
        string[] aBuf = x.Split('/');
        string[] bBuf = y.Split('/');
        string[] aNameBuf = aBuf[aBuf.Length - 1].Split('.');
        string[] bNameBuf = bBuf[bBuf.Length - 1].Split('.');
        if (aNameBuf[0].Length > bNameBuf[0].Length)
        {
            return 1;
        }
        else if (aNameBuf[0].Length < bNameBuf[0].Length)
        {
            return -1;
        }
        else if (aNameBuf[0].Length == bNameBuf[0].Length)
        {
            return aNameBuf[0].CompareTo(bNameBuf[0]);
        }
        return 0;
    }
}