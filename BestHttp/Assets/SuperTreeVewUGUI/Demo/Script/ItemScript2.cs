using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace SuperTreeView
{
    public class ItemScript2 : MonoBehaviour
    {
        public Button mExpandBtn;
        public Image mIcon;
        public Image mSelectImg;
        public Button mClickBtn;
        public Text mLabelText;

        public Image mLineHorizontal;
        public Image mLineVertical;
        public Image mLineVertical_2;
        public Image mExpandImg;

        void Start()
        {
            mExpandBtn.onClick.AddListener(OnExpandBtnClicked);
            mClickBtn.onClick.AddListener(OnItemClicked);
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


        public void SetItemInfo(string iconSpriteName, string labelTxt)
        {
            Init();
            mIcon.sprite = ResManager.Instance.GetSpriteByName(iconSpriteName);
            mLabelText.text = labelTxt;

        }

        void OnItemClicked()
        {
            TreeViewItem item = GetComponent<TreeViewItem>();
            item.RaiseCustomEvent(CustomEvent.ItemClicked, null);
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
                mExpandImg.sprite = ResManager.Instance.GetSpriteByName("expandicon1");
            }
            else
            {
                mExpandImg.sprite = ResManager.Instance.GetSpriteByName("expandicon2");
            }
        }


    }

}