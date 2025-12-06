using System.Collections.Generic;
using UnityEngine;

public class CommandToAction : MonoBehaviour
{
    delegate void RobotAction();

    Dictionary<string, RobotAction> robotActions = new Dictionary<string, RobotAction>();

    void Awake()
    {

    }
}
