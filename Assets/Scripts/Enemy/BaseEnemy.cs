using UnityEngine;

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
    protected float startTime;
    protected int health;
    
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float chaseSpeed = 4;
    [SerializeField] private float chaseRange = 10;
    [SerializeField] private float attackRange = 2;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float attackTime = 1;
    [SerializeField] private float cooldownTime = 2;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        chaseTarget = GameObject.FindFirstObjectByType<PlayerController>().transform; 
        health = maxHealth;

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

    private void UpdateLogic()
    {
        switch(_state)
        {
            case state.idle: updateIdle(); break; 
            case state.chase: updateChase(); break; 
            case state.attack: updateAttack(); break; 
            case state.cooldown: updateCooldown(); break; 
        }
    }

    protected void transitionIdle() 
    {
        rb.linearVelocity = Vector2.zero;
    }
    
    protected void updateIdle()
    {
        if (Physics2D.OverlapCircle(transform.position, chaseRange, playerLayer))
            ChangeState(state.chase);
    }

    protected void updateChase() 
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
        Debug.Log("attack-end");
    }

    protected virtual void enterAttack() 
    {
        rb.linearVelocity = Vector2.zero;
        startTime = Time.time;
        Debug.Log("attack");
    }

    protected void updateAttack() 
    {
        if (Time.time > startTime + attackTime)
            ChangeState(state.cooldown);
    }

    protected void exitCooldown() 
    {  
        Debug.Log("cooldown-end");
    }

    protected void enterCooldown() 
    {
        Debug.Log("enter-cooldown");
        startTime = Time.time;
    }

    protected void updateCooldown() 
    {
        if (Time.time > startTime + cooldownTime)
            ChangeState(state.idle);
    }

    protected void enterDeath() 
    {
        Destroy(this.gameObject);
    }

    public void OnDamage(int damage)
    {
        rb.linearVelocity = Vector2.zero;
        health -= damage;
        TimeController.instances.HitStop(0.05f);
    }
}
