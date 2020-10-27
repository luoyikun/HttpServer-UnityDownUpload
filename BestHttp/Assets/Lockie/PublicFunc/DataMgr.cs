using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnSoftId
{
    DtZhenChe,
    DtBogieJx,
    DtMen,
    DtQianYinJx,
    DtKongTiaoJx,
    CrhBogieJx,
    CrhBogieKnow
}
public class DataMgr : MonoBehaviour {

    
    static public string m_tagDrag = "Drag";
    static public string m_tagHover = "Hover";
    static public string m_tagSmallBtn = "SmallBtn";
    static public string m_tagUi = "Ui";
    static public string m_tagHighLight = "HighLight";
    static public string m_tagUnuse = "Unuse";
    static public string m_tagDrawLine = "DrawLine";
    static public string m_shaTwoSideAlpha = "Custom/TwoSideAlpha";
    static public string m_textOnly = "textOnly";
    static public string m_textAndSpr = "textAndSpr";
    static public string m_gotoScene = "GotoScene";
    static public string m_reScene = "MainFirst";
    static public string m_tagPenMove = "PenMove";//针对ui 平移
	static public string m_tagPenDrag = "PenDrag";//针对ui 滚动层的拖拽
    static public string m_tagPackBox = "m_tagPackBox";
    static public string m_tagPenLimitMove = "PenLimitMove"; //笔限制方向移动
    static public string m_tagUntagged = "Untagged";
    static public string m_tagChildPos = "ChildPos";
    static public int m_layerPackBox = 10;
    static public int m_layerPenDrag = 9;
    static public int m_layerPenMove = 8;
    static public int m_layerPenLimitMove = 11;//限制物体的移动方向，xyz取一个方向
    static public GameObject m_curShowObj = null;//当前场景展示的总的父物体

    public static Dictionary<GameObject, Vector3> m_dicOriPos = new Dictionary<GameObject, Vector3>();
    public static Dictionary<GameObject, Quaternion> m_dicOriQua = new Dictionary<GameObject, Quaternion>();
    public static Dictionary<GameObject, Vector3> m_dicOriEuler = new Dictionary<GameObject, Vector3>();
    public static Dictionary<GameObject, Vector3> m_dicOriScale = new Dictionary<GameObject, Vector3>();

    public static Dictionary<GameObject, Vector3> m_dicOriLocalPos = new Dictionary<GameObject, Vector3>();
    public static Dictionary<GameObject, Quaternion> m_dicOriLocalQua = new Dictionary<GameObject, Quaternion>();
    public static Dictionary<GameObject, Vector3> m_dicOriLocalScale = new Dictionary<GameObject, Vector3>();
    public static Dictionary<GameObject, Vector3> m_dicOriLocalEuler = new Dictionary<GameObject, Vector3>();
    public static List<GameObject> m_listHideObj = new List<GameObject>();
    public static List<Canvas> m_listLabelCanvas = new List<Canvas>();
    static public GameObject m_curSelectObj = null;//当前笔拖动的物体
    static public GameObject m_curAdsorbObj = null;//当前吸附提示物体
    static public GameObject m_curHitObj = null;//当前笔碰撞到的物体
    static public bool m_isPenMidPress = false;
    static public bool m_isPenBigMove = true;
    static public bool m_isPenMidMove = false;//中间是放大功能
    static public bool m_isHighLight = true;
    static public Vector3 m_oriScale = Vector3.zero;
    static public GameObject m_penRightObj = null;
    static public Vector3 m_penEndPos = Vector3.zero;
    static public Vector3 m_penEndAngle = Vector3.zero;

    //static public Vector3 m_abPos = Vector3.zero;
    static public GameObject m_moveObj = null;
    static public GameObject m_moveToObj = null;

    static public Shader m_shader;

    static public int m_ctrlMode = 0;//当0为ppt模式，当1为模型模式
    static public float m_radius = 0.1f;
    static public float m_changeRate = 3.0f;

