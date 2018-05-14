using UnityEngine;

namespace Com.Suriyun.LinePlugin {
    public class WebViewLinePluginHandler : MonoBehaviour, ILinePluginHandler {
        WebViewObject pluginObject;
        public void Init(string gameObjectName, string channelId) {
            Debug.Log("Init() called");
        }

        public void Login() {
            Debug.Log("Login() called");
        }

        public void LoginWebView() {
            Debug.Log("LoginWebView() called");
        }

        public void Logout() {
            Debug.Log("Logout() called");
        }

        public void VerifyToken() {
            Debug.Log("VerifyToken() called");
        }

        public void GetCurrentAccessToken() {
            Debug.Log("GetCurrentAccessToken() called");
        }

        public void RefreshToken() {
            Debug.Log("RefreshToken() called");
        }

        public void GetProfile() {
            Debug.Log("GetProfile() called");
        }
        public void Dispose() {
            Debug.Log("Dispose() called");
        }

    }
}