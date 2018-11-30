using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public Vector2Int mapSize = new Vector2Int(8, 8);
    public Vector2 unitSize = new Vector2(1, 1);
    private Vector2 mapOrigin;

    public HexGrid map;

    public GameObject[] hexPrefabs;
    public int prefabSelect = 0;

    public Vector2 noiseOrigin;

    private void Awake() {
        this.mapOrigin = gameObject.transform.position;
        map = new HexGrid(mapSize, new Layout(Orientation.pointy, unitSize, mapOrigin));

        noiseOrigin = new Vector2(Random.Range(0f, 10f),Random.Range(0f, 10f));
        for (int i = 0; i < mapSize.x; i++) {
            for (int j = 0; j < mapSize.y; j++) {
                map.SetHex(new Vector2Int(i, j), new Hex(new Vector2Int(i, j)));
            }
        }

        for (int i = 0; i < mapSize.x; i++) {
            for (int j = 0; j < mapSize.y; j++) {
                float perlinNoise = Mathf.Clamp(Mathf.PerlinNoise((float)i / hexPrefabs.Length + noiseOrigin.x, (float)j / hexPrefabs.Length + noiseOrigin.y), 0, 0.999f);
                prefabSelect = Mathf.FloorToInt(perlinNoise * hexPrefabs.Length);
                Hex tile = map.GetHex(new Vector2Int(i, j));
                Vector3 pos = map.HexToWorld(tile);
                GameObject newHex = Instantiate(hexPrefabs[prefabSelect], pos, Quaternion.identity);

                newHex.transform.parent = gameObject.transform;
                newHex.gameObject.name = ("Tile (" + i + ", " + j + ")");

                tile.SetHexTile(newHex);
            }
        }
    }
}
