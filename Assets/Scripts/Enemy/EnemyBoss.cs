using UnityEngine;
using System.Collections;
using System;

public class EnemyBoss : BaseEnemy 
{
    [SerializeField] protected Transform[] startSpawn;
    [SerializeField] protected BossUI ui;

    protected Vector3 dir;
    protected Vector3 currentPatternPosition;
    protected int magicFollowCount;
    public bool isSecondPhase { get; protected set; }

    protected override void Initialize()
    {
        base.Initialize();
        ui.setMaxHealth(data.health);
    }

    protected override void UpdateLogic()
    {
        if (!ui.startBossBattle)
            return;

        base.UpdateLogic();

        ui.UpdateHealthbar(health);
        if (health <= data.health/2 && !isSecondPhase)
        {
            isSecondPhase = true;
            cooldownTime = 0.5f;
        }
    }

    protected override void updateIdle()
    {
        ChangeState(state.attack);
    }

    protected override void updateChase()
    {
        ChangeState(state.attack);
    }

    protected override void enterAttack()
    {
        // dir = chaseTarget.position - transform.position;
        magicFollowCount = 3;

        StartCoroutine(attackCoroutine());
    }

    protected override void updateAttack() {}
    
    protected IEnumerator attackCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        int r = UnityEngine.Random.Range(0, 4);
        // int r = 2;
        if (r == 2)
        {
            for (int i = 0; i < magicFollowCount; i++)
            {
                getAttackBehaviour(r).Invoke();
                yield return new WaitForSeconds(2.5f);
            }
        } else 
        if (r == 3) 
        {
            if (isSecondPhase)
                getAttackBehaviour(0);
            foreach (Transform t in startSpawn)
            {
                currentPatternPosition = t.position;
                for (int i = 0; i < 10; i++)
                {
                    currentPatternPosition += new Vector3(
                        currentPatternPosition.x > transform.position.x ? 1.25f : -1.25f,
                        currentPatternPosition.y > transform.position.y ? 1.25f : -1.25f
                    );
                    getAttackBehaviour(r).Invoke();
                    // yield return new WaitForSeconds(1f);
                }
                // yield return new WaitForSeconds(1f);
            }
        } else { getAttackBehaviour(r).Invoke(); }

        yield return new WaitForSeconds(isSecondPhase ? 0.01f : 0.05f);
        ChangeState(state.cooldown);
    }

    protected Action getAttackBehaviour(int i)
    {
        Action a = ()=> {};
        switch (i)
        {
            // NOTE: BEAM ATTACK
            case 0 :
            {
                a = ()=> 
                {
                    dir = chaseTarget.position - transform.position;
                    string attackPrefab = "enemyRailgun";

                    float rotateDir = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    DamageComponent a = Pool.instances.CreateObject(attackPrefab, transform.position + (dir.normalized * 2), new Vector3(0, 0, rotateDir)).GetComponent<DamageComponent>();
                    a.enemyReference = this;
                    a.damage = 10;
                };
            } break;

            // NOTE: SPAWN ON PLAYER 
            case 1:
            {
                a = ()=> 
                {
                    string attackPrefab = "enemySpawnAttack";

                    Transform target = MonoBehaviour.FindFirstObjectByType<PlayerController>().transform;
                    DamageComponent a = Pool.instances.CreateObject(attackPrefab, target.position, Vector2.zero).GetComponent<DamageComponent>();

                    a.enemyReference = this;
                    a.damage = 3;
                };
            } break;

            // NOTE: MULTIPLE MAGIC FOLLOW
            case 2:
            {
                a = ()=> 
                {
                    string attackPrefab = "enemyMagicBullet";

                    DamageComponent b = Pool.instances.CreateObject(attackPrefab, transform.position + (dir.normalized *2), Vector2.zero).GetComponent<DamageComponent>();
                    MissileComponent m = b.GetComponent<MissileComponent>();
                    b.GetComponent<DestroyOnExitView>().enabled = false;

                    m.chaseSpeed = 20;
                    m.enemyRef = this;
                    m.SetTarget("Player");

                    b.enemyReference = this;
                    b.damage = 5;
                };
            } break;
            
            // NOTE: SPAWN ON PATTERN
            case 3:
            {
                a = ()=> 
                {
                    string attackPrefab = "enemySpawnAttack";

                    DamageComponent a = Pool.instances.CreateObject(attackPrefab, currentPatternPosition, Vector2.zero).GetComponent<DamageComponent>();
                    a.GetComponent<DestroyOnExitView>().enabled = false;

                    a.enemyReference = this;
                    a.damage = 3;
                };
            } break;

            // NOTE: MELEE ON CLOSE
            case -1:
            {
                a = ()=> 
                {

                };
            } break;
        }

        return a;
    }
}
