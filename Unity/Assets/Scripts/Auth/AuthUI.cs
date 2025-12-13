using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthUI : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;

    public Button registerButton;
    public Button loginButton;

    public Text statusText;

    private AuthManager authManager;

    // Start is called before the first frame update
    void Start()
    {
        authManager = GetComponent<AuthManager>();
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

        StartCoroutine(RegisterCoroutine());
    }

    private void OnLoginClick()
    {
        StartCoroutine(LoginCoroutine());
    }

    private IEnumerator LoginCoroutine()
    {
        statusText.text = "로그인 중 ....";
        yield return StartCoroutine(authManager.Login(usernameInput.text, passwordInput.text));
        statusText.text = "로그인 성공";
    }


    private IEnumerator RegisterCoroutine()
    {
        statusText.text = "회원 가입 중 ....";
        yield return StartCoroutine(authManager.Register(
                usernameInput.text,
                passwordInput.text,
                (string message) => statusText.text = message               // 콜백으로 상태 표시
            ));
    }

}
