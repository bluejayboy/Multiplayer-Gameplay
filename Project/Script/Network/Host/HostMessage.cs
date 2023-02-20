using Photon.Bolt;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public sealed class HostMessage : GlobalEventListener
    {
        private const string messageFormat = "<color=#></color>: ";
        private const string teamFormat = "(Team) ";
        private const string spectateFormat = "(Spectate) ";
        private const string modFormat = "<color=yellow><sprite name=\"hammer\" tint=1></color> ";
        private const string devFormat = "<sprite name=\"spleen\"><color=#FFAA55> ";
        private const string serverPrefix = "<color=#CCCCCC>[<color=red>Server<color=#CCCCCC>] ";
        private const string mutedPrefix = "<color=#CCCCCC>[<color=red>Muted<color=#CCCCCC>] ";
        private string moderators = string.Empty;
        private string spleens = string.Empty;

        private void Start()
        {
            InvokeRepeating("UpdateTHings", 5, 1200);
        }

        private void UpdateTHings()
        {
            StartCoroutine(RefreshLists());
        }

        public override void OnEvent(ChatEvent evnt)
        {
            if (evnt.RaisedBy.GetPlayerConnection() == null)
            {
                return;
            }

            PlayerInfo playerInfo = evnt.RaisedBy.GetPlayerConnection().PlayerInfo;
            string penName = playerInfo.state.PenName;
            string color = ColorUtility.ToHtmlStringRGB(playerInfo.state.Color);
            string team = playerInfo.state.Team;

            if (!Regex.IsMatch(evnt.Text, "^[a-zA-Z0-9. -_?]*$"))
            {
                return;
            }

            if (evnt.Text.IndexOf("nig", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                evnt.Text = "I'm gay.";
            }

            if (evnt.IsTeam)
            {
                for (var i = 0; i < HostPlayerRegistry.Instance.PlayerConnections.Count; i++)
                {
                    PlayerConnection playerConnection = HostPlayerRegistry.Instance.PlayerConnections[i];
                    string playerTeam = playerConnection.PlayerInfo.state.Team;

                    if (playerTeam == team)
                    {
                        int capacity = messageFormat.Length + color.Length + penName.Length + evnt.Text.Length + team.Length;
                        var stringBuilder = new StringBuilder(capacity);

                        stringBuilder.Append("<color=#").Append(color).Append(">").Append(teamFormat).Append(penName).Append("</color>").Append(": ").Append(evnt.Text);

                        BoltGlobalEvent.SendMessage(stringBuilder.ToString(), Color.white, playerConnection.BoltConnection);
                    }
                }
            }
            else if (GameMode.Instance.Data.HideSpectateMessage && team == "Default" && (Game.Instance.MatchStatus == Game.MatchState.PreStart || Game.Instance.MatchStatus == Game.MatchState.MidRound))
            {
                for (var i = 0; i < HostPlayerRegistry.Instance.PlayerConnections.Count; i++)
                {
                    PlayerConnection playerConnection = HostPlayerRegistry.Instance.PlayerConnections[i];
                    string playerTeam = playerConnection.PlayerInfo.state.Team;

                    if (playerTeam == "Default")
                    {
                        //int capacity = messageFormat.Length + color.Length + penName.Length + evnt.Text.Length + team.Length;
                        //var stringBuilder = new StringBuilder(capacity);

                        //stringBuilder.Append("<color=#").Append(color).Append(">").Append(spectateFormat).Append(penName).Append("</color>").Append(": ").Append(evnt.Text);

                        string parsedUsername = Regex.Replace(Regex.Replace(penName, "</noparse>", string.Empty, RegexOptions.IgnoreCase), "<noparse>", string.Empty, RegexOptions.IgnoreCase);
                        string parsedChatMsg = Regex.Replace(Regex.Replace(evnt.Text, "</noparse>", string.Empty, RegexOptions.IgnoreCase), "<noparse>", string.Empty, RegexOptions.IgnoreCase);

                        string chatMsg = string.Format("<color=#{0}>{1}{4}<noparse>{2}</noparse>{5}</color>: <noparse>{3}</noparse>", color, spectateFormat, parsedUsername, parsedChatMsg);

                        BoltGlobalEvent.SendMessage(chatMsg, Color.white, playerConnection.BoltConnection);
                    }
                }
            }
            else
            {
                int capacity = messageFormat.Length + color.Length + penName.Length + evnt.Text.Length;
                var stringBuilder = new StringBuilder(capacity);

                stringBuilder.Append("<color=#").Append(color).Append(">").Append(penName).Append("</color>").Append(": ").Append(evnt.Text);

                BoltGlobalEvent.SendMessage(stringBuilder.ToString(), Color.white);
            }
        }

        public static bool containsUser(string steamid, string text)
        {
            string[] splitString = text.Split('|');

            for (int i = 0; i < splitString.Length; i++)
            {
                if (splitString[i] == steamid)
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerator RefreshLists()
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(string.Format("http://api.scramgame.com/legacy/moderators/"));
            yield return webRequest.Send();
            moderators = webRequest.downloadHandler.text;

            webRequest = UnityWebRequest.Get(string.Format("http://api.scramgame.com/legacy/spleen/"));
            yield return webRequest.Send();
            spleens = webRequest.downloadHandler.text;
        }

    }
}