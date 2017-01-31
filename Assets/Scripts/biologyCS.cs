using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using myMath;

[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
// [RequireComponent(typeof(BoxCollider))]

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
    * actionSpeed      行動速度
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
    * bioAnimation		生物狀態
    *
    * [生物清單相關變數]
    * WalkSteptweek	生物步伐()
    * rotateSpeed		生物旋轉速度(未儲存)
    */
    public float LV, ATTACK, HP, DEF, HPMAX;
    float lastHitTime, hitTime, actionSpeed, WalkSteptweek, attackDistance, moveSpeedMax, startPosDis, DAMAGE, hpPlus, aTimes, MP, MPMAX, EXP, lastActionTime, lastDanceTime, runBackDist, rotateSpeed, moveSpeed, seeMax, catchSpeed, attackCoolDown, bais, dectefrequency;
    public int bioType, bioCamp, players;
    public float targetDistance;
    Vector3 nametextScreenPos, startPos, Sphere = new Vector3(0, 0, 0), Sphere2, Sphere3;
    bool runBack;
    internal GameObject[] collisionCubes = new GameObject[28], allBiologys;
    GameObject model, HPBarLine, nameText, targeLine, HID, HPbar;
    gameCS maingameCS;
    BoxCollider bioCollider;
    string bioAnimation, nameShort, bioDataPath, leaderName, bioAction;
    bool isVisible, isEnable = false;
    public Transform target;
    float[] biologyListData = new float[6];
    Animation anim;
    gameBits gameBits;
    Renderer rend;

    // Use this for initialization
    public biologyCS(gameCS parent)
    {
        maingameCS = parent;
    }
    void Start()
    {
        //如果該生物在玩家清單，改變陣營為玩家。
        LV = 50;                    //todo:應該記錄在生物表
        checkBioCamp();
        bioAction = "";
        maingameCS = GameObject.Find("mainGame").GetComponent<gameCS>();

        if (bioCamp == 1) model = this.transform.FindChild("Model").gameObject; //todo:因為美術資源用別人的，只好先寫死
        if (bioCamp == 0) model = this.transform.FindChild("Group Locator/Knight").gameObject; //todo:因為美術資源用別人的，只好先寫死

        allBiologys = maingameCS.getAllBiologys();

        attackCoolDown = 15;
        actionSpeed = 20.2f;          //todo:應該記錄在c_biology.json
        players = 3;
        target = null;
        rotateSpeed = 15;
        runBack = false;

        WalkSteptweek = 40;         //todo:應該記錄在biologyList.json
        moveSpeed = 0.12f;          //todo:應該記錄在c_ai.json
        runBackDist = 10f;           //todo:應該記錄在c_ai.json
        seeMax = 15f;               //todo:應該記錄在c_ai.json
        attackDistance = 2;         //todo:應該記錄在c_ai.json
        catchSpeed = 0.05f;          //todo:應該記錄在c_ai.json

        GameObject.Find("playerINFO/P3/name").GetComponent<Text>().text = this.name;
        GameObject.Find("playerINFO/P3/HPMAX").GetComponent<Text>().text = HPMAX.ToString("F0");
        GameObject.Find("playerINFO/P3/HP").GetComponent<Text>().text = HP.ToString("F0");
        GameObject.Find("playerINFO/P3/MP").GetComponent<Text>().text = MPMAX.ToString("F0");

        moveSpeedMax = moveSpeed;
        startPos = this.transform.position;
        bais = Mathf.Floor(UnityEngine.Random.Range(-4f, 6f)); //-4~6

        setNameText();

        bioAnimation = "mWait";

        Sphere3 = this.transform.position;
        gameBits = new gameBits(this);
        setTargeLine();
        loadBiologyList();
        setCollisionCubes();
        dynamicCollision();
        loadAnimation();
        setEffect();
    }
    public biologyCS()
    {

    }
    // Update is called once per frame
    void Update()
    {

        if (this.name == maingameCS.Player.name)
        {
            // this._catchMonster(1);
            this._movment();
            this._bioAnimation();
            this._bioAction(bioAction);
            this.gameBits.Update();
            this.updateUI();
            this.effect();
        }
        else
        {
            GameObject.Find("Sphere3").transform.position = Sphere3;
            this._movment();
            this._bioAnimation();
            // this.gameBits.Update();
            // GameObject.Find("nodeINFO").transform.position = Sphere3;
            this.updateUI();
            this.effect();
        }

    }
    void setEffect()
    {
        rend = model.GetComponent<Renderer>();
    }

    void effect()
    {

        if (anim.IsPlaying("Attack") && anim["Attack"].time > hitTime)
        {
            if (Time.time - lastHitTime > anim["Attack"].length)
            {
                lastHitTime = Time.time;
                target.GetComponent<biologyCS>().rend.material.SetFloat("_RimPower", 0);
            }


        }

        if (Time.time - lastHitTime < 0.2)
        {
            if (target != null)
            {
                target.GetComponent<biologyCS>().anim.CrossFade("Damage");
                // target.GetComponent<biologyCS>().anim["Wait"].speed = 0.0f;
                anim["Attack"].speed = 0.0f;
            }
        }
        else
        {
            if (target != null)
            {
                target.GetComponent<biologyCS>().anim["Wait"].speed = 1.0f;
                anim["Attack"].speed = 1.0f;
            }
        }

        foreach (var t in rend.materials)
        {
            if (t.GetFloat("_RimPower") < 3)
            {
                t.SetFloat("_RimPower", t.GetFloat("_RimPower") + 0.5f);
            }
        }

    }
    void setNameText()
    {
        nameText = Instantiate(GameObject.Find("nameText"));
        nameText.name = this.name + "_nameText";
        nameText.transform.parent = GameObject.Find("4-UI/Canvas").transform;
        nameText.GetComponent<Text>().text = this.name;
        HPBarLine = nameText.gameObject.transform.FindChild("HPBar/HPBarLine").gameObject;
    }
    public string getBioAction()
    {
        return bioAction;
    }
    internal void setBioAction(string n)
    {
        bioAction = n;
    }
    public Transform getTransform()
    {
        return this.transform;
    }

    public float getHP()
    {
        return HP;
    }
    public float getHPMAX()
    {
        return HPMAX;
    }
    public Transform getTarget()
    {
        return target;
    }
    internal void setTarget(Transform t)
    {
        target = t;
    }
    public float getSeeMax()
    {
        return seeMax;
    }
    public string getLeadername()
    {
        return leaderName;
    }
    public int getBioCamp()
    {
        return bioCamp;
    }
    public void bioGoto(Vector3 v)
    {
        Sphere3 = v;
    }
    public void bioStop()
    {
        Sphere3 = this.transform.position;
    }
    public void takeDAMAGE(float n)
    {
        if (n - DEF > 0)
        {
            HP -= n - DEF;
        }
        // HID.transform.FindChild("HP").gameObject.GetComponent<changeN>().targNU = HP;
        // HID.transform.FindChild("HP").gameObject.GetComponent<changeN>().go = true;
    }

    void setValueByBioType()
    {
        switch (bioType)
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

    bool _bioAction(string n)
    {
        bool actionIsDone = false;
        switch (n)
        {
            case "actionAttack":
                actionIsDone = actionAttack();
                break;
            case "actionRunback":
                actionIsDone = actionRunback();
                break;
            case "DanceTatget":
                //todo:隨機圍繞目標
                setBioAnimation("mWait");
                break;
            case "":
                setBioAnimation("mWait");
                break;
            default:
                Debug.Log("biologyCS:_bioAction()--收到無效指令指令");
                return false;
        }
        //有目標但是尚未時間到
        if (actionIsDone)
        {
            gameBits.resetActionTime();
            setBioAction("DanceTatget");
            return true;
        }
        return false;

    }

    bool actionAttack()
    {
        float targetDist = Vector3.Distance(target.position, this.transform.position);
        if (targetDist > attackDistance)
        {
            bioGoto(target.position);
            return false;
        }
        else
        {
            bioStop();
            setBioAnimation("mAttack");
            target.GetComponent<biologyCS>().takeDAMAGE(ATTACK);
            return true;
        }

    }
    bool actionRunback()
    {
        runBack = true;
        Sphere3 = startPos;
        return true;
    }
    void _bioAnimation()
    {
        //對應生物所處狀態，播放對應動作
        if (!anim.IsPlaying("Attack"))
        {
            switch (this.bioAnimation)
            {
                case "mAttack":
                    anim.CrossFade("Attack");
                    break;
                case "mDamage":
                    anim.CrossFade("Damage");
                    break;
                case "mWalk":
                    anim.CrossFade("Walk");
                    break;
                case "mPicking":
                    break;
                case "mWait":
                    anim.CrossFade("Wait");
                    runBack = false; //todo: 不該寫在這裡
                    Sphere2 = this.transform.position;//todo: 不該寫在這裡
                    break;
                case "mJump":
                    //todo:目前沒有使用
                    break;
            }
        }
        if (anim.IsPlaying("Wait"))
        {
            this.bioAnimation = "mWait";
        }

    }

    void _movment()
    {

        float SphereDistance = 0;

        // if (this.bioAnimation == "Walk")
        // {
        //     this.bioAnimation = "Wait";
        // }

        //如果該生物是被使用者操控的
        if (maingameCS.Player.name == this.name)
        {
            //如果使用者操作搖桿
            if (maingameCS.clickStart &&
                maingameCS.hitUIObjectName == "moveStick")
            {

                this.bioAnimation = "mWalk";

                //自搖桿取得的移動向量直
                Sphere.x = maingameCS.mouseDragVector.x;
                Sphere.z = maingameCS.mouseDragVector.z;

                //轉換搖桿向量至角色位置
                float Angle = MathS.AngleBetweenVector3(Sphere, new Vector3(0, Sphere.y, 0)) - 270;
                float r = Vector3.Distance(Sphere, new Vector3(0, Sphere.y, 0));

                float tempAngel = Vector3.Angle(maingameCS.mainCamera.transform.forward, (Sphere - this.transform.position));
                tempAngel = -maingameCS.mainCamera.transform.eulerAngles.y * Mathf.PI / 180;

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
                    this.bioAnimation = "mWalk";

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
        }

        if (this.bioAnimation == "mWalk")
        {
            //將生物轉向目標
            this.transform.position = new Vector3(this.transform.position.x, 0.5f, this.transform.position.z);
            Sphere2.y = this.transform.position.y;
            Sphere.y = this.transform.position.y;
            if (target == null)
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
                    this.bioAnimation = "mWait";
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

        var n = HP > 0 ? HP / HPMAX * 12 : 0;
        HPBarLine.transform.localScale = new Vector3(n, 1, 1);

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
                    if (maingameCS.checkCubesDictionary(temp))
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
        biologyListData[5] = maingameCS.biologyList.biodata[biologyNumber + 5 + biologyNumber * biologyListData.Length - biologyNumber]; //hitTime
        hitTime = biologyListData[5];                                                                                                             //調整生物縮放值
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
    void setHPbar()
    {

    }

    internal void drawTargetLine()
    {
        targeLine.GetComponent<Bezier>().line2target(target);
    }

    public float getActionSpeed()
    {
        return actionSpeed;
    }
    public float getAttackDistance()
    {
        return attackDistance;
    }
    public Vector3 getDestination()
    {
        return Sphere3;
    }
    public string getBioAnimation()
    {
        return bioAnimation;
    }
    internal bool setBioAnimation(string n)
    {
        if (bioAnimation != n)
        {
            bioAnimation = n;
        }
        return true;
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
    void checkBioCamp()
    {
        maingameCS = GameObject.Find("mainGame").GetComponent<gameCS>();
        bioCamp = (maingameCS.checkPlayerBioCSListByName(this.name)) ? 0 : 1;
        bioType = (bioCamp == 0) ? 0 : 1;
        setValueByBioType();
    }

}