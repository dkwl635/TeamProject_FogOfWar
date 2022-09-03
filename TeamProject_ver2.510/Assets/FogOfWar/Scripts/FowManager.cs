using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class FowManager : MonoBehaviour
{
    #region Fog Renderer
    private Material _fogMaterial;
    public GameObject _rendererPrefab;

    void InitFogTexture()
    {

        var renderer = Instantiate(_rendererPrefab, transform);
        renderer.transform.localPosition = Vector3.zero;
        renderer.transform.localScale = new Vector3(_fogWidthX * 0.5f, 1, _fogWidthZ * 0.5f);
        _fogMaterial = renderer.GetComponentInChildren<Renderer>().material;
    }

    void UpdateFogTexture()
    {
        if (Map.FogTexture != null)
        {
            _fogMaterial.SetTexture("_MainTex", Map.FogTexture);
        }
    }

    #endregion
    #region Fields

    [Space]
    public float _fogWidthX = 40;
    public float _fogWidthZ = 40;
    public float _tileSize = 1;       // 타일 하나의 크기
    public float _updateCycle = 0.5f; // 시야 계산 주기

    [System.Serializable]
    public class FogAlpha
    {
        [Range(0, 1)] public float current = 0.0f;
        [Range(0, 1)] public float visited = 0.8f;
        [Range(0, 1)] public float never = 1.0f;
    }
    [Space]
    public FogAlpha _fogAlpha = new FogAlpha();

    public FowMap Map { get; private set; }

    List<FowUnit> UnitList { get; set; } // Fow 시스템이 추적할 유닛들

    #endregion
    #region Unity Events
    // 싱글톤 인스턴스
    public static FowManager Inst;

    void Awake()
    {
        Inst = this;
        UnitList = new List<FowUnit>();
        InitMap();
        InitFogTexture();

    }
    void OnEnable()
    {
        StartCoroutine(UpdateFogRoutine());
    }

    void Update()
    {
        Map.LerpBlur();
        UpdateFogTexture();
    }
    void OnDestroy()
    {
        Map.Release();
    }
    #endregion
    #region Static Methods (Add/Remove)
    public static void AddUnit(FowUnit unit)
    {
        if (!Inst.UnitList.Contains(unit))
        {
            Inst.UnitList.Add(unit);
        }
    }
    public static void RemoveUnit(FowUnit viewer)
    {
        if (Inst.UnitList.Contains(viewer))
        {
            Inst.UnitList.Remove(viewer);
        }
    }

    #endregion
    #region Private Methods
    public void InitMap()
    {
        Map = new FowMap();
        Map.InitMap(new float[(int)(_fogWidthX / _tileSize), (int)(_fogWidthZ / _tileSize)]);
    }

    /// <summary> 대상 유닛의 위치를 타일좌표(x, y, height)로 환산 </summary>
    TilePos GetTilePos(FowUnit unit)
    {
        int x = (int)((unit.transform.position.x - transform.position.x + _fogWidthX * 0.5f) / _tileSize);
        int y = (int)((unit.transform.position.z - transform.position.z + _fogWidthZ * 0.5f) / _tileSize);

        return new TilePos(x, y);
    }

    #endregion
    #region Coroutine
    public IEnumerator UpdateFogRoutine()
    {
        while (true)
        {
            if (Map != null)
            {
                Map.RefreshFog();

                foreach (var unit in UnitList)
                {
                    TilePos pos = GetTilePos(unit);
                    Map.ComputeFog(pos, unit.sightRange / _tileSize, 0);
                }
            }

            yield return new WaitForSeconds(_updateCycle);
        }
    }
    #endregion
}
