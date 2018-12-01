using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMovementInfo {
    public List<Hex> traversableTiles { get; private set; }
    public Dictionary<Hex, Hex> tileReturnPath { get; private set; }

    public HexMovementInfo(List<Hex> traversableTiles, Dictionary<Hex, Hex> tileReturnPath) {
        this.traversableTiles = traversableTiles;
        this.tileReturnPath = tileReturnPath;
    }
}

public class HexGrid {
    public Vector2Int gridSize;
    // TODO upgrade grid storage to Hash table
    private Hex[,] hexGrid;
    private Layout gridLayout;

    public HexGrid(Vector2Int gridSize, Layout gridLayout) {
        this.gridSize = gridSize;
        this.hexGrid = new Hex[gridSize.x, gridSize.y];
        this.gridLayout = gridLayout;
    }

    public List<Hex> GetHexesInRange(Hex startingHex, int range) {
        if (range < 0) throw new ArgumentException("range cannot be negative");

        List<Hex> results = new List<Hex>();
        for(int x = -range; x <= range; x++) {
            for(int y = Math.Max(-range, -x-range); y <= Math.Min(range, -x + range); y++) {
                int z = -x -y;
                Hex targetHex = GetHex(startingHex.coords + new Vector3Int(x, y, z));
                if(!System.Object.ReferenceEquals(targetHex, null) && !targetHex.GetTileInfo().isBlocked) {
                    results.Add(targetHex);
                }
            }
        }
        return results;
    }

    public List<Hex> GetHexesInRange(Vector2Int axialCoords, int range) {
	    return GetHexesInRange(GetHex(axialCoords), range);
    }

    public List<Hex> GetHexesInRange(Vector3 worldCoords, int range) {
	    return GetHexesInRange(GetHexAt(worldCoords), range);
    }

    public HexMovementInfo GetHexesInMovementRange(Hex startingHex, int movementRange) {
        // Modified Dijkstra's Algorithm AKA Uniform Cost Search
	    List<Hex> results = new List<Hex>();
	    results.Add(startingHex);

        BucketPriority<Hex> frontier = new BucketPriority<Hex>(128, true);
        frontier.Enqueue(0, startingHex);
        Dictionary<Hex, int> costSoFar = new Dictionary<Hex, int>();
        Dictionary<Hex, Hex> cameFrom = new Dictionary<Hex, Hex>();
        if (!UnityEngine.Object.ReferenceEquals(startingHex, null)) {
            costSoFar[startingHex] = 0;
        }
        while(!frontier.Empty()) {
            Hex currentHex = frontier.Dequeue();
            List<Hex> hexNeighbors = GetHexNeighbors(currentHex);
            if (hexNeighbors.Count > 0) {
                foreach (Hex nextHex in hexNeighbors) {
                    int newCost = costSoFar[currentHex] + nextHex.GetTileInfo().movementCost;
                    if ((!costSoFar.ContainsKey(nextHex) || newCost < costSoFar[nextHex]) && newCost < movementRange && !(nextHex.GetTileInfo().isBlocked || nextHex.GetTileInfo().isOccupied)) {
                        costSoFar[nextHex] = newCost;
                        int newPriority = newCost;
                        frontier.Enqueue(newPriority, nextHex);
                        cameFrom[nextHex] = currentHex;
                        results.Add(nextHex);
                    }
                }
            }
        }
        return new HexMovementInfo(results, cameFrom);
    }

    public Hex GetHex(Vector2Int axialCoords) {
        if(axialCoords.x < 0 || axialCoords.y < 0 || axialCoords.x > gridSize.x -1 || axialCoords.y > gridSize.y -1) {
            return null;
        }
        return hexGrid[axialCoords.x, axialCoords.y];
    }

    public Hex GetHex(Vector3Int coords) {
        if (coords.x < 0 || coords.z < 0 || coords.x > gridSize.x -1 || coords.z > gridSize.y -1) {
            return null;
        }
        return hexGrid[coords.x, coords.z];
    }

