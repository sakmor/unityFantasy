﻿using System;
using System.Collections;
using System.Collections.Generic;
using myMath;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(DrawCircle))]
// [RequireComponent (typeof (BoxCollider))]

public class biologyCS : MonoBehaviour
{
    public bool walkable;
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
    float LV, ATTACK, HP, DEF, HPMAX;
    float goalPosDist, lastCheckAnim, effectTime, lastHitTime, hitTime, actionSpeed, WalkSteptweek, attackDistance, moveSpeedMax, startPosDis, DAMAGE, hpPlus, aTimes, MP, MPMAX, EXP, lastActionTime, lastDanceTime, runBackDist, rotateSpeed, moveSpeed, seeMax, catchSpeed, attackCoolDown, bais, dectefrequency;
    public int bioType, bioCamp, players, oder;

    Vector3 nametextScreenPos, beforeShakePos, startPos, Sphere, Sphere2, goalPos;
    bool isPlayer = false, runBack;
    internal GameObject[] collisionCubes = new GameObject[28];
    internal List<GameObject> allBiologys;
    GameObject model, HPBarLine, nameText, targeLine, HID, HPbar;
    gameCS maingameCS;
    BoxCollider bioCollider;
    string bioAnimation, nameShort, bioDataPath, leaderNumber, bioAction;
    bool isVisible, effectIsOn, isEnable = false;
    public Transform target;
    float[] biologyListData;
    Animation anim;
    gamBits gamBits;
    Renderer rend;
    List<string> effectList, _playingAnims, justOverAnimList;

    biologyCS targetCS;

    // Use this for initialization

    void Start()
    {
        GameObject[] collisionCubes = new GameObject[28];
        biologyListData = new float[6];
        effectList = new List<string>();
        _playingAnims = new List<string>();
        justOverAnimList = new List<string>();
        bioStop();
        GetComponent<Animation>().playAutomatically = false;
        maingameCS = GameObject.Find("mainGame").GetComponent<gameCS>();
        //如果該生物在玩家清單，改變陣營為玩家。
        LV = 50; //todo:應該記錄在生物表
        checkBioCamp();
        bioAction = "";

        if (bioCamp == 1) model = this.transform.Find("Model").gameObject; //todo:因為美術資源用別人的，只好先寫死
        if (bioCamp == 0) model = this.transform.Find("Group Locator/Knight").gameObject; //todo:因為美術資源用別人的，只好先寫死

        allBiologys = maingameCS.getAllBiologys();

        attackCoolDown = 15;
        actionSpeed = 15.2f; //todo:應該記錄在c_biology.json
        players = 3;
        target = null;
        rotateSpeed = 15;
        runBack = false;

        WalkSteptweek = 40; //todo:應該記錄在biologyList.json
        moveSpeed = 0.12f; //todo:應該記錄在c_ai.json
        runBackDist = 10f; //todo:應該記錄在c_ai.json
        seeMax = 10f; //todo:應該記錄在c_ai.json
        attackDistance = 2; //todo:應該記錄在c_ai.json
        catchSpeed = 0.05f; //todo:應該記錄在c_ai.json

        GameObject.Find("playerINFO/P3/name").GetComponent<Text>().text = this.name;
        GameObject.Find("playerINFO/P3/HPMAX").GetComponent<Text>().text = HPMAX.ToString("F0");
        GameObject.Find("playerINFO/P3/HP").GetComponent<Text>().text = HP.ToString("F0");
        GameObject.Find("playerINFO/P3/MP").GetComponent<Text>().text = MPMAX.ToString("F0");

        moveSpeedMax = moveSpeed;
        startPos = this.transform.position;

        setNameText();

        bioAnimation = "mWait";

        goalPos = this.transform.position;
        gamBits = new gamBits(this);
        setTargeLine();
        loadBiologyList();
        setCollisionCubes();
        dynamicCollision();
        loadAnimation();
        setEffect();
        checkOder();
    }

