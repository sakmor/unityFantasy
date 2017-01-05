using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
	 * bioAction		生物狀態
	 *
	 * [生物清單相關變數]
	 * WalkSteptweek	生物步伐()
	 * rotateSpeed		生物旋轉速度(未儲存)
	 */
    float lastActionTime, runBackDist, moveSpeedMax, rotateSpeed, moveSpeed, seeMax, catchSpeed, attackCoolDown, bais, dectefrequency;

    public float WalkSteptweek, attackDistance;//todo:attackDistance應該要放在安全的地方
    public Vector3 nametextScreenPos, startPos, Sphere, Sphere2, Sphere3, nametextScreenPo;
    bool runBack;
    GameObject[] collisionCubes = new GameObject[28];
    GameObject nameText;
    gameCS maingameCS;

    public string bioAction, targetName;

    Animation anim;

    // Use this for initialization
    void Start()
    {
        WalkSteptweek = 40;         //todo:應該記錄在biologyList.json
        moveSpeed = 0.09f;          //todo:應該記錄在c_ai.json
        rotateSpeed = 10;
        moveSpeedMax = moveSpeed;
        startPos = this.transform.position;

        bais = Mathf.Floor(UnityEngine.Random.value * 10) - 5; //-5~5
        nameText = Instantiate(GameObject.Find("nameText"));
        nameText.name = this.name + "_nameText";
        nameText.transform.parent = GameObject.Find("4-UI/Canvas").transform;
        bioAction = "Wait";
        maingameCS = GameObject.Find("mainGame").GetComponent<gameCS>();
        Sphere3 = this.transform.position;

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
        }
        else
        {
            this._catchPlayer(5 + bais);
            this._movment();
            this._bioStatus();
        }

    }
    void _catchPlayer(float n)
    {
        if (10 * Time.fixedTime % n < 0.05)
        {
            var playerDistance = Vector3.Distance(maingameCS.Player.transform.position, this.transform.position);
            var startPosDis = Vector3.Distance(this.transform.position, startPos);

            if (startPosDis > runBackDist)
            {
                _runback();
            }
            if (playerDistance < seeMax && !runBack)
            {

                if (targetName != maingameCS.Player.name)
                {
                    targetName = maingameCS.Player.name;
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
                if (targetName != "")
                {
                    targetName = "";
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
                if (SphereDistance < 1)
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
                            maingameCS.logg("目前無法移動至該處");
                            Sphere2 = this.transform.position;
                            Sphere3 = this.transform.position;
                        }
                        // GameObject.Find("Sphere2").transform.position = Sphere2;
                        // GameObject.Find("Sphere2").transform.position.y = 1;
                    }

                }
            }
        }

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

    void buttonDetect()
    {

        if (touchScreen)
        {
            lineDecte();
            //取得按壓的物件名稱
            if (EventSystem.current.IsPointerOverGameObject())
            {
                hitUIObject = EventSystem.current.currentSelectedGameObject;
                if (hitUIObject)
                {
                    hitUIObjectName = hitUIObject.name;
                }
            }
            else if (EventSystem.current.IsPointerOverGameObject(0))
            {
                hitUIObject = EventSystem.current.currentSelectedGameObject;
                if (hitUIObject)
                {
                    hitUIObjectName = hitUIObject.name;
                }
            }

            //取得首次點擊座標
            if (!clickStart)
            {
                clickStart = true;
                mouseStartPOS = myIputPostion;
            }
            //如果點選到了攝影機搖桿
            if (hitUIObjectName == 'cammeraPlate')
            {
                var _sprite = hitUIObject.GetComponent. < UI.Image > ().sprite;
                var _rect = hitUIObject.GetComponent. < RectTransform > ().rect;
                var temp: Vector2;
                var UIObjectRGB: Color;
                var imageScale: Vector2 = hitUIObject.GetComponent. < RectTransform > ().localScale;


                //取得使用者滑鼠點擊處的Alpha值(為了不規則的按鈕)
                temp.x = myIputPostion.x - hitUIObject.transform.position.x + _rect.width * 0.5;
                temp.y = myIputPostion.y - hitUIObject.transform.position.y + _rect.height * 0.5;

                UIObjectRGB = _sprite.texture.GetPixel(Mathf.FloorToInt(temp.x * _sprite.texture.width / (_rect.width * imageScale.x)), Mathf.FloorToInt(temp.y * _sprite.texture.height / (_rect.height * imageScale.y)));

                if (UIObjectRGB.a != 0 && Vector2.Distance(myIputPostion, hitUIObject.transform.position) < _rect.width * 0.5)
                {
                    cammeraPlatein2out = true;
                    cammeraPlateMouse.transform.position = myIputPostion;
                }
                else if (cammeraPlatein2out)
                {
                    //如果拖拉滑鼠盤脫離搖桿盤的範圍，取得圓的交點
                    var a: Vector2 = Vector2(myIputPostion.x, myIputPostion.y);
                    var b: Vector2 = Vector2(hitUIObject.transform.position.x, hitUIObject.transform.position.y);
                    var c: Vector3 = Vector3(hitUIObject.transform.position.x, hitUIObject.transform.position.y, _rect.width * 0.5);
                    var x: Vector2 = getIntersections(a.x, a.y, b.x, b.y, c.x, c.y, c.z);
                    cammeraPlateMouse.transform.position.x = x.x;
                    cammeraPlateMouse.transform.position.y = x.y;
                }

                //控制攝影機--香菇頭左右
                mainCamera.transform.RotateAround(Player.transform.position, Vector3.up, (hitUIObject.transform.position.x - cammeraPlateMouse.transform.position.x) * Time.deltaTime);

                //控制攝影機--香菇頭上下
                var camera2PlayerVector = mainCamera.transform.position - Player.transform.position;
                var tempVector = camera2PlayerVector;
                tempVector.y = 0;
                tempVector = Quaternion.Euler(0, 90, 0) * tempVector;

                //限制攝影機上下移動的角度
                if (hitUIObject.transform.position.y - cammeraPlateMouse.transform.position.y < 0)
                {
                    if (Vector3.Angle(camera2PlayerVector, Vector3.up) >= 10)
                    {
                        mainCamera.transform.RotateAround(Player.transform.position, tempVector, (hitUIObject.transform.position.y - cammeraPlateMouse.transform.position.y) * Time.deltaTime);
                    }
                }
                else
                    if (Vector3.Angle(camera2PlayerVector, Vector3.up) <= 160)
                {
                    mainCamera.transform.RotateAround(Player.transform.position, tempVector, (hitUIObject.transform.position.y - cammeraPlateMouse.transform.position.y) * Time.deltaTime);
                }

                //更新攝影機與目標的相對位置
                cameraRELtarget = mainCamera.transform.position - Player.transform.position;

            }
            //如果點選到了移動搖桿
            if (hitUIObjectName == 'movePlate')
            {

                _sprite = hitUIObject.GetComponent. < UI.Image > ().sprite;
                _rect = hitUIObject.GetComponent. < RectTransform > ().rect;
                imageScale = hitUIObject.GetComponent. < RectTransform > ().localScale;


                //取得使用者滑鼠點擊處的Alpha值(為了不規則的按鈕)
                temp.x = myIputPostion.x - hitUIObject.transform.position.x + _rect.width * 0.5;
                temp.y = myIputPostion.y - hitUIObject.transform.position.y + _rect.height * 0.5;
                UIObjectRGB = _sprite.texture.GetPixel(Mathf.FloorToInt(temp.x * _sprite.texture.width / (_rect.width * imageScale.x)), Mathf.FloorToInt(temp.y * _sprite.texture.height / (_rect.height * imageScale.y)));

                if (UIObjectRGB.a != 0 && Vector2.Distance(myIputPostion, hitUIObject.transform.position) < _rect.width * 0.5)
                {
                    movePlatein2out = true;
                    movePlateMouse.transform.position = myIputPostion;
                }
                else if (movePlatein2out)
                {
                    //如果拖拉滑鼠盤脫離搖桿盤的範圍，取得圓的交點
                    a = Vector2(myIputPostion.x, myIputPostion.y);
                    b = Vector2(hitUIObject.transform.position.x, hitUIObject.transform.position.y);
                    c = Vector3(hitUIObject.transform.position.x, hitUIObject.transform.position.y, _rect.width * 0.5);
                    x = getIntersections(a.x, a.y, b.x, b.y, c.x, c.y, c.z);
                    movePlateMouse.transform.position.x = x.x;
                    movePlateMouse.transform.position.y = x.y;
                }

                //控制生物移動
                if (Vector2.Distance(myIputPostion, hitUIObject.transform.position) > 0)
                {
                    mouseDragVector.x = (myIputPostion.x - mouseStartPOS.x) * 2.5;
                    mouseDragVector.z = (myIputPostion.y - mouseStartPOS.y) * 2.5;
                }
            }
            //如果點選到了CUBE按鈕
            if (hitUIObjectName == 'cubePlate')
            {
                if (cubePlateTimer.transform.localScale.x < 0.95)
                {
                    cubePlateTimer.transform.localScale = Vector3.MoveTowards(cubePlateTimer.transform.localScale, Vector3(1, 1, 1), 0.05);
                }
                else
                {
                    itemBagJS.itemBagON = true;
                }
                _sprite = hitUIObject.GetComponent. < UI.Image > ().sprite;
                _rect = hitUIObject.GetComponent. < RectTransform > ().rect;
                imageScale = hitUIObject.GetComponent. < RectTransform > ().localScale;


                //取得使用者滑鼠點擊處的Alpha值(為了不規則的按鈕)
                temp.x = myIputPostion.x - hitUIObject.transform.position.x + _rect.width * 0.5;
                temp.y = myIputPostion.y - hitUIObject.transform.position.y + _rect.height * 0.5;
                UIObjectRGB = _sprite.texture.GetPixel(Mathf.FloorToInt(temp.x * _sprite.texture.width / (_rect.width * imageScale.x)), Mathf.FloorToInt(temp.y * _sprite.texture.height / (_rect.height * imageScale.y)));

                if (UIObjectRGB.a != 0 && Vector2.Distance(myIputPostion, hitUIObject.transform.position) < _rect.width * 0.5)
                {
                    cubePlatein2out = true;
                    cubePlateMouse.transform.position = myIputPostion;
                    pickTouchSide.transform.position = Vector3(-100, -100, 0.5);
                }
                else if (cubePlatein2out)
                {
                    //如果拖拉滑鼠盤脫離搖桿盤的範圍
                    cubePlateMouse.transform.position = cubePlate.transform.position;
                    cubePlateMouse.GetComponent. < UI.Graphic > ().color.a = 0.55;
                    cubePlateTimer.transform.localScale = Vector3(0, 0, 0);
                }
            }

            //如果點選到了CUBE按鈕
            if (hitUIObjectName == 'cubePlate')
            {
                _sprite = hitUIObject.GetComponent. < UI.Image > ().sprite;
                _rect = hitUIObject.GetComponent. < RectTransform > ().rect;
                imageScale = hitUIObject.GetComponent. < RectTransform > ().localScale;


                //取得使用者滑鼠點擊處的Alpha值(為了不規則的按鈕)
                temp.x = myIputPostion.x - hitUIObject.transform.position.x + _rect.width * 0.5;
                temp.y = myIputPostion.y - hitUIObject.transform.position.y + _rect.height * 0.5;
                UIObjectRGB = _sprite.texture.GetPixel(Mathf.FloorToInt(temp.x * _sprite.texture.width / (_rect.width * imageScale.x)), Mathf.FloorToInt(temp.y * _sprite.texture.height / (_rect.height * imageScale.y)));

                if (UIObjectRGB.a != 0 && Vector2.Distance(myIputPostion, hitUIObject.transform.position) < _rect.width * 0.5)
                {
                    cubePlatein2out = true;
                    cubePlateMouse.transform.position = myIputPostion;
                    pickTouchSide.transform.position = Vector3(-100, -100, 0.5);
                }
                else if (cubePlatein2out)
                {
                    //如果拖拉滑鼠盤脫離搖桿盤的範圍
                    cubePlateMouse.transform.position = cubePlate.transform.position;
                    cubePlateMouse.GetComponent. < UI.Graphic > ().color.a = 0.55;
                    //                itemBag.GetComponent. < UI.RawImage > ().color.a = 0.0;
                }
            }
            //如果點選到了itemBag介面
            if (hitUIObjectName == 'itemBag')
            {
                itemBagJS.drag(mouseStartPOS, myIputPostion);
            }

            //如果點選到了itemBag介面
            if (hitUIObjectName == '')
            {
                if (itemBagJS.itemBagON)
                {
                    itemBagJS.itemBagON = false;
                }
            }
        }
        else
        {

            cubePlateMouse.GetComponent. < UI.Graphic > ().color.a = 1.0;
            cammeraPlateMouse.transform.position = cammeraPlate.transform.position;
            cubePlateMouse.transform.position = cubePlate.transform.position;
            movePlateMouse.transform.position = movePlate.transform.position;
            hitUIObject = null;
            cubePlateTimer.transform.localScale = Vector3(0.0, 0.0, 1);
            //放開滑鼠時...如果前一個按鍵removePlate，則移除
            //放開滑鼠時...如果前一個按鍵cubePlate，則新增
            if (hitUIObjectName != "")
            {
                if (hitUIObjectName == 'removePlate')
                {
                    playerBioJS.bioAction = "Action";
                }
                if (hitUIObjectName == 'cubePlate')
                {
                    playerBioJS.bioAction = "Create";
                }
                if (hitUIObjectName == 'movePlate')
                {
                    playerBioJS.Sphere2 = playerBioJS.transform.position;
                    playerBioJS.Sphere3.transform.position = playerBioJS.transform.position;
                }
                hitUIObjectName = "";
            }
            clickStart = false;
            cammeraPlatein2out = false;
            cubePlatein2out = false;
            movePlatein2out = false;
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
        Vector3 tempVector3 = maingameCS.normalized(this.transform.position);
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                for (int z = -1; z < 2; z++)
                {
                    g++;
                    Vector3 temp = new Vector3(tempVector3.x + x, tempVector3.y + y, tempVector3.z + z);
                    if (GameObject.Find(temp.ToString("F0")))
                    {
                        collisionCubes[g].transform.position = temp;
                    }
                }
            }
        }
    }




    void loadAnimation()
    {
        anim = this.GetComponent<Animation>();
        string bioName = this.name;
        string nameShort = "";
        string bioDataPath = "";
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

        string[] animationsName = new string[] { "Attack", "Damage", "Dead", "Wait", "Walk" }; //todo:應該記錄在biologyList
        foreach (string name in animationsName)
        {
            GameObject mdl = Resources.Load(bioDataPath + "/Animation/" + nameShort + "@" + name) as GameObject;
            Animation anim = this.GetComponent<Animation>();
            AnimationClip aClip = mdl.GetComponent<Animation>().clip;
            anim.AddClip(aClip, name);
        }

        //讀取生物清單表

        float[] biologyListData = new float[5];
        string[] drawNumbers = maingameCS.biologyList.drawNumber;
        int biologyNumber = Array.FindIndex(drawNumbers, n => n.Contains(this.name)) * biologyListData.Length;
        biologyListData[0] = maingameCS.biologyList.biodata[biologyNumber];     //walkSteptweek
        biologyListData[1] = maingameCS.biologyList.biodata[biologyNumber + 1]; //center.y
        biologyListData[2] = maingameCS.biologyList.biodata[biologyNumber + 2]; //size.x
        biologyListData[3] = maingameCS.biologyList.biodata[biologyNumber + 3]; //size.y
        biologyListData[4] = maingameCS.biologyList.biodata[biologyNumber + 4]; //size.z
        biologyListData[5] = maingameCS.biologyList.biodata[biologyNumber + 5]; //scale.xyz

        this.WalkSteptweek = biologyListData[0];
        BoxCollider collider = this.GetComponent<BoxCollider>() as BoxCollider;
        collider.center = new Vector3(0, biologyListData[1], 0);
        collider.size = new Vector3(biologyListData[2], biologyListData[3], biologyListData[4]);
        collider.transform.localScale = new Vector3(biologyListData[5], biologyListData[5], biologyListData[5]);
        this.GetComponent<Rigidbody>().freezeRotation = true;
    }

}
