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

        public vec3 Position;
        public vec3 Target;
        public vec3 CurrentSpeed;
        public vec3 IntialSpeed;
        public vec3 Acceleration;
        public bool IsJumping;


        public void MoveToX(float TargetX)
        {
            this.Target.x = TargetX;
            ChangeSpeedX(IntialSpeed.x);

            if (Target.x > Position.x)
            {
                if (CurrentSpeed.x < 0)
                {
                    CurrentSpeed.x *= -1;
                }
            }
            else
            {
                if (CurrentSpeed.x > 0)
                {
                    CurrentSpeed.x *= -1;
                }
            }

            if (Acceleration.x > 0 && CurrentSpeed.x < 0 || Acceleration.x < 0 && CurrentSpeed.x > 0)
            {
                Acceleration.x *= -1;
            }
        }

        public void MoveToY(float TargetY)
        {
            this.Target.y = TargetY;
            if (Target.y > Position.y)
            {
                if (CurrentSpeed.y < 0)
                {
                    CurrentSpeed.y *= -1;
                }
            }
            else
            {
                if (CurrentSpeed.y > 0)
                {
                    CurrentSpeed.y *= -1;
                }
            }
        }

        public void MoveToZ(float TargetZ)
        {
            this.Target.z = TargetZ;
            if (Target.z > Position.z)
            {
                if (CurrentSpeed.z < 0)
                {
                    CurrentSpeed.z *= -1;
                }
            }
            else
            {
                if (CurrentSpeed.z > 0)
                {
                    CurrentSpeed.z *= -1;
                }
            }
        }

        public void MoveTo(vec3 Target)
        {
            MoveToX(Target.x);
            MoveToY(Target.y);
            MoveToZ(Target.z);
        }

        public void TranslateTo(vec3 Displacement)
        {
            MoveTo(new vec3(Position.x + Displacement.x, Position.y + Displacement.y, Position.z + Displacement.z));
        }
        public void Jump()
        {
            if (!IsJumping)
            {
                IsJumping = true;
                StartAnimation((animType)2);
                ChangeSpeedY(3f);
                MoveToY(30);
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

        public void ChangeSpeed(vec3 newSpeed)
        {
            CurrentSpeed = newSpeed;
        }
        public void ChangeSpeedX(float newX)
        {
            CurrentSpeed.x = newX;
        }
        public void ChangeSpeedY(float newY)
        {
            CurrentSpeed.y = newY;
        }
        public void ChangeSpeedZ(float newZ)
        {
            CurrentSpeed.z = newZ;
        }
        public void UpdateMovement()
        {
            if ((Target.x > Position.x && CurrentSpeed.x > 0) || (Target.x < Position.x && CurrentSpeed.x < 0))
            {
                Position.x += CurrentSpeed.x;
                CurrentSpeed.x += Acceleration.x;

            }
   
            if ((Target.y > Position.y && CurrentSpeed.y > 0) || (Target.y < Position.y && CurrentSpeed.y < 0))
            {
                Position.y += CurrentSpeed.y;
            }

            if ((Target.z > Position.z && CurrentSpeed.z > 0) || (Target.z < Position.z && CurrentSpeed.z < 0))
            {
                Position.z += CurrentSpeed.z;
            }

            if (IsJumping)
            {

                if (CurrentSpeed.y > 0)
                {
                    CurrentSpeed.y += Acceleration.y;
                }
                else
                {
                    CurrentSpeed.y += -Acceleration.y;
                    ChangeSpeedY(1.8f);
                    MoveToY(0);
                }

                if(Position.y <= 0)
                {
                    IsJumping = false;
                    StartAnimation((animType)0);
                    CurrentSpeed.y = 0;
                }
            }

            TranslationMatrix = glm.translate(new mat4(1), Position);
            UpdateAnimation();

        }
        public Md2Object(string filename) : base(filename)
        {
            ChangeSpeed(new vec3(0.25f, 0.25f, 0.25f));
            Acceleration = new vec3(0.12f, -0.15f, 0.12f);
            IntialSpeed = CurrentSpeed;
            StartAnimation((animType)0);
            List<mat4> Rotations = new List<mat4>();
            Rotations.Add(glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0)));
            Rotations.Add(glm.rotate((float)((90.0f / 180) * Math.PI), new vec3(0, 1, 0)));
            rotationMatrix = MathHelper.MultiplyMatrices(Rotations);
            scaleMatrix = glm.scale(new mat4(1), new vec3(3.0f, 3.0f, 3.0f));
            Position = new vec3(0, 0, -30);
            Target = Position;
            IsJumping = false;
        }
    }
}
