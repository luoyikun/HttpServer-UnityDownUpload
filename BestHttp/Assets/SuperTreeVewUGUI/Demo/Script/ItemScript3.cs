using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace SuperTreeView
{
    public class ItemScript3 : MonoBehaviour
    {
        public Text mLabelText;
        
        public void SetText(string txt)
        {
            mLabelText.text = txt;
            //let ContentSizeFitter refresh recttranform height at once.
            mLabelText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            float height = mLabelText.GetComponent<RectTransform>().rect.height;
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }


    }

}