using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;

public class Echo : WebSocketBehavior
{
    public  static Queue<string> m_qMsg = new Queue<string>();
    public static string GetMsg() 
    {
        return m_qMsg.Count == 0 ? "" : m_qMsg.Dequeue();
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        m_qMsg.Enqueue(e.Data);
    }

    public void Response(string msg)
    {
        Sessions.Broadcast(msg);
    }
}