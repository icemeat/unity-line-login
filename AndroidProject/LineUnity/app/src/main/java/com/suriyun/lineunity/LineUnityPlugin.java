package com.suriyun.lineunity;

import android.app.Activity;
import android.content.Intent;
import android.util.Log;

import com.linecorp.linesdk.LineAccessToken;
import com.linecorp.linesdk.LineApiError;
import com.linecorp.linesdk.LineApiResponse;
import com.linecorp.linesdk.LineApiResponseCode;
import com.linecorp.linesdk.LineCredential;
import com.linecorp.linesdk.LineProfile;
import com.linecorp.linesdk.api.LineApiClient;
import com.linecorp.linesdk.api.LineApiClientBuilder;
import com.linecorp.linesdk.auth.LineLoginResult;
import com.unity3d.player.UnityPlayer;

import java.util.List;

public class LineUnityPlugin {
    public static final String TAG = "LineUnityPlugin";
    public static final String METHOD_API_ERROR = "OnMessageApiError";
    public static final String METHOD_LOGIN_SUCCUSS = "OnMessageLoginSuccess";
    public static final String METHOD_ACCESS_TOKEN_RECEIVED = "OnMessageAccessTokenReceived";
    public static final String METHOD_CREDENTIAL_RECEIVED = "OnMessageCredentialReceived";
    public static final String METHOD_VERIFY_RESULT_RECEIVED = "OnMessageVerifyResultReceived";
    public static final String METHOD_PROFILE_RECEIVED = "OnMessageProfileReceived";

    private static LineUnityPlugin instance;

    private LineApiClient lineApiClient;
    private String channelId = "";
    private String gameObjectName = "";
    private boolean isInit = false;

    public void init(String gameObjectName, String channelId) {
        Log.d(TAG, "init()");
        if (isInit) {
            Log.w(TAG, "Plugin has been initialized, Don't do it again!");
            return;
        }
        isInit = true;

        this.gameObjectName = gameObjectName;
        this.channelId = channelId;

        final Activity activity = UnityPlayer.currentActivity;
        LineApiClientBuilder apiClientBuilder = new LineApiClientBuilder(activity.getApplicationContext(), channelId);
        lineApiClient = apiClientBuilder.build();

        instance = this;
    }

    public static LineUnityPlugin getInstance() {
        return instance;
    }

    public boolean isInit() {
        return isInit;
    }

    private boolean checkInitToDoAction() {
        if (!isInit)
            Log.w(TAG, "Plugin does not initialized, Please do it!");
        return isInit;
    }

