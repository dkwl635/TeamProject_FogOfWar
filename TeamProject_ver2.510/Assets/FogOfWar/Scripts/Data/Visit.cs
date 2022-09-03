using System.Runtime.InteropServices;

/// <summary> 현재 존재, 과거 방문 여부 </summary>
[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct Visit
{
    /// <summary> 현재 위치함 </summary>
    [FieldOffset(0)]
    public bool current;

    /// <summary> 과거에 방문한 적 있음 </summary>
    [FieldOffset(4)]
    public bool ever;
}