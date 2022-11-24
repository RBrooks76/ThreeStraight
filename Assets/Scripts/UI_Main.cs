using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Mathematics;
using TMPro;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System.IO;

public class UI_Main : MonoBehaviour
{
    private RewardedAd rewardedAd;
    private string adUnityAndroidId = "ca-app-pub-4108171522870230/4363800195";
    private string adUnityIOSId = "ca-app-pub-4108171522870230/5182057845";
    private string testAdsId = "ca-app-pub-3940256099942544/5224354917";
    private string adsId = string.Empty;

    public Transform main;
    public Transform startPage;
    public Transform loginPage;
    public Transform gamePlayPage;
    public Transform gameMenuPage;
    public Transform matchingPage;
    public Transform playAreaPage;
    public Transform leaderBoardPage;
    public Transform fireworksTrans;
    public Transform activityBar;
    public Transform settingAlert;
    public Transform coinWarnningAlert;
    public Transform contactUsPage;
    public Transform networkErrAlert;
    public Transform enterShareKeyAlert;
    public Transform shopPage;

    static public UI_Main UIM;

    public AudioSource soundPlayer;
    public AudioSource musicPlayer;

    public DateTime singlePlayTime;
    public bool isMyTurn;
    public bool isBackEvent;
    public bool isChangeSetting;
    public bool isCached;
    public bool isVibration;
    public bool isDisplayShopPage;

    public string selectedTournamentId;
    public string matchFromWhere;
    public string roomId;
    public string playerToken;
    public string playerId;
    public string playerName;
    public string playerEmail;
    public string playerAvatar;
    public string playerScore;
    public int playerPoint;
    public string playerCountry;
    public int playerLevel;
    public string playerX;
    public string playMode;
    public string rivalPlayer;
    public string rivalName;
    public string rivalAvatar;
    public string rivalScore;
    public string rivalCountry;
    public string resourceUrl;

    public Texture2D sel_img_texture;
    public Texture2D rival_img_texture;

    public int diff_time;
    public int playerCoins;
    public int joinCoins;
    public int selectedMap;
    int tmp_coins;
    string tmp_score;

    // Start is called before the first frame update
    void Start()
    {
        if (UIM == null) UIM = this;

        initGoogleAds();
        InitializeAllValues();

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerID")))
        {
            playerId = PlayerPrefs.GetString("PlayerID");
            playerName = PlayerPrefs.GetString("PlayerName");
            playerEmail = PlayerPrefs.GetString("PlayerEmail");
            playerScore = PlayerPrefs.GetString("PlayerScore");
            playerCountry = PlayerPrefs.GetString("PlayerCountry");
            playerAvatar = PlayerPrefs.GetString("PlayerAvatar");
            playerToken = PlayerPrefs.GetString("PlayerToken");
            playerCoins = int.Parse(PlayerPrefs.GetString("PlayerCoin"));
            playerPoint = int.Parse(PlayerPrefs.GetString("PlayerPoint"));
            playerLevel = PlayerPrefs.GetInt("PlayerLevel");
        }
        else
        {
            playerId = string.Empty;
            playerName = string.Empty;
            playerEmail = string.Empty;
            playerScore = string.Empty;
            playerCountry = string.Empty;
            playerAvatar = string.Empty;
            playerToken = string.Empty;
            playerCoins = 0;
            playerPoint = 0;
            playerLevel = 0;
        }

        if (string.IsNullOrEmpty(PlayerPrefs.GetString("ThreeStraight_Sound")))
        {
            soundPlayer.mute = false;
        }
        else
        {
            if (PlayerPrefs.GetString("ThreeStraight_Sound") == gamePropertySettings.OPTIONS_SOUNDFX_TRUE)
            {
                soundPlayer.mute = true;
            }
            else
            {
                soundPlayer.mute = false;
            }
        }

        if (string.IsNullOrEmpty(PlayerPrefs.GetString("ThreeStraight_Music")))
        {
            musicPlayer.mute = false;
        }
        else
        {
            if (PlayerPrefs.GetString("ThreeStraight_Music") == gamePropertySettings.OPTIONS_MUSIC_TRUE)
            {
                musicPlayer.mute = true;
            }
            else
            {
                musicPlayer.mute = false;
            }
        }

        if (string.IsNullOrEmpty(PlayerPrefs.GetString("ThreeStraight_Vibration")))
        {
            isVibration = false;
        }
        else
        {
            if (PlayerPrefs.GetString("ThreeStraight_Vibration") == gamePropertySettings.OPTIONS_VIBERATION_FALSE)
            {
                isVibration = false;
            }
            else
            {
                isVibration = true;
            }
        }
    }

