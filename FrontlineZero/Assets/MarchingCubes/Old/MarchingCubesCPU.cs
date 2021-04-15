using System.Collections.Generic;
using UnityEngine;

public class MarchingCubesCPU : MonoBehaviour
{
    public bool drawGizmos;

    public int gridWidth;
    public int gridHeight;
    public int gridLength;

    int threshholdValue = 10;

    [HideInInspector]
    public int gridPointMaxValue = 20;

    [HideInInspector]
    public GridPoint[,,] gridPoints;
    [HideInInspector]
    public Cube[,,] cubes;

  
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    public Vector3 offSet;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        GenerateGrid();
    }

    // generate and initialize GridPoints and Cubes
    public void GenerateGrid()
    {
        gridPoints = new GridPoint[gridWidth, gridHeight, gridLength];
        cubes = new Cube[gridWidth-1, gridHeight-1, gridLength-1];

        // create and initialize Gridpoints
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                for (int z = 0; z < gridLength; z++)
                {
                    if(x == 0 || y == 0 || z == 0 || x == gridWidth-1 || y == gridHeight-1 || z == gridLength - 1 || y > gridHeight/4)
                    {
                        // gridPoints above floorheight, and on the edge get assigned highest value
                        gridPoints[x, y, z] = new GridPoint(new Vector3(x, y, z), gridPointMaxValue);
                    }
                    else
                    {
                        // gridpoints beneath floorheigt get assigned 0
                        gridPoints[x, y, z] = new GridPoint(new Vector3(x, y, z), 0);
                    }
                }
            }
        }

        // create cubes and assign corner points
        for (int x = 0; x < gridWidth-1; x++)
        {
            for (int y = 0; y < gridHeight-1; y++)
            {
                for (int z = 0; z < gridLength-1; z++)
                {
                    cubes[x, y, z] = new Cube(x,y,z, gridPoints);
                }
            }
        }

        UpdateMesh();
    }


    [ContextMenu("UpdateMesh")]
    public void UpdateMesh()
    {
        List <Vector3>  vertices = new List<Vector3>();
        List <int>  triangles = new List<int>();

        int vertexCounter = 0;
        for (int x = 0; x < gridWidth - 1; x++)
        {
            for (int y = 0; y < gridHeight - 1; y++)
            {
                for (int z = 0; z < gridLength - 1; z++)
                {
                    Cube cCube = cubes[x, y, z];

                    int cubeIndex = 0;
                    if (cCube.cornerPoints[0].currentValue < threshholdValue) cubeIndex |= 1;
                    if (cCube.cornerPoints[1].currentValue < threshholdValue) cubeIndex |= 2;
                    if (cCube.cornerPoints[2].currentValue < threshholdValue) cubeIndex |= 4;
                    if (cCube.cornerPoints[3].currentValue < threshholdValue) cubeIndex |= 8;
                    if (cCube.cornerPoints[4].currentValue < threshholdValue) cubeIndex |= 16;
                    if (cCube.cornerPoints[5].currentValue < threshholdValue) cubeIndex |= 32;
                    if (cCube.cornerPoints[6].currentValue < threshholdValue) cubeIndex |= 64;
                    if (cCube.cornerPoints[7].currentValue < threshholdValue) cubeIndex |= 128;                    

                    int[] triangulation = new int[16];
                    for (int i = 0; i < 16; i++)
                    {
                        triangulation[i] = LookUpTables.TriangleConnectionTable[cubeIndex, i];
                    }
                   
                    foreach(int edgeIndex in triangulation)
                    {
                        if (edgeIndex != -1)
                        {
                            int indexA = LookUpTables.cornerIndexAFromEdge[edgeIndex];
                            int indexB = LookUpTables.cornerIndexBFromEdge[edgeIndex];                            
                            float interPolValue = Mathf.InverseLerp(cCube.cornerPoints[indexA].currentValue, cCube.cornerPoints[indexB].currentValue, threshholdValue);
                            Vector3 vertexPos = Vector3.Lerp(cCube.cornerPoints[indexA].position , cCube.cornerPoints[indexB].position, interPolValue) + offSet;                       
                            vertices.Add(vertexPos);
                            triangles.Add(vertexCounter);
                            vertexCounter++;                            
                        }
                    }
                }
            }
        }

        // Mesh creation      
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();       
        mesh.RecalculateNormals();        
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }    


    public class GridPoint
    {
        public GridPoint(Vector3 pos, int startValue)
        {
            currentValue = startValue;
            position = pos;
        }
        public int currentValue;
        public Vector3 position;
        public Transform debugSphere;
    }

    public class Cube
    {
        public GridPoint[] cornerPoints;

        public Cube(int x, int y, int z, GridPoint[,,] gridPoints)
        {
            cornerPoints = new GridPoint[8];

            cornerPoints[0] = gridPoints[x, y, z];
            cornerPoints[1] = gridPoints[x + 1, y, z];
            cornerPoints[2] = gridPoints[x + 1, y, z + 1];
            cornerPoints[3] = gridPoints[x, y, z + 1];

            cornerPoints[4] = gridPoints[x, y + 1, z]; 
            cornerPoints[5] = gridPoints[x + 1, y + 1, z];
            cornerPoints[6] = gridPoints[x + 1, y + 1, z + 1];
            cornerPoints[7] = gridPoints[x, y + 1, z + 1];
        }
    }
}
