using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class prefabEventTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Transform parentTrans;
    public Transform selectedTrans;

    private string tournamentId;

    // Use this for initialization
    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        tournamentId = parentTrans.gameObject.name;

        // event trigger in MyTournament

        if (selectedTrans.gameObject.name == "close")
        {
            gameApi.request.RemoveTournament(tournamentId);
            Destroy(parentTrans.gameObject);
        }
        else if (selectedTrans.gameObject.name == "edit")
        {
            UI_MyTournament.UIMT.EditTournament(tournamentId);
        }
        else if (selectedTrans.gameObject.name == "social_share")
        {
            //UI_Main.UIM.enterShareKeyAlert.gameObject.SetActive(true);
            int inviteKey = Random.Range(100000, 999999);
            UI_MyTournament.UIMT.ShareTournament(tournamentId, inviteKey);
           
        }
        else if (selectedTrans.gameObject.name == "viewBtn")
        {
            UI_MyTournament.UIMT.ViewSpecificTournament(tournamentId);
        }

        // event trigger in JoinableTournament

        else if (selectedTrans.gameObject.name == "joinBtn")
        {
            UI_JoinTournament.UIJT.SelectSpecificTournament(tournamentId);
        }
    }

}
