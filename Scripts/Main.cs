using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject humanPrefab;
    public GameObject enemyPrefab;
    public Material enemyMaterial;
     BaseHuman myHuman;
    public Dictionary<string, BaseHuman> otherHumans = new Dictionary<string, BaseHuman>();//此处不初始化就会死人的哈哈哈

    
    private void Start()

    {
        Debug.Log("Test!!!!");
        NetManager.AddListener("Attack", OnAttack);
        NetManager.AddListener("List", OnList);
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Leave", Leave);
        NetManager.Connect("192.168.235.60", 8888);  //此处连接不上就不执行下面的代码，即无法创建角色
        GameObject obj = (GameObject)Instantiate(humanPrefab);
        obj.tag = "Myself";
        obj.name = "Player";
        GameObject[] g= GameObject.FindGameObjectsWithTag("Shoulder");
        Material[] materials = g[0].GetComponent<MeshRenderer>().materials;
        Material[] materials2 = g[1].GetComponent<MeshRenderer>().materials;
        materials[0] = enemyMaterial;
        materials2[0] = enemyMaterial;
        
        float x = Random.Range(-6, 6);
        float z = Random.Range(-6, 6);
        obj.transform.position = new Vector3(x, 0, z);
        myHuman = obj.AddComponent<CtrlHuman>();
        myHuman.desc = NetManager.GetDesc();

        //发送协议
        Vector3 pos = myHuman.transform.position;//获取坐标
        Vector3 eul = myHuman.transform.eulerAngles;//获取旋转角
        string sendStr = "Enter|";
        sendStr += NetManager.GetDesc() + ",";
        sendStr += pos.x + ",";
        sendStr += pos.y + ",";
        sendStr += pos.z + ",";
        sendStr += eul.y+",";
        NetManager.Send(sendStr);
        NetManager.Send("List|");
    }
    //攻击
    void OnAttack(string msgArgs)
    {
        Debug.Log("I am Fighting!");
        //解析参数
        string[] split = msgArgs.Split(',');
        string des = split[0];
        float euly = float.Parse(split[1]);
        //
        if (!otherHumans.ContainsKey(des)) return;
        SyncHuman h = (SyncHuman)otherHumans[des];
        h.SyncAttack(euly);
    }
    //死亡
    void OnDie(string msg)
    {
        //解析
        string[] split = msg.Split(',');
        string attDec = split[0];
        string beenHitDec = split[1];
        //自己死了
        if (beenHitDec == myHuman.desc) { Debug.Log("Game Over!");
            Destroy(GameObject.FindGameObjectWithTag("Myself"));
        }
        if (!otherHumans.ContainsKey(beenHitDec)) return;
        SyncHuman h = (SyncHuman)otherHumans[beenHitDec];
        h.gameObject.SetActive(false);

    }
    
    public void OnList(string msgArgs)
    {
        Debug.Log("OnList" + msgArgs);
        //解析参数
        string[] split = msgArgs.Split(',');
        for (int i = 0; i < ((split.Length-1)/6); i++)
        {
            string desc = split[i * 6 + 0];
            float x = float.Parse(split[i * 6 + 1]);
            float y = float.Parse(split[i * 6 + 2]);
            float z = float.Parse(split[i * 6 + 3]);
            float euly = float.Parse(split[i * 6 + 4]);
            int hp = int.Parse(split[i * 6 + 5]);
            //若是自己,退出该此循环

            if (desc == NetManager.GetDesc())
            {
                continue;
            }
            //同步初始化网络角色
            GameObject other = (GameObject)Instantiate(enemyPrefab);
            other.transform.position = new Vector3(x, y, z);
            other.transform.eulerAngles = new Vector3(0, euly, 0);
            BaseHuman h = other.AddComponent<SyncHuman>();                  //此处子类对象赋值给父类，留有悬念！！2！看看后期如何处理。
            h.desc = desc;
            otherHumans.Add(desc, h);
        }

    }
    public void OnEnter(string msg) //此处进入的是“地址,x,y,z,euly;
    {
        Debug.Log("I just come here!!!"+msg);
        string[] split = msg.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y= float.Parse(split[2]); float z= float.Parse(split[3]);
        float euly = float.Parse(split[4]);
        if (desc == NetManager.GetDesc()) return;
        //初始化角色同步
        GameObject other = (GameObject)Instantiate(enemyPrefab);
        other.transform.position = new Vector3(x, y, z);
        other.transform.eulerAngles = new Vector3(0, euly, 0);
        BaseHuman h = other.AddComponent<SyncHuman>();                  //此处子类对象赋值给父类，留有悬念！！！看看后期如何处理。
        h.desc = desc;
        otherHumans.Add(desc, h);
    }
    public void OnMove(string msg)
    {
        Debug.Log("I am moving!!!");
        //解析参数
        string[] split = msg.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        //移动
        if (!otherHumans.ContainsKey(desc))
        {
            return;
        }
        BaseHuman h = otherHumans[desc];
        Vector3 changePosition = new Vector3(x, y, z);
        h.MoveTo(changePosition);
    }
    public void Leave(string msg)
    {
        Debug.Log("I just left!!!");
        //解析参数
        string[] split = msg.Split(',');
        string des = split[0];
        //删除对应角色
        if (!otherHumans.ContainsKey(des)) return;
        BaseHuman h = otherHumans[des];
        Destroy(h.gameObject);
        otherHumans.Remove(des);
    }
    private void Update()
    {
        NetManager.Update();
    }
}
