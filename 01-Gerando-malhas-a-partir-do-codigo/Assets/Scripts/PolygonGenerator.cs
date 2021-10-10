using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonGenerator : MonoBehaviour {
    // Esta primeira lista contém todos os vértices da malha que iremos renderizar.
    public List<Vector3> newVectices = new List<Vector3>();

    // Os triângulos dizem ao Unity como construir cada seção da malha unindo os vértices.
    public List<int> newTriangles = new List<int>();

    // A lista UV não é importante agora, mas diz ao Unity como a textura é alinhada em cada polígono.
    public List<Vector2> newUV = new List<Vector2>();

    // Uma malha é composta de vértices, triângulos e UVs que vamos definir, depois de criá-los, vamos salvá-los como esta malha.
    private Mesh mesh;

    public Material material;

    private float tUnit = 1.0f / 4;
    private Vector2 tStone = new Vector2(0, 0);
    private Vector2 tGrass = new Vector2(0, 1);

    void Start() {
        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = this.gameObject.AddComponent<MeshCollider>();
        meshRenderer.material = material;

        mesh = GetComponent<MeshFilter>().mesh;

        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        newVectices.Add(new Vector3(x, y, z));
        newVectices.Add(new Vector3(x + 1, y, z));
        newVectices.Add(new Vector3(x + 1, y - 1, z));
        newVectices.Add(new Vector3(x, y - 1, z));

        newTriangles.Add(0);
        newTriangles.Add(1);
        newTriangles.Add(2);
        newTriangles.Add(0);
        newTriangles.Add(2);
        newTriangles.Add(3);

        newUV.Add(new Vector2(tUnit * tStone.x, tUnit * tStone.y + tUnit));
        newUV.Add(new Vector2(tUnit * tStone.x + tUnit, tUnit * tStone.y + tUnit));
        newUV.Add(new Vector2(tUnit * tStone.x + tUnit, tUnit * tStone.y));
        newUV.Add(new Vector2(tUnit * tStone.x, tUnit * tStone.y));        
    }

    void Update() {
        mesh.Clear();
        mesh.vertices = newVectices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();
    }
}
