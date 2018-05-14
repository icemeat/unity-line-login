using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode.Custom;
using UnityEngine;

public class iOSLineLoginPostPrecess : MonoBehaviour {
    [PostProcessBuild]
    private static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath) {

        if (buildTarget != BuildTarget.iOS) {
            return;
        }
        LineLoginSetting setting = Resources.Load<LineLoginSetting>("LineLoginSetting");
        if (setting == null || setting.IsValid() == false) {
            Debug.LogErrorFormat("no LineLoginSetting Asset In Resources or Invalid");
            return;
        }
        PlistDocument plist = setting.GetInfoPlist(buildPath);
        string pbxProjPath = PBXProject.GetPBXProjectPath(buildPath);
        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromString(File.ReadAllText(pbxProjPath));

        string targetGuid = pbxProject.TargetGuidByName(PBXProject.GetUnityTargetName());
        pbxProject.UpdateBuildProperty(targetGuid, "OTHER_LDFLAGS", new string[] { "-ObjC" }, null);

        File.WriteAllText(pbxProjPath, pbxProject.WriteToString());

        if (plist.root.values.ContainsKey("LineSDKConfig")) {
            plist.root.values.Remove("LineSDKConfig");
        }
        if (plist.root.values.ContainsKey("CFBundleURLTypes")) {
            plist.root.values.Remove("CFBundleURLTypes");
        }
        if (plist.root.values.ContainsKey("LSApplicationQueriesSchemes")) {
            plist.root.values.Remove("LSApplicationQueriesSchemes");
        }
        plist.root.CreateDict("LineSDKConfig").SetString("ChannelID", setting.CHANNEL_ID);

        PlistElementArray CFBundleURLTypes = plist.root.CreateArray("CFBundleURLTypes");
        PlistElementDict CFBundleURLTypesDict = CFBundleURLTypes.AddDict();
        CFBundleURLTypesDict.SetString("CFBundleTypeRole", "Editor");
        CFBundleURLTypesDict.SetString("CFBundleTypeName", "linelogin");
        //$(PRODUCT_BUNDLE_IDENTIFIER) dosen't works for CFBundleTypeSchemes?
        //CFBundleURLTypesDict.CreateArray("CFBundleURLSchemes").AddString("line3rdp.$(PRODUCT_BUNDLE_IDENTIFIER)");
        CFBundleURLTypesDict.CreateArray("CFBundleURLSchemes").AddString("line3rdp." + Application.identifier);
        PlistElementArray arr = plist.root.CreateArray("LSApplicationQueriesSchemes");
        arr.AddString("lineauth");
        arr.AddString("line3rdp." + Application.identifier);
        plist.WriteToFile(setting.GetInfoPlistPath(buildPath));
        Debug.Log("iOS Line Login Post Process done. with ChannelID:" + setting.CHANNEL_ID);
    }

}