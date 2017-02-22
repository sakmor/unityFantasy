using UnityEngine;

public class hitEffect : MonoBehaviour
{
    Renderer rend;
    float startTime;
    public bool isPlay = false;
    public float sec = 0.035f;

    void Start()
    {

    }

    public void playEffect(Transform giver, float scale)
    {
        rend = GetComponent<Renderer>();
        rend.material.mainTextureScale = new Vector2(0.2f, 1);
        rend.material.mainTextureOffset = new Vector2(0.8f, 1);

        if (!isPlay)
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
            this.transform.position += new Vector3(0, 1, 0) + (giver.position - transform.position) * scale;
            rend.material.mainTextureOffset = new Vector2(0.8f, 1);
            startTime = Time.time;
            isPlay = true;
        }
    }

    public void playEffect(Transform giver, float scale, float min, float max, float roateRange)
    {
        rend = GetComponent<Renderer>();
        rend.material.mainTextureScale = new Vector2(0.2f, 1);
        rend.material.mainTextureOffset = new Vector2(0.8f, 1);

        if (!isPlay)
        {
            var n = Random.Range(min, max);
            this.transform.Rotate(new Vector3(1, 1, Random.Range(1f, roateRange)));
            this.transform.localScale = new Vector3(this.transform.localScale.x * n, this.transform.localScale.y * n, this.transform.localScale.z * n);
            this.transform.position += new Vector3(0, 1, 0) + (giver.position - transform.position) * scale;
            rend.material.mainTextureOffset = new Vector2(0.8f, 1);
            startTime = Time.time;
            isPlay = true;
        }
    }

    void Update()
    {
        if (isPlay)
        {
            if (rend.material.mainTextureOffset.x <= 1.8f)
            {
                if (Time.time - startTime > sec)
                {
                    transform.LookAt(GameObject.Find("mainCamera").transform);
                    rend.material.mainTextureOffset += new Vector2(0.2f, 0);
                    startTime = Time.time;
                }

            }
            else
            {
                rend.material.mainTextureOffset = new Vector2(0.8f, 0);
                isPlay = false;
                Destroy(this.gameObject);
            }
        }
    }
}