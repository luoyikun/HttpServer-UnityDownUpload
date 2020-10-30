using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace SuperTreeView
{
    public class DemoSceneScript : MonoBehaviour
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
            TreeViewItem item1 = mTreeView.AppendItem("ItemPrefab1");
            item1.GetComponent<ItemScript1>().SetItemInfo("Home", "Home", "1");
            TreeViewItem item2 = mTreeView.AppendItem("ItemPrefab1");
            item2.GetComponent<ItemScript1>().SetItemInfo("Setting", "Setting", "2");
            TreeViewItem item3 = mTreeView.AppendItem("ItemPrefab1");
            item3.GetComponent<ItemScript1>().SetItemInfo("Folder", "Folder", "3");
            TreeViewItem item4 = mTreeView.AppendItem("ItemPrefab1");
            item4.GetComponent<ItemScript1>().SetItemInfo("Locked", "Locked", "4");
            TreeViewItem item5 = mTreeView.AppendItem("ItemPrefab1");
            item5.GetComponent<ItemScript1>().SetItemInfo("Photo", "Photo", "5");


            TreeViewItem childItem1_1 = item1.ChildTree.AppendItem("ItemPrefab1");
            childItem1_1.GetComponent<ItemScript1>().SetItemInfo("Movie", "Movie", "1_1");
            TreeViewItem childItem1_2 = item1.ChildTree.AppendItem("ItemPrefab1");
            childItem1_2.GetComponent<ItemScript1>().SetItemInfo("Song", "Song", "1_2");
            TreeViewItem childItem1_3 = item1.ChildTree.AppendItem("ItemPrefab1");
            childItem1_3.GetComponent<ItemScript1>().SetItemInfo("Trash", "Trash", "1_3");
            TreeViewItem childItem1_4 = item1.ChildTree.AppendItem("ItemPrefab1");
            childItem1_4.GetComponent<ItemScript1>().SetItemInfo("Time", "Time", "1_4");



            TreeViewItem childItem1_1_1 = childItem1_1.ChildTree.AppendItem("ItemPrefab1");
            childItem1_1_1.GetComponent<ItemScript1>().SetItemInfo("Camera", "Camera", "1_1_1");
            TreeViewItem childItem1_1_2 = childItem1_1.ChildTree.AppendItem("ItemPrefab1");
            childItem1_1_2.GetComponent<ItemScript1>().SetItemInfo("Tools", "Tools", "1_1_2");



            TreeViewItem childItem1_1_2_1 = childItem1_1_2.ChildTree.AppendItem("ItemPrefab1");
            childItem1_1_2_1.GetComponent<ItemScript1>().SetItemInfo("Link", "Link", "1_1_2_1");
            TreeViewItem childItem1_1_2_2 = childItem1_1_2.ChildTree.AppendItem("ItemPrefab1");
            childItem1_1_2_2.GetComponent<ItemScript1>().SetItemInfo("Game", "Game", "1_1_2_2");


            TreeViewItem childItem1_1_2_2_1 = childItem1_1_2_2.ChildTree.AppendItem("ItemPrefab1");
            childItem1_1_2_2_1.GetComponent<ItemScript1>().SetItemInfo("Diamond", "Diamond", "1_1_2_2_1");



            TreeViewItem childItem3_1 = item3.ChildTree.AppendItem("ItemPrefab1");
            childItem3_1.GetComponent<ItemScript1>().SetItemInfo("Download", "Download", "3_1");
            TreeViewItem childItem3_2 = item3.ChildTree.AppendItem("ItemPrefab1");
            childItem3_2.GetComponent<ItemScript1>().SetItemInfo("Favorite", "Favorite", "3_2");

        }


        void OnItemExpandBegin(TreeViewItem item)
        {
            ItemScript1 st = item.GetComponent<ItemScript1>();
            st.SetExpandStatus(true);
        }

        void OnItemCollapseBegin(TreeViewItem item)
        {
            ItemScript1 st = item.GetComponent<ItemScript1>();
            st.SetExpandStatus(false);
        }

        void OnItemCustomEvent(TreeViewItem item, CustomEvent customEvent, System.Object param)
        {
            if (customEvent == CustomEvent.ItemClicked)
            {
                ItemScript1 st = item.GetComponent<ItemScript1>();
                if (mCurSelectedItemId > 0)
                {
                    if (item.ItemId == mCurSelectedItemId)
                    {
                        return;
                    }
                    TreeViewItem curSelectedItem = mTreeView.GetTreeItemById(mCurSelectedItemId);
                    if (curSelectedItem != null)
                    {
                        curSelectedItem.GetComponent<ItemScript1>().IsSelected = false;
                    }
                    mCurSelectedItemId = 0;
                }
                st.IsSelected = true;
                mCurSelectedItemId = item.ItemId;
            }
        }

        void OnTreeListAddOneItem(TreeList treeList)
        {
            int count = treeList.ItemCount;
            TreeViewItem parentTreeItem = treeList.ParentTreeItem;
            if (count > 0 && parentTreeItem != null)
            {
                ItemScript1 st = parentTreeItem.GetComponent<ItemScript1>();
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
                ItemScript1 st = parentTreeItem.GetComponent<ItemScript1>();
                st.SetExpandBtnVisible(false);
            }
        }

        TreeViewItem CurSelectedItem
        {
            get
            {
                if(mCurSelectedItemId <= 0)
                {
                    return null;
                }
                TreeViewItem item = mTreeView.GetTreeItemById(mCurSelectedItemId);
                if(item == null)
                {
                    mCurSelectedItemId = 0;
                    return null;
                }
                return item;
            }
        }

        public void OnExpandAllBtnClicked()
        {
            mTreeView.ExpandAllItem();
        }
        public void OnCollapseAllBtnClicked()
        {
            mTreeView.CollapseAllItem();
        }

        public void OnExpandBtnClicked()
        {
            TreeViewItem item = CurSelectedItem;
            if (item == null)
            {
                Debug.Log("Please Select a Item First");
                return;
            }
            item.Expand();
        }

        public void OnCollapseBtnClicked()
        {
            TreeViewItem item = CurSelectedItem;
            if (item == null)
            {
                Debug.Log("Please Select a Item First");
                return;
            }
            item.Collapse();
        }

        public void OnAddTongJiAtLast()
        {
            if (mTreeView.IsEmpty)
            {
                TreeViewItem childItem = mTreeView.InsertItem(0, "ItemPrefab1");
                childItem.GetComponent<ItemScript1>().SetItemInfo("Movie", "Movie" + mNewItemCount);
            }
            else
            {
                TreeViewItem item = CurSelectedItem;
                if (item == null)
                {
                    Debug.Log("Please Select a Item First");
                    return;
                }
                

                TreeViewItem childItem = item.ParentTreeList.AddItemAtLast("ItemPrefab1");

                childItem.GetComponent<ItemScript1>().SetItemInfo("Movie",  childItem.uID);
            }
        }

        public static string GetSelfID(string name,TreeViewItem item)
        {
            name += item.ItemIndex;
            if (item.ParentTreeItem != null)
            {
                //name += item.ParentTreeItem.ItemIndex;
                GetSelfID(name,item.ParentTreeItem);
            }
            else
            {
                
                Debug.Log("id:" + name);
            }
            return item.ItemIndex.ToString();
        }
        public void OnInsertBeforeBtnClicked()
        {
            mNewItemCount++;
            if (mTreeView.IsEmpty)
            {
                TreeViewItem childItem = mTreeView.InsertItem(0, "ItemPrefab1");
                childItem.GetComponent<ItemScript1>().SetItemInfo("Movie", "Movie" + mNewItemCount);
            }
            else
            {
                TreeViewItem item = CurSelectedItem;
                if (item == null)
                {
                    Debug.Log("Please Select a Item First");
                    return;
                }
                TreeViewItem childItem = item.ParentTreeList.InsertItem(item.ItemIndex, "ItemPrefab1");
                
                childItem.GetComponent<ItemScript1>().SetItemInfo("Movie", "Movie" + mNewItemCount);
            }

        }

        public void OnInsertAfterBtnClicked()
        {
            mNewItemCount++;
            if (mTreeView.IsEmpty)
            {
                TreeViewItem childItem = mTreeView.InsertItem(0, "ItemPrefab1");
                childItem.GetComponent<ItemScript1>().SetItemInfo("Movie", "Movie"+mNewItemCount);
            }
            else
            {
                TreeViewItem item = CurSelectedItem;
                if (item == null)
                {
                    Debug.Log("Please Select a Item First");
                    return;
                }
                TreeViewItem childItem = item.ParentTreeList.InsertItem(item.ItemIndex+1, "ItemPrefab1");
                childItem.GetComponent<ItemScript1>().SetItemInfo("Movie", "Movie" + mNewItemCount);
            }

        }

        public void OnAddChildBtnClicked()
        {
            mNewItemCount++;
            if (mTreeView.IsEmpty)
            {
                TreeViewItem childItem = mTreeView.AppendItem("ItemPrefab1");
                childItem.GetComponent<ItemScript1>().SetItemInfo("Movie", "Movie" + mNewItemCount);
            }
            else
            {
                TreeViewItem item = CurSelectedItem;
                if (item == null)
                {
                    Debug.Log("Please Select a Item First");
                    return;
                }
                TreeViewItem childItem = item.ChildTree.AppendItem("ItemPrefab1");
                childItem.GetComponent<ItemScript1>().SetItemInfo("Movie", "Movie" + mNewItemCount);
                

            }

        }

        public void OnAddChildAtLast()
        {
            mNewItemCount++;
            if (mTreeView.IsEmpty)
            {
                TreeViewItem childItem = mTreeView.AppendItem("ItemPrefab1");
                childItem.GetComponent<ItemScript1>().SetItemInfo("Movie", "Movie" + mNewItemCount);
            }
            else
            {
                TreeViewItem item = CurSelectedItem;
                if (item == null)
                {
                    Debug.Log("Please Select a Item First");
                    return;
                }
                TreeViewItem childItem = item.ChildTree.AddItemAtLast("ItemPrefab1");
                childItem.GetComponent<ItemScript1>().SetItemInfo("Movie", childItem.uID);
            }

        }

        public void OnDeleteBtnClicked()
        {
            TreeViewItem item = CurSelectedItem;
            if (item == null)
            {
                Debug.Log("Please Select a Item First");
                return;
            }
            item.ParentTreeList.DeleteItem(item);
        }

        public void OnBackBtnClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

    }
}