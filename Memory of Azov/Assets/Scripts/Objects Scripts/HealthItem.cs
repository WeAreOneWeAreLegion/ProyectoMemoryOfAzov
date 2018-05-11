using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour
{
    #region Public Variables
    [Header("\tGame Designers Variables")]
    [Tooltip("Cantidad de vida que da este objeto")]
    public int healthGiven = 50;
    #endregion

    #region Unity Triggers Method
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameManager.Instance.GetTagOfDesiredType(GameManager.TypeOfTag.Player))
        {
            other.GetComponent<PlayerController>().IncreaseHealth(healthGiven);
            ObjectsManager.Instance.ReturnRequest(gameObject, ObjectsManager.ItemRequest.Health);
        }    
    }
    #endregion
}
