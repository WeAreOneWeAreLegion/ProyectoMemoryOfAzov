using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class FadingScript : MonoBehaviour {

    public Image blackImage;
    public Animator anim;
    public bool fade = false;

    void Start()
    {
        blackImage.gameObject.SetActive(true);
    }

    IEnumerator FadingIn()
    {
        fade = !fade;
        Debug.Log("hello?");
        anim.SetBool("Fade", fade); //I start the FadingIn animation
        yield return new WaitUntil(() => blackImage.color.a == 1); //I wait until alpha value is 1
    }

    /*IEnumerator FadingOut()
    {
        blackImage.gameObject.SetActive(true);
        anim.SetBool("Fade", false); //I start the FadingOut animation
        yield return new WaitUntil(() => blackImage.color.a == 0); //I wait until alpha value is 0
    }*/
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(FadingIn());
        }
    }
}
