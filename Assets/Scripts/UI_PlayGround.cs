using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_PlayGround : MonoBehaviour
{
    static public UI_PlayGround UIP;

    public Transform PlayObjectTrans;
    public Transform scoresTrans;
    public Transform profileArea;
    public Transform playGroundTrans;
    public Transform menuAlertTrans;
    public Transform finishAlertTrans;
    public Transform optionAlertTras;
    public Transform victory_1_AlertTrans;
    public Transform victory_2_AlertTrans;
    public Transform scoreAreaTrans;

    public GameObject soundFxBtn;
    public GameObject musicBtn;
    public GameObject viberationBtn;

    public bool isClickViberation = true;
    public bool isGameFinished = false;
    public bool isStartGame = false;
    public bool stop_time_status = false;
    public bool isAiTurn = false;

    public tileController _tile = null;
    public Vector3 tilePos = Vector3.zero;

    public int myProfilePos = 0;
    public int tileCount = 0;
    public int player_tileCount = 0;
    public int playerRival_tileCount = 0;
    public int time = 0;
    public int move = 0;
    public int ai_total_move = 0;
    public int currMiliseconds = 0;

    public string winPlayer = string.Empty;

    int tmp_move = 0;
    int tmp_total_move = 0;

    Transform victoryScoreArea;
    DateTime currTime;
    TimeSpan diffTime;
    string timeFormat = string.Empty;

    // Start is called before the first frame update
    void Start()
    {
        if (UIP == null) UIP = this;
        victoryScoreArea = victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(1).GetChild(0);

        StartCoroutine(countTime(DateTime.Now - DateTime.Now));
        AssignPlayersInfo();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStartGame)
        {
            isStartGame = !isStartGame;
            stop_time_status = true;
            if (winPlayer == gamePropertySettings.PLAYER_RIVAL)
            {
                if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_MATCH)
                {
                    //print("display score----");
                }
                DisplayVictoryAlert(false);
                UI_Main.UIM.musicPlayer.loop = false;
                UI_Main.UIM.musicPlayer.Stop();
            }
            else
            {
                DisplayVictoryAlert(true);
                UI_Main.UIM.musicPlayer.loop = false;
                UI_Main.UIM.musicPlayer.Stop();
            }
            menuAlertTrans.gameObject.SetActive(false);
            optionAlertTras.gameObject.SetActive(false);
        }

        if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_MATCH)
        {
            if (tmp_total_move != socketApi.instance.total_move)
            {
                tmp_total_move = socketApi.instance.total_move;
                scoreAreaTrans.GetChild(2).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = socketApi.instance.total_move.ToString();

                if (UI_Main.UIM.playerX == UI_Main.UIM.playerId)
                {
                    if (myProfilePos == 1)
                    {
                        profileArea.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = move.ToString();
                    }
                    else
                    {
                        profileArea.GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = move.ToString();
                    }
                }
                else if (UI_Main.UIM.playerX == UI_Main.UIM.rivalPlayer)
                {
                    if (myProfilePos == 1)
                    {
                        profileArea.GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = (socketApi.instance.total_move - move).ToString();
                    }
                    else
                    {
                        profileArea.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = (socketApi.instance.total_move - move).ToString();
                    }
                }
            }
        }
        else if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_SINGLE)
        {
            if (tmp_total_move != ai_total_move)
            {
                profileArea.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = move.ToString();
                profileArea.GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = (ai_total_move - move).ToString();
                tmp_total_move = ai_total_move;
                scoreAreaTrans.GetChild(2).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = ai_total_move.ToString();
            }
        }
        else if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_OFFLINE)
        {
            if (tmp_total_move != ai_total_move)
            {
                profileArea.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = move.ToString();
                profileArea.GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = (ai_total_move - move).ToString();
                tmp_total_move = ai_total_move;
                scoreAreaTrans.GetChild(2).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = ai_total_move.ToString();
            }
        }

        if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_MATCH)
        {
            // display time limitation in playground.
            if (UI_Main.UIM.playerX == UI_Main.UIM.playerId)
            {
                if (myProfilePos == 1)
                {
                    if (profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.activeSelf)
                    {
                        profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.SetActive(false);
                    }
                    if (!profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.activeSelf)
                    {
                        profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(true);
                    }
                    if (profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.activeSelf)
                    {
                        if (currMiliseconds < 30 && !stop_time_status)
                        {
                            currTime = DateTime.Now;
                            diffTime = currTime - socketApi.instance.turnLimitTime;
                            timeFormat = diffTime.Hours.ToString() + ":" + diffTime.Minutes.ToString() + ":" + diffTime.Seconds.ToString();
                            currMiliseconds = gameApi.request.changeTimeToInt(timeFormat);
                            if (currMiliseconds > 29)
                            {
                                currMiliseconds = 30;
                            }
                            profileArea.GetChild(0).GetChild(0).GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = (30 - currMiliseconds).ToString() + "  sec";
                        }
                    }
                }
                else
                {
                    if (profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.activeSelf)
                    {
                        profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);
                    }
                    if (!profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.activeSelf)
                    {
                        profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.SetActive(true);
                    }
                    if (profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.activeSelf)
                    {
                        if (currMiliseconds < 30 && !stop_time_status)
                        {
                            currTime = DateTime.Now;
                            diffTime = currTime - socketApi.instance.turnLimitTime;
                            timeFormat = diffTime.Hours.ToString() + ":" + diffTime.Minutes.ToString() + ":" + diffTime.Seconds.ToString();
                            currMiliseconds = gameApi.request.changeTimeToInt(timeFormat);
                            if (currMiliseconds > 29)
                            {
                                currMiliseconds = 30;
                            }
                            profileArea.GetChild(1).GetChild(0).GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = (30 - currMiliseconds).ToString() + "  sec";
                        }
                    }
                }
            }
            else if (UI_Main.UIM.playerX == UI_Main.UIM.rivalPlayer)
            {
                if (myProfilePos == 1)
                {
                    if (profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.activeSelf)
                    {
                        profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);
                    }
                    if (!profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.activeSelf)
                    {
                        profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.SetActive(true);
                    }
                    if (profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.activeSelf)
                    {
                        if (currMiliseconds < 30 && !stop_time_status)
                        {
                            currTime = DateTime.Now;
                            diffTime = currTime - socketApi.instance.turnLimitTime;
                            timeFormat = diffTime.Hours.ToString() + ":" + diffTime.Minutes.ToString() + ":" + diffTime.Seconds.ToString();
                            currMiliseconds = gameApi.request.changeTimeToInt(timeFormat);
                            if (currMiliseconds > 29)
                            {
                                currMiliseconds = 30;
                            }
                            profileArea.GetChild(1).GetChild(0).GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = (30 - currMiliseconds).ToString() + "  sec";
                        }
                    }
                }
                else
                {
                    if (profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.activeSelf)
                    {
                        profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.SetActive(false);
                    }
                    if (!profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.activeSelf)
                    {
                        profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(true);
                    }
                    if (profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.activeSelf)
                    {
                        if (currMiliseconds < 30 && !stop_time_status)
                        {
                            currTime = DateTime.Now;
                            diffTime = currTime - socketApi.instance.turnLimitTime;
                            timeFormat = diffTime.Hours.ToString() + ":" + diffTime.Minutes.ToString() + ":" + diffTime.Seconds.ToString();
                            currMiliseconds = gameApi.request.changeTimeToInt(timeFormat);
                            if (currMiliseconds > 29)
                            {
                                currMiliseconds = 30;
                            }
                            profileArea.GetChild(0).GetChild(0).GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = (30 - currMiliseconds).ToString() + "  sec";
                        }
                    }
                }
            }
        }
        else if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_SINGLE)
        {
            // display time limitation in playground.
            if (UI_Main.UIM.playerX == gamePropertySettings.PLAYER_SELF)
            {
                if (profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.activeSelf)
                {
                    pausedPeriod = DateTime.Now - DateTime.Now;
                    profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.SetActive(false);
                }
                if (!profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.activeSelf)
                {
                    profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(true);
                }
                if (profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.activeSelf)
                {
                    if (currMiliseconds < 30 && !stop_time_status)
                    {
                        currTime = DateTime.Now;
                        diffTime = currTime - socketApi.instance.turnLimitTime - pausedPeriod;
                        timeFormat = diffTime.Hours.ToString() + ":" + diffTime.Minutes.ToString() + ":" + diffTime.Seconds.ToString();
                        currMiliseconds = gameApi.request.changeTimeToInt(timeFormat);
                        profileArea.GetChild(0).GetChild(0).GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = (30 - currMiliseconds).ToString() + "  sec";
                    }
                    else if (currMiliseconds > 29)
                    {
                        if (UI_Main.UIM.playerX == gamePropertySettings.PLAYER_SELF)
                        {
                            if (!isAiTurn)
                            {
                                pausedPeriod = DateTime.Now - DateTime.Now;
                                isAiTurn = true;
                                gameMode.GM.AICreateTile();
                            }
                        }
                    }
                }
            }
            else if (UI_Main.UIM.playerX == gamePropertySettings.PLAYER_RIVAL)
            {
                if (profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.activeSelf)
                {
                    pausedPeriod = DateTime.Now - DateTime.Now;
                    profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);
                }
                if (!profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.activeSelf)
                {
                    profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.SetActive(true);
                }
                if (profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.activeSelf)
                {
                    if (currMiliseconds < 30 && !stop_time_status)
                    {
                        currTime = DateTime.Now;
                        diffTime = currTime - socketApi.instance.turnLimitTime;
                        timeFormat = diffTime.Hours.ToString() + ":" + diffTime.Minutes.ToString() + ":" + diffTime.Seconds.ToString();
                        currMiliseconds = gameApi.request.changeTimeToInt(timeFormat);
                        profileArea.GetChild(1).GetChild(0).GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = (30 - currMiliseconds).ToString() + "  sec";
                    }
                }
            }
        }
        else if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_OFFLINE)
        {
            // display time limitation in playground.
            if (UI_Main.UIM.playerX == gamePropertySettings.PLAYER_SELF)
            {
                if (profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.activeSelf)
                {
                    pausedPeriod = DateTime.Now - DateTime.Now;
                    profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.SetActive(false);
                }
                if (!profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.activeSelf)
                {
                    profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(true);
                }
                if (profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.activeSelf)
                {
                    if (currMiliseconds < 30 && !stop_time_status)
                    {
                        currTime = DateTime.Now;
                        diffTime = currTime - socketApi.instance.turnLimitTime - pausedPeriod;
                        timeFormat = diffTime.Hours.ToString() + ":" + diffTime.Minutes.ToString() + ":" + diffTime.Seconds.ToString();
                        currMiliseconds = gameApi.request.changeTimeToInt(timeFormat);
                        profileArea.GetChild(0).GetChild(0).GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = (30 - currMiliseconds).ToString() + "  sec";
                    }
                    else if (currMiliseconds > 29)
                    {
                        socketApi.instance.turnLimitTime = DateTime.Now;
                        currMiliseconds = 0;
                        pausedPeriod = DateTime.Now - DateTime.Now;
                        gameMode.GM.TurnPlayer(UI_Main.UIM.playerX);
                    }
                }
            }
            else if (UI_Main.UIM.playerX == gamePropertySettings.PLAYER_RIVAL)
            {
                if (profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.activeSelf)
                {
                    pausedPeriod = DateTime.Now - DateTime.Now;
                    profileArea.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);
                }
                if (!profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.activeSelf)
                {
                    profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.SetActive(true);
                }
                if (profileArea.GetChild(1).GetChild(0).GetChild(2).gameObject.activeSelf)
                {
                    if (currMiliseconds < 30 && !stop_time_status)
                    {
                        currTime = DateTime.Now;
                        diffTime = currTime - socketApi.instance.turnLimitTime;
                        timeFormat = diffTime.Hours.ToString() + ":" + diffTime.Minutes.ToString() + ":" + diffTime.Seconds.ToString();
                        currMiliseconds = gameApi.request.changeTimeToInt(timeFormat);
                        profileArea.GetChild(1).GetChild(0).GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = (30 - currMiliseconds).ToString() + "  sec";
                    }
                    else if (currMiliseconds > 29)
                    {
                        socketApi.instance.turnLimitTime = DateTime.Now;
                        currMiliseconds = 0;
                        pausedPeriod = DateTime.Now - DateTime.Now;
                        gameMode.GM.TurnPlayer(UI_Main.UIM.playerX);
                    }
                }
            }
        }

        if (currMiliseconds > 29)
        {
            if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_MATCH)
            {
                currMiliseconds = 0;
                socketApi.instance.turnLimitTime = DateTime.Now;
                socketApi.instance.PlayerTurn(UI_Main.UIM.roomId, UI_Main.UIM.playerX, -1, -1, 0);
            }
        }
    }

    TimeSpan pausedPeriod;
    TimeSpan totalPausedPeriod;
    DateTime pausedTime;

    public void OnClickPauseGame()
    {
        if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_SINGLE || UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_OFFLINE)
        {
            stop_time_status = true;
            pausedTime = DateTime.Now;
        }
        isGameFinished = true;
        menuAlertTrans.gameObject.SetActive(true);
        menuAlertTrans.GetChild(0).GetComponent<Animation>().Play("alert");
    }
    public void OnClickCancelPauseGame(int where)
    {
        if (UI_Main.UIM.playMode != gamePropertySettings.PLAY_MODE_MATCH)
        {
            stop_time_status = false;
            pausedPeriod += (DateTime.Now - pausedTime);
            totalPausedPeriod += (DateTime.Now - pausedTime);

            StartCoroutine(countTime(totalPausedPeriod));
        }
        isGameFinished = false;
        if (where == 0)
        {
            if (optionAlertTras.gameObject.activeSelf)
            {
                optionAlertTras.gameObject.SetActive(false);
            }
            UI_Main.UIM.closeAlertAnimation(menuAlertTrans);
        }
        else
        {
            UI_Main.UIM.closeAlertAnimation(optionAlertTras);
            StartCoroutine(SetAnchoredPos(optionAlertTras));
        }
    }

    IEnumerator SetAnchoredPos(Transform trans)
    {
        yield return new WaitForSeconds(0.5f);
        trans.GetChild(0).GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        trans.GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
    }

    /**
     *
     *  Events on menuAlert (Exit Game, Achievement, Option)
     *
     * **/
    public void OnClickExit()
    {
        if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_MATCH)
        {
            socketApi.instance.EndGame(UI_Main.UIM.roomId, UI_Main.UIM.rivalPlayer);
        }
        else //if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_SINGLE || UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_OFFLINE)
        {
            GotoStartPageAfterExit(string.Empty);
        }
    }

    public void GotoStartPageAfterExit(string winner)
    {
        if (string.IsNullOrEmpty(winner))
        {
            InitializeValues();
        }
        else
        {
            if (!victory_2_AlertTrans.gameObject.activeSelf)
            {
                if (winner == UI_Main.UIM.playerId)
                {
                    if (UI_Main.UIM.matchFromWhere == gamePropertySettings.FROM_TOURNAMENT_ROOM)
                    {
                        gameApi.request.FinishTournamentRoom(UI_Main.UIM.selectedTournamentId, UI_Main.UIM.roomId, winner);
                    }
                    else
                    {
                        gameApi.request.FinishGame(UI_Main.UIM.roomId, winner, 0, totalTime, 0, tmp_total_move);
                    }
                    DisplayVictoryAlert(true);
                }
                else
                {
                    gameApi.request.LoadMyGameScore(UI_Main.UIM.playerId);
                    DisplayVictoryAlert(false);
                }
                stop_time_status = true;
                UI_Main.UIM.roomId = string.Empty;
                UI_Main.UIM.rivalPlayer = string.Empty;
            }
        }
    }

    public void OnClickAchevement()
    {

    }
    public void OnClickOption()
    {
        menuAlertTrans.gameObject.SetActive(false);
        optionAlertTras.gameObject.SetActive(true);
        optionAlertTras.GetChild(0).GetComponent<Animation>().Play("pageNavAnimation");
    }

    /**
     *
     *  Events on optionAlert
     *
     * **/
    public void OnClickBack()
    {
        optionAlertTras.gameObject.SetActive(false);
        menuAlertTrans.gameObject.SetActive(true);
        //menuAlertTrans.GetChild(0).GetComponent<Animation>().playAutomatically = false;
        menuAlertTrans.GetChild(0).GetComponent<Animation>().Play("pageNavAnimation");
    }

    public void OnClickViberation()
    {
        if (isClickViberation)
        {
            viberationBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/switch_off");
        }
        else
        {
            viberationBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/switch_on");
        }
        isClickViberation = !isClickViberation;
    }

    /**
     *
     *  Single Victory Alert
     *
     * **/
    public void OnClickBackFromSingleVictory()
    {
        CloseVicAlert();
    }

    public void CloseVicAlert()
    {
        UI_Main.UIM.closeAlertAnimation(victory_1_AlertTrans);
        UI_Main.UIM.closeAlertAnimation(victory_2_AlertTrans);
        StartCoroutine(DelayInitialize());
    }

    IEnumerator DelayInitialize()
    {
        yield return new WaitForSeconds(0.2f);
        InitializeValues();
    }

    /**
     *
     *  Calculate playing time.
     *
     * **/
    DateTime cur_time;
    TimeSpan totalDiff;
    string totalTime = "00:00:00";
    IEnumerator countTime(TimeSpan paused)
    {
        while (!stop_time_status)
        {
            yield return new WaitForSeconds(0.2f);
            cur_time = DateTime.Now;

            if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_MATCH)
            {
                totalDiff = cur_time - socketApi.instance.startTime;
            }
            else
            {
                totalDiff = cur_time - UI_Main.UIM.singlePlayTime;
            }

            totalDiff -= paused;
            totalTime = changeTimeFormat(totalDiff);
            scoreAreaTrans.GetChild(1).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = totalTime;
        }
    }

    /**
     *
     *  Change item format with seconds.
     *  @param {int} c_time (seconds)
     *
     * **/
    public string changeTimeFormat(TimeSpan c_time)
    {
        int _hours = 0;
        int _minutes = 0;
        int _seconds = 0;

        _hours = c_time.Hours;
        _minutes = c_time.Minutes;
        _seconds = c_time.Seconds;
        string display_time = string.Empty;
        string _hours_string = string.Empty;
        string _minutes_string = string.Empty;
        string _seconds_string = string.Empty;
        if (_hours < 10)
            _hours_string = "0" + _hours.ToString();
        else
            _hours_string = _hours.ToString();

        if (_minutes < 10)
            _minutes_string = "0" + _minutes.ToString();
        else
            _minutes_string = _minutes.ToString();

        if (_seconds < 10)
            _seconds_string = "0" + _seconds.ToString();
        else
            _seconds_string = _seconds.ToString();

        display_time = _hours_string + ":" + _minutes_string + ":" + _seconds_string;
        return display_time;
    }

    private void InitializeValues()
    {
        move = 0;
        time = 0;
        tileCount = 0;
        player_tileCount = 0;
        ai_total_move = 0;
        tmp_total_move = 0;
        tmp_move = 0;

        playerRival_tileCount = 0;
        totalTime = "00:00:00";

        socketApi.instance.total_move = 0;
        socketApi.instance.turnLimitTime = DateTime.Now;

        UI_Main.UIM.joinCoins = 0;
        UI_Main.UIM.singlePlayTime = DateTime.Now;
        UI_Main.UIM.playerX = gamePropertySettings.PLAYER_SELF;
        UI_Main.UIM.matchFromWhere = gamePropertySettings.FROM_NORMAL_ROOM;

        if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_MATCH)
        {
            Transform PlayerVS = UI_Main.UIM.matchingPage.GetChild(0).GetChild(1);
            UI_Main.UIM.roomId = string.Empty;
            UI_Main.UIM.rivalAvatar = string.Empty;
            UI_Main.UIM.rivalCountry = string.Empty;
            UI_Main.UIM.rivalPlayer = string.Empty;
            UI_Main.UIM.rivalScore = "0";
            UI_Main.UIM.rival_img_texture = null;
            profileArea.GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "0";

            PlayerVS.GetChild(1).gameObject.SetActive(false);
        }
        isGameFinished = false;

        UI_Main.UIM.musicPlayer.loop = false;
        UI_Main.UIM.musicPlayer.Stop();

        StartCoroutine(DestoryExistTiles());
    }

    IEnumerator DestoryExistTiles()
    {
        if (UI_Main.UIM.fireworksTrans.gameObject.activeSelf)
        {
            UI_Main.UIM.fireworksTrans.gameObject.SetActive(false);
        }

        if (menuAlertTrans.gameObject.activeSelf)
        {
            while (menuAlertTrans.GetChild(0).GetComponent<Animation>().isPlaying)
            {
                yield return null;
            }
        }
        if (victory_1_AlertTrans.gameObject.activeSelf)
        {
            while (victory_1_AlertTrans.GetChild(0).GetComponent<Animation>().isPlaying)
            {
                yield return null;
            }
        }
        if (victory_2_AlertTrans.gameObject.activeSelf)
        {
            while (victory_2_AlertTrans.GetChild(0).GetComponent<Animation>().isPlaying)
            {
                yield return null;
            }
        }
        Destroy(UI_Main.UIM.main.GetChild(UI_Main.UIM.selectedMap).GetChild(0).gameObject);
        UI_Main.UIM.main.GetChild(0).gameObject.SetActive(true);
        UI_Main.UIM.main.GetChild(UI_Main.UIM.selectedMap).gameObject.SetActive(false);
        myProfilePos = 0;
    }

    private void DisplayVictoryAlert(bool isMyVictory)
    {
        if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_MATCH)
        {
            int tmp_point = 0;
            int tmp_score = 0;
            if (!victory_2_AlertTrans.gameObject.activeSelf)
            {
                victory_2_AlertTrans.gameObject.SetActive(true);
                Transform victory2Trans = victory_2_AlertTrans.GetChild(0).GetChild(0);
                if (isMyVictory)
                {
                    UI_Main.UIM.PlaySound("Particle_Wow");
                    UI_Main.UIM.fireworksTrans.gameObject.SetActive(true);

                    tmp_point = 100;
                    tmp_score = 1;

                    victory2Trans.GetChild(0).GetChild(7).GetChild(0).GetComponent<TextMeshProUGUI>().text = "You Win";
                    victory2Trans.GetChild(0).GetChild(7).GetChild(1).gameObject.SetActive(true);
                    victory2Trans.GetChild(0).GetChild(7).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(250.0f, 198.0f, 0.0f);
                    victory2Trans.GetChild(0).GetChild(6).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/star");
                    victory2Trans.GetChild(0).GetChild(8).gameObject.SetActive(true);
                    victory_2_AlertTrans.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/victoryBG");
                    if (myProfilePos == 1)
                    {
                        victory2Trans.GetChild(0).GetChild(4).gameObject.SetActive(true);
                        victory2Trans.GetChild(0).GetChild(5).gameObject.SetActive(false);
                        victory2Trans.GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = DisplayGameScores(UI_Main.UIM.playerScore, 1);
                        victory2Trans.GetChild(0).GetChild(3).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = UI_Main.UIM.rivalScore;
                    }
                    else
                    {
                        victory2Trans.GetChild(0).GetChild(5).gameObject.SetActive(true);
                        victory2Trans.GetChild(0).GetChild(4).gameObject.SetActive(false);
                        victory2Trans.GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = UI_Main.UIM.rivalScore;
                        victory2Trans.GetChild(0).GetChild(3).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = DisplayGameScores(UI_Main.UIM.playerScore, 1);
                    }
                }
                else
                {
                    UI_Main.UIM.PlaySound("warning_sound");

                    tmp_point = -50;
                    tmp_score = 0;

                    victory2Trans.GetChild(0).GetChild(7).GetChild(0).GetComponent<TextMeshProUGUI>().text = "You Lose";
                    victory2Trans.GetChild(0).GetChild(7).GetChild(1).gameObject.SetActive(false);
                    victory_2_AlertTrans.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/loseBG");
                    if (myProfilePos == 1)
                    {
                        victory2Trans.GetChild(0).GetChild(5).gameObject.SetActive(true);
                        victory2Trans.GetChild(0).GetChild(4).gameObject.SetActive(false);
                        victory2Trans.GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = UI_Main.UIM.playerScore;
                        victory2Trans.GetChild(0).GetChild(3).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = DisplayGameScores(UI_Main.UIM.rivalScore, 1);
                    }
                    else
                    {
                        victory2Trans.GetChild(0).GetChild(4).gameObject.SetActive(true);
                        victory2Trans.GetChild(0).GetChild(5).gameObject.SetActive(false);
                        victory2Trans.GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = DisplayGameScores(UI_Main.UIM.rivalScore, 1);
                        victory2Trans.GetChild(0).GetChild(3).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = UI_Main.UIM.playerScore;
                    }
                    victory2Trans.GetChild(0).GetChild(7).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(159.0f, 159.0f, 159.0f);
                    victory2Trans.GetChild(0).GetChild(6).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/star_lose");
                    victory2Trans.GetChild(0).GetChild(8).gameObject.SetActive(false);
                }

                if (myProfilePos == 1)
                {
                    victory2Trans.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = UI_Main.UIM.sel_img_texture;
                    victory2Trans.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = UI_Main.UIM.rival_img_texture;
                    victory2Trans.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = UI_Main.UIM.playerName;
                    victory2Trans.GetChild(0).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = UI_Main.UIM.rivalName;
                }
                else
                {
                    victory2Trans.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = UI_Main.UIM.sel_img_texture;
                    victory2Trans.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = UI_Main.UIM.rival_img_texture;
                    victory2Trans.GetChild(0).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = UI_Main.UIM.playerName;
                    victory2Trans.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = UI_Main.UIM.rivalName;
                }

                victory2Trans.GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = totalTime;
                victory2Trans.GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = DisplayGameScores(UI_Main.UIM.playerScore, tmp_score);
                victory2Trans.GetChild(1).GetChild(0).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = DisplayGameScores(UI_Main.UIM.playerPoint.ToString(), tmp_point);
            }

        }
        else  // Single Play Mode || Offline Play Mode
        {
            if (isMyVictory)
            {
                UI_Main.UIM.fireworksTrans.gameObject.SetActive(true);
                UI_Main.UIM.PlaySound("Particle_Wow");
                victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = Resources.Load<Texture>("designs/Player1");
                victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(4).GetComponent<TextMeshProUGUI>().text = "CONGRATULATIONS!";
                victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(4).GetComponent<TextMeshProUGUI>().color = new Color(250.0f, 198.0f, 0.0f);
                victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/star");
                victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/star");
                victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);
                victory_1_AlertTrans.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/victoryBG");
            }
            else
            {
                UI_Main.UIM.PlaySound("warning_sound");
                victory_1_AlertTrans.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/loseBG");
                if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_SINGLE)
                {
                    //victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = Resources.Load<Texture>("designs/avatar computer@2x");
                    victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = Resources.Load<Texture>("designs/Player1");
                    victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(4).GetComponent<TextMeshProUGUI>().text = "You lose!";
                    victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(4).GetComponent<TextMeshProUGUI>().color = new Color(250.0f, 198.0f, 0.0f);
                    victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/star_lose");
                    victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/star_lose");
                    victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = Resources.Load<Texture>("designs/Player1");
                    victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(4).GetComponent<TextMeshProUGUI>().text = "You lose!";
                    victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(4).GetComponent<TextMeshProUGUI>().color = new Color(250.0f, 198.0f, 0.0f);
                    victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/star_lose");
                    victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/star_lose");
                    victory_1_AlertTrans.GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
                }
            }

            victoryScoreArea.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = totalTime;
            victoryScoreArea.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = UI_Main.UIM.playerScore;
            victoryScoreArea.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = UI_Main.UIM.playerPoint.ToString();
            victory_1_AlertTrans.gameObject.SetActive(true);

        }
    }

    private string DisplayGameScores(string origin, int addition)
    {
        int result = int.Parse(origin) + addition;
        if (result < 1)
            return "0";
        return result.ToString();
    }

    /**
     *
     *  assign players infomation at play ground.
     *
     * **/
    public void AssignPlayersInfo()
    {
        if (UI_Main.UIM.selectedMap == 1)
        {
            PlayObjectTrans.GetChild(0).GetChild(0).gameObject.SetActive(true);
            PlayObjectTrans.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = gamePropertySettings.ISLAND_BET_COUNT;
            PlayObjectTrans.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/escrowbg");
            PlayObjectTrans.GetChild(0).GetChild(0).GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/coinicon");
            PlayObjectTrans.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/Game_Map_Iland");
            playGroundTrans.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/iland_cells");

            profileArea.GetChild(0).GetChild(0).GetComponent<Image>().color = new Vector4(0.0934988f, 0.245283f, 0.04049483f, 0.7803922f);
            profileArea.GetChild(0).GetChild(1).GetComponent<Image>().color = new Vector4(0.05724044f, 0.1698113f, 0.01842293f, 1.0f);
            profileArea.GetChild(1).GetChild(0).GetComponent<Image>().color = new Vector4(0.0934988f, 0.245283f, 0.04049483f, 0.7803922f);
            profileArea.GetChild(1).GetChild(1).GetComponent<Image>().color = new Vector4(0.05724044f, 0.1698113f, 0.01842293f, 1.0f);

            for (int i = 0; i < 9; i++)
            {
                playGroundTrans.GetChild(i).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/iland_stones");
            }
        }
        else if (UI_Main.UIM.selectedMap == 2)
        {
            PlayObjectTrans.GetChild(0).GetChild(0).gameObject.SetActive(true);
            PlayObjectTrans.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = gamePropertySettings.ICELAND_BET_COUNT;
            PlayObjectTrans.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/escrowbg");
            PlayObjectTrans.GetChild(0).GetChild(0).GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/coinicon");
            PlayObjectTrans.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/Game_Map_Ice");
            playGroundTrans.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/iceland_cells");

            profileArea.GetChild(0).GetChild(0).GetComponent<Image>().color = new Vector4(0.1415094f, 0.1415094f, 0.1415094f, 0.7803922f);
            profileArea.GetChild(0).GetChild(1).GetComponent<Image>().color = new Vector4(0.1226415f, 0.1226415f, 0.1226415f, 1.0f);
            profileArea.GetChild(1).GetChild(0).GetComponent<Image>().color = new Vector4(0.1415094f, 0.1415094f, 0.1415094f, 0.7803922f);
            profileArea.GetChild(1).GetChild(1).GetComponent<Image>().color = new Vector4(0.1226415f, 0.1226415f, 0.1226415f, 1.0f);

            for (int i = 0; i < 9; i++)
            {
                playGroundTrans.GetChild(i).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/fireland_stones");
            }
        }
        else if (UI_Main.UIM.selectedMap == 3)
        {
            PlayObjectTrans.GetChild(0).GetChild(0).gameObject.SetActive(true);
            PlayObjectTrans.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = gamePropertySettings.FIRELAND_BET_COUNT;
            PlayObjectTrans.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/escrowbg");
            PlayObjectTrans.GetChild(0).GetChild(0).GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/coinicon");
            PlayObjectTrans.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/Game_Map_Fire");
            playGroundTrans.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/fireland_cells");

            profileArea.GetChild(0).GetChild(0).GetComponent<Image>().color = new Vector4(0.1415094f, 0.1415094f, 0.1415094f, 0.7803922f);
            profileArea.GetChild(0).GetChild(1).GetComponent<Image>().color = new Vector4(0.1226415f, 0.1226415f, 0.1226415f, 1.0f);
            profileArea.GetChild(1).GetChild(0).GetComponent<Image>().color = new Vector4(0.1415094f, 0.1415094f, 0.1415094f, 0.7803922f);
            profileArea.GetChild(1).GetChild(1).GetComponent<Image>().color = new Vector4(0.1226415f, 0.1226415f, 0.1226415f, 1.0f);

            for (int i = 0; i < 9; i++)
            {
                playGroundTrans.GetChild(i).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/fireland_stones");
            }
        }

        if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_MATCH)
        {
            if (UI_Main.UIM.playerX == UI_Main.UIM.playerId)
            {
                myProfilePos = 1;
                profileArea.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = UI_Main.UIM.sel_img_texture;
                profileArea.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = UI_Main.UIM.playerName;
                profileArea.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("flags/" + UI_Main.UIM.playerCountry);
                profileArea.GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = UI_Main.UIM.playerScore;

                profileArea.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = UI_Main.UIM.rival_img_texture;
                profileArea.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = UI_Main.UIM.rivalName;
                profileArea.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("flags/" + UI_Main.UIM.rivalCountry);
                profileArea.GetChild(1).GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = UI_Main.UIM.rivalScore;
            }
            else
            {
                myProfilePos = 2;
                profileArea.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = UI_Main.UIM.sel_img_texture;
                profileArea.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = UI_Main.UIM.playerName;
                profileArea.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("flags/" + UI_Main.UIM.playerCountry);
                profileArea.GetChild(1).GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = UI_Main.UIM.playerScore;

                profileArea.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = UI_Main.UIM.rival_img_texture;
                profileArea.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = UI_Main.UIM.rivalName;
                profileArea.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("flags/" + UI_Main.UIM.rivalCountry);
                profileArea.GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = UI_Main.UIM.rivalScore;
            }
        }
        else //if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_SINGLE || UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_OFFLINE)
        {
            myProfilePos = 1;
            PlayObjectTrans.GetChild(0).GetChild(0).gameObject.SetActive(false);
            profileArea.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = Resources.Load<Texture>("designs/player1");
            profileArea.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Player1";

            if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_SINGLE)
            {
                profileArea.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = Resources.Load<Texture>("designs/avatar computer@2x");
                profileArea.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Guest";
            }
            else
            {
                profileArea.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = Resources.Load<Texture>("designs/player2");
                profileArea.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Player2";
            }

            profileArea.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetChild(1).gameObject.SetActive(false);
            profileArea.GetChild(0).GetChild(1).GetChild(1).GetChild(1).gameObject.SetActive(false);
            profileArea.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 0.0f);
            profileArea.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2(1.0f, 1.0f);

            profileArea.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(0).gameObject.SetActive(false);
            profileArea.GetChild(1).GetChild(1).GetChild(1).GetChild(1).gameObject.SetActive(false);
            profileArea.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 0.0f);
            profileArea.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2(1.0f, 1.0f);
        }

        UI_Main.UIM.PlayMusic("background_music");
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            // send request to server in order to get current tile pos history.
            if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_MATCH)
            {
                socketApi.instance.CancelSave(UI_Main.UIM.roomId);
            }
        }
        else
        {
            if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_MATCH)
            {
                socketApi.instance.SaveHistory(UI_Main.UIM.roomId);
            }
        }
    }
}
