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
        public int SuperDirection;
        bool IsSuper;
        public float Scale;
        public float GroundY;
        vec3 IntialPosition;
        public float RingCount;
        public void ToggleJump()
        {
            if (!IsJumping && Constants.PlayingGame && !IsSuper)
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

        public void GoSuper()
        {
            if (!IsSuper && RingCount >= 10)
            {
                string projectPath = System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
                IsSuper = true;
                IsJumping = false;
                ChangeAccelerationY(0.0005f);
                ChangeVelocityY(0.005f);
                StartAnimation((animType)3);
                LoadModel(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Sonic1.md2");
                MoveToY(IntialPosition.y + 25f*Constants.AspectRatio,0.6f*Constants.AspectRatio);
            }
        }

        public void UndoSuper()
        {
            if (IsSuper)
            {
                string projectPath = System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
                IsSuper = false;
                RestoreDefaultVelocity();
                StartAnimation((animType)0);
                LoadModel(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Sonic.md2");
                MoveToY(GroundY, 0.6f * Constants.AspectRatio);
            }
        }

        public void Fly()
        {

            if (FinishedMovement.y == 1)
            {
                SuperDirection *= -1;
                TranslateByY(3f * SuperDirection);
            }
            else
            {
                CurrentVelocity.y += -(Acceleration.y * 2f);
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
                //Console.WriteLine("declerating Up :" + CurrentVelocity.y + "   " + Acceleration.y);
                //Console.WriteLine("Current Position : " + Position.y);
            }
            else
            {
                CurrentVelocity.y += -Acceleration.y - 0.05f;
                //Console.WriteLine("Down");
            }


            if ((Acceleration.y > 0 && (CurrentVelocity.y <= 0)))
            {
                MoveToY(GroundY - 200, 0.5f);
                //Target.y = GroundY;
                //Console.WriteLine("Top");
                //Console.WriteLine("Velocity :" + CurrentVelocity.y);
                //Console.WriteLine("acc :" + Acceleration.y);
            }

            if (Acceleration.y < 0 && FinishedMovement.y == 1)
            {
                IsDieing = false;
                StartAnimation((animType)0);
                CurrentVelocity.y = 0;
                SetPostionY(IntialPosition.y);
                UpdateAnimationAndMove();
                Hide();

                Constants.currentMen = MenuType.main;
                Constants.playType = PlayType.no;

                Constants.MainMenu.ShowMenu();
                Constants.PlayingGame = false;
                Constants.isGameOver = true;
                Constants.MainMenu.SetPositionZ(Position.z + 10 * Constants.AspectRatio);
                Constants.choosep1.SetPositionZ(Position.z + 10 * Constants.AspectRatio);
                Constants.choosep2.SetPositionZ(Position.z + 10 * Constants.AspectRatio);
                Constants.gameover.SetPositionZ(Position.z + 10 * Constants.AspectRatio);
                for (int i = 0; i < Constants.renderer.numOfCharacters; i++)
                    Constants.renderer.charcters[i].SetPostionZ(Position.z);

                Constants.SelectScreen = false;
                Constants.renderer.characterSelected = 0;
                Constants.renderer.selectedCharacter = 0;
                Constants.renderer.charcter = Constants.renderer.charcters[Constants.renderer.selectedCharacter];
                Constants.renderer.charcter2 = Constants.renderer.charcters[Constants.renderer.selectedCharacter];
                Constants.renderer.SetCharactersPosition();
            }
        }

        void Jump()
        {
            if (Acceleration.y > 0)
            {
                CurrentVelocity.y += -(Acceleration.y * 2);
                //Console.WriteLine("declerating Up :" + CurrentVelocity.y + "   " + Acceleration.y);
                //Console.WriteLine("Current Position : " + Position.y);
            }
            else
            {
                CurrentVelocity.y += -Acceleration.y - 0.05f;
                //Console.WriteLine("Down");
            }


            if ((Acceleration.y > 0 && (ReachedTarget() || CurrentVelocity.y <= 0)))
            {
                MoveToY(GroundY, 0.5f);
                //Console.WriteLine("Top");
                //Console.WriteLine("acc :" + Acceleration.y);
            }

            if (Acceleration.y < 0 && FinishedMovement.y == 1)
            {
                IsJumping = false;
                StartAnimation((animType)0);
                CurrentVelocity.y = 0;
                //Console.WriteLine("Ground");
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

            if (IsSuper && FinishedMovement.y == 1)
            {
                RingCount -= 0.5f;
                Console.WriteLine("RingCount : " + RingCount);
                Fly();
            }

            if (RingCount <= 0)
            {
                RingCount = 0;
                UndoSuper();
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
            SuperDirection = 1;
            IsSuper = false;
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
            RingCount = 0;
        }
    }
}
