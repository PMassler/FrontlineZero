using UnityEngine;

// just for debug purposes
public class VRAudioDebugger : MonoBehaviour
{
    public AudioSource trueAS;
    public AudioSource falseAS;

    // Singleton
    private static VRAudioDebugger _instance;
    public static VRAudioDebugger Instance { get { return _instance; } }

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

    public void DebugTrue()
    {
        trueAS.Play();
    }

    public void DebugFalse()
    {
        falseAS.Play();
    }

}
