using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_JoinTournament : MonoBehaviour
{

    static public UI_JoinTournament UIJT;

    public Transform allTournamentListView;

    public Transform listContentView;
    private string selTournamentId = string.Empty;

    public List<gameApi.Tournament> allTournamentList;
    


    // Start is called before the first frame update
    void Start()
    {
        if (UIJT == null) UIJT = this;

        allTournamentList = new List<gameApi.Tournament>();
        UI_Ranking.UIR.selectedTournament = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region All Tournament List

    GameObject prefabObj = null;

    /////////////////////////////////////////// Instantiate All Tournaments /////////////////////////////////////
    public void DisplayAllTournaments(List<gameApi.Tournament> list)
    {
        StartCoroutine(WaitDestory(list));
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
        contentView.GetChild(4).GetChild(3).GetComponent<TextMeshProUGUI>().text = tournament.maxMembers.ToString();
        contentView.GetChild(5).GetComponent<TextMeshProUGUI>().text = "LEVEL" + tournament.gameLevel.ToString();

        // operation field (display view or join button)
        if (tournament.isJoined)
        {
            contentView.GetChild(6).GetChild(0).gameObject.SetActive(true);
            contentView.GetChild(6).GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            contentView.GetChild(6).GetChild(0).gameObject.SetActive(false);
            contentView.GetChild(6).GetChild(1).gameObject.SetActive(true);
        }
    }

    ///////////////////////////////////////// Destory GameObjects ///////////////////////////////////////////////////
    IEnumerator WaitDestory(List<gameApi.Tournament> list)
    {
        while(listContentView.childCount > 0)
        {
            Destroy(listContentView.GetChild(listContentView.childCount-1).gameObject);
            yield return null;
        }

        selTournamentId = string.Empty;
        if (list.Count > 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                prefabObj = Instantiate(Resources.Load<GameObject>(gamePropertySettings._joinableTournament));
                prefabObj.transform.SetParent(listContentView);
                prefabObj.transform.localScale = Vector2.one;
                prefabObj.transform.localPosition = Vector2.zero;
                prefabObj.GetComponent<RectTransform>().sizeDelta = listContentView.GetComponent<RectTransform>().sizeDelta;
                AssignValues(list[i], listContentView, i);
            }
            listContentView.GetComponent<RectTransform>().SetHeight(150 * list.Count);
        }
    }

    public void DestoryGameObjects()
    {
        // Destory Old Tournaments
        if (listContentView.childCount > 0)
        {
            for (int i = 0; i < listContentView.childCount; i++)
            {
                Destroy(listContentView.GetChild(i).gameObject);
            }
        }
        selTournamentId = string.Empty;
    }

    ///////////////////////////////////////// Select Specific Tournament to Join ////////////////////////////////////
    public void SelectSpecificTournament(string tournamentId)
    {
        for (int i = 0; i < allTournamentList.Count; i++)
        {
            if (allTournamentList[i]._id == tournamentId)
            {
                selTournamentId = tournamentId;
                InitializeAllFields(allTournamentList[i]);
                break;
            }
        }
    }

    /// <summary>
    /// Display Current Information of Selected Tournament
    /// </summary>
    /// <param name="tournament"> Selected Tournament Information
    /// 
    GameObject prefabJoinViewObj = null;

    private void InitializeAllFields(gameApi.Tournament tournament)
    {
        UI_Ranking.UIR.selectedTournament = tournament;
        allTournamentListView.gameObject.SetActive(false);
        prefabJoinViewObj = Instantiate(Resources.Load<GameObject>(gamePropertySettings._joinView));
        prefabJoinViewObj.transform.SetParent(transform);
        prefabJoinViewObj.transform.localScale = Vector2.one;
        prefabJoinViewObj.transform.localPosition = Vector2.zero;
        prefabJoinViewObj.GetComponent<RectTransform>().sizeDelta = allTournamentListView.GetComponent<RectTransform>().sizeDelta;
        prefabJoinViewObj.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        prefabJoinViewObj.GetComponent<RectTransform>().offsetMax = Vector2.zero;

    }

    /// <summary>
    /// Join Private Tournament
    /// </summary>
    public void OnClickJoinPrivateTournament()
    {

    }

    /// <summary>
    /// Leave From Tournament
    /// </summary>
    public void OnClickLeave()
    {

    }

    

    #endregion
}