    public void login() {
        Log.d(TAG, "login()");
        if (!checkInitToDoAction())
            return;

        final Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Intent intent = new Intent(activity, LineLoginActivity.class);
                intent.putExtra(LineLoginActivity.EXTRA_CHANNEL_ID, channelId);
                intent.putExtra(LineLoginActivity.EXTRA_LOGIN_TYPE, LineLoginActivity.LOGIN_TYPE_AUTO);
                activity.startActivity(intent);
            }
        });
    }

    public void loginWebView() {
        Log.d(TAG, "loginWebView()");
        if (!checkInitToDoAction())
            return;

        final Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Intent intent = new Intent(activity, LineLoginActivity.class);
                intent.putExtra(LineLoginActivity.EXTRA_CHANNEL_ID, channelId);
                intent.putExtra(LineLoginActivity.EXTRA_LOGIN_TYPE, LineLoginActivity.LOGIN_TYPE_MANUAL);
                activity.startActivity(intent);
            }
        });
    }

    public void logout() {
        Log.d(TAG, "logout()");
        if (!checkInitToDoAction())
            return;

        lineApiClient.logout();
    }

    public void validateLoginResult(LineLoginResult result) {
        if (result.isSuccess()) {
            sendLoginSuccessMessage(result);
        } else {
            sendMessage(METHOD_API_ERROR, errorToJson(result.getErrorData().getMessage()));
        }
    }

    public void verifyToken() {
        Log.d(TAG, "verifyToken()");
        if (!checkInitToDoAction())
            return;

        LineApiResponse<LineCredential> response = lineApiClient.verifyToken();
        if (response.isSuccess()) {
            sendVerifyResultMessage(response.getResponseData());
        } else {
            sendApiErrorMessage(response);
        }
    }

    public void getCurrentAccessToken() {
        Log.d(TAG, "getCurrentAccessToken()");
        if (!checkInitToDoAction())
            return;

        LineApiResponse<LineAccessToken> response = lineApiClient.getCurrentAccessToken();
        if (response.isSuccess()) {
            sendAccessTokenMessage(response.getResponseData());
        } else {
            sendApiErrorMessage(response);
        }
    }

    public void refreshToken() {
        Log.d(TAG, "refreshToken()");
        if (!checkInitToDoAction())
            return;

        LineApiResponse<LineAccessToken> response = lineApiClient.refreshAccessToken();
        if (response.isSuccess()) {
            sendAccessTokenMessage(response.getResponseData());
        } else {
            sendApiErrorMessage(response);
        }
    }

    public void getProfile() {
        Log.d(TAG, "getProfile()");
        if (!checkInitToDoAction())
            return;

        LineApiResponse<LineProfile> response = lineApiClient.getProfile();
        if (response.isSuccess()) {
            sendProfileMessage(response.getResponseData());
        } else {
            sendApiErrorMessage(response);
        }
    }


    public void sendLoginSuccessMessage(LineLoginResult loginResult) {
        sendMessage(METHOD_LOGIN_SUCCUSS, loginSuccessToJson(loginResult));
    }

    public void sendAccessTokenMessage(LineAccessToken accessToken) {
        sendMessage(METHOD_ACCESS_TOKEN_RECEIVED, accessTokenToJson(accessToken));
    }

    public void sendProfileMessage(LineProfile profile) {
        sendMessage(METHOD_PROFILE_RECEIVED, profileToJson(profile));
    }

    public void sendCredentialMessage(LineCredential credential) {
        sendMessage(METHOD_CREDENTIAL_RECEIVED, credentialToJson(credential));
    }

    public void sendVerifyResultMessage(LineCredential credential) {
        sendMessage(METHOD_VERIFY_RESULT_RECEIVED, verifyResultToJson(credential));
    }

    public void sendApiErrorMessage(LineApiResponse response) {
        sendMessage(METHOD_API_ERROR, errorToJson(response.getErrorData().getMessage()));
    }

    public void sendMessage(String method, String json) {
        UnityPlayer.UnitySendMessage(gameObjectName, method, json);
    }

    public static String loginSuccessToJson(LineLoginResult result) {
        return "{\"profile\":" + profileToJson(result.getLineProfile()) + "," +
                "\"credential\":" + credentialToJson(result.getLineCredential()) + "}";
    }

    public static String accessTokenToJson(LineAccessToken accessToken) {

        return "{\"accessToken\":\"" + accessToken.getAccessToken() + "\"," +
                "\"estimatedExpirationTimeMillis\":" + accessToken.getEstimatedExpirationTimeMillis() + "," +
                "\"expiresInMillis\":" + accessToken.getExpiresInMillis() + "" +
                "}";
    }

    public static String profileToJson(LineProfile profile) {
        return "{\"userId\":\"" + profile.getUserId() + "\"," +
                "\"displayName\":\"" + profile.getDisplayName() + "\"," +
                "\"pictureUrl\":\"" + profile.getPictureUrl() + "\"," +
                "\"statusMessage\":\"" + profile.getStatusMessage() + "\"" +
                "}";
    }

    public static String credentialToJson(LineCredential credential) {
        return "{\"accessToken\":" + accessTokenToJson(credential.getAccessToken()) + "," +
                "\"permission\":" + stringListToJson(credential.getPermission()) + "}";
    }

    public static String verifyResultToJson(LineCredential credential) {
        return "{\"expiresInMillis\":" + credential.getAccessToken().getExpiresInMillis() + "," +
                "\"permission\":" + stringListToJson(credential.getPermission()) + "}";
    }

    public static String errorToJson(String errorMessage) {
        return "{\"message\":\"" + errorMessage + "\"}";
    }

    public static String stringListToJson(List<String> list) {
        String listJson = "[";
        for (int i = 0; i < list.size(); ++i) {
            if (i > 0)
                listJson += ",";
            listJson += "\"" + list.get(i) + "\"";
        }
        listJson += "]";
        return listJson;
    }
}
