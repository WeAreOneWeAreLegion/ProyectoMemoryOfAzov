using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputsTest : MonoBehaviour {

    public float speed = 10;

	// Update is called once per frame
	void Update ()
    {
        Vector3 direction = new Vector3(Input.GetAxis("LeftJoystickHorizontal"), 0, Input.GetAxis("LeftJoystickVertical"));

        direction.Normalize();

        if (Input.GetButtonDown("AButton"))
        {
            Debug.Log(direction);
        }

        transform.position += direction * speed * Time.deltaTime;
	}
}
