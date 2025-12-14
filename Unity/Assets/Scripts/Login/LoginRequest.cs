[System.Serializable]
public class LoginRequest
{
    public string username;
    public string password;
}

[System.Serializable]
public class BaseResponse
{
    public bool success;
    public string message;
}

[System.Serializable]
public class LoginResponse : BaseResponse
{
    public int playerId;
    public string accessToken;
    public string refreshToken;
}
