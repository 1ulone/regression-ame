using UnityEngine;

public class EnemyMelee : BaseEnemy 
{
    [SerializeField] protected float chargeTime = 2.5f;
    protected bool chargeDone = false;
    protected Vector3 dir;

    protected override void enterAttack()
    {
        base.enterAttack();
        chargeDone = false;

        anim.Play("telegraph");
        Pool.instances.CreateObject("telegraph", transform.position + ( Vector3.right*0.25f ), Vector2.zero);
    }

    protected override void updateAttack()
    {
        if (Time.time > startTime + chargeTime && chargeDone == false)
        {
            startTime = Time.time;
            chargeDone = true;
            dir = chaseTarget.position - transform.position;
            melee();
        }
    }

    protected void melee()
    {
        anim.Play("attack");
        data.getAttackBehaviour(transform.position, dir, this as BaseEnemy).Invoke();
    }

    protected override void exitAttack()
    {
        base.exitAttack();
        anim.Play("idle");
    }

    public void endAttack()
    {
        ChangeState(state.cooldown);
    }

}
