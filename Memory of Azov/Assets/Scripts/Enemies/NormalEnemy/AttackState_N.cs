using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState_N : IEnemyState
{
    private Enemy enemy;

    private float attackTimer;
    private bool isAttacking;

    public void Enter(Enemy e)
    {
        enemy = e;

        attackTimer = 0;
        isAttacking = true;

        enemy.StopMovement();

        enemy.SetTarget(GameManager.Instance.GetPlayer());
        enemy.ChangeAnimation(Enemy.AnimationState.Attack);
    }

    public void Execute()
    {
        if (isAttacking)
        {
            if (!enemy.IsAttackAnimationPlaying())
            {
                isAttacking = false;
                attackTimer = 0;
            }
        }
        else
        {
            if (!enemy.IsInAttackRadius())
            {
                enemy.ChangeState(new ChaseState_N());
                return;
            }

            attackTimer += Time.deltaTime;

            if (attackTimer >= enemy.GetAttackDelay())
            {
                isAttacking = true;
                enemy.ChangeAnimation(Enemy.AnimationState.Attack);
            }
        }

        enemy.RotateToTarget();
    }

    public void Exit()
    {
    }
}
