﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using myMath;

[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
// [RequireComponent(typeof(BoxCollider))]

/*******
 *
 *
 */


public class biologyCS : MonoBehaviour
{
    /* [AI相關變數]
     * isEnable         停止運作
	 * runBackDist		脫戰距離
	 * seeMax			視線距離
	 * catchSpeed		追擊速度
	 * attackCoolDown	行動冷卻
	 * moveSpeed		現在移動速度
	 * moveSpeedMax		最大移動速度
     * attackDistance   攻擊距離
     * lastActionTime   上次行動時間
	 *
     * [戰鬥相關變數]
     * bioCamp          生物陣營 0=玩家 1=敵方 2=第三方
     * bioType          生物型態 0=玩家 1=小怪 2=菁英 3=王怪
     * hpPlus           血量值調整
     * aTimes           可被同等級小怪或玩家的素體攻擊幾次 0=玩家30, 1＝小怪10,2=精英25,3=王怪300
     *
     * LV               現在等級
     * EXP              現在經驗值(全部)
     * HP               現在血量        (STR/2) + 50
     * Damage           傷害值
     * HPMAX            血量最大值
     * MP               現在魔法值
     * MPMAX            魔法最大值
     * DEF              防禦值
     * DAMAGE           傷害值(遭受)
     * Attack
     *
     *

     *
	 * [系統相關變數]
	 * dectefrequency	偵測頻率
	 * bais				偵測頻率乖離變數
     * players          使用哪個顯示器  0=怪物不使用 1=p1  2=p2 3=p3;
     *
	 *
	 * [生物狀態變數]
	 * startPos			出生位置
	 * targetName		追擊目標的名字
     * targt            追擊目標的資訊
	 * bioAction		生物狀態
	 *
	 * [生物清單相關變數]
	 * WalkSteptweek	生物步伐()
	 * rotateSpeed		生物旋轉速度(未儲存)
	 */
    public float moveSpeedMax;//todo:測試用改成Public
    float startPosDis, targetDistance, ATTACK, DAMAGE, hpPlus, aTimes, HP, HPMAX, MP, DEF, MPMAX, LV, EXP, lastActionTime, lastDanceTime, runBackDist, rotateSpeed, moveSpeed, seeMax, catchSpeed, attackCoolDown, bais, dectefrequency;
    public int bioType = 1, bioCamp = 1, players;
    public float WalkSteptweek, attackDistance;//todo:attackDistance應該要放在安全的地方
    public Vector3 nametextScreenPos, startPos, Sphere = new Vector3(0, 0, 0), Sphere2, Sphere3;
    bool runBack;
    GameObject[] collisionCubes = new GameObject[28];
    List<GameObject> battleBios;

    GameObject nameText, targeLine, HID;
    gameCS maingameCS;
    BoxCollider bioCollider;
    public string bioAction, nameShort, bioDataPath;

    public bool isVisible, isEnable = false;
    public Transform target;

    float[] biologyListData = new float[5];

    Animation anim;

    // Use this for initialization
    void Start()
    {
        attackCoolDown = 15;
        players = 3;
        target = this.transform;
        rotateSpeed = 15;
        runBack = false;

        WalkSteptweek = 40;         //todo:應該記錄在biologyList.json
        moveSpeed = 0.12f;          //todo:應該記錄在c_ai.json
        runBackDist = 10f;           //todo:應該記錄在c_ai.json
        seeMax = 15f;               //todo:應該記錄在c_ai.json
        attackDistance = 2;         //todo:應該記錄在c_ai.json
        catchSpeed = 0.05f;          //todo:應該記錄在c_ai.json
        LV = 50;
        bioTypeSet(bioType);

        GameObject.Find("playerINFO/P3/name").GetComponent<Text>().text = this.name;
        GameObject.Find("playerINFO/P3/HPMAX").GetComponent<Text>().text = HPMAX.ToString("F0");
        GameObject.Find("playerINFO/P3/HP").GetComponent<Text>().text = HP.ToString("F0");
        GameObject.Find("playerINFO/P3/MP").GetComponent<Text>().text = MPMAX.ToString("F0");

        moveSpeedMax = moveSpeed;
        startPos = this.transform.position;
        bais = Mathf.Floor(UnityEngine.Random.Range(-4f, 6f)); //-4~6


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
            // this._catchMonster(1);
            this._movment();
            this._bioStatus();
            this._targeLineUpdate();
        }
        else
        {
            GameObject.Find("Sphere3").transform.position = Sphere3; this._catchPlayer(5 + bais);
            this._movment();
            this._bioStatus();
            this._targeLineUpdate();
            // GameObject.Find("nodeINFO").transform.position = Sphere3;
        }

    }
    public void giveDAMAGE(float n)
    {
        if (n - DEF > 0)
        {
            HP -= n - DEF;
        }
        HID.transform.FindChild("HP").gameObject.GetComponent<changeN>().targNU = HP;
        HID.transform.FindChild("HP").gameObject.GetComponent<changeN>().go = true;
    }

    public void bioTypeSet(int Type)
    {
        bioType = Type;
        switch (Type)
        {
            case 0:
                hpPlus = 2;
                aTimes = 35;
                break;
            case 1:
                hpPlus = 1;
                aTimes = 10;
                break;
            case 2:
                hpPlus = 3;
                aTimes = 25;
                break;
            case 3:
                hpPlus = 5;
                aTimes = 300;
                break;
        }
        switch (players)
        {
            case 0:
                break;
            case 1:
                HID = GameObject.Find("playerINFO").gameObject.transform.FindChild("P1").gameObject;
                break;
            case 2:
                HID = GameObject.Find("playerINFO").gameObject.transform.FindChild("P2").gameObject;
                break;
            case 3:
                HID = GameObject.Find("playerINFO").gameObject.transform.FindChild("P3").gameObject;
                break;
        }
        NumCalculate();
        HP = HPMAX;
        MP = MPMAX;
    }
    void NumCalculate()
    {
        HPMAX = (50 + Mathf.Pow(LV * 5, 1.5f)) * hpPlus;
        MPMAX = HPMAX * 0.05f;

        //如果該生物是玩家(bioType=0)，對方視為小怪，血量用1來計算
        //反之，該生物是怪物(不管精英還是王怪)，對方視為玩家，血量用2來計算
        float temp = (bioType == 0) ? 1 : 2;
        ATTACK = (50 + Mathf.Pow(LV * 5, 1.5f) * temp) * 0.5f;

        //防禦力=敵方攻擊-(我的血量/預期次數)
        //
        temp = (bioType == 0) ? 2 : 1;
        float eATTACK = 50 + Mathf.Pow(LV * 5, 1.5f) * temp * 0.5f;
        DEF = eATTACK - (HPMAX / aTimes);

    }
    void _catchMonster(float n)
    {
        if (10 * Time.fixedTime % n < 1)
        {

        }
    }
    void _randomGo()
    {

    }

    void _attack()
    {
        if (targetDistance < attackDistance)
        {

        }
        else
        {

        }
    }
    List<GameObject> getSeeMaxBio()
    {
        GameObject[] tempALL = GameObject.FindGameObjectsWithTag("biology");
        List<GameObject> tempNew = new List<GameObject>();
        foreach (var t in tempALL)
        {
            if (Vector3.Distance(this.transform.position, t.transform.position) < seeMax)
            {
                tempNew.Add(t);
            }
        }
        return tempNew;

    }

    void _AI(float n)
    {
        if (10 * Time.fixedTime % n < 1)
        {
            //更新戰場情報(可視範圍內的敵方我)
            battleBios = getSeeMaxBio();

            //如果距離出身點過遠，則脫戰
            startPosDis = Vector3.Distance(this.transform.position, startPos);
            if (startPosDis > runBackDist)
                _runback();

            //gameBits--1
            if (target2ClosestEnemy())
            {

            }
            //action--1
            {


            }



            //如果所有指令皆不達成，則閒晃
            _randomGo();
        }
    }
    void _catchPlayer(float n)
    {
        if (10 * Time.fixedTime % n < 1)
        {
            var playerDistance = Vector3.Distance(maingameCS.Player.transform.position, this.transform.position);
            var startPosDis = Vector3.Distance(this.transform.position, startPos);


            //如果距離出身點過遠則脫戰
            if (startPosDis > runBackDist) _runback();

            //如果玩家在可視範圍內
            if (playerDistance < seeMax && !runBack)
            {
                //如果玩家在可視範圍內，但在攻擊範圍外
                if (playerDistance > attackDistance)
                {
                    //如果玩家在可視範圍內，但在攻擊範圍外，且尚未鎖定玩家
                    if (target != maingameCS.Player.transform)
                    {
                        target = maingameCS.Player.transform;
                        maingameCS.logg(this.name + "開始追擊你了");
                        Sphere3 = maingameCS.Player.transform.position;
                        this.moveSpeedMax = catchSpeed;
                        nameText.GetComponent<UnityEngine.UI.Text>().color = Color.red;
                    }
                    //如果玩家在可視範圍內，但在攻擊範圍外，且鎖定玩家
                    else
                    {
                        nameText.GetComponent<UnityEngine.UI.Text>().color = Color.red;
                        Sphere3 = maingameCS.Player.transform.position;
                    }

                }
                //如果玩家在可視範圍內，並且在攻擊範圍內
                else
                {
                    //如果玩家在可視範圍內，並且在攻擊範圍內，且攻擊時間到了
                    if (Time.time - lastActionTime > attackCoolDown)
                    {
                        lastActionTime = Time.time;

                        nameText.GetComponent<UnityEngine.UI.Text>().color = Color.yellow;
                        bioAction = "Attack";
                        maingameCS.logg(this.name + "攻擊！");
                        target.gameObject.GetComponent<biologyCS>().giveDAMAGE(ATTACK);
                    }
                    //如果玩家在可視範圍內，並且在攻擊範圍內，但攻擊尚未到
                    else
                    {
                        if (Time.time - lastDanceTime > 3)
                        {
                            lastDanceTime = Time.time;
                            Sphere3 = MathS.getCirclePath(this.transform.position, target.position, UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(1, attackDistance));
                        }
                        else
                        {

                        }
                    }
                }
            }
            //如果玩家在可視範圍內之外
            else if (target != this.transform)
            {
                target = this.transform;
                maingameCS.logg(this.name + "放棄追擊你");
                _runback();
                nameText.GetComponent<UnityEngine.UI.Text>().color = Color.white;
            }
            //脫戰判斷
            if (startPosDis > runBackDist) { _runback(); }


            //追擊判斷 //判斷是否為首次追擊
            if (playerDistance < seeMax && playerDistance > attackDistance && !runBack && target != maingameCS.Player.transform)
            {
                maingameCS.logg(this.name + "開始追擊你了");
                nameText.GetComponent<UnityEngine.UI.Text>().color = Color.red;
                Sphere3 = maingameCS.Player.transform.position;
                target = maingameCS.Player.transform;
                this.moveSpeedMax = catchSpeed;
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

        // if (this.bioAction == "Walk")
        // {
        //     this.bioAction = "Wait";
        // }

        //如果使用者操作搖桿
        if (maingameCS.clickStart &&
            maingameCS.hitUIObjectName == "moveStick" &&
            maingameCS.Player.name == this.name)
        {

            this.bioAction = "Walk";

            //自搖桿取得的移動向量直
            Sphere.x = maingameCS.mouseDragVector.x;
            Sphere.z = maingameCS.mouseDragVector.z;

            //轉換搖桿向量至角色位置
            float Angle = MathS.AngleBetweenVector3(Sphere, new Vector3(0, Sphere.y, 0)) - 270 + maingameCS.temp;
            float r = Vector3.Distance(Sphere, new Vector3(0, Sphere.y, 0));

            float tempAngel = Vector3.Angle(maingameCS.mainCamera.transform.forward, (Sphere - this.transform.position));
            tempAngel = -maingameCS.mainCamera.transform.eulerAngles.y * Mathf.PI / 180;
            Debug.Log(tempAngel * Mathf.Rad2Deg);

            Vector3 temp = MathS.getCirclePath(this.transform.position, this.transform.position, Angle + tempAngel * Mathf.Rad2Deg, r);
            Sphere2.x = temp.x;
            Sphere2.z = temp.z;
            Sphere2.y = this.transform.position.y;

            // //將spere2依據攝影機位置做座標轉換
            // float tempAngel = Vector3.Angle(maingameCS.mainCamera.transform.forward, (Sphere - this.transform.position));
            // tempAngel = -maingameCS.mainCamera.transform.eulerAngles.y * Mathf.PI / 180;
            // Sphere2.x = Sphere.x * Mathf.Cos(tempAngel) - Sphere.z * Mathf.Sin(tempAngel) + this.transform.position.x;
            // Sphere2.z = Sphere.x * Mathf.Sin(tempAngel) + Sphere.z * Mathf.Cos(tempAngel) + this.transform.position.z;
            // Sphere2.y = this.transform.position.y;

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
            this.transform.position = new Vector3(this.transform.position.x, 0.5f, this.transform.position.z);
            Sphere2.y = this.transform.position.y;
            Sphere.y = this.transform.position.y;
            if (target == this.transform)
            {
                Vector3 targetDir = Sphere2 - this.transform.position;
                float step = rotateSpeed * Time.deltaTime;
                Vector3 newDir = Vector3.RotateTowards(this.transform.forward, targetDir, step, 0.0f);
                this.transform.rotation = Quaternion.LookRotation(newDir);
            }
            else
            {
                this.transform.LookAt(new Vector3(target.transform.position.x, this.transform.position.y, target.transform.position.z));
            }

            //依照目標距離調整移動速度
            moveSpeed = moveSpeedMax;
            if (SphereDistance < 0.5f)
            {
                moveSpeed = 0.01f + moveSpeed * (SphereDistance / 0.5f);
                if (SphereDistance < 0.03f)
                {
                    this.bioAction = "Wait";
                    Sphere2 = Sphere3;

                }
            }

            //更新動態碰撞物
            dynamicCollision();

            //移動生物到目標點
            this.transform.position = Vector3.MoveTowards(this.transform.position, Sphere2, moveSpeed * Time.deltaTime * 50);

            //調整步伐
            anim["Walk"].speed = 1f + WalkSteptweek * moveSpeed;
        }
    }

    public void updateUI()
    {
        isVisible = GetComponent<Renderer>().isVisible;
        if (isVisible)
        {
            nametextScreenPos = Camera.main.WorldToScreenPoint(new Vector3(
                this.transform.position.x,
                this.transform.position.y + 2.5f,
                this.transform.position.z));
            nameText.transform.position = nametextScreenPos;
        }


    }

    void setCollisionCubes()
    {

        GameObject temp = Instantiate(GameObject.Find("GameObject"));
        temp.name = "bioCollider";
        temp.transform.parent = this.transform;
        temp.transform.localScale = new Vector3(1, 1, 1);
        temp.transform.localPosition = new Vector3(0, 0, 0);
        temp.transform.eulerAngles = new Vector3(0, 45, 0);
        temp.AddComponent<BoxCollider>();
        bioCollider = temp.GetComponent<BoxCollider>();

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
    void OnCollisionEnter(Collision collision)
    {
        if (this.bioType == 0)
        {
            if (collision.gameObject.tag == "biology")
            {
                Physics.IgnoreCollision(collision.collider, bioCollider);
            }
        }

    }

    //回傳隨便一個敵方
    bool target2AnyEnemy()
    {
        foreach (var t in battleBios)
        {
            if (bioCamp != t.GetComponent<biologyCS>().bioCamp)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }

    //回傳離我最近敵方
    bool target2ClosestEnemy()
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (var t in battleBios)
        {
            if (bioCamp != t.GetComponent<biologyCS>().bioCamp)
            {
                float dist = Vector3.Distance(t.transform.position, currentPos);
                if (dist < minDist)
                {
                    tMin = t.transform;
                    minDist = dist;
                }
                target = tMin;
                return true;
            }
        }
        return false;
    }
    //回傳離我最遠敵方
    bool target2FurthestEnemy()
    {
        Transform tMax = null;
        float maxDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (var t in battleBios)
        {
            if (bioCamp != t.GetComponent<biologyCS>().bioCamp)
            {
                float dist = Vector3.Distance(t.transform.position, currentPos);
                if (dist > maxDist)
                {
                    tMax = t.transform;
                    maxDist = dist;
                }
                target = tMax;
                return true;
            }
        }
        return false;
    }
    //回傳敵方中血量最高者
    bool target2HighestHPEnemy()
    {
        float highestHP = 0;
        Transform tMax = null;
        foreach (var t in battleBios)
        {
            if (bioCamp != t.GetComponent<biologyCS>().bioCamp)
            {
                float tempHP = t.GetComponent<biologyCS>().HP;
                if (tempHP > highestHP)
                {
                    tMax = t.transform;
                    highestHP = tempHP;
                }
                return tMax;
            }
        }
        return false;
    }
    //回傳敵方中血量最低者
    bool target2LowestHPEnemy()
    {
        float lowestHP = 9999999999;
        Transform tMin = null;
        foreach (var t in battleBios)
        {
            if (bioCamp != t.GetComponent<biologyCS>().bioCamp)
            {
                float tempHP = t.GetComponent<biologyCS>().HP;
                if (tempHP < lowestHP)
                {
                    tMin = t.transform;
                    lowestHP = tempHP;
                }
                target = tMin;
                return true;
            }
        }
        return false;
    }
    //回傳敵方中血量上限最高者
    bool target2HighestHPMaxEnemy()
    {
        float highestHP = 0;
        Transform tMax = null;
        foreach (var t in battleBios)
        {
            if (bioCamp != t.GetComponent<biologyCS>().bioCamp)
            {
                float tempHP = t.GetComponent<biologyCS>().HPMAX;
                if (tempHP > highestHP)
                {
                    tMax = t.transform;
                    highestHP = tempHP;
                }
                target = tMax;
                return true;
            }
        }
        return false;
    }
    //回傳敵方中血量上限最低者
    bool target2LowestHPMaxEnemy()
    {
        float lowestHP = 9999999999;
        Transform tMin = null;
        foreach (var t in battleBios)
        {
            if (bioCamp != t.GetComponent<biologyCS>().bioCamp)
            {
                float tempHP = t.GetComponent<biologyCS>().HPMAX;
                if (tempHP < lowestHP)
                {
                    tMin = t.transform;
                    lowestHP = tempHP;
                }
                return tMin;
            }
        }
        return false;
    }
    //回傳敵方等級最低者
    bool target2LowestLevelEnemy()
    {
        float lowestLevel = 9999999999;
        Transform tMin = null;
        foreach (var t in battleBios)
        {
            if (bioCamp != t.GetComponent<biologyCS>().bioCamp)
            {
                float tempHP = t.GetComponent<biologyCS>().HP;
                if (tempHP < lowestLevel)
                {
                    tMin = t.transform;
                    lowestLevel = tempHP;
                }
                target = tMin;
                return true;
            }
        }
        return false;
    }
    //回傳敵方等級最高者
    bool target2HighestLevelEnemy()
    {
        float highestLevel = 0;
        Transform tMax = null;
        foreach (var t in battleBios)
        {
            if (bioCamp != t.GetComponent<biologyCS>().bioCamp)
            {
                float tempHP = t.GetComponent<biologyCS>().HP;
                if (tempHP > highestLevel)
                {
                    tMax = t.transform;
                    highestLevel = tempHP;
                }
                target = tMax;
                return true;
            }
        }
        return false;
    }
    //回傳隊長的目標(玩家專用)
    bool target2LeaderTarget()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().target.name == maingameCS.playerBioCS[0].name)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    //回傳以我為目標的目標
    bool target2TargetSelfEnemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().target.name == this.name)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }

    //回傳以隊友為目標的目標
    bool target2TargetAllyEnemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().target.name == this.name)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    //回傳殘存血量高於90%目標
    bool target2HpOver90percentEnemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP / t.GetComponent<biologyCS>().MPMAX >= 0.9)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    //回傳殘存血量高於70%目標
    bool target2HpOver70percentEnemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP / t.GetComponent<biologyCS>().MPMAX >= 0.7)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    //回傳殘存血量高於50%目標
    bool target2HpOver50percentEnemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP / t.GetComponent<biologyCS>().MPMAX >= 0.5)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    //回傳殘存血量高於30%目標
    bool target2HpOver30percentEnemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP / t.GetComponent<biologyCS>().MPMAX >= 0.3)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    //回傳殘存血量高於10%目標
    bool target2HpOver10percentEnemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP / t.GetComponent<biologyCS>().MPMAX >= 0.1)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    //回傳殘存血量低於90%目標
    bool target2HpUnder90percentEnemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP / t.GetComponent<biologyCS>().MPMAX <= 0.9)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    //回傳殘存血量低於70%目標
    bool target2HpUnder70percentEnemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP / t.GetComponent<biologyCS>().MPMAX <= 0.7)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    //回傳殘存血量高於50%目標
    bool target2HpUnder50percentEnemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP / t.GetComponent<biologyCS>().MPMAX <= 0.5)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    //回傳殘存血量高於30%目標
    bool target2HpUnder30percentEnemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP / t.GetComponent<biologyCS>().MPMAX <= 0.3)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    //回傳殘存血量高於10%目標
    bool target2HpUnder10percentEnemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP / t.GetComponent<biologyCS>().MPMAX <= 0.1)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    //回傳血量高於100000目標
    bool target2HpOver100000Enemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP >= 100000)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    bool target2HpOver50000Enemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP >= 50000)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    bool target2HpOver10000Enemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP >= 10000)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    bool target2HpOver5000Enemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP >= 5000)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }

    bool target2HpOver3000Enemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP >= 3000)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    bool target2HpOver2000Enemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP >= 2000)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    bool target2HpOver1000Enemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP >= 1000)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    bool target2HpOver500Enemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP >= 500)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    bool target2HpOver100Enemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP >= 500)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    //回傳血量低於100000目標
    bool target2HpUnder100000Enemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP <= 100000)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    bool target2HpUnder50000Enemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP <= 50000)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    bool target2HpUnder10000Enemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP <= 10000)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    bool target2HpUnder5000Enemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP <= 5000)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }

    bool target2HpUnder3000Enemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP <= 3000)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    bool target2HpUnder2000Enemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP <= 2000)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    bool target2HpUnder1000Enemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP <= 1000)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    bool target2HpUnder500Enemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP <= 500)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    bool target2HpUnder100Enemy()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP <= 500)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }



    //------------------
    //回傳隨便一個我方
    bool target2AnyAlly()
    {
        foreach (var t in battleBios)
        {
            if (bioCamp == t.GetComponent<biologyCS>().bioCamp)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }

    //回傳我方中血量最低者
    bool target2LowestHPAlly()
    {
        float lowestHP = 9999999999;
        Transform tMin = null;
        foreach (var t in battleBios)
        {
            if (bioCamp == t.GetComponent<biologyCS>().bioCamp)
            {
                float tempHP = t.GetComponent<biologyCS>().HP;
                if (tempHP < lowestHP)
                {
                    tMin = t.transform;
                    lowestHP = tempHP;
                }
                target = tMin;
                return true;
            }
        }
        return false;
    }


    //回傳殘存血量低於90%目標
    bool target2HpUnder90percentAlly()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP / t.GetComponent<biologyCS>().MPMAX <= 0.9)
            {
                target = t.transform;
                return true;
                return true;
            }
        }
        return false;
    }
    //回傳殘存血量低於70%目標
    bool target2HpUnder70percentAlly()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP / t.GetComponent<biologyCS>().MPMAX <= 0.7)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    //回傳殘存血量高於50%目標
    bool target2HpUnder50percentAlly()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP / t.GetComponent<biologyCS>().MPMAX <= 0.5)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    //回傳殘存血量高於30%目標
    bool target2HpUnder30percentAlly()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP / t.GetComponent<biologyCS>().MPMAX <= 0.3)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    //回傳殘存血量高於10%目標
    bool target2HpUnder10percentAlly()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP / t.GetComponent<biologyCS>().MPMAX <= 0.1)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    //回傳戰鬥不能的我方
    bool target2HpUnder0percentAlly()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().HP / t.GetComponent<biologyCS>().MPMAX <= 0.0)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }


}