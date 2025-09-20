using System;
using UnityEngine;
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

        children[0].onClick.AddListener(() => playerController.OnButtonUp());
        children[1].onClick.AddListener(() => playerController.OnButtonDown());
        children[2].onClick.AddListener(() => playerController.OnButtonLeft());
        children[3].onClick.AddListener(() => playerController.OnButtonRight());
    }
}
