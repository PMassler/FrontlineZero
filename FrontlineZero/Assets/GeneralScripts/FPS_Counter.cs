using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPS_Counter : MonoBehaviour
{
    public int avgFrameRate;
    public Text display_Text;
    public Text averageFPS_Text;  
    int counter = 0;
    float total = 0;


    private void Start()
    {
        StartCoroutine(AverageFPS());
    }

    public void Update()
    {
        float current = (int)(1f / Time.unscaledDeltaTime);
        avgFrameRate = (int)current;
        counter++;
        total += avgFrameRate;
        display_Text.text = avgFrameRate.ToString() + " FPS";
        averageFPS_Text.text = (int)(total / counter) + " avgFPS";
    }

    IEnumerator AverageFPS()
    {
        yield return new WaitForSeconds(10);
        Debug.Log((int)(total / counter) + " avg FPS");
    }
}