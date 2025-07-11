using Assets.Scripts;
using UnityEngine;
using EasyTransition;
using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
#nullable enable

public class Restart : MonoBehaviour
{
    public void RestartLevel()
    {
        GameLevels.Reload();
    }
}
