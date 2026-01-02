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
    public int attackCount;

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
                };
            }

            default: { return ()=> {}; };
        }
    }

    public PlayerBuffData data; 
}
