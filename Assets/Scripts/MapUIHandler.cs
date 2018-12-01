using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUIHandler : MonoBehaviour {
    public Camera cameraRef;
    private HexGrid map;

    public GameObject unitHighlight;

    public GameObject hexHoverRing;
    public GameObject hexSelectRing;

    public GameObject hexHighlights;
    private HexHighlights hexHighlighter;

    public int arrowPoolSize = 32;

    public GameObject arrowPrefab;
    List<GameObject> arrows = new List<GameObject>();
    public GameObject arrowObjects;

    void Start() {
        GameObject mapGenObject = GameObject.FindGameObjectWithTag("MapGenerator");
        MapGenerator mapGen = mapGenObject.GetComponent<MapGenerator>();
        map = mapGen.map;

        hexHighlighter = hexHighlights.GetComponent<HexHighlights>();

        for (int i = 0; i < arrowPoolSize; i++) {
            arrows.Add(Object.Instantiate(arrowPrefab, new Vector3(0, 1000, 0), Quaternion.identity, arrowObjects.transform));
        }
    }

    public void SelectHex(Hex selectedHex) {
        hexSelectRing.transform.position = map.HexToWorld(selectedHex);
    }

    public void SelectHex(Vector3 pos) {
        hexSelectRing.transform.position = pos;
    }

    public void DeselectHex() {
        hexSelectRing.transform.position = new Vector3(0, 1000, 0);
    }

    public void HoverHex(Hex hoveredHex) {
        hexHoverRing.transform.position = map.HexToWorld(hoveredHex);
    }

    public void HoverHex(Vector3 pos) {
        hexHoverRing.transform.position = pos;
    }

    public void RemoveHoverHex() {
        hexHoverRing.transform.position = new Vector3(0, 1000, 0);
    }

    public void HighlightTiles(List<Hex> tiles) {
        hexHighlighter.HighlightTiles(tiles);
    }

    public void UnhighlightTiles() {
        hexHighlighter.UnhighlightAllTiles();
    }

    public void DrawMovementArrows(List<Hex> movementPath) {
        ResetMovementArrows();

        for (int i = movementPath.Count - 1; i > 0; i--) {
            Vector3 pos = map.HexToWorld(movementPath[i]);
            arrows[i] = Object.Instantiate(arrowPrefab, pos, Quaternion.identity);
            arrows[i].transform.LookAt(map.HexToWorld(movementPath[i - 1]));
            arrows[i].transform.parent = arrowObjects.transform;
        }
    }

    public void ResetMovementArrows() {
        foreach (GameObject arrow in arrows) {
            arrow.transform.position = Vector3.up * 1000;
        }
    }

    public void HighlightUnit(Transform unit) {
        unitHighlight.transform.position = unit.position;
    }

    public void UnhighlightUnit() {
        unitHighlight.transform.position = Vector3.up * 1000;
    }

    public void DebugDrawLineBetweenHexes(Hex a, Hex b) {
        Debug.DrawLine(map.HexToWorld(a) + new Vector3(0, 0.5f, 0), map.HexToWorld(b) + new Vector3(0, 0.5f, 0), Color.cyan, 5f);

        int distance = a.Distance(b);
        List<Vector3> lerpPoints = new List<Vector3>();

        for (int i = 0; i <= distance; i++) {
            Vector3 posA = map.HexToWorld(a);
            Vector3 posB = map.HexToWorld(b);
            lerpPoints.Add(HexUtil.CubeLerp(posA, posB, (float)i / distance));
        }

        int rayLength = 2;

        for (int i = 0; i < (lerpPoints.Count - 1); i++) {
            Hex first = map.GetHexAt(lerpPoints[i]);
            Vector3 firstPos = map.HexToWorld(first) + new Vector3(0, 0.5f, 0);
            Hex second = map.GetHexAt(lerpPoints[i + 1]);
            Vector3 secondPos = map.HexToWorld(second) + new Vector3(0, 0.5f, 0);
            Debug.DrawLine(firstPos, secondPos, Color.green, 5f);
            rayLength++;
        }
    }
}
