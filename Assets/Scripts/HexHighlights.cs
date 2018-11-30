using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexHighlights : MonoBehaviour {
    public HexGrid map;

    public GameObject hexHighlight;

    public int highlightPoolSize = 64;
    private GameObject[] hexHighlightPool;

    private int highlightIndex = 0;


    private void Awake() {
        hexHighlightPool = new GameObject[highlightPoolSize];
    }

    void Start () {
        for (int i = 0; i < hexHighlightPool.Length; i++) {
            hexHighlightPool[i] = GameObject.Instantiate(hexHighlight, new Vector3(0, 1000, 0), Quaternion.identity, gameObject.transform);
        }

        GameObject mapGenObject = GameObject.FindGameObjectWithTag("MapGenerator");
        MapGenerator mapGen = mapGenObject.GetComponent<MapGenerator>();
        map = mapGen.map;
    }

    public void HighlightTiles(List<Hex> tilesToHighlight) {
        foreach (Hex tile in tilesToHighlight) {
            hexHighlightPool[highlightIndex++ % highlightPoolSize].transform.position = map.HexToWorld(tile);
        }
    }

    public void UnhighlightAllTiles() {
        highlightIndex = 0;
        foreach(GameObject highlight in hexHighlightPool) {
            highlight.transform.position = new Vector3(0, 1000, 0);
        }
    }
}
