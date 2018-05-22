using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputsTest : MonoBehaviour {

    public float speed = 10;

	// Update is called once per frame
	void Update ()
    {
        float xArrow = Input.GetAxisRaw("ArrowsVertical");

        Debug.Log(xArrow);
	}
}
