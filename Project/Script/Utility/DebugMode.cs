using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class DebugMode : MonoBehaviour
    {
        public static bool IsDebug { get; private set; }

        [SerializeField] private bool isDebug = false;
        [SerializeField] private bool isDedicatedBuild = false;
        [SerializeField] private bool enableHostMigration = false;

        private void Awake()
        {
            IsDebug = isDebug;
        }

        void OnValidate()
        {
            #if UNITY_EDITOR

            string definesString = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup ( UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup );
            List<string> allDefines = definesString.Split ( ';' ).ToList ();
            for(int i = 0; i < allDefines.Count; i++){
                if(allDefines[i] == "ISDEDICATED" || allDefines[i] == "ENABLEHOSTMIGRATION"){
                    allDefines.RemoveAt(i);
                    i--;
                }
            }
            if(isDedicatedBuild){
                allDefines.Add("ISDEDICATED");
                //transform.GetChild(0).gameObject.SetActive(true);
            }else{
                //transform.GetChild(0).gameObject.SetActive(false);
            }
            if(enableHostMigration)
                allDefines.Add("ENABLEHOSTMIGRATION");
            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup (
            UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup,
            string.Join ( ";", allDefines.ToArray () ) );
            #endif
        }
    }
}