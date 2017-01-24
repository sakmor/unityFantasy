using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
public class gameBits
{
    public bool actionNoRun = true;
    Transform target;
    List<GameObject> battleBios;
    List<string> decideList;
    List<string> actionList;

    int bioCamp, gameBitsNO;
    string leaderName, name;
    Transform Transform;
    float seeMax;

    public gameBits(biologyCS thisBio)
    {
        target = thisBio.target;
        battleBios = thisBio.battleBios;
        bioCamp = thisBio.bioCamp;
        leaderName = thisBio.leaderName;
        name = thisBio.name;
        seeMax = thisBio.seeMax;
        Transform = thisBio.transform;

        decideList.Add("decideHpUnder50percentEnemy");
        actionList.Add("actionAttack");
        decideList.Add("decideClosestEnemy");
        actionList.Add("actionAttack");


    }

    public void Update()
    {
        getSeeMaxBio();
        getDecide();

    }

    bool getDecide()
    {
        bool decideIS = false;
        if (actionNoRun && battleBios.Count > 0)
        {
            for (var i = 0; i < 6; i++)
            {
                decideIS = str2Function(decideList[i]);
                if (decideIS && str2Function(actionList[i]))
                {
                    actionNoRun = false;
                    return true;
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }


    bool str2Function(string t)
    {
        switch (t)
        {
            case "decideAnyEnemy": return decideAnyEnemy();
            case "decideClosestEnemy": return decideClosestEnemy();
            case "decideFurthestEnemy": return decideFurthestEnemy();
            case "decideHighestHPEnemy": return decideHighestHPEnemy();
            case "decideLowestHPEnemy": return decideLowestHPEnemy();
            case "decideHighestHPMaxEnemy": return decideHighestHPMaxEnemy();
            case "decideLowestHPMaxEnemy": return decideLowestHPMaxEnemy();
            case "decideLowestLevelEnemy": return decideLowestLevelEnemy();
            case "decideHighestLevelEnemy": return decideHighestLevelEnemy();
            case "decideLeaderTarget": return decideLeaderTarget();
            case "decideTargetSelfEnemy": return decideTargetSelfEnemy();
            case "decideTargetAllyEnemy": return decideTargetAllyEnemy();
            case "decideHpOver90percentEnemy": return decideHpOver90percentEnemy();
            case "decideHpOver70percentEnemy": return decideHpOver70percentEnemy();
            case "decideHpOver50percentEnemy": return decideHpOver50percentEnemy();
            case "decideHpOver30percentEnemy": return decideHpOver30percentEnemy();
            case "decideHpOver10percentEnemy": return decideHpOver10percentEnemy();
            case "decideHpUnder90percentEnemy": return decideHpUnder90percentEnemy();
            case "decideHpUnder70percentEnemy": return decideHpUnder70percentEnemy();
            case "decideHpUnder50percentEnemy": return decideHpUnder50percentEnemy();
            case "decideHpUnder30percentEnemy": return decideHpUnder30percentEnemy();
            case "decideHpUnder10percentEnemy": return decideHpUnder10percentEnemy();
            case "decideHpOver100000Enemy": return decideHpOver100000Enemy();
            case "decideHpOver50000Enemy": return decideHpOver50000Enemy();
            case "decideHpOver10000Enemy": return decideHpOver10000Enemy();
            case "decideHpOver5000Enemy": return decideHpOver5000Enemy();
            case "decideHpOver3000Enemy": return decideHpOver3000Enemy();
            case "decideHpOver2000Enemy": return decideHpOver2000Enemy();
            case "decideHpOver1000Enemy": return decideHpOver1000Enemy();
            case "decideHpOver500Enemy": return decideHpOver500Enemy();
            case "decideHpOver100Enemy": return decideHpOver100Enemy();
            case "decideHpUnder100000Enemy": return decideHpUnder100000Enemy();
            case "decideHpUnder50000Enemy": return decideHpUnder50000Enemy();
            case "decideHpUnder10000Enemy": return decideHpUnder10000Enemy();
            case "decideHpUnder5000Enemy": return decideHpUnder5000Enemy();
            case "decideHpUnder3000Enemy": return decideHpUnder3000Enemy();
            case "decideHpUnder2000Enemy": return decideHpUnder2000Enemy();
            case "decideHpUnder1000Enemy": return decideHpUnder1000Enemy();
            case "decideHpUnder500Enemy": return decideHpUnder500Enemy();
            case "decideHpUnder100Enemy": return decideHpUnder100Enemy();
            case "decideAnyAlly": return decideAnyAlly();
            case "decideLowestHPAlly": return decideLowestHPAlly();
            case "decideHpUnder90percentAlly": return decideHpUnder90percentAlly();
            case "decideHpUnder70percentAlly": return decideHpUnder70percentAlly();
            case "decideHpUnder50percentAlly": return decideHpUnder50percentAlly();
            case "decideHpUnder30percentAlly": return decideHpUnder30percentAlly();
            case "decideHpUnder10percentAlly": return decideHpUnder10percentAlly();
            case "decideHpUnder0percentAlly": return decideHpUnder0percentAlly();

            case "actionAttack": return actionAttack();

            default:
                return false;
        }

    }


    void getSeeMaxBio()
    {
        GameObject[] tempALL = GameObject.FindGameObjectsWithTag("biology");
        List<GameObject> tempNew = new List<GameObject>();
        foreach (var t in tempALL)
        {
            if (Vector3.Distance(Transform.position, t.transform.position) < seeMax)
            {
                tempNew.Add(t);
            }
        }
        battleBios = tempNew;

    }
    bool actionAttack()
    {
        if (!target == Transform && target.GetComponent<biologyCS>().HP > 0)
        {
            Transform.GetComponent<biologyCS>().bioAction = "actionAttack";
            return true;
        }
        return false;
    }


    //回傳隨便一個敵方
    bool decideAnyEnemy()
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
    bool decideClosestEnemy()
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = Transform.transform.position;
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
    bool decideFurthestEnemy()
    {
        Transform tMax = null;
        float maxDist = Mathf.Infinity;
        Vector3 currentPos = Transform.position;
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
    bool decideHighestHPEnemy()
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
    bool decideLowestHPEnemy()
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
    bool decideHighestHPMaxEnemy()
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
    bool decideLowestHPMaxEnemy()
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
    bool decideLowestLevelEnemy()
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
    bool decideHighestLevelEnemy()
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
    bool decideLeaderTarget()
    {
        foreach (var t in battleBios)
        {
            if (t.GetComponent<biologyCS>().target.name == leaderName)
            {
                target = t.transform;
                return true;
            }
        }
        return false;
    }
    //回傳以我為目標的目標
    bool decideTargetSelfEnemy()
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
    bool decideTargetAllyEnemy()
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
    bool decideHpOver90percentEnemy()
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
    bool decideHpOver70percentEnemy()
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
    bool decideHpOver50percentEnemy()
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
    bool decideHpOver30percentEnemy()
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
    bool decideHpOver10percentEnemy()
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
    bool decideHpUnder90percentEnemy()
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
    bool decideHpUnder70percentEnemy()
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
    bool decideHpUnder50percentEnemy()
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
    bool decideHpUnder30percentEnemy()
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
    bool decideHpUnder10percentEnemy()
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
    bool decideHpOver100000Enemy()
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
    bool decideHpOver50000Enemy()
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
    bool decideHpOver10000Enemy()
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
    bool decideHpOver5000Enemy()
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

    bool decideHpOver3000Enemy()
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
    bool decideHpOver2000Enemy()
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
    bool decideHpOver1000Enemy()
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
    bool decideHpOver500Enemy()
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
    bool decideHpOver100Enemy()
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
    bool decideHpUnder100000Enemy()
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
    bool decideHpUnder50000Enemy()
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
    bool decideHpUnder10000Enemy()
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
    bool decideHpUnder5000Enemy()
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

    bool decideHpUnder3000Enemy()
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
    bool decideHpUnder2000Enemy()
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
    bool decideHpUnder1000Enemy()
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
    bool decideHpUnder500Enemy()
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
    bool decideHpUnder100Enemy()
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
    bool decideAnyAlly()
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
    bool decideLowestHPAlly()
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
    bool decideHpUnder90percentAlly()
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
    bool decideHpUnder70percentAlly()
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
    bool decideHpUnder50percentAlly()
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
    bool decideHpUnder30percentAlly()
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
    bool decideHpUnder10percentAlly()
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
    bool decideHpUnder0percentAlly()
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