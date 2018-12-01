using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    public int movementLimit;
    public int speed;

    private Hex currentTile;

    private bool isUnitSelected = false;

    private void Update() {

    }

    public void SetCurrentTile(Hex newTile) {
        // tell the old tile it's open
        if(!Object.ReferenceEquals(currentTile, null)) {
            currentTile.GetTileInfo().isOccupied = false;
        }
        // tell the new tile it's full
        if (!Object.ReferenceEquals(newTile, null)) {
            currentTile = newTile;
            currentTile.GetTileInfo().isOccupied = true;
        }
    }

    public Hex GetCurrentTile() {
        return this.currentTile;
    }

    public void SelectUnit() {
        this.isUnitSelected = true;
    }

    public void DeselectUnit() {
        this.isUnitSelected = false;
    }
}