    static public string m_leftChapterName = "";
    static public bool m_isEnableToPpt = true;
    static public bool m_isEnableDrag = true;
    static public bool m_isEnableRightBigMenu = true;

    static public string m_path = "C:/Ardez/Data";
    static public string m_imgFolderName = "PptImg";


	static public string m_LastScene = "";
	static public string m_LastLastScene = "";
	//public static Dictionary<string, IndexClass> m_dicIndexClass = new Dictionary<string, IndexClass>();

    static public bool m_isPreLoad = true;
    static public bool m_isClosePreLoadCnt = false;
    static public bool m_isAutoClickPreLoad = false; //是否自动点击加载界面
    static public bool m_isJianXiu = false;
    static public int m_jianXiuIdx = 0;

    //热更新相关
    static public bool m_isUpdate = false;
    static public bool m_isTotalDown = false;
    static public long m_downTotal = 0;
    static public long m_downCur = 0;
    //右键按钮相关
    static public bool m_isShowPingAndScale = true;
    static public bool m_isShowExplode = true;
    static public bool m_isShowReset = true;
    //string加密相关
    static public string m_rsaPublicKey = "<RSAKeyValue><Modulus>sz9WgkPGw/AD9wCGAFAOyynex8huYDjd7IVWhPEBBwQSvv9wP5u4hnrouxXWaaA72Yth/RnKgFsobnY15bJ4w6e2eGqsmj7idPYVWHi7XnnuJQG84O+7ctUWk06QDzY50Neb+3DSfgSQ0HinK2xBdhk1RqydQXyFBk6sa9tNcQ0=</Modulus><Exponent>EQ==</Exponent></RSAKeyValue>";
    static public string m_rsaPrivateKey = "<RSAKeyValue><Modulus>sz9WgkPGw/AD9wCGAFAOyynex8huYDjd7IVWhPEBBwQSvv9wP5u4hnrouxXWaaA72Yth/RnKgFsobnY15bJ4w6e2eGqsmj7idPYVWHi7XnnuJQG84O+7ctUWk06QDzY50Neb+3DSfgSQ0HinK2xBdhk1RqydQXyFBk6sa9tNcQ0=</Modulus><Exponent>EQ==</Exponent><P>4tL5JePui5WGRq/+kFe+CisKyX/UgGgESatNk/VbernDIdyB1KUik1miXMlNok84PSr6zFa2PL1YTHiBNwr5HQ==</P><Q>yk25nNd5bcpsQD13YYi+aGqm8vfEImA/MGoiQUonf8FnFwVMmgkbEtBLPiuNBoXXXwpcsb/SQ12a6SGKamHEsQ==</Q><DP>UA450SNFIjTF+tS0MvHKmi1PGfDhlrtMzrTuNDh6o8kXsZkew4WTu4kMIL+Ez9+5fwAcSB6arAaXooTiT6mFGQ==</DP><DQ>L5nRUhSVCsY3lqUNB+PwkQoJKhwuJjTDkuvL0yCBw7UJMpfVyeQGXseZO84DEJf2cLck3od8xI5+zXFNvq2XsQ==</DQ><InverseQ>Yw0k1G6k1hR3Fsw0YYhUj+BojITedgoT9aNb/q5PJK58cb+1SwY6cq9/TrS+vWF4zrBAJwzETMndZocF5/cTGw==</InverseQ><D>aXCNPZFHvo0vgjyLDz4m0dxk7f1uGnvN1myNPyRa9RF0cFoFyxBObTk9mzoFp4tuf/ejDVp3HlO9bidq/5Yo6pNnKPlgtCjjW8jqSQQISLkG2mT7k/EHhUSXQgsgUn1/NNXW7UyNy4LTHAnwa74/w9EPBSptXcGnzv+wdNbAaXE=</D></RSAKeyValue>";

    static public int m_mainFirstIdx = 5;//从主场景进入的序号
    static public EnSoftId m_softId = EnSoftId.DtZhenChe;

}
