using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserHandler : MonoBehaviour {
    public Camera cameraRef;
    private HexGrid map;
    public MapUIHandler mapUI;
    public TurnManager turnManager;

    public LayerMask tileLayerMask;
    public LayerMask unitLayerMask;

    private Transform selectedUnitTransform;
    private Unit selectedUnit;
    private Dictionary<Unit, int> movementUsed = new Dictionary<Unit, int>();
    private List<Hex> selectedUnitsMovableTiles;
    private Dictionary<Hex, Hex> selectedUnitsTileReturnPath;

    private bool isUnitSelected {
        get {
            return !Object.ReferenceEquals(selectedUnit, null);
        }
    }

    void Start() {
        turnManager = FindObjectOfType<TurnManager>();
        turnManager.playerTurnEnded += EndTurn;
        cameraRef = Camera.main;

        GameObject mapGenObject = GameObject.FindGameObjectWithTag("MapGenerator");
        MapGenerator mapGen = mapGenObject.GetComponent<MapGenerator>();
        map = mapGen.map;

        Hex startingHex = map.GetRandomHex();
        while (startingHex.GetTileInfo().isBlocked) {
            startingHex = map.GetRandomHex();
        }
        gameObject.transform.position = map.HexToWorld(startingHex);

        DeselectUnit();
    }

    void Update() {
        HandleInput();
    }

    private void HandleInput() {
        HandleMouseInput();
    }

    private void HandleMouseInput() {
        RaycastHit hit;

        if (IsHoveringOverUnit(out hit)) {
            if (Input.GetMouseButtonDown(0)) {
                mapUI.UnhighlightTiles();
                mapUI.UnhighlightUnit();
                mapUI.ResetMovementArrows();
                if (isUnitSelected && selectedUnit.GetCurrentTile() == map.GetHexAt(hit.transform.position)) {
                    DeselectUnit();
                }
                else {
                    DeselectUnit();
                    SelectUnit(hit.transform.parent);
                    mapUI.HighlightUnit(hit.transform);
                    HexMovementInfo movementInfo = map.GetHexesInMovementRange(selectedUnit.GetCurrentTile(), selectedUnit.movementLimit - movementUsed[selectedUnit]);
                    selectedUnitsMovableTiles = movementInfo.traversableTiles;
                    selectedUnitsTileReturnPath = movementInfo.tileReturnPath;
                    mapUI.HighlightTiles(selectedUnitsMovableTiles);
                }
            }
        }
        else if (IsHoveringOverTile(out hit)) {
            mapUI.HoverHex(hit.transform.position);
            if (isUnitSelected && selectedUnitsMovableTiles.Contains(map.GetHexAt(hit.transform.position))) {
                // follow chain of selectedUnitsTileReturnPath until startingTile
                List<Hex> movementPath = new List<Hex>();
                int movementCost = 0;
                Hex returnTile = map.GetHexAt(hit.transform.position);
                if (returnTile != selectedUnit.GetCurrentTile()) {
                    movementPath.Add(returnTile);
                    movementCost += returnTile.GetTileInfo().movementCost;
                    while (selectedUnitsTileReturnPath[returnTile] != selectedUnit.GetCurrentTile()) {
                        returnTile = selectedUnitsTileReturnPath[returnTile];
                        movementPath.Add(returnTile);
                        movementCost += returnTile.GetTileInfo().movementCost;
                    }
                    movementPath.Add(selectedUnit.GetCurrentTile());
                    movementCost += returnTile.GetTileInfo().movementCost;
                }

                mapUI.DrawMovementArrows(movementPath);

                if (Input.GetMouseButtonDown(0)) {
                    selectedUnit.SetCurrentTile(map.GetHexAt(hit.transform.position));
                    selectedUnit.transform.position = hit.transform.position + hit.transform.GetComponent<TileInfo>().unitOffset;
                    movementUsed[selectedUnit] += movementCost;
                    DeselectUnit();
                    mapUI.UnhighlightUnit();
                    mapUI.UnhighlightTiles();
                    mapUI.ResetMovementArrows();
                }

            }
            if (Input.GetMouseButtonDown(0)) {
                mapUI.SelectHex(hit.transform.position);
                DeselectUnit();
            }
        }
        else {
            mapUI.RemoveHoverHex();
            if (Input.GetMouseButtonDown(0)) {
                mapUI.UnhighlightTiles();
                mapUI.UnhighlightUnit();
                mapUI.ResetMovementArrows();
                DeselectUnit();
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

    public void EndTurn() {
        List<Unit> keysBuffer = movementUsed.Keys.ToList();
        foreach (Unit unit in keysBuffer) {
            movementUsed[unit] = 0;
        }
    }

    private void SelectUnit(Transform newUnitTransform) {
        this.selectedUnitTransform = newUnitTransform;
        this.selectedUnit = newUnitTransform.GetComponent<Unit>();
        if(!movementUsed.ContainsKey(selectedUnit)) {
            movementUsed.Add(selectedUnit, 0);
        }
    }

    private void DeselectUnit() {
        this.selectedUnit = null;
        this.selectedUnitTransform = null;
    }
}