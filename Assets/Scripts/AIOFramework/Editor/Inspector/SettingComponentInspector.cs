﻿
using UnityEditor;
using UnityEngine;
using AIOFramework.Runtime;
using AIOFramework.Editor;

namespace AIOFramework.Setting
{
    [CustomEditor(typeof(SettingComponent))]
    internal sealed class SettingComponentInspector : GameFrameworkInspector
    {
        private HelperInfo<SettingHelperBase> m_SettingHelperInfo = new HelperInfo<SettingHelperBase>("Setting");

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SettingComponent t = (SettingComponent)target;

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                m_SettingHelperInfo.Draw();
            }
            EditorGUI.EndDisabledGroup();

            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Setting Count", t.Count >= 0 ? t.Count.ToString() : "<Unknown>");
                if (t.Count > 0)
                {
                    string[] settingNames = t.GetAllSettingNames();
                    foreach (string settingName in settingNames)
                    {
                        EditorGUILayout.LabelField(settingName, t.GetString(settingName));
                    }
                }
            }

            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button("Save Settings"))
                {
                    t.Save();
                }
                if (GUILayout.Button("Remove All Settings"))
                {
                    t.RemoveAllSettings();
                }
            }

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        protected override void OnCompileComplete()
        {
            base.OnCompileComplete();

            RefreshTypeNames();
        }

        private void OnEnable()
        {
            m_SettingHelperInfo.Init(serializedObject);

            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            m_SettingHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