    private void InitializeAllValues()
    {
        isVibration = false;
        selectedTournamentId = string.Empty;
        matchFromWhere = gamePropertySettings.FROM_NORMAL_ROOM;
        roomId = string.Empty;
        playerToken = string.Empty;
        playerCoins = 0;
        joinCoins = 0;
        tmp_coins = -1;
        playerX = string.Empty;
        playMode = string.Empty; ;
        rivalPlayer = string.Empty; 
        rivalName = string.Empty; ;
        rivalAvatar = string.Empty; 
        rivalScore = string.Empty; ;
        rivalCountry = string.Empty; 
        sel_img_texture = null;
        rival_img_texture = null;
        resourceUrl = string.Empty;
        diff_time = 0;
        selectedMap = 0;
        isMyTurn = true;
        tmp_score = string.Empty;
        isBackEvent = true;
        isChangeSetting = true;
        isCached = false;
        isDisplayShopPage = false;

        Davinci.ClearAllCachedFiles();
    }

    /**
     * initialize google Ads
     **/
    private void initGoogleAds()
    {
        MobileAds.Initialize(initStatus => { });
        List<string> deviceIds = new List<string>();
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        deviceIds.Add(deviceId);
        RequestConfiguration requestConfiguration = new RequestConfiguration
            .Builder()
            .SetTestDeviceIds(deviceIds)
            .build();
        MobileAds.SetRequestConfiguration(requestConfiguration);
    }

    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        //rewardedAd = new RewardedAd(testAdsId);
        rewardedAd = new RewardedAd(adUnityAndroidId);
#elif UNITY_IPHONE
        rewardedAd = new RewardedAd(adUnityIOSId);
#endif
        // Clean up interstitial before using it

