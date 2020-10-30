using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;


namespace SuperTreeView
{

    [CustomEditor(typeof(TreeView))]
    public class TreeViewEditor : Editor
    {

        SerializedProperty mExpandAnimType;
        SerializedProperty mItemPrefabDataList;
        SerializedProperty mExpandUseTime;
        SerializedProperty mExpandClipMoveSpeed;
        SerializedProperty mItemPadding;
        SerializedProperty mItemIndent;
        SerializedProperty mChildTreeListPadding;

        protected virtual void OnEnable()
        {
            mExpandAnimType = serializedObject.FindProperty("mExpandAnimType");
            mItemPrefabDataList = serializedObject.FindProperty("mItemPrefabDataList");
            mExpandUseTime = serializedObject.FindProperty("mExpandUseTime");
            mExpandClipMoveSpeed = serializedObject.FindProperty("mExpandClipMoveSpeed");
            mItemPadding = serializedObject.FindProperty("mItemPadding");
            mItemIndent = serializedObject.FindProperty("mItemIndent");
            mChildTreeListPadding = serializedObject.FindProperty("mChildTreeListPadding");

        }

        void ShowItemPrefabDataList()
        {
            EditorGUILayout.PropertyField(mItemPrefabDataList, new GUIContent("ItemPrefabList"));
            if (mItemPrefabDataList.isExpanded == false)
            {
                return;
            }
            EditorGUI.indentLevel += 1;
            if (GUILayout.Button("Add New"))
            {
                mItemPrefabDataList.InsertArrayElementAtIndex(mItemPrefabDataList.arraySize);
            }
            int removeIndex = -1;
            EditorGUILayout.PropertyField(mItemPrefabDataList.FindPropertyRelative("Array.size"));
            for (int i = 0; i < mItemPrefabDataList.arraySize; i++)
            {
                SerializedProperty itemData = mItemPrefabDataList.GetArrayElementAtIndex(i);
                SerializedProperty mInitCreateCount = itemData.FindPropertyRelative("mInitCreateCount");
                SerializedProperty mItemPrefab = itemData.FindPropertyRelative("mItemPrefab");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(itemData);
                if (GUILayout.Button("Remove"))
                {
                    removeIndex = i;
                }
                EditorGUILayout.EndHorizontal();
                if (itemData.isExpanded == false)
                {
                    continue;
                }
                mItemPrefab.objectReferenceValue = EditorGUILayout.ObjectField("ItemPrefab", mItemPrefab.objectReferenceValue, typeof(GameObject), true);
                mInitCreateCount.intValue = EditorGUILayout.IntField("InitCreateCount", mInitCreateCount.intValue);
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            if(removeIndex >= 0)
            {
                mItemPrefabDataList.DeleteArrayElementAtIndex(removeIndex);
            }
            EditorGUI.indentLevel -= 1;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            TreeView tTreeView = serializedObject.targetObject as TreeView;
            if(tTreeView == null)
            {
                return;
            }
            ShowItemPrefabDataList();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(mExpandAnimType, new GUIContent("ExpandAnimType"));

            if (tTreeView.ExpandAnimateType == ExpandAnimType.Clip)
            {
                EditorGUILayout.PropertyField(mExpandClipMoveSpeed, new GUIContent("ExpandClipMoveSpeed"));
            }
            else if(tTreeView.ExpandAnimateType == ExpandAnimType.Scale)
            {
                EditorGUILayout.PropertyField(mExpandUseTime, new GUIContent("ExpandUseTime"));
            }

            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(mItemPadding, new GUIContent("ItemPadding"));
            EditorGUILayout.PropertyField(mItemIndent, new GUIContent("ItemIndent"));
            EditorGUILayout.PropertyField(mChildTreeListPadding, new GUIContent("ChildTreeListPadding"));
            if (EditorGUI.EndChangeCheck())
            {
                tTreeView.NeedRepositionAll = true;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
