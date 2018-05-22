using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggerer : MonoBehaviour {

    #region Public Variables
    public EventClass myEvent;
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameManager.Instance.GetTagOfDesiredType(GameManager.TypeOfTag.Player))
        {
            myEvent.EventAction();

            gameObject.SetActive(false);
        }
    }
}
