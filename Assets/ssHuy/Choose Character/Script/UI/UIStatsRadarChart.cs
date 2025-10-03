using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UIStatsRadarChart : MonoBehaviour
{
    private Stats stats;
    [SerializeField] Material radarMaterial;
    [SerializeField] Texture2D radarTexture;
    private CanvasRenderer radarMeshcanvasRenderer;
    private void Awake()
    {
        radarMeshcanvasRenderer = transform.Find("radarMesh").GetComponent<CanvasRenderer>();
    }
    public void SetStats(Stats stats)
    {
        this.stats = stats;
        stats.OnStatsChanged += Stats_OnStatsChanged;
        UpdateStatsVisual();
    }
    private void Stats_OnStatsChanged(object sender, System.EventArgs e)
    {
        UpdateStatsVisual();
    }
    private void UpdateStatsVisual()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[6];
        Vector2[] uv = new Vector2[6];
        int[] triangles = new int[3 * 5];
        float angleIncrement = 360f / 5;
        float radarChartSize = 100f;
        Vector3 attackvertex = Quaternion.Euler(0, 0, -angleIncrement * 0) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(Stats.Type.Attack);
        int attackvertexIndex = 1;
        Vector3 defensevertex = Quaternion.Euler(0, 0, -angleIncrement * 1) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(Stats.Type.Defense);
        int defensevertexIndex = 2;
        Vector3 Speedvertex = Quaternion.Euler(0, 0, -angleIncrement * 2) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(Stats.Type.Speed);
        int SpeedvertexIndex = 3;
        Vector3 Manavertex = Quaternion.Euler(0, 0, -angleIncrement * 3) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(Stats.Type.Mana);
        int ManavertexIndex = 4;
        Vector3 Healthvertex = Quaternion.Euler(0, 0, -angleIncrement * 4) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(Stats.Type.Health);
        int HealthvertexIndex = 5;

        vertices[0] = Vector3.zero;
        vertices[attackvertexIndex]     = attackvertex;
        vertices[defensevertexIndex]    = defensevertex;
        vertices[SpeedvertexIndex]      = Speedvertex;
        vertices[ManavertexIndex]       = Manavertex;
        vertices[HealthvertexIndex]     = Healthvertex;

        triangles[0] = 0;
        triangles[1] = attackvertexIndex;
        triangles[2] = defensevertexIndex;

        triangles[3] = 0;
        triangles[4] = defensevertexIndex;
        triangles[5] = SpeedvertexIndex;

        triangles[6] = 0;
        triangles[7] = SpeedvertexIndex;
        triangles[8] = ManavertexIndex;

        triangles[9] = 0;
        triangles[10]= ManavertexIndex;
        triangles[11]= HealthvertexIndex;
        
        triangles[12]= 0;
        triangles[13]= HealthvertexIndex;
        triangles[14]= attackvertexIndex;

        uv[0] = Vector2.zero;
        uv[attackvertexIndex]   = Vector2.one;
        uv[defensevertexIndex]  = Vector2.one;
        uv[SpeedvertexIndex]    = Vector2.one;
        uv[ManavertexIndex]     = Vector2.one;
        uv[HealthvertexIndex]   = Vector2.one;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        radarMeshcanvasRenderer.SetMesh(mesh);
        radarMeshcanvasRenderer.SetMaterial(radarMaterial, radarTexture);
    }
}
