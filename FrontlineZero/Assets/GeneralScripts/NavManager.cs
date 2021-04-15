using System.Collections.Generic;
using UnityEngine;

public class NavManager : MonoBehaviour
{
    public float navCheckHeight;
    public float arenaMargin;
    public Vector3 arenaMidpoint;
    float arenaScale;

    // Singleton
    private static NavManager _instance;
    public static NavManager Instance { get { return _instance; } }

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

    private void Start()
    {
        CalcArenaBounds();
    }

    void CalcArenaBounds()
    {
        // calculate the size of the arena and the middle point
        arenaMidpoint = new Vector3((MC_GPU.Instance.chunkCount.x * MC_GPU.Instance.chunkSize) / 2f, MC_GPU.Instance.floorHeight, (MC_GPU.Instance.chunkCount.z * MC_GPU.Instance.chunkSize) / 2f);
        arenaScale = (MC_GPU.Instance.chunkCount.x * MC_GPU.Instance.chunkSize) / 2f - arenaMargin;
    }  

    // Get Movement Vector with target position
    public Vector3 GetMovementVector(Vector3 pos, float checkAngle, int rayCountPerDir, float dirCheckLength, float tolerance, float startOffset, Vector3 targetPos)
    {
        // Initialize Values
        Vector3 startDirection;
        float currentAngle = 0f;
        int counter = 0;
        List<Vector4> checkedDirections = new List<Vector4>();
        float floorHeight = MC_GPU.Instance.floorHeight;

        // Set Startdirection from targetPos
        startDirection = Vector3.ProjectOnPlane(targetPos - pos, Vector3.up).normalized;

        // Check all directions around start position, if a direction is within tolerance return this direction
        while (currentAngle < 90)
        {
            Vector4 directionValue;
            if (counter % 2 == 0)
            {
                directionValue = CheckDirection(pos, floorHeight, Quaternion.AngleAxis(currentAngle, Vector3.up) * startDirection, rayCountPerDir, dirCheckLength, startOffset);
            }
            else
            {
                directionValue = CheckDirection(pos, floorHeight, Quaternion.AngleAxis(-currentAngle, Vector3.up) * startDirection, rayCountPerDir, dirCheckLength, startOffset);
                currentAngle += checkAngle;
            }
            counter++;

            if (directionValue.w <= tolerance)
            {
                //Debug.Log(directionValue.w);
                return (Vector3)directionValue;
            }
            else
            {
                checkedDirections.Add(directionValue);
            }
        }

        // If no direction is within the tolerance, return the best one
        Vector4 bestVector = Vector4.positiveInfinity;
        for (int i = 0; i < checkedDirections.Count; i++)
        {
            if (checkedDirections[i].w < bestVector.w)
            {
                bestVector = checkedDirections[i];
            }
        }
        return (Vector3)bestVector;
    }

    //Get random movement vector without target position
    public Vector3 GetMovementVector(Vector3 pos, float checkAngle, int rayCountPerDir, float dirCheckLength, float tolerance, float startOffset)
    {
        // Initialize Values
        Vector3 startDirection;
        float currentAngle = 0f;
        int counter = 0;
        List<Vector4> checkedDirections = new List<Vector4>();
        float floorHeight = MC_GPU.Instance.floorHeight;

        // Set random Startdirection
        startDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        // Check all directions around start position, if a direction is within tolerance return this direction
        while (currentAngle < 180)
        {
            Vector4 directionValue;
            if (counter % 2 == 0)
            {
                directionValue = CheckDirection(pos, floorHeight, Quaternion.AngleAxis(currentAngle, Vector3.up) * startDirection, rayCountPerDir, dirCheckLength, startOffset);
            }
            else
            {
                directionValue = CheckDirection(pos, floorHeight, Quaternion.AngleAxis(-currentAngle, Vector3.up) * startDirection, rayCountPerDir, dirCheckLength, startOffset);
                currentAngle += checkAngle;
            }
            counter++;

            if (directionValue.w <= tolerance)
            {
                return (Vector3)directionValue;
            }
            else
            {
                checkedDirections.Add(directionValue);
            }
        }

        // If no direction is within the tolerance, return the best one
        Vector4 bestVector = Vector4.positiveInfinity;
        for (int i = 0; i < checkedDirections.Count; i++)
        {
            if (checkedDirections[i].w < bestVector.w)
            {
                bestVector = checkedDirections[i];
            }
        }
        return (Vector3)bestVector;
    }

    Vector4 CheckDirection(Vector3 pos, float floorHeight, Vector3 dir, int checkCount, float checkLength, float startOffset)
    {
        float vectorValue = 0;
        RaycastHit hit;
        float straightCheckValue = 0f;

        // Checek horizontaly in the direction
        if (Physics.Raycast(pos + dir.normalized*startOffset, dir, out hit))
        {
            straightCheckValue = Mathf.Clamp(50f - Vector3.Distance(pos, hit.point),0f,50f);
        }

        // Check height of terrain in the direction over the the length of checkLength, checkCount times
        for (int i = 0; i < checkCount; i++)
        {
            Vector3 checkPos = pos + dir * i * checkLength + dir * startOffset;
            checkPos.y = navCheckHeight;

            if (Physics.Raycast(checkPos, -Vector3.up, out hit))
            {
                vectorValue += Mathf.Abs(floorHeight - hit.point.y);
            }
        }
        vectorValue += straightCheckValue;

        // the further apart vectorValue is from 0, the more rough the terrain in the direction
        return new Vector4(dir.x, dir.y, dir.z, vectorValue);
    }

    // checks ift position ist inside arena
    public bool CheckIfPosIsInBounds(Vector3 pos)
    {
        if(Vector3.ProjectOnPlane(pos-arenaMidpoint,Vector3.up).magnitude < arenaScale)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // returns closest position in arena radius
    public Vector3 PutPosInBounds(Vector3 pos)
    {
        Vector3 returnPos;       
        Vector3 posInArena = Vector3.ProjectOnPlane(pos - arenaMidpoint, Vector3.up);
        if (posInArena.magnitude < arenaScale)
        {
            returnPos = pos;
        }
        else
        {
            returnPos = pos - posInArena.normalized * (posInArena.magnitude - (arenaScale));
        }
        return returnPos;
    }

    // returns closest position in arena radius with additional margin
    public Vector3 PutPosInBounds(Vector3 pos, float additionalMargin)
    {
        Vector3 returnPos;
        Vector3 posInArena = Vector3.ProjectOnPlane(pos - arenaMidpoint, Vector3.up);
        if (posInArena.magnitude < arenaScale - additionalMargin)
        {
            returnPos =  pos;
        }
        else
        {
            returnPos = pos - posInArena.normalized * (posInArena.magnitude - (arenaScale - additionalMargin));            
        }        
        return returnPos;
    }

    // return position in arena that lies und the radius
    public Vector3 GetValidPosOnRadius(Vector3 pos, float radius)
    {
        int counter = 0;
        while(counter < 20)
        {
            Vector3 newPos = pos + Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * new Vector3(radius, 0f, 0f);
            if (CheckIfPosIsInBounds(newPos))
            {
                return newPos;
            }
            counter++;
        }
        return PutPosInBounds(pos + Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * new Vector3(radius, 0f, 0f));
    }

    // returns random position inside arena
    public Vector3 GetRandomPosInBounds(float margin)
    {
        float centerDistance = Mathf.Sqrt(Random.Range(0, Mathf.Pow(arenaScale - margin, 2)));
        Vector3 randomPos = arenaMidpoint + Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * new Vector3(centerDistance, 0f, 0f);
        return randomPos;
    }
}
