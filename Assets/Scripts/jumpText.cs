
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class jumpText : MonoBehaviour
{
    public string number;
    public Vector3 direct;
    public Color color;
    float startTime = 0;
    Vector3 startPOS;
    GameObject text2D;
    float liveTime = 1f;
    float alpha = 0;

    // Use this for initialization
    void Start()
    {

        startTime = Time.time;
        this.GetComponent<Rigidbody>().AddForce(direct * 2 + new Vector3(0, 5, 0), ForceMode.Impulse);
        text2D = Instantiate(GameObject.Find("jumpText2D"));
        text2D.transform.SetParent(GameObject.Find("Canvas").transform) ;
        startPOS = this.transform.position;
        text2D.GetComponent<UnityEngine.UI.Text>().text = number;
        text2D.GetComponent<UnityEngine.UI.Text>().color = color;

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

        if (((Time.time - startTime) / liveTime) < 0.3f && alpha < 1) alpha = 0.01f;
        if (((Time.time - startTime) / liveTime) >= 0.5f) alpha = -0.1f;
        text2D.GetComponent<UnityEngine.UI.Text>().color += new Color(0, 0, 0, alpha);

        if (Time.time - startTime > liveTime)
        {
            Destroy(this.gameObject);
            Destroy(this.text2D);
        }

    }
}
