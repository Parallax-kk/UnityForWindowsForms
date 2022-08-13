using UnityEngine;
using WebSocketSharp.Server;

public class MessageReceiver : MonoBehaviour
{
    [SerializeField]
    private SpownCube m_SpownCube = null;

    [SerializeField]
    private SaveDataManager m_SaveDataManager = null;

    private WebSocketServer m_Server = null;

    private const int PORT = 3000;

    private void Awake()
    {
        m_Server = new WebSocketServer(PORT);
        m_Server.AddWebSocketService<Echo>("/");
        m_Server.Start();
    }

    private void OnDestroy()
    {
        m_Server.Stop();
        m_Server = null;
    }

    private void Update()
    {
        string msg = Echo.GetMsg();

        if (string.IsNullOrEmpty(msg))
            return;

        string[] splitedMsg = msg.Split(',');
        MessageType type = (MessageType)int.Parse(splitedMsg[0]);

        switch (type)
        {
            case MessageType.MSG_NEW:
                {
                    m_SpownCube.RemoveAllCube();
                    m_SpownCube.RemoveAllHistory();
                    break;
                }
            case MessageType.MSG_OPEN:
                {
                    m_SaveDataManager.Load(splitedMsg[1]);
                    break;
                }
            case MessageType.MSG_SAVE:
                {
                    string json = m_SaveDataManager.Save(splitedMsg[1]);
                    
                    break;
                }
            case MessageType.MSG_UNDO:
                {
                    m_SpownCube.Undo();
                    break;
                }
            case MessageType.MSG_REDO:
                {
                    m_SpownCube.Redo();
                    break;
                }
            case MessageType.MSG_VERSION:
                {
                    Time.timeScale = 0.0f;
                    break;
                }
            case MessageType.MSG_RESUME:
                {
                    Time.timeScale = 1.0f;
                    break;
                }
            case MessageType.MSG_SHUTDOWN:
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
                    UnityEngine.Application.Quit();
#endif
                    break;
                }
            case MessageType.MSG_DROP_CUBE:
                {
                    m_SpownCube.Spown();
                    break;
                }
            default:
                break;
        }
    }
}
