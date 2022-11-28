using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CtrlHuman : BaseHuman
{
    //使用本继承类的start
    new void Start()
    {
        base.Start();
    }
    new void Update()
    {
        base.Update();
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            Physics.Raycast(ray, out hit);
            if (hit.collider.tag == "Terrain")
            {
                MoveTo(hit.point);
                //发送协议
                string sendStr = "Move|";
                sendStr += NetManager.GetDesc() + ",";
                sendStr += hit.point.x + ",";
                sendStr += hit.point.y + ",";
                sendStr += hit.point.z + ",";
                NetManager.Send(sendStr);
            }
        }
        //if (Input.GetMouseButtonDown(1))
        //{
        //    if (isAttacking) return;
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;
        //    Physics.Raycast(ray, out hit);
        //    transform.LookAt(hit.point);
        //    Attack();
        //    //发送协议
        //    string sendStr = "Attack|";
        //    sendStr += NetManager.GetDesc() + ",";
        //    NetManager.Send(sendStr);
        //    //攻击判定
        //    Vector3 lineEnd = transform.position + 5f * Vector3.up;
        //    Vector3 lineStart = lineEnd + 20 * transform.forward;
        //    if(Physics.Linecast(lineStart,lineEnd,out hit))
        //    {
        //        GameObject hitobj = hit.collider.gameObject;
        //        if (hitobj == gameObject) return;
        //        SyncHuman h = hitobj.GetComponent<SyncHuman>();
        //        if (h == null) return;
        //        sendStr = "Hit|";
        //        sendStr += NetManager.GetDesc() + ",";//得到"Hit|自己，击打对象，"
        //        sendStr += h.desc + ",";
        //        NetManager.Send(sendStr);
        //    }

        //}
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (isAttacking) return;
            Attack();
            //发送协议
            string sendStr = "Attack|";
            sendStr += NetManager.GetDesc() + ",";
            NetManager.Send(sendStr);
            //攻击判定

            RaycastHit hit;
            Vector3 lineEnd = transform.position + 5f * Vector3.up;
            Vector3 lineStart = lineEnd + 20 * transform.forward;
            if (Physics.Linecast(lineStart, lineEnd, out hit))
            {
                GameObject hitobj = hit.collider.gameObject;
                if (hitobj == gameObject) return;
                SyncHuman h = hitobj.GetComponent<SyncHuman>();
                if (h == null) return;
                sendStr = "Hit|";
                sendStr += NetManager.GetDesc() + ",";//得到"Hit|自己，击打对象，"
                sendStr += h.desc + ",";
                NetManager.Send(sendStr);
            }
           
        }
    }
    
    private void OnDestroy()
    {
       GameObject cube= Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube));
        cube.transform.position = transform.position;
    }
}
