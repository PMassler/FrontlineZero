using UnityEngine;

public class RadarCam : MonoBehaviour
{
    Transform player;
    public float height = 100f;

    void Start()
    {
        player = Player.Instance.transform;
    }

    void Update()
    {
        if (player != null)
        {
            transform.position = player.position + Vector3.up * height;
            transform.forward = Vector3.ProjectOnPlane(player.forward, Vector3.up);
        }
    }
}
