using System.Runtime.InteropServices;
using UnityEngine;

/// <summary> 타일의 (x, y) 좌표 </summary>
[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 12)]
public struct TilePos
{
    [FieldOffset(0)] public int x;
    [FieldOffset(4)] public int y;

    public TilePos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public int Distance(in TilePos other)
    {
        int distX = other.x - x;
        int distY = other.y - y;
        return (distX * distX) + (distY * distY);
    }
    public float NDot(in TilePos A, in TilePos B)
    {
        Vector2 nA = new Vector2(A.x - x, A.y - y).normalized;
        Vector2 nB = new Vector2(B.x - x, B.y - y).normalized;
        return Vector2.Dot(nA, nB);
    }

    public bool Equals(in TilePos obj)
    {
        return x == obj.x && y == obj.y;
    }

    /// <summary> (x, y) 인덱스를 일차원배열의 인덱스로 변환 </summary>
    public int GetTileIndex(in int mapWidth)
    {
        return x + y * mapWidth;
    }
}