using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(BoxCollider))]

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
     * attackDistance   攻擊距離
     * lastActionTime   上次行動時間
	 *
	 * [系統相關變數]
	 * dectefrequency	偵測頻率
	 * bais				偵測頻率乖離變數
	 *
	 * [生物狀態變數]
	 * startPos			出生位置
	 * targetName		追擊目標的名字
     * targt   追擊目標的資訊
	 * bioAction		生物狀態
	 *
	 * [生物清單相關變數]
	 * WalkSteptweek	生物步伐()
	 * rotateSpeed		生物旋轉速度(未儲存)
	 */
    public float moveSpeedMax;//todo:測試用改成Public
    float lastActionTime, runBackDist, rotateSpeed, moveSpeed, seeMax, catchSpeed, attackCoolDown, bais, dectefrequency;

    public float WalkSteptweek, attackDistance;//todo:attackDistance應該要放在安全的地方
    public Vector3 nametextScreenPos, startPos, Sphere, Sphere2, Sphere3;
    bool runBack;
    GameObject[] collisionCubes = new GameObject[28];
    GameObject nameText, bioCollider, targeLine;
    gameCS maingameCS;

    public string bioAction, nameShort, bioDataPath;
    public Transform target;

    float[] biologyListData = new float[5];

    Animation anim;

    // Use this for initialization
    void Start()
    {
        target = this.transform;
        rotateSpeed = 10;
        runBack = false;

        WalkSteptweek = 40;         //todo:應該記錄在biologyList.json
        moveSpeed = 0.09f;          //todo:應該記錄在c_ai.json
        runBackDist = 10f;           //todo:應該記錄在c_ai.json
        seeMax = 15f;               //todo:應該記錄在c_ai.json
        attackDistance = 2;         //todo:應該記錄在c_ai.json
        catchSpeed = 0.09f;          //todo:應該記錄在c_ai.json

        moveSpeedMax = moveSpeed;
        startPos = this.transform.position;
        bais = Mathf.Floor(UnityEngine.Random.value * 10) - 5; //-5~5


        nameText = Instantiate(GameObject.Find("nameText"));
        nameText.name = this.name + "_nameText";
        nameText.transform.parent = GameObject.Find("4-UI/Canvas").transform;
        nameText.GetComponent<Text>().text = this.name;

        bioAction = "Wait";
        maingameCS = GameObject.Find("mainGame").GetComponent<gameCS>();

        Sphere3 = this.transform.position;

        setTargeLine();
        loadBiologyList();
        setCollisionCubes();
        dynamicCollision();
        loadAnimation();
    }
    // Update is called once per frame
    void Update()
    {
        if (this.name == maingameCS.Player.name)
        {
            //        this._catchPlayer(); todo:玩家也要追擊
            this._movment();
            this._bioStatus();
            this._targeLineUpdate();
        }
        else
        {
            this._catchPlayer(5 + bais);
            this._movment();
            this._bioStatus();
            this._targeLineUpdate();
        }

    }
    void _catchPlayer(float n)
    {
        if (10 * Time.fixedTime % n < 0.1)
        {
            var playerDistance = Vector3.Distance(maingameCS.Player.transform.position, this.transform.position);
            var startPosDis = Vector3.Distance(this.transform.position, startPos);

            if (startPosDis > runBackDist)
            {
                _runback();
            }
            if (playerDistance < seeMax && !runBack)
            {
                if (target != maingameCS.Player.transform)
                {
                    target = maingameCS.Player.transform;
                    maingameCS.logg(this.name + "開始追擊你了");

                }
                if (playerDistance > attackDistance)
                {
                    Sphere3 = maingameCS.Player.transform.position;
                }

                this.moveSpeedMax = catchSpeed;
                nameText.GetComponent<UnityEngine.UI.Text>().color = Color.red;

                if (playerDistance < attackDistance)
                {
                    if (Time.time * 1000 - lastActionTime > attackCoolDown)
                    {
                        lastActionTime = Time.time * 1000;
                        Sphere3 = this.transform.position;
                        nameText.GetComponent<UnityEngine.UI.Text>().color = Color.yellow;
                        bioAction = "Attack";
                        maingameCS.logg(this.name + "攻擊！");
                    }
                }
            }
            else
            {
                if (target != this.transform)
                {
                    target = this.transform;
                    maingameCS.logg(this.name + "放棄追擊你");
                    _runback();
                }
                nameText.GetComponent<UnityEngine.UI.Text>().color = Color.white;
                //        this.Sphere3.transform.position = this.transform.position;
                //        this.Sphere3.transform.position.y = 1;
                //        maingameCS.logg("here");

            }


        }
    }

    void _runback()
    {
        runBack = true;
        Sphere3 = startPos;
    }
    void _bioStatus()
    {
        //對應生物所處狀態，播放對應動作
        if (!anim.IsPlaying("Attack"))
        {
            switch (this.bioAction)
            {
                case "Attack":
                    anim.CrossFade("Attack");
                    break;
                case "Damage":
                    anim.CrossFade("Damage");
                    break;
                case "Walk":
                    anim.CrossFade("Walk");
                    break;
                case "picking":
                    break;
                case "Wait":
                    anim.CrossFade("Wait");
                    runBack = false;
                    Sphere2 = this.transform.position;
                    break;
                case "Jump":
                    //todo:目前沒有使用
                    break;
            }
        }
        if (anim.IsPlaying("Wait"))
        {
            this.bioAction = "Wait";
        }
    }

    void _movment()
    {

        float SphereDistance;

        if (this.bioAction == "Walk")
        {
            this.bioAction = "Wait";
        }

        //如果使用者操作搖桿
        if (maingameCS.clickStart &&
            maingameCS.hitUIObjectName == "movePlate" &&
            maingameCS.Player.name == this.name)
        {

            this.bioAction = "Walk";

            //自搖桿取得的移動向量直
            Sphere.x = maingameCS.mouseDragVector.x * 0.02f;
            Sphere.z = maingameCS.mouseDragVector.z * 0.02f;

            //將spere2依據攝影機位置做座標轉換
            var tempAngel = Vector3.Angle(maingameCS.mainCamera.transform.forward, (Sphere - this.transform.position));
            tempAngel = -maingameCS.mainCamera.transform.eulerAngles.y * Mathf.PI / 180;
            Sphere2.x = Sphere.x * Mathf.Cos(tempAngel) - Sphere.z * Mathf.Sin(tempAngel) + this.transform.position.x;
            Sphere2.z = Sphere.x * Mathf.Sin(tempAngel) + Sphere.z * Mathf.Cos(tempAngel) + this.transform.position.z;
            Sphere2.y = this.transform.position.y;

            SphereDistance = Vector3.Distance(this.transform.position, Sphere2);
        }
        //如果使用者點擊螢幕操作
        else
        {

            SphereDistance = Vector3.Distance(this.transform.position, Sphere3);
            var Sphere2Distance = Vector3.Distance(this.transform.position, Sphere2);
            if (SphereDistance > 0.05f)
            {
                this.bioAction = "Walk";

                //如果Sphere3距離低於1，或是與Sphere3之間沒有阻礙時
                if (SphereDistance < 1 ||
                maingameCS.PathfindingCS.decteBetween(this.transform.position, Sphere3))
                {
                    Sphere2 = Sphere3;
                }
                else
                {
                    if (Sphere2Distance < 1)
                    {
                        // Debug.Log("PathfindingCS");
                        Sphere2 = maingameCS.PathfindingCS.FindPath_Update(this.transform.position, Sphere3);
                        if (Sphere2 == new Vector3(-999, -999, -999))
                        {
                            maingameCS.logg("<b>" + this.name + "</b>: 目前無法移動至該處");
                            Sphere2 = this.transform.position;
                            Sphere3 = this.transform.position;
                        }
                    }

                }
            }
        }

        if (this.bioAction == "Walk")
        {
            //將生物轉向目標
            Sphere2.y = this.transform.position.y;
            Sphere.y = this.transform.position.y;
            Vector3 targetDir = Sphere2 - this.transform.position;
            float step = rotateSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(this.transform.forward, targetDir, step, 0.0f);
            this.transform.rotation = Quaternion.LookRotation(newDir);

            //依照目標距離調整移動速度
            moveSpeed = moveSpeedMax;
            if (maingameCS.hitUIObjectName ==
                "movePlate" && SphereDistance < 5)
            {
                moveSpeed = moveSpeed * (SphereDistance / 5);
            }

            //更新動態碰撞物
            dynamicCollision();

            //移動生物到目標點
            this.transform.position = Vector3.MoveTowards(this.transform.position, Sphere2, moveSpeed * Time.deltaTime * 50);

            //調整步伐
            anim["Walk"].speed = WalkSteptweek * moveSpeed;
        }
    }

    public void updateUI()
    {
        nametextScreenPos = Camera.main.WorldToScreenPoint(new Vector3(
            this.transform.position.x,
            this.transform.position.y + 2.5f,
            this.transform.position.z));
        nameText.transform.position = nametextScreenPos;
    }

    void setCollisionCubes()
    {

        bioCollider = Instantiate(GameObject.Find("GameObject"));
        bioCollider.name = "bioCollider";
        bioCollider.transform.parent = this.transform;
        bioCollider.transform.localScale = new Vector3(1, 1, 1);
        bioCollider.transform.localPosition = new Vector3(0, 0, 0);
        bioCollider.transform.eulerAngles = new Vector3(0, 45, 0);
        bioCollider.AddComponent<BoxCollider>();

        BoxCollider collider = bioCollider.GetComponent<BoxCollider>() as BoxCollider;
        collider.center = new Vector3(0, biologyListData[1], 0);
        collider.size = new Vector3(biologyListData[2], biologyListData[3], biologyListData[2]);
        this.GetComponent<Rigidbody>().freezeRotation = true;

        GameObject collisionCubeOBJ;
        collisionCubeOBJ = new GameObject(this + "_collisionCubeOBJ");
        collisionCubeOBJ.transform.parent = GameObject.Find("Biology/Items").transform;
        for (var i = 0; i <= 27; i++)
        {
            collisionCubes[i] = Instantiate(GameObject.Find("collisionCube"));
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
        Vector3 tempVector3 = maingameCS.normalized(this.transform.position);
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                for (int z = -1; z < 2; z++)
                {
                    g++;
                    Vector3 temp = new Vector3(tempVector3.x + x, tempVector3.y + y, tempVector3.z + z);
                    if (maingameCS.cubesDictionary.ContainsKey(maingameCS.normalized(temp)))
                    {
                        collisionCubes[g].transform.position = new Vector3(temp.x, temp.y, temp.z);
                    }
                }
            }
        }
    }




    void loadAnimation()
    {
        anim = this.GetComponent<Animation>();
        string[] animationsName = new string[] { "Attack", "Damage", "Dead", "Wait", "Walk" }; //todo:應該記錄在biologyList
        foreach (string name in animationsName)
        {
            GameObject mdl = Resources.Load(bioDataPath + "/Animation/" + nameShort + "@" + name) as GameObject;
            Animation anim = this.GetComponent<Animation>();
            AnimationClip aClip = mdl.GetComponent<Animation>().clip;
            anim.AddClip(aClip, name);
        }



    }
    void loadBiologyList()
    {
        string bioName = this.name;
        if (bioName[0] == 'm')  //todo:生物角色有可能不是C開的
        {
            bioDataPath = "Biology";
            nameShort = "" + bioName[0] + bioName[1] + bioName[2] + bioName[3];
        }
        else if (bioName[0] == 'C') //todo:玩家角色有可能不是C開的
        {
            bioDataPath = "char/" + bioName;
            nameShort = bioName;
        }
        //讀取生物清單表
        string[] drawNumbers = maingameCS.biologyList.drawNumber;
        int biologyNumber = Array.FindIndex(drawNumbers, n => n.Contains(nameShort));
        biologyListData[0] = maingameCS.biologyList.biodata[biologyNumber + biologyNumber * biologyListData.Length - biologyNumber];     //walkSteptweek
        biologyListData[1] = maingameCS.biologyList.biodata[biologyNumber + 1 + biologyNumber * biologyListData.Length - biologyNumber]; //center.y
        biologyListData[2] = maingameCS.biologyList.biodata[biologyNumber + 2 + biologyNumber * biologyListData.Length - biologyNumber]; //size.x
        biologyListData[3] = maingameCS.biologyList.biodata[biologyNumber + 3 + biologyNumber * biologyListData.Length - biologyNumber]; //size.y
        biologyListData[4] = maingameCS.biologyList.biodata[biologyNumber + 4 + biologyNumber * biologyListData.Length - biologyNumber]; //size.z

        //調整生物縮放值
        transform.localScale = new Vector3(biologyListData[4], biologyListData[4], biologyListData[4]);
        this.WalkSteptweek = biologyListData[0];
    }
    void setTargeLine()
    {
        targeLine = Instantiate(GameObject.Find("targetLine"));
        targeLine.name = "targetLine";
        targeLine.transform.parent = this.transform;
        targeLine.GetComponent<Bezier>().controlPoints[0] = this.transform;
        targeLine.GetComponent<Bezier>().controlPoints[1] = targeLine.transform.Find("p0");
        targeLine.GetComponent<Bezier>().controlPoints[2] = this.transform;
        targeLine.GetComponent<Bezier>().controlPoints[3] = this.transform;
        targeLine.transform.Find("p0").position = new Vector3(this.transform.position.x, this.transform.position.y + 5.0f, this.transform.position.z);


    }
    void _targeLineUpdate()
    {
        if (target != this.transform)
        {
            targeLine.GetComponent<Bezier>().line2target(target);
        }
        else
        {
            targeLine.GetComponent<Bezier>().closeLine();

        }
    }

}