using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using GlmNet;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GOSonic3D
{
    public partial class GOSonic3DForm : Form
    {
        Thread MainLoopThread;
        float deltaTime;

        Socket serverSocket;
        string serverMessage = "";
        string clientMessage = "";

        public GOSonic3DForm()
        {
            InitializeComponent();
            //this.TopMost = true;
            //this.FormBorderStyle = FormBorderStyle.None;
            //this.WindowState = FormWindowState.Maximized;

            //this.Bounds = Screen.PrimaryScreen.Bounds;
            //this.simpleOpenGlControl1.Bounds = Screen.PrimaryScreen.Bounds;
            simpleOpenGlControl1.InitializeContexts();

            initialize();
            deltaTime = 0.005f;
            MainLoopThread = new Thread(MainLoop);
            MainLoopThread.Start();

            ////server
            //IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Any, 8000);
            //serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //serverSocket.Bind(hostEndPoint);
            //serverSocket.Listen(2);
            //Socket cs = serverSocket.Accept();

            //Thread ClientThread = new Thread(new ParameterizedThreadStart(handelServer));
            //ClientThread.Start(cs);
        }

        void initialize()
        {
            float Width = Screen.PrimaryScreen.Bounds.Width;
            float Height = Screen.PrimaryScreen.Bounds.Height;
            Constants.AspectRatio = Width / Height;
            Constants.renderer = new Renderer();
            Constants.renderer.Initialize();
        }

        void MainLoop()
        {
            while (true)
            {
                Constants.renderer.Draw();
                Constants.renderer.Update(deltaTime);
                simpleOpenGlControl1.Invoke(new Action(() => simpleOpenGlControl1.Refresh()));
            }
        }

        void handelServer(object obj)
        {
            Console.WriteLine("server");

            Socket cs = (Socket)obj;
            while (true)
            {
                byte[] serverMessageByte = Encoding.ASCII.GetBytes(serverMessage);
                cs.Send(serverMessageByte);

                //---------------------------------------

                byte[] clientMessageByte = new byte[1024];
                int len = cs.Receive(clientMessageByte);
                clientMessage = Encoding.ASCII.GetString(clientMessageByte, 0, len);
                MessageBox.Show(clientMessage);
            }
        }

        private void GOSonic3DForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Constants.renderer.CleanUp();
            MainLoopThread.Abort();
        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e)
        {
            Constants.renderer.Draw();
            Constants.renderer.Update(deltaTime);
        }

        private void simpleOpenGlControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            float speed = 0.6f;
            if (e.KeyChar == 'a')
                Constants.renderer.cam.Strafe(-speed);
            if (e.KeyChar == 'd')
                Constants.renderer.cam.Strafe(speed);
            if (e.KeyChar == 's')
                Constants.renderer.cam.Walk(-speed);
            if (e.KeyChar == 'w')
                Constants.renderer.cam.Walk(speed);
            if (e.KeyChar == 'z')
                Constants.renderer.cam.Fly(-speed);
            if (e.KeyChar == 'c')
                Constants.renderer.cam.Fly(speed);


            if (Constants.PlayingGame)
            {
                if (e.KeyChar == 'i')
                {
                    serverMessage = "j";
                    Constants.renderer.charcter.ToggleJump();
                }
                else
                if (e.KeyChar == 'l')
                {
                    serverMessage = "r";
                    Constants.renderer.charcter.ShiftRight();
                }
                else
                if (e.KeyChar == 'j')
                {
                    serverMessage = "l";
                    Constants.renderer.charcter.ShiftLeft();
                }
                else
                    serverMessage = "";

                if (e.KeyChar == 'w')
                //if (clientMessage == "j")
                {
                    Constants.renderer.charcter2.ToggleJump();
                }
                if (e.KeyChar == 'd')
                //if (clientMessage == "r")
                {
                    Constants.renderer.charcter2.ShiftRight();
                }
                if (e.KeyChar == 'a')
                //if (clientMessage == "l")
                {
                    Constants.renderer.charcter2.ShiftLeft();
                }
            }

            if (e.KeyChar == 'p')
            {
                Constants.MainMenu.MoveDown();
            }
            if (e.KeyChar == 'o')
            {
                Constants.MainMenu.MoveUp();
            }
            if (e.KeyChar == '\r')
            {
                if (Constants.MainMenu.Selected == 0)
                {
                    if (Constants.SelectScreen)
                    {
                        Constants.renderer.characterSelected++;
                        Constants.renderer.SwitchCharacter();

                        if(Constants.renderer.characterSelected == 1)
                        {
                            Constants.renderer.charcter.Show();
                        }

                        if(Constants.renderer.characterSelected == 2)
                        {
                            Constants.renderer.charcter2.Show();
                            Constants.PlayingGame = true;
                        }
                    }
                    else
                    {
                        Constants.MainMenu.HideMenu();
                        Constants.SelectScreen = true;
                    }
                }
                else
                if (Constants.MainMenu.Selected == 1)
                {
                    this.Close();
                    serverSocket.Close();
                }
            }

            if (Constants.SelectScreen)
            {
                if (e.KeyChar == 'm')
                {
                    Constants.renderer.selectedCharacter = (Constants.renderer.selectedCharacter + 1) % Constants.renderer.numOfCharacters;
                }

                if (e.KeyChar == 'n')
                {
                    Constants.renderer.selectedCharacter = (Constants.renderer.selectedCharacter - 1 + Constants.renderer.numOfCharacters) % Constants.renderer.numOfCharacters;
                }
            }

            if (e.KeyChar == '\b')
            {
                if (Constants.PlayingGame)
                {
                    Constants.renderer.charcter.Hide();
                    Constants.MainMenu.ShowMenu();
                }
                else
                {
                    Constants.renderer.charcter.Show();
                    Constants.MainMenu.HideMenu();
                }
            }

            if (e.KeyChar == 'x')
            {
                this.Close();
                serverSocket.Close();
            }
        }
    }
}