    public Hex GetHexAt(Vector2 worldCoords) {
        FractionalHex fractHex = gridLayout.WorldToHex(worldCoords);
        return GetHex(fractHex.HexRound().axialCoords);
    }

    public Hex GetHexAt(Vector3 worldCoords) {
        return GetHexAt(new Vector2(worldCoords.x, worldCoords.z));
    }

    public List<Hex> GetHexNeighbors(Vector2Int axialCoords) {
	    List<Hex> neighbors = new List<Hex>();
	    Hex currentHex = GetHex(axialCoords);
            for(int i = 0; i < 6; i++) {
	        Vector3Int neighborCoords = currentHex.GetNeighborCoords(i);
	        if(!object.ReferenceEquals(GetHex(neighborCoords),null)) {
	            neighbors.Add(GetHex(neighborCoords));
	        }
	    }
	    return neighbors;
    }

    public Hex GetRandomHex() {
        Vector2Int randomCoords = GetRandomHexCoords();
        Hex randomHex = GetHex(randomCoords);
        return randomHex;
    }

    public Vector2Int GetRandomHexCoords() {
        Vector2Int randomCoords = new Vector2Int();
        randomCoords.x = UnityEngine.Random.Range(0, this.gridSize.x - 1);
        randomCoords.y = UnityEngine.Random.Range(0, this.gridSize.y - 1);

        return randomCoords;
    }

    public List<Hex> GetHexNeighbors(Vector3Int coords) {
	    return GetHexNeighbors(new Vector2Int(coords.x, coords.z));
    }

    public List<Hex> GetHexNeighbors(Hex currentHex) {
	    return GetHexNeighbors(currentHex.axialCoords);
    }

    public void SetHex(Vector2Int axialCoords, Hex hex) {
        this.hexGrid[axialCoords.x, axialCoords.y] = hex;
    }

    public void SetHexAt(Vector2 worldCoords, Hex hex) {
        FractionalHex fractHex = gridLayout.WorldToHex(worldCoords);
        SetHex(fractHex.HexRound().axialCoords, hex);
    }

    public void SetHexAt(Vector3 worldCoords, Hex hex) {
        SetHexAt(new Vector2(worldCoords.x, worldCoords.z), hex);
    }

    public Vector2Int WorldToHex(Vector2 worldCoords) {
        return this.gridLayout.WorldToHex(worldCoords).HexRound().axialCoords;
    }

    public Vector2Int WorldToHex(Vector3 worldCoords) {
        return WorldToHex(new Vector2(worldCoords.x, worldCoords.z));
    }

    public Vector3 HexToWorld(Vector2Int hexCoords) {
        Vector2 temp = this.gridLayout.HexToWorld(GetHex(hexCoords));
        return new Vector3(temp.x, 0, temp.y);
    }

    public Vector3 HexToWorld(Hex hex) {
        Vector2 temp = this.gridLayout.HexToWorld(hex);
        return new Vector3(temp.x, 0, temp.y);
    }

    public Vector2 HexToWorld2D(Vector2Int hexCoords) {
        return this.gridLayout.HexToWorld(GetHex(hexCoords));
    }

    public Vector2 HexToWorld2D(Hex hex) {
        return this.gridLayout.HexToWorld(hex);
    }
}


public class Hex {
    // vectorized cube constructor
    public Hex(Vector3Int coords) {
        this.coords = coords;
        //this.hexTile = new GameObject("Blank Tile (" + coords.x + ", " + coords.z +")");
        if (coords.x + coords.y + coords.z != 0) throw new ArgumentException("x + y + z must be 0");
    }

    // vectorized axial constructor
    public Hex(Vector2Int axialCoords) : this(new Vector3Int(axialCoords.x, -axialCoords.x - axialCoords.y, axialCoords.y)) {
    }

    // cube constructor
    public Hex(int x, int y, int z) : this(new Vector3Int(x, y, z)) {
    }

