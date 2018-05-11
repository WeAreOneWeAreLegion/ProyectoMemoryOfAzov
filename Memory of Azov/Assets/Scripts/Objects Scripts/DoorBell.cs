using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBell : MonoBehaviour {

    #region Public Variables
    [Header("\t    Own Script Variables")]
    [Tooltip("Puerta que accionara este timbre")]
    public ConectionScript myDoor;
    #endregion

    #region Open Door Method
    public void OpenDoor()
    {
        myDoor.OpenByBell();
        enabled = false;
        tag = GameManager.Instance.GetTagOfDesiredType(GameManager.TypeOfTag.Wall);
    }
    #endregion
}
