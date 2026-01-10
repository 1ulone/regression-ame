using UnityEngine;
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
}
