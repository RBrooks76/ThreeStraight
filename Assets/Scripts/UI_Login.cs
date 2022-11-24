using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using SimpleFileBrowser;
using System.IO;
using UnityEngine.UI;
using System.Threading.Tasks;
using Google;
using Facebook.Unity;
using UnityEditor;
using UnityEngine.Networking;

using static NativeGallery;
using static NativeCamera;

public class UI_Login : MonoBehaviour
{
    static public UI_Login UIL;
    public Transform loginArea;
    public Transform avatarArea;
    public Transform choiceAlert;
    public Transform registerArea;
    public Transform guideArea;
    public Transform changePasswordArea;
    public Transform errorAlert;

    public string imgUrl = string.Empty;
    public string imgName = string.Empty;
    public int avatar_number = -1;

    string userEmail = string.Empty;

    public string webClientId = "182212612555-97ptqidikdm8hf5tcoj59omlckm1e7vv.apps.googleusercontent.com";
    public string iOSwebClientId = "1014485479365-se0mc7gqvd5iukiipon5tiftbjqcro13.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;

    // Start is called before the first frame update
    void Start()
    {
        if (UIL == null) UIL = this;
        SetScreenHeightWithInputField();

        if (Application.platform == RuntimePlatform.Android)
        {
            configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                RequestIdToken = true
            };
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            configuration = new GoogleSignInConfiguration
            {
                WebClientId = iOSwebClientId,
                RequestIdToken = true
            };
        }

        InitFB();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void PickImage(int maxSize, int index)
    {

        if (index == 0)
        {
            if (NativeCamera.IsCameraBusy())
                return;
            NativeCamera.Permission permission_2 = NativeCamera.TakePicture((path) =>
            {
                if (path != null)
                {
                    // Create a Texture2D from the captured image
                    Texture2D _texture = NativeCamera.LoadImageAtPath(path, maxSize);
                    if (_texture == null)
                    {
                        return;
                    }
                    else
                    {
                        Cropping(_texture);
                    }
                }
            }, maxSize);
        }
        else
        {
            if (NativeGallery.IsMediaPickerBusy())
                return;
            NativeGallery.Permission permission_1 = NativeGallery.GetImageFromGallery((path) =>
            {
                if (path != null)
                {
                    // Create Texture from selected image
                    Texture2D _texture = NativeGallery.LoadImageAtPath(path, -1, false, false);
                    if (_texture == null)
                    {
                        return;
                    }
                    else
                    {
                        Cropping(_texture);
                    }
                }
            });
        }
    }

    private void Cropping(Texture2D myTexture)
    {
        bool autoZoom = true;

        ImageCropper.Instance.Show(myTexture, (bool result, Texture originalImage, Texture2D croppedImage) => {

            if (result)
            {
                //UI_Main.UIM.sel_img_texture = croppedImage;
                createReadabeTexture2D(croppedImage);
                byte[] pngData = createReadabeTexture2D(croppedImage).EncodeToPNG();
                CreatePNG(pngData);
                avatarArea.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = UI_Main.UIM.sel_img_texture;
                avatar_number = 0;
            }
        },
        settings: new ImageCropper.Settings()
        {
            ovalSelection = true,
            autoZoomEnabled = autoZoom,
            imageBackground = Color.clear, // transparent background
            selectionMinAspectRatio = 1.0f,
            selectionMaxAspectRatio = 1.0f
        },
        croppedImageResizePolicy: (ref int width, ref int height) =>
        {
            // uncomment lines below to save cropped image at half resolution
            //width /= 2;
            //height /= 2;
        }); ;
    }

    public Texture2D createReadabeTexture2D(Texture2D texture2d)
    {
        RenderTexture renderTexture = RenderTexture.GetTemporary(
                    texture2d.width,
                    texture2d.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(texture2d, renderTexture);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;
        Texture2D readableTextur2D = new Texture2D(texture2d.width, texture2d.height);
        readableTextur2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        readableTextur2D.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);

        return readableTextur2D;
    }


