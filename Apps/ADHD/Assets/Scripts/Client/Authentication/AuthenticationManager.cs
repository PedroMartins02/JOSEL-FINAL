using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class AuthenticationManager : MonoBehaviour
{
    [SerializeField] private GameObject LogInPanel;
    [SerializeField] private GameObject RegisterPanel;

    [SerializeField] private TMP_InputField LogInUsername;
    [SerializeField] private TMP_InputField LogInPassword;
    [SerializeField] private TextMeshProUGUI LoginErrorText;

    [SerializeField] private TMP_InputField RegisterUsername;
    [SerializeField] private TMP_InputField RegisterPassword;
    [SerializeField] private TMP_InputField RegisterConfirmPassword;
    [SerializeField] private TextMeshProUGUI RegisterErrorText;

    private bool IsShowingLogIn = true;

    async void Start()
    {
        await UnityServices.InitializeAsync();
        SetupEvents();

        DisplayCurrentPanel();
    }

    private void DisplayCurrentPanel()
    {
        LogInPanel.SetActive(IsShowingLogIn);
        RegisterPanel.SetActive(!IsShowingLogIn);

        ClearFields();
    }

    private void ClearFields()
    {
        LogInUsername.text = string.Empty;
        LogInPassword.text = string.Empty;
        LoginErrorText.text = string.Empty;

        RegisterUsername.text = string.Empty;
        RegisterPassword.text = string.Empty;
        RegisterConfirmPassword.text = string.Empty;
        RegisterErrorText.text = string.Empty;
    }

    public void ToggleScreen()
    {
        IsShowingLogIn = !IsShowingLogIn;
        DisplayCurrentPanel();
    }

    public void LogIn()
    {
        LoginErrorText.text = string.Empty;
        if (LogInUsername.text == string.Empty)
        {
            LoginErrorText.text = "Username cannot be empty!";
            return;
        }
        if (LogInPassword.text == string.Empty)
        {
            LoginErrorText.text = "Password cannot be empty!";
            return;
        }
        LoginUser(LogInUsername.text, LogInPassword.text);
    }

    public void Register()
    {
        RegisterErrorText.text = string.Empty;
        if (RegisterUsername.text == string.Empty)
        {
            RegisterErrorText.text = "Username cannot be empty!";
            return;
        }
        if (RegisterPassword.text == string.Empty)
        {
            RegisterErrorText.text = "Password cannot be empty!";
            return;
        }
        if (RegisterPassword.text != RegisterConfirmPassword.text)
        {
            RegisterErrorText.text = "Passwords do not match!";
            return;
        }
        RegisterUser(RegisterUsername.text, RegisterPassword.text);

    }

    private void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
        };

        AuthenticationService.Instance.SignInFailed += (err) =>
        {
            Debug.LogError(err);
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("Player signed out.");
        };

        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session expired.");
        };
    }

    private async void RegisterUser(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            Debug.Log("Registration successful!");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError($"Registration failed: {ex.Message}");
            RegisterErrorText.text = ex.Message;
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError($"Request failed: {ex.Message}");
            RegisterErrorText.text = ex.Message;
        }
    }

    private async void LoginUser(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Debug.Log("Login successful!");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError($"Login failed: {ex.Message}");
            LoginErrorText.text = ex.Message;
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError($"Request failed: {ex.Message}");
            LoginErrorText.text = ex.Message;
        }
    }
}
