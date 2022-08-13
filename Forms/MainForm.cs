using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using WebSocketSharp;

namespace Forms
{
    public partial class MainForm : Form
    {
        private Process m_Process = null;
        private IntPtr m_UnityHWND = IntPtr.Zero;

        private string m_UnityPath = @"UnityApp\App.exe";

        private WebSocket m_WebSocket = null;

        private string m_CurrentSaveFile = "";

        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);

        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

        public MainForm()
        {
            InitializeComponent();
            Application.ApplicationExit += new EventHandler(Exit);

            try
            {
                m_Process = new Process();
                m_Process.StartInfo.FileName = m_UnityPath;
                m_Process.StartInfo.Arguments = "-parentHWND " + UnityPanel.Handle.ToInt32() + " " + Environment.CommandLine;
                m_Process.StartInfo.UseShellExecute = true;
                m_Process.StartInfo.CreateNoWindow = true;
                m_Process.Start();
                m_Process.WaitForInputIdle();
                EnumChildWindows(UnityPanel.Handle, WindowEnum, IntPtr.Zero);

                m_WebSocket = new WebSocket("ws://localhost:3000/");
                m_WebSocket.OnOpen += (sender, e) =>
                {
                    Console.Write("WebSocket Open\n");
                };

                m_WebSocket.OnMessage += (sender, e) =>
                {
                    Console.Write("WebSocket Message Type: " + e.GetType() + ", Data: " + e.Data + "\n");
                };

                m_WebSocket.OnError += (sender, e) =>
                {
                    Console.Write("WebSocket Error Message: " + e.Message + "\n");
                };

                m_WebSocket.OnClose += (sender, e) =>
                {
                    Console.Write("WebSocket Close\n");
                };

                m_WebSocket.ConnectAsync();

                this.ActiveControl = UnityPanel;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Exit(object sender, EventArgs e)
        {
            m_WebSocket.Send(((int)MessageType.MSG_SHUTDOWN).ToString());
            m_WebSocket.Close();
            m_WebSocket = null;
            Application.ApplicationExit -= new EventHandler(Exit);
        }

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            m_UnityHWND = hwnd;
            return 0;
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_WebSocket.Send(((int)MessageType.MSG_NEW).ToString());
            m_CurrentSaveFile = "";
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "セーブファイル(*.txt)|*.txt";
            openFileDialog.Title = "開くファイルを選択してください";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(openFileDialog.FileName))
                {
                    string path = openFileDialog.FileName.Replace("\\", "/");
                    m_WebSocket.Send(((int)MessageType.MSG_OPEN).ToString() + "," + path);
                    m_CurrentSaveFile = path;
                }
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "save.txt";
            saveFileDialog.Filter = "セーブファイル(*.txt)|*.txt";
            saveFileDialog.Title = "保存先のファイルを選択してください";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = saveFileDialog.FileName.Replace("\\", "/");
                m_WebSocket.Send(((int)MessageType.MSG_SAVE).ToString() + "," + path);
                m_CurrentSaveFile = path;
            }
        }

        private void OverWriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(m_CurrentSaveFile))
            {
                SaveAsToolStripMenuItem_Click(null, null);
            }
            else
            {
                m_WebSocket.Send(((int)MessageType.MSG_SAVE).ToString() + "," + m_CurrentSaveFile);
            }
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_WebSocket.Send(((int)MessageType.MSG_UNDO).ToString());
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_WebSocket.Send(((int)MessageType.MSG_REDO).ToString());
        }

        private void VersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_WebSocket.Send(((int)MessageType.MSG_VERSION).ToString());
        }

        private void CreateCubeButton_Click(object sender, EventArgs e)
        {
            m_WebSocket.Send(((int)MessageType.MSG_DROP_CUBE).ToString());
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_WebSocket.Send(((int)MessageType.MSG_SHUTDOWN).ToString());
            m_WebSocket.Close();
            m_WebSocket = null;
            Application.ApplicationExit -= new EventHandler(Exit);
            this.Close();
        }
    }
}
