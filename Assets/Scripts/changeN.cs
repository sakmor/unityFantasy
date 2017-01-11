using UnityEngine;
using UnityEngine.UI;

public class changeN : MonoBehaviour
{
    public bool go = true;
    public float targNU, starNU, result;
    float _targNU;
    float startTime, duration = 0.5f;
    void start()
    {
        starNU = float.Parse(gameObject.GetComponent<Text>().text);
        startTime = Time.time;
        _targNU = targNU;
    }

    void Update()
    {
        if (go)
        {
            if (targNU != _targNU)
            {
                starNU = float.Parse(GetComponent<Text>().text);
                startTime = Time.time;
                _targNU = targNU;
            }
            float t = (Time.time - startTime) / duration;
            result = Mathf.Lerp(starNU, _targNU, t);
            GetComponent<Text>().text = result.ToString("F0");
            if (result >= _targNU)
            {
                go = true;
            }
        }
    }
}