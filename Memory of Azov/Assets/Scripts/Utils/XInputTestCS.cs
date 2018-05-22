using UnityEngine;
using XInputDotNetPure; // Required in C#

public class XInputTestCS : MonoSingleton<XInputTestCS>
{
    bool playerIndexSet = false;
    PlayerIndex playerIndex1;
    public GamePadState statePlayer1;
    GamePadState prevState1;

    private float timer;
    private float pack;

    // Use this for initialization
    void Start()
    {
        // No need to initialize anything for the plugin
    }

    void FixedUpdate()
    {
        // SetVibration should be sent in a slower rate.
        // Set vibration according to triggers
        GamePad.SetVibration(playerIndex1, InputsManager.Instance.leftPlayer1Vibrator, InputsManager.Instance.rightPlayer1Vibrator);
    }

    // Update is called once per frame
    void Update()
    {

        // Find a PlayerIndex, for a single player game
        // Will find the first controller that is connected ans use it
        if (!playerIndexSet || !prevState1.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    //Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    playerIndex1 = testPlayerIndex;

                    playerIndexSet = true;
                    break;
                }
            }

        }

        prevState1 = statePlayer1;
        statePlayer1 = GamePad.GetState(playerIndex1);

        //if (timer >= 1)
        //{
        //    if (pack == statePlayer2.PacketNumber)
        //    {
        //        Debug.Log("No detected joystick");
        //    }
        //    else
        //    {
        //        pack = statePlayer2.PacketNumber;
        //    }
        //    timer = 0;
        //}

        //timer += Time.deltaTime;

        // Detect if a button was pressed this frame
        //if (prevState1.Buttons.A == ButtonState.Released && statePlayer1.Buttons.A == ButtonState.Pressed)
        //{
        //    GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value, 1.0f);
        //}
        //// Detect if a button was released this frame
        //if (prevState1.Buttons.A == ButtonState.Pressed && statePlayer1.Buttons.A == ButtonState.Released)
        //{
        //    GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        //}

        //// Make the current object turn
        //transform.localRotation *= Quaternion.Euler(0.0f, statePlayer2.ThumbSticks.Left.X * 25.0f * Time.deltaTime, 0.0f);
    }

    //void OnGUI()
    //{
    //    string text = "Use left stick to turn the cube, hold A to change color\n";
    //    text += string.Format("IsConnected {0} Packet #{1}\n", statePlayer1.IsConnected, statePlayer1.PacketNumber);
    //    text += string.Format("\tTriggers {0} {1}\n", statePlayer1.Triggers.Left, statePlayer1.Triggers.Right);
    //    text += string.Format("\tD-Pad {0} {1} {2} {3}\n", statePlayer1.DPad.Up, statePlayer1.DPad.Right, statePlayer1.DPad.Down, statePlayer1.DPad.Left);
    //    text += string.Format("\tButtons Start {0} Back {1} Guide {2}\n", statePlayer1.Buttons.Start, statePlayer1.Buttons.Back, statePlayer1.Buttons.Guide);
    //    text += string.Format("\tButtons LeftStick {0} RightStick {1} LeftShoulder {2} RightShoulder {3}\n", statePlayer1.Buttons.LeftStick, statePlayer1.Buttons.RightStick, statePlayer1.Buttons.LeftShoulder, statePlayer1.Buttons.RightShoulder);
    //    text += string.Format("\tButtons A {0} B {1} X {2} Y {3}\n", statePlayer1.Buttons.A, statePlayer1.Buttons.B, statePlayer1.Buttons.X, statePlayer1.Buttons.Y);
    //    text += string.Format("\tSticks Left {0} {1} Right {2} {3}\n", statePlayer1.ThumbSticks.Left.X, statePlayer1.ThumbSticks.Left.Y, statePlayer1.ThumbSticks.Right.X, statePlayer1.ThumbSticks.Right.Y);
    //    GUI.Label(new Rect(0, Screen.height/2, Screen.width, Screen.height), text);
    //    text = "Use left stick to turn the cube, hold A to change color\n";
    //    text += string.Format("IsConnected {0} Packet #{1}\n", statePlayer2.IsConnected, statePlayer2.PacketNumber);
    //    text += string.Format("\tTriggers {0} {1}\n", statePlayer2.Triggers.Left, statePlayer2.Triggers.Right);
    //    text += string.Format("\tD-Pad {0} {1} {2} {3}\n", statePlayer2.DPad.Up, statePlayer2.DPad.Right, statePlayer2.DPad.Down, statePlayer2.DPad.Left);
    //    text += string.Format("\tButtons Start {0} Back {1} Guide {2}\n", statePlayer2.Buttons.Start, statePlayer2.Buttons.Back, statePlayer2.Buttons.Guide);
    //    text += string.Format("\tButtons LeftStick {0} RightStick {1} LeftShoulder {2} RightShoulder {3}\n", statePlayer2.Buttons.LeftStick, statePlayer2.Buttons.RightStick, statePlayer2.Buttons.LeftShoulder, statePlayer2.Buttons.RightShoulder);
    //    text += string.Format("\tButtons A {0} B {1} X {2} Y {3}\n", statePlayer2.Buttons.A, statePlayer2.Buttons.B, statePlayer2.Buttons.X, statePlayer2.Buttons.Y);
    //    text += string.Format("\tSticks Left {0} {1} Right {2} {3}\n", statePlayer2.ThumbSticks.Left.X, statePlayer2.ThumbSticks.Left.Y, statePlayer2.ThumbSticks.Right.X, statePlayer2.ThumbSticks.Right.Y);
    //    GUI.Label(new Rect(0, 0, Screen.width, Screen.height), text);
    //}
}
