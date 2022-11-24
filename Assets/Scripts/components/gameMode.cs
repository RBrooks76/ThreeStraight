using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameMode : MonoBehaviour
{
    static public gameMode GM;
    public Transform self;
    public Transform movingTrans;
    public bool isTileMovable = false;
    public bool isTouchPosContain = false;
    public int ME_result = 0;
    public int RIVAL_result = 0;
    public int origin_pos = -1;
    public int new_pos = -1;
    Dictionary<string, List<int>> pos_dic = new Dictionary<string, List<int>>();
    List<int> emptyPos = new List<int>();
    List<int> hu_posList = new List<int>();
    List<int> ai_posList = new List<int>();
    List<int> completePosList = new List<int>();
    List<int> tilePosHistory = new List<int>();
    int clickCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (GM == null) GM = this;
        ThreeStraight.Instance.Init();
        socketApi.instance.turnLimitTime = DateTime.Now;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (socketApi.instance.isResWhoTurn)
        {
            socketApi.instance.isResWhoTurn = false;
                if (socketApi.instance.multiple_startPos > -1 && socketApi.instance.multiple_endPos > -1)
                {
                    //if rival move the tile
                    if (self.GetChild(socketApi.instance.multiple_endPos).childCount == 1)
                    {
                        // moving tile
                        movingTile(self.GetChild(socketApi.instance.multiple_startPos), self.GetChild(socketApi.instance.multiple_endPos), gamePropertySettings.RIVAL_MOVING_TURN, socketApi.instance.multiple_startPos, socketApi.instance.multiple_endPos);
                    }
                    else
                    {
                        MultipleCheckResultAndTurn();
                    }
                }
                else if (socketApi.instance.multiple_startPos > -1 && socketApi.instance.multiple_endPos == -1)
                {
                    // if rival put the tile.
                    if (self.GetChild(socketApi.instance.multiple_startPos).childCount == 1)
                    {
                        Transform parent = self.GetChild(socketApi.instance.multiple_startPos);

                        GameObject go = Instantiate(Resources.Load<GameObject>(gamePropertySettings._objUrl));
                        go.transform.SetParent(parent);
                        go.transform.localScale = Vector2.one;
                        go.transform.localPosition = Vector2.zero;
                        go.GetComponent<RectTransform>().sizeDelta = parent.GetComponent<RectTransform>().sizeDelta;
                        self.GetChild(socketApi.instance.multiple_startPos).GetChild(1).GetComponent<tileController>().player = socketApi.instance.oldPlayer;
                        if (UI_PlayGround.UIP.myProfilePos == 1)
                        {
                            self.GetChild(socketApi.instance.multiple_startPos).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/Green@2x");
                        }
                        else
                        {
                            self.GetChild(socketApi.instance.multiple_startPos).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/Red@2x");
                        }
                    }
                    MultipleCheckResultAndTurn();
                }
                else if (socketApi.instance.multiple_startPos == -1 && socketApi.instance.multiple_endPos == -1)
                {
                    MultipleCheckResultAndTurn();
                }
        }

        if (!UI_PlayGround.UIP.isGameFinished)
        {
            if (Input.GetMouseButtonDown(0)) 
            { 
                if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_MATCH)
                {
                    // multi-play with socket.
                    if (UI_Main.UIM.playerX == UI_Main.UIM.playerId && UI_Main.UIM.isMyTurn)
                    {
                        Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        for (int i = 0; i < 9; i++)
                        {
                            if (RectTransformUtility.RectangleContainsScreenPoint(self.GetChild(i).GetComponent<RectTransform>(), touchPos))
                            {
                                CheckTileCreate(i);
                            }
                        }
                    }
                }
                else if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_SINGLE)
                {
                    // when playing with ai or offline mode.
                    if (UI_Main.UIM.playerX == gamePropertySettings.PLAYER_SELF)
                    {
                        Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        for (int i = 0; i < 9; i++)
                        {
                            if (RectTransformUtility.RectangleContainsScreenPoint(self.GetChild(i).GetComponent<RectTransform>(), touchPos))
                            {
                                CheckTileCreate(i);
                            }
                        }
                    }
                }
                else if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_OFFLINE)
                {
                    Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    for (int i = 0; i < 9; i++)
                    {
                        if (RectTransformUtility.RectangleContainsScreenPoint(self.GetChild(i).GetComponent<RectTransform>(), touchPos))
                        {
                            CheckTileCreate(i);
                        }
                    }
                }
            }
        }
    }

    void MultipleCheckResultAndTurn()
    {
        // check whether the game is finished or not.
        UI_PlayGround.UIP.winPlayer = GameComplete(UI_Main.UIM.playerId);
        if (string.IsNullOrEmpty(UI_PlayGround.UIP.winPlayer))
        {
            socketApi.instance.multiple_startPos = -1;
            socketApi.instance.multiple_endPos = -1;
            UI_Main.UIM.playerX = socketApi.instance.newPlayer;
            UI_Main.UIM.isMyTurn = true;
        }
        else
        {
            socketApi.instance.EndGame(UI_Main.UIM.roomId, UI_PlayGround.UIP.winPlayer);
            GameFinishAnimation(UI_PlayGround.UIP.winPlayer);
        }
    }

    /**
     * 
     *  check tile create.
     * 
     * 
     * **/
    private void CheckTileCreate(int pos)
    {
        clickCount++;
        tilePosHistory.Add(pos);
        UI_PlayGround.UIP.player_tileCount = CheckMyTileCount(UI_Main.UIM.playerX);

        if (UI_PlayGround.UIP.player_tileCount > 2)
        {
            // exceed tile count in default. then we should move tile to other position.
            if (clickCount == 2)
            {
                int newPos = -1;
                if (tilePosHistory.Count > 1)
                {
                    newPos = tilePosHistory[1];
                }
                if (self.GetChild(newPos).childCount > 1)
                {
                    InitializeClickCount(tilePosHistory[0]);
                }
                else
                {
                    if (self.GetChild(tilePosHistory[0]).childCount > 1)
                    {
                        if (self.GetChild(tilePosHistory[0]).GetChild(1).GetComponent<tileController>().player == UI_Main.UIM.playerX)
                        {
                            CheckMovableTile(tilePosHistory[0], tilePosHistory[1]);
                        }
                        else
                        {
                            InitializeClickCount(tilePosHistory[0]);
                        }
                    }
                    else
                    {
                        InitializeClickCount(tilePosHistory[0]);
                    }
                }
            }
            else
            {
                if (self.GetChild(pos).childCount > 1)
                {
                    if (self.GetChild(tilePosHistory[0]).GetChild(1).GetComponent<tileController>().player == UI_Main.UIM.playerX)
                    {
                        self.GetChild(tilePosHistory[0]).GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1.2f);
                        FindMovablePos(pos, ThreeStraight.Instance.GetMovableListFromDictionary(pos));
                    }
                    else
                    {
                        InitializeClickCount(tilePosHistory[0]);
                    }
                }
                else
                {
                    HideMovablePos();
                }
            }
        }
        else
        {
            // create new tile.
            if (self.GetChild(pos).childCount == 1)
            {
                Transform parent = self.GetChild(pos);
                GameObject go = Instantiate(Resources.Load<GameObject>(gamePropertySettings._objUrl));
                go.transform.SetParent(parent);
                go.transform.localScale = Vector2.one;
                go.transform.localPosition = Vector2.zero;
                go.GetComponent<RectTransform>().sizeDelta = parent.GetComponent<RectTransform>().sizeDelta;
                self.GetChild(pos).GetChild(1).GetComponent<tileController>().player = UI_Main.UIM.playerX;

                if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_MATCH)
                {
                    if (UI_PlayGround.UIP.myProfilePos == 1)
                    {
                        self.GetChild(pos).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/Red@2x");
                    }
                    else
                    {
                        self.GetChild(pos).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/Green@2x");
                    }
                    UI_PlayGround.UIP.move++;
                    UI_Main.UIM.isMyTurn = false;
                    socketApi.instance.PlayerTurn(UI_Main.UIM.roomId, UI_Main.UIM.playerId, pos, -1, 1);
                }
                else //if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_SINGLE)
                {
                    if (UI_Main.UIM.playerX == gamePropertySettings.PLAYER_SELF)
                    {
                        self.GetChild(pos).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/Red@2x");
                    }
                    else
                    {
                        self.GetChild(pos).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/Green@2x");
                    }

                    if (UI_Main.UIM.playerX == gamePropertySettings.PLAYER_SELF)
                    {
                        UI_PlayGround.UIP.move++;
                    }
                    UI_PlayGround.UIP.ai_total_move++;

                    // check game result.
                    UI_PlayGround.UIP.winPlayer = GameComplete(UI_Main.UIM.playerX);
                    // turn player
                    if (string.IsNullOrEmpty(UI_PlayGround.UIP.winPlayer))
                    {
                        UI_Main.UIM.PlaySound("Card_Tap");
                        if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_SINGLE)
                        {
                            AICreateTile();
                        }
                        else
                        {
                            TurnPlayer(UI_Main.UIM.playerX);
                        }
                    }
                    else
                    {
                        GameFinishAnimation(UI_PlayGround.UIP.winPlayer);
                    }
                }
                
                socketApi.instance.turnLimitTime = DateTime.Now;
                clickCount = 0;
                tilePosHistory.Clear();
            }
            else
            {
                ApplyVibrate();
            }
        }
    }

    private void InitializeClickCount(int originPos)
    {
        if (self.GetChild(originPos).childCount > 1)
        {
            self.GetChild(originPos).GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        tilePosHistory.Clear();   // initialize selected tile.
        clickCount = 0;
        ApplyVibrate();   // apply vibrate.
        HideMovablePos();
    }

    // return all position of tiles.
    private string CheckTilePos()
    {
        string mine = UI_Main.UIM.playerId;
        string rival = UI_Main.UIM.rivalPlayer;
        string myPosHistory = mine + "-";
        string rivalPosHistory = rival + "-";
        int myTileIndex = 0;
        int rivalTileIndex = 0;
        for (int i = 0; i < 9; i++)
        {
            if (self.GetChild(i).childCount > 1)
            {
                if (self.GetChild(i).GetChild(1).GetComponent<tileController>().player == mine)
                {
                    if (myTileIndex == 0)
                    {
                        myPosHistory = myPosHistory + i.ToString();
                    }
                    else
                    {
                        myPosHistory += "-" + i.ToString();
                    }
                    myTileIndex++;
                }
                else if (self.GetChild(i).GetChild(1).GetComponent<tileController>().player == rival)
                {
                    if (rivalTileIndex == 0)
                    {
                        rivalPosHistory = rivalPosHistory + i.ToString();
                    }
                    else
                    {
                        rivalPosHistory += "-" + i.ToString();
                    }
                    rivalTileIndex++;
                }
            }
        }
        return myPosHistory + ":" + rivalPosHistory;
    }
    private void ApplyVibrate()
    {
        if (UI_Main.UIM.isVibration)
        {
            #if UNITY_ANDROID || UNITY_IOS
                Handheld.Vibrate();
            #endif
        }
    }

    /**
     * 
     *  checking whether game is completed or not.
     *  @param {String} player : me or rival
     * 
     *  @return string : Me or Rival.
     * **/
    private string GameComplete(string player)
    {
        string whoWin = string.Empty;
        int length = ThreeStraight.Instance.winCombos.Count;
        List<int> completeList = new List<int>();
        for (int i = 0; i < length; i++)
        {
            completeList = ThreeStraight.Instance.GetListFromDictionary(i);
            whoWin = CheckGameComplete(player, completeList);
            if (!string.IsNullOrEmpty(whoWin))
                return whoWin;
        }
        return string.Empty;
    }
    private string CheckGameComplete(string player, List<int> completeList)
    {
        ME_result = 0;
        RIVAL_result = 0;
        for (int j = 0; j < completeList.Count; j++)
        {
            if (self.GetChild(completeList[j]).childCount > 1)
            {
                if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_MATCH)
                {
                    if (self.GetChild(completeList[j]).GetChild(1).GetComponent<tileController>().player == player)
                    {
                        ME_result++;
                    }
                    else
                    {
                        RIVAL_result++;
                    }
                }
                else //if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_SINGLE) || PLAY_MODE_OFFLINE
                {
                    if (self.GetChild(completeList[j]).GetChild(1).GetComponent<tileController>().player == player)
                    {
                        if (player == gamePropertySettings.PLAYER_SELF)
                            ME_result++;
                        else
                            RIVAL_result++;
                    }
                }
            }

        }
        if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_MATCH)
        {
            if (ME_result == 3)
                return player;
            else if (RIVAL_result == 3)
                return gamePropertySettings.PLAYER_RIVAL;
        }
        else  //if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_SINGLE) || gamePropertySettings.PLAY_MODE_OFFLINE
        {
            if (ME_result == 3 || RIVAL_result == 3)
            {
                return player;
            }
        }
        return string.Empty;
    }
    public void TurnPlayer(string flag)
    {
        if (flag == gamePropertySettings.PLAYER_SELF)
        {
            UI_Main.UIM.playerX = gamePropertySettings.PLAYER_RIVAL;
        }
        else
        {
            UI_Main.UIM.playerX = gamePropertySettings.PLAYER_SELF;
        }
    }
    private void HideMovablePos()
    {
        for (int i = 0; i < 9; i++)
        {
            self.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }
    }

    /**
     * 
     *  Check movable Tile
     * 
     * **/
    private void CheckMovableTile(int originPos, int pos)
    {
        // check whether touchup position is available to put or not.
        List<int> availablePos = ThreeStraight.Instance.GetMovableListFromDictionary(originPos);
        bool isOk = CheckMovablePos(pos, availablePos);
        if (isOk)
        {
            HideMovablePos();
            // moving tile
            if (self.GetChild(originPos).childCount > 1)
            {
                self.GetChild(originPos).GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            movingTile(self.GetChild(originPos), self.GetChild(pos), gamePropertySettings.MY_MOVING_TURN, originPos, pos);
        }
        else
        {
            InitializeClickCount(originPos);
        }
    }
    private void AnimationAction(int originPos, int pos)
    {
        isTouchPosContain = false;

        if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_MATCH)
        {
            UI_PlayGround.UIP.move++;
            UI_Main.UIM.isMyTurn = false;
            UI_Main.UIM.PlaySound("Card_Tap");
            socketApi.instance.PlayerTurn(UI_Main.UIM.roomId, UI_Main.UIM.playerId, originPos, pos, 1);
        }
        else  //if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_SINGLE) || PLAY_MODE_OFFLINE
        {
            if (UI_Main.UIM.playerX == gamePropertySettings.PLAYER_SELF)
            {
                UI_PlayGround.UIP.move++;
            }
            UI_PlayGround.UIP.ai_total_move++;

            // check whether the game is finished or not.
            UI_PlayGround.UIP.winPlayer = GameComplete(UI_Main.UIM.playerX);
            if (string.IsNullOrEmpty(UI_PlayGround.UIP.winPlayer))
            {
                UI_Main.UIM.PlaySound("Card_Tap");
                if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_SINGLE)
                {
                    AICreateTile();
                }
                else
                {
                    TurnPlayer(UI_Main.UIM.playerX);
                }
            }
            else
            {
                GameFinishAnimation(UI_PlayGround.UIP.winPlayer);
            }
        }
        socketApi.instance.turnLimitTime = DateTime.Now;
        tilePosHistory.Clear();
        clickCount = 0;
    }
    private void FindMovablePos(int pos, List<int>movablePos)
    {     
        int posLength = movablePos.Count;
        List<int> posList = new List<int>();
        for (int i = 0; i < posLength; i++)
        {
            if (self.GetChild(movablePos[i]).childCount == 1)
            {
                self.GetChild(movablePos[i]).GetChild(0).gameObject.SetActive(true);
            }
        }
    }
    private bool CheckMovablePos(int pos, List<int> posList)
    {
        int length = posList.Count;
        for (int i = 0; i < length; i++)
        {
            if (pos == posList[i])
            {
                return true;
            }
        }
        return false;
    }

    private void CalcTileCount()
    {
        if (UI_PlayGround.UIP.tileCount < 6)
        {
            UI_PlayGround.UIP.tileCount = 0;
            for (int i = 0; i < 9; i++)
            {
                if (self.GetChild(i).childCount > 1)
                {
                    UI_PlayGround.UIP.tileCount++;
                }
                else
                {
                    emptyPos.Add(i);
                }
            }
        }
    }
    private int CheckMyTileCount(string player)
    {
        int myTiles = 0;
        for (int i = 0; i < 9; i++)
        {
            if (self.GetChild(i).childCount > 1)
            {
                if (self.GetChild(i).GetChild(1).GetComponent<tileController>().player == player)
                {
                    myTiles++;
                }
            }
        }
        return myTiles;
    }
    private void SetPositionDictionary()
    {
        pos_dic.Clear();
        hu_posList.Clear();
        ai_posList.Clear();
        for (int i = 0; i < 9; i++)
        {
            if (self.GetChild(i).childCount > 1)
            {
                if (self.GetChild(i).GetChild(1).GetComponent<tileController>().player == gamePropertySettings.PLAYER_SELF)
                {
                    hu_posList.Add(i);
                }
                else if (self.GetChild(i).GetChild(1).GetComponent<tileController>().player == gamePropertySettings.PLAYER_RIVAL)
                {
                    ai_posList.Add(i);
                }
            }
        }
        pos_dic.Add(gamePropertySettings.PLAYER_SELF, hu_posList);
        pos_dic.Add(gamePropertySettings.PLAYER_RIVAL, ai_posList);
    }

    // -------------------------------------------------  ai player. -----------------------------------------------------------------
    public void InitializeAllScale()
    {
        for (int i = 0; i < 9; i++)
        {
            if (self.GetChild(i).childCount > 1)
            {
                self.GetChild(i).GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }
    }
    public void AICreateTile()
    {
        TurnPlayer(UI_Main.UIM.playerX);
        InitializeAllScale();
        StartCoroutine(AIOrder());
    }
    IEnumerator AIOrder()
    {
        yield return new WaitForSeconds(0.5f);
        SetPositionDictionary();
        UI_PlayGround.UIP.player_tileCount = CheckMyTileCount(gamePropertySettings.PLAYER_RIVAL);
        if (UI_PlayGround.UIP.player_tileCount > 2)
        {
            Possible_spot best = new Possible_spot();
            best = Minimax(pos_dic[gamePropertySettings.PLAYER_SELF], pos_dic[gamePropertySettings.PLAYER_RIVAL]).best_move;
            movingTile(self.GetChild(best.key), self.GetChild(best.moveablePos), gamePropertySettings.AI_MOVING_TURN, best.key, best.moveablePos);
        }
        else
        {
            if (UI_PlayGround.UIP.player_tileCount == 0)
            {
                randomIndex();
            }
            else
            {
                checkAIPos();
            }
        }
        yield break;
    }
    private void AI_AnimationAction()
    {
        UI_PlayGround.UIP.winPlayer = GameComplete(UI_Main.UIM.playerX);
        UI_PlayGround.UIP.ai_total_move++;
        // turn player
        if (string.IsNullOrEmpty(UI_PlayGround.UIP.winPlayer))
        {
            UI_Main.UIM.PlaySound("Card_Tap");
            TurnPlayer(UI_Main.UIM.playerX);
            UI_PlayGround.UIP.currMiliseconds = 0;
        }
        else
        {
            GameFinishAnimation(UI_PlayGround.UIP.winPlayer);
        }
        socketApi.instance.turnLimitTime = DateTime.Now;
        UI_PlayGround.UIP.isAiTurn = false;
    }

    // choose random number for tile index.
    private void randomIndex()
    {
        int random = UnityEngine.Random.Range(0, 8);
        if (self.GetChild(random).childCount > 1)
        {
            randomIndex();
        }
        else
        {   
            if (random %2 == 0)
            {
                AllocateTile(random);
            }
            else
            {
                randomIndex();
            }
        }
    }

    // ai creates tile on defined index.
    private void AllocateTile(int pos)
    {
        Transform parent = self.GetChild(pos);
        GameObject go = Instantiate(Resources.Load<GameObject>(gamePropertySettings._objUrl));
        go.transform.SetParent(parent);
        go.transform.localScale = Vector2.one;
        go.transform.localPosition = Vector2.zero;
        go.GetComponent<RectTransform>().sizeDelta = parent.GetComponent<RectTransform>().sizeDelta;
        self.GetChild(pos).GetChild(1).GetComponent<tileController>().player = UI_Main.UIM.playerX;
        self.GetChild(pos).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/Green@2x");
        UI_PlayGround.UIP.ai_total_move++;
        UI_PlayGround.UIP.winPlayer = GameComplete(UI_Main.UIM.playerX);
        // turn player
        if (string.IsNullOrEmpty(UI_PlayGround.UIP.winPlayer))
        {
            UI_Main.UIM.PlaySound("Card_Tap");
            TurnPlayer(UI_Main.UIM.playerX);
            UI_PlayGround.UIP.currMiliseconds = 0;
        }
        else
        {
            GameFinishAnimation(UI_PlayGround.UIP.winPlayer);
        }
        socketApi.instance.turnLimitTime = DateTime.Now;
        UI_PlayGround.UIP.isAiTurn = false;
    }

    // check position where ai gonna put a tile on.
    List<int> compList = new List<int>();
    private void checkAIPos()
    {
        int ai_result = -1;
        int hu_result = -1;
        int length = ThreeStraight.Instance.winCombos.Count;
        // check ai status.
        for (int i = 0; i < length; i++)
        {
            compList = ThreeStraight.Instance.GetListFromDictionary(i);
            if (ai_result == -1)
            {
                ai_result = CalcAIResult(compList);
            }
        }
        // if ai have a chance to complete.
        if (ai_result > -1)
        {
            AllocateTile(ai_result);
        }
        else
        {
            // check human status
            for (int i = 0; i < length; i++)
            {
                compList = ThreeStraight.Instance.GetListFromDictionary(i);
                if (hu_result == -1)
                {
                    hu_result = CalcHumanResult(compList);
                }
            }
            if (hu_result > -1)
            {
                AllocateTile(hu_result);
            }
            else
            {
                randomIndex();
            }
        }
    }
    int CalcAIResult(List<int> list)
    {
        int ai_result = 0;
        int rightPos = -1;

        for (int i = 0; i < list.Count; i++)
        {
            if (pos_dic[gamePropertySettings.PLAYER_RIVAL].Contains(list[i]))
            {
                ai_result++;
            }
            else
            {
                if (self.GetChild(list[i]).childCount == 1)
                {
                    rightPos = list[i];
                }
            }
        }
        if (ai_result == 2 && rightPos > -1)
        {
            return rightPos;
        }
        return -1;
    }
    int CalcHumanResult(List<int> list)
    {
        int hu_result = 0;
        int rightPos = -1;

        for (int i = 0; i < list.Count; i++)
        {
            if (pos_dic[gamePropertySettings.PLAYER_SELF].Contains(list[i]))
            {
                hu_result++;
            }
            else
            {
                if (self.GetChild(list[i]).childCount == 1)
                {
                    rightPos = list[i];
                }
            }
        }
        if (hu_result == 2 && rightPos > -1)
        {
            return rightPos;
        }
        return -1;
    }

    int DEPTH = 7;
    Possible_spot best_move = new Possible_spot();
    MiniMaxReturnValue Minimax(List<int> playerPos, List<int> aiPos, int depth= 7, float alpha= -Mathf.Infinity, float beta= Mathf.Infinity, bool maximizingPlayer = true)
    { 
    
        //  Minimax algorithm with alpha-beta pruning.
        // :param player_1_board: containing board information of player_1
        // :param player_2_board: containing board information of player_2
        // :param depth: the depth of the algorithm.
        // :param alpha: alpha of alpha-beta pruning
        // :param beta: beta of alpha-beta pruning
        // :param maximizingPlayer: if the player is maximizing player or minimizing player
        // :return: tuple of length two, first giving the value of the state and second giving the steps to take

        MiniMaxReturnValue returnVal = new MiniMaxReturnValue();
        returnVal.best_move = new Possible_spot();
        if (depth == 0 || is_game_over(playerPos, aiPos) > 0)
        {  
            if (is_game_over(playerPos, aiPos) == 2)
            {
                returnVal.value = 1000 + depth;
                return returnVal;
            }
            else if (is_game_over(playerPos, aiPos) == 1)
            {
                returnVal.value = -1000 - depth;
                return returnVal;
            }
            returnVal.value = 0;
            return returnVal;
        }

        List<Possible_spot> possible_drags = new List<Possible_spot>();
        if (maximizingPlayer)
        {
            float value = -Mathf.Infinity;
            possible_drags = get_possible_drags(playerPos, aiPos, 2);
            for(int i = 0; i < possible_drags.Count; i++)
            {
                aiPos[aiPos.IndexOf(possible_drags[i].key)] = possible_drags[i].moveablePos;
                float value_t = Minimax(playerPos, aiPos, depth - 1, alpha, beta, false).value;
                if (value_t > value)
                {
                    value = value_t;
                    if (depth == DEPTH)
                    {
                        best_move = possible_drags[i];
                    }
                    if (alpha < value)
                    {
                        alpha = value;
                    }
                }
                aiPos[aiPos.IndexOf(possible_drags[i].moveablePos)] = possible_drags[i].key;
                if (alpha >= beta)
                    break;
            }
            returnVal.value = value;
            returnVal.best_move = best_move;
            return returnVal;
        }
        else
        {
            float value = Mathf.Infinity;
            possible_drags = get_possible_drags(playerPos, aiPos, 1);
            for (int i = 0; i < possible_drags.Count; i++)
            {
                playerPos[playerPos.IndexOf(possible_drags[i].key)] = possible_drags[i].moveablePos;
                float value_t = Minimax(playerPos, aiPos, depth - 1, alpha, beta, true).value;
                if (value_t < value)
                {
                    value = value_t;
                    if (depth == DEPTH)
                    {
                        best_move = possible_drags[i];
                    }
                    if (beta < value)
                    {
                        beta = value;
                    }
                }
                playerPos[playerPos.IndexOf(possible_drags[i].moveablePos)] = possible_drags[i].key;
                if (alpha >= beta)
                    break;
            }
            returnVal.value = value;
            returnVal.best_move = best_move;
            return returnVal;
        }
    }
    int is_game_over(List<int> playerPos, List<int> aiPos)
    {
        bool isVictory = false;
        int length = ThreeStraight.Instance.winCombos.Count;
        List<int> completeList = new List<int>();
        for (int i = 0; i < length; i++)
        {
            completeList = ThreeStraight.Instance.GetListFromDictionary(i);
            isVictory = check_game_over(completeList, aiPos);
            if (isVictory)
            {
                return 2;
            }
        }
        for (int i = 0; i < length; i++)
        {
            completeList = ThreeStraight.Instance.GetListFromDictionary(i);
            isVictory = check_game_over(completeList, playerPos);
            if (isVictory)
            {
                return 1;
            }
        }
        return 0;
    }
    private bool check_game_over(List<int> list, List<int> currPosList)
    {
        int result = 0;
        for (int i = 0; i <list.Count; i++)
        {
            if (currPosList.Contains(list[i]))
            {
                result++;
            }
            else
            {
                return false;
            }
        }
        if (result == 3)
            return true;
        return false;
    }

    // get movable positions
    List<Possible_spot> get_possible_drags(List<int> playerPos, List<int> aiPos, int who)
    {
        List<Possible_spot> _possible_drags = new List<Possible_spot>();        
        List<int> curr_player_board = new List<int>();
        if (who == 1)
        {
            curr_player_board = playerPos;
        }
        else
        {
            curr_player_board = aiPos;
        }
        for (int i = 0; i < curr_player_board.Count; i++)
        {
            List<int> pos_list = ThreeStraight.Instance.GetMovableListFromDictionary(curr_player_board[i]);
            for (int j = 0; j < pos_list.Count; j++)
            {
                if (!playerPos.Contains(pos_list[j]) && !aiPos.Contains(pos_list[j]))
                {
                    Possible_spot empty_pos = new Possible_spot();
                    empty_pos.key = curr_player_board[i];
                    empty_pos.moveablePos = pos_list[j];
                    _possible_drags.Add(empty_pos);
                }
            }
        }
        return _possible_drags;
    }
    public void GameFinishAnimation(string player)
    {
        StartCoroutine(DispVictoryAlert(player));
    }
    IEnumerator DispVictoryAlert(string player)
    {
        for (int i = 0; i < 9; i++)
        {
            if (self.GetChild(i).childCount > 1)
            {
                if (self.GetChild(i).GetChild(1).GetComponent<tileController>().player == player)
                {
                    completePosList.Add(i);
                }
            }
        }
        for (int i = 0; i < completePosList.Count; i++)
        {
            self.GetChild(completePosList[i]).GetChild(1).GetComponent<Animation>().Play();
        }
        yield return new WaitForSeconds(2.0f);
        completePosList.Clear();
        UI_PlayGround.UIP.isGameFinished = true;
        UI_PlayGround.UIP.isStartGame = true;
        yield break;
    }

    /**
     * 
     *  Tile moving animationm
     * 
     * **/
    public void movingTile(Transform fromPosition, Transform toPosition, string whoTurn, int originPos, int pos)
    {
        StartCoroutine(movingAnimation(fromPosition, toPosition, whoTurn, originPos, pos));
    }

    bool isStoneMoving = false;
 
    IEnumerator movingAnimation(Transform fromPos, Transform toPosition, string whoTurn, int originPos, int pos)
    {
        Debug.Log("fromPos Child Count ===>" + originPos);
        Transform fromPosition = fromPos.GetChild(1);
        Vector3 startPos = fromPosition.position;
        Vector3 toPos = toPosition.position;
        startPos = new Vector3(startPos.x, startPos.y, 0.0f);
        toPos = new Vector3(toPos.x, toPos.y, 0.0f);
        float dist = Vector3.Distance(startPos, toPos);
        float counter = 0;
        float duration = dist / 12.0f;

        fromPosition.SetParent(toPosition);
        while (counter < (duration + 0.15f))
        {
            counter += Time.deltaTime;
            fromPosition.position = Vector3.Lerp(startPos, toPos, counter / duration);
            yield return null;
        }
        if (whoTurn == gamePropertySettings.MY_MOVING_TURN)
        {
            AnimationAction(originPos, pos);
        }
        else if (whoTurn == gamePropertySettings.AI_MOVING_TURN)
        {
            if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_SINGLE)
            {
                AI_AnimationAction();
            }
            else if (UI_Main.UIM.playMode == gamePropertySettings.PLAY_MODE_OFFLINE)
            {
                AnimationAction(originPos, pos);
            }
        }
        else if (whoTurn == gamePropertySettings.RIVAL_MOVING_TURN)
        {
            MultipleCheckResultAndTurn();
        }
        yield break;
    }

    /**
     * 
     *   re-arrange new tiles
     * 
     * **/
    private void ArrangeTiles(List<string> his_list, int index)
    {
        string moveHis = string.Empty;
        for (int i = 0; i < his_list.Count-index; i++)
        {
            moveHis = his_list[i];
            string[] positions = moveHis.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            int startPos = int.Parse(positions[0]);
            int endPos = int.Parse(positions[1]);
            if (startPos > -1 && endPos > -1)
            {
                if (self.GetChild(endPos).childCount < 2 && self.GetChild(startPos).childCount > 1)
                {
                    self.GetChild(startPos).GetChild(1).SetParent(self.GetChild(endPos));
                    self.GetChild(endPos).GetChild(1).GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                }
            }
            if (startPos > -1 && endPos == -1)
            {
                if (self.GetChild(startPos).childCount < 2)
                {
                    Transform parent = self.GetChild(startPos);
                    GameObject go = Instantiate(Resources.Load<GameObject>(gamePropertySettings._objUrl));
                    go.transform.SetParent(parent);
                    go.transform.localScale = Vector2.one;
                    go.transform.localPosition = Vector2.zero;
                    go.GetComponent<RectTransform>().sizeDelta = parent.GetComponent<RectTransform>().sizeDelta;

                    self.GetChild(startPos).GetChild(1).GetComponent<tileController>().player = UI_Main.UIM.rivalPlayer;
                    if (UI_PlayGround.UIP.myProfilePos == 1)
                    {
                        self.GetChild(startPos).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/Red@2x");
                    }
                    else
                    {
                        self.GetChild(startPos).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/Green@2x");
                    }
                }
            }
        }
        MultipleCheckResultAndTurn();
    }
    
    [Serializable]
    public class Possible_spot
    {
        public int key;
        public int moveablePos;
    }

    [Serializable]
    public class MiniMaxReturnValue
    {
        public float value;
        public Possible_spot best_move;
    }
}

