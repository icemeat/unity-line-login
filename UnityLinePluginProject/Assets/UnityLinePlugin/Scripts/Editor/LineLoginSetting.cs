using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.iOS.Xcode.Custom;
using UnityEngine;

/// <summary>
/// Xcodeのプロジェクトを書き出す際の設定値
/// </summary>
public class LineLoginSetting : ScriptableObject {
    public string INFO_PLIST_NAME = "Info.plist";
    public string CHANNEL_ID = string.Empty;

    [MenuItem("Assets/Create/LineLoginSetting")]
    public static void CreateAsset() {
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/LineLoginSetting.asset");
        LineLoginSetting data = ScriptableObject.CreateInstance<LineLoginSetting>();
        AssetDatabase.CreateAsset(data, path);
        AssetDatabase.SaveAssets();
    }
    public string GetInfoPlistPath(string buildPath) {
        return Path.Combine(buildPath, INFO_PLIST_NAME);
    }
    public PlistDocument GetInfoPlist(string buildPath) {
        string plistPath = GetInfoPlistPath(buildPath);
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        return plist;
    }
    public bool IsValid() {
        return (!string.IsNullOrEmpty(INFO_PLIST_NAME) && !string.IsNullOrEmpty(CHANNEL_ID));
    }

}