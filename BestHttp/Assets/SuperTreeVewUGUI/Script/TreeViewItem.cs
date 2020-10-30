using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperTreeView
{
    public enum ExpandStatus
    {
        Expanding,
        ExpandEnd,
        Collapsing,
        CollapseEnd,
    }

    public enum ExpandAnimType
    {
        Immediate,
        Scale,
        Clip,
    }

    public class TreeViewItem : MonoBehaviour
    {
        int mItemIndex;//this value is starting at 0 and adding 1 from top to bottom in a TreeList. That is to say the top most item of a TreeList is indexed 0 and the right below item is indexed 1.
        int mItemId; //the value is unique and not repeated
        TreeView mRootTreeView;
        TreeList mParentTreeList;
        TreeList mChildTreeList;
        const float mMinScaleValue = 0.1f;

        public float MaxWidth
        {
            get
            {
                float selfWidth = CachedRectTransform.rect.width;
                if (HasChildItem == false)
                {
                    return selfWidth;
                }
                if (IsCollapseEnd)
                {
                    return selfWidth;
                }
                float childListWidth = mChildTreeList.ContentTotalWidth;
                return Mathf.Max(selfWidth, childListWidth + ChildTreeIndent);
            }
        }


        [SerializeField]
        bool mUseOverridedConfig;
        [SerializeField]
        float mOverridedChildTreeItemPadding;
        [SerializeField]
        float mOverridedChildTreeIndent;
        [SerializeField]
        float mOverridedChildTreeListPadding;

        string mItemPrefabName;
        
        [SerializeField]
        string mDefaultItemPrefabName;

        public float ChildTreeItemPadding
        {
            get
            {
                if(mUseOverridedConfig)
                {
                    return mOverridedChildTreeItemPadding;
                }
                return RootTreeView.ItemPadding;
            }
            set
            {
                mOverridedChildTreeItemPadding = value;
                if(mUseOverridedConfig)
                {
                    mNeedReposition = true;
                }
            }
        }

        public float ChildTreeIndent
        {
            get
            {
                if (mUseOverridedConfig)
                {
                    return mOverridedChildTreeIndent;
                }
                return RootTreeView.ItemIndent;
            }
            set
            {
                mOverridedChildTreeIndent = value;
                if (mUseOverridedConfig)
                {
                    mNeedReposition = true;
                }
            }
        }

        public float ChildTreeListPadding
        {
            get
            {
                if (mUseOverridedConfig)
                {
                    return mOverridedChildTreeListPadding;
                }
                return RootTreeView.ChildTreeListPadding;
            }
            set
            {
                mOverridedChildTreeListPadding = value;
                if (mUseOverridedConfig)
                {
                    mNeedReposition = true;
                }
            }
        }

        RectTransform mCachedRectTransform;
        bool mNeedReposition = true;

        float mTotalHeight = 0f;
        ExpandStatus mExpandStatus = ExpandStatus.ExpandEnd;
        public ExpandStatus CurExpandStatus
        {
            get { return mExpandStatus; }
            set { mExpandStatus = value; }
        }

        System.Object mUserData = null;


        float mCurExpandAnimatedValue = 0;


        float mExpandUseTime = 0.5f;
        public float ExpandUseTime
        {
            get
            {
                if (ExpandAnimateType == ExpandAnimType.Clip)
                {
                    return mExpandUseTime;
                }
                return RootTreeView.ExpandUseTime;
            }
        }


        public float ExpandClipMoveSpeed
        {
            get { return RootTreeView.ExpandClipMoveSpeed; }
        }

        public ExpandAnimType ExpandAnimateType
        {
            get { return RootTreeView.ExpandAnimateType; }
        }


        public System.Object UserData
        {
            get { return mUserData; }
            set { mUserData = value; }
        }

        public string DefaultItemPrefabName
        {
            get { return mDefaultItemPrefabName; }
            set { mDefaultItemPrefabName = value; }
        }

        public bool IsExpanding
        {
            get { return (CurExpandStatus == ExpandStatus.Expanding); }
        }

        public bool IsExpandEnd
        {
            get { return (CurExpandStatus == ExpandStatus.ExpandEnd); }
        }
        public bool IsExpand
        {
            get { return (CurExpandStatus == ExpandStatus.ExpandEnd || CurExpandStatus == ExpandStatus.Expanding); }
        }

        public bool IsCollapsing
        {
            get { return (CurExpandStatus == ExpandStatus.Collapsing); }
        }

        public bool IsCollapseEnd
        {
            get { return (CurExpandStatus == ExpandStatus.CollapseEnd); }
        }
        public bool IsCollapse
        {
            get { return (CurExpandStatus == ExpandStatus.CollapseEnd || CurExpandStatus == ExpandStatus.Collapsing); }
        }


        public void DoExpandOrCollapse()
        {
            if (IsExpand)
            {
                Collapse();
            }
            else
            {
                Expand();
            }
        }

        void CheckClip()
        {
            if (mChildTreeList == null)
            {
                return;
            }
            ExpandAnimType animType = ExpandAnimateType;
            if (animType == ExpandAnimType.Immediate)
            {
                Image img = mChildTreeList.GetComponent<Image>();
                if (img != null)
                {
                    Object.Destroy(img);
                }
                Mask mask = mChildTreeList.GetComponent<Mask>();
                if (mask != null)
                {
                    Object.Destroy(mask);
                }
            }
            else if (animType == ExpandAnimType.Scale)
            {
                Image img = mChildTreeList.GetComponent<Image>();
                if (img != null)
                {
                    Object.Destroy(img);
                }
                Mask mask = mChildTreeList.GetComponent<Mask>();
                if (mask != null)
                {
                    Object.Destroy(mask);
                }
            }
            else if (animType == ExpandAnimType.Clip)
            {
                Image img = mChildTreeList.GetComponent<Image>();
                if (img == null)
                {
                    img = mChildTreeList.gameObject.AddComponent<Image>();
                }
                Mask mask = mChildTreeList.GetComponent<Mask>();
                if (mask == null)
                {
                    mask = mChildTreeList.gameObject.AddComponent<Mask>();
                    mask.showMaskGraphic = false;
                }
            }
        }

        void SetClipMaskEnable(bool enable)
        {
            if (ExpandAnimateType != ExpandAnimType.Clip)
            {
                return;
            }
            CheckClip();
            Mask mask = mChildTreeList.GetComponent<Mask>();
            mask.enabled = enable;
            Image img = mChildTreeList.GetComponent<Image>();
            img.enabled = enable;

        }

        public void ExpandAll(bool immediate = false)
        {
            Expand(immediate);
            if(mChildTreeList != null)
            {
                mChildTreeList.ExpandAllItem(immediate);
            }
            
        }

        public void CollapseAll(bool immediate = false)
        {
            Collapse(immediate);
            if(mChildTreeList != null)
            {
                mChildTreeList.CollapseAllItem(immediate);
            }
        }


        public void Expand(bool immediate = false)
        {
            if (HasChildItem == false)
            {
                CurExpandStatus = ExpandStatus.ExpandEnd;
                return;
            }
            CheckClip();
            if (ExpandAnimateType == ExpandAnimType.Immediate)
            {
                immediate = true;
            }
            if (CurExpandStatus == ExpandStatus.ExpandEnd)
            {
                return;
            }
            mNeedReposition = true;
            if (CurExpandStatus == ExpandStatus.Expanding && !immediate)
            {
                return;
            }
            mChildTreeList.gameObject.SetActive(true);
            if (!IsExpand)
            {
                OnExpandBegin();
            }
            CurExpandStatus = ExpandStatus.Expanding;
            if (immediate)
            {
                mCurExpandAnimatedValue = 1;
                CurExpandStatus = ExpandStatus.ExpandEnd;
                OnExpandEnd();
            }
        }
        public void Collapse(bool immediate = false)
        {
            if (HasChildItem == false)
            {
                CurExpandStatus = ExpandStatus.CollapseEnd;
                return;
            }
            CheckClip();
            if (ExpandAnimateType == ExpandAnimType.Immediate)
            {
                immediate = true;
            }
            if (CurExpandStatus == ExpandStatus.CollapseEnd)
            {
                return;
            }
            mNeedReposition = true;
            if (CurExpandStatus == ExpandStatus.Collapsing && !immediate)
            {
                return;
            }
            if (!IsCollapse)
            {
                OnCollapseBegin();
            }
            CurExpandStatus = ExpandStatus.Collapsing;
            if (immediate)
            {
                mCurExpandAnimatedValue = 0f;
                CurExpandStatus = ExpandStatus.CollapseEnd;
                mChildTreeList.gameObject.SetActive(false);
                OnCollapseEnd();
            }
        }


        public float TotalHeight
        {
            get { return mTotalHeight; }
        }

        public bool NeedReposition
        {
            get
            {
                if (mNeedReposition)
                {
                    return true;
                }
                if (mChildTreeList != null)
                {
                    return mChildTreeList.NeedReposition;
                }
                return false;
            }
        }

        public RectTransform CachedRectTransform
        {
            get
            {
                if (mCachedRectTransform == null)
                {
                    mCachedRectTransform = gameObject.GetComponent<RectTransform>();
                }
                return mCachedRectTransform;
            }
        }

        public string ItemPrefabName
        {
            get { return mItemPrefabName; }
            set { mItemPrefabName = value; }
        }

        public string ID
        {
            set;get;
        }
        public int ItemIndex
        {
            get { return mItemIndex; }
            set { mItemIndex = value; }
        }

        public int ItemId
        {
            get { return mItemId; }
            set { mItemId = value; }
        }
        public TreeView RootTreeView
        {
            get { return mRootTreeView; }
            set { mRootTreeView = value; }
        }
        public TreeList ParentTreeList
        {
            get { return mParentTreeList; }
            set { mParentTreeList = value; }
        }
        public TreeViewItem ParentTreeItem
        {
            get
            {
                if (ParentTreeList == null)
                {
                    return null;
                }
                return ParentTreeList.ParentTreeItem;
            }

        }

        public TreeList ChildTree
        {
            get
            {
                EnsureChildTreeListCreated();
                return mChildTreeList;
            }
        }

        public bool HasChildItem
        {
            get
            {
                return (ChildItemCount > 0);
            }
        }

        public int ChildItemCount
        {
            get
            {
                if (mChildTreeList == null)
                {
                    return 0;
                }
                return mChildTreeList.ItemCount;
            }

        }


        void OnExpandBegin()
        {
            SetClipMaskEnable(true);
            if (RootTreeView.OnItemExpandBegin != null)
            {
                RootTreeView.OnItemExpandBegin(this);
            }
        }

        void OnExpanding()
        {
            if (RootTreeView.OnItemExpanding != null)
            {
                RootTreeView.OnItemExpanding(this);
            }
        }

        void OnExpandEnd()
        {
            SetClipMaskEnable(false);
            if (RootTreeView.OnItemExpandEnd != null)
            {
                RootTreeView.OnItemExpandEnd(this);
            }
        }


        void OnCollapseBegin()
        {
            SetClipMaskEnable(true);
            if (RootTreeView.OnItemCollapseBegin != null)
            {
                RootTreeView.OnItemCollapseBegin(this);
            }
        }

        void OnCollapseing()
        {
            if (RootTreeView.OnItemCollapsing != null)
            {
                RootTreeView.OnItemCollapsing(this);
            }
        }
        void OnCollapseEnd()
        {
            SetClipMaskEnable(false);
            if (RootTreeView.OnItemCollapseEnd != null)
            {
                RootTreeView.OnItemCollapseEnd(this);
            }
        }


        public void OnActived()
        {

        }
       
        public void Init()
        {
            UserData = null;
            mNeedReposition = true;
            mTotalHeight = 0f;
            mCurExpandAnimatedValue = 1;
            CurExpandStatus = ExpandStatus.ExpandEnd;
            if(mChildTreeList != null)
            {
                mChildTreeList.gameObject.SetActive(true);
                mChildTreeList.Init();
                SetClipMaskEnable(false);
            }
        }

        public void Clear()
        {
            UserData = null;
            //mNeedReposition = true;
            if (mChildTreeList != null)
            {
                mChildTreeList.Clear();
                SetClipMaskEnable(false);
            }
        }

        public void RaiseCustomEvent(CustomEvent customEvent,System.Object param)
        {
            if(RootTreeView.OnItemCustomEvent != null)
            {
                RootTreeView.OnItemCustomEvent(this, customEvent, param);
            }
        }

        void EnsureChildTreeListCreated()
        {
            if (mChildTreeList != null)
            {
                return;
            }
            GameObject go = new GameObject();
            go.name = "ChildTree";
            go.layer = CachedRectTransform.gameObject.layer;
            RectTransform tf = go.GetComponent<RectTransform>();
            if (tf == null)
            {
                tf = go.AddComponent<RectTransform>();
            }
            tf.SetParent(CachedRectTransform);
            tf.anchorMax = new Vector2(0f, 1f);
            tf.anchorMin = tf.anchorMax;
            tf.pivot = new Vector2(0.5f, 1);
            mChildTreeList = tf.gameObject.AddComponent<TreeList>();
            mChildTreeList.RootTreeView = RootTreeView;
            mChildTreeList.ParentTreeItem = this;
            mChildTreeList.CachedRectTransform.localEulerAngles = Vector3.zero;
            mChildTreeList.CachedRectTransform.localScale = Vector3.one;
            mNeedReposition = true;
            CheckClip();
            SetClipMaskEnable(false);
            mCurExpandAnimatedValue = 1f;
            if(CurExpandStatus == ExpandStatus.CollapseEnd)
            {
                mCurExpandAnimatedValue = 0f;
                mChildTreeList.gameObject.SetActive(false);
            }
        }


        public void OnUpdate()
        {
            if (RootTreeView.NeedRepositionAll)
            {
                mNeedReposition = true;
            }
            if (HasChildItem == false)
            {
                return;
            }
            mChildTreeList.OnUpdate();
            if (mChildTreeList.NeedReposition)
            {
                mNeedReposition = true;
            }
            UpdateExpandingState();
        }

        void UpdateExpandingState()
        {
            if (CurExpandStatus == ExpandStatus.Expanding)
            {
                mNeedReposition = true;
                bool isExpandEnd = false;
                if (ExpandAnimateType == ExpandAnimType.Clip)
                {
                    mExpandUseTime = mChildTreeList.ContentTotalHeight / ExpandClipMoveSpeed;
                }
                mCurExpandAnimatedValue = mCurExpandAnimatedValue + Time.deltaTime / ExpandUseTime;
                if (mCurExpandAnimatedValue >= 1f)
                {
                    isExpandEnd = true;
                    mCurExpandAnimatedValue = 1f;
                }
                if (isExpandEnd)
                {
                    CurExpandStatus = ExpandStatus.ExpandEnd;
                    OnExpandEnd();
                }
                else
                {
                    OnExpanding();
                }
            }
            else if (CurExpandStatus == ExpandStatus.Collapsing)
            {
                if (ExpandAnimateType == ExpandAnimType.Clip)
                {
                    mExpandUseTime = mChildTreeList.ContentTotalHeight / ExpandClipMoveSpeed;
                }
                mNeedReposition = true;
                bool isCollapseEnd = false;
                mCurExpandAnimatedValue = mCurExpandAnimatedValue - Time.deltaTime / ExpandUseTime;

                if(ExpandAnimateType == ExpandAnimType.Scale)
                {
                    if (mCurExpandAnimatedValue <= mMinScaleValue)
                    {
                        isCollapseEnd = true;
                        mCurExpandAnimatedValue = 0f;
                    }
                }
                else
                {
                    if (mCurExpandAnimatedValue <= 0f)
                    {
                        isCollapseEnd = true;
                        mCurExpandAnimatedValue = 0f;
                    }
                }
                
                if (isCollapseEnd)
                {
                    CurExpandStatus = ExpandStatus.CollapseEnd;
                    mChildTreeList.gameObject.SetActive(false);
                    OnCollapseEnd();
                }
                else
                {
                    OnCollapseing();
                }
            }
        }


        public void Reposition()
        {
            if (NeedReposition == false)
            {
                return;
            }
            DoReposition();
        }

        public void DoReposition()
        {
            mNeedReposition = false;
            if (mChildTreeList == null)
            {
                mTotalHeight = mCachedRectTransform.rect.height;
                return;
            }
            mChildTreeList.Reposition();
            ExpandAnimType animType = ExpandAnimateType;
            if (CurExpandStatus == ExpandStatus.ExpandEnd)
            {
                float itemHeight = CachedRectTransform.rect.height;
                mChildTreeList.CachedRectTransform.anchoredPosition3D = new Vector3(ChildTreeIndent, -itemHeight, 0);
                float childTreeListHeight = mChildTreeList.ContentTotalHeight;
                mTotalHeight = mCachedRectTransform.rect.height + childTreeListHeight;
                mChildTreeList.CachedRectTransform.localScale = Vector3.one;
                mChildTreeList.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, childTreeListHeight);
            }
            else if (CurExpandStatus == ExpandStatus.CollapseEnd)
            {
                mTotalHeight = mCachedRectTransform.rect.height;
                if (animType == ExpandAnimType.Immediate)
                {
                    mChildTreeList.CachedRectTransform.localScale = Vector3.one;
                }
                else if (animType == ExpandAnimType.Scale)
                {
                    mChildTreeList.CachedRectTransform.localScale = new Vector3(1, mMinScaleValue, 1);
                }
                else if (animType == ExpandAnimType.Clip)
                {
                    mChildTreeList.CachedRectTransform.localScale = Vector3.one;
                    mChildTreeList.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
                }
            }
            else
            {
                float itemHeight = CachedRectTransform.rect.height;
                mChildTreeList.CachedRectTransform.anchoredPosition3D = new Vector3(ChildTreeIndent, -itemHeight, 0);
                float childTreeListHeight = mChildTreeList.ContentTotalHeight * mCurExpandAnimatedValue;
                mTotalHeight = mCachedRectTransform.rect.height + childTreeListHeight;
                if (animType == ExpandAnimType.Scale)
                {
                    float scaleValue = mCurExpandAnimatedValue;
                    if(scaleValue < mMinScaleValue)
                    {
                        scaleValue = mMinScaleValue;
                    }
                    mChildTreeList.CachedRectTransform.localScale = new Vector3(1, scaleValue, 1);
                    mChildTreeList.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mChildTreeList.ContentTotalHeight);
                }
                else if (animType == ExpandAnimType.Clip)
                {
                    mChildTreeList.CachedRectTransform.localScale = Vector3.one;
                    mChildTreeList.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, childTreeListHeight);
                }
            }

        }

    }

}