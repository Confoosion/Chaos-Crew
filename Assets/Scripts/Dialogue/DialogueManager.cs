using System.Collections;
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(HandleSceneLoaded(scene));
    }

    private IEnumerator HandleSceneLoaded(Scene scene)
    {
        if (scene.name == "MainMenu" || scene.name == "ShopTest")
            yield break;

        // Wait a frame so other objects (spawner, managers, UI) can Awake/Start
        yield return null;

        // Find DialogueController if needed
        if (dialogueController == null)
            dialogueController = FindFirstObjectByType<DialogueController>();

        if (dialogueController == null)
            yield break;

        // Wait until CharacterManager exists
        if (CharacterManager.Singleton == null)
            yield break;

        var current = CharacterManager.Singleton.GetCurrentCharacter();
        if (current == null)
            yield break;

        dialogueController.PlayLine(current.characterName, "Heh, I can get used to this.");
    }
}
