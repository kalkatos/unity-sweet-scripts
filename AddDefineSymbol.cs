using UnityEditor;

namespace Kalkatos
{
    [InitializeOnLoad]
    public class AddDefineSymbol : Editor
    {
        private static void OnScriptsReloaded ()
        {
            string symbol = "KALKATOS_NETWORK";
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            if (!currentSymbols.Contains(symbol))
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, $"{currentSymbols};{symbol}");
        }
    } 
}
