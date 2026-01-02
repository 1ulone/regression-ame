using UnityEngine;
using UnityEngine.InputSystem;

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
    
    private InputAction move;
    private InputAction attack;
    private InputAction roll;

    private Rigidbody2D rb;
    private Vector2 rdir;
    private Vector3 shootRandomness;

    private float startTime;
    private int health;

    private bool isRolling;
    private bool rollOnCooldown;
    private bool isAttack;
    private bool attackOnCooldown;
    private PlayerBuffData[] buffs;

    public int maxHealth { get; set; } 
    public float moveSpeed { get; set; } 
    public float rollMultiplier { get; set; }
    public float rollTime { get; set; }
    public float rollCooldownTime { get; set; } 
    public float attackTime { get; set; }
    public float attackCooldownTime { get; set; }
    public float randomShootMultiplier { get; set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        ResetPlayerStats();
        health = maxHealth;

        ui.UpdateHealth(health, maxHealth);
    }

    private void OnEnable()
    {
        move = input.actions["Move"];
        attack = input.actions["Attack"];
        roll = input.actions["Jump"];

        move.Enable();
        attack.Enable();
    }

    private void Update()
    {
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

        if (attack.WasPressedThisFrame() && !isAttack && !attackOnCooldown && !isRolling)
        {
            startTime = Time.time;
            isAttack = true;

            shootRandomness = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
            Vector3 dir = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
            GameObject bullet = Pool.instances.CreateObject("playerBullet", transform.position + dir.normalized + shootRandomness, Vector3.zero);

            bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * 3;
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

    public void OnDamage(int damage)
    {
        TimeController.instances.HitStop(0.1f);
        rb.linearVelocity = Vector2.zero;
        health -= damage;
        ui.UpdateHealth(health, maxHealth);
    }

    public void UpdatePlayerStats()
    {
        PlayerBuffData totalBuff = new PlayerBuffData();
        for (int i = 0; i < buffs.Length; i++)
        {
            totalBuff.health += buffs[i].health;
            totalBuff.moveSpeed += buffs[i].moveSpeed;
            totalBuff.rollMultiplier += buffs[i].rollMultiplier;
            totalBuff.rollTime += buffs[i].rollTime;
            totalBuff.rollCooldownTime += buffs[i].rollCooldownTime;
            totalBuff.attackTime += buffs[i].attackTime;
            totalBuff.attackCooldownTime += buffs[i].attackCooldownTime;
            totalBuff.randomShootMultiplier += buffs[i].randomShootMultiplier;
        }

        maxHealth = defaultMaxHealth + totalBuff.health;
        moveSpeed = defaultMoveSpeed + totalBuff.moveSpeed;
        rollMultiplier = defaultRollMultiplier + totalBuff.rollMultiplier;
        rollTime = defaultRollTime + totalBuff.rollTime;
        rollCooldownTime = defaultRollCooldownTime + totalBuff.rollCooldownTime;
        attackTime = defaultAttackTime + totalBuff.attackTime;
        attackCooldownTime = defaultAttackCooldownTime + totalBuff.attackCooldownTime;
        randomShootMultiplier = defaultRandomShootMultiplier + totalBuff.randomShootMultiplier;
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
    }


}
