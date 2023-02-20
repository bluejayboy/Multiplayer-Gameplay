using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Scram {
	public class DataOptionsMenu : MenuParent {

		public TMP_Text cacheInfo;

		public override void Activate(){
			if(Directory.Exists(Application.persistentDataPath + "/inventoryCache")){
				string[] files = Directory.GetFiles(Application.persistentDataPath + "/inventoryCache/");

				long cacheSize = 0;
				foreach(string file in files){
					FileInfo info = new FileInfo(file);
					cacheSize += info.Length;
				}

				cacheInfo.text = (cacheSize / 1000) / 1000 + " mb.";
			}else{
				cacheInfo.text = "0 bytes.";
			}
		}

		public void ClearCache(){
			if(Directory.Exists(Application.persistentDataPath + "/inventoryCache")){
				string[] files = Directory.GetFiles(Application.persistentDataPath + "/inventoryCache/");

				foreach(string file in files){
					File.Delete(file);
				}
			}

			Activate();
		}
	}
}