using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace SuperTreeView
{
    public class DemoScene2Script : MonoBehaviour
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
            mTreeView.OnTreeListRepositionFinish = OnTreeListRepositionFinish;
            mTreeView.InitView();
            TreeViewItem item1 = mTreeView.AppendItem("ItemPrefab1");
            item1.GetComponent<ItemScript2>().SetItemInfo("Home", "Home");
            TreeViewItem item2 = mTreeView.AppendItem("ItemPrefab1");
            item2.GetComponent<ItemScript2>().SetItemInfo("Setting", "Setting");
            TreeViewItem item3 = mTreeView.AppendItem("ItemPrefab1");
            item3.GetComponent<ItemScript2>().SetItemInfo("Folder", "Folder");
            TreeViewItem item4 = mTreeView.AppendItem("ItemPrefab1");
            item4.GetComponent<ItemScript2>().SetItemInfo("Locked", "Locked");
            TreeViewItem item5 = mTreeView.AppendItem("ItemPrefab1");
            item5.GetComponent<ItemScript2>().SetItemInfo("Photo", "Photo");


            TreeViewItem childItem1_1 = item1.ChildTree.AppendItem("ItemPrefab1");
            childItem1_1.GetComponent<ItemScript2>().SetItemInfo("Movie", "Movie");
            TreeViewItem childItem1_2 = item1.ChildTree.AppendItem("ItemPrefab1");
            childItem1_2.GetComponent<ItemScript2>().SetItemInfo("Song", "Song");
            TreeViewItem childItem1_3 = item1.ChildTree.AppendItem("ItemPrefab1");
            childItem1_3.GetComponent<ItemScript2>().SetItemInfo("Trash", "Trash");
            TreeViewItem childItem1_4 = item1.ChildTree.AppendItem("ItemPrefab1");
            childItem1_4.GetComponent<ItemScript2>().SetItemInfo("Time", "Time");



            TreeViewItem childItem1_1_1 = childItem1_1.ChildTree.AppendItem("ItemPrefab1");
            childItem1_1_1.GetComponent<ItemScript2>().SetItemInfo("Camera", "Camera");
            TreeViewItem childItem1_1_2 = childItem1_1.ChildTree.AppendItem("ItemPrefab1");
            childItem1_1_2.GetComponent<ItemScript2>().SetItemInfo("Tools", "Tools");



            TreeViewItem childItem1_1_2_1 = childItem1_1_2.ChildTree.AppendItem("ItemPrefab1");
            childItem1_1_2_1.GetComponent<ItemScript2>().SetItemInfo("Link", "Link");
            TreeViewItem childItem1_1_2_2 = childItem1_1_2.ChildTree.AppendItem("ItemPrefab1");
            childItem1_1_2_2.GetComponent<ItemScript2>().SetItemInfo("Game", "Game");


            TreeViewItem childItem1_1_2_2_1 = childItem1_1_2_2.ChildTree.AppendItem("ItemPrefab1");
            childItem1_1_2_2_1.GetComponent<ItemScript2>().SetItemInfo("Diamond", "Diamond");



            TreeViewItem childItem3_1 = item3.ChildTree.AppendItem("ItemPrefab1");
            childItem3_1.GetComponent<ItemScript2>().SetItemInfo("Download", "Download");
            TreeViewItem childItem3_2 = item3.ChildTree.AppendItem("ItemPrefab1");
            childItem3_2.GetComponent<ItemScript2>().SetItemInfo("Favorite", "Favorite");

        }


        void OnItemExpandBegin(TreeViewItem item)
        {
            ItemScript2 st = item.GetComponent<ItemScript2>();
            st.SetExpandStatus(true);
        }

        void OnItemCollapseBegin(TreeViewItem item)
        {
            ItemScript2 st = item.GetComponent<ItemScript2>();
            st.SetExpandStatus(false);
        }

        void OnItemCustomEvent(TreeViewItem item, CustomEvent customEvent, System.Object param)
        {
            if (customEvent == CustomEvent.ItemClicked)
            {
                ItemScript2 st = item.GetComponent<ItemScript2>();
                if (mCurSelectedItemId > 0)
                {
                    if (item.ItemId == mCurSelectedItemId)
                    {
                        return;
                    }
                    TreeViewItem curSelectedItem = mTreeView.GetTreeItemById(mCurSelectedItemId);
                    if (curSelectedItem != null)
                    {
                        curSelectedItem.GetComponent<ItemScript2>().IsSelected = false;
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
                ItemScript2 st = parentTreeItem.GetComponent<ItemScript2>();
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
                ItemScript2 st = parentTreeItem.GetComponent<ItemScript2>();
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


        public void OnInsertBeforeBtnClicked()
        {
            mNewItemCount++;
            if (mTreeView.IsEmpty)
            {
                TreeViewItem childItem = mTreeView.InsertItem(0, "ItemPrefab1");
                childItem.GetComponent<ItemScript2>().SetItemInfo("Movie", "Movie" + mNewItemCount);
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
                childItem.GetComponent<ItemScript2>().SetItemInfo("Movie", "Movie" + mNewItemCount);
            }

        }

        public void OnInsertAfterBtnClicked()
        {
            mNewItemCount++;
            if (mTreeView.IsEmpty)
            {
                TreeViewItem childItem = mTreeView.InsertItem(0, "ItemPrefab1");
                childItem.GetComponent<ItemScript2>().SetItemInfo("Movie", "Movie" + mNewItemCount);
            }
            else
            {
                TreeViewItem item = CurSelectedItem;
                if (item == null)
                {
                    Debug.Log("Please Select a Item First");
                    return;
                }
                TreeViewItem childItem = item.ParentTreeList.InsertItem(item.ItemIndex + 1, "ItemPrefab1");
                childItem.GetComponent<ItemScript2>().SetItemInfo("Movie", "Movie" + mNewItemCount);
            }

        }

        public void OnAddChildBtnClicked()
        {
            mNewItemCount++;
            if (mTreeView.IsEmpty)
            {
                TreeViewItem childItem = mTreeView.AppendItem("ItemPrefab1");
                childItem.GetComponent<ItemScript2>().SetItemInfo("Movie", "Movie" + mNewItemCount);
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
                childItem.GetComponent<ItemScript2>().SetItemInfo("Movie", "Movie" + mNewItemCount);
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

        void SetRectTransformHeight(RectTransform rt,float height)
        {
            Vector2 size = rt.sizeDelta;
            size.y = height;
            rt.sizeDelta = size;
        }

        void OnTreeListRepositionFinish(TreeList treeList)
        {
            int count = treeList.ItemCount;
            if(count == 0)
            {
                return;
            }
            //draw the line linked from item to item
            TreeViewItem item0 = treeList.GetItemByIndex(0);
            ItemScript2 itemScript0 = item0.GetComponent<ItemScript2>();
            float topY = item0.transform.localPosition.y - item0.CachedRectTransform.rect.height / 2;
            RectTransform rt0 = itemScript0.mLineVertical.GetComponent<RectTransform>();
            RectTransform lineRf= rt0;
            for (int i = 1; i< count ; ++i)
            {
                TreeViewItem item = treeList.GetItemByIndex(i);
                ItemScript2 itemScript = item.GetComponent<ItemScript2>();
                float centerY = item.transform.localPosition.y - item.CachedRectTransform.rect.height / 2;
                float dist = topY - centerY;
                topY = centerY;
                RectTransform rt = itemScript.mLineVertical.GetComponent<RectTransform>();
                //set the line length
                SetRectTransformHeight(lineRf, dist);
                lineRf = rt;
                
            }
            SetRectTransformHeight(lineRf, 0);

            //draw the line linked from item to item' childTree
            for (int i = 0; i < count; ++i)
            {
                TreeViewItem item = treeList.GetItemByIndex(i);
                ItemScript2 itemScript = item.GetComponent<ItemScript2>();
                RectTransform rt2 = itemScript.mLineVertical_2.GetComponent<RectTransform>();

                if (item.ChildItemCount > 0 && item.IsCollapseEnd == false)
                {
                    //get the childTree's first item
                    TreeViewItem childItem0 = item.ChildTree.GetItemByIndex(0);
                    ItemScript2 childItemScript0 = childItem0.GetComponent<ItemScript2>();
                    Vector3 pos0 = item.transform.InverseTransformPoint(childItemScript0.mExpandBtn.transform.position);
                    float dist2 = rt2.localPosition.y - pos0.y;
                    //set the line length
                    SetRectTransformHeight(rt2, dist2);
                }
                else
                {
                    SetRectTransformHeight(rt2, 0);
                }
            }

        }

    }
}