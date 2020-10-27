using UnityEngine;
using System.Collections;

public class AbTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
       
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CreateCube()
    {
        //AbResCtrl.Instance.ObjCreateFromAsset("cube");
        string text = AbResCtrl.Instance.GetText("data","data");
        Debug.Log(text);
    }
}
