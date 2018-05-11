using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsManager : MonoSingleton<ObjectsManager> {

    public enum ItemRequest { None, Gem, Health }

    #region Public Variables
    [Header("\tGame Designers Variables")]
    [Tooltip("Cantidad inicial de gemas para tener una pool inicial de gemas")]
    public int initialAmountOfGems = 1;
    [Tooltip("Cantidad inicial de objetos de vida para tener una pool inicial de objetos")]
    public int initialAmountOfHealths = 1;

    [Header("\t    Own Script Variables")]
    [Tooltip("Prefab de la gema")]
    public GameObject gemPrefab;
    [Tooltip("Prefab del objeto de vida")]
    public GameObject healthPrefab;
    #endregion

    #region Private Variables
    private List<GameObject> gemsPoolList = new List<GameObject>();
    private List<GameObject> healthPoolList = new List<GameObject>();
    #endregion

    private void Start()
    {
        CreateStarterItems();
    }

    #region Creation Methods
    private void CreateStarterItems()
    {
        CreateStarterGems();
        CreateStarterHealths();
    }

    private void CreateStarterGems()
    {
        for (int i = 0; i < initialAmountOfGems; i++)
        {
            GameObject go = Instantiate(gemPrefab, Vector3.one * 9999, Quaternion.identity) as GameObject;
            go.transform.SetParent(this.transform);
            go.SetActive(false);
            gemsPoolList.Add(go);
        }
    }

    private void CreateStarterHealths()
    {
        for (int i = 0; i < initialAmountOfHealths; i++)
        {
            GameObject go = Instantiate(healthPrefab, Vector3.one * 9999, Quaternion.identity) as GameObject;
            go.transform.SetParent(this.transform);
            go.SetActive(false);
            healthPoolList.Add(go);
        }
    }
    #endregion

    #region Item Managment Methods
    public GameObject GetItem(Transform callTransform, ItemRequest rq)
    {
        if (rq == ItemRequest.None)
            return null;

        Ray ray = new Ray(callTransform.position + Vector3.up, Vector3.down);

        RaycastHit hit;

        Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("FloorLayer"));

        switch (rq)
        {
            case ItemRequest.Gem:
                return GetGem(hit.transform.parent);
            case ItemRequest.Health:
                return GetHealth(hit.transform.parent);
            default:
                return null;
        }
    }

    private GameObject GetGem (Transform parent)
    {
        GameObject g;

        if (gemsPoolList.Count == 0)
        {
            //Create
            g = Instantiate(gemPrefab, Vector3.one * 9999, Quaternion.identity) as GameObject;
        }
        else
        {
            //Get the first and remove it
            g = gemsPoolList[0];
            gemsPoolList.Remove(g);
        }

        g.transform.SetParent(parent);
        g.SetActive(true);

        return g;
    }

    private GameObject GetHealth(Transform parent)
    {
        GameObject g;

        if (healthPoolList.Count == 0)
        {
            //Create
            g = Instantiate(healthPrefab, Vector3.one * 9999, Quaternion.identity) as GameObject;
        }
        else
        {
            //Get the first and remove it
            g = healthPoolList[0];
            healthPoolList.Remove(g);
        }

        g.transform.SetParent(parent);
        g.SetActive(true);

        return g;
    }

    public void ReturnRequest(GameObject g, ItemRequest rq)
    {
        //Reset ghost
        g.transform.position = Vector3.one * 9999;
        g.transform.SetParent(this.transform);
        g.SetActive(false);

        switch (rq)
        {
            case ItemRequest.Gem:
                gemsPoolList.Add(g);
                break;
            case ItemRequest.Health:
                healthPoolList.Add(g);
                break;
        }
    }
    #endregion
}
