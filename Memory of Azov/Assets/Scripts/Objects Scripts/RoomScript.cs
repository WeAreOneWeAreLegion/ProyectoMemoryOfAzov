using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    #region Public Variables
    [Header("\t    Own Script Variables")]
    [Tooltip("Lista de muros oscuros")]
    public List<GameObject> blackWalls = new List<GameObject>();
    [Tooltip("Lista de objetos que sueltan enemigos")]
    public List<TransparentObject> objectsWithEnemies = new List<TransparentObject>();
    #endregion

    #region Enemy Spawner Method
    public void ShowAllEnemiesFromRoom()
    {
        foreach (TransparentObject t in objectsWithEnemies)
        {
            if (t.spawnGhost)
            {
                t.ShakeObjectAnimation(false);
            }
        }
    }
    #endregion

    #region Black Wall Methods
    public void HideBlackWalls()
    {
        blackWalls.ForEach(x => x.SetActive(false));
    }

    public void ShowBlackWalls()
    {
        blackWalls.ForEach(x => x.SetActive(true));
    }
    #endregion
}
