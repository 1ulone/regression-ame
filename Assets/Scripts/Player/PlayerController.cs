using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using FirstGearGames.SmoothCameraShaker;

public class PlayerController : MonoBehaviour, IHealthComponent
{
    [SerializeField] private PlayerInput input;
    [SerializeField] private PlayerUI ui;

    [SerializeField] private int defaultMaxHealth = 10;
    [SerializeField] private int defaultDamage = 1;
    [SerializeField] private float defaultMoveSpeed = 7.5f;
    [SerializeField] private float defaultRollMultiplier = 1.5f;
    [SerializeField] private float defaultRollTime = 0.15f;
    [SerializeField] private float defaultRollCooldownTime = 1f;
    [SerializeField] private float defaultAttackTime = 0.15f;
    [SerializeField] private float defaultAttackCooldownTime = 0.3f;
    [SerializeField] private float defaultRandomShootMultiplier = 0.25f;
    [SerializeField] private float defaultBulletSpeed = 3f;

    [SerializeField] private ShakeData shootScreenshake;
    [SerializeField] private ShakeData hurtScreenshake;

    [SerializeField] private LayerMask enemyLayer;
    
    private InputAction move;
    private InputAction attack;
    private InputAction roll;
    private InputAction pause;

    private Rigidbody2D rb;
    private Vector2 rdir;
    private Vector3 shootRandomness;
    private BaseEnemy lastHit;
    private Animator anim;
    private SpriteRenderer rend;

    private float startTime;
    private int health;
    private string state;

    private bool isRolling;
    private bool rollOnCooldown;
    private bool isAttack;
    private bool attackOnCooldown;
    private bool isHurt;

    public int maxHealth { get; set; } 
    public float moveSpeed { get; set; } 
    public float rollMultiplier { get; set; }
    public float rollTime { get; set; }
    public float rollCooldownTime { get; set; } 
    public float attackTime { get; set; }
    public float attackCooldownTime { get; set; }
    public float randomShootMultiplier { get; set; }
    public float bulletSpeed { get; set; }
    public int damage { get; set; }

    [HideInInspector] public List<PlayerBuffData> buffs = new List<PlayerBuffData>(); 

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        // ResetPlayerStats();
        
        buffs = GameController.instances.currentSave.playerSkill;
        UpdatePlayerStats();

        InvokeRepeating("UpdatePassive", 2, 3.5f);
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
                    attackOnCooldown = false;
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
                    rollOnCooldown = false;
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


            shootRandomness = new Vector3(UnityEngine.Random.Range(-1f, 1f) * randomShootMultiplier, UnityEngine.Random.Range(-1f, 1f) * randomShootMultiplier, 0);
            Vector3 dir = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;

            Action attackBehaviour = DefaultShootBehaviour(dir, damage, transform.position + dir.normalized + shootRandomness);
            if (buffs != null)
            {
                foreach (PlayerBuffData data in buffs)
                {
                    if (data.behaviour == attackType.shoot || data.behaviour == attackType.shotgun || data.behaviour == attackType.railgun)
                        attackBehaviour = data.GetAttackBehaviour(dir, damage, transform.position + dir.normalized + shootRandomness);
                }
            }
            attackBehaviour.Invoke();

            // GameObject bullet = Pool.instances.CreateObject("playerBullet", transform.position + dir.normalized + shootRandomness, Vector3.zero);
            //
            // bullet.GetComponent<Rigidbody2D>().linearVelocity = (dir.normalized * 10) * bulletSpeed;
            // bullet.GetComponent<DamageComponent>().damage = damage;

            rb.AddForce((-dir.normalized * 100) * moveSpeed /2f);
        }

        if (roll.WasPressedThisFrame() && !isRolling && !rollOnCooldown && !isAttack)
        {
            Audio.instances.PlaySFX("dash");

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
    
    private IEnumerator hurtCoroutine()
    {
        int i = 0;
        while(i < 5)
        {
            rend.enabled = true;
            yield return new WaitForSeconds(0.05f); 
            rend.enabled = false;
            yield return new WaitForSeconds(0.05f);
            i++;
        }

        rend.enabled = true;
        isHurt = false;
    }

    public void OnDamage(int damage, MonoBehaviour reference = null)
    {
        if (isHurt)
            return;

        if (isRolling)
            return;

        if (reference != null)
            lastHit = reference as BaseEnemy;

        isHurt = true;
        StartCoroutine(hurtCoroutine());
        CameraShakerHandler.Shake(hurtScreenshake);
        TimeController.instances.HitStop(0.1f);
        rb.linearVelocity = Vector2.zero;
        health -= damage;
        ui.UpdateHealth(health, maxHealth);
    }

    public void UpdatePlayerStats()
    {
        if (buffs == null)
        {
            // Debug.Log(buffs == null);
            ResetPlayerStats();

            health = maxHealth;
            ui.UpdateHealth(health, maxHealth);
            return;
        }

        if (buffs.Count <= 0)
        {
            // Debug.Log(buffs.Count);
            ResetPlayerStats();

            health = maxHealth;
            ui.UpdateHealth(health, maxHealth);
            return;
        }

        ResetPlayerStats();
        for (int i = 0; i < buffs.Count; i++)
        {
            maxHealth = defaultMaxHealth + buffs[i].health;
            damage = defaultDamage + buffs[i].attack;
            moveSpeed = defaultMoveSpeed + buffs[i].speed;
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
        damage = defaultDamage;
    }

    private Action DefaultShootBehaviour(Vector2 dir, int damage, Vector2 pos)
    {
        return ()=> 
        {
            string attackPrefab = "playerBullet";

            DamageComponent b = Pool.instances.CreateObject(attackPrefab, pos, Vector2.zero).GetComponent<DamageComponent>();
            b.gameObject.GetComponent<Rigidbody2D>().linearVelocity = dir * 3; 
            b.damage = damage;
        };
    }

    private void UpdatePassive()
    {
        foreach (PlayerBuffData pb in buffs)
        {
            if (pb.passive != passiveType.none)
                pb.GetPassiveBehaviour(damage, transform.position, enemyLayer).Invoke();
        }
    }
}
