using UnityEngine;

public class hitEffect : MonoBehaviour
{
    Renderer rend;
    float startTime;
    public bool isPlay = false;
    public float sec = 0.035f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.mainTextureScale = new Vector2(0.2f, 1);
        rend.material.mainTextureOffset = new Vector2(0.8f, 1);
    }

    public void playEffect()
    {

        if (!isPlay)
        {
            var n = Random.Range(50f, 250f);
            this.transform.Rotate(new Vector3(1, 1, Random.Range(1f, 180f)));
            this.transform.localScale = new Vector3(n, n, n);
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
            }
        }
    }
}