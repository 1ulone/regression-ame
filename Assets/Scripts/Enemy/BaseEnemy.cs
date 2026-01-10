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
    protected float chaseSpeed;
    
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

    protected void exitCooldown() 
    {  
        // Debug.Log("cooldown-end");
    }

    protected void enterCooldown() 
    {
        // Debug.Log("enter-cooldown");
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

    public void OnDamage(int damage, MonoBehaviour reference = null)
    {
        rb.linearVelocity = Vector2.zero;
        health -= damage;
        TimeController.instances.HitStop(0.05f);
    }
}
