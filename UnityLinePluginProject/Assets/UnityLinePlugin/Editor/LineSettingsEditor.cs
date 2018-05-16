
using System;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Com.Suriyun.LinePlugin {
    [InitializeOnLoad]
    [CustomEditor(typeof(LineLoginSetting))]
    public class LineSettingsEditor : Editor
    {
        [MenuItem("Tools/Line Login")]
        public static void Init()
        {
            var instance = LineLoginSetting.NullableInstance;

            if (instance == null)
            {
                string path = AssetDatabase.GenerateUniqueAssetPath("Assets/LineLoginSetting.asset");
                instance = ScriptableObject.CreateInstance<LineLoginSetting>();
                string properPath = Path.Combine(Application.dataPath, LineLoginSetting.LINE_LOGIN_SETTING_PATH);
                if (!Directory.Exists(properPath))
                {
                    Directory.CreateDirectory(properPath);
                }
                string fullPath = Path.Combine(Path.Combine("Assets", LineLoginSetting.LINE_LOGIN_SETTING_PATH), LineLoginSetting.LINE_LOGIN_SETTING_ASSETNAME+ ".asset");
                AssetDatabase.CreateAsset(instance, fullPath);
                AssetDatabase.SaveAssets();
            }
            Selection.activeObject = LineLoginSetting.Instance;
        }

        public override void OnInspectorGUI()
        {
            GUI.changed = false;
            EditorGUILayout.LabelField("Line Kit Settings", EditorStyles.boldLabel);
            EditorGUILayout.Separator ();
            EditorGUILayout.Separator ();
            EditorGUILayout.LabelField("Channel ID:");
            bool isDirty = LineLoginSetting.SetChannelID(EditorGUILayout.TextField(LineLoginSetting.ChannelID));
            EditorGUILayout.Separator ();
            EditorGUILayout.LabelField("PListName:");
            isDirty |= LineLoginSetting.SetPListName(EditorGUILayout.TextField(LineLoginSetting.PListName));

            if (GUI.changed || isDirty) {
		        EditorUtility.SetDirty((LineLoginSetting)target);
            }
        }

    }
}

