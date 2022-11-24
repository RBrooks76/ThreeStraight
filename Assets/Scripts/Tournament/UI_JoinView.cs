using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_JoinView : MonoBehaviour
{

    static public UI_JoinView UIJV;

    public Transform mapImg;
    public Transform joinField;
    public Transform leaveField;
    public Transform passedDate;
    public Transform remainDate;
    public Transform participantTrans;
    public Transform privacyLabel;
    public Transform rankLevel;
    public Transform VerificationCode;
    private gameApi.Tournament tournament;
    private bool isPrivate;
    private string tournamentId;

    // Start is called before the first frame update
    void Start()
    {
        tournament = null;
    }

    private void Awake()
    {
        StartCoroutine(AssignValues());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    ///         Display Proper Information for Selected Tournament
    /// </summary>

    private IEnumerator AssignValues()
    {
        while (UI_Ranking.UIR.selectedTournament == null)
        {
            yield return null;
        }

        tournament = UI_Ranking.UIR.selectedTournament;
        tournamentId = tournament._id;
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
        TournamentNotStarted(tournament);
        // operation field (display view or join button)
        if (tournament.isJoined)
        {
            joinField.gameObject.SetActive(false);
            leaveField.gameObject.SetActive(true);
        }
        else
        {
            // if current tournament is private, display enter-invite-code field
            // else hide input field.
            if (tournament.privacy.ToUpper() == gamePropertySettings.TOURNAMENT_PRIVATE.ToUpper())
            {
                isPrivate = true;
                joinField.GetChild(0).gameObject.SetActive(true);
            }
            else if (tournament.privacy.ToUpper() == gamePropertySettings.TOURNAMENT_PUBLIC.ToUpper())
            {
                isPrivate = false;
                joinField.GetChild(0).gameObject.SetActive(false);
            }
            joinField.gameObject.SetActive(true);
            leaveField.gameObject.SetActive(false);
        }
    }
    private void TournamentNotStarted(gameApi.Tournament tournament)
    {
        // set tournament name
        UI_Main.UIM.leaderBoardPage.GetChild(0).GetComponent<TextMeshProUGUI>().text = tournament.name;
        // set participant count
        participantTrans.GetChild(1).GetComponent<TextMeshProUGUI>().text = tournament.participants.ToString();
        participantTrans.GetChild(3).GetComponent<TextMeshProUGUI>().text = tournament.maxMembers.ToString();
        // set privacy
        privacyLabel.GetComponent<TextMeshProUGUI>().text = tournament.privacy.ToUpper();
        // set rank leve
        rankLevel.GetComponent<TextMeshProUGUI>().text = "LEVEL " + tournament.gameLevel.ToString();
        // display time
        passedDate.GetComponent<TextMeshProUGUI>().text = dateValidation.Instance.CalcTime(tournament.startDate);
        remainDate.GetComponent<TextMeshProUGUI>().text = dateValidation.Instance.CalcTime(tournament.endDate);
    }

    /// <summary>
    ///         Click Join Button to Join.
    /// </summary>
    public void OnClickJoin()
    {
        string code = string.Empty;
        if (isPrivate)
        {
            string v_code = VerificationCode.GetComponent<TMP_InputField>().text;
            if (string.IsNullOrEmpty(v_code))
            {
                // error alert
                UI_Main.UIM.SetErrAlertText(gamePropertySettings.WARNNING_TITLE, gamePropertySettings.EMPTY_VERIFYCODE);
                UI_Main.UIM.networkErrAlert.gameObject.SetActive(true);
                return;
            }
            code = v_code;
            // check some more 
        }

        gameApi.request.JoinTournament(tournamentId, code);
    }

    /// <summary>
    ///         Click Leave Button to Exit
    /// </summary>
    public void OnClickLeave()
    {
        gameApi.request.LeaveTournament(tournamentId);
    }
}
