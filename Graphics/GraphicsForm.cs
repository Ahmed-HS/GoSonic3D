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
        Renderer renderer = new Renderer();
        Thread MainLoopThread;

        float deltaTime;
        public GOSonic3DForm()
        {
            InitializeComponent();
            simpleOpenGlControl1.InitializeContexts();

            //MoveCursor();
            

            initialize();
            deltaTime = 0.005f;
            MainLoopThread = new Thread(MainLoop);
            MainLoopThread.Start();

        }
        void initialize()
        {
            renderer.Initialize();   
        }
        void MainLoop()
        {
            while (true)
            {
                renderer.Draw();
                renderer.Update(deltaTime);
                simpleOpenGlControl1.Invoke(new Action(() => simpleOpenGlControl1.Refresh()));
            }
        }
        private void GOSonic3DForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            renderer.CleanUp();
            MainLoopThread.Abort();
        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e)
        {
            renderer.Draw();
            renderer.Update(deltaTime);
        }

        private void simpleOpenGlControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            float speed = 0.6f;
            if (e.KeyChar == 'a')
                renderer.cam.Strafe(-speed);
            if (e.KeyChar == 'd')
                renderer.cam.Strafe(speed);
            if (e.KeyChar == 's')
                renderer.cam.Walk(-speed);
            if (e.KeyChar == 'w')
                renderer.cam.Walk(speed);
            if (e.KeyChar == 'z')
                renderer.cam.Fly(-speed);
            if (e.KeyChar == 'c')
                renderer.cam.Fly(speed);
            if (e.KeyChar == ' ')
            {
                renderer.Sonic.Jump();
            }
            if (e.KeyChar == 'l' && renderer.PlayingGame)
            {
                renderer.Sonic.ShiftRight();       
            }
            if (e.KeyChar == 'k' && renderer.PlayingGame)
            {
                renderer.Sonic.ShiftLeft();
            }
            if (e.KeyChar == 'p')
            {
                renderer.MainMenu.MoveDown();
            }
            if (e.KeyChar == 'o')
            {
                renderer.MainMenu.MoveUp();
            }
            if (e.KeyChar == '\r')
            {
                if (renderer.MainMenu.Selected == 0)
                {
                    renderer.MainMenu.HideMenu();
                    renderer.Sonic.Show();
                    renderer.PlayingGame = true;
                }
                else if (renderer.MainMenu.Selected == 1)
                {
                    this.Close();
                }
            }

            if (e.KeyChar == '\b')
            {
                if (renderer.PlayingGame)
                {
                    renderer.Sonic.Hide();
                    renderer.MainMenu.ShowMenu();
                }
            }

        }

        private void GOSonic3DForm_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
        }

        private void MoveCursor()
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Point p = PointToScreen(simpleOpenGlControl1.Location);
            Cursor.Position = new Point(simpleOpenGlControl1.Size.Width/2+p.X, simpleOpenGlControl1.Size.Height/2+p.Y);
            Cursor.Clip = new Rectangle(this.Location, this.Size);
        }
    }
}
