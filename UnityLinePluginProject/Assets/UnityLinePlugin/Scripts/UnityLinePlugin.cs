﻿using UnityEngine;

namespace Com.Suriyun.LinePlugin {

    public class UnityLinePlugin : MonoBehaviour {
        public string channelId;
        public ILinePluginHandler Handler { get; protected set; }
        string _goName;
        public bool IsInit { get; protected set; }
        public System.Action<LineLogin> onLoginSuccess;
        public System.Action<LineError> onApiError;
        public System.Action<LineAccessToken> onAccessTokenReceived;
        public System.Action<LineCredential> onCredentialReceived;
        public System.Action<LineVerifyResult> onVerifyResultReceived;
        public System.Action<LineProfile> onProfileReceived;

        protected virtual void Awake() {
            // TODO: Create handler for iOS later.
#if UNITY_ANDROID
            Handler = new AndroidLinePluginHandler();
#elif UNITY_IOS
            Handler = new IOSLinePluginHandler();
#endif

            IsInit = false;
        }

        public void Init() {
            if (IsInit)
                return;

            if (Handler != null) {
                IsInit = true;
                if(string.IsNullOrEmpty(channelId)){
                    Debug.LogAssertion("channelId is INVALID! please set channelID use [Tools/Line Login]");
                }
                Handler.Init(name, channelId);
            }
        }

        public void Login() {
            if (!IsInit)
                return;
            Handler.Login();
        }
        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy() {
            if (!IsInit)
                return;
            Handler.Dispose();
        }

        public void LoginWebView() {
            if (!IsInit)
                return;
            Handler.LoginWebView();
        }

        public void Logout() {
            if (!IsInit)
                return;
            Handler.Logout();
        }

        public void VerifyToken() {
            if (!IsInit)
                return;
            Handler.VerifyToken();
        }

        public void GetCurrentAccessToken() {
            if (!IsInit)
                return;
            Handler.GetCurrentAccessToken();
        }

        public void RefreshToken() {
            if (!IsInit)
                return;
            Handler.RefreshToken();
        }

        public void GetProfile() {
            if (!IsInit)
                return;
            Handler.GetProfile();
        }

        public virtual void OnMessageApiError(string json) {
            LineError error = JsonUtility.FromJson<LineError>(json);
            if (onApiError != null)
                onApiError(error);
        }

        public virtual void OnMessageLoginSuccess(string json) {
            LineLogin loginResult = JsonUtility.FromJson<LineLogin>(json);
            if (onLoginSuccess != null)
                onLoginSuccess(loginResult);
        }

        public virtual void OnMessageAccessTokenReceived(string json) {
            LineAccessToken accessToken = JsonUtility.FromJson<LineAccessToken>(json);
            if (onAccessTokenReceived != null)
                onAccessTokenReceived(accessToken);
        }

        public virtual void OnMessageCredentialReceived(string json) {
            LineCredential credential = JsonUtility.FromJson<LineCredential>(json);
            if (onCredentialReceived != null)
                onCredentialReceived(credential);
        }

        public virtual void OnMessageVerifyResultReceived(string json) {
            LineVerifyResult result = JsonUtility.FromJson<LineVerifyResult>(json);
            if (onVerifyResultReceived != null)
                onVerifyResultReceived(result);
        }

        public virtual void OnMessageProfileReceived(string json) {
            LineProfile profile = JsonUtility.FromJson<LineProfile>(json);
            if (onProfileReceived != null)
                onProfileReceived(profile);
        }
    }
}