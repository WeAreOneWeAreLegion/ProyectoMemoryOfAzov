using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwakeState_N : IEnemyState
{
    private Enemy enemy;

    public void Enter(Enemy e)
    {   
        enemy = e;

        enemy.PlayAwakenSound();
        enemy.Invencible();

        enemy.StopMovement();
    }

    public void Execute()
    {

        if (!enemy.IsSoundPlaying())
            enemy.ChangeState(new ChaseState_N());

    }

    public void Exit()
    {
        enemy.Vulnerable();
    }
}