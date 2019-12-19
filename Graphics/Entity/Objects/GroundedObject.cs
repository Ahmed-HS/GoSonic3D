using GlmNet;
using GOSonic3D._3D_Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public override void UpdateMovement()
        {
            TranslateByZ(20,2.5f);
            if (DetectCollision(Player) && Constants.PlayingGame)
            {
                if (ObjectType == Type.Enemy)
                {
                    Player.ToggleDeath();
                }
                else if (ObjectType == Type.Ring)
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