        // Called when an ad request has successfully loaded.
        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().AddKeyword("unity-admob-sample").Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
        gameMenuPage.GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>().text = rewardedAd.IsLoaded().ToString();
        rewardedAd.Show();
    }
    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded Failed");
    }
    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received" + sender);
    }
    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event Failed to show" + sender);
    }
    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdClosed event received");
    }
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        gameMenuPage.GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>().text = amount.ToString();
        gameMenuPage.GetChild(6).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (activityBar.gameObject.activeSelf)
        {
            activityBar.GetChild(0).GetComponent<Animation>().Play();
        }

        if (tmp_coins != playerCoins)
        {
            tmp_coins = playerCoins;
            gameMenuPage.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = playerCoins.ToString();
            playAreaPage.GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>().text = playerCoins.ToString();
            shopPage.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = playerCoins.ToString();
            if (playerCoins != 0)
            {
                PlayerPrefs.SetString("PlayerCoin", playerCoins.ToString());
            }
        }
    }
    public void OnClickGameStart()
    {
        if (!string.IsNullOrEmpty(playerId))
        {
            startPage.gameObject.SetActive(false);
            DispGamePlayPage();
        }
        else
        {
            startPage.gameObject.SetActive(false);
            loginPage.gameObject.SetActive(true);
        }
        //startPage.gameObject.SetActive(false);
        //loginPage.gameObject.SetActive(true);
    }

    /**
     * 
     *  Display game play page
     * 
     * **/
    public void DispGamePlayPage()
    {
        loginPage.gameObject.SetActive(false);
        gamePlayPage.gameObject.SetActive(true);
    }
    
    /**
     * 
     *  Gameplay page
     * 
     * **/
    public void OnClickPlay()
    {
        gamePlayPage.gameObject.SetActive(false);
        gameMenuPage.gameObject.SetActive(true);
    }
    public void OnClickSettings()
    {
        isChangeSetting = true;
        settingAlert.gameObject.SetActive(true);
    }
    public void OnCloseSetting()
    {
        closeAlertAnimation(settingAlert);
    }
    public void OnClickCredit()
    {
        if (!isDisplayShopPage && sel_img_texture == null)
        {
            isDisplayShopPage = true;
            gameApi.request.UserLoginApi(string.Empty, playerId, string.Empty, string.Empty, string.Empty, "AUTOLOGIN");
        }
        else
            shopPage.gameObject.SetActive(true);

    }

    /**
     * 
     *  GameMenu Page
     * 
     * **/
    #region Main_Menu_page
    public void OnClickSiglePlayer()
    {
        playMode = gamePropertySettings.PLAY_MODE_SINGLE;
        //gamelevel();
        GotoMapPage(playMode);
    }
    public void GotoOnlinePlayer()
    {
        socketApi.instance.ConnectWithSocketServer();

        if (!string.IsNullOrEmpty(playerId))
        {
            if (sel_img_texture == null)
            {
                gameApi.request.UserLoginApi(string.Empty, playerId, string.Empty, string.Empty, string.Empty, "AUTOLOGIN");
            }
            else
            {
                DispOnlineModes();
            }
        }
    }
    public void DispOnlineModes()
    {
        gameMenuPage.GetChild(0).gameObject.SetActive(false);
        gameMenuPage.GetChild(1).gameObject.SetActive(true);
    }
    public void OnClickOfflinePlayer()
    {
        playMode = gamePropertySettings.PLAY_MODE_OFFLINE;
        rivalPlayer = gamePropertySettings.PLAYER_RIVAL;
        GotoMapPage(playMode);
    }

    // Go to leaderboard page by clicking leaderboard button.
    public void OnClickLeaderBoard()
    {
        if (!string.IsNullOrEmpty(playerId))
        {
            DisplayRankingPage();
            gameApi.request.LoadingRankingApi();
        }
        
    }
    public void DisplayRankingPage()
    {
        leaderBoardPage.gameObject.SetActive(true);
        leaderBoardPage.GetChild(0).GetComponent<TextMeshProUGUI>().text = gamePropertySettings.RANKING_TITLE;
        leaderBoardPage.GetChild(1).gameObject.SetActive(true);
        leaderBoardPage.GetChild(2).gameObject.SetActive(false);
        leaderBoardPage.GetChild(3).gameObject.SetActive(false);
        gameMenuPage.gameObject.SetActive(false);
    }

    #endregion

    /**
     * 
     *  Online Player Page 
     *
     * **/
    #region Online_Play (Tournament)

    public void OnClickMultiPlayer()
    {
        playMode = gamePropertySettings.PLAY_MODE_MATCH;
        matchingPage.GetChild(3).gameObject.SetActive(false);
        GotoMapPage(playMode);
    }

    /// <summary>
    /// Load All Tournaments to Join
    /// </summary>
    public void OnClickTournament()
    {
        AllTournamentList();
        if (!string.IsNullOrEmpty(playerId))
        {
            gameApi.request.LoadAllTournaments();
        }
    }
    public void AllTournamentList()
    {
        leaderBoardPage.gameObject.SetActive(true);
        leaderBoardPage.GetChild(0).GetComponent<TextMeshProUGUI>().text = gamePropertySettings.JOIN_TOURNAMENT_TITLE;
        leaderBoardPage.GetChild(1).gameObject.SetActive(false);
        leaderBoardPage.GetChild(2).gameObject.SetActive(true);
        leaderBoardPage.GetChild(3).gameObject.SetActive(false);
        gameMenuPage.gameObject.SetActive(false);
    }
    public void OnClickMyTournament()
    {
        MyTournament();
        if (!string.IsNullOrEmpty(playerId))
        {
            gameApi.request.LoadMyTournaments();
        }
    }
    public void MyTournament()
    {
        leaderBoardPage.gameObject.SetActive(true);
        leaderBoardPage.GetChild(0).GetComponent<TextMeshProUGUI>().text = gamePropertySettings.MY_TOURNAMENT_TITLE;
        leaderBoardPage.GetChild(1).gameObject.SetActive(false);
        leaderBoardPage.GetChild(2).gameObject.SetActive(false);
        leaderBoardPage.GetChild(3).gameObject.SetActive(true);
        gameMenuPage.gameObject.SetActive(false);
    }
    public void OnClickBackFromOnline()
    {
        gameMenuPage.GetChild(1).gameObject.SetActive(false);
        gameMenuPage.GetChild(0).gameObject.SetActive(true);
    }

    #endregion


    public void OnClickBackFromMenu()
    {
        gamePlayPage.gameObject.SetActive(true);
        gameMenuPage.gameObject.SetActive(false);
    }
    private void gamelevel()
    {
        gameApi.request.LoadMyGameScore(playerId);
    }

    // go to map selection page after loading user's score from server.
    public void GotoMapPage(string mode)
    {
        gameMenuPage.gameObject.SetActive(false);
        if (mode == gamePropertySettings.PLAY_MODE_SINGLE || mode == gamePropertySettings.PLAY_MODE_OFFLINE)
        {
            for (int i = 0; i < 3; i++)
            {
                playAreaPage.GetChild(2).GetChild(i).GetChild(0).GetChild(0).gameObject.SetActive(false);
            }
        }
        else if (mode == gamePropertySettings.PLAY_MODE_MATCH)
        {
            playAreaPage.GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
            if (int.Parse(playerScore) > gamePropertySettings.ROOM_ENTERABLE_SCORE)
            {
                playAreaPage.GetChild(2).GetChild(1).GetChild(0).GetChild(0).gameObject.SetActive(true);
                playAreaPage.GetChild(2).GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(true);
            } 
            else
            {
                playAreaPage.GetChild(2).GetChild(1).GetChild(0).GetChild(0).gameObject.SetActive(false);
                playAreaPage.GetChild(2).GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(false);
            }
        }

        if (int.Parse(playerScore) > gamePropertySettings.ROOM_ENTERABLE_SCORE)
        {
            playAreaPage.GetChild(2).GetChild(1).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/02");
            playAreaPage.GetChild(2).GetChild(2).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/unlock_fireland");            
        }
        else
        {
            playAreaPage.GetChild(2).GetChild(1).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/map_iceland");
            playAreaPage.GetChild(2).GetChild(2).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/map_fireland");
        }
        playAreaPage.gameObject.SetActive(true);
    }

    /**
     * 
     *  Join game 
     * 
     * **/
    public void OnClickJoinGame(int gameMode)
    {
        selectedMap = gameMode;
        if (playMode == gamePropertySettings.PLAY_MODE_SINGLE || playMode == gamePropertySettings.PLAY_MODE_OFFLINE)
        {
            playerX = gamePropertySettings.PLAYER_SELF;
            if (int.Parse(playerScore) > gamePropertySettings.ROOM_ENTERABLE_SCORE)
            {
                GotoSelectedGameGround(gameMode);
            }
            else
            {
                if (gameMode == 1)
                {
                    GotoSelectedGameGround(gameMode);
                }
            }
        }
        else if (playMode == gamePropertySettings.PLAY_MODE_MATCH)
        {
            if (int.Parse(playerScore) > gamePropertySettings.ROOM_ENTERABLE_SCORE)
            {
                checkCoinCounts(gameMode);
            }
            else
            {
                if (gameMode == 1)
                {
                    checkCoinCounts(gameMode);
                }
            }
        }
    }

    // display coins to join.
    void checkCoinCounts(int gameMode)
    {
        if (gameMode == 1)
        {
            if (playerCoins >= gamePropertySettings.JOIN_ISLAND_COINS)
            {
                joinCoins = gamePropertySettings.JOIN_ISLAND_COINS;
                GotoMatchingPage(1);
            } 
            else
            {
                playAreaPage.GetChild(5).gameObject.SetActive(true);
            }
        }
        else if (gameMode == 2)
        {
            if (playerCoins >= gamePropertySettings.JOIN_ICELAND_COINS)
            {
                joinCoins = gamePropertySettings.JOIN_ICELAND_COINS;
                GotoMatchingPage(2);
            }
            else
            {
                playAreaPage.GetChild(5).gameObject.SetActive(true);
            }
        }
        else if (gameMode == 3)
        {
            if (playerCoins >= gamePropertySettings.JOIN_FIRELAND_COINS)
            {
                joinCoins = gamePropertySettings.JOIN_FIRELAND_COINS;
                GotoMatchingPage(3);
            }
            else
            {
                playAreaPage.GetChild(5).gameObject.SetActive(true);
            }
        }
    }

    /**
     * 
     *  Display Ads
     * 
     * **/
    public void OnClickViewAds()
    {
         RequestInterstitial();
    }
    public void OnClickConfirmAdsGift()
    {
        closeAlertAnimation(gameMenuPage.GetChild(6));
        if (!string.IsNullOrEmpty(playerId))
        {
            playerCoins += gamePropertySettings.AWARDED_COINS;
            gameApi.request.AddAdsCoins(gamePropertySettings.AWARDED_COINS.ToString());
        }
    }
    public void OnClickBackFromPlayLevel()
    {
        gameMenuPage.gameObject.SetActive(true);
        playAreaPage.gameObject.SetActive(false);
    }

    /**
     * 
     *  Matching user
     * 
     * **/
    public void OnClickBackFromMatching(int fromWhere)
    {
        if (matchingPage.GetChild(3).gameObject.activeSelf)
        {
            matchingPage.GetChild(3).gameObject.SetActive(false);
        }
        if (fromWhere == 1)
        {
            if (socketApi.instance.socket != null)
            {
                socketApi.instance.NoMatchedAndLeaveRoom(fromWhere);
            }
        }
        else
        {
            gameApi.request.RemoveRoom(UI_Main.UIM.roomId);
        }

    }
    void GotoMatchingPage(int gameMode)
    {
        string map = string.Empty;
        if (gameMode == 1)
        {
            map = gamePropertySettings.MAP_ISLAND;
        }
        else if (gameMode == 3)
        {
            map = gamePropertySettings.MAP_FIRELAND;
        }
        else if (gameMode == 2)
        {
            map = gamePropertySettings.MAP_ICELAND;
        }
        if (!string.IsNullOrEmpty(playerId))
        {
            matchFromWhere = gamePropertySettings.FROM_NORMAL_ROOM;
            gameApi.request.CreateGameRoom(playerId, map, playerScore, string.Empty);
        }
        else
        {
            networkErrAlert.gameObject.SetActive(true);
        }        
    }
    public void DisplayMatchingPage()
    {
        gameMenuPage.gameObject.SetActive(false);
        playAreaPage.gameObject.SetActive(false);
        leaderBoardPage.gameObject.SetActive(false);
        matchingPage.gameObject.SetActive(true);
        socketApi.instance.matchResult = false;
        matchingPage.GetChild(2).gameObject.SetActive(true);
        StartCoroutine(DisplayMatchingAnimation(0));
    }
    IEnumerator DisplayMatchingAnimation(int imgCount)
    {
        while(!socketApi.instance.matchResult)
        {
            if (matchingPage.GetChild(2).gameObject.activeSelf)
            {
                yield return new WaitForSeconds(0.05f);
                if (imgCount > 14)
                {
                    imgCount = 0;
                }
                imgCount++;
                matchingPage.GetChild(2).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("matchingAnimation/matching" + imgCount);
            }
            else
            {
                yield break;
            }
        }
        yield return null;        
    }
    void CreatePNG(byte[] b_array)
    {
        rival_img_texture = new Texture2D(1, 1);
        rival_img_texture.LoadImage(b_array);
    }
    public void GotoSelectedGameGround(int gameMode)
    {
        main.GetChild(gameMode).gameObject.SetActive(true);
        int childCount = main.GetChild(0).childCount;

        for (int i = 0; i < childCount; i++)
        {
            main.GetChild(0).GetChild(i).gameObject.SetActive(false);
        }
        main.GetChild(0).GetChild(childCount - 4).gameObject.SetActive(true);
        singlePlayTime = DateTime.Now;
        Transform parent = main.GetChild(gameMode);
        SetGameObjectParent(parent);
    }
    public void SetGameObjectParent(Transform trans)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>(gamePropertySettings._GameGroundUrl));
        go.transform.SetParent(trans);
        go.transform.localScale = Vector2.one;
        go.transform.localPosition = Vector2.zero;
        go.GetComponent<RectTransform>().sizeDelta = trans.GetComponent<RectTransform>().sizeDelta;
    }

    /**
     * 
     *  Display player avatar and rival avatar, name, score.
     *  @params {string} who : user or rival.
     * 
     * **/
    public void SetProfileImage(string who)
    {
        Transform PlayerVS = matchingPage.GetChild(0).GetChild(1);
        PlayerVS.GetChild(0).gameObject.SetActive(true);
        PlayerVS.GetChild(1).gameObject.SetActive(true);
        if (who == playerId)
        {
            PlayerVS.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = sel_img_texture;
            PlayerVS.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = playerName;
            PlayerVS.GetChild(0).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = playerScore;

            PlayerVS.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = rival_img_texture;
            PlayerVS.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = rivalName;
            PlayerVS.GetChild(1).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = rivalScore;
        }
        else
        {
            PlayerVS.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = sel_img_texture;
            PlayerVS.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = playerName;
            PlayerVS.GetChild(1).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = playerScore;

            PlayerVS.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = rival_img_texture;
            PlayerVS.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = rivalName;
            PlayerVS.GetChild(0).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = rivalScore;
        }
        PlayerVS.GetChild(2).gameObject.SetActive(true);
    }

    /**
     * 
     *  Loading audio info from external files
     * 
     * **/
    public void PlayMusic(string name)
    {
        if (musicPlayer.clip == null || musicPlayer.clip.name != name)
        {
            musicPlayer.clip = Resources.Load("audio/" + name, typeof(AudioClip)) as AudioClip;
            musicPlayer.Stop();
            musicPlayer.loop = true;
            musicPlayer.Play();
        }
        else
        {
            musicPlayer.loop = true;
            musicPlayer.Play();
        }
    }

    /**
     * 
     *  Loading audio info from external files
     * 
     * **/
    public void PlaySound(string name)
    {
        if (soundPlayer.clip == null || soundPlayer.clip.name != name)
        {
            soundPlayer.clip = Resources.Load("audio/" + name, typeof(AudioClip)) as AudioClip;
            soundPlayer.Stop();
            soundPlayer.loop = false;
            soundPlayer.Play();
        }
        else
        {
            soundPlayer.loop = false;
            soundPlayer.Play();
        }

    }
    public void SetErrText(string errorContent)
    {
        SetErrAlertText(gamePropertySettings.NETWORK_ERROR_TITLE, errorContent);
    }
    public void OnCloseNetworkErrorAlert()
    {
        closeAlertAnimation(networkErrAlert);
    }
    public void OnCloseCoinWarnningAlert()
    {
        closeAlertAnimation(coinWarnningAlert);
    }
    public void closeAlertAnimation(Transform trans)
    {
        trans.GetChild(0).GetComponent<Animation>().Play("closeAlert");
        StartCoroutine(closeAlert(trans));
    }
    IEnumerator closeAlert(Transform trans)
    {
        if (trans.gameObject.activeSelf)
        {
            while (trans.GetChild(0).GetComponent<Animation>().isPlaying)
            {
                yield return null;
            }
            trans.gameObject.SetActive(false);
        }
        yield break;
    }

    /**
     * 
     *  Contact Us
     * 
     * **/

    public void onClickBackFromContactUs()
    {
        CleanInputField();
        contactUsPage.gameObject.SetActive(false);
    }
    public void onClickSendMessageToSupporter()
    {
        string subject = contactUsPage.GetChild(1).GetComponent<TMP_InputField>().text;
        string content = contactUsPage.GetChild(2).GetComponent<TMP_InputField>().text;

        if (string.IsNullOrEmpty(subject))
        {
            SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.CONTACT_SUBJECT_ERROR);
            networkErrAlert.gameObject.SetActive(true);
            return;
        }
        if (string.IsNullOrEmpty(content))
        {
            SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.CONTACT_CONTENT_ERROR);
            networkErrAlert.gameObject.SetActive(true);
            return;
        }

        if (!string.IsNullOrEmpty(playerId))
        {
            gameApi.request.SendMessageToSupporter(subject, content);
        }
    }
    public void CleanInputField()
    {
        contactUsPage.GetChild(1).GetComponent<TMP_InputField>().text = string.Empty;
        contactUsPage.GetChild(2).GetComponent<TMP_InputField>().text = string.Empty;
    }

    /**
     * 
     *  Error alert text
     *  
     * **/
    public void SetErrAlertText(string errorTitle, string errorContent)
    {
        networkErrAlert.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = errorTitle;
        networkErrAlert.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = errorContent;
    }

    #region Alert Operation where enters key to be shared
    /// <summary>
    /// Finish enterShareKeyAlert
    /// </summary>
    public void OnClickShareKeyConfirmBtn()
    {
        string keytext = enterShareKeyAlert.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_InputField>().text;

        if (string.IsNullOrEmpty(keytext))
        {
            return;
        }
    }

    /// <summary>
    /// Close enterShareKeyAlert
    /// </summary>
    public void OnCloseShareKeyAlert()
    {
        enterShareKeyAlert.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_InputField>().text = string.Empty;
        closeAlertAnimation(enterShareKeyAlert);
    }
    #endregion
}
