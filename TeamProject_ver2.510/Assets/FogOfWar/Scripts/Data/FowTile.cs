public class FowTile
{
    /// <summary> 타일 (x,y) 좌표 </summary>
    public TilePos pos;

    /// <summary> (x,y) 좌표, width를 이용해 계산한 일차원 배열 내 인덱스 </summary>
    public int index;

    public int X => pos.x;
    public int Y => pos.y;

    public FowTile(int x, int y, int width)
    {
        pos.x = x;
        pos.y = y;

        index = x + y * width;
    }

    public int Distance(FowTile other)
    {
        int distX = other.X - X;
        int distY = other.Y - Y;
        return (distX * distX) + (distY * distY);
    }
}