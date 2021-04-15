using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public GameObject dummyPrefab;
    Vector3 nextPos;
    Quaternion nextRot;

    // Singleton
    private static SimulationManager _instance;
    public static SimulationManager Instance { get { return _instance; } }

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

    public void SpawnDummy(Vector3 pos, Quaternion rot)
    {
        nextPos = pos;
        nextRot = rot;
        Invoke("InstatiateDummy", 1);
    }

    void InstatiateDummy()
    {
        Instantiate(dummyPrefab, nextPos, nextRot);
    }
}
