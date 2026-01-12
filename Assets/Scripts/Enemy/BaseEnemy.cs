using UnityEngine;
using System.Collections;

public class BaseEnemy : MonoBehaviour, IHealthComponent 
{
    protected enum state
    {
        idle,
        chase,
        attack,
        cooldown,
        death
    }

    protected state _state;
    protected Rigidbody2D rb;
    protected Transform chaseTarget;
    protected SpriteRenderer rend;
    protected Animator anim;

    protected int health;
    protected float startTime;
    protected float chaseSpeed;
    protected bool isHurt;
    
    [SerializeField] protected EnemyData data;
    [SerializeField] protected float chaseRange = 10;
    [SerializeField] protected float attackRange = 2;
    [SerializeField] protected float attackTime = 1;
    [SerializeField] protected float cooldownTime = 2;
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] public float attackSpeed = 3;

    public EnemyData _data { get { return data; } }
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        chaseTarget = GameObject.FindFirstObjectByType<PlayerController>().transform; 
        health = data.health;
        chaseSpeed = data.moveSpeed;

        Initialize();
    }

    protected virtual void Initialize() {}

    private void Update()
    {
        if (Time.timeScale == 0)
            return;
            
        if (health <= 0)
            ChangeState(state.death);

        UpdateLogic();
    }

    protected void ChangeState(state newState)
    {
        if (_state == newState)
            return;

        ExitLogic(_state);
        _state = newState;
        EnterLogic(_state);
    }

    private void ExitLogic(state s)
    {
        switch(s)
        {
            case state.idle: transitionIdle(); break; 
            case state.chase: transitionIdle(); break; 
            case state.attack: exitAttack(); break; 
            case state.cooldown: exitCooldown(); break; 
        }
    }

    private void EnterLogic(state s)
    {
        switch(s)
        {
            case state.idle: transitionIdle(); break; 
            case state.chase: transitionIdle(); break; 
            case state.attack: enterAttack(); break; 
            case state.cooldown: enterCooldown(); break; 
            case state.death: enterDeath(); break; 
        }
    }

    protected virtual void UpdateLogic()
    {
        switch(_state)
        {
            case state.idle: updateIdle(); break; 
            case state.chase: updateChase(); break; 
            case state.attack: updateAttack(); break; 
            case state.cooldown: updateCooldown(); break; 
        }
    }

    protected virtual void transitionIdle() 
    {
        rb.linearVelocity = Vector2.zero;
    }
    
    protected virtual void updateIdle()
    {
        if (Physics2D.OverlapCircle(transform.position, chaseRange, playerLayer))
            ChangeState(state.chase);
    }

    protected virtual void updateChase() 
    {
        if (!Physics2D.OverlapCircle(transform.position, chaseRange, playerLayer))
            ChangeState(state.idle);

        if (chaseTarget != null)
        {
            Vector2 dir = chaseTarget.position - transform.position;
            rb.linearVelocity = dir * chaseSpeed; 
        }

        if (Physics2D.OverlapCircle(transform.position, attackRange, playerLayer))
            ChangeState(state.attack);
    }

    protected virtual void exitAttack() 
    {
        rb.linearVelocity = Vector2.zero;
        // Debug.Log("attack-end");
    }

    protected virtual void enterAttack() 
    {
        rb.linearVelocity = Vector2.zero;
        startTime = Time.time;
        // Debug.Log("attack");
    }

    protected virtual void updateAttack() 
    {
        if (Time.time > startTime + attackTime)
            ChangeState(state.cooldown);
    }

    protected virtual void exitCooldown() 
    {  
        // Debug.Log("cooldown-end");
    }

    protected virtual void enterCooldown() 
    {
        // Debug.Log("enter-cooldown");
        startTime = Time.time;
    }

    protected virtual void updateCooldown() 
    {
        if (Time.time > startTime + cooldownTime)
            ChangeState(state.idle);
    }

    protected virtual void enterDeath() 
    {
        Destroy(this.gameObject);
    }

    public void OnDamage(int damage, MonoBehaviour reference = null)
    {
        if (isHurt)
            return;

        rb.linearVelocity = Vector2.zero;
        health -= damage;
        TimeController.instances.HitStop(0.05f);
        isHurt = true;
        StartCoroutine(hurtCoroutine());
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
}
