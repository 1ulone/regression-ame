using UnityEngine;
using System;


public enum attackType
{
    melee,
    shoot,
}

[CreateAssetMenu(fileName = "new EnemyData", menuName = "Data/EnemyData")]
public class EnemyData : ScriptableObject 
{
    public attackType type;
    public string attackPrefab;
    public int attackCount = 1;
    public float attackSpeed = 0;

    public Action getAttackBehaviour(Vector2 pos, Vector2 dir, BaseEnemy mb)
    {
        switch(type)
        {
            case attackType.melee: 
            {
                return ()=> 
                {
                    for (int i = 0; i < attackCount; i++)
                    {
                        GameObject a = Pool.instances.CreateObject(attackPrefab, pos + (dir.normalized * 2), dir.normalized);
                        a.GetComponent<DamageComponent>().enemyReference = mb;
                    }
                };
            }

            case attackType.shoot:
            {
                return ()=>
                {
                    for (int i = 0; i < attackCount; i++)
                    {
                        GameObject b = Pool.instances.CreateObject(attackPrefab, pos + (dir.normalized *2), Vector2.zero);
                        b.GetComponent<Rigidbody2D>().linearVelocity = dir * mb.attackSpeed; 
                        b.GetComponent<DamageComponent>().enemyReference = mb;
                    }
                };
            }

            default: { return ()=> {}; };
        }
    }

    public PlayerBuffData[] data; 
}
