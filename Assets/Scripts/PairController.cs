using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PairController : MonoBehaviour
{
    public float delayChange;
    private bool onChange;
    private GameObject leftPlayer;
    private GameObject rightPlayer;
    private PlayerController leftPlayerController;
    private PlayerController rightPlayerController;
    private float onControll = 1;

    void Start()
    {
        leftPlayer = this.gameObject.transform.GetChild(0).gameObject;
        rightPlayer = this.gameObject.transform.GetChild(1).gameObject;

        leftPlayerController = leftPlayer.GetComponent<PlayerController>();
        rightPlayerController = rightPlayer.GetComponent<PlayerController>();

        leftPlayerController.isActive = false;
        rightPlayerController.isActive = true;
    }

    public void OnChangeGame(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() != 0 && context.ReadValue<float>() != onControll && !onChange)
        {
            leftPlayerController.isActive = !leftPlayerController.isActive;
            rightPlayerController.isActive = !rightPlayerController.isActive;
            onControll = context.ReadValue<float>();
            StartCoroutine(DelayChange());
        }
    }

    public void CopySpriteColor()
    {
        SpriteRenderer leftSprite = leftPlayer.GetComponent<SpriteRenderer>();
        SpriteRenderer rightSprite = rightPlayer.GetComponent<SpriteRenderer>();

        rightSprite.color = leftSprite.color;
    }

    public void ChangeGame() {
        leftPlayerController.isActive = !leftPlayerController.isActive;
        rightPlayerController.isActive = !rightPlayerController.isActive;
        onControll = onControll * -1;
        StartCoroutine(DelayChange());
    }

    IEnumerator DelayChange()
    {
        onChange = true;
        yield return new WaitForSeconds(delayChange);
        onChange = false;
    }
}
