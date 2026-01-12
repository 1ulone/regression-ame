using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

public enum passiveType
{
    none,
    bulletHell,
    shockwave,
    randomSpawn
}
//needs better name fossho

[CreateAssetMenu(fileName = "new Buff Data", menuName = "Data/BuffData")]
[System.Serializable]
public class PlayerBuffData : ScriptableObject 
{
    public Sprite icon;
    public string tag;

    public int health; 
    public int attack;
    public float speed; 

    public attackType behaviour;
    public passiveType passive; 

    public Dictionary<string, float> GetNonZeroValues()
    {
        var result = new Dictionary<string, float>();

        FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(int))
            {
                int value = (int)field.GetValue(this);
                if (value != 0)
                    result.Add(field.Name, value);
            }
            else if (field.FieldType == typeof(float))
            {
                float value = (float)field.GetValue(this);
                if (Mathf.Abs(value) > 0f)
                    result.Add(field.Name, value);
            }
        }

        return result;
    }
    
    public string GetDescription()
    {
        string h = health == 0 ? "" : (health > 0 ? "health +"+health.ToString() : "health -"+health.ToString());
        string a = attack == 0 ? "" : (attack > 0 ? "attack +"+attack.ToString() : "attack -"+attack.ToString());
        string s = speed == 0 ? "" : (speed > 0 ? "speed +"+speed.ToString() : "speed -"+speed.ToString());

        string b = "";
        switch (behaviour)
        {
            case attackType.melee : { b = ""; } break;
            case attackType.shoot : { b = "Weapon->Default gun"; } break;
            case attackType.shotgun : { b = "Weapon->Reliable Shotgun"; } break;
            case attackType.railgun : { b = "Weapon->White-Stripe Rifle"; } break;
        }
        
        string p = "";
        switch (passive)
        {
            case passiveType.none : { p = ""; } break;
            case passiveType.bulletHell : { p = "Spawns 3 Magic Bullets around ame every 5s"; } break;
            case passiveType.randomSpawn : { p = "Spawn tako tentacle on Random nearby Enemy for every 5s"; } break;
            case passiveType.shockwave : { p = "Randomly Throws Ame-nade to a random Direction every 3s"; } break;
        }

        return h + "\n" + a + "\n" + s + "\n" + "\n" + b + "\n" + p;

    }

    public Action GetAttackBehaviour(Vector2 dir, int damage, Vector2 pos)
    {
        switch (behaviour) 
        {
            case attackType.shoot : 
            {
                return ()=> 
                {
                    string attackPrefab = "playerBullet";

                    DamageComponent b = Pool.instances.CreateObject(attackPrefab, pos, Vector2.zero).GetComponent<DamageComponent>();
                    b.gameObject.GetComponent<Rigidbody2D>().linearVelocity = dir * 3; 
                    b.damage = damage;
                };
            }

            case attackType.shotgun : 
            {
                return ()=>
                {
                    string attackPrefab = "playerBullet";
                    float spreadAngle = 30f;
                    float angleStep = spreadAngle / 2;
                    float startAngle = -spreadAngle / 2;

                    for (int i = 0; i < 3; i++)
                    {
                        float currentAngle = startAngle + (angleStep * i);
                        Vector2 currentDirection = Quaternion.Euler(0, 0, currentAngle) * dir.normalized;

                        DamageComponent b = Pool.instances.CreateObject(attackPrefab, pos, Vector2.zero).GetComponent<DamageComponent>();

                        b.gameObject.GetComponent<Rigidbody2D>().linearVelocity = currentDirection * 6 * 3; 
                        b.damage = damage;
                    }

                };
            }
            
            case attackType.railgun :
            {
                return ()=>
                {
                    string attackPrefab = "playerRailgun";

                    float rotateDir = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    DamageComponent a = Pool.instances.CreateObject(attackPrefab, pos + (dir.normalized * 2), new Vector3(0, 0, rotateDir)).GetComponent<DamageComponent>();
                    a.damage = damage;

                };
            }

            default :
            {
                return ()=> {};
            }
        }
    }


    public Action GetPassiveBehaviour(int damage, Vector2 pos, LayerMask enemy)
    {
        switch (passive)
        {
            case passiveType.bulletHell :
            {
                return ()=> 
                {
                    float bulletPerRound = 8f;
                    string attackPrefab = "playerBullet";
                    float angleStep = 45;
                    float startAngle = 0;

                    for (int i = 0; i < bulletPerRound; i++)
                    {
                        float currentAngle = startAngle + (angleStep * i);
                        Vector2 currentDirection = Quaternion.Euler(0, 0, currentAngle) * Vector2.right;

                        DamageComponent b = Pool.instances.CreateObject(attackPrefab, pos + (currentDirection*2), Vector2.zero).GetComponent<DamageComponent>();

                        b.gameObject.GetComponent<Rigidbody2D>().linearVelocity = currentDirection * 3 * 3; 
                        b.damage = damage;
                    }
                };
            }

            case passiveType.randomSpawn :
            {
                return ()=> 
                {
                    string attackPrefab = "playerSpawnAttack";

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, 16, enemy);
                    if (colliders.Length > 0)
                    {
                        foreach (Collider2D c in colliders)
                        {
                            if (c.TryGetComponent<BaseEnemy>(out BaseEnemy e))
                            {
                                DamageComponent a = Pool.instances.CreateObject(attackPrefab, e.transform.position, Vector2.zero).GetComponent<DamageComponent>();
                                a.damage = damage;
                            }
                        }
                    }
                };
            }

            case passiveType.shockwave :
            {
                return ()=> 
                {
                    string attackPrefab = "playerShockwave";
                    DamageComponent b = Pool.instances.CreateObject(attackPrefab, pos, Vector2.zero).GetComponent<DamageComponent>();
                    b.damage = damage;
               };
            }

            default : { return ()=> {}; }
        }
    }
}
