using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Singleton { get; private set; }

    [SerializeField] private DialogueController dialogueController;

    void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Runs this only if it's a MAP (player is in game)
        if (scene.name != "MainMenu" && scene.name != "ShopTest")
        {
            dialogueController = FindFirstObjectByType<DialogueController>();
        }

        if (dialogueController)
        {
            string name = CharacterManager.Singleton.GetCurrentCharacter().characterName;
            dialogueController.PlayLine(name, "Heh, I can get used to this.");
        }
    }
}
