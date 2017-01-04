using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]

/*******
 *
 *
 */


public class biologyCS : MonoBehaviour
{
    /* [AI相關變數]
	 * runBackDist		脫戰距離
	 * seeMax			視線距離
	 * catchSpeed		追擊速度
	 * attackCoolDown	行動冷卻
	 * moveSpeed		現在移動速度
	 * moveSpeedMax		最大移動速度
	 *
	 * [系統相關變數]
	 * dectefrequency	偵測頻率
	 * bais				偵測頻率乖離變數
	 *
	 * [生物狀態變數]
	 * startPos			出生位置
	 * targetName		追擊目標的名字
	 * bioAction		生物狀態
	 *
	 * [生物清單相關變數]
	 * WalkSteptweek	生物步伐()
	 * rotateSpeed		生物旋轉速度(未儲存)
	 */
    float runBackDist, moveSpeedMax, rotateSpeed, moveSpeed, seeMax, catchSpeed, attackCoolDown, bais, dectefrequency;
    public float WalkSteptweek;
    Vector3 startPos, Sphere, Sphere2, Sphere3, nametextScreenPo;
    bool runBack;
    GameObject[] collisionCubes = new GameObject[28];
    GameObject nameText;
    gameCS maingameCS;

    string bioAction;

    // Use this for initialization
    void Start()
    {
        WalkSteptweek = 40;         //todo:應該記錄在biologyList.json
        moveSpeed = 0.09f;          //todo:應該記錄在c_ai.json
        rotateSpeed = 10;
        moveSpeedMax = moveSpeed;
        startPos = this.transform.position;

        bais = Mathf.Floor(Random.value * 10) - 5; //-5~5
        nameText = Instantiate(GameObject.Find("nameText"));
        nameText.name = this.name + "_nameText";
        nameText.transform.parent = GameObject.Find("4-UI/Canvas").transform;
        bioAction = "Wait";
        maingameCS = GameObject.Find("mainGame").GetComponent<gameCS>();
        Sphere3 = this.transform.position;
        setCollisionCubes();
    }
    void setCollisionCubes()
    {
        GameObject collisionCubeOBJ;
        collisionCubeOBJ = new GameObject(this + "_collisionCubeOBJ");
        collisionCubeOBJ.transform.parent = GameObject.Find("Biology/Items").transform;
        for (var i = 0; i <= 27; i++)
        {
            collisionCubes[i] = Instantiate(GameObject.Find("pickPlayer"));
            collisionCubes[i].name = "dynamicCollision_" + i;
            collisionCubes[i].AddComponent<BoxCollider>();
            collisionCubes[i].transform.parent = collisionCubeOBJ.transform;
            collisionCubes[i].GetComponent<Renderer>().enabled = false;
        }
    }

    void dynamicCollision()
    {
        //更新碰撞物狀態
        var g = 0;
        Vector3 tempVector3 = normalized(this.transform.position);
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                for (int z = -1; z < 2; z++)
                {
                    g++;
                    // if (maingameCS.checkArray(Vector3(tempVector3.x + x, tempVector3.y + y, tempVector3.z + z)) != false)
                    // {
                    //     collisionCubes[g].transform.position = new Vector3(tempVector3.x + x, tempVector3.y + y, tempVector3.z + z);
                    // }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    Vector3 normalized(Vector3 pos)
    {
        Vector3 temp;
        //正規化生物座標
        temp.x = Mathf.Floor(pos.x + 0.5f);
        temp.z = Mathf.Floor(pos.z + 0.5f);
        temp.y = Mathf.Floor(pos.y + 0.5f) + 0.5f;
        return temp;
    }
}
