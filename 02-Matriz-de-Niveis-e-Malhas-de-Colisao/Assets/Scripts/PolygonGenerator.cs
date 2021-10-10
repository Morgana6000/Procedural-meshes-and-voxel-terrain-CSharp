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

    private int squareCount;

    public byte[,] blocks;

    public List<Vector3> colVertices = new List<Vector3>();
    public List<int> colTriangles = new List<int>();
    private int colCount;

    private MeshCollider col;

    void Start() {
        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        col = this.gameObject.AddComponent<MeshCollider>();
        meshRenderer.material = material;

        mesh = GetComponent<MeshFilter>().mesh;        

        GenTerrain();
        BuildMesh();
        UpdateMesh();

        col = GetComponent<MeshCollider>();
    }

    void Update() {
        
    }

    void UpdateMesh() {
        mesh.Clear();
        mesh.vertices = newVectices.ToArray();
        mesh.triangles = newTriangles.ToArray();        
        mesh.uv = newUV.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        squareCount = 0;
        newVectices.Clear();
        newTriangles.Clear();
        newUV.Clear();

        Mesh newMesh = new Mesh();
        newMesh.vertices = colVertices.ToArray();
        newMesh.triangles = colTriangles.ToArray();
        col.sharedMesh = newMesh;

        colVertices.Clear();
        colTriangles.Clear();
        colCount = 0;
    }

    void GenSquare(int x, int y, Vector2 texture) {
        float z = transform.position.z;

        newVectices.Add(new Vector3(x, y, z));
        newVectices.Add(new Vector3(x + 1, y, z));
        newVectices.Add(new Vector3(x + 1, y - 1, z));
        newVectices.Add(new Vector3(x, y - 1, z));

        newTriangles.Add((squareCount * 4) + 0);
        newTriangles.Add((squareCount * 4) + 1);
        newTriangles.Add((squareCount * 4) + 2);
        newTriangles.Add((squareCount * 4) + 0);
        newTriangles.Add((squareCount * 4) + 2);
        newTriangles.Add((squareCount * 4) + 3);

        squareCount++;

        newUV.Add(new Vector2(tUnit * texture.x, tUnit * texture.y + tUnit));
        newUV.Add(new Vector2(tUnit * texture.x + tUnit, tUnit * texture.y + tUnit));
        newUV.Add(new Vector2(tUnit * texture.x + tUnit, tUnit * texture.y));
        newUV.Add(new Vector2(tUnit * texture.x, tUnit * texture.y));
    }

    void GenTerrain() {
        blocks = new byte[10, 10];

        for(int px = 0; px < blocks.GetLength(0); px++) {
            for(int py = 0; py < blocks.GetLength(1); py++) {
                if(py == 5) {
                    blocks[px, py] = 2;
                }
                else if(py < 5) {
                    blocks[px, py] = 1;
                }
            }
        }
    }

    void BuildMesh() {
        for(int px = 0; px < blocks.GetLength(0); px++) {
            for(int py = 0; py < blocks.GetLength(1); py++) {
                // Se o bloco não for ar
                if(blocks[px, py] != 0) {
                    // GenCollider aqui, isso o aplicará a todos os blocos que não sejam ar.
                    GenCollider(px, py);

                    if(blocks[px, py] == 1) {
                    GenSquare(px, py, tStone);
                    }
                    else if(blocks[px, py] == 2) {
                        GenSquare(px, py, tGrass);
                    }
                } // Fim da verificação dos blocos de ar.
            }
        }
    }

    void GenCollider(int x, int y) {
        // Top
        if(Block(x, y + 1) == 0) {
            colVertices.Add(new Vector3(x, y, 1));
            colVertices.Add(new Vector3(x + 1, y, 1));
            colVertices.Add(new Vector3(x + 1, y, 0));
            colVertices.Add(new Vector3(x, y, 0));

            ColliderTriangles();

            colCount++;
        }        
        
        // bot
        if(Block(x, y - 1) == 0) {
            colVertices.Add(new Vector3(x, y - 1, 0));
            colVertices.Add(new Vector3(x + 1, y - 1, 0));
            colVertices.Add(new Vector3(x + 1, y - 1, 1));
            colVertices.Add(new Vector3(x, y - 1, 1));

            ColliderTriangles();

            colCount++;
        }        
        
        // left
        if(Block(x - 1, y) == 0) {
            colVertices.Add(new Vector3(x, y - 1, 1));
            colVertices.Add(new Vector3(x, y, 1));
            colVertices.Add(new Vector3(x, y, 0));
            colVertices.Add(new Vector3(x, y - 1, 0));

            ColliderTriangles();

            colCount++;
        }
        
        // right
        if(Block(x + 1, y) == 0) {
            colVertices.Add(new Vector3(x + 1, y, 1));
            colVertices.Add(new Vector3(x + 1, y - 1, 1));
            colVertices.Add(new Vector3(x + 1, y - 1, 0));
            colVertices.Add(new Vector3(x + 1, y, 0));

            ColliderTriangles();

            colCount++;
        }
    }

    void ColliderTriangles() {
        colTriangles.Add((colCount * 4) + 0);
        colTriangles.Add((colCount * 4) + 1);
        colTriangles.Add((colCount * 4) + 2);
        colTriangles.Add((colCount * 4) + 0);
        colTriangles.Add((colCount * 4) + 2);
        colTriangles.Add((colCount * 4) + 3);
    }

    byte Block(int x, int y) {
        if(x == -1 || x == blocks.GetLength(0) || y == -1 || y == blocks.GetLength(1)) {
            return (byte)1;
        }

        return blocks[x, y];
    }
}
