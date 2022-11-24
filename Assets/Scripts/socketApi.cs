using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KyleDulce.SocketIo;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class socketApi : MonoBehaviour
{
    static public socketApi instance;
    public Socket socket;
    public bool isListerCreated;
    public bool isMatched;
    public bool isJoined;
    public bool isEndGame;
    public bool isResWhoTurn;
    public bool updateMoveHistory;
    public bool matchResult; // check whether matching person found or not.
    public bool isBackground;
    public string oldPlayer;
    public string newPlayer;
    public string tilePosList; // present current tiles position.
    public string bgUser;
    public int multiple_startPos;
    public int multiple_endPos;
    public int total_move;
    public DateTime startTime;
    public DateTime turnLimitTime;
    public DateTime bgStartTime;
    public List<string> moveHistoryArr;
    public long timeOver;
    string firstTurn;
    bool isMatchedNothing;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) instance = this;
        isListerCreated = false;
        InitializeAllScopedValues();
    }

    void InitializeAllScopedValues()
    {
        isListerCreated = false;
        isMatched = false;
        isJoined = false;
        isEndGame = false;
        firstTurn = string.Empty;
        isResWhoTurn = false;
        oldPlayer = string.Empty;
        newPlayer = string.Empty;
        multiple_startPos = -1;
        multiple_endPos = -1;
        total_move = 0;
        tilePosList = string.Empty; // present current tiles position.
        updateMoveHistory = false;
        matchResult = false; // check whether matching person found or not.
        isMatchedNothing = false;
        moveHistoryArr = new List<string>();
        isBackground = false;
        bgStartTime = DateTime.Now;
        timeOver = 0;
        bgUser = string.Empty;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMatched)
        {
            isMatched = false;
            MatchedPlayer();
        }
        if (isJoined)
        {
            isJoined = false;
            OpenGameGround();
        }
        if (isEndGame)
        {
            isEndGame = false;
            FinishGame(UI_PlayGround.UIP.winPlayer);
        }
        if (isBackground)
        {
            if (bgUser != UI_Main.UIM.playerId)
            {
                if ( (DateTime.Now - bgStartTime).TotalSeconds > 100)
                {
                    isBackground = false;
                    bgUser = string.Empty;
                    NonResponseAndKeepBackground();
                }
            }
        }
    }
    
    /**
     * 
     *  connet socket. 
     * 
     **/
    public void ConnectWithSocketServer()
    {
        if (socket == null)
        {
            socket = SocketIo.establishSocketConnection(gameApi.request._socketUrl);
            socket.connect();
            if (!isListerCreated)
            {
                Debug.Log("<===== Creating Socket Listener =====>");
                ListenSocket();
            }
            isListerCreated = true;
        }
    }

    public void NonResponseAndKeepBackground()
    {
        if (socket != null)
        {
            if (!string.IsNullOrEmpty(UI_Main.UIM.roomId))
            {
                if (!string.IsNullOrEmpty(UI_Main.UIM.playerId))
                {
                    EndGame(UI_Main.UIM.roomId, UI_Main.UIM.playerId);
                }
                else
                {
                    NoMatchedAndLeaveRoom(1);
                    gameApi.request.RemoveRoom(UI_Main.UIM.roomId);
                }
            }
        }
    }


    void ListenSocket()
    {
        if (socket != null)
        {
            socket.on("matched", OnMatch);
            socket.on("end", OnEnd);
            socket.on("whoTurn", OnWhoTurn);
            socket.on("nothing-matched", OnNothingMatched);
            socket.on("left", OnLeftRoom);
            socket.on("closeApp", OnDisconnection);
        }
    }

    // Match
    void OnMatch(string ev)
    {
        string resp = string.Empty;
        socket.InvokeEvent(ev, resp);
        var res = JsonUtility.FromJson<MatchUserData>(ev);

        if (UI_Main.UIM.playerId != res.player1.userId)
        {
            UI_Main.UIM.rivalPlayer = res.player1.userId;
            UI_Main.UIM.rivalAvatar = res.player1.avatar;
            UI_Main.UIM.rivalCountry = res.player1.country;
            UI_Main.UIM.rivalName = res.player1.name;
            UI_Main.UIM.rivalScore = res.player1.score;
        } 
        else
        {
            UI_Main.UIM.rivalPlayer = res.player2.userId;
            UI_Main.UIM.rivalAvatar = res.player2.avatar;
            UI_Main.UIM.rivalCountry = res.player2.country;
            UI_Main.UIM.rivalName = res.player2.name;
            UI_Main.UIM.rivalScore = res.player2.score;
        }
        total_move = 0;
        isMatched = true;
        UI_Main.UIM.playerX = res.turnPlayer;
      
    }

    // Join
    void OnCreated(string ev)
    {
        isJoined = true;
    }

    // End
    void OnEnd(string ev)
    {
        string resp = string.Empty;
        socket.InvokeEvent(ev, resp);
        var endData = JsonUtility.FromJson<SocketChannelName>(ev);
        UI_PlayGround.UIP.winPlayer = endData.player1;
        isEndGame = true;
    }

    // WhoTurn
    void OnWhoTurn(string ev)
    {
        string resp = string.Empty;
        socket.InvokeEvent(ev, resp);
        var res_data = JsonUtility.FromJson<NextTurn>(ev);
        newPlayer = res_data.newPlayer;
        oldPlayer = res_data.oldPlayer;
        multiple_startPos = int.Parse(res_data.startPos);
        multiple_endPos = int.Parse(res_data.endPos);
        total_move += int.Parse(res_data.moveCount);
        turnLimitTime = DateTime.Now;
        isResWhoTurn = true;
    }

    // Nothing Matched
    void OnNothingMatched(string ev)
    {
        UI_Main.UIM.matchingPage.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = ev;
        isMatchedNothing = true;
        UI_Main.UIM.matchingPage.GetChild(2).gameObject.SetActive(false);
        UI_Main.UIM.matchingPage.GetChild(3).gameObject.SetActive(true);
    }

    // Leave Room
    void OnLeftRoom(string ev)
    {
        gameApi.request.RemoveRoom(UI_Main.UIM.roomId);
    }
    void OnDisconnection(string ev)
    {
        FinishGame(UI_Main.UIM.playerId);
    }

    /**
     * 
     *  Finish Game
     * 
     * **/
    private void FinishGame(string winner) 
    {
        StartCoroutine(DispWinnerAlert(winner));
    }
    IEnumerator DispWinnerAlert(string winner)
    {
        yield return new WaitForSeconds(0.5f);
        if (UI_Main.UIM.main.GetChild(UI_Main.UIM.selectedMap).gameObject.activeSelf)
        {
            UI_PlayGround.UIP.isStartGame = true;           
            UI_PlayGround.UIP.GotoStartPageAfterExit(winner);
        }
    }

    /**
     * 
     *  go to game-play-ground upon match.
     * 
    **/
    private void MatchedPlayer()
    {
        if (!string.IsNullOrEmpty(UI_Main.UIM.rivalPlayer))
        {
            UI_Main.UIM.isMyTurn = true;
            UI_Main.UIM.matchingPage.GetChild(1).GetComponent<Button>().interactable = false;
            AvatarFromServer();
        }
    }
    void AvatarFromServer()
    {
        StartCoroutine(DownloadImageFromServer());
    }
    IEnumerator DownloadImageFromServer()
    {
        string path = UI_Main.UIM.rivalAvatar;
        
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path);
        gameApi.request.setHeader(uwr);
        yield return uwr.SendWebRequest();
        UI_Main.UIM.rival_img_texture = DownloadHandlerTexture.GetContent(uwr);
        Transform PlayerVS = UI_Main.UIM.matchingPage.GetChild(0).GetChild(1);
        matchResult = true;

        UI_Main.UIM.SetProfileImage(UI_Main.UIM.playerX);
        UI_Main.UIM.matchingPage.GetChild(2).gameObject.SetActive(false);

        yield return new WaitForSeconds(2.0f);
        PlayerVS.GetChild(0).GetChild(0).GetComponent<Animation>().Play();
        PlayerVS.GetChild(1).GetChild(0).GetComponent<Animation>().Play();
        yield return new WaitForSeconds(1.0f);
        PlayerVS.GetChild(2).gameObject.SetActive(true);
        PlayerVS.GetChild(2).GetComponent<Animation>().Play();
        yield return new WaitForSeconds(1.0f);

        var playRoom = new SocketChannelName();
        playRoom.roomId = UI_Main.UIM.roomId;
        playRoom.player1 = UI_Main.UIM.playerId;

        OpenGameGround();
    }
    private void OpenGameGround()
    {
        UI_Main.UIM.main.GetChild(UI_Main.UIM.selectedMap).gameObject.SetActive(true);
        Transform PlayerVS = UI_Main.UIM.matchingPage.GetChild(0).GetChild(1);
        PlayerVS.GetChild(0).GetChild(0).GetComponent<RectTransform>().anchorMin = new Vector2(0.1733846f, 0.2975f);
        PlayerVS.GetChild(0).GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2(0.8376154f, 0.8146428f);
        PlayerVS.GetChild(1).GetChild(0).GetComponent<RectTransform>().anchorMin = new Vector2(0.1733846f, 0.2975f);
        PlayerVS.GetChild(1).GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2(0.8376154f, 0.8146428f);
        PlayerVS.GetChild(0).gameObject.SetActive(false);
        PlayerVS.GetChild(1).gameObject.SetActive(false);
        PlayerVS.GetChild(2).gameObject.SetActive(false);
        UI_Main.UIM.matchingPage.GetChild(1).GetComponent<Button>().interactable = true;
        int childCount = UI_Main.UIM.main.GetChild(0).childCount;

        for (int i = 0; i < childCount-1; i++)
        {
            UI_Main.UIM.main.GetChild(0).GetChild(i).gameObject.SetActive(false);
        }

        if (UI_Main.UIM.matchFromWhere == gamePropertySettings.FROM_TOURNAMENT_ROOM)
        {
            UI_Main.UIM.leaderBoardPage.GetChild(3).gameObject.SetActive(true);
            UI_Main.UIM.leaderBoardPage.GetChild(4).gameObject.SetActive(false);
            UI_Main.UIM.leaderBoardPage.gameObject.SetActive(true);
        }
        else if (UI_Main.UIM.matchFromWhere == gamePropertySettings.FROM_NORMAL_ROOM)
        {
            UI_Main.UIM.gameMenuPage.GetChild(0).gameObject.SetActive(true);
            UI_Main.UIM.gameMenuPage.GetChild(1).gameObject.SetActive(false);
            UI_Main.UIM.gameMenuPage.gameObject.SetActive(true);
        }

        UI_Main.UIM.main.GetChild(0).gameObject.SetActive(false);

        if (UI_Main.UIM.main.GetChild(UI_Main.UIM.selectedMap).childCount < 1)
        {
            UI_Main.UIM.SetGameObjectParent(UI_Main.UIM.main.GetChild(UI_Main.UIM.selectedMap));
        }
        startTime = DateTime.Now;
        turnLimitTime = DateTime.Now;
    }

    /**
     * 
     *  enter room.
     * 
     **/
    public void JoinRoom(string roomId, string playerId)
    {
        UI_Main.UIM.matchingPage.GetChild(2).gameObject.SetActive(true);
        UI_Main.UIM.matchingPage.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
        UI_Main.UIM.matchingPage.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(false);
        UI_Main.UIM.matchingPage.GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(false);

        var jr = new JoinRoomData();
        jr.roomId = roomId;
        jr.playerId = playerId;
        jr.avatar = UI_Main.UIM.playerAvatar;
        jr.country = UI_Main.UIM.playerCountry;
        jr.name = UI_Main.UIM.playerName;
        jr.score = UI_Main.UIM.playerScore;

        StartCoroutine(DelayToJoin(jr));
    }

    IEnumerator DelayToJoin(JoinRoomData jr)
    {
        while (socket == null)
        {
            yield return new WaitForSeconds(0.5f);
        }
        socket.emit("join-room", JsonUtility.ToJson(jr));
        isMatchedNothing = false;
        HideNoMatchAlert();
        matchResult = false;
        StartCoroutine(MatchingCountTime(0));
    }

    float step = 0.5f;
    IEnumerator MatchingCountTime(float countTime)
    {
        while (!matchResult)
        {
            countTime += step;
            if (countTime < 30.0f)
            {
                yield return new WaitForSeconds(step);
            }
            else
            {
                if (socket == null)
                {
                    Debug.Log("Socket is Null");
                }
                NoMatchedAndLeaveRoom(0);
                yield break;
            }
        }
    }
    private void HideNoMatchAlert()
    {
        if (UI_Main.UIM.matchingPage.GetChild(3).gameObject.activeSelf)
        {
            UI_Main.UIM.matchingPage.GetChild(3).gameObject.SetActive(false);
        }
    }
    
    /**
     * 
     *  restart game    // to be removed.
     *  
     **/
    public void EndGame(string roomId, string winner)
    {
        SocketChannelName s_c_name = new SocketChannelName();
        s_c_name.roomId = roomId;
        s_c_name.player1 = winner;
        socket.emit("end-game", JsonUtility.ToJson(s_c_name));
    }

    /**
     * 
     *  Leave Room by Back event or No Matched 
     * 
     * **/
    public void NoMatchedAndLeaveRoom(int fromWhere)
    {
        var room = new RoomId();
        room.roomId = UI_Main.UIM.roomId;
        if (fromWhere == 1)
        {
            socket.emit("leave", JsonUtility.ToJson(room));
        }
        else
        {
            socket.emit("no-match-found", JsonUtility.ToJson(room));
        }
    }

    /**
     * 
     *   change player's turn
     * 
     * **/
    public void PlayerTurn(string room, string player, int originPos, int newPos, int moveCount)
    {
        for (int i = 0; i < 9; i++)
        {
            UI_PlayGround.UIP.playGroundTrans.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }
        gameMode.GM.InitializeAllScale();
        var s_data = new TurnSocket();
        s_data.roomId = room;
        s_data.currPlayer = player;
        if (player == UI_Main.UIM.playerId)
        {
            s_data.nextPlayer = UI_Main.UIM.rivalPlayer;
        } 
        else
        {
            s_data.nextPlayer = UI_Main.UIM.playerId;
        }
        s_data.startPos = originPos;
        s_data.endPos = newPos;
        s_data.moveCount = moveCount;
        socket.emit("turn-player", JsonUtility.ToJson(s_data));
    }

    /**
     * 
     * 
     * 
     * **/
    public void SaveHistory(string roomId)
    {
        SocketChannelName room = new SocketChannelName();
        room.roomId = roomId;
        room.player1 = UI_Main.UIM.playerId;
    }

    public void CancelSave(string roomId)
    {
        var room = new M_History();
        room.roomId = roomId;
        room.player1 = UI_Main.UIM.playerId;
    }

    [Serializable]
    public class JoinRoomData
    {
        public string roomId;
        public string playerId;
        public string avatar;
        public string country;
        public string name;
        public string score;
        public string rivalId;
        public string turnPlayer;
    }

    [Serializable]
    public class MatchUserData
    {
        public UserData player1;
        public UserData player2;
        public string roomId;
        public string turnPlayer;
    }

    [Serializable]
    public class UserData
    {
        public string userId;
        public string name;
        public string avatar;
        public string score;
        public string country;
        public bool isJoined;
        public string userSocketId;
    }

    [Serializable]
    public class SocketChannelName
    {
        public string roomId;
        public string player1;
        public string player2;
    }

    [Serializable]
    public class TurnSocket
    {
        public string roomId;
        public string currPlayer;
        public string nextPlayer;
        public int startPos;
        public int endPos;
        public int moveCount;
        //public string posHistory;
        public List<string> posHistory;
    }

    [Serializable]
    public class NextTurn
    {
        public string newPlayer;
        public string oldPlayer;
        public string startPos;
        public string endPos;
        public string moveCount;
        //public string moveHistory;
        public List<string> moveHistory;
    }

    [Serializable]
    public class M_History
    {
        public List<string> moveHistory;
        public string player1;
        public string roomId;
    }

    [Serializable]
    public class TurnUser
    {
        public string turnPlayer;
    }

    [Serializable]
    public class RoomId
    {
        public string roomId;
    }

    [Serializable]
    public class Userinfo
    {
        public string name;
        public string avatar;
        public string score;
        public string country;
    }

}
