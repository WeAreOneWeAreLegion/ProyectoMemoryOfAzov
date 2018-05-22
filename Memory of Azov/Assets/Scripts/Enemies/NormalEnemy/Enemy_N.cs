using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_N : Enemy
{
    #region Override Methods
    protected override void Start()
    {
        base.Start();

        ChangeState(new AwakeState_N());
    }

    public override void SetUpEnemyVariables(EnemySO enemyData)
    {
        base.SetUpEnemyVariables(enemyData);


        if (currentState != null)
        {
            ChangeState(new AwakeState_N());
            Debug.Log("SetUpByVariables");
        }
    }

    public override void RecieveDamage()
    {
        if (currentState.GetType() != typeof(ScapeState_N))
        {
            ChangeState(new ScapeState_N());
        }

        base.RecieveDamage();
    }
    #endregion
}
