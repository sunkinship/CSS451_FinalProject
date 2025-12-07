using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputHandler : MonoBehaviour
{
    private UIManager mamager => UIManager.Instance;

    private void Update()
    {
        ReadKeyInput();
    }

    private void ReadKeyInput()
    {
        //move claw
        if (Input.GetKeyDown(KeyCode.W))
            mamager.OnClawUpPressed();
        else if (Input.GetKeyUp(KeyCode.W))
            mamager.OnClawUpReleased();

        if (Input.GetKeyDown(KeyCode.A))
            mamager.OnClawLeftPressed();
        else if (Input.GetKeyUp(KeyCode.A))
            mamager.OnClawLeftReleased();

        if (Input.GetKeyDown(KeyCode.S))
            mamager.OnClawDownPressed();
        else if (Input.GetKeyUp(KeyCode.S))
            mamager.OnClawDownReleased();

        if (Input.GetKeyDown(KeyCode.D))
            mamager.OnClawRightPressed();
        else if (Input.GetKeyUp(KeyCode.D))
            mamager.OnClawRightReleased();

        //rotate claw
        if (Input.GetKeyDown(KeyCode.Q))
            mamager.OnClawRotateLeftPressed();
        else if (Input.GetKeyUp(KeyCode.Q))
            mamager.OnClawRotateLeftReleased();

        if (Input.GetKeyDown(KeyCode.E))
            mamager.OnClawRotateRightPressed();
        else if (Input.GetKeyUp(KeyCode.E))
            mamager.OnClawRotateRightReleased();

        //drop
        if (Input.GetKeyDown(KeyCode.Space))
            mamager.OnClawDropPressed();

        //pause
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            UIManager.Instance.OnPauseToggle();
    }
}
