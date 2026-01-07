using UnityEngine;

public class EnemyMelee : BaseEnemy 
{
    protected override void enterAttack()
    {
        base.enterAttack();

        Vector3 dir = chaseTarget.position - transform.position;
        data.getAttackBehaviour(transform.position, dir, this as BaseEnemy).Invoke();
    }
}
