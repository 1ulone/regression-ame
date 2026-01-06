using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using FirstGearGames.SmoothCameraShaker;

public class PlayerController : MonoBehaviour, IHealthComponent
{
    [SerializeField] private PlayerInput input;
    [SerializeField] private PlayerUI ui;

    [SerializeField] private int defaultMaxHealth = 10;
    [SerializeField] private float defaultMoveSpeed = 7.5f;
    [SerializeField] private float defaultRollMultiplier = 1.5f;
    [SerializeField] private float defaultRollTime = 0.15f;
    [SerializeField] private float defaultRollCooldownTime = 1f;
    [SerializeField] private float defaultAttackTime = 0.15f;
    [SerializeField] private float defaultAttackCooldownTime = 0.3f;
    [SerializeField] private float defaultRandomShootMultiplier = 0.25f;
    [SerializeField] private float defaultBulletSpeed = 3f;
    [SerializeField] private ShakeData shootScreenshake;
    
    private InputAction move;
    private InputAction attack;
    private InputAction roll;
    private InputAction pause;

    // private const string idle = "Idle";
    // private const string walkDown = "WalkDown";
    // private const string walkRight = "WalkRight";
    // private const string walkLeft = "WalkLeft";
    // private const string walkUp = "WalkUp";

    private Rigidbody2D rb;
    private Vector2 rdir;
    private Vector3 shootRandomness;
    private BaseEnemy lastHit;
    private Animator anim;

    private float startTime;
    private int health;
    private string state;

    private bool isRolling;
    private bool rollOnCooldown;
    private bool isAttack;
    private bool attackOnCooldown;

    public int maxHealth { get; set; } 
    public float moveSpeed { get; set; } 
    public float rollMultiplier { get; set; }
    public float rollTime { get; set; }
    public float rollCooldownTime { get; set; } 
    public float attackTime { get; set; }
    public float attackCooldownTime { get; set; }
    public float randomShootMultiplier { get; set; }
    public float bulletSpeed { get; set; }

    [HideInInspector] public List<PlayerBuffData> buffs = new List<PlayerBuffData>(); 

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        // ResetPlayerStats();
        
