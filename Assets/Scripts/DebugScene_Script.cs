using UnityEngine;

public class DebugScene_Script : MonoBehaviour
{
    public static DebugScene_Script Singleton;

    void Awake()
    {
        if(Singleton == null)
            Singleton = this;
    }

    public void DebugStart()
    {
        CharacterManager.Singleton.InitializeCharacters();
    }
}
