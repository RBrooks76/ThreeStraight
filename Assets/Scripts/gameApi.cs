using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
//using System.Net;
//using System.Security.Cryptography;

public class gameApi : MonoBehaviour
{
    static public gameApi request;
    private string auth_status = string.Empty;
    public bool isNetworkError = false;
    public bool userLoginStatus = false;
    public string _base = String.Empty;
    public string _socketUrl = String.Empty;
    public string loginType = string.Empty;
    public string callbackResult = string.Empty;
    public string socialId = string.Empty;
    public string socialType = string.Empty;

    // Start is called before the first frame update
    void Start()
    {
        if (request == null) request = this;

        // There are 3 modes to test.
        // LOCAL_ENV, HEROKU_ENV, PROD_ENV
        var MODE = gamePropertySettings.PROD_ENV;

        if (MODE == gamePropertySettings.LOCAL_ENV)
        {
            _base = gamePropertySettings.LOCAL_SERVER_URL;
            _socketUrl = gamePropertySettings.LOCAL_SOCKET_URL;
        }
        else if (MODE == gamePropertySettings.HEORKU_ENV)
        {
            _base = gamePropertySettings.HEROKU_SERVER_URL;
            _socketUrl = gamePropertySettings.HEROKU_SOCKET_URL;
        }
        else if (MODE == gamePropertySettings.PROD_ENV)
        {
            _base = gamePropertySettings.PROD_SERVER_URL;
            _socketUrl = gamePropertySettings.PROD_SOCKET_URL;
        }
        loginType = gamePropertySettings.EMAIL_LOGIN;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setHeader(UnityWebRequest uwrq)
    {
        uwrq.SetRequestHeader("Accept", "*/*");
        uwrq.SetRequestHeader("Accept-Encoding", "gzip, deflate");
        uwrq.SetRequestHeader("User-Agent", "runscope/0.1");
    }

    /**
     *
     *  social login
     *
    **/
    public void googleLogin(string token)
    {
        string URL = _base + "auth/google-login";
        try
        {
            WWWForm form = new WWWForm();
            Debug.Log("WWWForm");
            form.AddField("token", "token_" + token);
            Debug.Log("token2: " + token);
            if (Application.platform == RuntimePlatform.Android)
            {
                form.AddField("platform", "android");
            }

            UI_Main.UIM.activityBar.gameObject.SetActive(true); // display activity until get response from server

            UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
            setHeader(uwrq);
            StartCoroutine(WaitForRequest(uwrq, "USER_LOGIN"));
            Debug.Log("completed: " + token);
        }
        catch (Exception e)
        {
            Debug.LogError("Exception: " + e);
        }
    }
    public void facebookLogin(string token)
    {
        string URL = _base + "auth/facebook-login";
        WWWForm form = new WWWForm();
        form.AddField("token", token);
        UI_Main.UIM.activityBar.gameObject.SetActive(true); // display activity until get response from server
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        StartCoroutine(WaitForRequest(uwrq, "USER_LOGIN"));
    }

    /**
     *
     *  User login and register
     *
     * */
    public void UserLoginApi(string name, string email, string password, string country, string avatarName, string flag)
    {
        string URL = _base + "auth/login";
        WWWForm form = new WWWForm();

        if (flag == "AUTOLOGIN")
        {
            form.AddField("userId", email);
            UI_Main.UIM.activityBar.gameObject.SetActive(true); // display activity until get response from server
        }
        else
        {
            if (flag == "REGISTER")
            {
                form.AddField("country", country);
                form.AddField("name", name);
                form.AddField("avatar", avatarName);
                if (!string.IsNullOrEmpty(socialId))
                {
                    form.AddField("socialId", socialId);
                    form.AddField("socialType", socialType);
                }
                else
                {
                    form.AddField("socialId", string.Empty);
                    form.AddField("socialType", "email");
                }
            }
            else
            {
                UI_Main.UIM.activityBar.gameObject.SetActive(true); // display activity until get response from server
            }
            form.AddField("email", email);
            form.AddField("pw", password);
        }
        form.AddField("flag", flag);
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        auth_status = flag;
        StartCoroutine(WaitForRequest(uwrq, "AUTH"));
    }

    /**
     *
     *  Upload avatar to server
     *
     **/
    public void UploadAvatar(string name, string email, string password, string country, int avatar_num, string flag)
    {
        StartCoroutine(UploadFile(name, email, password, country, avatar_num, flag));
    }

    /**
     *
     *  Forgot password api
     *
     **/
    public void ForgotPasswordApi(string email)
    {
        string URL = _base + "auth/forgotpw";
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        UI_Main.UIM.activityBar.gameObject.SetActive(true); // display activity until get response from server
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        StartCoroutine(WaitForRequest(uwrq, "FORGOT_PASSWORD"));
    }

    /**
     *
     *  check Verification Code to create new passsword
     *
     **/
    public void CheckVerificationCodeApi(string email, string verifitcationCode)
    {
        string URL = _base + "auth/verification";
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("code", verifitcationCode);
        UI_Main.UIM.activityBar.gameObject.SetActive(true); // display activity until get response from server
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        StartCoroutine(WaitForRequest(uwrq, "VERIFICATION"));
    }
    /**
     *
     *  Create new Password
     *
     **/
    public void CreateNewPasswordApi(string email, string newPw)
    {
        string URL = _base + "auth/createPw";
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", newPw);
        UI_Main.UIM.activityBar.gameObject.SetActive(true); // display activity until get response from server
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        StartCoroutine(WaitForRequest(uwrq, "CREATEPW"));
    }

    /**
     *
     *   Loading Ranking data from Server
     *
     **/
    public void LoadingRankingApi()
    {
        string URL = _base + "auth/ranking";
        UI_Main.UIM.activityBar.gameObject.SetActive(true); // display activity until get response from server
        UnityWebRequest uwrq = UnityWebRequest.Get(URL);
        setHeader(uwrq);
        uwrq.SetRequestHeader("Content-Type", "application/json");
        uwrq.SetRequestHeader("Authorization", UI_Main.UIM.playerToken);
        StartCoroutine(WaitForRequest(uwrq, "RANKING_DATA"));
    }

    /**
     *
     *   Send Message to Supporter
     *
     **/
    public void SendMessageToSupporter(string subject, string content)
    {
        string URL = _base + "support/send";
        WWWForm form = new WWWForm();
        form.AddField("sender", UI_Main.UIM.playerId);
        form.AddField("subject", subject);
        form.AddField("content", content);
        UI_Main.UIM.activityBar.gameObject.SetActive(true); // display activity until get response from server
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        StartCoroutine(WaitForRequest(uwrq, "MESSAGE_TO_SUPPORT"));
    }

    /**
     *
     *  Increase Victory Count.
     *
     **/
    public void UpdateGameScore(string userId)
    {
        string URL = _base + "auth/updateScore";
        UI_Main.UIM.activityBar.gameObject.SetActive(true);
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Authorization", UI_Main.UIM.playerToken);
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        WWW www = new WWW(URL, form.data, headers);

        StartCoroutine(OnRequestForAuth(www));
    }

    /**
     *
     *  Loading Game Score
     *
     **/
    public void LoadMyGameScore(string userId)
    {
        string URL = _base + "auth/loadmyscore";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        StartCoroutine(WaitForRequest(uwrq, "LOAD_MY_SCORE"));
    }

    /**
     *
     *  Create Room
     *
     **/
    public void CreateGameRoom(string userId, string map, string score, string tournamentId)
    {
        if (string.IsNullOrEmpty(tournamentId))
        {
            string URL = _base + "room/create";
            WWWForm form = new WWWForm();
            form.AddField("userId", userId);
            form.AddField("gameMap", map);
            form.AddField("score", score);
            form.AddField("betCoins", UI_Main.UIM.joinCoins);
            UI_Main.UIM.activityBar.gameObject.SetActive(true); // display activity until get response from server
            UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
            setHeader(uwrq);
            StartCoroutine(WaitForRequest(uwrq, "CREATE_ROOM"));
        }
        else
        {
            string URL = _base + "tournament/create-room";
            WWWForm form = new WWWForm();
            form.AddField("userId", userId);
            form.AddField("gameMap", map);
            form.AddField("score", score);
            form.AddField("betCoins", UI_Main.UIM.joinCoins);
            form.AddField("tournamentId", tournamentId);
            UI_Main.UIM.activityBar.gameObject.SetActive(true); // display activity until get response from server
            UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
            setHeader(uwrq);
            StartCoroutine(WaitForRequest(uwrq, "CREATE_ROOM"));
        }

    }

    /**
     *
     *  Update Room for Nothing to be Matched.
     *
     **/
    public void RemoveRoom(string roomId)
    {
        string URL = string.Empty;
        if (UI_Main.UIM.matchFromWhere == gamePropertySettings.FROM_NORMAL_ROOM)
        {
            URL = _base + "room/remove";
        }
        else if (UI_Main.UIM.matchFromWhere == gamePropertySettings.FROM_TOURNAMENT_ROOM)
        {
            URL = _base + "tournament/remove-room";
        }

        WWWForm form = new WWWForm();
        form.AddField("roomId", roomId);
        form.AddField("userId", UI_Main.UIM.playerId);
        form.AddField("betCoins", UI_Main.UIM.joinCoins);
        UI_Main.UIM.activityBar.gameObject.SetActive(true); // display activity until get response from server
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        StartCoroutine(WaitForRequest(uwrq, "REMOVE_ROOM"));
    }

    /**
     *
     *  Update Room
     *
     * **/
    public void FinishGame(string room, string winner, int score, string duration, int point, int move)
    {
        string URL = _base + "room/update";
        WWWForm form = new WWWForm();
        form.AddField("roomId", room);
        form.AddField("winner", winner);
        form.AddField("loser", UI_Main.UIM.rivalPlayer);
        form.AddField("score", score);
        form.AddField("duration", duration);
        form.AddField("point", point);
        form.AddField("move", move);
        form.AddField("earnCoins", UI_Main.UIM.joinCoins * 2);
        form.AddField("userId", UI_Main.UIM.playerId);
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        StartCoroutine(WaitForRequest(uwrq, "UPDATE_ROOM"));
    }

    /**
     *
     *  Add Coins After Ads View.
     *
     **/
    public void AddAdsCoins(string addedCoin)
    {
        string URL = _base + "auth/ads-coins";
        WWWForm form = new WWWForm();
        form.AddField("userId", UI_Main.UIM.playerId);
        form.AddField("adsCoins", addedCoin);
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        StartCoroutine(WaitForRequest(uwrq, "ADD_COIN"));
    }


    #region <========= Tournament ==========>

    // Create New Tournament
    public void CreateNewTournament(string name, string map, string startDate, string endDate, string privacy, string participants, string gameLevel)
    {
        string URL = _base + "tournament/create";
        WWWForm form = new WWWForm();
        form.AddField("userId", UI_Main.UIM.playerId);
        form.AddField("name", name);
        form.AddField("privacy", privacy);
        form.AddField("map", map);
        form.AddField("maxMembers", participants);
        form.AddField("startDate", startDate);
        form.AddField("endDate", endDate);
        form.AddField("gameLevel", gameLevel);
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        UI_Main.UIM.activityBar.gameObject.SetActive(true);
        StartCoroutine(WaitForRequest(uwrq, "CREATE_TOURNAMENT"));
    }
    public void EditTournament(string tournamentId, string name, string map, string startDate, string endDate, string privacy, string participants, string gameLevel)
    {
        string URL = _base + "tournament/edit";
        WWWForm form = new WWWForm();
        form.AddField("tournamentId", tournamentId);
        form.AddField("userId", UI_Main.UIM.playerId);
        form.AddField("name", name);
        form.AddField("privacy", privacy);
        form.AddField("map", map);
        form.AddField("maxMembers", participants);
        form.AddField("startDate", startDate);
        form.AddField("endDate", endDate);
        form.AddField("gameLevel", gameLevel);
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        UI_Main.UIM.activityBar.gameObject.SetActive(true);
        StartCoroutine(WaitForRequest(uwrq, "EDIT_TOURNAMENT"));
    }
    public void RemoveTournament(string id)
    {
        string URL = _base + "tournament/remove";
        WWWForm form = new WWWForm();
        form.AddField("userId", UI_Main.UIM.playerId);
        form.AddField("tournamentId", id);
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        UI_Main.UIM.activityBar.gameObject.SetActive(true);
        StartCoroutine(WaitForRequest(uwrq, "REMOVE_TOURNAMENT"));
    }
    public void LoadMyTournaments()
    {
        string URL = _base + "tournament/my-tournaments";
        WWWForm form = new WWWForm();
        form.AddField("userId", UI_Main.UIM.playerId);
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        UI_Main.UIM.activityBar.gameObject.SetActive(true);
        StartCoroutine(WaitForRequest(uwrq, "LOAD_MY_TOURNAMENTS"));
    }

    public void LoadAllTournaments()
    {
        string URL = _base + "tournament/all-tournaments";
        WWWForm form = new WWWForm();
        form.AddField("userId", UI_Main.UIM.playerId);
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        UI_Main.UIM.activityBar.gameObject.SetActive(true);
        StartCoroutine(WaitForRequest(uwrq, "LOAD_ALL_TOURNAMENTS"));
    }
    public void JoinTournament(string tournamentId, string code)
    {
        string URL = _base + "tournament/join";
        WWWForm form = new WWWForm();
        form.AddField("userId", UI_Main.UIM.playerId);
        form.AddField("tournamentId", tournamentId);
        form.AddField("verifyCode", code);
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        UI_Main.UIM.activityBar.gameObject.SetActive(true);
        StartCoroutine(WaitForRequest(uwrq, "JOIN_TOURNAMENT"));
    }
    public void LeaveTournament(string tournamentId)
    {
        string URL = _base + "tournament/leave";
        WWWForm form = new WWWForm();
        form.AddField("userId", UI_Main.UIM.playerId);
        form.AddField("tournamentId", tournamentId);
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        UI_Main.UIM.activityBar.gameObject.SetActive(true);
        StartCoroutine(WaitForRequest(uwrq, "LEAVE_TOURNAMENT"));
    }

    /**
     *
     *  Update Tournament Room
     *
     **/
    public void FinishTournamentRoom(string tournamentId, string room, string winner)
    {
        string URL = _base + "tournament/update-room";
        WWWForm form = new WWWForm();
        form.AddField("tournamentId", tournamentId);
        form.AddField("roomId", room);
        form.AddField("winner", winner);
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        StartCoroutine(WaitForRequest(uwrq, "UPDATE_TOURNAMENT_ROOM"));
    }

    /**
     *
     *  Loading Participants Ranking
     *
     **/
    public void LoadRankingInTournament(string tournamentId)
    {
        string URL = _base + "tournament/load-tournament-score";
        WWWForm form = new WWWForm();
        form.AddField("tournamentId", tournamentId);
        UI_Main.UIM.activityBar.gameObject.SetActive(true); // display activity until get response from server
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        StartCoroutine(WaitForRequest(uwrq, "RANKING_IN_TOURNAMENT"));
    }

    /**
     *
     *  Loading Participants Ranking
     *
    **/
    public void publishCode(string tournamentId, int inviteCode)
    {
        string URL = _base + "tournament/share-code";
        WWWForm form = new WWWForm();
        form.AddField("userId", UI_Main.UIM.playerId);
        form.AddField("tournamentId", tournamentId);
        form.AddField("code", inviteCode.ToString());
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        StartCoroutine(WaitForRequest(uwrq, "PUBLISH_CODE"));
    }

    //// Change Tournament Status
    //public void ChangeTournamentStatus(string tournamentId, string status)
    //{
    //    string URL = _base + "tournament/change-status";

    //    WWWForm form = new WWWForm();
    //    form.AddField("userId", UI_Main.UIM.playerId);
    //    form.AddField("tournamentId", tournamentId);
    //    form.AddField("status", status);
    //    UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);

    //    UI_Main.UIM.activityBar.gameObject.SetActive(true);

    //    StartCoroutine(WaitForRequest(uwrq, "CHANGE_TOURNAMENT_STATUS"));
    //}

    #endregion

    #region <========== Shop (In app purchase ============>

    int level = 0;
    public void CoinPurchase(int type)
    {
        if (string.IsNullOrEmpty(UI_Main.UIM.playerId))
            return;

        level = type;
        string URL = _base + "purchase/buy";
        WWWForm form = new WWWForm();
        form.AddField("userId", UI_Main.UIM.playerId);
        form.AddField("type", type);
        UI_Main.UIM.activityBar.gameObject.SetActive(true);
        UnityWebRequest uwrq = UnityWebRequest.Post(URL, form);
        setHeader(uwrq);
        StartCoroutine(WaitForRequest(uwrq, "PURCHASE"));
    }
    #endregion

    /**
     *
     *  Api Listener
     *
     **/
    IEnumerator OnRequestForAuth(WWW www)
    {
        yield return www;
        Debug.Log(www.text);
        UI_Main.UIM.activityBar.gameObject.SetActive(false);
        GameOver gameover = JsonUtility.FromJson<GameOver>(callbackResult);
        if (gameover.status)
        {
            if (gameover.scoreData.id == UI_Main.UIM.playerId)
            {
                UI_Main.UIM.playerScore = gameover.scoreData.score;
            }
        }
    }

    /**
     *
     *  Api Listener
     *
     * **/
    IEnumerator WaitForRequest(UnityWebRequest uwrq, string flag)
    {
        uwrq.timeout = 30;
        yield return uwrq.SendWebRequest();
        UI_Main.UIM.activityBar.gameObject.SetActive(false);
        callbackResult = uwrq.downloadHandler.text;

        if (uwrq.result == UnityWebRequest.Result.ConnectionError)
        {
            isNetworkError = true;
            UI_Main.UIM.networkErrAlert.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = gamePropertySettings.NETWORK_ERROR_TITLE;
            UI_Main.UIM.networkErrAlert.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = gamePropertySettings.NETWROK_ERROR_CONTENT;
            UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
        }
        else
        {
            isNetworkError = false;
            switch (flag)
            {
                case "USER_LOGIN":
                    Auth socialLogin = JsonUtility.FromJson<Auth>(callbackResult);
                    if (socialLogin.status)
                    {
                        UI_Main.UIM.playerId = socialLogin.result.user_id;
                        UI_Main.UIM.playerName = socialLogin.result.name;
                        UI_Main.UIM.playerEmail = socialLogin.result.email;
                        UI_Main.UIM.playerAvatar = socialLogin.result.avatar;
                        UI_Main.UIM.playerScore = socialLogin.result.score;
                        UI_Main.UIM.playerCountry = socialLogin.result.country;
                        UI_Main.UIM.playerToken = socialLogin.result.token;
                        UI_Main.UIM.playerCoins = int.Parse(socialLogin.result.coin);
                        UI_Main.UIM.playerPoint = int.Parse(socialLogin.result.point);
                        UI_Main.UIM.playerLevel = socialLogin.level;
                        socialId = socialLogin.result.socialId;
                        socialType = socialLogin.result.socialType;
                        try
                        {
                            if (string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerID")))
                            {
                                PlayerPrefs.SetString("PlayerID", socialLogin.result.user_id);
                                PlayerPrefs.SetString("PlayerName", socialLogin.result.name);
                                PlayerPrefs.SetString("PlayerEmail", socialLogin.result.email);
                                PlayerPrefs.SetString("PlayerScore", socialLogin.result.score);
                                PlayerPrefs.SetString("PlayerAvatar", socialLogin.result.avatar);
                                PlayerPrefs.SetString("PlayerCountry", socialLogin.result.country);
                                PlayerPrefs.SetString("PlayerToken", socialLogin.result.token);
                                PlayerPrefs.SetString("PlayerCoin", socialLogin.result.coin);
                                PlayerPrefs.SetString("PlayerPoint", socialLogin.result.point);
                                PlayerPrefs.SetInt("PlayerLevel", socialLogin.level);
                                PlayerPrefs.Save();
                            }
                        }
                        catch (SystemException err)
                        {
                            Debug.Log("GOT :" + err);
                        }

                        loginType = gamePropertySettings.SOCIAL_LOGIN;
                        AvatarFromServer(socialLogin.result.avatar);
                        UI_Login.UIL.loginArea.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = loginType;
                        if (socialLogin.message == gamePropertySettings.RES_SOCIAL_LOGIN)
                        {
                            UI_Login.UIL.GotoGamePage();
                        }
                        else if (socialLogin.message == gamePropertySettings.RES_SOCIAL_SIGNUP)
                        {
                            UI_Login.UIL.OnClickGoRegister();
                        }
                    }
                    break;
                case "AUTH":
                    Auth userAuthInfo = JsonUtility.FromJson<Auth>(callbackResult);
                    userLoginStatus = userAuthInfo.status;
                    if (userLoginStatus)
                    {
                        UI_Main.UIM.playerId = userAuthInfo.result.user_id;
                        UI_Main.UIM.playerName = userAuthInfo.result.name;
                        UI_Main.UIM.playerEmail = userAuthInfo.result.email;
                        UI_Main.UIM.playerAvatar = userAuthInfo.result.avatar;
                        UI_Main.UIM.playerScore = userAuthInfo.result.score;
                        UI_Main.UIM.playerCountry = userAuthInfo.result.country;
                        UI_Main.UIM.playerToken = userAuthInfo.result.token;
                        UI_Main.UIM.playerCoins = int.Parse(userAuthInfo.result.coin);
                        UI_Main.UIM.playerPoint = int.Parse(userAuthInfo.result.point);
                        UI_Main.UIM.playerLevel = userAuthInfo.level;
                        try
                        {
                            if (string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerID")))
                            {
                                PlayerPrefs.SetString("PlayerID", userAuthInfo.result.user_id);
                                PlayerPrefs.SetString("PlayerName", userAuthInfo.result.name);
                                PlayerPrefs.SetString("PlayerEmail", userAuthInfo.result.email);
                                PlayerPrefs.SetString("PlayerScore", userAuthInfo.result.score);
                                PlayerPrefs.SetString("PlayerAvatar", userAuthInfo.result.avatar);
                                PlayerPrefs.SetString("PlayerCountry", userAuthInfo.result.country);
                                PlayerPrefs.SetString("PlayerToken", userAuthInfo.result.token);
                                PlayerPrefs.SetString("PlayerCoin", userAuthInfo.result.coin);
                                PlayerPrefs.SetString("PlayerPoint", userAuthInfo.result.point);
                                PlayerPrefs.SetInt("PlayerLevel", userAuthInfo.level);
                                PlayerPrefs.Save();
                            }
                        }
                        catch (SystemException err)
                        {
                            Debug.Log("GOT :" + err);
                        }

                        loginType = gamePropertySettings.EMAIL_LOGIN;
                        AvatarFromServer(userAuthInfo.result.avatar);
                        if (auth_status == "REGISTER")
                        {
                            UI_Login.UIL.GotoHowToPage();
                        }
                        else if (auth_status == "LOGIN")
                        {
                            UI_Login.UIL.GotoGamePage();
                        }
                        else
                        {
                            if (UI_Main.UIM.isDisplayShopPage)
                            {
                                UI_Main.UIM.shopPage.gameObject.SetActive(true);
                            }
                            UI_Main.UIM.DispOnlineModes();
                        }
                    }
                    else
                    {
                        if (auth_status == "AUTOLOGIN")
                        {
                            if (userAuthInfo.message == gamePropertySettings.RES_USER_NOT_FOUND)
                            {
                                UI_Main.UIM.SetErrText(gamePropertySettings.ACCOUNT_NOT_FOUND);
                            }
                            else
                            {
                                UI_Main.UIM.SetErrText(gamePropertySettings.NETWROK_ERROR_CONTENT);
                            }
                            UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
                        }
                        else
                        {
                            if (userAuthInfo.message == gamePropertySettings.RES_USER_NOT_FOUND)
                            {
                                UI_Login.UIL.SetErrAlertText(gamePropertySettings.ACCOUNT_NOT_FOUND);
                            }
                            else if (userAuthInfo.message == gamePropertySettings.RES_PASSWORD_FAILED)
                            {
                                UI_Login.UIL.SetErrAlertText(gamePropertySettings.LOGIN_PASSWORD_FAILED);
                            }
                            else if (userAuthInfo.message == gamePropertySettings.RES_USER_EXIST)
                            {
                                UI_Login.UIL.SetErrAlertText(gamePropertySettings.USER_ALREADY_EXIST);
                            }
                            else
                            {
                                UI_Login.UIL.SetErrAlertText(gamePropertySettings.NETWROK_ERROR_CONTENT);
                            }
                            UI_Login.UIL.errorAlert.gameObject.SetActive(true);
                        }
                    }
                    break;
                case "FORGOT_PASSWORD":
                    ApiResStatus resStatus = JsonUtility.FromJson<ApiResStatus>(callbackResult);
                    if (resStatus.status)
                    {
                        UI_Login.UIL.GotoVerificationPage();
                    }
                    else
                    {
                        if (resStatus.message == gamePropertySettings.SERVER_ERR)
                        {
                            UI_Login.UIL.SetErrAlertText(gamePropertySettings.NETWROK_ERROR_CONTENT);
                        }
                        else
                        {
                            UI_Login.UIL.SetErrAlertText(resStatus.message);
                        }
                        UI_Login.UIL.errorAlert.gameObject.SetActive(true);
                    }
                    break;
                case "VERIFICATION":
                    ApiResStatus verifyStatus = JsonUtility.FromJson<ApiResStatus>(callbackResult);
                    if (verifyStatus.status)
                    {
                        UI_Login.UIL.GotoCreatePasswordPage();
                    }
                    else
                    {
                        if (verifyStatus.message == gamePropertySettings.SERVER_ERR)
                        {
                            UI_Login.UIL.SetErrAlertText(gamePropertySettings.NETWROK_ERROR_CONTENT);
                        }
                        else
                        {
                            UI_Login.UIL.SetErrAlertText(verifyStatus.message);
                        }
                        UI_Login.UIL.errorAlert.gameObject.SetActive(true);
                    }
                    break;
                case "CREATEPW":
                    ApiResStatus createPwStatus = JsonUtility.FromJson<ApiResStatus>(callbackResult);
                    if (createPwStatus.status)
                    {
                        UI_Login.UIL.GoToLoginAfterCreateNewPassword();
                    }
                    else
                    {
                        if (createPwStatus.message == gamePropertySettings.SERVER_ERR)
                        {
                            UI_Login.UIL.SetErrAlertText(gamePropertySettings.NETWROK_ERROR_CONTENT);
                        }
                        else if (createPwStatus.message == gamePropertySettings.RES_CHANGE_PASSWORD_FAILED)
                        {
                            UI_Login.UIL.SetErrAlertText(gamePropertySettings.CHANGE_PASSWORD_FAILED);
                        }
                        UI_Login.UIL.errorAlert.gameObject.SetActive(true);
                    }
                    break;
                case "UPDATE_SCORE":
                    GameOver gameover = JsonUtility.FromJson<GameOver>(callbackResult);
                    if (gameover.status)
                    {
                        if (gameover.scoreData.id == UI_Main.UIM.playerId)
                        {
                            UI_Main.UIM.playerScore = gameover.scoreData.score;
                        }
                    }
                    break;
                case "LOAD_MY_SCORE":
                    GameOver exitFromGame = JsonUtility.FromJson<GameOver>(callbackResult);
                    if (exitFromGame.status)
                    {
                        PlayerPrefs.SetString("PlayerScore", exitFromGame.scoreData.score);
                        PlayerPrefs.SetString("PlayerCoin", exitFromGame.scoreData.coin);
                        PlayerPrefs.SetString("PlayerPoint", exitFromGame.scoreData.point);
                        UI_Main.UIM.playerScore = exitFromGame.scoreData.score;
                        UI_Main.UIM.playerPoint = int.Parse(exitFromGame.scoreData.point);
                        UI_Main.UIM.playerCoins = int.Parse(exitFromGame.scoreData.coin);
                    }
                    break;
                case "RANKING_DATA":
                    Ranking rankingData = JsonUtility.FromJson<Ranking>(callbackResult);
                    if (rankingData.status)
                    {
                        string ranking = string.Empty;
                        if (rankingData.result.Count > 0)
                        {
                            for (int i = 0; i < rankingData.result.Count; i++)
                            {
                                ranking = rankingData.result[i].name + ";" + rankingData.result[i].avatar + ";" + rankingData.result[i].country + ";" + rankingData.result[i].point + ";" + rankingData.result[i].score;
                                UI_Ranking.UIR.rankingList.Add(ranking);
                                ranking = string.Empty;
                            }
                            UI_Ranking.UIR.displayRanking(UI_Ranking.UIR.rankingList);
                        }
                    }
                    break;
                case "CREATE_ROOM":
                    CreateRoomRes room = JsonUtility.FromJson<CreateRoomRes>(callbackResult);
                    if (room.status)
                    {
                        UI_Main.UIM.DisplayMatchingPage();
                        UI_Main.UIM.roomId = room.roomId;
                        socketApi.instance.JoinRoom(UI_Main.UIM.roomId, UI_Main.UIM.playerId);
                        UI_Main.UIM.playerCoins -= UI_Main.UIM.joinCoins;
                    }
                    break;
                case "REMOVE_ROOM":
                    ApiResStatus delRoomRes = JsonUtility.FromJson<ApiResStatus>(callbackResult);
                    if (delRoomRes.status)
                    {
                        if (UI_Main.UIM.matchFromWhere == gamePropertySettings.FROM_NORMAL_ROOM)
                        {
                            UI_Main.UIM.playAreaPage.gameObject.SetActive(true);
                        }
                        else if (UI_Main.UIM.matchFromWhere == gamePropertySettings.FROM_TOURNAMENT_ROOM)
                        {
                            UI_Main.UIM.leaderBoardPage.gameObject.SetActive(true);
                        }
                        UI_Main.UIM.matchingPage.gameObject.SetActive(false);
                        UI_Main.UIM.playerCoins += UI_Main.UIM.joinCoins;
                    }
                    break;
                case "UPDATE_ROOM":
                    GameOver updatedRes = JsonUtility.FromJson<GameOver>(callbackResult);
                    if (updatedRes.status)
                    {
                        if (updatedRes.scoreData.id == UI_Main.UIM.playerId)
                        {
                            PlayerPrefs.SetString("PlayerScore", updatedRes.scoreData.score);
                            PlayerPrefs.SetString("PlayerCoin", updatedRes.scoreData.coin);
                            PlayerPrefs.SetString("PlayerPoint", updatedRes.scoreData.point);
                            UI_Main.UIM.playerScore = updatedRes.scoreData.score;
                            UI_Main.UIM.playerPoint = int.Parse(updatedRes.scoreData.point);
                            UI_Main.UIM.playerCoins = int.Parse(updatedRes.scoreData.coin);
                        }
                    }
                    break;
                case "MESSAGE_TO_SUPPORT":
                    ApiResStatus supportRes = JsonUtility.FromJson<ApiResStatus>(callbackResult);
                    if (supportRes.status)
                    {
                        UI_Main.UIM.onClickBackFromContactUs();
                        UI_Main.UIM.settingAlert.gameObject.SetActive(false);
                    }
                    break;

                #region
                case "CREATE_TOURNAMENT":

                    TournamentList res = JsonUtility.FromJson<TournamentList>(callbackResult);
                    if (res.status)
                    {
                        UI_MyTournament.UIMT.ResetTournamentInfo();
                        UI_MyTournament.UIMT.ManageToCreate();
                        UI_MyTournament.UIMT.tournamentList = res.result;
                        if (res.result.Count > 0)
                        {
                            UI_MyTournament.UIMT.DisplayMyTournaments(res.result);
                        }
                    }
                    break;
                case "LOAD_MY_TOURNAMENTS":
                    TournamentList t_res = JsonUtility.FromJson<TournamentList>(callbackResult);
                    if (t_res.status)
                    {
                        UI_MyTournament.UIMT.tournamentList = t_res.result;
                        if (t_res.result.Count > 0)
                        {
                            UI_MyTournament.UIMT.DisplayMyTournaments(t_res.result);
                        }
                    }
                    break;
                case "EDIT_TOURNAMENT":
                    TournamentList e_res = JsonUtility.FromJson<TournamentList>(callbackResult);
                    if (e_res.status)
                    {
                        UI_MyTournament.UIMT.tournamentList = e_res.result;
                        UI_MyTournament.UIMT.HideEditView();
                        if (e_res.result.Count > 0)
                        {
                            UI_MyTournament.UIMT.DisplayMyTournaments(e_res.result);
                        }
                    }
                    break;
                case "LOAD_ALL_TOURNAMENTS":
                    TournamentList a_res = JsonUtility.FromJson<TournamentList>(callbackResult);
                    if (a_res.status)
                    {
                        UI_JoinTournament.UIJT.allTournamentList = a_res.result;
                        if (a_res.result.Count > 0)
                        {
                            UI_JoinTournament.UIJT.DisplayAllTournaments(a_res.result);
                        }
                    }
                    break;
                case "JOIN_TOURNAMENT":
                    TournamentList j_res = JsonUtility.FromJson<TournamentList>(callbackResult);
                    if (j_res.status)
                    {
                        UI_Main.UIM.leaderBoardPage.GetChild(0).GetComponent<TextMeshProUGUI>().text = gamePropertySettings.MY_TOURNAMENT_TITLE;
                        UI_Ranking.UIR.selectedTournament = null;
                        if (UI_JoinTournament.UIJT.transform.childCount > 1)
                        {
                            Destroy(UI_JoinTournament.UIJT.transform.GetChild(1).gameObject);
                            UI_JoinTournament.UIJT.allTournamentListView.gameObject.SetActive(true);
                        }

                        UI_JoinTournament.UIJT.allTournamentList = j_res.result;
                            UI_JoinTournament.UIJT.DisplayAllTournaments(j_res.result);
                        if (j_res.result.Count > 0)
                        {
                        }
                    }
                    else
                    {
                        UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.WRONG_VERIFYCODE);
                        UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
                    }
                    break;
                case "LEAVE_TOURNAMENT":
                    TournamentList l_res = JsonUtility.FromJson<TournamentList>(callbackResult);
                    if (l_res.status)
                    {
                        UI_Main.UIM.leaderBoardPage.GetChild(0).GetComponent<TextMeshProUGUI>().text = gamePropertySettings.MY_TOURNAMENT_TITLE;
                        UI_Ranking.UIR.selectedTournament = null;
                        if (UI_MyTournament.UIMT.transform.childCount > 3)
                        {
                            Destroy(UI_MyTournament.UIMT.transform.GetChild(3).gameObject);
                            UI_MyTournament.UIMT.myTournamentListView.gameObject.SetActive(true);
                        }
                        UI_Ranking.UIR.readyTournamentView.gameObject.SetActive(false);
                        UI_Ranking.UIR.myTournamentView.gameObject.SetActive(true);
                        UI_MyTournament.UIMT.myTournamentListView.gameObject.SetActive(true);
                        UI_MyTournament.UIMT.tournamentList = l_res.result;
                        UI_MyTournament.UIMT.DisplayMyTournaments(l_res.result);
                    }

                    break;
                case "RANKING_IN_TOURNAMENT":
                    TournamentRanking tr_res = JsonUtility.FromJson<TournamentRanking>(callbackResult);
                    if (tr_res.status)
                    {
                        string ranking = string.Empty;
                        if (tr_res.result.Count > 0)
                        {
                            UI_ReadyTournament.UIRT.CreateGameObjectsForRanking(tr_res.result);
                        }
                    }
                    break;
                #endregion

                case "PURCHASE":
                    ChargeCoin cc = JsonUtility.FromJson<ChargeCoin>(callbackResult);
                    if (cc.status)
                    {
                        UI_Main.UIM.playerCoins = cc.result;

                        if (cc.level == 1 || cc.level == 2)
                        {
                            UI_Main.UIM.playerLevel = cc.level;
                        }
                    }
                    break;
            }
        }
    }

    #region <========== Upload and Download Photo ==========>
    IEnumerator UploadFile(string name, string email, string password, string country, int avatar_num, string flag)
    {
        string URL = _base + "auth/uploadImg/";
        byte[] bytes = UI_Main.UIM.sel_img_texture.EncodeToPNG();
        WWWForm form = new WWWForm();
        if (avatar_num < 1)
        {
            form.AddBinaryData("custom_avatar", bytes);
        }
        else
        {
            form.AddBinaryData("default" + avatar_num, bytes);
        }

        UI_Main.UIM.activityBar.gameObject.SetActive(true);
        var www = UnityWebRequest.Post(URL, form);
        setHeader(www);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            UI_Main.UIM.activityBar.gameObject.SetActive(false);
            UI_Login.UIL.SetErrAlertText(gamePropertySettings.NETWROK_ERROR_CONTENT);
            UI_Login.UIL.errorAlert.gameObject.SetActive(true);
        }
        else
        {
            string result = www.downloadHandler.text;
            FileName fileName = JsonUtility.FromJson<FileName>(result);
            if (fileName.status)
            {
                UI_Main.UIM.playerAvatar = fileName.filename;
                UserLoginApi(name, email, password, country, fileName.filename, "REGISTER");
            }
        }
    }
    public void AvatarFromServer(string path)
    {
        StartCoroutine(DownloadImageFromServer(path));
    }
    IEnumerator DownloadImageFromServer(string path)
    {
        UI_Main.UIM.activityBar.gameObject.SetActive(true);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);
        setHeader(www);
        yield return www.SendWebRequest();
        UI_Main.UIM.activityBar.gameObject.SetActive(false);
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            UI_Main.UIM.sel_img_texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            if (!string.IsNullOrEmpty(socialId))
            {
                UI_Login.UIL.AssignSocialAvatar(UI_Main.UIM.sel_img_texture);
            }
        }
    }
    #endregion


    public int changeTimeToInt(string time)
    {
        string[] t_arr = time.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
        int seconds = int.Parse(t_arr[0]) * 3600 + int.Parse(t_arr[1]) * 60 + int.Parse(t_arr[2]);
        return seconds;
    }

    /**
     *
     * Serializable Classes.
     *
     * **/
    #region <========== Custom Serialization ===========>

    [Serializable]
    public class Auth
    {
        public Userinfo result;
        public bool status;
        public string message;
        public int level;
    }

    [Serializable]
    public class Userinfo
    {
        public string user_id;
        public string name;
        public string email;
        public string avatar;
        public string score;
        public string country;
        public string reg_date;
        public string token;
        public string coin;
        public string point;
        public string socialId;
        public string socialType;
    }

    [Serializable]
    public class FileName
    {
        public bool status;
        public string filename;
    }

    [Serializable]
    public class ApiResStatus
    {
        public bool status;
        public string message;
    }

    [Serializable]
    public class GameOver
    {
        public bool status;
        public ScoreInfo scoreData;
    }

    [Serializable]
    public class ScoreInfo
    {
        public string id;
        public string score;
        public string point;
        public string coin;
    }

    [Serializable]
    public class Ranking
    {
        public bool status;
        public List<Userinfo> result;
        public string message;
    }

    [Serializable]
    public class CreateRoomRes
    {
        public bool status;
        public string roomId;
        public string roomStatus;
        //public Userinfo rivalInfo;
    }

    #endregion

    #region <========== Tournament Serialization ==========>
    [Serializable]
    public class TournamentList
    {
        public bool status;
        public List<Tournament> result;
    }

    [Serializable]
    public class Tournament
    {
        public string _id;
        public string creator;
        public string name;
        public string privacy;
        public string map;
        public int maxMembers;
        public int participants;
        public double startDate;
        public double endDate;
        public int gameLevel;
        public string status;
        public int createdAt;
        public int __v;
        public bool isJoined;
    }

    [Serializable]
    public class TournamentRanking
    {
        public bool status;
        public List<TOrder> result;
    }

    [Serializable]
    public class TOrder
    {
        public string name;
        public string avatar;
        public string country;
        public int level;
        public int score;
    }
    #endregion

    #region <========== Shop ===========>

    [Serializable]
    public class ChargeCoin
    {
        public bool status;
        public int result;
        public int level;
    }
    #endregion
}
