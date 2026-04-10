using UnityEngine;

public class Muffin : MonoBehaviour
{
    [SerializeField] private SpriteRenderer fortuneTellerSprite;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            LevelManager.Singleton.AddPotion();
            CharacterManager.Singleton.BecomeNewCharacter();
            PlayerAttack.Singleton.ResetAttackCooldown();
            MuffinSpawner.Singleton.SpawnPotion();

            Destroy(this.gameObject);
        }
    }

    public void ChangeFortuneSprite(Sprite sprite)
    {
        fortuneTellerSprite.gameObject.SetActive(true);
        fortuneTellerSprite.sprite = sprite;
    }
}
