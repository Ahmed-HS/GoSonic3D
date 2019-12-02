using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmNet;

namespace GOSonic3D.Entity.Objects
{
    public class MoveableObject
    {
        public vec3 Position;
        public vec3 Target;
        public vec3 CurrentVelocity;
        public vec3 IntialVelocity;
        public vec3 MaxVelocity;
        public vec3 Acceleration;
        public vec3 FinishedMovement;

        public MoveableObject(vec3 StartPosition)
        {
            Position = StartPosition;
            ChangeVelocity(new vec3(0.25f, 0.25f, 0.25f));
            ChangeAcceleration(new vec3(0.12f, 0.11f, 0.12f));
            ChangeMaxVelocity(new vec3(3, 3, 3));
            IntialVelocity = CurrentVelocity;
        }

        public MoveableObject()
        {
            Position = new vec3(0, 0, 0);
            ChangeVelocity(new vec3(0.25f, 0.25f, 0.25f));
            ChangeAcceleration(new vec3(0.12f, 0.11f, 0.12f));
            ChangeMaxVelocity(new vec3(3, 3, 3));
            IntialVelocity = CurrentVelocity;
        }

        public void MoveToX(float TargetX,float SpeedX=0)
        {
            this.Target.x = TargetX;
            FinishedMovement.x = 0;
            if (SpeedX == 0)
            {
                ChangeVelocityX(IntialVelocity.x);
            }
            else
            {
                ChangeVelocityX(SpeedX);
            }
            
            if (Target.x > Position.x)
            {
                if (CurrentVelocity.x < 0)
                {
                    CurrentVelocity.x *= -1;
                }
            }
            else
            {
                if (CurrentVelocity.x > 0)
                {
                    CurrentVelocity.x *= -1;
                }
            }

            if (Acceleration.x > 0 && CurrentVelocity.x < 0 || Acceleration.x < 0 && CurrentVelocity.x > 0)
            {
                Acceleration.x *= -1;
            }
        }

        public void MoveToY(float TargetY,float SpeedY=0)
        {
            this.Target.y = TargetY;
            FinishedMovement.y = 0;

            if (SpeedY == 0)
            {
                ChangeVelocityY(IntialVelocity.y);
            }
            else
            {
                ChangeVelocityY(SpeedY);
            }

            if (Target.y > Position.y)
            {
                if (CurrentVelocity.y < 0)
                {
                    CurrentVelocity.y *= -1;
                }
            }
            else
            {
                if (CurrentVelocity.y > 0)
                {
                    CurrentVelocity.y *= -1;
                }
            }

            if (Acceleration.y > 0 && CurrentVelocity.y < 0 || Acceleration.y < 0 && CurrentVelocity.y > 0)
            {
                Acceleration.y *= -1;
            }
        }

        public void MoveToZ(float TargetZ, float SpeedZ=0)
        {
            this.Target.z = TargetZ;
            FinishedMovement.z = 0;
            if (SpeedZ == 0)
            {
                ChangeVelocityZ(IntialVelocity.z);
            }
            else
            {
                ChangeVelocityZ(SpeedZ);
            }
            if (Target.z > Position.z)
            {
                if (CurrentVelocity.z < 0)
                {
                    CurrentVelocity.z *= -1;
                }
            }
            else
            {
                if (CurrentVelocity.z > 0)
                {
                    CurrentVelocity.z *= -1;
                }
            }
            if (Acceleration.z > 0 && CurrentVelocity.z < 0 || Acceleration.z < 0 && CurrentVelocity.z > 0)
            {
                Acceleration.z *= -1;
            }
        }

        public void MoveTo(vec3 Target)
        {
            MoveToX(Target.x);
            MoveToY(Target.y);
            MoveToZ(Target.z);
        }

        public void TranslateBy(vec3 Displacement)
        {
            MoveTo(new vec3(Position.x + Displacement.x, Position.y + Displacement.y, Position.z + Displacement.z));
        }

        public void ChangeVelocity(vec3 newSpeed)
        {
            CurrentVelocity = newSpeed;
        }
        public void ChangeVelocityX(float newX)
        {
            CurrentVelocity.x = newX;
        }
        public void ChangeVelocityY(float newY)
        {
            CurrentVelocity.y = newY;
        }
        public void ChangeVelocityZ(float newZ)
        {
            CurrentVelocity.z = newZ;
        }

        public void ChangeMaxVelocity(vec3 newSpeed)
        {
            MaxVelocity = newSpeed;
        }
        public void ChangeMaxVelocityX(float newX)
        {
            MaxVelocity.x = newX;
        }
        public void ChangeMaxVelocityY(float newY)
        {
            MaxVelocity.y = newY;
        }
        public void ChangeMaxVelocityZ(float newZ)
        {
            MaxVelocity.z = newZ;
        }

        public void ChangeAcceleration(vec3 newAcceleration)
        {
            Acceleration = newAcceleration;
        }
        public void ChangeAccelerationX(float newX)
        {
            Acceleration.x = newX;
        }
        public void ChangeAccelerationY(float newY)
        {
            Acceleration.y = newY;
        }
        public void ChangeAccelerationZ(float newZ)
        {
            Acceleration.z = newZ;
        }

        public void UpdatePositon()
        {
            if ((Target.x > Position.x && CurrentVelocity.x > 0) || (Target.x < Position.x && CurrentVelocity.x < 0))
            {
                Position.x += CurrentVelocity.x;
                if (Math.Abs(CurrentVelocity.x) < MaxVelocity.x)
                {
                    CurrentVelocity.x += Acceleration.x;
                }             
            }
            else
            {
                FinishedMovement.x = 1;
            }

            if ((Target.y > Position.y && CurrentVelocity.y > 0) || (Target.y < Position.y && CurrentVelocity.y < 0))
            {
                Position.y += CurrentVelocity.y;
                if (Math.Abs(CurrentVelocity.y) < MaxVelocity.y)
                {
                    CurrentVelocity.y += Acceleration.y;
                }
            }
            else
            {
                FinishedMovement.y = 1;
            }

            if ((Target.z > Position.z && CurrentVelocity.z > 0) || (Target.z < Position.z && CurrentVelocity.z < 0))
            {
                Position.z += CurrentVelocity.z;
                if (Math.Abs(CurrentVelocity.z) < MaxVelocity.z)
                {
                    CurrentVelocity.z += Acceleration.z;
                }
            }
            else
            {
                FinishedMovement.z = 1;
            }

        }

        public bool ReachedTarget()
        {
            if(FinishedMovement.x == 1 && FinishedMovement.y == 1 && FinishedMovement.z == 1)
            {
                return true;
            }

            return false;
        }

        public double DistanceFrom(MoveableObject Target)
        {
            return Math.Sqrt(Math.Pow(Position.x - Target.Position.x,2) + Math.Pow(Position.y - Target.Position.y, 2) + Math.Pow(Position.z - Target.Position.z, 2));
        }

    }
}
