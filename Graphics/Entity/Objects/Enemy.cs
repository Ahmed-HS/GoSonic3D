using GlmNet;
using GOSonic3D._3D_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSonic3D.Entity.Objects
{
    class Enemy : md2
    {
        Character Player;
        Random MyRandom;
        float[] Lanes;
        float Scale;
        public Enemy(string FilePath,Character Player) : base(FilePath)
        {
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

        public override void UpdateMovement()
        {
            TranslateByZ(20,2.5f);
            if (DetectCollision(Player) && Constants.PlayingGame)
            {
                Player.ToggleDeath();
            }
            if (Position.z > 600)
            {
                SetPostionZ(-1100*Constants.AspectRatio);
                MoveToX(Lanes[MyRandom.Next(0,3)]* Constants.AspectRatio);
            }
            UpdatePositon();
            UpdateAnimationAndMove();
        }

    }
}
