using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemObject : MonoBehaviour
{
    #region Public Variables
    [Header("\tGame Designers Variables")]
    [Tooltip("Fuerza de salida al aparecer")]
    [Range(1, 10)] public float initialForce = 5f;
    [Tooltip("Tiempo que tardara en autodestruirse si se es una gema que se auto recoje")]
    [Range(1, 10)] public float timeForAutoDestroy = 1f;

    [Header("\t    Own Script Variables")]
    [Tooltip("Referencia al Rigidbody")]
    public Rigidbody myRGB;
    #endregion

    #region Private Variables
    private bool autoDestroy;
    private float timeToDesapear;
    #endregion

    private void Start()
    {
        if(myRGB == null)
            myRGB = GetComponent<Rigidbody>();

        if (myRGB.useGravity)
            myRGB.AddForce(transform.forward * initialForce, ForceMode.Impulse);

    }

    private void Update()
    {
        if (autoDestroy)
        {
            timeToDesapear += Time.deltaTime;

            if (timeToDesapear >= timeForAutoDestroy)
            {
                timeToDesapear = 0;
                DestroyGem();
            }
        } 
    }

    #region Destroy Method
    private void DestroyGem()
    {
        GameManager.Instance.IncreaseNumOfGems();
        ObjectsManager.Instance.ReturnRequest(gameObject, ObjectsManager.ItemRequest.Gem);
    }
    #endregion

    #region Discover Method
    public void DiscoveredByFeature()
    {
        if (myRGB == null)
            myRGB = GetComponent<Rigidbody>();

        myRGB.velocity = Vector3.zero;
        myRGB.useGravity = false;
        myRGB.AddForce(Vector3.up * 2, ForceMode.Impulse);

        GetComponent<Collider>().enabled = false;

        autoDestroy = true;
    }
    #endregion

    #region Unity Collision/Trigger Methods
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == GameManager.Instance.GetTagOfDesiredType(GameManager.TypeOfTag.Player))
        {
            DestroyGem();
        }
        else
        {
            myRGB.isKinematic = true;
            GetComponent<Collider>().isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameManager.Instance.GetTagOfDesiredType(GameManager.TypeOfTag.Player))
        {
            DestroyGem();
        }
    }
    #endregion
}
