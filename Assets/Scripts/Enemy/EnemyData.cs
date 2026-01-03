using UnityEngine;
using System;


public enum attackType
{
    melee,
    shoot,
}

public class EnemyData : ScriptableObject 
{
    public attackType type;
    public string attackPrefab;
    public int attackCount = 1;

    public Action getAttackBehaviour(Vector2 dir)
    {
        switch(type)
        {
            case attackType.melee: 
            {
                return ()=> 
                {
                    for (int i = 0; i < attackCount; i++)
                    {
                        GameObject a = Pool.instances.CreateObject(attackPrefab, dir, Vector2.zero);
                        a.transform.LookAt(dir);
                    }
                };
            }

            case attackType.shoot:
            {
                return ()=>
                {
                    for (int i = 0; i < attackCount; i++)
                    {
                        GameObject b = Pool.instances.CreateObject(attackPrefab, dir, Vector2.zero);
                        b.GetComponent<Rigidbody2D>().linearVelocity = dir * data.bulletSpeed; 
                    }
                };
            }

            default: { return ()=> {}; };
        }
    }

    public PlayerBuffData data; 
}
