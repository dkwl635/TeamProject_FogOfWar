using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FowMap
{
    List<FowTile> map = new List<FowTile>();
    int mapWidth;
    int mapHeight;

    int mapLength;

    // 배열들은 타일 개수만큼 크기 생성
    float[] visit;

    Color[] colorBuffer;
    Material blurMat;

    Texture2D texBuffer;
    RenderTexture blurBuffer; // 렌더텍스쳐 여러장 쓰는 이유 : 안개를 더 부드럽게 보이게 하기 위해
    RenderTexture blurBuffer2;

    RenderTexture curTexture;
    RenderTexture lerpBuffer;
    RenderTexture nextTexture;

    public Texture FogTexture => curTexture;

    public FowManager FM => FowManager.Inst;
    public FowManager.FogAlpha AlphaData => FowManager.Inst._fogAlpha;

    #region Init, Release
    public void InitMap(float[,] heightMap)
    {
        map.Clear();
        mapWidth = heightMap.GetLength(0);
        mapHeight = heightMap.GetLength(1);

        mapLength = mapWidth * mapHeight;

        visit = new float[mapLength];
        colorBuffer = new Color[mapLength];

        for (int i = 0; i < mapLength; i++)
            visit[i] = AlphaData.never;

        blurMat = new Material(Shader.Find("FogOfWar/AverageBlur"));
        texBuffer = new Texture2D(mapWidth, mapHeight, TextureFormat.ARGB32, false);
        texBuffer.wrapMode = TextureWrapMode.Clamp;

        int width = (int)(mapWidth * 1.5f);
        int height = (int)(mapHeight * 1.5f);

        blurBuffer = RenderTexture.GetTemporary(width, height, 0);
        blurBuffer2 = RenderTexture.GetTemporary(width, height, 0);

        curTexture = RenderTexture.GetTemporary(width, height, 0);
        nextTexture = RenderTexture.GetTemporary(width, height, 0);
        lerpBuffer = RenderTexture.GetTemporary(width, height, 0);

        for (int j = 0; j < mapHeight; j++)
        {
            for (int i = 0; i < mapWidth; i++)
            {
                // 타일정보 : 높이, X좌표, Y좌표, 너비
                map.Add(new FowTile(i, j, mapWidth));
            }
        }
    }

    public void Release()
    {
        RenderTexture.ReleaseTemporary(blurBuffer);
        RenderTexture.ReleaseTemporary(blurBuffer2);
        RenderTexture.ReleaseTemporary(curTexture);
        RenderTexture.ReleaseTemporary(nextTexture);
        RenderTexture.ReleaseTemporary(lerpBuffer);
    }

    #endregion

    #region Getter, Calculators
    /// <summary> 해당 위치에 있는 타일 얻어내기 </summary>
    private FowTile GetTile(in int x, in int y)
    {
        if (InMapRange(x, y))
        {
            return map[GetTileIndex(x, y)];
        }
        else
        {
            return null;
        }
    }

    /// <summary> 해당 좌표가 맵 내에 있는지 검사 </summary>
    public bool InMapRange(in int x, in int y)
    {
        return x >= 0 && y >= 0 &&
               x < mapWidth && y < mapHeight;
    }

    /// <summary> (x, y) 타일좌표를 배열인덱스로 변환 </summary>
    public int GetTileIndex(in int x, in int y)
    {
        return x + y * mapWidth;
    }

    #endregion

    #region Public Methods
    // <summary> 이전 프레임의 안개를 현재 프레임에 부드럽게 보간 </summary>
    public void LerpBlur()
    {
        // CurTexture  -> LerpBuffer
        // LerpBuffer  -> "_LastTex"
        // NextTexture -> FogTexture [Pass 1 : Lerp]

        Graphics.Blit(curTexture, lerpBuffer);
        blurMat.SetTexture("_LastTex", lerpBuffer);

        Graphics.Blit(nextTexture, curTexture, blurMat, 1);
    }

    // <summary> 지난 번 시행에 유닛이 존재해서 밝게 나타냈던 부분을 다시 안개로 가려줌 </summary>
    public void RefreshFog()
    {
        for (int i = 0; i < mapLength; i++)
        {
            if (visit[i] == AlphaData.current)
                visit[i] = AlphaData.visited;
        }
    }

    List<FowTile> tilesInSight = new List<FowTile>();
    /// <summary> 타일 위치로부터 시야만큼 안개 계산 </summary>
    public void ComputeFog(TilePos pos, in float sightXZ, in float sightY)
    {
        int sightRangeInt = (int)sightXZ;
        int rangeSquare = sightRangeInt * sightRangeInt;

        // 현재 시야(원형 범위)만큼의 타일들 목록
        // x^2 + y^2 <= range^2
        tilesInSight.Clear();
        for (int i = -sightRangeInt; i <= sightRangeInt; i++)
        {
            for (int j = -sightRangeInt; j <= sightRangeInt; j++)
            {
                if (i * i + j * j <= rangeSquare)
                {
                    var tile = GetTile(pos.x + i, pos.y + j);
                    if (tile != null)
                    {
                        tilesInSight.Add(tile);
                    }
                }
            }
        }

        // 현재 방문, 과거 방문 여부 true
        foreach (FowTile visibleTile in tilesInSight)
        {
            visit[visibleTile.index] = AlphaData.current;
        }

        ApplyFogAlpha();
    }

    #endregion

    #region Private Methods

    /// <summary> 방문 정보를 텍스처의 알파 정보로 변환하고 가우시안 블러 적용 </summary>
    private void ApplyFogAlpha()
    {

        foreach (var tile in map)
        {
            int index = tile.index;

            colorBuffer[index].a = visit[index];
        }

        // ColorBuffer -> TexBuffer
        texBuffer.SetPixels(colorBuffer);
        texBuffer.Apply();

        // TexBuffer -> nextTexture

        // Pass 0 : Blur
        Graphics.Blit(texBuffer, blurBuffer, blurMat, 0);
        Graphics.Blit(blurBuffer, blurBuffer2, blurMat, 0);
        Graphics.Blit(blurBuffer2, blurBuffer, blurMat, 0);
        Graphics.Blit(blurBuffer, nextTexture);
    }
    #endregion
}