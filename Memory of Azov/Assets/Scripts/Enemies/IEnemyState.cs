using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyState {

    void Enter(Enemy ghost);
    void Execute();
    void Exit();

}
