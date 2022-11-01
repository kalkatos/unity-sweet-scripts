using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MissingScriptsFinder
{
        
    [MenuItem("Tools/Find Missing Scripts")]
    public static void FindMissingScripts ()
    {
        Debug.Log("Found");
        GameObject[] gos = SceneManager.GetActiveScene().GetRootGameObjects();

        for (int i = 0; i < gos.Length; i++)
        {
            Find(gos[i]);
        }

        void Find (GameObject go)
        {
            Component[] components = go.GetComponents(typeof(Component));
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                    Debug.Log("Null script on object " + go.name, go);
            }
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Find(go.transform.GetChild(i).gameObject);
            }
        }
    }
}