    // Update is called once per frame
    void Update()
    {
        this.updateGoalDist();
        this._movment();
        this._bioAnimation();
        this._bioAction();
        this.effect();
        this.gamBits.Update();
        this.updateUI();
        this.fellowLeader();

    }
    void checkOder()
    {
        int o = 1;
        for (int i = 0; i < maingameCS.playerBioCSList.Count; i++)
        {
            if (maingameCS.playerBioCSList[i].oder == o)
            {
                o++;
                i = 0;
            }
        }
        oder = o;
    }


    void fellowLeader()
    {

        if (bioCamp == 0 && !isPlayer) //todo:目前這樣的寫法比較耗效能
        {
            float leaderDist = Vector3.Distance(maingameCS.getPlayerPos(), this.transform.position);
            if (leaderDist > 6f)
            {
                Vector3 go = GameObject.Find("postion (" + oder + ")").transform.position;
                bool test = GameObject.Find("aStart").GetComponent<Grid>().NodeFromWorldPoint(go).walkable;
                if (test)
                {
                    bioGoto(go);
                }

            }
            else if (leaderDist < 2.0f)
            {

                // bioStop();
            }
        }
    }

    void keepDistWithFrontBio(float n)
    {
        //以生物的的正前方射出一條線
        Ray ray = new Ray();
        ray.origin = this.transform.position + new Vector3(0, 1, 0);
        ray.direction = Sphere2 - this.transform.position;

        //取得射線擊中的物件
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, n) &&
            rayHit.transform.tag == "Player")
        {

            biologyCS rayHitBioCS = rayHit.transform.gameObject.GetComponent<biologyCS>();

            if (Vector3.Distance(rayHitBioCS.transform.position, this.transform.position) < 1.5f)
            {
                Debug.Log("stopShake");
                bioStop();

            }

        }
    }

    void setEffect()
    {
        rend = model.GetComponent<Renderer>();
    }

    bool effectPlay()
    {
        switch (bioAction)
        {
            case "actionAttack":
                if (hitTimeIsGot("Attack", hitTime))
                {
                    targetCS.setBioAnimation("Damge");
                    targetCS.takeDAMAGE(ATTACK, transform.forward);
                    targetCS.addEffect("playDamage");
                    targetCS.addEffect("white");
                    targetCS.addEffect("shake");
                    targetCS.addEffect("aniPause");
                    addEffect("aniPause");
                    addEffect("hitEffect");
                    return true;
                }
                break;
            case "actionHeal":
                if (hitTimeIsGot("Attack", hitTime))
                {
                    targetCS.takeHeal(ATTACK * 0.5f);
                    targetCS.addEffect("white");
                    targetCS.addEffect("shake");
                    targetCS.addEffect("aniPause");
                    addEffect("aniPause");
                    return true;
                }
                break;
        }

        return false;

    }

    bool effect()
    {
        if (effectList.Count > 0 && !effectIsOn)
        {
            effectIsOn = true;
            effectTime = Time.time;
            beforeShakePos = transform.position;
        }

        if (!effectIsOn) return false;

        if (Time.time - effectTime <= 0.25f)
        {
            foreach (var t in effectList)
            {
                switch (t)
                {
                    case "boomEffect":
                        effectBoomeffect();
                        continue;
                    case "hitEffect":
                        effectHiteffect();
                        continue;
                    case "white":
                        effectWhite();
                        continue;
                    case "shake":
                        if (bioCamp != 0)
                            effectShake();
                        continue;
                    case "aniPause":
                        setBioAniPause();
                        continue;
                    case "playDamage":
                        setBioAnimation("Damage");
                        anim.Play("Damage");
                        continue;
                }
            }
        }
        else
        {
            foreach (var t in effectList)
            {
                switch (t)
                {
                    case "white":
                        setBioNormal();
                        continue;
                    case "shake":
                        //stopShake
                        continue;
                    case "aniPause":
                        setBioAniPlay();
                        continue;
                }
            }
            effectList.Clear();
            effectIsOn = false;

        }
        return true;
    }

    void addEffect(string effectName)
    {
        effectList.Add(effectName);
    }

    bool effectWhite()
    {
        foreach (var t in rend.materials)
        {
            t.SetFloat("_RimPower", 1);
        }
        return true;

    }
    bool setBioNormal()
    {
        foreach (var t in rend.materials)
        {
            t.SetFloat("_RimPower", 3);
        }
        return true;

    }

    bool effectHiteffect()
    {
        // Debug.Log(target.name);
        if (target.transform.Find("hitEffect") == null)
        {
            GameObject hitEffect = Instantiate(GameObject.Find("hitEffect"));
            hitEffect.name = "hitEffect";
            hitEffect.transform.parent = target.transform;
            hitEffect.transform.localPosition = new Vector3(0, 0, 0);
            hitEffect.GetComponent<hitEffect>().playEffect(0.05f, 5, target.transform, targetCS.biologyListData[3], 1, 2.5f, 180);
        }

        return true;
    }

    bool effectBoomeffect()
    {
        // Debug.Log(target.name);
        if (GameObject.Find(this.name + "boomEffect") == null)
        {
            GameObject hitEffect = Instantiate(GameObject.Find("boomEffect"));
            hitEffect.name = this.name + "boomEffect";
            hitEffect.transform.position = this.transform.position + new Vector3(0, 0.8f, 0);
            hitEffect.GetComponent<hitEffect>().playEffect(0.08f, 10);
        }

        return true;
    }


    bool effectShake()
    {
        this.transform.position = beforeShakePos;
        this.transform.position += new Vector3(
            UnityEngine.Random.Range(-0.1f, 0.1f), 0, UnityEngine.Random.Range(-0.1f, 0.1f));
        bioStop();

        return true;
    }
    bool setBioAniPlay()
    {
        foreach (AnimationState state in anim)
        {
            state.speed = 1F;
        }
        return true;

    }
    bool setBioAniPause()
    {
        foreach (AnimationState state in anim)
        {
            state.speed = 0F;
        }
        return true;

    }
    bool hitTimeIsGot(string action, float htime)
    {
        if (anim[action].time > htime)
        {
            return true;
        }
        return false;
    }

    void getjumpText(float n, Vector3 direct, string color)
    {
        GameObject jumpText = Instantiate(GameObject.Find("jumpText"));
        jumpText.AddComponent<jumpText>();

        Color tempColor;
        switch (color)
        {
            case "GREEN":
                tempColor = Color.green;
                break;
            case "WHITE":
                tempColor = Color.white;
                break;
            case "ORANGE":
                tempColor = new Color(1, 0.5f, 0, 1);
                break;
            case "RED":
                tempColor = Color.red;
                break;
            case "YELLOW":
                tempColor = Color.yellow;
                break;
            default:
                tempColor = Color.black;
                break;
        }

        jumpText.GetComponent<jumpText>().color = tempColor;
        jumpText.GetComponent<jumpText>().number = n.ToString("F0");
        jumpText.GetComponent<jumpText>().direct = direct;
        jumpText.transform.position = this.transform.position;
        jumpText.transform.position += new Vector3(0, 1.5f, 0);
    }
    void setNameText()
    {
        nameText = Instantiate(GameObject.Find("nameText"));
        nameText.name = this.name + "_nameText";
        nameText.transform.SetParent(GameObject.Find("4-UI/Canvas").transform);
        nameText.GetComponent<Text>().text = this.name;
        HPBarLine = nameText.gameObject.transform.Find("HPBar/HPBarLine").gameObject;
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
        if (t != null)
        {
            targetCS = t.GetComponent<biologyCS>();
        }

    }
    public float getSeeMax()
    {
        return seeMax;
    }
    public string getleaderNumber()
    {
        return leaderNumber;
    }
    public int getBioCamp()
    {
        return bioCamp;
    }
    public void bioGoto(Vector3 v)
    {
        goalPos = v;

    }
    public void bioStop()
    {
        goalPos = this.transform.position;
        Sphere2 = this.transform.position;
    }
    public void takeHeal(float n)
    {
        var finalHeal = n * UnityEngine.Random.Range(0.8f, 1f);
        if (HP + finalHeal < HPMAX)
        {
            HP += finalHeal;
        }
        else
        {
            HP = HPMAX;
        }
        getjumpText(finalHeal, new Vector3(), "GREEN");
    }
    public void takeDAMAGE(float n, Vector3 direct)
    {
        //是否爆擊判定
        bool gritical = MathS.chancePersent(10);
        //爆擊時傷害加乘
        n = gritical ? n * 1.5f : n;

        //是否穿刺判定
        bool puncture = MathS.chancePersent(10);
        //穿刺時傷害加乘
        float p = puncture ? p = n * 0.1f : 0;

        //爆擊時跳字變色
        //爆擊黃色、穿刺紅色、同時發生橘色
        string color;
        if (gritical && puncture)
        {
            color = "ORANGE";
        }
        else
        if (gritical)
        {
            color = "YELLOW";
        }
        else
        if (puncture)
        {
            color = "RED";
        }
        else
        {
            color = "WHITE";
        }

        var r = UnityEngine.Random.Range(0.8f, 1.2f);
        var finalDamage = (p + n - DEF) * r;
        if (finalDamage > 0)
        {
            HP -= finalDamage;
        }
        getjumpText(finalDamage, direct, color);

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
                HID = GameObject.Find("playerINFO").gameObject.transform.Find("P1").gameObject;
                break;
            case 2:
                HID = GameObject.Find("playerINFO").gameObject.transform.Find("P2").gameObject;
                break;
            case 3:
                HID = GameObject.Find("playerINFO").gameObject.transform.Find("P3").gameObject;
                break;
        }
        NumCalculate();
        HP = HPMAX;
        MP = MPMAX;
    }
    void shakeThisModel()
    {
        if (isPlayer)
        {
            this.transform.position += new Vector3(
                UnityEngine.Random.Range(-0.15f, 0.15f), 0, UnityEngine.Random.Range(-0.15f, 0.15f));
        }
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

    bool _bioAction()
    {
        bool actionIsDone = false;
        switch (bioAction)
        {
            case "actionHeal":
                actionIsDone = actionHeal();
                break;
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

                break;
            default:
                Debug.Log("biologyCS:_bioAction ()--收到無效指令指令");
                return false;
        }
        //已正確執行完畢action時
        if (actionIsDone)
        {
            if (effectPlay())
            {
                gamBits.resetActionTime();
                return true;
            }
        }
        return false;

    }

    bool actionAttack()
    {
        if (targetCS.HP < 0)
            return false;
        float targetDist = Vector3.Distance(target.position, this.transform.position);

        if (targetDist > attackDistance)
        {
            bioGoto(target.position);
            return false;
        }
        else
        {
            bioStop();
            faceTarget(target.transform.position, 100);
            setBioAnimation("mAttack");
            return true;
        }

    }
    bool actionHeal()
    {
        if (targetCS.HP < 0)
            return false;
        float targetDist = Vector3.Distance(target.position, this.transform.position);

        if (targetDist > attackDistance)
        {
            bioGoto(target.position);
            return false;
        }
        else
        {
            bioStop();
            faceTarget(target.transform.position, 100);
            setBioAnimation("mAttack");
            return true;
        }

    }
    bool actionRunback()
    {
        runBack = true;
        goalPos = startPos;
        return true;
    }

    void setDead()
    {
        setBioAction("");
        setBioAnimation("mDead");
    }

    void checkAnimation()
    {
        if (Time.time - lastCheckAnim > 0.005)
        {
            lastCheckAnim = Time.time;
            //將該動畫列入剛播完清單
            justOverAnimList.Clear();
            foreach (AnimationState state in anim)
            {
                //如果之前清單有撥放這個動畫，但現在沒有撥了
                if (!anim.IsPlaying(state.name) &&
                    _playingAnims.Contains(state.name))
                {
                    justOverAnimList.Add(state.name);
                    _playingAnims.Remove(state.name);
                }

                //如果之前清單沒有撥放這個動畫，但現在正在撥了
                if (anim.IsPlaying(state.name) &&
                    !_playingAnims.Contains(state.name))
                {
                    _playingAnims.Add(state.name);
                }

            }
        }
    }
    void _bioAnimation()
    {
        checkAnimation();
        //如果攻擊動作剛播完，回到wait狀態
        if (justOverAnimList.Contains("Attack") &&
            HP > 0)
        {
            this.bioAnimation = "mWait";
        }
        //如果受傷動作剛播完，並血量等於下於0
        if (justOverAnimList.Contains("Damage") &&
            HP <= 0)
        {
            this.bioAnimation = "mDead";

            //如果死亡的是隊長
            if (isPlayer)
                maingameCS.changePlayerRight();

        }
        //如果死亡動作剛播完，撥放爆炸
        if (justOverAnimList.Contains("Dead"))
        {
            this.addEffect("boomEffect");
            this.transform.gameObject.SetActive(false);
            this.bioAnimation = "mHide";
        }

        //對應生物所處狀態，播放對應動作
        switch (this.bioAnimation)
        {
            case "mAttack":
                anim.CrossFade("Attack");
                break;
            case "mDamage":
                anim.CrossFade("Damage");
                break;
            case "mWalk":
                anim.Play("Walk");
                break;
            case "mPicking":
                break;
            case "mDead":
                anim.Play("Dead");
                break;
            case "mWait":
                anim.CrossFade("Wait");
                break;
            case "mHide":
                //todo:目前沒有使用
                break;
        }

        if (anim.IsPlaying("Wait") && HP > 0)
        {
            this.bioAnimation = "mWait";
        }

    }
    void updateGoalDist()
    {
        goalPosDist = Vector3.Distance(this.transform.position, goalPos);
    }


    void _movment()
    {

        if (goalPosDist > 0.5f) setBioAnimation("mWalk");

        //如果使用者操作搖桿
        if (maingameCS.clickStart &&
            isPlayer &&
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
            // float tempAngel = Vector3.Angle (maingameCS.mainCamera.transform.forward, (Sphere - this.transform.position));
            // tempAngel = -maingameCS.mainCamera.transform.eulerAngles.y * Mathf.PI / 180;
            // Sphere2.x = Sphere.x * Mathf.Cos (tempAngel) - Sphere.z * Mathf.Sin (tempAngel) + this.transform.position.x;
            // Sphere2.z = Sphere.x * Mathf.Sin (tempAngel) + Sphere.z * Mathf.Cos (tempAngel) + this.transform.position.z;
            // Sphere2.y = this.transform.position.y;

        }
        //如果使用者點擊螢幕操作
        else
        {

            var Sphere2Distance = Vector3.Distance(this.transform.position, goalPos);

            if (this.bioAnimation == "mWalk")
            {
                //如果goalPos距離低於1，或是與goalPos之間沒有阻礙時
                if (goalPosDist < 1 ||
                    maingameCS.PathfindingCS.decteBetween(this.transform.position, goalPos))
                {
                    Sphere2 = goalPos;
                }
                else
                {
                    if (Sphere2Distance > 1)
                    {
                        // Debug.Log ("PathfindingCS");
                        Sphere2 = maingameCS.PathfindingCS.FindPath_Update(this.transform.position, goalPos);
                        if (Sphere2 == new Vector3(-999, -999, -999))
                        {
                            maingameCS.logg("<b>" + this.name + "</b>: 目前無法移動至該處");
                            Sphere2 = this.transform.position;
                            goalPos = this.transform.position;
                        }
                    }

                }

                //如果有人在我正前方的人是同陣營時
                //我會停下來
                // keepDistWithFrontBio(Sphere2Distance);
            }
        }

        if (this.bioAnimation == "mWalk")
        {
            //將生物轉向目標
            this.transform.position = new Vector3(this.transform.position.x, 0.5f, this.transform.position.z);
            Sphere2.y = this.transform.position.y;
            Sphere.y = this.transform.position.y;

            faceTarget(Sphere2, rotateSpeed);

            //依照目標距離調整移動速度
            moveSpeed = moveSpeedMax;
            if (goalPosDist < 0.5f)
            {
                moveSpeed = 0.01f + moveSpeed * (goalPosDist / 0.5f);
                if (goalPosDist < 0.03f)
                {
                    this.bioAnimation = "mWait";
                    Sphere2 = goalPos;

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
    void faceTarget(Vector3 etarget, float espeed)
    {
        Vector3 targetDir = etarget - this.transform.position;
        float step = espeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(this.transform.forward, targetDir, step, 0.0f);
        this.transform.rotation = Quaternion.LookRotation(newDir);
    }

    public void updateUI()
    {
        isVisible = GetComponent<Renderer>().isVisible;
        if (isVisible && getHP() > 0)
        {
            nametextScreenPos = Camera.main.WorldToScreenPoint(new Vector3(
                this.transform.position.x,
                this.transform.position.y + 2.5f,
                this.transform.position.z));
            nameText.transform.position = nametextScreenPos;
        }

        var n = HP > 0 ? HP / HPMAX * 12 : 0;
        HPBarLine.transform.localScale = new Vector3(n, 1, 1);

        if (getHP() <= 0)
        {
            nameText.SetActive(false);

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
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
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
        if (bioName[0] == 'm') //todo:生物角色有可能不是C開的
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
        biologyListData[0] = maingameCS.biologyList.biodata[biologyNumber + biologyNumber * biologyListData.Length - biologyNumber]; //walkSteptweek
        biologyListData[1] = maingameCS.biologyList.biodata[biologyNumber + 1 + biologyNumber * biologyListData.Length - biologyNumber]; //center.y
        biologyListData[2] = maingameCS.biologyList.biodata[biologyNumber + 2 + biologyNumber * biologyListData.Length - biologyNumber]; //size.x
        biologyListData[3] = maingameCS.biologyList.biodata[biologyNumber + 3 + biologyNumber * biologyListData.Length - biologyNumber]; //size.y
        biologyListData[4] = maingameCS.biologyList.biodata[biologyNumber + 4 + biologyNumber * biologyListData.Length - biologyNumber]; //size.z
        biologyListData[5] = maingameCS.biologyList.biodata[biologyNumber + 5 + biologyNumber * biologyListData.Length - biologyNumber]; //hitTime
        hitTime = biologyListData[5]; //調整生物縮放值
        transform.localScale = new Vector3(biologyListData[4], biologyListData[4], biologyListData[4]);
        GetComponent<DrawCircle>().setScale(biologyListData[2]);
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
        return goalPos;
    }
    public string getBioAnimation()
    {
        return bioAnimation;
    }
    internal void setBioAnimation(string n)
    {
        bioAnimation = n;
    }

    public void setIsPlayer(bool n)
    {
        isPlayer = n;
    }
    public bool getIsPlayer()
    {
        return isPlayer;
    }

    public void setActionCancel()
    {
        gamBits.resetActionTime();

    }
    void OnCollisionEnter(Collision collision)
    {
        //如果該生物是玩家陣營
        if (this.transform.tag == "Player")
        {
            //該生物不會被怪物陣營擋住
            if (collision.gameObject.tag == "biology")
            {
                Physics.IgnoreCollision(collision.collider, bioCollider);
            }
            // if (collision.gameObject.tag == "Player")
            // {
            //     Physics.IgnoreCollision(collision.collider, bioCollider);
            // }
        }
    }
    void checkBioCamp()
    {
        bioCamp = (this.tag == "Player") ? 0 : 1;
        bioType = (bioCamp == 0) ? 0 : 1;
        setValueByBioType();

    }

}