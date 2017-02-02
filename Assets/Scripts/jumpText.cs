
using UnityEngine;
using myMath;
[RequireComponent(typeof(Rigidbody))]
public class jumpText : MonoBehaviour
{
    public string number;
    float startTime = 0;
    Vector3 startPOS;
    GameObject text2D;
    float liveTime = 1f;
    float alpha = 1;
    // Use this for initialization
    void Start()
    {

        startTime = Time.time;
        this.GetComponent<Rigidbody>().AddForce(new Vector3(0, 3, 0), ForceMode.Impulse);
        text2D = Instantiate(GameObject.Find("jumpText2D"));
        text2D.transform.parent = GameObject.Find("Canvas").transform;
        startPOS = this.transform.position;
        text2D.GetComponent<UnityEngine.UI.Text>().text = number;
        text2D.GetComponent<UnityEngine.UI.Text>().color = new Color(1, 1, 1, 0);

    }

    // Update is called once per frame
    void Update()
    {
        var nametextScreenPos = Camera.main.WorldToScreenPoint(new Vector3(
                    this.transform.position.x,
                    this.transform.position.y,
                    this.transform.position.z));
        text2D.transform.position = nametextScreenPos;
        var dist = Vector3.Distance(startPOS, this.transform.position);
        var lastVelocity = this.GetComponent<Rigidbody>().velocity;

        if (((Time.time - startTime) / liveTime) < 0.3f && alpha < 1) alpha += 0.01f;
        if (((Time.time - startTime) / liveTime) >= 0.5f) alpha -= 0.1f;
        text2D.GetComponent<UnityEngine.UI.Text>().color = new Color(1, 1, 1, alpha);

        if (Time.time - startTime > liveTime)
        {
            Destroy(this.gameObject);
            Destroy(this.text2D);
        }

    }
}
