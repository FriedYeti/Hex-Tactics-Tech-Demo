using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UIControl : MonoBehaviour {

    public TurnManager turnManager;
    public Text turnCounterText;
    private int turnCounter = 0;

    public void RestartScene() {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void EndTurn() {
        turnManager.EndTurn();
        turnCounter = turnManager.turnCounter;
    }

    private void Update() {
        turnCounterText.text = "Turn: " + turnCounter;
    }
}
