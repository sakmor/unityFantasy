using UnityEngine;
using UnityEngine.UI;

public class FramesPerSecond : MonoBehaviour
{
    public float updateInterval = 0.5f;
    float accum = 0.0f; // FPS accumulated over the interval
    float frames = 0; // Frames drawn over the interval
    float timeleft; // Left time for current interval

    void Start()
    {
        timeleft = updateInterval;
    }

    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            this.GetComponent<Text>().text = "fps:" + (accum / frames).ToString("f5");
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }

}