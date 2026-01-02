using UnityEngine;

public class EnemyMelee : BaseEnemy 
{
    protected GameObject attackObject;

    protected override void enterAttack()
    {
        base.enterAttack();

        Vector3 dir = chaseTarget.position - transform.position;
        attackObject = Pool.instances.CreateObject("enemyMeleeAttack", transform.position + (dir.normalized * 2f), dir);
    }

    protected override void exitAttack()
    {
        base.exitAttack();
        Pool.instances.DestroyObject(attackObject);
        attackObject = null;
    }
}
