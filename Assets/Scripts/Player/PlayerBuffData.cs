using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "new Buff Data", menuName = "Data/BuffData")]
public class PlayerBuffData : ScriptableObject 
{
    public Sprite icon;
    public string tag;

    public int health; 
    public float moveSpeed; 
    public float rollMultiplier;
    public float rollTime;
    public float rollCooldownTime; 
    public float attackTime;
    public float attackCooldownTime;
    public float randomShootMultiplier;
    public float bulletSpeed;

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
}
