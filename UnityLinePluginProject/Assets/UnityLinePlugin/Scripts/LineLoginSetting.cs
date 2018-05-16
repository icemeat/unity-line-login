using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Com.Suriyun.LinePlugin {
    public class LineLoginSetting : ScriptableObject {
		public const string LINE_LOGIN_SETTING_PATH = "Plugins/LineLogin/Resources";
		public const string LINE_LOGIN_SETTING_ASSETNAME = "LineLoginSetting";
        public string plistName = "Info.plist";
        public string channelID = string.Empty;
		private static LineLoginSetting instance;
		public static LineLoginSetting Instance
		{
			get
			{
				instance = NullableInstance;

				if (instance == null)
				{
					instance = ScriptableObject.CreateInstance<LineLoginSetting>();
				}

				return instance;
			}
		}
		public static string ChannelID
		{
			get
			{
				return Instance.channelID;
			}
		}
        public static bool SetChannelID(string val){
            if (Instance.channelID != val)
            {
                Instance.channelID = val;
                return true;
            }
            return false;
        }
		public static string PListName
		{
			get
			{
				return Instance.plistName;
			}
		}
        public static bool SetPListName(string val){
            if (Instance.plistName != val)
            {
                Instance.plistName = val;
                return true;
            }
            return false;
        }

		public static LineLoginSetting NullableInstance
		{
			get
			{
				if (instance == null)
				{
					instance = Resources.Load(LINE_LOGIN_SETTING_ASSETNAME) as LineLoginSetting;
				}

				return instance;
			}
		}
        public static string GetInfoPlistPath(string buildPath) {
            return Path.Combine(buildPath, PListName);
        }
        public static bool IsValid() {
            return (!string.IsNullOrEmpty(PListName) && !string.IsNullOrEmpty(ChannelID));
        }

    }
}