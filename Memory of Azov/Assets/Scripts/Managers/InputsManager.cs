using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputsManager : MonoSingleton<InputsManager> {

    public enum XboxInputs {
        LeftJoystickHorizontal,
        LeftJoystickVertical,
        RightJoystickHorizontal,
        RightJoystickVertical,
        AButton,
        BButton,
        XButton,
        YButton,
        RTrigger,
        LTrigger,
        StartButton
    }

    [System.Serializable]
    public struct GameInputsXbox
    {
        public XboxInputs moveHorizontal;
        public XboxInputs moveVertical;
        public XboxInputs rotateHorizontal;
        public XboxInputs rotateVertical;
        public XboxInputs actionButton;
        public XboxInputs switchLanternButton;
        public XboxInputs changeColorButton;
        public XboxInputs chargeTrigger1;
        public XboxInputs chargeTrigger2;
        public XboxInputs pauseButton;
    }

    [System.Serializable]
    public struct GameInputsPc
    {
        public string moveHorizontal;
        public string moveVertical;
        public string rotateHorizontal;
        public string rotateVertical;
        public KeyCode actionButton;
        public KeyCode switchLanternButton;
        public KeyCode changeColorButton;
        public string chargeTrigger;
        public KeyCode pauseButton;
    }

    #region Public Variables
    [Header("\t         --Inputs Zone--")]
    [Header("Inputs")]
    public GameInputsXbox xboxInputs;
    //public GameInputs playInputs;
    public GameInputsPc pcInputs;

    [Header("Input Variables")]
    [Tooltip("Esta jugando con controlador, caso contrario controles de pc")]
    public bool isControllerPlaying;
    [Tooltip("Invertir el control vertical del joystick de rotacion")]
    public bool invertVerticalRotation;
    [Tooltip("Cuantas veces girara mas rapido el joystick")]
    [Range(1, 10)]
    public float joystickRotationFactor = 3;
    #endregion

    #region Input Methods
    public void InvertY()
    {
        invertVerticalRotation = !invertVerticalRotation;
    }

    public void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public float GetMovementX()
    {
        return isControllerPlaying ? Input.GetAxis(xboxInputs.moveHorizontal.ToString()) : Input.GetAxisRaw(pcInputs.moveHorizontal.ToString());
    }

    public float GetMovementY()
    {
        return isControllerPlaying ? Input.GetAxis(xboxInputs.moveVertical.ToString()) : Input.GetAxisRaw(pcInputs.moveVertical.ToString());
    }

    public float GetRotationX()
    {
        return isControllerPlaying ? Input.GetAxis(xboxInputs.rotateHorizontal.ToString()) * joystickRotationFactor : Input.GetAxisRaw(pcInputs.rotateHorizontal.ToString());
    }

    public float GetRotationY()
    {
        return isControllerPlaying ? (invertVerticalRotation ? -Input.GetAxis(xboxInputs.rotateVertical.ToString()) * joystickRotationFactor : Input.GetAxis(xboxInputs.rotateVertical.ToString()) * joystickRotationFactor) : (invertVerticalRotation ? -Input.GetAxisRaw(pcInputs.rotateVertical.ToString()) : Input.GetAxisRaw(pcInputs.rotateVertical.ToString()));
    }

    public bool GetActionButtonInputDown()
    {
        return isControllerPlaying ? Input.GetButtonDown(xboxInputs.actionButton.ToString()) : Input.GetKeyDown(pcInputs.actionButton);
    }

    public bool GetSwitchButtonInput()
    {
        return isControllerPlaying ? Input.GetButton(xboxInputs.switchLanternButton.ToString()) : Input.GetKey(pcInputs.switchLanternButton);
    }

    public bool GetChangeColorButtonInputDown()
    {
        return isControllerPlaying ? Input.GetButtonDown(xboxInputs.changeColorButton.ToString()) : Input.GetKeyDown(pcInputs.changeColorButton);
    }

    public bool GetIntensityButtonDown()
    {
        return isControllerPlaying ? Input.GetAxisRaw(xboxInputs.chargeTrigger1.ToString()) > 0.2f || Input.GetAxisRaw(xboxInputs.chargeTrigger2.ToString()) > 0.2f : Input.GetButtonDown(pcInputs.chargeTrigger.ToString());
    }

    public bool GetIntensityButtonUp()
    {
        return isControllerPlaying ? Input.GetAxisRaw(xboxInputs.chargeTrigger1.ToString()) < 0.2f && Input.GetAxisRaw(xboxInputs.chargeTrigger2.ToString()) < 0.2f : Input.GetButtonUp(pcInputs.chargeTrigger.ToString());
    }

    public bool GetStartButtonDown()
    {
        return isControllerPlaying ? Input.GetButtonUp(xboxInputs.pauseButton.ToString()) : Input.GetKeyDown(pcInputs.pauseButton);
    }
    #endregion

}
