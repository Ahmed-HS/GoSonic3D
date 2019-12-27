using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using GlmNet;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GOSonic3D.Entity.Objects;

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
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            this.Bounds = Screen.PrimaryScreen.Bounds;
            this.simpleOpenGlControl1.Bounds = Screen.PrimaryScreen.Bounds;
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

            string projectPath = System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

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
                    Constants.renderer.player[0].ToggleJump();
                }
                else
                if (e.KeyChar == 'l')
                {
                    serverMessage = "r";
                    Constants.renderer.player[0].ShiftRight();
                }
                else
                if (e.KeyChar == 'j')
                {
                    serverMessage = "l";
                    Constants.renderer.player[0].ShiftLeft();
                }
                else
                    serverMessage = "";

                //if (e.KeyChar == 'w')
                ////if (clientMessage == "j")
                //{
                //    Constants.renderer.charcter2.ToggleJump();
                //}
                //if (e.KeyChar == 'd')
                ////if (clientMessage == "r")
                //{
                //    Constants.renderer.charcter2.ShiftRight();
                //}
                //if (e.KeyChar == 'a')
                ////if (clientMessage == "l")
                //{
                //    Constants.renderer.charcter2.ShiftLeft();
                //}
            }

            if (e.KeyChar == 'p')
            {
                if (Constants.currentMen == MenuType.main)
                    Constants.MainMenu.MoveDown();
                else if (Constants.currentMen == MenuType.choosep1)
                {
                    Constants.choosep1.MoveDown();
                    Constants.renderer.selectedCharacter = (Constants.renderer.selectedCharacter + 1) % Constants.renderer.numOfCharacters;
                    Constants.renderer.charcter = Constants.renderer.charcters[Constants.renderer.selectedCharacter];
                }
                else if (Constants.currentMen == MenuType.choosep2)
                {
                    Constants.choosep2.MoveDown();
                    Constants.renderer.selectedCharacter = (Constants.renderer.selectedCharacter + 1) % Constants.renderer.numOfCharacters;
                    Constants.renderer.charcter2 = Constants.renderer.charcters[Constants.renderer.selectedCharacter];
                }
            }
            if (e.KeyChar == 'o')
            {
                if (Constants.currentMen == MenuType.main)
                    Constants.MainMenu.MoveUp();
                else if (Constants.currentMen == MenuType.choosep1)
                {
                    Constants.choosep1.MoveUp();
                    Constants.renderer.selectedCharacter = (Constants.renderer.selectedCharacter - 1 + Constants.renderer.numOfCharacters) % Constants.renderer.numOfCharacters;
                    Constants.renderer.charcter = Constants.renderer.charcters[Constants.renderer.selectedCharacter];
                }
                else if (Constants.currentMen == MenuType.choosep2)
                {
                    Constants.choosep2.MoveUp();
                    Constants.renderer.selectedCharacter = (Constants.renderer.selectedCharacter - 1 + Constants.renderer.numOfCharacters) % Constants.renderer.numOfCharacters;
                    Constants.renderer.charcter2 = Constants.renderer.charcters[Constants.renderer.selectedCharacter];
                }
            }
            if (e.KeyChar == '\r')
            {
                if (Constants.currentMen == MenuType.main)
                {
                    if (Constants.MainMenu.Selected == 0)
                    {
                        Constants.playType = PlayType.single;
                        Constants.MainMenu.HideMenu();

                        Constants.currentMen = MenuType.choosep1;
                        Constants.choosep1.Selected = 0;
                        Constants.choosep1.ShowMenu();
                    }
                    else if (Constants.MainMenu.Selected == 1)
                    {
                        Constants.playType = PlayType.multi;

                        Constants.MainMenu.HideMenu();

                        Constants.currentMen = MenuType.choosep1;
                        Constants.choosep1.Selected = 0;
                        Constants.choosep1.ShowMenu();
                    }
                    else if (Constants.MainMenu.Selected == 2)
                    {
                        this.Close();
                        serverSocket.Close();
                    }
                }
                else if (Constants.currentMen == MenuType.choosep1)
                {
                    if (Constants.playType == PlayType.single)
                    {
                        Constants.renderer.charcter.Show();
                        Constants.currentMen = MenuType.play;
                        Constants.choosep1.HideMenu();
                        Constants.PlayingGame = true;
                        Constants.isGameOver = false;
                        Constants.gameover.HideMenu();

                        for (int i = 0; i < Constants.renderer.Enemies.Length; i++)
                        {
                            Constants.renderer.Enemies[i] = new GroundedObject(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Shadow.md2", new Character[] { Constants.renderer.charcter, Constants.renderer.charcter2 }, Constants.renderer.cam, GroundedObject.Type.Enemy);
                        }

                        for (int i = 0; i < Constants.renderer.Rings.Length; i++)
                        {
                            Constants.renderer.Rings[i] = new GroundedObject(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Ring.md2", new Character[] { Constants.renderer.charcter, Constants.renderer.charcter2 }, Constants.renderer.cam, GroundedObject.Type.Ring);
                        }
                    }
                    else if (Constants.playType == PlayType.multi)
                    {
                        Constants.renderer.charcter.Show();
                        Constants.currentMen = MenuType.choosep2;
                        Constants.choosep1.HideMenu();
                        Constants.choosep2.ShowMenu();
                    }
                }
                else if (Constants.currentMen == MenuType.choosep2)
                {
                    Constants.renderer.charcter2.SetPostionZ(Constants.renderer.charcter.Position.z);
                    Constants.renderer.charcter2.UpdatePositon();
                    Constants.renderer.charcter2.UpdateMovement();
                    Constants.currentMen = MenuType.play;
                    Constants.renderer.charcter2.Show();
                    Constants.choosep2.HideMenu();
                    Constants.PlayingGame = true;
                    Constants.isGameOver = false;
                    Constants.gameover.HideMenu();

                    for (int i = 0; i < Constants.renderer.Enemies.Length; i++)
                    {
                        Constants.renderer.Enemies[i] = new GroundedObject(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Shadow.md2", new Character[] { Constants.renderer.charcter, Constants.renderer.charcter2 }, Constants.renderer.cam, GroundedObject.Type.Enemy);
                    }

                    for (int i = 0; i < Constants.renderer.Rings.Length; i++)
                    {
                        Constants.renderer.Rings[i] = new GroundedObject(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Ring.md2", new Character[] { Constants.renderer.charcter, Constants.renderer.charcter2 }, Constants.renderer.cam, GroundedObject.Type.Ring);
                    }
                }
                /*if (Constants.MainMenu.Selected == 0)
                {
                    if (Constants.SelectScreen)
                    {
                        Constants.renderer.characterSelected++;
                        Constants.renderer.SwitchCharacter();

                        if(Constants.renderer.characterSelected == 1)
                        {
                            Constants.renderer.charcter.Show();
                            Constants.PlayingGame = true;
                            Console.WriteLine("da5al");
                        }

                        if(Constants.renderer.characterSelected == 2)
                        {
                            Constants.renderer.charcter2.Show();
                            Console.WriteLine("da5al2");
                        }

                        if (Constants.renderer.player.Length > 1)
                            if (Constants.renderer.characterSelected == 2)
                            {
                                Constants.renderer.player[1].Show();
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
                if (Constants.MainMenu.Selected == 2)
                {
                    this.Close();
                    serverSocket.Close();
                }*/
            }

            if (e.KeyChar == '\b')
            {
                if (Constants.PlayingGame)
                {
                    Constants.currentMen = MenuType.main;
                    Constants.renderer.charcter.Hide();
                    Constants.MainMenu.ShowMenu();
                    Constants.PlayingGame = false;

                    Constants.MainMenu.SetPositionZ(Constants.renderer.charcter.Position.z + 10 * Constants.AspectRatio);
                    Constants.choosep1.SetPositionZ(Constants.renderer.charcter.Position.z + 10 * Constants.AspectRatio);
                    Constants.choosep2.SetPositionZ(Constants.renderer.charcter.Position.z + 10 * Constants.AspectRatio);
                    Constants.gameover.SetPositionZ(Constants.renderer.charcter.Position.z + 10 * Constants.AspectRatio);

                }
                else
                {
                    Constants.currentMen = MenuType.play;
                    Constants.renderer.charcter.Show();
                    if (Constants.playType == PlayType.multi)
                        Constants.renderer.charcter2.Show();
                    Constants.MainMenu.HideMenu();
                    Constants.PlayingGame = true;
                }
            }

            if (e.KeyChar == 'x')
            {
                this.Close();
                serverSocket.Close();
            }
        }

        private void GOSonic3DForm_Load(object sender, EventArgs e)
        {

        }
    }
}
