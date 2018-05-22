using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBell : MonoBehaviour {

    #region Public Variables
    [Header("\tGame Designers Variables")]
    [Tooltip("Si es fake spawnea fantasmas")]
    public bool isFakeBell;
    [Tooltip("Posiciones donde apareceran los fantasmas, por transform habra un fantasma")]
    public List<Transform> ghostSpawns = new List<Transform>();
    public List<EnemySO> enemiesData = new List<EnemySO>();
    [Header("\t    Own Script Variables")]
    [Tooltip("Puerta que accionara este timbre")]
    public ConectionScript myDoor;
    #endregion

    #region Open Door Method
    public void OpenDoor()
    {
        if (isFakeBell)
        {
            for (int i = 0; i < ghostSpawns.Count; i++)
            {
                Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);

                RaycastHit hit;

                Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("FloorLayer"));

                GameObject go = EnemyManager.Instance.GetEnemy(hit.transform != null ? hit.transform.parent : this.transform, enemiesData[i]);

                go.transform.position = ghostSpawns[i].position;
                go.transform.forward = GameManager.Instance.GetPlayer().position - transform.position;
            }
        }
        else
        {
            myDoor.OpenByBell();
        }

        enabled = false;
        tag = GameManager.Instance.GetTagOfDesiredType(GameManager.TypeOfTag.Wall);
    }
    #endregion
}
