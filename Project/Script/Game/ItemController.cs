using Scram;
using System.Collections;
using UnityEngine.Networking;

public static class ItemController
{
    public static string Password { get; set; }

    public enum Type { present = 42, redPresent = 43, yellowPresent = 44, orangePresent = 45, purplePresent = 46, bluePresent = 47, blackPresent = 48, whitePresent = 49, antlers = 50 }

    public static IEnumerator UserOwns(ulong _steamId, Type type, System.Action<bool, ulong, Type> action)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(string.Format("http://scramshit.ddns.net/shit/UserOwnsItem{0}.php?steamid={1}", (int)type, _steamId));
        yield return webRequest.Send();

        bool ownsItem = bool.TryParse(webRequest.downloadHandler.text, out ownsItem) ? ownsItem : false;

        action(ownsItem, _steamId, type);
    }

    public static IEnumerator GiveUserItem(ulong steamId, int itemId)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(string.Format("https://api.scramgame.com/legacy/giveitem/?key={1}&steamid={0}&itemid={2}", steamId, Password, itemId));
        yield return webRequest.Send();
    }

    public static IEnumerator GetLeaderboardScore(ulong steamId, int leaderboardId, System.Action<ulong, int, int> action)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(string.Format("http://scramshit.ddns.net/shit/CheckBoard.php?steamid={0}&leaderboard={1}", steamId, leaderboardId));
        yield return webRequest.Send();

        int currentValue = int.TryParse(webRequest.downloadHandler.text, out currentValue) ? currentValue : 0;

        action(steamId, currentValue, leaderboardId);
    }

    public static void UserAddBack(ulong steamId, int value, int leaderboardId)
    {
        GameMode.Instance.CheckScore(steamId, leaderboardId);
    }

    public static IEnumerator AddLeaderboardScore(ulong _steamId, int leaderboardId)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(string.Format("http://scramshit.ddns.net/shit/UpdateBoard.php?steamid={0}&key={1}&leaderboard={2}", _steamId, Password, leaderboardId));
        yield return webRequest.Send();

        int currentValue = int.TryParse(webRequest.downloadHandler.text, out currentValue) ? currentValue : 0;

        UserAddBack(_steamId, currentValue, leaderboardId);
    }

    public static IEnumerator GiveUserAchievement(ulong steamId, string achievementAPIName)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(string.Format("http://scramshit.ddns.net/shit/GiveAchievment.php?steamid={0}&key={1}&stat={2}", steamId, Password, achievementAPIName));
        yield return webRequest.Send();
    }
}