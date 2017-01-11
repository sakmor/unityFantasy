using UnityEngine;
using UnityEngine.UI;

public class changeN : MonoBehaviour
{
    public bool go = false;
    public float targNU, starNU, result, duration = 1.5f;
    private float _targNU, startTime;

    void Start()
    {
        starNU = float.Parse(GetComponent<Text>().text);
        targNU = _targNU = starNU;
    }

    void LateUpdate()
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
            result = Mathf.SmoothStep(starNU, _targNU, t);
            if (_targNU > starNU)
            {
                GetComponent<Text>().color = Color.green;
            }
            else
            {
                GetComponent<Text>().color = Color.red;
            }

            GetComponent<Text>().text = result.ToString("F0");
            if (result == _targNU)
            {
                GetComponent<Text>().color = Color.yellow;
                go = false;
            }
        }
    }
}