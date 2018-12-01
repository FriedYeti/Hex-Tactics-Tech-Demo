using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {
    // handles turn order, turn count, and users ending their turns

    public event Action playerTurnEnded;
    public int turnCounter = 0;

    // TODO update to priority queue
    private BucketPriority<Transform> units;

    private void Start() {
        units = new BucketPriority<Transform>(10);
    }

    public void EndTurn() {
        turnCounter++;
        if (playerTurnEnded != null) {
            playerTurnEnded();
        }
    }

    public void AddUnitToQueue(Transform newUnit) {
        units.Enqueue(10 - newUnit.GetComponent<Unit>().speed, newUnit);
    }

    public Transform GetNextUnit() {
        return units.Dequeue();
    }

    public Transform PeakNextUnit() {
        return units.Peak();
    }
}
