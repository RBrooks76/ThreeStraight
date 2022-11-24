using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MyTournament : MonoBehaviour
{
    static public UI_MyTournament UIMT;
    public Transform createTournamentView;
    public Transform myTournamentListView;
    public Transform editTournamentView;
    public Transform tournamentListContent;
    public Transform createTournamentContent;
    public Transform startDateField;
    public Transform endDateField;
    public Transform editTournamentContent;
    public Transform editStartDateField;
    public Transform editEndDateField;
    private string selTournamentId = string.Empty;
    public List<gameApi.Tournament> tournamentList;

    // Start is called before the first frame update
    void Start()
    {
        if (UIMT == null) UIMT = this;
        tournamentList = new List<gameApi.Tournament>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!string.IsNullOrEmpty(UI_Ranking.UIR.startDate))
        {
            if (UI_Ranking.UIR.startDate != startDateField.GetComponent<TMP_InputField>().text)
            {
                startDateField.GetComponent<TMP_InputField>().text = UI_Ranking.UIR.startDate;
                editStartDateField.GetComponent<TMP_InputField>().text = UI_Ranking.UIR.startDate;
            }
        }
        if (!string.IsNullOrEmpty(UI_Ranking.UIR.endDate))
        {
            if (UI_Ranking.UIR.endDate != endDateField.GetComponent<TMP_InputField>().text)
            {
                endDateField.GetComponent<TMP_InputField>().text = UI_Ranking.UIR.endDate;
                editEndDateField.GetComponent<TMP_InputField>().text = UI_Ranking.UIR.endDate;
            }
        }
    }

    /***
     *
     *  Manage My Tournaments
     *
     ***/
    #region Manage Tournament

    GameObject prefabObj = null;

    /////////////////////////////////////////// Instantiate All Tournaments /////////////////////////////////////
    public void DisplayMyTournaments(List<gameApi.Tournament> list)
    {
        StartCoroutine(WaitDestory(list));
    }

    ///////////////////////////////////////// Destory GameObjects ///////////////////////////////////////////////////
    public void DestoryGameObjects()
    {
        // Destory Old Tournaments
        if (tournamentListContent.childCount > 0)
        {
            for (int i = 0; i < tournamentListContent.childCount; i++)
            {
                Destroy(tournamentListContent.GetChild(i).gameObject);
            }
        }
        selTournamentId = string.Empty;
    }

    ///////////////////////////////////////// Destory GameObjects ///////////////////////////////////////////////////
    IEnumerator WaitDestory(List<gameApi.Tournament> list)
    {
        while (tournamentListContent.childCount > 0)
        {
            Destroy(tournamentListContent.GetChild(tournamentListContent.childCount - 1).gameObject);
            yield return null;
        }

        selTournamentId = string.Empty;
        if (list.Count > 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                prefabObj = Instantiate(Resources.Load<GameObject>(gamePropertySettings._manageTournament));
                prefabObj.transform.SetParent(tournamentListContent);
                prefabObj.transform.localScale = Vector2.one;
                prefabObj.transform.localPosition = Vector2.zero;
                prefabObj.GetComponent<RectTransform>().sizeDelta = tournamentListContent.GetComponent<RectTransform>().sizeDelta;
                AssignValues(list[i], tournamentListContent, i);
            }
            tournamentListContent.GetComponent<RectTransform>().SetHeight(150 * list.Count);
        }
    }

    ////////////////////////////////////////////// Assign Values //////////////////////////////////////////////////
    public void AssignValues(gameApi.Tournament tournament, Transform trans, int index)
    {
        Transform contentView = trans.GetChild(index).GetChild(0);
        trans.GetChild(index).gameObject.name = tournament._id;
        contentView.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = tournament.status.ToUpper();
        contentView.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = tournament.name.ToUpper();
        contentView.GetChild(2).GetComponent<TextMeshProUGUI>().text = tournament.map.ToUpper();
        contentView.GetChild(3).GetComponent<TextMeshProUGUI>().text = tournament.privacy.ToUpper();
        contentView.GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>().text = tournament.participants.ToString();
        contentView.GetChild(4).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/participants");
        contentView.GetChild(4).GetChild(3).GetComponent<TextMeshProUGUI>().text = tournament.maxMembers.ToString();
        contentView.GetChild(5).GetComponent<TextMeshProUGUI>().text = "LEVEL" + tournament.gameLevel.ToString();

        if (tournament.creator == UI_Main.UIM.playerId)
        {
            string tournamentStatus = dateValidation.Instance.CalcTime(tournament.startDate);
            if (tournamentStatus == "PROGRESS" || string.IsNullOrEmpty(tournamentStatus))
            {
                for (int i = 0; i < 3; i++)
                {
                    contentView.GetChild(6).GetChild(i).gameObject.SetActive(false);
                }
                contentView.GetChild(6).GetChild(3).gameObject.SetActive(true);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    contentView.GetChild(6).GetChild(i).gameObject.SetActive(true);
                }
                contentView.GetChild(6).GetChild(3).gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                contentView.GetChild(6).GetChild(i).gameObject.SetActive(false);
            }
            contentView.GetChild(6).GetChild(3).gameObject.SetActive(true);
        }

        if (tournament.privacy.ToUpper() == gamePropertySettings.TOURNAMENT_PUBLIC.ToUpper())
        {
            contentView.GetChild(6).GetChild(0).gameObject.SetActive(false);
        }
        else if (tournament.privacy.ToUpper() == gamePropertySettings.TOURNAMENT_PRIVATE.ToUpper())
        {
            contentView.GetChild(6).GetChild(0).gameObject.SetActive(true);
        }
    }

    #endregion


    /**
     *
     *  Switch Tabs in My Tournament Page
     *
     * **/
    #region Switch Tabs

    // Switch Tabs
    public void ManageToCreate()
    {
        myTournamentListView.gameObject.SetActive(true);
        createTournamentView.gameObject.SetActive(false);
    }
    public void CreateToManage()
    {
        myTournamentListView.gameObject.SetActive(false);
        createTournamentView.gameObject.SetActive(true);
    }
    #endregion

    #region Create Tournament
    // Create Tournament
    public void OnClickCreateNewTournament()
    {
        string name = createTournamentContent.GetChild(0).GetChild(0).GetComponent<TMP_InputField>().text;
        int area = createTournamentContent.GetChild(0).GetChild(1).GetComponent<TMP_Dropdown>().value;
        string startDate = startDateField.GetComponent<TMP_InputField>().text;
        string endDate = endDateField.GetComponent<TMP_InputField>().text;
        int privacy = createTournamentContent.GetChild(2).GetChild(0).GetComponent<TMP_Dropdown>().value;
        int participants = createTournamentContent.GetChild(2).GetChild(1).GetComponent<TMP_Dropdown>().value;
        int level = createTournamentContent.GetChild(3).GetChild(0).GetComponent<TMP_Dropdown>().value;

        if (string.IsNullOrEmpty(name))
        {
            // error alert
            UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.TOURNAMENT_NAME_INPUT_ERROR);
            UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
            return;
        }

        if (area < 1)
        {
            // error alert
            UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.SELECT_AREA);
            UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
            return;
        }

        if (string.IsNullOrEmpty(startDate))
        {
            // error alert
            UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.SELECT_START_DATE);
            UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
            return;
        }
        else
        {
            string currentDate = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
            bool isValid = dateValidation.Instance.CheckTournamentPeriod(currentDate, startDate);
            if (!isValid)
            {
                UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.START_DATE_INVALID);
                UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
            }
        }

        if (string.IsNullOrEmpty(endDate))
        {
            // error alert
            UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.SELECT_END_DATE);
            UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
            return;
        }
        else
        {
            bool isValid = dateValidation.Instance.CheckTournamentPeriod(startDate, endDate);
            if (!isValid)
            {
                UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.END_DATE_INVALID);
                UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
            }
        }

        if (privacy < 1)
        {
            // error alert
            UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.SELECT_PRIVACY);
            UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
            return;
        }

        if (participants < 1)
        {
            // error alert
            UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.SELECT_PARTICIPANTS);
            UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
            return;
        }

        if (level < 1)
        {
            // error alert
            UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.SELECT_LEVEL);
            UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
            return;
        }

        string str_area = createTournamentContent.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text;
        string str_privacy = createTournamentContent.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text;
        string str_participants = createTournamentContent.GetChild(2).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text;
        string str_level = createTournamentContent.GetChild(3).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text;
        DestoryGameObjects();
        gameApi.request.CreateNewTournament(name, str_area, startDate, endDate, str_privacy, str_participants, str_level);
    }

    public void ResetTournamentInfo()
    {
        UI_Ranking.UIR.startDate = string.Empty;
        UI_Ranking.UIR.endDate = string.Empty;
        createTournamentContent.GetChild(0).GetChild(0).GetComponent<TMP_InputField>().text = string.Empty;
        createTournamentContent.GetChild(0).GetChild(1).GetComponent<TMP_Dropdown>().value = 0;
        startDateField.GetComponent<TMP_InputField>().text = string.Empty;
        endDateField.GetComponent<TMP_InputField>().text = string.Empty;
        createTournamentContent.GetChild(2).GetChild(0).GetComponent<TMP_Dropdown>().value = 0;
        createTournamentContent.GetChild(2).GetChild(1).GetComponent<TMP_Dropdown>().value = 0;
        createTournamentContent.GetChild(3).GetChild(0).GetComponent<TMP_Dropdown>().value = 0;
    }

    #endregion

    #region Edit Tournament

    // Edit Tournament
    public void OnClickEditTournament()
    {
        string name = editTournamentContent.GetChild(0).GetChild(0).GetComponent<TMP_InputField>().text;
        int area = editTournamentContent.GetChild(0).GetChild(1).GetComponent<TMP_Dropdown>().value;
        string startDate = editStartDateField.GetComponent<TMP_InputField>().text;
        string endDate = editEndDateField.GetComponent<TMP_InputField>().text;
        int privacy = editTournamentContent.GetChild(2).GetChild(0).GetComponent<TMP_Dropdown>().value;
        int participants = editTournamentContent.GetChild(2).GetChild(1).GetComponent<TMP_Dropdown>().value;

        if (string.IsNullOrEmpty(name))
        {
            // error alert
            UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.TOURNAMENT_NAME_INPUT_ERROR);
            UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
            return;
        }

        if (area < 1)
        {
            // error alert
            UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.SELECT_AREA);
            UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
            return;
        }

        if (string.IsNullOrEmpty(startDate))
        {
            // error alert
            UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.SELECT_START_DATE);
            UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
            return;
        }
        else
        {
            string currentDate = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
            bool isValid = dateValidation.Instance.CheckTournamentPeriod(currentDate, startDate);
            if (!isValid)
            {
                UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.START_DATE_INVALID);
                UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
                return;
            }
        }

        if (string.IsNullOrEmpty(endDate))
        {
            // error alert
            UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.SELECT_END_DATE);
            UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
            return;
        }
        else
        {
            bool isValid = dateValidation.Instance.CheckTournamentPeriod(startDate, endDate);
            if (!isValid)
            {
                UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.END_DATE_INVALID);
                UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
            }
        }

        if (privacy < 1)
        {
            // error alert
            UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.SELECT_PRIVACY);
            UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
            return;
        }

        if (participants < 1)
        {
            // error alert
            UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.SELECT_PARTICIPANTS);
            UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
            return;
        }

        string str_area = editTournamentContent.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text;
        string str_privacy = editTournamentContent.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text;
        string str_participants = editTournamentContent.GetChild(2).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text;
        string str_level = editTournamentContent.GetChild(3).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text;

        gameApi.request.EditTournament(selTournamentId, name, str_area, startDate, endDate, str_privacy, str_participants, str_level);
        DestoryGameObjects();
    }

    public void EditTournament(string tournamentId)
    {
        DisplayEditView();
        for (int i = 0; i < tournamentList.Count; i++)
        {
            if (tournamentList[i]._id == tournamentId)
            {
                selTournamentId = tournamentId;
                InitializeAllFields(tournamentList[i]);
                break;
            }
        }
    }

    public void DisplayEditView()
    {
        myTournamentListView.gameObject.SetActive(false);
        editTournamentView.gameObject.SetActive(true);
    }

    public void HideEditView()
    {
        // change Title
        UI_Ranking.UIR.leaderboardTiele.GetComponent<TextMeshProUGUI>().text = gamePropertySettings.MY_TOURNAMENT_TITLE;
        myTournamentListView.gameObject.SetActive(true);
        editTournamentView.gameObject.SetActive(false);
    }

    /// <summary>
    /// Share Tournament via Email or Social
    /// </summary>
    /// <param name="tournamentId"> Tournament Id to be shared </param>
    public void ShareTournament(string tournamentId, int inviteKey)
    {
        for (int i = 0; i < tournamentList.Count; i++)
        {
            if (tournamentList[i]._id == tournamentId)
            {
                selTournamentId = tournamentId;

                string inviteMessage = UI_Main.UIM.playerName + " has invited you to join '" + tournamentList[i].name + "' tournament of Threestraight Game with code: " + "'" + inviteKey.ToString() + "'";
                Debug.Log("Share Information =====>" + inviteMessage);
                new NativeShare().SetText(inviteMessage).SetCallback((result, shareTarget) =>
                {
                    gameApi.request.publishCode(tournamentId, inviteKey);
                }).Share();

                break;
            }
        }
    }

    /// <summary>
    /// Initialize all fields with selected data in tournament (Edit)
    /// </summary>
    /// <param name="tournament"> Tournament Data </param>
    private void InitializeAllFields(gameApi.Tournament tournament)
    {
        // leaderboard title.
        UI_Ranking.UIR.leaderboardTiele.GetComponent<TextMeshProUGUI>().text = "EDIT \n <" + tournament.name + ">";

        // tournament name
        editTournamentContent.GetChild(0).GetChild(0).GetComponent<TMP_InputField>().text = tournament.name;

        // tournament map
        if (tournament.map == gamePropertySettings.MAP_ISLAND.ToUpper())
        {
            editTournamentContent.GetChild(0).GetChild(1).GetComponent<TMP_Dropdown>().value = 1;
        }
        else if (tournament.map == gamePropertySettings.MAP_ICELAND.ToUpper())
        {
            editTournamentContent.GetChild(0).GetChild(1).GetComponent<TMP_Dropdown>().value = 2;
        }
        else if (tournament.map == gamePropertySettings.MAP_FIRELAND.ToUpper())
        {
            editTournamentContent.GetChild(0).GetChild(1).GetComponent<TMP_Dropdown>().value = 3;
        }
        else
        {
            editTournamentContent.GetChild(0).GetChild(1).GetComponent<TMP_Dropdown>().value = 0;
        }

        editStartDateField.GetComponent<TMP_InputField>().text = dateValidation.Instance.ConvertTimeStampToString(tournament.startDate);
        editEndDateField.GetComponent<TMP_InputField>().text = dateValidation.Instance.ConvertTimeStampToString(tournament.endDate);

        // tournament privacy
        if (tournament.privacy.ToUpper() == gamePropertySettings.TOURNAMENT_PRIVATE.ToUpper())
        {
            editTournamentContent.GetChild(2).GetChild(0).GetComponent<TMP_Dropdown>().value = 1;
        }
        else if (tournament.privacy.ToUpper() == gamePropertySettings.TOURNAMENT_PUBLIC.ToUpper())
        {
            editTournamentContent.GetChild(2).GetChild(0).GetComponent<TMP_Dropdown>().value = 2;
        }
        else
        {
            editTournamentContent.GetChild(2).GetChild(0).GetComponent<TMP_Dropdown>().value = 0;
        }

        // tournament participants
        editTournamentContent.GetChild(2).GetChild(1).GetComponent<TMP_Dropdown>().value = tournament.maxMembers - 5;

        // tournament rank-level
        editTournamentContent.GetChild(3).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = tournament.gameLevel.ToString();
    }

    #endregion

    #region View Joined Tournament

    ///////////////////////////////////////// Select Specific Tournament to Join ////////////////////////////////////
    public void ViewSpecificTournament(string tournamentId)
    {
        for (int i = 0; i < tournamentList.Count; i++)
        {
            if (tournamentList[i]._id == tournamentId)
            {
                selTournamentId = tournamentId;
                InitializeJoinableView(tournamentList[i]);
                break;
            }
        }
    }

    /// <summary>
    ///         Display Current Information of Selected Tournament
    /// </summary>
    /// <param name="tournament">
    ///         Selected Tournament Information
    /// </param>
    ///
    GameObject prefabJoinViewObj = null;
    private void InitializeJoinableView(gameApi.Tournament tournament)
    {
        UI_Ranking.UIR.selectedTournament = tournament;

        if (tournament.status == gamePropertySettings.TOURNAMENT_PENDING)
        {
            // if it's pending status, only display details and user is able to cancel
            myTournamentListView.gameObject.SetActive(false);

            prefabJoinViewObj = Instantiate(Resources.Load<GameObject>(gamePropertySettings._joinView));
            prefabJoinViewObj.transform.SetParent(transform);
            prefabJoinViewObj.transform.localScale = Vector2.one;
            prefabJoinViewObj.transform.localPosition = Vector2.zero;
            prefabJoinViewObj.GetComponent<RectTransform>().sizeDelta = myTournamentListView.GetComponent<RectTransform>().sizeDelta;
            prefabJoinViewObj.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            prefabJoinViewObj.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        }
        else if (tournament.status == gamePropertySettings.TOURNAMENT_ACTIVE)
        {
            // if it's active status, user can play the game.
            transform.gameObject.SetActive(false);
            UI_Ranking.UIR.readyTournamentView.gameObject.SetActive(true);
            if (UI_ReadyTournament.UIRT != null)
            {
                UI_ReadyTournament.UIRT.DisplayDetails(tournament);
            }
            //StartCoroutine(CheckReadyTournamentViewActive(tournament));
        }
    }

    //IEnumerator CheckReadyTournamentViewActive(gameApi.Tournament tournament)
    //{
    //    yield return new WaitForSeconds(0.1f);
    //    while (!UI_Ranking.UIR.readyTournamentView.gameObject.activeSelf)
    //    {
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //    transform.gameObject.SetActive(false);
    //    UI_ReadyTournament.UIRT.DisplayDetails(tournament);
    //}
    #endregion

    /***
     *
     *  Date picker
     *
     ***/
    public void OnClickDatePicker(int flag)
    {
        UI_Ranking.UIR.customCalendarView.gameObject.SetActive(true);
        if (flag == 0)
        {
            UI_Ranking.UIR.startDateSelected = true;
            UI_Ranking.UIR.endDateSelected = false;
        }
        else
        {
            UI_Ranking.UIR.startDateSelected = false;
            UI_Ranking.UIR.endDateSelected = true;
        }
    }

    public void OnClickCloseDataPicker()
    {
        UI_Ranking.UIR.customCalendarView.gameObject.SetActive(false);
    }

}
