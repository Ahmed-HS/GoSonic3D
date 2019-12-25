using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using GlmNet;
using System;

namespace GOSonic3D
{
    public partial class GOSonic3DForm : Form
    {
        Thread MainLoopThread;

        float deltaTime;
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
                    Constants.renderer.charcter.ToggleJump();
                }
                if (e.KeyChar == 'l')
                {
                    Constants.renderer.charcter.ShiftRight();
                }
                if (e.KeyChar == 'j')
                {
                    Constants.renderer.charcter.ShiftLeft();
                }

                if (e.KeyChar == 'w')
                {
                    Constants.renderer.charcter2.ToggleJump();
                }
                if (e.KeyChar == 'd')
                {
                    Constants.renderer.charcter2.ShiftRight();
                }
                if (e.KeyChar == 'a')
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
                        Constants.renderer.SwitchCharacter();
                        Constants.renderer.charcter.Show();
                        Constants.renderer.charcter2.Show();
                        Constants.PlayingGame = true;
                        Constants.renderer.characterSelected = true;
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
            }

        }

        private void GOSonic3DForm_Load(object sender, EventArgs e)
        {
        }

        private void MoveCursor()
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Point p = PointToScreen(simpleOpenGlControl1.Location);
            Cursor.Position = new Point(simpleOpenGlControl1.Size.Width/2+p.X, simpleOpenGlControl1.Size.Height/2+p.Y);
            Cursor.Clip = new Rectangle(this.Location, this.Size);
        }

        private void SimpleOpenGlControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
