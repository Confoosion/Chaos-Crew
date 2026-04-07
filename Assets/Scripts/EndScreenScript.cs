using UnityEngine;
using TMPro;
// using TMPro.EditorUtilities;
using UnityEngine.SceneManagement;

public class EndScreenScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Death Screen")]
    [SerializeField] private TMP_Text potionsCollectedText;
    [SerializeField] private TMP_Text enemiesKilledText;
    [SerializeField] private TMP_Text moreEnemyStatsText;

    [Header("End Screen")]
    [SerializeField] private TMP_Text potionsCollectedText2;
    [SerializeField] private TMP_Text enemiesKilledText2;
    [SerializeField] private TMP_Text moreEnemyStatsText2;




    void Start()
    {
        moreEnemyStatsText.enabled = false;
    }

    public void moreEnemyStatsButton()
    {
        if (moreEnemyStatsText.enabled || moreEnemyStatsText2.enabled)
        {
            moreEnemyStatsText.enabled = false;
            moreEnemyStatsText2.enabled = false;
        }
        else
        {
            moreEnemyStatsText.enabled = true;
            moreEnemyStatsText2.enabled = true;
        }

    }

    public void mainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
        GameManager.Singleton.disableEndScreen();
        GameManager.Singleton.disableDeathScreen();

        GameManager.Singleton.resetEnemyKills();
        GameManager.Singleton.ResetPotionCount();

        if(PlayerControl.Singleton)
            Destroy(PlayerControl.Singleton.gameObject);
    }

    //public void restartButton()
    //{
    //    // SceneManager.LoadScene("StarterMap");
    //    // GameManager.Singleton.disableEndScreen();

    //    // GameManager.Singleton.resetEnemyKills();
    //    // GameManager.Singleton.resetMuffinCount();

    //    // PlayerControl.Singleton.movementLocked = false;
    //}

    public void updateText()
    {
        potionsCollectedText.text = "Potions Collected: " + GameManager.Singleton.GetPotionSum();
        enemiesKilledText.text = "Enemies Killed: " + (GameManager.Singleton.getBasicEnemiesKilled() + GameManager.Singleton.getHeavyEnemiesKilled() + GameManager.Singleton.getAngryBasicEnemiesKilled() + GameManager.Singleton.getAngryHeavyEnemiesKilled());
        moreEnemyStatsText.text = "Basic Enemy Deaths: " + GameManager.Singleton.getBasicEnemiesKilled() + "\n" + "Heavy Enemy Deaths: " + GameManager.Singleton.getHeavyEnemiesKilled() + "\nAngry Basic Enemy Deaths: " + GameManager.Singleton.getAngryBasicEnemiesKilled() + "\nAngry Heavy Enemy Deaths: " + GameManager.Singleton.getAngryHeavyEnemiesKilled();
    }

    public void updateText2()
    {
        potionsCollectedText2.text = "Potions Collected: " + GameManager.Singleton.GetPotionSum();
        enemiesKilledText2.text = "Enemies Killed: " + (GameManager.Singleton.getBasicEnemiesKilled() + GameManager.Singleton.getHeavyEnemiesKilled() + GameManager.Singleton.getAngryBasicEnemiesKilled() + GameManager.Singleton.getAngryHeavyEnemiesKilled());
        moreEnemyStatsText2.text = "Basic Enemy Deaths: " + GameManager.Singleton.getBasicEnemiesKilled() + "\n" + "Heavy Enemy Deaths: " + GameManager.Singleton.getHeavyEnemiesKilled() + "\nAngry Basic Enemy Deaths: " + GameManager.Singleton.getAngryBasicEnemiesKilled() + "\nAngry Heavy Enemy Deaths: " + GameManager.Singleton.getAngryHeavyEnemiesKilled();
    }
}
