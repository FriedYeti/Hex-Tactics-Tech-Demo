using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public GameObject cameraController;
    public Camera cameraRef;
    private HexGrid map;

    public LayerMask tileLayerMask;
    public LayerMask unitLayerMask;

    private Hex currentTile;

    public Transform selectedUnit;

    public GameObject hexTargetRing;
    public GameObject hexSelectRing;

    public GameObject hexHighlights;
    private HexHighlights hexHighlighter;

    private List<Hex> movableTiles;
    private Dictionary<Hex, Hex> tileReturnPath;

    private bool isUnitSelected {
        get {
            return !Object.ReferenceEquals(selectedUnit, null);
        }
    }

    public int characterMovementLimit = 10;
    private int movementUsed = 0;

    public GameObject arrowPrefab;
    List<GameObject> arrows = new List<GameObject>();
    public GameObject arrowObjects;

    void Start () {
        cameraRef = Camera.main;

        GameObject mapGenObject = GameObject.FindGameObjectWithTag("MapGenerator");
        MapGenerator mapGen = mapGenObject.GetComponent<MapGenerator>();
        map = mapGen.map;

        hexHighlighter = hexHighlights.GetComponent<HexHighlights>();

        Hex startingHex = map.GetRandomHex();
        while(startingHex.GetTileInfo().isBlocked) {
            startingHex = map.GetRandomHex();
        }
        gameObject.transform.position = map.HexToWorld(startingHex);
        cameraController.transform.position = map.HexToWorld(startingHex);

        selectedUnit = null;

        for(int i = 0; i < characterMovementLimit; i++) {
            arrows.Add(Object.Instantiate(arrowPrefab, new Vector3(0, 1000, 0), Quaternion.identity, arrowObjects.transform));
        }
	}
	
	void Update () {
        SetCurrentTile();
        HandleInput();
	}

    private void HandleInput() {
        HandleMouseInput();
    }

    private void HandleMouseInput() {
        RaycastHit hit;

        if (IsHoveringOverUnit(out hit)) {
            if (Input.GetMouseButtonDown(0)) {
                HexMovementInfo movementInfo = map.GetHexesInMovementRange(currentTile, characterMovementLimit - movementUsed);
                movableTiles = movementInfo.traversableTiles;
                tileReturnPath = movementInfo.tileReturnPath;
                hexHighlighter.HighlightTiles(movableTiles);

                selectedUnit = hit.transform.parent;
            }
        }
        else if (IsHoveringOverTile(out hit)) {
            hexTargetRing.transform.position = hit.transform.position;
            //Transform objectHit = hit.transform;
            //Debug.DrawLine(new Vector3(9, 10, 5), hit.transform.position + hit.transform.GetComponent<TileInfo>().unitOffset, Color.red);
            if(isUnitSelected && movableTiles.Contains(map.GetHexAt(hit.transform.position))) {

                // follow chain of tileReturnPath until startingTile
                List<Hex> movementPath = new List<Hex>();
                int movementCost = 0;
                Hex returnTile = map.GetHexAt(hit.transform.position);
                if (returnTile != currentTile) {
                    movementPath.Add(returnTile);
                    movementCost += returnTile.GetTileInfo().movementCost;
                    while (tileReturnPath[returnTile] != currentTile) {
                        returnTile = tileReturnPath[returnTile];
                        movementPath.Add(returnTile);
                        movementCost += returnTile.GetTileInfo().movementCost;
                    }
                    movementPath.Add(currentTile);
                    movementCost += returnTile.GetTileInfo().movementCost;
                }

                ResetMovementArrows();

                for (int i = movementPath.Count - 1; i > 0; i--) {
                    Vector3 pos = map.HexToWorld(movementPath[i]);
                    arrows[i] = Object.Instantiate(arrowPrefab, pos, Quaternion.identity);
                    arrows[i].transform.LookAt(map.HexToWorld(movementPath[i - 1]));
                    arrows[i].transform.parent = arrowObjects.transform;
                }

                //Debug.DrawRay(hit.transform.position, Vector3.up * 2, Color.yellow);
                if (Input.GetMouseButtonDown(0)) {
                    selectedUnit.transform.position = hit.transform.position + hit.transform.GetComponent<TileInfo>().unitOffset;
                    movementUsed += movementCost;
                    selectedUnit = null;
                    hexHighlighter.UnhighlightAllTiles();
                    foreach (GameObject arrow in arrows) {
                        arrow.transform.position = Vector3.up * 1000;
                    }
                }

            }
            if (Input.GetMouseButtonDown(0)) {
                //DrawLineBetweenHexes(currentTile, map.GetHexAt(hit.transform.position));
                hexSelectRing.transform.position = hit.transform.position;
            }
        }
        else {
            // hide target ring somewhere off camera
            hexTargetRing.transform.position = new Vector3(0, 1000, 0);
            if (Input.GetMouseButtonDown(0)) {
                hexHighlighter.UnhighlightAllTiles();
                selectedUnit = null;
            }
        }
    }

    private bool IsHoveringOverUnit(out RaycastHit hitReturn) {
        Ray ray = cameraRef.ScreenPointToRay(Input.mousePosition);
        return (Physics.Raycast(ray, out hitReturn, cameraRef.farClipPlane, unitLayerMask));
    }

    private bool IsHoveringOverTile(out RaycastHit hitReturn) {
        Ray ray = cameraRef.ScreenPointToRay(Input.mousePosition);
        return (Physics.Raycast(ray, out hitReturn, cameraRef.farClipPlane, tileLayerMask));
    }

    private void SetCurrentTile() {
        RaycastHit hit;
        Ray ray = new Ray(gameObject.transform.position + Vector3.up * 2, Vector3.down);
        
        if (Physics.Raycast(ray, out hit, cameraRef.farClipPlane, tileLayerMask)) {
            currentTile = map.GetHexAt(hit.transform.position);
            gameObject.transform.position = hit.transform.position + hit.transform.GetComponent<TileInfo>().unitOffset;
        }
    }

    private void ResetMovementArrows() {
        foreach (GameObject arrow in arrows) {
            arrow.transform.position = Vector3.up * 1000;
        }
    }

    public void EndTurn() {
        movementUsed = 0;
    }

    private void SelectUnit(GameObject newUnit) {
        //this.selectedUnit.unitMesh = newUnit;
    }

    public Vector3 CubeLerp(Vector3 a, Vector3 b, float t) {
        return new Vector3(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t), Mathf.Lerp(a.z, b.z, t));
    }

    public void DrawLineBetweenHexes(Hex a, Hex b) {
        Debug.DrawLine(map.HexToWorld(a) + new Vector3(0, 0.5f, 0), map.HexToWorld(b) + new Vector3(0, 0.5f, 0), Color.cyan, 5f);

        int distance = a.Distance(b);
        List<Vector3> lerpPoints = new List<Vector3>();

        for (int i = 0; i <= distance; i++) {
            Vector3 posA = map.HexToWorld(a);
            Vector3 posB = map.HexToWorld(b);
            lerpPoints.Add(CubeLerp(posA, posB, (float)i / distance));
        }

        int rayLength = 2;

        for (int i = 0; i < (lerpPoints.Count - 1); i++) {
            Hex first = map.GetHexAt(lerpPoints[i]);
            Vector3 firstPos = map.HexToWorld(first) + new Vector3(0, 0.5f, 0);
            Hex second = map.GetHexAt(lerpPoints[i + 1]);
            Vector3 secondPos = map.HexToWorld(second) + new Vector3(0, 0.5f, 0);
            //Debug.DrawRay(new Vector3(firstPos.x, 0, firstPos.y), Vector3.up * rayLength, Color.red, 5f);
            Debug.DrawLine(firstPos, secondPos, Color.green, 5f);
            rayLength++;
        }
    }

}


public class Unit {
    public GameObject unitMesh;
    public int movementLimit;
}
