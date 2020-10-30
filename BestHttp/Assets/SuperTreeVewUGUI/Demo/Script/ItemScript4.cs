using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace SuperTreeView
{
    public class ItemScript4 : MonoBehaviour
    {
        public Button mExpandBtn;
        public Image mIcon;
        public Image mSelectImg;
        public ButtonEx mClickBtn;
        public Text mLabelText;
        string mData = "";

        public string Data
        {
            get
            {
                return mData;
            }
            set
            {
                mData = value;
            }
        }

        void Start()
        {
            mExpandBtn.onClick.AddListener(OnExpandBtnClicked);
            mClickBtn.onClick.AddListener(OnItemClicked);
            mClickBtn.OnDoubleClick.AddListener(OnItemDoubleClicked);
        }

        public void Init()
        {
            SetExpandBtnVisible(false);
            SetExpandStatus(true);
            IsSelected = false;
        }

        void OnExpandBtnClicked()
        {
            TreeViewItem item = GetComponent<TreeViewItem>();
            item.DoExpandOrCollapse();
        }


        public void SetItemInfo(string iconSpriteName, string labelTxt, string data = "")
        {
            Init();
            mIcon.sprite = ResManager.Instance.GetSpriteByName(iconSpriteName);
            mLabelText.text = labelTxt;
            mData = data;

        }

        void OnItemClicked()
        {
            TreeViewItem item = GetComponent<TreeViewItem>();
            item.RaiseCustomEvent(CustomEvent.ItemClicked, null);
            Debug.Log("TreeViewItem Clicked " + Data);
        }

        void OnItemDoubleClicked()
        {
            TreeViewItem item = GetComponent<TreeViewItem>();
            item.RaiseCustomEvent(CustomEvent.ItemDoubleClicked, null);
            Debug.Log("TreeViewItem Double Clicked " + Data);
        }

        public void SetExpandBtnVisible(bool visible)
        {
            if (visible)
            {
                mExpandBtn.gameObject.SetActive(true);
            }
            else
            {
                mExpandBtn.gameObject.SetActive(false);
            }
        }

        public bool IsSelected
        {
            get
            {
                return mSelectImg.gameObject.activeSelf;
            }
            set
            {
                mSelectImg.gameObject.SetActive(value);
            }
        }
        public void SetExpandStatus(bool expand)
        {
            if (expand)
            {
                mExpandBtn.transform.localEulerAngles = new Vector3(0, 0, -90);
            }
            else
            {
                mExpandBtn.transform.localEulerAngles = new Vector3(0, 0, 0);

            }
        }


    }

}