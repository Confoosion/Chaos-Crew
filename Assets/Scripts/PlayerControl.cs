using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("Player Stats")]
    public float MAXSPEED;
    [SerializeField] private float speed;
    public float chargeSpeed;
    public int extraJumps;
    [SerializeField] private float jumpPower;
    public bool movementLocked = false;
    [SerializeField] private PerkSO perk;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Space]

    [Header("Sounds")]
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] float jumpVolume;

    [Space]

    [Header("Debug")]
    [SerializeField] private bool godMode = false;

    private bool isCharging = false;
    private bool jumpLocked = false;

    private float horizontal;
    private bool isFacingRight;
    private int extraJumpCount;
    private bool onGround = true;

    public static PlayerControl Singleton;

    private int godFactor = 10;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // if (perk != null)
        // {
        //     PerksManager.Singleton.EquipPerk(perk);
        // }
        PerksManager.Singleton.SetPlayerControlReference(this);

        PerksManager.Singleton.ApplyActivePerk();

        speed = MAXSPEED;
        ResetJumps();
    }

    void Update()
    {
        // Debug God mode
        if(Input.GetKeyDown(KeyCode.I))
        {
            godMode = !godMode;
            if (godMode)
            {
                MAXSPEED += godFactor;
                speed += godFactor;
                jumpPower += godFactor;
            }
            else
            {
                MAXSPEED -= godFactor;
                speed -= godFactor;
                jumpPower -= godFactor;
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape) && GameManager.Singleton.IsPlayerAlive())
        {
            if(!GameManager.Singleton.IsPaused())
            {
                GameManager.Singleton.PauseGame(true);
            }
            else
            {
                GameManager.Singleton.PauseGame(false);
            }
        }

        // Movement
        if (!movementLocked && !GameManager.Singleton.IsPaused())
        {
            horizontal = Input.GetAxisRaw("Horizontal");

            // Check if player is on ground
            onGround = IsGrounded();

            // Jumping from the ground
            if (Input.GetButtonDown("Jump") && onGround && !jumpLocked)
            {
                Jump();
            }
            // Double jump
            else if (Input.GetButtonDown("Jump") && !onGround && extraJumpCount > 0 && !jumpLocked)
            {
                Jump();
                extraJumpCount--;
            }

            // Dynamic jump height
            if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f && !jumpLocked)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }

            FlipPlayer();
        }
    }

    void FixedUpdate()
    {
        if (!movementLocked && !isCharging)
        {
            if(speed > MAXSPEED)
            {
                speed -= Time.deltaTime * 1.5f;
                if(speed < MAXSPEED)
                    speed = MAXSPEED;
            }

            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        }
        else if (movementLocked && isCharging)
        {
            horizontal = (isFacingRight) ? 1f : -1f;
            rb.linearVelocity = new Vector2(horizontal * chargeSpeed, rb.linearVelocity.y);
        }

    }

    private bool IsGrounded()
    {
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);

        if (grounded)
            ResetJumps();

        return grounded;
    }

    // Flips player into looking left or right
    private void FlipPlayer()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        SoundManager.Singleton.PlayPlayerAudio(jumpSFX, jumpVolume);
    }

    // Used if player equips Double Jump Perk
    private void ResetJumps()
    {
        if (extraJumps > 0)
        {
            extraJumpCount = extraJumps;
        }

        if (jumpLocked)
        {
            jumpLocked = false;
        }
    }

    public void ChargeMovement(float time)
    {
        StartCoroutine(DoChargeMovement(time));
    }

    IEnumerator DoChargeMovement(float time)
    {
        movementLocked = true;
        isCharging = true;

        yield return new WaitForSeconds(time);

        movementLocked = false;
        isCharging = false;
    }

    public void GroundPoundMovement(float groundPoundForce)
    {
        if (!IsGrounded())
        {
            jumpLocked = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.down * groundPoundForce, ForceMode2D.Impulse);
        }
    }

    public void BounceFromGroundPound(float bounceForce)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
        jumpLocked = false;
    }

    public void GainSpeedBoost(float boost)
    {
        speed += boost;
        if(speed > 15f)
            speed = 15f;
    }

    public void playerDeath()
    {
        if(!godMode)
        {
            // Debug.Log("Player died");
            movementLocked = true;
            Destroy(GameManager.Singleton.spawnedPlayer);

            // death animation
            // Open end UI screen 
            GameManager.Singleton.enableDeathScreen();
            SpawnerManager.Singleton.stopGame = true;
        }
    }
}