    // axial constructor
    public Hex(int q, int r) : this(new Vector3Int(q, -q -r, r)) {
    }

    
    public readonly Vector3Int coords;
    public Vector2Int axialCoords {
        get {
            return new Vector2Int(this.coords.x, this.coords.z);
        }
    }
    private GameObject hexTile;
    private TileInfo tileInfo;

    public void SetHexTile(GameObject newTile) {
        UnityEngine.Object.Destroy(hexTile);
        this.hexTile = newTile;
	this.tileInfo = hexTile.GetComponent<TileInfo>();
    }

    public GameObject GetHexTile() {
        return this.hexTile;
    }

    public TileInfo GetTileInfo() {
        return this.tileInfo;
    }

    public override bool Equals(object obj) {
        if (!object.ReferenceEquals(obj,null)) {
            return this.Equals((Hex)obj);
        }
        else {
            return false;
        }
    }

    public bool Equals(Hex obj) {
        if (!object.ReferenceEquals(obj, null)) {
            return this.coords == obj.coords;
        }
        else {
            return false;
        }
    }

    public override int GetHashCode() {
        return (coords.x * 0x100000) + (coords.y * 0x1000) + coords.z;
    }

    public static bool operator ==(Hex h1, Hex h2) {
        return h1.Equals(h2);
    }

    public static bool operator !=(Hex h1, Hex h2) {
        return !h1.Equals(h2);
    }

    public Hex Add(Hex b) {
        return new Hex(coords + b.coords);
    }

    public Hex Subtract(Hex b) {
        return new Hex(coords - b.coords);
    }

    public Hex Scale(int k) {
        return new Hex(coords.x * k, coords.y * k, coords.z * k);
    }

    public int Length() {
        return (int)((Math.Abs(coords.x) + Math.Abs(coords.y) + Math.Abs(coords.z)) / 2);
    }

    public int Distance(Hex b) {
        return Subtract(b).Length();
    }

    static public List<Hex> directions = new List<Hex>{new Hex(1, 0, -1), new Hex(1, -1, 0), new Hex(0, -1, 1), new Hex(-1, 0, 1), new Hex(-1, 1, 0), new Hex(0, 1, -1)};

    static public Hex Direction(int direction) {
        return Hex.directions[direction];
    }


    public Hex Neighbor(int direction) {
        return Add(Hex.Direction(direction));
    }
    
    static public List<Vector3Int> directionOffsets = new List<Vector3Int> { new Vector3Int(1, 0, -1), new Vector3Int(1, -1, 0), new Vector3Int(0, -1, 1), new Vector3Int(-1, 0, 1), new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, -1) };

    static public Vector3 GetDirectionOffset(int direction) {
        return Hex.directionOffsets[direction];
    }

    public Vector3Int GetNeighborCoords(int direction) {
        return this.coords + Hex.directionOffsets[direction];
    }
}

public struct Orientation {
    public Orientation(double f0, double f1, double f2, double f3, double b0, double b1, double b2, double b3, double start_angle) {
        this.f0 = f0;
        this.f1 = f1;
        this.f2 = f2;
        this.f3 = f3;
        this.b0 = b0;
        this.b1 = b1;
        this.b2 = b2;
        this.b3 = b3;
        this.start_angle = start_angle;
    }
    public readonly double f0;
    public readonly double f1;
    public readonly double f2;
    public readonly double f3;
    public readonly double b0;
    public readonly double b1;
    public readonly double b2;
    public readonly double b3;
    public readonly double start_angle;

    static public readonly Orientation pointy = new Orientation(Math.Sqrt(3.0), Math.Sqrt(3.0) / 2.0, 0.0, 3.0 / 2.0, Math.Sqrt(3.0) / 3.0, -1.0 / 3.0, 0.0, 2.0 / 3.0, 0.5);
    static public readonly Orientation flat = new Orientation(3.0 / 2.0, 0.0, Math.Sqrt(3.0) / 2.0, Math.Sqrt(3.0), 2.0 / 3.0, 0.0, -1.0 / 3.0, Math.Sqrt(3.0) / 3.0, 0.0);
}

