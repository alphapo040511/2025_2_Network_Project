using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using UnityEngine.Networking;
using Newtonsoft;
using Newtonsoft.Json;

public class AuthManager : MonoBehaviour
{
    //서버 URL 및 PlayerPrefs 키 상수 정의
    private const string SERVER_URL = "http://localhost:4000";
    private const string ACCESS_TOKEN_PREFS_KEY = "AccessToekn";
    private const string REFRESH_TOKEN_PREFS_KEY = "RefreshToekn";
    private const string TOKEN_EXPIRY_PREFS_KEY = "TokenExpiry";

    //토큰 및 만료 시간 저장 변수
    private string accessToken;
    private string refreshToken;
    private DateTime tokenExpiryTime;

    void Start()
    {
        LoadTokenFromPrefs();
    }

    //PlayerPrefs 에서 토큰 정보 로드
    private void LoadTokenFromPrefs()
    {
        accessToken = PlayerPrefs.GetString(ACCESS_TOKEN_PREFS_KEY, "");
        refreshToken = PlayerPrefs.GetString(REFRESH_TOKEN_PREFS_KEY, "");
        long expiryTicks = Convert.ToInt64(PlayerPrefs.GetString(TOKEN_EXPIRY_PREFS_KEY, "0"));
        tokenExpiryTime = new DateTime(expiryTicks);
    }

    // PlayerPrefs 에 토큰 정보 저장
    private void SaveTokenToPrefs(string accessToken, string refreshToken, DateTime expiryTime)
    {
        PlayerPrefs.SetString(ACCESS_TOKEN_PREFS_KEY, accessToken);
        PlayerPrefs.SetString(REFRESH_TOKEN_PREFS_KEY , refreshToken);
        PlayerPrefs.SetString(TOKEN_EXPIRY_PREFS_KEY, expiryTime.Ticks.ToString());

        this.accessToken = accessToken;
        this.refreshToken = refreshToken;
        this.tokenExpiryTime = expiryTime;
    }

    // 토큰 정보 저장
    private void SaveToken(string accessToken, DateTime expiryTime)
    {
        this.accessToken = accessToken;
        this.tokenExpiryTime = expiryTime;
    }

    //사용자 등록 코루틴
    public IEnumerator Register(string username, string password, Action<BaseResponse> callback)
    {
        var user = new { username, password };
        var jsonData = JsonConvert.SerializeObject(user);

        using (UnityWebRequest www = new UnityWebRequest($"{SERVER_URL}/register", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            // 통신 자체 실패
            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                // 실패 정보 담아서 전달
                BaseResponse failResponse = new BaseResponse
                {
                    success = false,
                    message = $"네트워크 오류: {www.error}"
                };
                yield break;
            }

            var response = JsonConvert.DeserializeObject<BaseResponse>(www.downloadHandler.text);

            if (response.success)
            {
                callback?.Invoke(response);
                Debug.Log($"플레이어 ID : {username} | {response.message}");
            }
            else
            {
                callback?.Invoke(response);
                Debug.LogWarning($"플레이어 ID : {username} |  {response.message}");
            }
        }
    }

    //사용자 등록 코루틴
    public IEnumerator Login(string username, string password, Action<LoginResponse> callback)
    {
        var user = new { username, password };
        var jsonData = JsonConvert.SerializeObject(user);

        using (UnityWebRequest www = new UnityWebRequest($"{SERVER_URL}/login", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            // 통신 자체 실패
            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                // 실패 정보 담아서 전달
                LoginResponse failResponse = new LoginResponse
                {
                    success = false,
                    message = $"네트워크 오류: {www.error}",
                    playerId = -1,
                    accessToken = "null"
                };
                yield break;
            }

            // 서버 응답 파싱
            var response = JsonConvert.DeserializeObject<LoginResponse>(www.downloadHandler.text);

            // 로그인 실패
            if (response.success)
            {
                Debug.Log($"플레이어 ID : {response.playerId} | 접근 토큰 : {response.accessToken}");
                SaveToken(response.accessToken, DateTime.UtcNow.AddMinutes(15));
                callback?.Invoke(response);
            }
            else  // 로그인 성공
            {
                Debug.Log($"(Error Code: {www.responseCode}) 로그인 실패: {response.message}");
                callback?.Invoke(response);
            }
        }
    }
}