    // set screenHeight with inputField.
    private void SetScreenHeightWithInputField()
    {
        loginArea.GetComponent<RectTransform>().SetHeight(Screen.height * 2436 / Screen.width);
        registerArea.GetComponent<RectTransform>().SetHeight(Screen.height * 2436 / Screen.width);
        changePasswordArea.GetComponent<RectTransform>().SetHeight(Screen.height * 2436 / Screen.width);
    }
    #region  ---- Login

    private void InitFB()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }
    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }
    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void OnClickFBLogin()
    {
        if ((Application.platform == RuntimePlatform.Android) || (Application.platform == RuntimePlatform.IPhonePlayer))
        {
            if (FB.IsInitialized)
            {
                UI_Main.UIM.activityBar.gameObject.SetActive(true);
                var perms = new List<string>() { "public_profile", "email" };
                FB.LogInWithReadPermissions(perms, AuthCallback);
            }
        }
        else
        {
            if (FB.IsInitialized)
            {
                UI_Main.UIM.activityBar.gameObject.SetActive(true);
                var perms = new List<string>() { "public_profile", "email" };
                FB.LogInWithReadPermissions(perms, AuthCallback);
            }
        }
    }
    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            string token = aToken.TokenString;
            Debug.Log(aToken.UserId);
            Debug.Log("fb token: " + token);

            UI_Main.UIM.activityBar.gameObject.SetActive(true);
            gameApi.request.facebookLogin(token);
        }
        else
        {
            UI_Main.UIM.activityBar.gameObject.SetActive(false);
            Debug.Log("User cancelled login");
        }
    }

    public void OnClickGoogleLogin()
    {
        //GotoGamePage();

        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        UI_Main.UIM.activityBar.gameObject.SetActive(false);
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith((Task<GoogleSignInUser> task) => {
            if (task.IsFaulted)
            {
                using (IEnumerator<System.Exception> enumerator =
                        task.Exception.InnerExceptions.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                        Debug.Log("Got Error: " + error.Status + " " + error.Message);
                        UI_Main.UIM.activityBar.gameObject.SetActive(false);
                    }
                    else
                    {
                        Debug.Log("Got Unexpected Exception: " + task.Exception);
                        UI_Main.UIM.activityBar.gameObject.SetActive(false);
                    }
                }
            }
            else if (task.IsCanceled)
            {
                Debug.Log("Google Login Cancelled");
                //loginArea.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Google Login Cancelled";
                UI_Main.UIM.activityBar.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Google Logged in " + task.Result.DisplayName + "!");
                string token = task.Result.IdToken;
                Debug.Log("token: " + token);
                gameApi.request.googleLogin(token);
                //loginArea.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Google Logged in " + task.Result.DisplayName + "!" + "token: " + token;
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
    public void OnSignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
    }
    public void OnDisconnect()
    {
        GoogleSignIn.DefaultInstance.Disconnect();
    }
    public void OnClickEmailLogin()
    {
        string email = loginArea.GetChild(1).GetChild(2).GetComponent<TMP_InputField>().text;
        string pw = loginArea.GetChild(1).GetChild(3).GetComponent<TMP_InputField>().text;

        string[] a_count = email.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
        string[] dot_count = null;
        if (a_count.Length > 1)
            dot_count = a_count[1].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

        if (string.IsNullOrEmpty(email))
        {
            // error alert
            SetErrAlertText(gamePropertySettings.EMAIL_INPUT_EMPTY);
            errorAlert.gameObject.SetActive(true);
            return;
        }
        else
        {
            if (dot_count == null || dot_count.Length < 2)
            {
                // error alert
                SetErrAlertText(gamePropertySettings.EMAIL_INPUT_ERROR);
                errorAlert.gameObject.SetActive(true);
                return;
            }
            if (a_count.Length > 2)
            {
                // error alert
                SetErrAlertText(gamePropertySettings.EMAIL_INPUT_ERROR);
                errorAlert.gameObject.SetActive(true);
                return;
            }
        }
        if (string.IsNullOrEmpty(pw))
        {
            // error alert
            SetErrAlertText(gamePropertySettings.PASSWORD_INPUT_EMPTY);
            errorAlert.gameObject.SetActive(true);
            return;
        }
        gameApi.request.UserLoginApi(string.Empty, email, pw, string.Empty, string.Empty, "LOGIN");
    }
    public void OnClickForgetPw()
    {
        loginArea.gameObject.SetActive(false);
        changePasswordArea.gameObject.SetActive(true);
        UI_Main.UIM.loginPage.GetComponent<Animation>().Play();
    }
    public void OnClickGoRegister()
    {
        UI_Main.UIM.loginPage.GetComponent<Animation>().Play();
        loginArea.gameObject.SetActive(false);
        avatarArea.gameObject.SetActive(true);
        InitializeInputField();
    }
#endregion

#region  ---- Avatar selection
    /**
     *
     *  Avatar Selection
     *
     **/
    public void OnClickNext()
    {
        if (UI_Main.UIM.sel_img_texture != null)
        {
            avatarArea.gameObject.SetActive(false);
            registerArea.gameObject.SetActive(true);
            UI_Main.UIM.loginPage.GetComponent<Animation>().Play();
            InitializeInputField();
        }
        else
        {
            SetErrAlertText(gamePropertySettings.SELECT_AVATAR_REQUIRED);
            errorAlert.gameObject.SetActive(true);
        }
    }
    public void OnClickBackFromAvatar()
    {
        loginArea.gameObject.SetActive(true);
        avatarArea.gameObject.SetActive(false);
        gameApi.request.socialId = string.Empty;
        InitializeInputField();
        UI_Main.UIM.loginPage.GetComponent<Animation>().Play();
    }
    public void OnClickSelectAvatar(int i)
    {
        disableAllAvatars();

        if (i == 0)
        {
            avatarArea.GetChild(2).GetChild(i).GetChild(0).gameObject.SetActive(true);
            avatarArea.GetChild(2).GetChild(i).GetChild(1).gameObject.SetActive(true);

            if (string.IsNullOrEmpty(gameApi.request.socialId))
            {
                UI_Main.UIM.sel_img_texture = null;
            }
            choiceAlert.gameObject.SetActive(true);
        }
        else
        {
            avatarArea.GetChild(2).GetChild(i).GetChild(0).gameObject.SetActive(true);
            avatarArea.GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(true);
            avatarArea.GetChild(2).GetChild(0).GetChild(1).gameObject.SetActive(false);
            avatar_number = i + 1;
            Texture2D s_path = Resources.Load<Texture2D>("designs/" + avatar_number);
            CreatePNG(s_path.EncodeToPNG());
        }
    }
    public void onSelectImgSource(int index)
    {
        choiceAlert.gameObject.SetActive(false);
        PickImage(512, index);
    }
    private void selectFileFromExplorer()
    {
        FileBrowser.SetFilters(false, ".png");
        FileBrowser.ShowLoadDialog(OnResultSelectFile, null, false, null, "Choose an image as your avatar", "Select");
    }
    public void CreatePNG(byte[] b_array)
    {
        UI_Main.UIM.sel_img_texture = new Texture2D(1, 1);
        UI_Main.UIM.sel_img_texture.LoadImage(b_array);
    }
    private void OnResultSelectFile(string path)
    {
        imgUrl = path;
        imgName = Path.GetFileName(imgUrl);
        byte[] infoP = File.ReadAllBytes(imgUrl);
        CreatePNG(infoP);
        avatarArea.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = UI_Main.UIM.sel_img_texture;
        // set bool true because of custom avatar.
        avatar_number = 0;
    }
    private void disableAllAvatars()
    {
        for (int idx = 0; idx < 5; idx++)
        {
            avatarArea.GetChild(2).GetChild(idx).GetChild(0).gameObject.SetActive(false);
        }
    }
    public void AssignSocialAvatar(Texture2D socialAvatar)
    {
        avatarArea.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = socialAvatar;
    }
    #endregion

#region ---- Sign up
    /**
     *
     *  Sign up
     *
     * */
    public void OnClickRegister()
    {
        string name = registerArea.GetChild(2).GetComponent<TMP_InputField>().text;
        string email = registerArea.GetChild(3).GetComponent<TMP_InputField>().text;
        string pw = registerArea.GetChild(4).GetComponent<TMP_InputField>().text;
        string confpw = registerArea.GetChild(5).GetComponent<TMP_InputField>().text;
        string selectedCountry = registerArea.GetChild(6).GetChild(0).GetComponent<TextMeshProUGUI>().text;

        string[] a_count = email.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
        string[] dot_count = null;
        if (a_count.Length > 1)
            dot_count = a_count[1].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

        if (string.IsNullOrEmpty(name))
        {
            // error alert
            SetErrAlertText(gamePropertySettings.NAME_INPUT_ERROR);
            errorAlert.gameObject.SetActive(true);
            return;
        }
        if (string.IsNullOrEmpty(email))
        {
            // error alert
            SetErrAlertText(gamePropertySettings.EMAIL_INPUT_EMPTY);
            errorAlert.gameObject.SetActive(true);
            return;
        }
        else
        {
            if (dot_count == null || dot_count.Length < 2)
            {
                // error alert
                SetErrAlertText(gamePropertySettings.EMAIL_INPUT_ERROR);
                errorAlert.gameObject.SetActive(true);
                return;
            }
            if (a_count.Length > 2)
            {
                // error alert
                SetErrAlertText(gamePropertySettings.EMAIL_INPUT_ERROR);
                errorAlert.gameObject.SetActive(true);
                return;
            }
        }
        if (string.IsNullOrEmpty(gameApi.request.socialId))
        {
            if (string.IsNullOrEmpty(pw))
            {
                // error alert
                SetErrAlertText(gamePropertySettings.PASSWORD_INPUT_EMPTY);
                errorAlert.gameObject.SetActive(true);
                return;
            }
            if (pw != confpw)
            {
                // error alert
                SetErrAlertText(gamePropertySettings.PASSWORD_INPUT_ERROR);
                errorAlert.gameObject.SetActive(true);
                return;
            }
            if (pw.Length < 8)
            {
                // error alert
                SetErrAlertText(gamePropertySettings.PASSWORD_LESS_CHARACTERS);
                errorAlert.gameObject.SetActive(true);
                return;
            }
        }
        else
        {
            pw = string.Empty;
        }
        gameApi.request.UploadAvatar(name, email, pw, selectedCountry, avatar_number, "REGISTER");
    }
    public void OnClickBackFromRegister()
    {
        avatarArea.gameObject.SetActive(true);
        registerArea.gameObject.SetActive(false);
        InitializeInputField();
        UI_Main.UIM.loginPage.GetComponent<Animation>().Play();
    }
    public void GotoGamePage()
    {
        UI_Main.UIM.DispGamePlayPage();
        InitializeInputField();
    }
#endregion
    /**
     *
     *  initialize all input field
     *
     * */
    private void InitializeInputField()
    {
        if (string.IsNullOrEmpty(gameApi.request.socialId))
        {
            if (registerArea.gameObject.activeSelf)
            {
                registerArea.GetChild(3).GetComponent<TMP_InputField>().text = string.Empty;
                registerArea.GetChild(4).GetComponent<TMP_InputField>().text = string.Empty;
                registerArea.GetChild(5).GetComponent<TMP_InputField>().text = string.Empty;
                registerArea.GetChild(2).GetComponent<TMP_InputField>().text = string.Empty;
                registerArea.GetChild(3).GetComponent<TMP_InputField>().interactable = true;
                registerArea.GetChild(4).GetComponent<TMP_InputField>().interactable = true;
                registerArea.GetChild(5).GetComponent<TMP_InputField>().interactable = true;
            }
            else if (loginArea.gameObject.activeSelf)
            {
                loginArea.GetChild(1).GetChild(2).GetComponent<TMP_InputField>().text = string.Empty;
                loginArea.GetChild(1).GetChild(3).GetComponent<TMP_InputField>().text = string.Empty;
            }
            if (!registerArea.gameObject.activeSelf && !loginArea.gameObject.activeSelf)
            {
                registerArea.GetChild(3).GetComponent<TMP_InputField>().text = string.Empty;
                registerArea.GetChild(4).GetComponent<TMP_InputField>().text = string.Empty;
                registerArea.GetChild(5).GetComponent<TMP_InputField>().text = string.Empty;
                registerArea.GetChild(2).GetComponent<TMP_InputField>().text = string.Empty;
                loginArea.GetChild(1).GetChild(2).GetComponent<TMP_InputField>().text = string.Empty;
                loginArea.GetChild(1).GetChild(3).GetComponent<TMP_InputField>().text = string.Empty;
            }
            for (int i=0; i< 3; i++)
            {
                changePasswordArea.GetChild(i).GetChild(2).GetComponent<TMP_InputField>().text = string.Empty;
            }
            changePasswordArea.GetChild(2).GetChild(3).GetComponent<TMP_InputField>().text = string.Empty;
        }
        else
        {
            if (registerArea.gameObject.activeSelf)
            {
                registerArea.GetChild(2).GetComponent<TMP_InputField>().text = UI_Main.UIM.playerName;
                registerArea.GetChild(3).GetComponent<TMP_InputField>().text = UI_Main.UIM.playerEmail;
                registerArea.GetChild(3).GetComponent<TMP_InputField>().interactable = false;
                registerArea.GetChild(4).GetComponent<TMP_InputField>().interactable = false;
                registerArea.GetChild(5).GetComponent<TMP_InputField>().interactable = false;
                registerArea.GetChild(4).GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                registerArea.GetChild(5).GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            }
            if (!registerArea.gameObject.activeSelf && !loginArea.gameObject.activeSelf)
            {
                registerArea.GetChild(3).GetComponent<TMP_InputField>().text = string.Empty;
                registerArea.GetChild(4).GetComponent<TMP_InputField>().text = string.Empty;
                registerArea.GetChild(5).GetComponent<TMP_InputField>().text = string.Empty;
                registerArea.GetChild(2).GetComponent<TMP_InputField>().text = string.Empty;
                loginArea.GetChild(1).GetChild(2).GetComponent<TMP_InputField>().text = string.Empty;
                loginArea.GetChild(1).GetChild(3).GetComponent<TMP_InputField>().text = string.Empty;
            }
            for (int i = 0; i < 3; i++)
            {
                changePasswordArea.GetChild(i).GetChild(2).GetComponent<TMP_InputField>().text = string.Empty;
            }
            changePasswordArea.GetChild(2).GetChild(3).GetComponent<TMP_InputField>().text = string.Empty;
        }
    }

#region ---- How to play
    /**
     *
     *  How to play
     *
     * **/
    public void GotoHowToPage()
    {
        registerArea.gameObject.SetActive(false);
        guideArea.gameObject.SetActive(true);
        InitializeInputField();
        UI_Main.UIM.loginPage.GetComponent<Animation>().Play();
    }
    public void OnClickNextInsideGuide()
    {
        transform.gameObject.SetActive(false);
        loginArea.gameObject.SetActive(true);
        guideArea.gameObject.SetActive(false);
        UI_Main.UIM.gamePlayPage.gameObject.SetActive(true);
    }
#endregion
#region ---- Forgot Password
    /**
     *
     *  forgot password
     *
     * **/
    public void OnClickResetPassword()
    {
        string email = changePasswordArea.GetChild(0).GetChild(2).GetComponent<TMP_InputField>().text;
        string[] a_count = email.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
        string[] dot_count = null;
        if (a_count.Length > 1)
            dot_count = a_count[1].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

        if (string.IsNullOrEmpty(email))
        {
            // error alert
            SetErrAlertText(gamePropertySettings.EMAIL_INPUT_EMPTY);
            errorAlert.gameObject.SetActive(true);
            return;
        }
        else
        {
            if (dot_count == null || dot_count.Length < 2)
            {
                // error alert
                SetErrAlertText(gamePropertySettings.EMAIL_INPUT_ERROR);
                errorAlert.gameObject.SetActive(true);
                return;
            }
            if (a_count.Length > 2)
            {
                // error alert
                SetErrAlertText(gamePropertySettings.EMAIL_INPUT_ERROR);
                errorAlert.gameObject.SetActive(true);
                return;
            }
        }
        userEmail = email;
        gameApi.request.ForgotPasswordApi(email);
    }
    public void OnClickBackFromResetPW()
    {
        loginArea.gameObject.SetActive(true);
        changePasswordArea.gameObject.SetActive(false);
        InitializeInputField();
        UI_Main.UIM.loginPage.GetComponent<Animation>().Play();
    }
    public void GotoVerificationPage()
    {
        changePasswordArea.GetChild(0).gameObject.SetActive(false);
        changePasswordArea.GetChild(1).gameObject.SetActive(true);
        UI_Main.UIM.loginPage.GetComponent<Animation>().Play();
    }

    /**
     *
     *  enter verification code
     *
     * **/
    public void OnClickVerificationNext()
    {
        string verificationCode = changePasswordArea.GetChild(1).GetChild(2).GetComponent<TMP_InputField>().text;
        gameApi.request.CheckVerificationCodeApi(userEmail, verificationCode);
    }
    public void OnClickBackFromVerification()
    {
        changePasswordArea.GetChild(0).gameObject.SetActive(true);
        changePasswordArea.GetChild(1).gameObject.SetActive(false);
        userEmail = string.Empty;
        InitializeInputField();
        UI_Main.UIM.loginPage.GetComponent<Animation>().Play();
    }
    public void GotoCreatePasswordPage()
    {
        changePasswordArea.GetChild(1).gameObject.SetActive(false);
        changePasswordArea.GetChild(2).gameObject.SetActive(true);
        UI_Main.UIM.loginPage.GetComponent<Animation>().Play();
    }

    /**
     *
     *   Set new password
     *
     * **/
    public void OnClickConfirmNewPw()
    {
        string newPw = changePasswordArea.GetChild(2).GetChild(2).GetComponent<TMP_InputField>().text;
        string confPw = changePasswordArea.GetChild(2).GetChild(3).GetComponent<TMP_InputField>().text;

        if (string.IsNullOrEmpty(newPw) || string.IsNullOrEmpty(confPw))
        {
            // error alert
            SetErrAlertText(gamePropertySettings.CHANGE_PASSWORD_FAILED);
            errorAlert.gameObject.SetActive(true);
            return;
        }
        if (newPw != confPw)
        {
            // error alert
            SetErrAlertText(gamePropertySettings.PASSWORD_INPUT_ERROR);
            errorAlert.gameObject.SetActive(true);
            return;
        }
        if (newPw.Length < 8)
        {
            // error alert
            SetErrAlertText(gamePropertySettings.PASSWORD_LESS_CHARACTERS);
            errorAlert.gameObject.SetActive(true);
            return;
        }
        gameApi.request.CreateNewPasswordApi(userEmail, newPw);
    }
    public void GoToLoginAfterCreateNewPassword()
    {
        loginArea.gameObject.SetActive(true);
        changePasswordArea.gameObject.SetActive(false);
        changePasswordArea.GetChild(0).gameObject.SetActive(true);
        changePasswordArea.GetChild(1).gameObject.SetActive(false);
        changePasswordArea.GetChild(2).gameObject.SetActive(false);
        userEmail = string.Empty;
        InitializeInputField();
        UI_Main.UIM.loginPage.GetComponent<Animation>().Play();
    }
    #endregion

#region ---- Error alerts
    /**
     *
     *  Error alert text
     *
     * **/
    public void SetErrAlertText(string errorContent)
    {
        errorAlert.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = errorContent;
    }
    /**
     *
     *  close error alert
     *
     * **/
    public void OnClickCloseErrAlert(Transform trans)
    {
        UI_Main.UIM.closeAlertAnimation(trans);
    }
#endregion


    #region  PhotoGallery


    #endregion
}
