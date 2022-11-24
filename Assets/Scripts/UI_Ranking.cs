using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UI_Ranking : MonoBehaviour
{
    static public UI_Ranking UIR;

    public Transform rankingView;
    public Transform joinTournamentView;
    public Transform myTournamentView;
    public Transform readyTournamentView;

    public Transform leaderboardTiele;
    public Transform rankingContent;

    public List<string> rankingList = new List<string>();

    public Transform customCalendarView;
    public string startDate;
    public string endDate;
    public bool startDateSelected;
    public bool endDateSelected;

    public gameApi.Tournament selectedTournament;

    // Start is called before the first frame update
    void Start()
    {
        if (UIR == null) UIR = this;

        startDate = string.Empty;
        endDate = string.Empty;
        startDateSelected = false;
        endDateSelected = false;
}

    // Update is called once per frame
    void Update()
    {
        
    }

    GameObject prefabObj = null;

    /**
     * 
     *  Ranking View
     * 
     * **/

    #region Leaderboard (Ranking View)
    public void displayRanking(List<string> list)
    {
        rankingList = list;

        if (list.Count > 3)
        {
            transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<ScrollRect>().verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
        }
        else
        {
            transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<ScrollRect>().verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
        }

        for (int i = 0; i < list.Count; i++)
        {
            prefabObj = Instantiate(Resources.Load<GameObject>(gamePropertySettings._rankingRow));
            prefabObj.transform.SetParent(rankingContent);
            prefabObj.transform.localScale = Vector2.one;
            prefabObj.transform.localPosition = Vector2.zero;
            prefabObj.GetComponent<RectTransform>().sizeDelta = rankingContent.GetComponent<RectTransform>().sizeDelta;

            assignTxt(list[i], rankingContent, i);
        }
        rankingContent.GetComponent<RectTransform>().SetHeight(150 * rankingContent.childCount);
    }

    void assignTxt(string info, Transform trans, int index)
    {
        // name, image, country, score;
        string[] orderInfo = info.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        if (index == 0)
        {
            trans.GetChild(index).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/order_1");
        }
        else if (index == 1)
        {
            trans.GetChild(index).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/order_2");
        }
        else if (index == 2)
        {
            trans.GetChild(index).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/order_3");
        }
        else 
        {
            trans.GetChild(index).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/order_4");
        }
  
        for (int i = 0; i < orderInfo.Length; i++)
        {
            trans.GetChild(index).GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "# " + (index + 1).ToString(); // order
            LoadAvatar(orderInfo[1], index, trans);  // avatar
            trans.GetChild(index).GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = orderInfo[0]; // name
            trans.GetChild(index).GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = orderInfo[2].ToUpper(); // country
            trans.GetChild(index).GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = orderInfo[3];   // point
            trans.GetChild(index).GetChild(0).GetChild(4).GetComponent<TextMeshProUGUI>().text = orderInfo[4];   // score
        }
    }

    //Renderer renderer;
    Image image;

    void LoadAvatar(string path, int index, Transform trans)
    {
        if (!UI_Main.UIM.isCached)
        {
            StartCoroutine(LoadProfileAvatars(path, index, trans));
        }

        Transform customTrans = trans.GetChild(index).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0);
        image = transform.GetChild(7).GetComponent<Image>();
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
        trans.GetChild(index).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().texture = imgTextureData; // avatar
    }

    public void OnClickBackFromLeaderBoard()
    {
        NavigatePageBack();        
    }

    /// <summary>
    ///         Back operation for each page.
    /// </summary>
    private void NavigatePageBack()
    {
        if (rankingView.gameObject.activeSelf)
        {
            if (rankingContent.childCount > 0)
            {
                for (int i = 0; i < rankingContent.childCount; i++)
                {
                    Destroy(rankingContent.GetChild(i).gameObject);
                }
            }
            rankingList.Clear();

            UI_Main.UIM.leaderBoardPage.gameObject.SetActive(false);
            UI_Main.UIM.gameMenuPage.gameObject.SetActive(true);
        }
        else if (myTournamentView.gameObject.activeSelf)
        {
            // if my tournament list is active
            if (!myTournamentView.GetChild(0).gameObject.activeSelf)
            {
                leaderboardTiele.GetComponent<TextMeshProUGUI>().text = gamePropertySettings.MY_TOURNAMENT_TITLE;
                myTournamentView.GetChild(0).gameObject.SetActive(true);
                myTournamentView.GetChild(1).gameObject.SetActive(false);
                myTournamentView.GetChild(2).gameObject.SetActive(false);

                if (myTournamentView.childCount > 3)
                    Destroy(myTournamentView.GetChild(3).gameObject);
            }
            else
            {
                UI_Main.UIM.leaderBoardPage.gameObject.SetActive(false);
                UI_Main.UIM.gameMenuPage.gameObject.SetActive(true);
                UI_MyTournament.UIMT.DestoryGameObjects();
            }
        }
        else if (joinTournamentView.gameObject.activeSelf)
        {
            if (joinTournamentView.GetChild(0).gameObject.activeSelf)
            {
                UI_Main.UIM.leaderBoardPage.gameObject.SetActive(false);
                UI_Main.UIM.gameMenuPage.gameObject.SetActive(true);
                UI_JoinTournament.UIJT.DestoryGameObjects();
            }
            else
            {
                leaderboardTiele.GetComponent<TextMeshProUGUI>().text = gamePropertySettings.JOIN_TOURNAMENT_TITLE;
                joinTournamentView.GetChild(0).gameObject.SetActive(true);
                if (joinTournamentView.childCount > 1)
                {
                    for (int i = 1; i < joinTournamentView.childCount; i++)
                    {
                        Destroy(joinTournamentView.GetChild(i).gameObject);
                    }
                }
            }
        }
        else if (readyTournamentView.gameObject.activeSelf)
        {

            myTournamentView.gameObject.SetActive(true);
            readyTournamentView.GetChild(0).gameObject.SetActive(true);
            readyTournamentView.GetChild(1).gameObject.SetActive(false);
            readyTournamentView.gameObject.SetActive(false);

            int childCount = readyTournamentView.GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetChild(0).childCount;
            for (int i = 0; i < childCount; i++)
            {
                Destroy(readyTournamentView.GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(childCount-1-i).gameObject);
            }
        }
        selectedTournament = null;
    }

    #endregion

    /**
     * 
     *  Join a Tournament
     * 
     * **/
    #region Join & Cancel Tournament
    public void OnClickJoin()
    {

    }

    public void OnClickLeave()
    {

    }
    #endregion
    
}
