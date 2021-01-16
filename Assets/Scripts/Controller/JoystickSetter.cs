using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoystickSetter : MonoBehaviour
{
    public VariableJoystick variableJoystick;

    public void ModeChanged(int index)
    {
        switch(index)
        {
            case 0:
                variableJoystick.SetMode(JoystickType.Fixed);
                break;
            case 1:
                variableJoystick.SetMode(JoystickType.Floating);
                break;
            case 2:
                variableJoystick.SetMode(JoystickType.Dynamic);
                break;
            default:
                index = 0;
                break;
        }
        PlayerPrefs.SetInt("ControllerMode", index);
        UIManager.GetInstance().ChangeControllerModeText(index);
    }
}