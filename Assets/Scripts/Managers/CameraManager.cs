using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Singleton { get; private set; }
    private CinemachineCamera cam;

    void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            //DontDestroyOnLoad(gameObject);
            //SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != "MainMenu" && scene.name != "ShopTest")
        {
            cam = FindFirstObjectByType<CinemachineCamera>();
            Transform playerTransform = GameManager.Singleton.spawnedPlayer.transform;

            if (playerTransform != null)
            {
                cam.Follow = playerTransform;
            }
        }
    }

    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    if (scene.name != "MainMenu" && scene.name != "ShopTest")
    //    {
    //        cam = FindFirstObjectByType<CinemachineCamera>();
    //        Transform playerTransform = GameManager.Singleton.spawnedPlayer.transform;

    //        if (playerTransform != null)
    //        {
    //            Debug.Log(playerTransform);
    //            cam.Follow = playerTransform;
    //        }
    //    }
    //}
}
