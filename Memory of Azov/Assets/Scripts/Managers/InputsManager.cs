﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

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
        RButton,
        LButton,
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
    [HideInInspector]
    public float leftPlayer1Vibrator; //Variable para poder vibrar valor izquierdo
    [HideInInspector]
    public float rightPlayer1Vibrator; //Variable para poder vibrar valor derecho
    [Tooltip("Cuantas veces mas pequeño sera el valor de vibracion respecto el normal")]
    public float vibrateReductionValue = 3;
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

    public bool GetChangeColorButtonInputDown()
    {
        return isControllerPlaying ? Input.GetButtonDown(xboxInputs.changeColorButton.ToString()) : Input.GetKeyDown(pcInputs.changeColorButton);
    }

    public bool GetIntensityButtonDown()
    {
        return isControllerPlaying ? Input.GetButtonDown(xboxInputs.chargeTrigger1.ToString()) || Input.GetButtonDown(xboxInputs.chargeTrigger2.ToString()) : Input.GetButtonDown(pcInputs.chargeTrigger.ToString());
    }

    public bool GetIntensityButtonUp()
    {
        return isControllerPlaying ? Input.GetButtonUp(xboxInputs.chargeTrigger1.ToString()) && Input.GetButtonUp(xboxInputs.chargeTrigger2.ToString()) : Input.GetButtonUp(pcInputs.chargeTrigger.ToString());
    }

    public bool GetStartButtonDown()
    {
        return isControllerPlaying ? Input.GetButtonDown(xboxInputs.pauseButton.ToString()) : Input.GetKeyDown(pcInputs.pauseButton);
    }

    public void ActiveVibration()
    {
        LeftTriggerPlayer1Value(1);
        RightTriggerPlayer1Value(1);
    }

    public void VibrationByValue(float value)
    {
        LeftTriggerPlayer1Value(value);
        RightTriggerPlayer1Value(value);
    }

    public void DeactiveVibration()
    {
        LeftTriggerPlayer1Value(0);
        RightTriggerPlayer1Value(0);
    }

    private void LeftTriggerPlayer1Value(float value)
    {
        leftPlayer1Vibrator = value;
    }

    private void RightTriggerPlayer1Value(float value)
    {
        rightPlayer1Vibrator = value / vibrateReductionValue;
    }
    #endregion

}
