using UnityEngine;

// just for enemy targeting, as of yet
public class Player : MonoBehaviour
{
    private static Player _instance;

    public static Player Instance { get { return _instance; } }


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
}
