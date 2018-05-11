using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSpawnEvent : MonoBehaviour {

    #region Public Variables
    [Header("\tGame Designers Variables")]
    [Tooltip("Tiene mas de un evento hermano?")]
    public bool hasMoreTriggersThisEvent;
    [Tooltip("La referencia de como sera el fantasma a generar")]
    public EnemySO enemyData;

    [Header("\t    Own Script Variables")]
    [Tooltip("Donde debe aparecer el fantasma")]
    public Transform spawnPosition;
    [Tooltip("Eventos hermanos si los hubiera")]
    public List<GhostSpawnEvent> theOtherEvents;
    #endregion

    #region Private Variables
    private bool ghostSpawned;
    #endregion

    private void TriggerEvent()
    {
        if (ghostSpawned)
        {
            return;
        }

        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);

        RaycastHit hit;

        Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("FloorLayer"));

        GameObject go = EnemyManager.Instance.GetEnemy(hit.transform != null ? hit.transform.parent : this.transform, enemyData);

        go.transform.position = new Vector3(spawnPosition.position.x, hit.point.y + EnemyManager.Instance.enemyFloorYOffset, spawnPosition.position.z);

        go.transform.forward = spawnPosition.forward;

        ghostSpawned = true;
        theOtherEvents.ForEach(x => x.gameObject.SetActive(false));
        gameObject.SetActive(false);
    }

    #region Unity Trigger Method
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameManager.Instance.GetTagOfDesiredType(GameManager.TypeOfTag.Player))
        {
            TriggerEvent();
        }
    }
    #endregion
}
