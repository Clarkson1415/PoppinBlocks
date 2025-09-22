using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HookUpButtonMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var playerController = ControlPlayers.Instance();

        if (playerController == null)
        {
            Debug.LogError("No player controller found.");
            throw new ArgumentOutOfRangeException("Control players component not found.");
        }

        var children = this.GetComponentsInChildren<Button>();
        up = children[0];
        down = children[1];
        left = children[2];
        right = children[3];

        //children[0].onClick.AddListener(() => playerController.OnButtonUp());
        //children[1].onClick.AddListener(() => playerController.OnButtonDown());
        //children[2].onClick.AddListener(() => playerController.OnButtonLeft());
        //children[3].onClick.AddListener(() => playerController.OnButtonRight());

        up.AddComponent<ButtonPressHandler>();
        var customU = up.GetComponent<ButtonPressHandler>();
        customU.AssignOnPressedAction(() => playerController.OnButtonUp());

        down.AddComponent<ButtonPressHandler>();
        var customD = down.GetComponent<ButtonPressHandler>();
        customD.AssignOnPressedAction(() => playerController.OnButtonDown());

        left.AddComponent<ButtonPressHandler>();
        var customL = left.GetComponent<ButtonPressHandler>();
        customL.AssignOnPressedAction(() => playerController.OnButtonLeft());

        right.AddComponent<ButtonPressHandler>();
        var customR = right.GetComponent<ButtonPressHandler>();
        customR.AssignOnPressedAction(() => playerController.OnButtonRight());
    }

    private Button up;
    private Button down;
    private Button left;
    private Button right;

    public class ButtonPressHandler : MonoBehaviour, IPointerDownHandler
    {
        public Action onPressedAction;

        public void AssignOnPressedAction(Action onPressed)
        {
            onPressedAction = onPressed;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onPressedAction?.Invoke();
        }
    }
}
