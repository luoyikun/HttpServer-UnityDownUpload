using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;


namespace SuperTreeView
{

    [CustomEditor(typeof(TreeViewItem))]
    public class TreeViewItemEditor : Editor
    {

        SerializedProperty mUseOverridedConfig;
        SerializedProperty mOverridedChildTreeItemPadding;
        SerializedProperty mOverridedChildTreeIndent;
        SerializedProperty mOverridedChildTreeListPadding;

        protected virtual void OnEnable()
        {
            mUseOverridedConfig = serializedObject.FindProperty("mUseOverridedConfig");
            mOverridedChildTreeItemPadding = serializedObject.FindProperty("mOverridedChildTreeItemPadding");
            mOverridedChildTreeIndent = serializedObject.FindProperty("mOverridedChildTreeIndent");
            mOverridedChildTreeListPadding = serializedObject.FindProperty("mOverridedChildTreeListPadding");

        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            TreeViewItem tTreeViewItem = serializedObject.targetObject as TreeViewItem;
            if (tTreeViewItem == null)
            {
                return;
            }
            bool useOverridedConfig = mUseOverridedConfig.boolValue;
            EditorGUILayout.PropertyField(mUseOverridedConfig, new GUIContent("UseOverridedConfig"));

            if (mUseOverridedConfig.boolValue)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(mOverridedChildTreeItemPadding, new GUIContent("OverridedChildTreeItemPadding"));
                EditorGUILayout.PropertyField(mOverridedChildTreeIndent, new GUIContent("OverridedChildTreeIndent"));
                EditorGUILayout.PropertyField(mOverridedChildTreeListPadding, new GUIContent("OverridedChildTreeListPadding"));
                if (EditorGUI.EndChangeCheck())
                {
                    tTreeViewItem.RootTreeView.NeedRepositionAll = true;
                }

            }
            if (useOverridedConfig != mUseOverridedConfig.boolValue)
            {
                tTreeViewItem.RootTreeView.NeedRepositionAll = true;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
