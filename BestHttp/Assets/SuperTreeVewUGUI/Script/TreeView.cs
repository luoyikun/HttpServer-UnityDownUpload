using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace SuperTreeView
{

    public class ItemPool
    {
        GameObject mPrefabObj;
        string mPrefabName;
        int mInitCreateCount = 1;
        List<TreeViewItem> mPooledItemList = new List<TreeViewItem>();
        static int mCurItemIdCount = 0;
        public ItemPool(GameObject prefabObj, int createCount)
        {
            mPrefabObj = prefabObj;
            mPrefabName = mPrefabObj.name;
            mInitCreateCount = createCount;
        }
        public void Init()
        {
            mPrefabObj.SetActive(false);
            for (int i = 0; i < mInitCreateCount; ++i)
            {
                TreeViewItem tViewItem = CreateItem();
                RecycleItem(tViewItem);
            }
        }
        public TreeViewItem GetItem()
        {
            mCurItemIdCount++;
            int count = mPooledItemList.Count;
            TreeViewItem tItem = null;
            if (count == 0)
            {
                tItem = CreateItem();
            }
            else
            { 
                tItem = mPooledItemList[count - 1];
                mPooledItemList.RemoveAt(count - 1);
                tItem.gameObject.SetActive(true);
            }
            tItem.ItemId = mCurItemIdCount;
            return tItem;

        }
        public TreeViewItem CreateItem()
        {
            
            GameObject go = GameObject.Instantiate<GameObject>(mPrefabObj);
            go.SetActive(true);
            TreeViewItem tViewItem = go.GetComponent<TreeViewItem>();
            tViewItem.ItemPrefabName = mPrefabName;
            return tViewItem;
        }
        public void RecycleItem(TreeViewItem item)
        {
            item.gameObject.SetActive(false);
            mPooledItemList.Add(item);
        }
    }
    [System.Serializable]
    public class ItemPrefabConfData
    {
        public GameObject mItemPrefab;
        public int mInitCreateCount;
    }

    public class TreeView : TreeList
    {
        //called when a item is added to a TreeList
        public Action<TreeList> OnTreeListAddOneItem;
        //called when a item is deleted to a TreeList
        public Action<TreeList> OnTreeListDeleteOneItem;
        //called when a item is begin to expand its chlidtree.
        public Action<TreeViewItem> OnItemExpandBegin;
        //called when a item is expanding its chlidtree.
        public Action<TreeViewItem> OnItemExpanding;
        //called when a item has expanded its chlidtree.
        public Action<TreeViewItem> OnItemExpandEnd;
        //called when a item is begin to collapse its chlidtree.
        public Action<TreeViewItem> OnItemCollapseBegin;
        //called when a item is collapsing its chlidtree.
        public Action<TreeViewItem> OnItemCollapsing;
        //called when a item has collapsed its chlidtree.
        public Action<TreeViewItem> OnItemCollapseEnd;
        //called when a custom event is raised. you can raise a custom event by 
        //call TreeViewItem:RaiseCustomEvent(CustomEvent customEvent,System.Object param)
        //to notify the TreeView something happens, such as a TreeViewItem is clicked.
        //This callback is a bridge to connect the TreeView and its child TreeViewItems.
        public Action<TreeViewItem, CustomEvent,System.Object> OnItemCustomEvent;
        //called when a TreeList reposition method finished.
        public Action<TreeList> OnTreeListRepositionFinish;

        [SerializeField]
        float mExpandUseTime = 5f;


        [SerializeField]
        float mExpandClipMoveSpeed = 100f;

        Dictionary<string, ItemPool> mItemPoolDict = new Dictionary<string, ItemPool>();
        RectTransform mPoolRootTrans;
        Dictionary<int, TreeViewItem> mTreeViewItemDict = new Dictionary<int, TreeViewItem>();
        int mCurTreeItemDictVersion = 0;

        List<TreeViewItem> mAllTreeViewItemList = null;
        int mCurTreeItemListVersion = 0;

        bool mNeedRepositionView = true;

        bool mNeedRepositionAll = true;

        [SerializeField]
        List<ItemPrefabConfData> mItemPrefabDataList = new List<ItemPrefabConfData>();

        [SerializeField]
        ExpandAnimType mExpandAnimType = ExpandAnimType.Clip;
        public ExpandAnimType ExpandAnimateType
        {
            get { return mExpandAnimType; }
            set { mExpandAnimType = value; }
        }

        public bool NeedRepositionAll
        {
            get { return mNeedRepositionAll; }
            set { mNeedRepositionAll = value;}
        }


        [SerializeField]
        float mItemIndent;
        [SerializeField]
        float mChildTreeListPadding;
        [SerializeField]
        float mItemPadding;


        public float ItemIndent
        {
            get { return mItemIndent; }
            set
            {
                mItemIndent = value;
                NeedRepositionAll = true;
            }
        }

        public float ChildTreeListPadding
        {
            get { return mChildTreeListPadding; }
            set
            {
                mChildTreeListPadding = value;
                NeedRepositionAll = true;
            }
        }

        public float ItemPadding
        {
            get { return mItemPadding; }
            set
            {
                mItemPadding = value;
                NeedRepositionAll = true;
            }
        }



        public float ExpandUseTime
        {
            get { return mExpandUseTime; }
            set { mExpandUseTime = value; }
        }


        public float ExpandClipMoveSpeed
        {
            get { return mExpandClipMoveSpeed; }
            set { mExpandClipMoveSpeed = value; }
        }


        public bool NeedRepositionView
        {
            get { return mNeedRepositionView; }
            set { mNeedRepositionView = value; }
        }

        public List<TreeViewItem> AllTreeViewItemList
        {
            get
            {
                if (mAllTreeViewItemList != null)
                {
                    if (mCurTreeItemDictVersion == mCurTreeItemListVersion)
                    {
                        return mAllTreeViewItemList;
                    }
                }
                mCurTreeItemListVersion = mCurTreeItemDictVersion;
                mAllTreeViewItemList = new List<TreeViewItem>(mTreeViewItemDict.Values);
                return mAllTreeViewItemList;
            }
        }

        public void InitView()
        {
            CachedRectTransform.anchorMax = new Vector2(0, 1);
            CachedRectTransform.anchorMin = CachedRectTransform.anchorMax;
            CachedRectTransform.pivot = new Vector2(0, 1);
            GameObject poolObj = new GameObject();
            poolObj.name = "ItemPool";
            RectTransform tf = poolObj.GetComponent<RectTransform>();
            if (tf == null)
            {
                tf = poolObj.AddComponent<RectTransform>();
            }
            tf.anchorMax = new Vector2(0.5f, 0.5f);
            tf.anchorMin = tf.anchorMax;
            tf.pivot = new Vector2(0.5f, 0.5f);
            tf.SetParent(CachedRectTransform);
            tf.anchoredPosition3D = Vector3.zero;
            mPoolRootTrans = tf;
            RootTreeView = this;
            ParentTreeItem = null;
            InitItemPool();
        }

        void InitItemPool()
        {
            foreach (ItemPrefabConfData data in mItemPrefabDataList)
            {
                if (data.mItemPrefab == null)
                {
                    Debug.LogError("A item prefab is null ");
                    return;
                }
                string prefabName = data.mItemPrefab.name;
                if (mItemPoolDict.ContainsKey(prefabName))
                {
                    Debug.LogError("A item prefab with name " + prefabName + " has existed!");
                    return;
                }
                RectTransform rtf = data.mItemPrefab.GetComponent<RectTransform>();
                if(rtf == null)
                {
                    Debug.LogError("RectTransform component is not found in the prefab " + prefabName);
                    return;
                }
                rtf.anchorMax = new Vector2(0, 1);
                rtf.anchorMin = rtf.anchorMax;
                rtf.pivot = new Vector2(0, 1);
                TreeViewItem tItem = data.mItemPrefab.GetComponent<TreeViewItem>();
                if(tItem == null)
                {
                    data.mItemPrefab.AddComponent<TreeViewItem>();
                }
                ItemPool pool = new ItemPool(data.mItemPrefab, data.mInitCreateCount);
                pool.Init();
                mItemPoolDict.Add(prefabName, pool);
            }
        }

        //This method is used to get a TreeViewItem by its itemId.Every TreeViewItem has a itemId property, 
        //and the value is unique and not repeated.
        public TreeViewItem GetTreeItemById(int itemId)
        {
            TreeViewItem item = null;
            if (mTreeViewItemDict.TryGetValue(itemId, out item))
            {
                return item;
            }
            return null;
        }

        public TreeViewItem NewTreeItem(string itemPrefabName)
        {
            ItemPool pool = null;
            if (mItemPoolDict.TryGetValue(itemPrefabName, out pool) == false)
            {
                return null;
            }
            TreeViewItem item = pool.GetItem();
            mTreeViewItemDict.Add(item.ItemId, item);
            mCurTreeItemDictVersion++;
            return item;
        }

        public void RecycleTreeItem(TreeViewItem item)
        {
            if (item == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(item.ItemPrefabName))
            {
                return;
            }
            ItemPool pool = null;
            if (mItemPoolDict.TryGetValue(item.ItemPrefabName, out pool) == false)
            {
                return;
            }
            item.ParentTreeList = null;
            mTreeViewItemDict.Remove(item.ItemId);
            mCurTreeItemDictVersion++;
            item.CachedRectTransform.SetParent(mPoolRootTrans);
            pool.RecycleItem(item);

        }


        void Update()
        {
            int count = mTreeItemList.Count;
            for (int i = 0; i < count; ++i)
            {
                TreeViewItem tItem = mTreeItemList[i];
                tItem.OnUpdate();
                if (tItem.NeedReposition)
                {
                    NeedRepositionView = true;
                }
            }
            NeedRepositionAll = false;
            if (NeedRepositionView)
            {
                NeedRepositionView = false;
                DoReposition();
                if(OnTreeListRepositionFinish != null)
                {
                    OnTreeListRepositionFinish(this);
                }
            }
        }
    }

}