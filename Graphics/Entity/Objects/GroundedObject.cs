using GlmNet;
using GOSonic3D._3D_Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOSonic3D.Entity.Objects
{
    class GroundedObject : md2
    {
        public enum Type
        {
            Enemy,Ring
        }

        Character Player;
        Random MyRandom;
        float[] Lanes;
        float Scale;
        MoveableObject Camera;
        Type ObjectType;
        public GroundedObject(string FilePath,Character Player, MoveableObject Camera,Type ObjectType) : base(FilePath)
        {
            this.ObjectType = ObjectType;
            this.Camera = Camera;
            this.Player = Player;
            MyRandom = new Random();
            Lanes = new float[3];
            Lanes[0] = -28;
            Lanes[1] = 0;
            Lanes[2] = 28;
            StartAnimation((animType)0);
            List<mat4> Rotations = new List<mat4>();
            Rotations.Add(glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0)));
            Rotations.Add(glm.rotate((float)((270.0f / 180) * Math.PI), new vec3(0, 1, 0)));
            rotationMatrix = MathHelper.MultiplyMatrices(Rotations);
            Scale = 0.3f * Constants.AspectRatio;
            scaleMatrix = glm.scale(new mat4(1), new vec3(Scale, Scale, Scale));
            Position = new vec3(Lanes[MyRandom.Next(0,3)], 550, MyRandom.Next(-1100,-800)) * Constants.AspectRatio;
            Target = Position;
        }

        public void setPlayer(Character Player)
        {
            this.Player = Player;
        }

        public bool OffScreen()
        {
            //Console.WriteLine("End : " + (StartZ - MapLength).ToString());
            //Console.WriteLine("Sonic : " + (Camera.Position.z).ToString());
            return Position.z >= Camera.Position.z;
        }

        public void Respawn()
        {
            SetPostionZ(Camera.Position.z - 770 - GameMap.MapLength);
            SetPostionX(Lanes[MyRandom.Next(0, 3)] * Constants.AspectRatio);
        }

        public double DistanceFrom(vec3 a, vec3 b)
        {
            return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2) + Math.Pow(a.z - b.z, 2));
        }

        bool collision(Character a)
        {
            double ax = Math.Abs((Math.Abs(a.maxxyz.x) - Math.Abs(a.minxyz.x))) / 2;
            double ay = Math.Abs((Math.Abs(a.maxxyz.y) - Math.Abs(a.minxyz.y))) / 2;
            double az = Math.Abs((Math.Abs(a.maxxyz.z) - Math.Abs(a.minxyz.z)));

            double bx = Math.Abs((Math.Abs(maxxyz.x) - Math.Abs(minxyz.x))) / 2;
            double by = Math.Abs((Math.Abs(maxxyz.y) - Math.Abs(minxyz.y))) / 2;
            double bz = Math.Abs((Math.Abs(maxxyz.z) - Math.Abs(minxyz.z))) / 2;
                        
            double xmiddis = a.Position.x - Position.x;
            double ymiddis = a.Position.y - Position.y;
            double zmiddis = a.Position.z - Position.z;

            double dis = DistanceFrom(a.Position, Position);
            

            return  dis < az
            ;
        }

        public override void UpdateMovement()
        {
            TranslateByZ(20,2.5f);
            if (collision(Player) && Constants.PlayingGame)
            //if (DetectCollision(Player) && Constants.PlayingGame)
            {
                if (ObjectType == Type.Enemy)
                {
                    Player.ToggleDeath();
                }
                else 
                if (ObjectType == Type.Ring)
                {
                    //string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
                    //Constants.SoundEffects = new System.Media.SoundPlayer(projectPath + "\\Audio\\Ring.wav");
                    //Constants.SoundEffects.Play();
                    Respawn();
                }
                
            }
            if (OffScreen())
            {
                Respawn();
            }
            UpdatePositon();
            UpdateAnimationAndMove();
        }

    }
}
