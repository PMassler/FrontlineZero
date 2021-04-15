using UnityEngine;

public class MC_EditInterface : MonoBehaviour
{
    public MC_GPU mcManager;
    public Vector3Int pointToEdit;
    public int editStrength;
    public float editRadius;

    // Singleton
    private static MC_EditInterface _instance;

    public static MC_EditInterface Instance { get { return _instance; } }


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

    // returns float from 0 to 1, being the percentage of points in the sphere above the isosurface
    public float CheckSphere(Vector3 pos, float radius)
    {
        int pointsChecked = 0;
        int pointsUnderSurface = 0;
        // determine points in sphere
        Vector3Int[] pointsAffected = GetAffectedPoints(pos, radius);
        Vector4[] pointArray = new Vector4[pointsAffected.Length];

        for (int i = 0; i < pointArray.Length; i++)
        {
            pointArray[i] = mcManager.gridPoints[pointsAffected[i].x, pointsAffected[i].y, pointsAffected[i].z];
        }

        for (int i = 0; i < pointArray.Length; i++)
        {
            if (Vector3.Distance(ToVector3(pointArray[i]), pos) < radius)
            {
                pointsChecked++;
                if (mcManager.gridPoints[(int)pointArray[i].x, (int)pointArray[i].y, (int)pointArray[i].z].w < mcManager.surfaceLevel)
                {
                    pointsUnderSurface++;
                }
            }
        }

        // return Percentage of points under Surface
        return (float)pointsUnderSurface / (float)pointsChecked;
    }

    public float ModifySphere(Vector3 pos, float radius, float strength,float smoothness)
    {
        int pointsModified = 0;
        int pointsUnderSurface = 0;        
        // determine affected chunks and points
        Vector3Int[] chunksAffected = GetAffectedChunks(pos, radius);
        Vector3Int[] pointsAffected = GetAffectedPoints(pos, radius);             
        Vector4[] pointArray = new Vector4[pointsAffected.Length];

        for (int i = 0; i < pointArray.Length; i++)
        {
            pointArray[i] = mcManager.gridPoints[pointsAffected[i].x, pointsAffected[i].y, pointsAffected[i].z];
        }

        // Loop over and modify each point
        for (int i = 0; i < pointArray.Length; i++)
        {
            if (Vector3.Distance(ToVector3(pointArray[i]), pos) < radius)
            {
                pointsModified++;                
                if(mcManager.gridPoints[(int)pointArray[i].x, (int)pointArray[i].y, (int)pointArray[i].z].w < mcManager.surfaceLevel)
                {
                    pointsUnderSurface++;
                }

                float smoothedStrength = Mathf.Lerp(strength, (1-Vector3.Distance(pointArray[i], pos)/radius) * strength, smoothness);
                int newVal = Mathf.Clamp( (int) pointArray[i].w + (int)smoothedStrength, 0,mcManager.pointMaxValue);
                mcManager.gridPoints[(int)pointArray[i].x, (int)pointArray[i].y, (int)pointArray[i].z].w = newVal;
            }
        }
        
        // Update Chunks
        for (int c = 0; c < chunksAffected.Length; c++)
        {
            // Check if in Bounds
            if (chunksAffected[c].x >= 0 && chunksAffected[c].x < mcManager.chunkCount.x && chunksAffected[c].y >= 0 && chunksAffected[c].y < mcManager.chunkCount.y && chunksAffected[c].z >= 0 && chunksAffected[c].z < mcManager.chunkCount.z)
            {               
                mcManager.UpdateChunk(chunksAffected[c]);
            }
        }

        // return Percentage of points under Surface
        return (float)pointsUnderSurface / (float)pointsModified;
    }

    public Vector3Int GetClosestChunk(Vector3 pos)
    {
        return Vector3Int.FloorToInt(pos/mcManager.chunkSize);
    }

    public Vector3Int[] GetAffectedChunks(Vector3 pos, float radius)
    {
        Vector3Int startChunk = GetClosestChunk(pos - new Vector3(radius, radius, radius));
        Vector3Int endChunk = GetClosestChunk(pos + new Vector3(radius, radius, radius));

        Vector3Int bigChunkSize = endChunk - startChunk + new Vector3Int(1,1,1);
        Vector3Int[] affectedChunks = new Vector3Int[bigChunkSize.x* bigChunkSize.y* bigChunkSize.z];
        int counter = 0;
        for (int x = startChunk.x; x < endChunk.x+1; x++)
        {
            for (int y = startChunk.y; y < endChunk.y+1; y++)
            {
                for (int z = startChunk.z; z < endChunk.z+1; z++)
                {
                    affectedChunks[counter] = new Vector3Int(x,y,z);
                    counter++;

                }
            }
        }
        return affectedChunks;
    }
    public Vector3Int[] GetAffectedPoints(Vector3 pos, float radius)
    {
        Vector3Int startPoint = Vector3Int.FloorToInt(pos - new Vector3(radius, radius, radius));
        startPoint.x = Mathf.Clamp(startPoint.x, 0, mcManager.gridPoints.GetLength(0) - 1);
        startPoint.y = Mathf.Clamp(startPoint.y, 0, mcManager.gridPoints.GetLength(1) - 1);
        startPoint.z = Mathf.Clamp(startPoint.z, 0, mcManager.gridPoints.GetLength(2) - 1);
        Vector3Int endPoint = Vector3Int.CeilToInt(pos + new Vector3(radius, radius, radius));
        endPoint.x = Mathf.Clamp(endPoint.x, 0, mcManager.gridPoints.GetLength(0) - 1);
        endPoint.y = Mathf.Clamp(endPoint.y, 0, mcManager.gridPoints.GetLength(1) - 1);
        endPoint.z = Mathf.Clamp(endPoint.z, 0, mcManager.gridPoints.GetLength(2)-1);

        Vector3Int affectedPointsSize = endPoint - startPoint + new Vector3Int(1, 1, 1);
        Vector3Int[] affectedPoints = new Vector3Int[affectedPointsSize.x * affectedPointsSize.y * affectedPointsSize.z];
        int counter = 0;
        for (int x = startPoint.x; x < endPoint.x + 1; x++)
        {
            for (int y = startPoint.y; y < endPoint.y + 1; y++)
            {
                for (int z = startPoint.z; z < endPoint.z + 1; z++)
                {
                    affectedPoints[counter] = new Vector3Int(x, y, z);
                    counter++;

                }
            }
        }
        return affectedPoints;
    }

    // Support
    public Vector3 ToVector3(Vector4 parent)
    {
        return new Vector3(parent.x, parent.y, parent.z);
    }
}
