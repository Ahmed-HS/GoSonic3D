using GlmNet;
using GOSonic3D._3D_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSonic3D.Entity.Objects
{
    public class Character : md2
    {
        public enum Lane
        {
            Left = -28,
            Middle = 0,
            Right = 28
        }

        public Lane CurrentLane = Lane.Middle;

        public bool IsJumping;
        public bool IsDieing;
        public float Scale;
        public float GroundY;
        vec3 IntialPosition;
        public void ToggleJump()
        {
            if (!IsJumping && Constants.PlayingGame)
            {
                IsJumping = true;
                StartAnimation((animType)2);
                TranslateByY(60, 3.5f);
                Console.WriteLine("Jump Presed");
                Console.WriteLine("Velocity :" + CurrentVelocity.y);
                Console.WriteLine("acc :" + Acceleration.y);
            }            
        }

        public void ToggleDeath()
        {
            
            if (!IsDieing)
            {
                IntialPosition = Position;
                IsDieing = true;
                StartAnimation((animType)2);
                TranslateByY(80, 4.5f);
                Console.WriteLine("Jump Presed");
                Console.WriteLine("Velocity :" + CurrentVelocity.y);
                Console.WriteLine("acc :" + Acceleration.y);
            }
        }

        public void ShiftRight()
        {
            if (!Constants.PlayingGame)
            {
                return;
            }

            if (CurrentLane == Lane.Left)
            {
                MoveToX((int)Lane.Middle *Constants.AspectRatio);
                CurrentLane = Lane.Middle;
            }
            else if (CurrentLane == Lane.Middle)
            {
                MoveToX((int)Lane.Right * Constants.AspectRatio);
                CurrentLane = Lane.Right;
            }
        }

        public void ShiftLeft()
        {
            if (!Constants.PlayingGame)
            {
                return;
            }

            if (CurrentLane == Lane.Middle)
            {
                MoveToX((int)Lane.Left * Constants.AspectRatio);
                CurrentLane = Lane.Left;
            }
            else if (CurrentLane == Lane.Right)
            {
                MoveToX((int)Lane.Middle * Constants.AspectRatio);
                CurrentLane = Lane.Middle;
            }
        }

        public void Die()
        {
            if (Acceleration.y > 0)
            {
                CurrentVelocity.y += -(Acceleration.y * 2);
                Console.WriteLine("declerating Up :" + CurrentVelocity.y + "   " + Acceleration.y);
                //Console.WriteLine("Current Position : " + Position.y);
            }
            else
            {
                CurrentVelocity.y += -Acceleration.y - 0.05f;
                Console.WriteLine("Down");
            }


            if ((Acceleration.y > 0 && (CurrentVelocity.y <= 0)))
            {
                MoveToY(GroundY - 200, 0.5f);
                //Target.y = GroundY;
                Console.WriteLine("Top");
                //Console.WriteLine("Velocity :" + CurrentVelocity.y);
                Console.WriteLine("acc :" + Acceleration.y);
            }

            if (Acceleration.y < 0 && FinishedMovement.y == 1)
            {
                IsDieing = false;
                StartAnimation((animType)0);
                CurrentVelocity.y = 0;
                SetPostionY(IntialPosition.y);
                UpdateAnimationAndMove();
                Hide();

                Constants.MainMenu.ShowMenu();
                Constants.PlayingGame = false;
                Constants.MainMenu.SetPositionZ(Position.z + 30 * Constants.AspectRatio);

                Constants.SelectScreen = false;
                Constants.renderer.characterSelected = 0;
                Constants.renderer.SetCharactersPosition();
            }
        }

        void Jump()
        {
            if (Acceleration.y > 0)
            {
                CurrentVelocity.y += -(Acceleration.y * 2);
                Console.WriteLine("declerating Up :" + CurrentVelocity.y + "   " + Acceleration.y);
                //Console.WriteLine("Current Position : " + Position.y);
            }
            else
            {
                CurrentVelocity.y += -Acceleration.y - 0.05f;
                Console.WriteLine("Down");
            }


            if ((Acceleration.y > 0 && (ReachedTarget() || CurrentVelocity.y <= 0)))
            {
                MoveToY(GroundY, 0.5f);
                Console.WriteLine("Top");
                Console.WriteLine("acc :" + Acceleration.y);
            }

            if (Acceleration.y < 0 && FinishedMovement.y == 1)
            {
                IsJumping = false;
                StartAnimation((animType)0);
                CurrentVelocity.y = 0;
                Console.WriteLine("Ground");
            }
        }

        public override void UpdateMovement()
        {          
            if (IsJumping && !IsDieing)
            {
                Jump();
            }

            if (IsDieing)
            {
                Die();
            }
            TranslateByZ(-20, 1.2f);
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

        public Character(string filename) : base(filename)
        {
            StartAnimation((animType)0);
            List<mat4> Rotations = new List<mat4>();
            Rotations.Add(glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0)));
            Rotations.Add(glm.rotate((float)((90.0f / 180) * Math.PI), new vec3(0, 1, 0)));
            rotationMatrix = MathHelper.MultiplyMatrices(Rotations);
            Scale = 2*Constants.AspectRatio;
            scaleMatrix = glm.scale(new mat4(1), new vec3(0, 0, 0));
            Position = new vec3(0, 550, -20 ) * Constants.AspectRatio;
            IntialPosition = Position;
            GroundY = Position.y;
            Target = Position;
            IsJumping = false;
        }
    }
}
