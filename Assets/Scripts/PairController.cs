using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PairController : MonoBehaviour
{
    private GameObject leftPlayer;
    private GameObject rightPlayer;
    private PlayerController leftPlayerController;
    private PlayerController rightPlayerController;
    private float onControll = -1;

    void Start()
    {
        leftPlayer = this.gameObject.transform.GetChild(0).gameObject;
        rightPlayer = this.gameObject.transform.GetChild(1).gameObject;

        leftPlayerController = leftPlayer.GetComponent<PlayerController>();
        rightPlayerController = rightPlayer.GetComponent<PlayerController>();

        leftPlayerController.isActive = true;
        rightPlayerController.isActive = false;
    }

    public void OnChangeGame(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() != 0 && context.ReadValue<float>() != onControll)
        {
            leftPlayerController.isActive = !leftPlayerController.isActive;
            rightPlayerController.isActive = !rightPlayerController.isActive;
            onControll = context.ReadValue<float>();
        }
    }
}
