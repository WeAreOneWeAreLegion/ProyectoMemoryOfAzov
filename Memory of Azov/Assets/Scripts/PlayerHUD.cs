using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {

    #region Public Variables
    [Header("\tGame Designers Variables")]
    [Header("Position Variables")]
    public Vector3 panelOffset;

    [Header("\t    Own Script Variables")]
    public Image buttonImage;
    [Tooltip("1.A, 2.B, 3.X, 4.Y, 5.RB, 6.LB, 7.RT, 8.LT")]
    public Sprite[] buttonImageArray;
    #endregion

    #region Private Variables
    private Transform target;
    private RectTransform myRectTransform;

    private float incrementFactorX;
    private float incrementFactorY;
    #endregion

    private void Start()
    {
        incrementFactorX = GameManager.Instance.GetCanvasResolution().x / Screen.width;
        incrementFactorY = GameManager.Instance.GetCanvasResolution().y / Screen.height;

        target = GameManager.Instance.GetPlayer();
        myRectTransform = GetComponent<RectTransform>();

        HideImage();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;

        Vector3 targetScreenPos = Camera.main.WorldToScreenPoint(target.position);
        targetScreenPos.x += -myRectTransform.sizeDelta.x / 2 + panelOffset.x;
        targetScreenPos.x *= incrementFactorX;
        targetScreenPos.y += -myRectTransform.sizeDelta.y / 2 + panelOffset.y;
        targetScreenPos.y *= incrementFactorY;
        myRectTransform.anchoredPosition = targetScreenPos;
    }

    #region Button Methods
    public void ShowSpecificButton(GameManager.ButtonRequest bReq)
    {
        buttonImage.sprite = buttonImageArray[(int)bReq];
        buttonImage.gameObject.SetActive(true);
    }

    public void HideImage()
    {
        buttonImage.gameObject.SetActive(false);
    }

    public bool GetIsActive()
    {
        return buttonImage.gameObject.activeInHierarchy;
    }
    #endregion
}
