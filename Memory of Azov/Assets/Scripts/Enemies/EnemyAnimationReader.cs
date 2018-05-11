using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationReader : MonoBehaviour {

    #region Public Variables
    [Header("\t    Own Script Variables")]
    [Tooltip("Enemigo sobre el que las animaciones haran efecto")]
    public Enemy myEnemy;
    #endregion

    private void Start()
    {
        if (myEnemy == null)
            myEnemy = transform.parent.GetComponent<Enemy>();
    }

    #region Animation Reader Methods
    public void Attack()
    {
        myEnemy.DoDamage();
    }
    #endregion
}
