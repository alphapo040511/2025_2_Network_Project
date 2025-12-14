using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthUI : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;

    public Button registerButton;
    public Button loginButton;

    public Text statusText;

    // Start is called before the first frame update
    void Start()
    {
        registerButton.onClick.AddListener(OnRegisterClick);
        loginButton.onClick.AddListener(OnLoginClick);
    }

    private void OnRegisterClick()
    {
        if (string.IsNullOrEmpty(usernameInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            statusText.text = "아이디 또는 비밀번호가 비어있습니다.";
            return;
        }

        statusText.text = "회원 가입 중 ....";
        StartCoroutine(AuthManager.Instance.Register(
                usernameInput.text,
                passwordInput.text,
                (BaseResponse response) =>
                {
                    if (response.success)
                        statusText.text = "회원 가입 성공";
                    else
                        statusText.text = $"회원 가입 실패 ({response.message})";
                }               // 콜백으로 상태 표시
            ));
    }

    private void OnLoginClick()
    {
        statusText.text = "로그인 중 ....";
        StartCoroutine(AuthManager.Instance.Login(
                usernameInput.text,
                passwordInput.text,
                (response) =>
                {
                    if (response.success)
                    {
                        statusText.text = "로그인 성공";
                        InventoryManager.Instance.FetchInvneory();
                        //SceneManager.LoadScene("ChatScene");        //로그인 성공 시 바로 게임 씬으로 이동 
                    }
                    else
                        statusText.text = $"로그인 실패 ({response.message})";
                }
            ));
    }
}
