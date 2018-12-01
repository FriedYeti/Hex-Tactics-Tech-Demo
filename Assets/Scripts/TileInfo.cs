using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour {

    public string tileName;
    public bool isBlocked = false;
    public bool isOccupied = false;
    public int movementCost = 1;
    public Vector3 unitOffset = new Vector3(0, 0.5f, 0);

}
