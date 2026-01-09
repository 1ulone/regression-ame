using UnityEngine;
using System;


public enum attackType
{
    melee,
    shoot,
    shotgun,
    magic,
    railgun,
    spawn,
}

[CreateAssetMenu(fileName = "new EnemyData", menuName = "Data/EnemyData")]
public class EnemyData : ScriptableObject 
{
    public int health;
    public int damage;
    public float moveSpeed;
    public attackType type;
    public passiveType passive;

    // public string attackPrefab;
    // public int attackCount = 1;

    public Action getAttackBehaviour(Vector2 pos, Vector2 dir, BaseEnemy mb)
    {
        switch(type)
        {
            case attackType.melee: 
            {
                return ()=> 
                {
                    string attackPrefab = "meleeAttack";

                    DamageComponent a = Pool.instances.CreateObject(attackPrefab, pos + (dir.normalized * 2), dir.normalized).GetComponent<DamageComponent>();
                    a.enemyReference = mb;
                    a.damage = damage;
                };
            }

            case attackType.shoot:
            {
                return ()=>
                {
                    string attackPrefab = "enemyBullet";

                    DamageComponent b = Pool.instances.CreateObject(attackPrefab, pos + (dir.normalized *2), Vector2.zero).GetComponent<DamageComponent>();
                    b.gameObject.GetComponent<Rigidbody2D>().linearVelocity = dir * 3; 
                    b.enemyReference = mb;
                    b.damage = damage;
                };
            }

            case attackType.shotgun: 
            {
                return ()=>
                {
                    string attackPrefab = "enemyBullet";
                    float spreadAngle = 30f;
                    float angleStep = spreadAngle / 2;
                    float startAngle = -spreadAngle / 2;

                    for (int i = 0; i < 3; i++)
                    {
                        float currentAngle = startAngle + (angleStep * i);
                        Vector2 currentDirection = Quaternion.Euler(0, 0, currentAngle) * dir.normalized;

                        DamageComponent b = Pool.instances.CreateObject(attackPrefab, pos + (dir.normalized *2), Vector2.zero).GetComponent<DamageComponent>();

                        b.gameObject.GetComponent<Rigidbody2D>().linearVelocity = currentDirection * 6 * 3; 
                        b.enemyReference = mb;
                        b.damage = damage;
                    }
                };
            }

            case attackType.railgun: 
            {
                return ()=> 
                {
                    string attackPrefab = "enemyRailgun";

                    float rotateDir = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    DamageComponent a = Pool.instances.CreateObject(attackPrefab, pos + (dir.normalized * 2), new Vector3(0, 0, rotateDir)).GetComponent<DamageComponent>();
                    a.enemyReference = mb;
                    a.damage = damage;
                };
            }

            case attackType.magic:
            {
                return ()=>
                {
                    string attackPrefab = "enemyMagicBullet";

                    DamageComponent b = Pool.instances.CreateObject(attackPrefab, pos + (dir.normalized *2), Vector2.zero).GetComponent<DamageComponent>();
                    MissileComponent m = b.GetComponent<MissileComponent>();

                    m.chaseSpeed = 20;
                    m.enemyRef = mb;
                    m.SetTarget("Player");

                    b.enemyReference = mb;
                    b.damage = damage;

                };
            }

            case attackType.spawn:
            {
                return ()=>
                {
                    string attackPrefab = "enemySpawnAttack";

                    Transform target = MonoBehaviour.FindFirstObjectByType<PlayerController>().transform;
                    DamageComponent a = Pool.instances.CreateObject(attackPrefab, target.position, Vector2.zero).GetComponent<DamageComponent>();
                    a.enemyReference = mb;
                    a.damage = damage;
                };
            }

            default: { return ()=> {}; };
        }
    }

    public PlayerBuffData[] data; 
}
