using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System;
public class NetManager : MonoBehaviour
{
    //定义套接字(全局变量)
    static Socket socket;  //只需要一个Socket连接体
    //接收缓冲区
    static byte[] readBuff = new byte[1024];
    //委托类型
    public delegate void MsgListener(string str);
    //监听列表
    private static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>();
    //消息列表
    static List<string> msgList = new List<string>();

    //添加监听
    public static void AddListener(string msgName,MsgListener listener)
    {
        listeners[msgName] = listener;
    }

    //获取描述
    public static string GetDesc()
    {
        if (socket == null) return "";
        if (!socket.Connected) return"";
        return socket.LocalEndPoint.ToString();
    }
    public static void Connect(string ip,int port)
    {
        //socket创建
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Connect
        socket.Connect(ip, port);
        //BeginReceive
        socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);
    }
    public static void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
            msgList.Add(recvStr);
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);
        }catch(SocketException vs)
        {
            Debug.Log("Error" + vs);
        }
    }
    //发送
    public static void Send(string sendStr)  //实现真正意义的同步
    {
        if (socket == null) return;
        if (!socket.Connected) return;
        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        socket.BeginSend(sendBytes,0,sendBytes.Length,0,SendCallBack,socket);
    }
    public static void SendCallBack(IAsyncResult ar)
    {
        try                                              // 这一块不会写！！！
        {
            Socket socket = (Socket)ar.AsyncState;
                     
        }
        catch (SocketException vs)
        {
            Debug.Log("Error" + vs);
        }
    }
  //Update
    public  static void Update()
    {
        if (msgList.Count <= 0) return;
        string msgStr = msgList[0];
        msgList.RemoveAt(0);
        string[] split = msgStr.Split('|');
        string msgName = split[0];
        string msgArgs = split[1];
        //监听回调（使用名字对应的方法）
        if (listeners.ContainsKey(msgName))
        {
            listeners[msgName](msgArgs);
        }

    }



}
