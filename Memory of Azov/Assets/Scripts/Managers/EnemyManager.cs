using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoSingleton<EnemyManager> {

    #region Public Variables
    [Header("\tGame Designers Variables")]
    [Tooltip("Cantidad inicial de fantasmas para tener una pool inicial de fantasmas")]
    public int initialAmountOfGhosts = 10;
    [Tooltip("Distancia que aparecera el fantasma desde el suelo")]
    public float enemyFloorYOffset = 1.6f;

    [Header("\t    Own Script Variables")]
    [Tooltip("Prefab del fantasma")]
    public GameObject ghostPrefab;
    #endregion

    #region Private Variables
    private int enemyGivenCounter = 0;

    private List<GameObject> enemyPoolList = new List<GameObject>();
    #endregion

    private void Start()
    {
        CreateStarterGhosts();
    }

    #region Creation Method
    private void CreateStarterGhosts()
    {
        for (int i = 0; i < initialAmountOfGhosts; i++)
        {
            GameObject go = Instantiate(ghostPrefab, Vector3.one * 9999, Quaternion.identity) as GameObject;
            go.transform.SetParent(this.transform);
            go.SetActive(false);
            enemyPoolList.Add(go);
        }
    }
    #endregion

    #region Enemy Managment Methods
    public GameObject GetEnemy(Transform parent, EnemySO enemyData)
    {
        GameObject g;

        if (enemyPoolList.Count == 0)
        {
            //Create
            g = Instantiate(ghostPrefab, Vector3.one * 9999, Quaternion.identity) as GameObject;
        }
        else
        {
            //Get the first and remove it
            g = enemyPoolList[0];
            enemyPoolList.Remove(g);
        }

        g.transform.SetParent(parent);
        g.SetActive(true);

        if (enemyGivenCounter == 0)
        {
            GameManager.Instance.BlockPlayerDoors();
        }

        g.GetComponent<Enemy>().SetUpEnemyVariables(enemyData);
        enemyGivenCounter++;

        return g;
    }

    public void ReturnEnemy(GameObject g)
    {
        //Reset ghost
        g.transform.position = Vector3.one * 9999;
        g.transform.SetParent(this.transform);
        g.SetActive(false);

        enemyPoolList.Add(g);
        enemyGivenCounter--;

        if (enemyGivenCounter == 0)
        {
            GameManager.Instance.UnblockPlayerDoors();
        }
    }
    #endregion

}
