using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState_N : IEnemyState
{
    private Enemy enemy;

    public void Enter(Enemy e)
    {
        enemy = e;

        enemy.SetTarget(GameManager.Instance.GetPlayer());

        enemy.ChangeAnimation(Enemy.AnimationState.Move);
    }

    public void Execute()
    {
        if (enemy.IsInAttackRadius())
        {
            enemy.ChangeState(new AttackState_N());
            return;
        }

        enemy.MoveToTarget();
        enemy.RotateToTarget();
    }

    public void Exit()
    {
    }
}