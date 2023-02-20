using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Photon.Bolt;

namespace Scram 
{

	public enum checkType { HardMonitoring = 15, MediumMonitoring = 25, LightMonitoring = 75 }

	public class CommandHandler : MonoBehaviour {

		// Variables
		public int SimulateRate = 60; // Tick rate of server
		public int SecondsToMonitor = 5; // The amount of seconds before checking the current commands
		public checkType MonitorType = checkType.MediumMonitoring; // Change if theres too many false positives
		public int pardonTime = 30; // The amount of seconds before a warning is removed
		public int maxWarnings = 3;
		float SimulationRateCheck = -1;
		public static CommandHandler instance;
		List<PlayerMonitor> monitoredPlayers = new List<PlayerMonitor>();

		void Awake(){
			// Assign our singleton
			if(instance == null){
				instance = this;
				DontDestroyOnLoad(gameObject);
			}else{
				Debug.LogError("ERROR - Two Command Handlers Found! FIX");
				Destroy(this);
			}
		}

		void Start(){
			// Determine how many commands are sent in our check timer
			SimulationRateCheck = SimulateRate * SecondsToMonitor;
		}    

		static void CheckSingleton(){
			if(instance == null){
				instance = new GameObject().AddComponent<CommandHandler>();
				instance.SimulationRateCheck = instance.SimulateRate * instance.SecondsToMonitor;
				DontDestroyOnLoad(instance.gameObject);
			}
		}

		public void RemovePlayer(BoltConnection connection){
			// When a player leaves their connection should be removed from the list so its easier/less cpu expensive to search.
			for(int i = 0; i < monitoredPlayers.Count; i++){
				if(monitoredPlayers[i].connection == connection){
					monitoredPlayers.RemoveAt(i);
					return;
				}
			}
		}

		// Uploads data to the discord webhook (REMOVE THIS WHEN DONE GETTING DATA)
		public static void logData(string data) {
			Debug.Log("[Anti-Hack] " + data);
		}

		public static void HandleCommand(BoltConnection connection, out bool shouldKick){

			CheckSingleton();

			shouldKick = false;
			// Get the player's monitor
			PlayerMonitor playerMonitor = instance.findMonitor(connection);
			if(playerMonitor != null && playerMonitor.connection != null){
				// Use exsisting monitor and increase the amount of commands
				playerMonitor.currentCommands++;
				double currentTime = TimeController.GetTime();

				if(playerMonitor.currentWarnings >= instance.maxWarnings){
					shouldKick = true;
					// Remove this log data after calibrating
					logData(string.Format("Kicked player had: {0} cmds in {1} seconds.", playerMonitor.currentCommands, ((float)(currentTime - playerMonitor.startTime))));
					return;
				}

				if(playerMonitor.currentCommands >= instance.SimulationRateCheck * 3){
					playerMonitor.currentWarnings = 10; // Make it a high amount so it will 100% kick them
					// Remove this log data after calibrating
					logData(string.Format("Obvious player had: {0} cmds in {1} seconds.", playerMonitor.currentCommands, ((float)(currentTime - playerMonitor.startTime))));
				}

				if((currentTime - playerMonitor.startTime) >= instance.SecondsToMonitor || playerMonitor.currentCommands >= instance.SimulationRateCheck * 3){
						// If the timer has passed, and our current commands is above the amount allowed for that time span (+ the max difference)
						if(playerMonitor.currentCommands > instance.SimulationRateCheck + (int)instance.MonitorType){
							playerMonitor.currentWarnings++;
						}
						// Reset their commands/time as they have not been detected as cheating
						playerMonitor.currentCommands = 0;
						playerMonitor.startTime = TimeController.GetTime();
					}
				// Check if its time to remove a warning
				if((currentTime - playerMonitor.removeTime) >= instance.pardonTime){
					// Remove a warning
					playerMonitor.currentWarnings -= playerMonitor.currentWarnings > 0 ? 1 : 0;
					playerMonitor.removeTime = TimeController.GetTime();
				}
			}else{
				// Create monitor for the player
				instance.monitoredPlayers.Add(new PlayerMonitor(){
				startTime = TimeController.GetTime(),
				removeTime = TimeController.GetTime() + 60,
				currentWarnings = 0,
				connection = connection  
				});
			}
		}

		// Finds a monitor of a player
		PlayerMonitor findMonitor(BoltConnection connection){
			for(int i = 0; i < monitoredPlayers.Count; i++){
				if(monitoredPlayers[i].connection == connection){
					return monitoredPlayers[i];
				}
			}
			return null;
		}
	}

	public class TimeController {
		public static double GetTime(double startTime){
			// Get the total amount of Epoch seconds minus the start time.
			return ((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds - startTime);
		}

		public static double GetTime(){
			// Get the total amount of Epoch seconds.
			return (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
		}
	}

	[System.Serializable]
	public class PlayerMonitor {
		public BoltConnection connection;
		public int currentCommands = 0;
		public double startTime = 0;
		public double removeTime = 0;
		public int currentWarnings = 0;
	}

}