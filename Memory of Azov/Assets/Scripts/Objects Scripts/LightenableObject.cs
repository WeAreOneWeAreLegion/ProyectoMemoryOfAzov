using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightenableObject : MonoBehaviour {

    #region Inner Methods
    public virtual void InsideLanternRange()
    {
    }

    public virtual void OutsideLanternRange()
    {
    }

    public virtual bool IsInSight()
    {
        return false;
    }
    #endregion

}
