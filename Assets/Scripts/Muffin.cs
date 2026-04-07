using UnityEngine;

public class Muffin : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            MuffinSpawner.Singleton.SpawnPotion();
            CharacterManager.Singleton.BecomeNewCharacter();
            PlayerAttack.Singleton.ResetAttackCooldown();
            LevelManager.Singleton.AddPotion();


            Destroy(this.gameObject);
        }
    }
}
