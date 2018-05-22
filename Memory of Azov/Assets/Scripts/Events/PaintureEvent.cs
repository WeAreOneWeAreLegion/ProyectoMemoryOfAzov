using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintureEvent : EventClass {

    public Animation myAnimation;

    public override void EventAction()
    {
        myAnimation.Play();
    }

}
