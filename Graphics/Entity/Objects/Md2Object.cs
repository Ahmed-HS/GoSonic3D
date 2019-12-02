using GlmNet;
using GOSonic3D._3D_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSonic3D.Entity.Objects
{
    public class Md2Object : md2
    {
        public Entity.Lane currentLane = Lane.Middle;

        public bool IsJumping;
        public float Scale;
        public void Jump()
        {
            if (!IsJumping)
            {
                IsJumping = true;
                StartAnimation((animType)2);
                MoveToY(36,3f);
                Console.WriteLine("Jump Presed");
                Console.WriteLine("Velocity :" + CurrentVelocity.y);
                Console.WriteLine("acc :" + Acceleration.y);
            }
        }

        public void ShiftRight()
        {
            if (currentLane == Entity.Lane.Left)
            {
                MoveToX((int)Lane.Middle);
                currentLane = Lane.Middle;
            }
            else if (currentLane == Entity.Lane.Middle)
            {
                MoveToX((int)Lane.Right);
                currentLane = Lane.Right;
            }
        }

        public void ShiftLeft()
        {
            if (currentLane == Entity.Lane.Middle)
            {
                MoveToX((int)Lane.Left);
                currentLane = Entity.Lane.Left;
            }
            else if (currentLane == Entity.Lane.Right)
            {
                MoveToX((int)Lane.Middle);
                currentLane = Lane.Middle;
            }
        }

        public override void UpdateMovement()
        {
            

            if (IsJumping)
            {
                if (Acceleration.y > 0)
                {
                    CurrentVelocity.y += -(Acceleration.y * 2);
                    Console.WriteLine("declerating Up :" + CurrentVelocity.y + "   " + Acceleration.y);
                    Console.WriteLine("Current Position : " + Position.y);
                }
                else
                {
                    CurrentVelocity.y += -Acceleration.y - 0.05f;
                }


                if((Acceleration.y > 0 && (ReachedTarget() || CurrentVelocity.y <= 0)))
                {
                    MoveToY(0,0.5f);
                    Console.WriteLine("Top");
                    Console.WriteLine("Velocity :" + CurrentVelocity.y);
                    Console.WriteLine("acc :" + Acceleration.y);
                }

                if (Target.y == 0 && Position.y <= 0)
                {
                    IsJumping = false;
                    StartAnimation((animType)0);
                    CurrentVelocity.y = 0;
                    Console.WriteLine("Ground");
                }
                //Console.WriteLine("Jumping :" + CurrentVelocity.y + "   " + Acceleration.y);
            }
            UpdatePositon();
            UpdateAnimationAndMove();
        }

        public void Show()
        {
            scaleMatrix = glm.scale(new mat4(1), new vec3(Scale, Scale, Scale));
        }

        public void Hide()
        {
            scaleMatrix = glm.scale(new mat4(1), new vec3(0, 0, 0));
        }

        public Md2Object(string filename) : base(filename)
        {
            StartAnimation((animType)0);
            List<mat4> Rotations = new List<mat4>();
            Rotations.Add(glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0)));
            Rotations.Add(glm.rotate((float)((90.0f / 180) * Math.PI), new vec3(0, 1, 0)));
            rotationMatrix = MathHelper.MultiplyMatrices(Rotations);
            Scale = 3;
            scaleMatrix = glm.scale(new mat4(1), new vec3(0, 0, 0));
            Position = new vec3(0, 0, -30);
            Target = Position;
            IsJumping = false;
        }
    }
}
