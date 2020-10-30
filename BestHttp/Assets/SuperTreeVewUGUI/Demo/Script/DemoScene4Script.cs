using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace SuperTreeView
{
    public class DemoScene4Script : MonoBehaviour
    {
        public TreeView mTreeView;

        int mCurSelectedItemId = 0;

        int mNewItemCount = 0;

        void Start()
        {
            ResManager rm = ResManager.Instance;
            mTreeView.OnTreeListAddOneItem = OnTreeListAddOneItem;
            mTreeView.OnTreeListDeleteOneItem = OnTreeListDeleteOneItem;
            mTreeView.OnItemExpandBegin = OnItemExpandBegin;
            mTreeView.OnItemCollapseBegin = OnItemCollapseBegin;
            mTreeView.OnItemCustomEvent = OnItemCustomEvent;

            mTreeView.InitView();

            TreeViewItem item1 = mTreeView.AppendItem("ItemPrefab1");//create an item
            item1.GetComponent<ItemScript4>().SetItemInfo("Home", "Home", "1");//update its content

            TreeViewItem item2 = mTreeView.AppendItem("ItemPrefab1");
            item2.GetComponent<ItemScript4>().SetItemInfo("Setting", "Setting", "2");

            TreeViewItem item3 = mTreeView.AppendItem("ItemPrefab1");
            item3.GetComponent<ItemScript4>().SetItemInfo("Folder", "Folder", "3");

            TreeViewItem item4 = mTreeView.AppendItem("ItemPrefab1");
            item4.GetComponent<ItemScript4>().SetItemInfo("Locked", "Locked", "4");


            TreeViewItem childItem1_1 = item1.ChildTree.AppendItem("ItemPrefab2");
            childItem1_1.GetComponent<ItemScript3>().SetText("childItem1_1 Text Content,childItem1_1 Text Content,childItem1_1 Text Content");

            TreeViewItem childItem2_1 = item2.ChildTree.AppendItem("ItemPrefab2");
            childItem2_1.GetComponent<ItemScript3>().SetText("childItem2_1 Text Content,childItem2_1 Text Content");

            TreeViewItem childItem3_1 = item3.ChildTree.AppendItem("ItemPrefab2");
            childItem3_1.GetComponent<ItemScript3>().SetText("childItem3_1 Text Content,childItem3_1 Text Content,childItem3_1 Text Content,childItem3_1 Text Content,childItem3_1 Text Content");

            TreeViewItem childItem4_1 = item4.ChildTree.AppendItem("ItemPrefab2");
            childItem4_1.GetComponent<ItemScript3>().SetText("childItem4_1 Text Content");
        }


        void OnItemExpandBegin(TreeViewItem item)
        {
            ItemScript4 st = item.GetComponent<ItemScript4>();
            st.SetExpandStatus(true);
        }

        void OnItemCollapseBegin(TreeViewItem item)
        {
            ItemScript4 st = item.GetComponent<ItemScript4>();
            st.SetExpandStatus(false);
        }

        void OnItemCustomEvent(TreeViewItem item, CustomEvent customEvent, System.Object param)
        {
            if (customEvent == CustomEvent.ItemClicked)
            {
                ItemScript4 st = item.GetComponent<ItemScript4>();
                if (mCurSelectedItemId > 0)
                {
                    if (item.ItemId == mCurSelectedItemId)
                    {
                        return;
                    }
                    TreeViewItem curSelectedItem = mTreeView.GetTreeItemById(mCurSelectedItemId);
                    if (curSelectedItem != null)
                    {
                        curSelectedItem.GetComponent<ItemScript4>().IsSelected = false;
                    }
                    mCurSelectedItemId = 0;
                }
                st.IsSelected = true;
                mCurSelectedItemId = item.ItemId;
            }
            else if(customEvent == CustomEvent.ItemDoubleClicked)
            {
                Debug.Log("Get CustomEvent.ItemDoubleClicked");
            }
        }

        void OnTreeListAddOneItem(TreeList treeList)
        {
            int count = treeList.ItemCount;
            TreeViewItem parentTreeItem = treeList.ParentTreeItem;
            if (count > 0 && parentTreeItem != null)
            {
                ItemScript4 st = parentTreeItem.GetComponent<ItemScript4>();
                st.SetExpandBtnVisible(true);
                st.SetExpandStatus(parentTreeItem.IsExpand);
            }
        }

        void OnTreeListDeleteOneItem(TreeList treeList)
        {
            int count = treeList.ItemCount;
            TreeViewItem parentTreeItem = treeList.ParentTreeItem;
            if (count == 0 && parentTreeItem != null)
            {
                ItemScript4 st = parentTreeItem.GetComponent<ItemScript4>();
                st.SetExpandBtnVisible(false);
            }
        }

        TreeViewItem CurSelectedItem
        {
            get
            {
                if (mCurSelectedItemId <= 0)
                {
                    return null;
                }
                TreeViewItem item = mTreeView.GetTreeItemById(mCurSelectedItemId);
                if (item == null)
                {
                    mCurSelectedItemId = 0;
                    return null;
                }
                return item;
            }
        }

    }
}