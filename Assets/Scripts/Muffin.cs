using UnityEngine;

public class Muffin : MonoBehaviour
{
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
}