public struct Layout {
    public Layout(Orientation orientation, Vector2 size, Vector2 origin) {
        this.orientation = orientation;
        this.size = size;
        this.origin = origin;
    }
    public readonly Orientation orientation;
    public readonly Vector2 size;
    public readonly Vector2 origin;

    public Vector2 HexToWorld(Hex h) {
        Orientation M = orientation;
        float x = (float)(M.f0 * h.coords.x + M.f1 * h.coords.z) * size.x;
        float y = (float)(M.f2 * h.coords.x + M.f3 * h.coords.z) * size.y;
        return new Vector2(x + origin.x, y + origin.y);
    }


    public FractionalHex WorldToHex(Vector2 p) {
        Orientation M = orientation;
        Vector2 pt = new Vector2((p.x - origin.x) / size.x, (p.y - origin.y) / size.y);
        float q = (float)(M.b0 * pt.x + M.b1 * pt.y);
        float r = (float)(M.b2 * pt.x + M.b3 * pt.y);
        return new FractionalHex(q, -q - r, r);
    }


    public Vector2 HexCornerOffset(int corner) {
        Orientation M = orientation;
        float angle = (float)(2.0 * Math.PI * (M.start_angle - corner) / 6.0);
        return new Vector2((float)(size.x * Math.Cos(angle)), (float)(size.y * Math.Sin(angle)));
    }


    public List<Vector2> PolygonCorners(Hex h) {
        List<Vector2> corners = new List<Vector2> { };
        Vector2 center = HexToWorld(h);
        for (int i = 0; i < 6; i++) {
            Vector2 offset = HexCornerOffset(i);
            corners.Add(new Vector2(center.x + offset.x, center.y + offset.y));
        }
        return corners;
    }

}

public struct FractionalHex {
    public FractionalHex(double x, double y, double z) {
        this.x = x;
        this.y = y;
        this.z = z;
        if (Math.Round(x + y + z) != 0) throw new ArgumentException("x + y + z must be 0");
    }
    public readonly double x;
    public readonly double y;
    public readonly double z;

    public Hex HexRound() {
        int xi = (int)(Math.Round(x));
        int yi = (int)(Math.Round(y));
        int zi = (int)(Math.Round(z));
        double x_diff = Math.Abs(xi - x);
        double r_diff = Math.Abs(yi - y);
        double s_diff = Math.Abs(zi - z);
        if (x_diff > r_diff && x_diff > s_diff) {
            xi = -yi - zi;
        }
        else
            if (r_diff > s_diff) {
            yi = -xi - zi;
        }
        else {
            zi = -xi - yi;
        }
        return new Hex(xi, yi, zi);
    }


    public FractionalHex HexLerp(FractionalHex b, double t) {
        return new FractionalHex(x * (1.0 - t) + b.x * t, y * (1.0 - t) + b.y * t, z * (1.0 - t) + b.z * t);
    }


    static public List<Hex> HexLineDraw(Hex a, Hex b) {
        int N = a.Distance(b);
        FractionalHex a_nudge = new FractionalHex(a.coords.x + 0.000001, a.coords.y + 0.000001, a.coords.z - 0.000002);
        FractionalHex b_nudge = new FractionalHex(b.coords.x + 0.000001, b.coords.y + 0.000001, b.coords.z - 0.000002);
        List<Hex> results = new List<Hex> { };
        double step = 1.0 / Math.Max(N, 1);
        for (int i = 0; i <= N; i++) {
            results.Add(a_nudge.HexLerp(b_nudge, step * i).HexRound());
        }
        return results;
    }

}

public static class HexUtil {
    public static Vector3 CubeLerp(Vector3 a, Vector3 b, float t) {
        return new Vector3(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t), Mathf.Lerp(a.z, b.z, t));
    }
}