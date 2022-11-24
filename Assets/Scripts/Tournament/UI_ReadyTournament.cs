using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UI_ReadyTournament : MonoBehaviour
{
    static public UI_ReadyTournament UIRT;

    public Transform tournamentDetailsView;
    public Transform mapImg;
    public Transform remainDate;
    public Transform participantsCount;
    public Transform MaxCount;
    public Transform privacyLabel;
    public Transform rankLevel;
    public Transform tournamentParticipantsView;
    public Transform participantsContentView;

    // Start is called before the first frame update
    void Start()
    {
        if (UIRT == null) UIRT = this;   
    }

    private void Awake()
    {
        StartCoroutine(AssignValues());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Initialize All Fields When component is awake.
    private IEnumerator AssignValues()
    {
        while (UI_Ranking.UIR.selectedTournament == null)
        {
            yield return null;
        }
        DisplayDetails(UI_Ranking.UIR.selectedTournament);
    }
    #endregion

    #region Switch Tabs

    // Switch Tabs
    public void DetailsToParticipants()
    {
        gameApi.request.LoadRankingInTournament(UI_Ranking.UIR.selectedTournament._id);
    }

    public void ActiveParticipantsRankingView()
    {
        tournamentParticipantsView.gameObject.SetActive(true);
        tournamentDetailsView.gameObject.SetActive(false);
    }

    public void ParticipantsToDetails()
    {
        tournamentParticipantsView.gameObject.SetActive(false);
        tournamentDetailsView.gameObject.SetActive(true);
    }
    #endregion


    #region Tournament Details

    /// <summary>
    ///         Display selected Tournament in detail, that user can play game.
    /// </summary>
    /// <param name="tournament"></param>
    public void DisplayDetails(gameApi.Tournament tournament)
    {
        // set map image
        if (tournament.map == gamePropertySettings.MAP_ISLAND.ToUpper())
        {
            mapImg.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/map_iland");
        }
        else if (tournament.map == gamePropertySettings.MAP_ICELAND.ToUpper())
        {
            mapImg.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/02");
        }
        else if (tournament.map == gamePropertySettings.MAP_FIRELAND.ToUpper())
        {
            mapImg.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/unlock_fireland");
        }

        string status = dateValidation.Instance.CalcTime(tournament.startDate);

        // set tournament name
        UI_Main.UIM.leaderBoardPage.GetChild(0).GetComponent<TextMeshProUGUI>().text = tournament.name;
        // set participant count
        participantsCount.GetComponent<TextMeshProUGUI>().text = tournament.participants.ToString();
        MaxCount.GetComponent<TextMeshProUGUI>().text = tournament.maxMembers.ToString();
        // set privacy
        privacyLabel.GetComponent<TextMeshProUGUI>().text = tournament.privacy.ToUpper();
        // set rank leve
        rankLevel.GetComponent<TextMeshProUGUI>().text = "LEVEL " + tournament.gameLevel.ToString();
        // display time
        remainDate.GetComponent<TextMeshProUGUI>().text = dateValidation.Instance.CalcTime(tournament.endDate);       
    }

    /// <summary>
    ///         Start playing game in Tournament
    /// </summary>
    public void OnClickPlayTournament()
    {
        //UI_Main.UIM.selectedTournamentId = UI_Ranking.UIR.selectedTournament._id;
        socketApi.instance.ConnectWithSocketServer();
        if (UI_Ranking.UIR.selectedTournament.map.ToUpper() == gamePropertySettings.MAP_ISLAND.ToUpper())
        {
            UI_Main.UIM.selectedMap = 1;
        }
        else if (UI_Ranking.UIR.selectedTournament.map.ToUpper() == gamePropertySettings.MAP_ICELAND.ToUpper())
        {
            UI_Main.UIM.selectedMap = 2;
        }
        else if (UI_Ranking.UIR.selectedTournament.map.ToUpper() == gamePropertySettings.MAP_FIRELAND.ToUpper())
        {
            UI_Main.UIM.selectedMap = 3;
        }
        UI_Main.UIM.playMode = gamePropertySettings.PLAY_MODE_MATCH;
        UI_Main.UIM.matchFromWhere = gamePropertySettings.FROM_TOURNAMENT_ROOM;
        UI_Main.UIM.selectedTournamentId = UI_Ranking.UIR.selectedTournament._id;
        gameApi.request.CreateGameRoom(UI_Main.UIM.playerId, UI_Ranking.UIR.selectedTournament.map, UI_Main.UIM.playerScore, UI_Ranking.UIR.selectedTournament._id);

    }

    /// <summary>
    ///         Exit from Tournament forever
    /// </summary>
    public void OnClickExitTournament()
    {
        gameApi.request.LeaveTournament(UI_Ranking.UIR.selectedTournament._id);
    }

    #endregion

    #region Participants Ranking in Tournament
    /// <summary>
    ///         Create rank GameObjects as many as the number of list.
    /// </summary>
    /// <param name="list">
    ///         Rank Data.
    /// </param>

    GameObject prefabObj = null;
    public void CreateGameObjectsForRanking(List<gameApi.TOrder> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            prefabObj = Instantiate(Resources.Load<GameObject>(gamePropertySettings._tournamentRank));
            prefabObj.transform.SetParent(participantsContentView);
            prefabObj.transform.localScale = Vector2.one;
            prefabObj.transform.localPosition = Vector2.zero;
            prefabObj.GetComponent<RectTransform>().sizeDelta = participantsContentView.GetComponent<RectTransform>().sizeDelta;
            DispAllInfo(list[i], participantsContentView, i);
        }
        ActiveParticipantsRankingView();
        participantsContentView.GetComponent<RectTransform>().SetHeight(150 * participantsContentView.childCount);
    }

    private void DispAllInfo(gameApi.TOrder data, Transform trans, int index)
    {
        Transform content = trans.GetChild(index).GetChild(0);
        content.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "LEVEL " + data.level.ToString();
        content.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = data.name;
        content.GetChild(2).GetComponent<TextMeshProUGUI>().text = data.country.ToUpper();
        content.GetChild(3).GetComponent<TextMeshProUGUI>().text = data.score.ToString();
        LoadAvatar(data.avatar, index, content);
    }

    //Renderer renderer;
    Image image;

    void LoadAvatar(string path, int index, Transform trans)
    {
        if (!UI_Main.UIM.isCached)
        {
            StartCoroutine(LoadProfileAvatars(path, index, trans));
        }

        Transform customTrans = trans.GetChild(1).GetChild(0);
        image = UI_Ranking.UIR.transform.GetChild(7).GetComponent<Image>();
        Davinci.get().load(path).setFadeTime(1).setCached(true).into(image).start();
        customTrans.GetComponent<RawImage>().texture = image.mainTexture;
    }

    IEnumerator LoadProfileAvatars(string path, int index, Transform trans)
    {
        Texture2D imgTextureData = null;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);
        UI_Main.UIM.activityBar.gameObject.SetActive(true);
        yield return www.SendWebRequest();
        UI_Main.UIM.activityBar.gameObject.SetActive(false);

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            imgTextureData = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
        trans.GetChild(1).GetChild(0).GetComponent<RawImage>().texture = imgTextureData; // avatar
    }
    #endregion
}