        buffs = GameController.instances.currentSave.playerSkill;
        UpdatePlayerStats();
    }

    private void OnEnable()
    {
        move = input.actions["Move"];
        attack = input.actions["Attack"];
        roll = input.actions["Jump"];
        pause = input.actions["Pause"];

        move.Enable();
        attack.Enable();
        roll.Enable();
        pause.Enable();
    }

    private void Update()
    {
        if (pause.WasPressedThisFrame())
            PauseMenuUI.instances.TogglePauseMenu();

        if (Time.timeScale == 0)
            return; 

        if (health <= 0 )
        {
            // if (lastHit == null)
            //     return;
            DeathUI.instances.StartDeathTransition(lastHit._data);
            return;
        }

        rdir = move.ReadValue<Vector2>();

        if (startTime != 0)
        {
            if (isRolling)
            {
                if (Time.time >= startTime + rollTime)
                {
                    isRolling = false;
                    rollOnCooldown = true;
                    startTime = Time.time;
                    ui.SetStamina(rollCooldownTime);
                }
            }

            if (rollOnCooldown)
            {
                if (Time.time >= startTime + rollCooldownTime)
                {
                    rollOnCooldown = false;
                    startTime = 0;
                }
            }

            if (isAttack)
            {
                if (Time.time >= startTime + attackTime)
                {
                    isAttack = false;
                    attackOnCooldown = true;
                    startTime = Time.time;
                }
            }

            if (attackOnCooldown)
            {
                if (Time.time >= startTime + attackCooldownTime)
                {
                    attackOnCooldown = false;
                    startTime = 0;
                }
            }
        }

        if (rdir != Vector2.zero)
        {
            anim.SetBool("IsRunUp", rdir.y > 0);
            anim.SetBool("IsRunRight", rdir.x > 0 && rdir.y == 0);
            anim.SetBool("IsRunDown", rdir.y < 0);
            anim.SetBool("IsRunLeft", rdir.x < 0 && rdir.y == 0);
        }

        if (attack.WasPressedThisFrame() && !isAttack && !attackOnCooldown && !isRolling)
        {
            CameraShakerHandler.Shake(shootScreenshake);
            
            startTime = Time.time;
            isAttack = true;

            shootRandomness = new Vector3(Random.Range(-1f, 1f) * randomShootMultiplier, Random.Range(-1f, 1f) * randomShootMultiplier, 0);
            Vector3 dir = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
            GameObject bullet = Pool.instances.CreateObject("playerBullet", transform.position + dir.normalized + shootRandomness, Vector3.zero);

            bullet.GetComponent<Rigidbody2D>().linearVelocity = (dir.normalized * 10) * bulletSpeed;

            rb.AddForce((-dir.normalized * 100) * moveSpeed /2f);
        }

        if (roll.WasPressedThisFrame() && !isRolling && !rollOnCooldown && !isAttack)
        {
            startTime = Time.time;
            isRolling = true;
            rb.linearVelocity = Vector2.zero;

            if (rdir == Vector2.zero)
            {
                Vector3 dir = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
                rb.AddForce((dir.normalized * 100) * moveSpeed * rollMultiplier);
            } else {
                rb.AddForce((rdir.normalized * 100) * moveSpeed * rollMultiplier);
            }
        }

    }

    private void FixedUpdate()
    {   
        if (isRolling)
            return;

        rb.linearVelocity = rdir * moveSpeed;
    }

    public void OnDamage(int damage, MonoBehaviour reference = null)
    {
        if (reference != null)
            lastHit = reference as BaseEnemy;

        TimeController.instances.HitStop(0.1f);
        rb.linearVelocity = Vector2.zero;
        health -= damage;
        ui.UpdateHealth(health, maxHealth);
    }

    public void UpdatePlayerStats()
    {
        if (buffs == null)
        {
            Debug.Log(buffs == null);
            ResetPlayerStats();

            health = maxHealth;
            ui.UpdateHealth(health, maxHealth);
            return;
        }

        if (buffs.Count <= 0)
        {
            Debug.Log(buffs.Count);
            ResetPlayerStats();

            health = maxHealth;
            ui.UpdateHealth(health, maxHealth);
            return;
        }

        ResetPlayerStats();
        for (int i = 0; i < buffs.Count; i++)
        {
            maxHealth = defaultMaxHealth + buffs[i].health;
            moveSpeed = defaultMoveSpeed + buffs[i].moveSpeed;
            rollMultiplier = defaultRollMultiplier + buffs[i].rollMultiplier;
            rollTime = defaultRollTime + buffs[i].rollTime;
            rollCooldownTime = defaultRollCooldownTime + buffs[i].rollCooldownTime;
            attackTime = defaultAttackTime + buffs[i].attackTime;
            attackCooldownTime = defaultAttackCooldownTime + buffs[i].attackCooldownTime;
            randomShootMultiplier = defaultRandomShootMultiplier + buffs[i].randomShootMultiplier;
            bulletSpeed = defaultBulletSpeed + buffs[i].bulletSpeed;
        }

        health = maxHealth;
        ui.UpdateHealth(health, maxHealth);
    }

    public void ResetPlayerStats()
    {
        maxHealth = defaultMaxHealth;
        moveSpeed = defaultMoveSpeed;
        rollMultiplier = defaultRollMultiplier;
        rollTime = defaultRollTime;
        rollCooldownTime = defaultRollCooldownTime;
        attackTime = defaultAttackTime;
        attackCooldownTime = defaultAttackCooldownTime;
        randomShootMultiplier = defaultRandomShootMultiplier;
        bulletSpeed = defaultBulletSpeed;
    }


}
