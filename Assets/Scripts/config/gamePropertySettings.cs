using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gamePropertySettings : MonoBehaviour
{
    //// Local Server
    public const string LOCAL_SERVER_URL = "http://192.168.1.118:5050/";
    public const string LOCAL_SOCKET_URL = "ws://192.168.1.118:5050/";
    ////Test server
    public const string HEROKU_SERVER_URL = "http://18.206.163.36:5050/";
    public const string HEROKU_SOCKET_URL = "ws://18.206.163.36:5050/";
//    public const string HEROKU_SERVER_URL = "https://threestraight.herokuapp.com/";
//    public const string HEROKU_SOCKET_URL = "ws://threestraight.herokuapp.com/";
    //// Product Server
    public const string PROD_SERVER_URL = "http://18.206.163.36:5050/";
    public const string PROD_SOCKET_URL = "ws://18.206.163.36:5050/";

    //public const string SERVER_URL = "http://18.206.163.36:5050/";
    //public const string SOCKET_URL = "ws://18.206.163.36:5050/";

    public const string LOCAL_ENV = "local";
    public const string HEORKU_ENV = "heorku";
    public const string PROD_ENV = "product";
    public const string EMAIL_INPUT_EMPTY = "Email shouldn't be empty!";
    public const string NAME_INPUT_ERROR = "You should input your name";
    public const string EMAIL_INPUT_ERROR = "Please input a correct email address.";
    public const string PASSWORD_INPUT_ERROR = "Failed to confirm your password";
    public const string PASSWORD_INPUT_EMPTY = "Password should be entered!";
    public const string PASSWORD_LESS_CHARACTERS = "Password must be at least 8 characters.";
    public const string SELECT_AVATAR_REQUIRED = "Please select your picture or an avatar";
    public const string USER_ALREADY_EXIST = "User already exists. Please login with your account";
    public const string ACCOUNT_NOT_FOUND = "Account not found!";
    public const string LOGIN_PASSWORD_FAILED = "Password is incorrect! \n Try again.";
    public const string CHANGE_PASSWORD_FAILED = "Password reset failed. \n Please try again";
    public const string NETWORK_ERROR_TITLE = "NETWORK ERROR!";
    public const string NETWROK_ERROR_CONTENT = "Please check your network status and then try again.";
    public const string WARNNING_TITLE = "WARNNING!";
    public const string CONTACT_SUBJECT_ERROR = "Subject should be entered!";
    public const string CONTACT_CONTENT_ERROR = "Message shouldn't be empty!";
    public const string EMPTY_VERIFYCODE = "Verification code should be entered";
    public const string WRONG_VERIFYCODE = "Please input correct verification code";
    // SERVER
    public const string SERVER_ERR = "100400";
    public const string RES_USER_NOT_FOUND = "100404";
    public const string RES_USER_EXIST = "100406";
    public const string RES_PASSWORD_FAILED = "100500";
    public const string RES_CHANGE_PASSWORD_FAILED = "100501";
    public const string RES_SOCIAL_SIGNUP = "100206";
    public const string RES_SOCIAL_LOGIN = "100207";
    public const string EMAIL_LOGIN = "EMAIL";
    public const string SOCIAL_LOGIN = "SOCIAL";
    public const string PLAYER_SELF = "ME";
    public const string PLAYER_RIVAL = "RIVAL";
    public const string PLAY_MODE_SINGLE = "SINGLE_PLAY";
    public const string PLAY_MODE_MATCH = "MATCH_PLAY";
    public const string PLAY_MODE_OFFLINE = "OFFLINE_PLAY";
    public const string _objUrl = "prefeb/player1";
    public const string _GameGroundUrl = "prefeb/PlayObject";
    public const string _rankingRow = "prefeb/orderData";
    public const string _movablePos = "prefeb/movable";
    public const string _manageTournament = "prefeb/MyTournament";
    public const string _joinableTournament = "prefeb/Tournament";
    public const string _joinView = "prefeb/JoinView";
    public const string _tournamentRank = "prefeb/TournamentRank";
    public const string OPTIONS_SOUNDFX_TRUE = "true";
    public const string OPTIONS_SOUNDFX_FALSE = "false";
    public const string OPTIONS_MUSIC_TRUE = "true";
    public const string OPTIONS_MUSIC_FALSE = "false";
    public const string OPTIONS_VIBERATION_TRUE = "true";
    public const string OPTIONS_VIBERATION_FALSE = "false";
    public const string MY_MOVING_TURN = "MY_TURN";
    public const string AI_MOVING_TURN = "AI_TURN";
    public const string RIVAL_MOVING_TURN = "RIVAL_TURN";
    public const int ROOM_ENTERABLE_SCORE = 199;
    public const int AWARDED_COINS = 30;
    public const int JOIN_ISLAND_COINS = 100;
    public const int JOIN_ICELAND_COINS = 500;
    public const int JOIN_FIRELAND_COINS = 1000;
    public const string MAP_ISLAND = "island";
    public const string MAP_FIRELAND = "fireland";
    public const string MAP_ICELAND = "iceland";
    public const string FROM_NORMAL_ROOM = "normalroom";
    public const string FROM_TOURNAMENT_ROOM = "tournamentroom";
    public const string TOURNAMENT_PRIVATE = "private";
    public const string TOURNAMENT_PUBLIC = "public";
    public const string TOURNAMENT_PENDING = "pending";
    public const string TOURNAMENT_ACTIVE = "active";
    public const string TOURNAMENT_CANCELLED = "closed";
    public const string TOURNAMENT_EXPIRED = "expired";
    public const string ISLAND_BET_COUNT = "200";
    public const string ICELAND_BET_COUNT = "1000";
    public const string FIRELAND_BET_COUNT = "2000";
    // shop page.
    public const int SHOP_TIER_1 = 1;
    public const int SHOP_TIER_2 = 2;
    public const int SHOP_ADDON_1 = 3;
    public const int SHOP_ADDON_2 = 4;
    public const int TIER_1_COIN = 10;
    public const int TIER_2_COIN = 25;
    public const int ADDON_1_COIN = 10;
    public const int ADDON_2_COIN = 50;
    public const string ALREADY_OWNED = "You can't select this level";
    #region
    public const string RANKING_TITLE = "LEADERBOARD";
    public const string JOIN_TOURNAMENT_TITLE = "JOIN A TOURNAMENT";
    public const string MY_TOURNAMENT_TITLE = "MY TOURNAMENT";
    public const string TOURNAMENT_NAME_INPUT_ERROR = "Enter tournament name";
    public const string SELECT_PRIVACY = "Select a privacy";
    public const string SELECT_AREA = "Select an area";
    public const string SELECT_PARTICIPANTS = "Select a number of participants";
    public const string SELECT_START_DATE = "Select start date";
    public const string SELECT_END_DATE = "Select end date";
    public const string SELECT_LEVEL = "Select a level";
    public const string START_DATE_INVALID = "Start date should be after the current date";
    public const string END_DATE_INVALID = "End date should be after the start date";
    #endregion

}
