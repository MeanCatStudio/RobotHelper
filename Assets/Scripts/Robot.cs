using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
    [SerializeField] string[] objectKeywords;
    [SerializeField] Transform[] objectTranforms;
    [SerializeField] Vector3 holdPoint;
    [SerializeField] string[] actions;
    [SerializeField] Vector3 GivePoint;

    delegate void Action(Transform thing);

    Dictionary<string, Transform> objectsInHouse;

    NavMeshAgent agent;
    bool moving { get => targetThing != null && agent.remainingDistance <= 0.1f; }
    string currentAction;
    Transform targetThing;
    Transform thingInHand;

    void Awake()
    {
        objectsInHouse = new Dictionary<string, Transform>();
        //actions = new Dictionary<string, Action[]>();
        for (int i = 0; i < objectKeywords.Length; i++)
        {
            objectsInHouse.Add(objectKeywords[i], objectTranforms[i]);
        }
        agent = GetComponent<NavMeshAgent>();
        //agent.SetDestination(new Vector3(0, 0, 4));
    }

    void Update()
    {
        if (!moving)
        {
            Debug.Log(targetThing);
            switch (currentAction)
            {
                case "go":
                    currentAction = null;
                    targetThing = null;
                    break;
                case "take":
                    currentAction = null;
                    targetThing = null;
                    Take(targetThing); break;
                case "bring":
                    if (thingInHand != null)
                    {
                        Give();
                        currentAction = null;
                        targetThing = null;
                    }
                    else
                    {
                        Take(targetThing);
                        targetThing = objectsInHouse["me"];
                        agent.SetDestination(targetThing.position);
                    }
                    break;
            }
        }

    }

    void Take(Transform thing)
    {
        thing.SetParent(transform, false);
        thing.localPosition = holdPoint;
        thingInHand = thing;
    }

    void Drop()
    {
        if (thingInHand != null)
        {
            thingInHand.SetParent(null, true);
            thingInHand.position = thingInHand.position - Vector3.up * thingInHand.position.y;
            thingInHand = null;
        }
    }

    void Give()
    {
        if (thingInHand != null)
        {
            thingInHand.SetParent(null, true);
            thingInHand.position = GivePoint;
            thingInHand = null;
        }
    }

    public void TakeCommand(string command)
    {
        if (moving) { return; }
        targetThing = null;
        currentAction = null;
        command = command.ToLower();
        foreach (string key in objectsInHouse.Keys)
        {
            if (command.Contains(key))
            {
                targetThing = objectsInHouse[key];
                break;
            }
        }
        foreach (string key in actions)
        {
            if (command.Contains(key))
            {
                currentAction = key;
                break;
            }
        }
        if (targetThing != null && currentAction != null)
        {
            switch (currentAction)
            {
                case "go":
                    agent.SetDestination(targetThing.position); break;
                case "take":
                    agent.SetDestination(targetThing.position); Drop(); break;
                case "bring":
                    agent.SetDestination(targetThing.position); Drop(); break;
            }
        }
        else { Debug.Log("I dont get that"); targetThing = null; currentAction = null; }
    }
}
