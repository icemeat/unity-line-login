using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.iOS.Xcode.Custom;
using System.Runtime.InteropServices;

public class iOSLineLoginPostPrecess : MonoBehaviour {
	const string INFO_PLIST_NAME = "Info.plist";
	static string GetInfoPlistPath(string buildPath){
		return Path.Combine(buildPath, INFO_PLIST_NAME);
	}
	static PlistDocument GetInfoPlist(string buildPath){
		string plistPath = GetInfoPlistPath(buildPath);
		PlistDocument plist = new PlistDocument();
		plist.ReadFromFile(plistPath);

		return plist;
	}
	[PostProcessBuild]
	private static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath){

		if (buildTarget != BuildTarget.iOS){
			return;
		}
		PlistDocument plist = GetInfoPlist(buildPath);
		if(plist.root.values.ContainsKey("LineSDKConfig")){
			plist.root.values.Remove ("LineSDKConfig");
		}
		if(plist.root.values.ContainsKey("CFBundleURLTypes")){
			plist.root.values.Remove ("CFBundleURLTypes");
		}
		if(plist.root.values.ContainsKey("LSApplicationQueriesSchemes")){
			plist.root.values.Remove ("LSApplicationQueriesSchemes");
		}
        plist.root.CreateDict("LineSDKConfig").SetString("ChannelID","1578004721");
        
        PlistElementArray CFBundleURLTypes = plist.root.CreateArray("CFBundleURLTypes");
        PlistElementDict CFBundleURLTypesDict = CFBundleURLTypes.AddDict();
        CFBundleURLTypesDict.SetString("CFBundleTypeRole","Editor");
        CFBundleURLTypesDict.SetString("CFBundleTypeName","linelogin");
        //$(PRODUCT_BUNDLE_IDENTIFIER) dosen't works for CFBundleTypeSchemes?
        //CFBundleURLTypesDict.CreateArray("CFBundleURLSchemes").AddString("line3rdp.$(PRODUCT_BUNDLE_IDENTIFIER)");
        CFBundleURLTypesDict.CreateArray("CFBundleURLSchemes").AddString("line3rdp."+Application.identifier);
        PlistElementArray arr = plist.root.CreateArray("LSApplicationQueriesSchemes");
        arr.AddString("lineauth");
        arr.AddString("line3rdp."+Application.identifier);
		plist.WriteToFile(GetInfoPlistPath(buildPath));
	}
    

}
