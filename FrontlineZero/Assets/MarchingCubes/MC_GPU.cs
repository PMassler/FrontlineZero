using UnityEngine;
using System.Linq;

public class MC_GPU : MonoBehaviour
{
    // marching cube data
    public int chunkSize = 8;
    public Vector3Int chunkCount = new Vector3Int(1, 1, 1);
    public float floorHeight;
    public float arenaRadius;
    public float arenaBoarderHeight;

    public float surfaceLevel = 20f;
    public int pointMaxValue = 40;

    [HideInInspector]
    public Vector4[,,] gridPoints;
    [HideInInspector]
    public Chunk[,,] chunks;

    public Material groundMat;
    public string mcTerrainLayerName;

    // Compute Shader
    const int threadGroupSize = 8;
    public ComputeShader mcCompute;

    // Buffers
    ComputeBuffer triangleBuffer;
    ComputeBuffer pointsBuffer;
    ComputeBuffer triCountBuffer;

    // Arena generation

    public float borderHeight;
    public float borderNoiseScale;
    public float borderHeightNoiseScale;
    public float borderHeightNoiseMultiplier;

    public bool proceduralGeneration;

    // Singleton
    private static MC_GPU _instance;
    public static MC_GPU Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        CreateBuffers();
        GenerateChunks();
        if (proceduralGeneration)
        {
            ProceduralArenaManager.Instance.ProceduralGeneration();
        }
    }

    void GenerateChunks()
    {
        Vector3 arenaCenter = new Vector3((chunkSize * chunkCount.x) / 2f, floorHeight, (chunkSize * chunkCount.z) / 2f);
        // Declare arrays
        int gridWidth = (chunkSize * chunkCount.x) + 1;
        int gridHeight = (chunkSize * chunkCount.y) + 1;
        int gridLength = (chunkSize * chunkCount.z) + 1;

        gridPoints = new Vector4[gridWidth, gridHeight, gridLength];
        chunks = new Chunk[chunkCount.x, chunkCount.y, chunkCount.z];

        // Generate PointGrid

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                for (int z = 0; z < gridLength; z++)
                {
                    // fill terrain above floorheight, and leave terrain below empty
                    if (y > floorHeight)
                    {

                        gridPoints[x, y, z] = new Vector4(x, y, z, pointMaxValue - surfaceLevel / 2);
                    }
                    else
                    {
                        gridPoints[x, y, z] = new Vector4(x, y, z, surfaceLevel / 2);
                    }

                    //ArenaBorderGeneration
                    Vector2 vecToCenter = new Vector2(x, z) - new Vector2(arenaCenter.x, arenaCenter.z);
                    float distanceToCenter = vecToCenter.magnitude;
                    float borderOverlap = distanceToCenter - arenaRadius;
                    float borderWidth = (chunkSize * chunkCount.x) / 2f - arenaRadius;
                    float borderCurve = (-Mathf.Pow(borderOverlap / borderWidth, 2f)/0.5f + (borderOverlap / borderWidth)*4 );
                    if (borderOverlap > 0f && floorHeight < y)
                    {
                        float angle = Vector2.Angle(vecToCenter, Vector2.one);
                        float borderHeightNoiseValue = Mathf.PerlinNoise(0f, angle * borderHeightNoiseScale) *borderHeightNoiseMultiplier;

                        if (y < floorHeight + borderCurve * borderHeight +borderHeightNoiseValue)
                        {
                            gridPoints[x, y, z] = new Vector4(x, y, z, surfaceLevel / 2);
                        }
                    }
                }
            }
        }

        // Generate Chunks
        for (int x = 0; x < chunkCount.x; x++)
        {
            for (int y = 0; y < chunkCount.y; y++)
            {
                for (int z = 0; z < chunkCount.z; z++)
                {
                    chunks[x, y, z] = new Chunk(new Vector3Int(x, y, z), this);
                }
            }
        }
        UpdateAllChunks();
    }

    public void UpdateAllChunks()
    {
        for (int x = 0; x < chunkCount.x; x++)
        {
            for (int y = 0; y < chunkCount.y; y++)
            {
                for (int z = 0; z < chunkCount.z; z++)
                {
                    UpdateChunk(new Vector3Int(x, y, z));
                }
            }
        }
    }

    public void UpdateChunk(Vector3Int chunkIndex)
    {
        // set current chunk to be updated
        Chunk cChunk = chunks[chunkIndex.x, chunkIndex.y, chunkIndex.z];

        // turn gridPoints into 1d array for pointsBuffer
        Vector4[] pointArray = new Vector4[cChunk.pointRef1D.Length];
        for (int i = 0; i < pointArray.Length; i++)
        {
            pointArray[i] = gridPoints[cChunk.pointRef1D[i].x, cChunk.pointRef1D[i].y, cChunk.pointRef1D[i].z];
        }
        pointsBuffer.SetData(pointArray);

        // initialize compute shader
        int numThreadsPerAxis = Mathf.CeilToInt(chunkSize / (float)threadGroupSize);
        int kernel = mcCompute.FindKernel("MarchCubesInChunk");

        triangleBuffer.SetCounterValue(0);
        // input buffer
        mcCompute.SetBuffer(kernel, "points", pointsBuffer);
        // outputBuffer
        mcCompute.SetBuffer(kernel, "triangles", triangleBuffer);
        mcCompute.SetInt("chunkSize", chunkSize);
        mcCompute.SetInts("chunkIndex", chunkIndex.x, chunkIndex.y, chunkIndex.z);
        mcCompute.SetFloat("surfaceLevel", surfaceLevel);
        mcCompute.Dispatch(kernel, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

        // get number of triangles in the triangle buffer
        ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);
        int[] triCountArray = { 0 };
        triCountBuffer.GetData(triCountArray);
        int numTris = triCountArray[0];       

        // get triangle data from shader
        Triangle[] tris = new Triangle[numTris];
        triangleBuffer.GetData(tris, 0, 0, numTris);

        // initialize mesh
        Mesh mesh = cChunk.mesh;
        mesh.Clear();

        // assign vertices to triangles
        Vector3[] vertices = new Vector3[numTris * 3];
        int[] meshTriangles = new int[numTris * 3];

        for (int i = 0; i < numTris; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                meshTriangles[i * 3 + j] = i * 3 + j;
                vertices[i * 3 + j] = tris[i][j];
            }
        }

        // set mesh data
        mesh.vertices = vertices;
        mesh.triangles = meshTriangles;
        mesh.triangles = mesh.triangles.Reverse().ToArray();
        mesh.RecalculateNormals();      
        cChunk.chunkMC.sharedMesh = mesh;
    }

    void CreateBuffers()
    {
        int numPoints = (chunkSize + 1) * (chunkSize + 1) * (chunkSize + 1);
        int numVoxelsPerAxis = chunkSize;
        int numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
        int maxTriangleCount = numVoxels * 5;

        if (!Application.isPlaying || (pointsBuffer == null || numPoints != pointsBuffer.count))
        {
            if (Application.isPlaying)
            {
                // assure not to create buffers twice
                ReleaseBuffers();
            }
            triangleBuffer = new ComputeBuffer(maxTriangleCount, sizeof(float) * 3 * 3, ComputeBufferType.Append);
            pointsBuffer = new ComputeBuffer(numPoints, sizeof(int) * 4);
            triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        }
    }

    void OnDestroy()
    {
        if (Application.isPlaying)
        {
            ReleaseBuffers();
        }
    }

    void ReleaseBuffers()
    {
        if (triangleBuffer != null)
        {
            triangleBuffer.Release();
            pointsBuffer.Release();
            triCountBuffer.Release();
        }
    }

    public class Chunk
    {
        // general stuff
        public GameObject chunkGO;
        public MeshFilter chunkMF;
        public MeshRenderer chunkMR;
        public MeshCollider chunkMC;
        public Mesh mesh;
        public MC_GPU mcScript;

        // marching cube stuff
        public Vector3Int chunkIndex;
        public Vector3Int[] pointRef1D;

        public Chunk(Vector3Int _chunkIndex, MC_GPU script)
        {
            mcScript = script;
            chunkGO = new GameObject();
            chunkGO.transform.parent = script.transform;
            chunkGO.name = "Chunk_" + _chunkIndex.x + "_" + _chunkIndex.y + "_" + _chunkIndex.z;
            chunkGO.layer = LayerMask.NameToLayer(script.mcTerrainLayerName);
            chunkGO.tag = "MCTerrain";
            chunkMF = chunkGO.AddComponent<MeshFilter>();
            chunkMR = chunkGO.AddComponent<MeshRenderer>();
            chunkMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            chunkMR.sharedMaterial = script.groundMat;
            chunkMC = chunkGO.AddComponent<MeshCollider>();
            mesh = new Mesh();
            chunkMF.mesh = mesh;
            chunkMC.sharedMesh = mesh;
            //chunkMC.cookingOptions = MeshColliderCookingOptions.WeldColocatedVertices;

            //3d to 1d for easier conversion to buffer   
            chunkIndex = _chunkIndex;
            pointRef1D = new Vector3Int[(script.chunkSize + 1) * (script.chunkSize + 1) * (script.chunkSize + 1)];
            for (int x = 0; x < script.chunkSize + 1; x++)
            {
                for (int y = 0; y < script.chunkSize + 1; y++)
                {
                    for (int z = 0; z < script.chunkSize + 1; z++)
                    {
                        int index = z * (script.chunkSize + 1) * (script.chunkSize + 1) + y * (script.chunkSize + 1) + x;
                        pointRef1D[index] = new Vector3Int(chunkIndex.x * (script.chunkSize) + x, chunkIndex.y * (script.chunkSize) + y, chunkIndex.z * (script.chunkSize) + z);
                    }
                }
            }
        }
    }

    public Chunk GetChunk(Vector3Int index)
    {
        return chunks[index.x, index.y, index.z];
    }

    struct Triangle
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public Vector3 this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return a;
                    case 1:
                        return b;
                    default:
                        return c;
                }
            }
        }
    }
}
