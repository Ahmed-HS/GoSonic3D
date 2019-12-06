using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmNet;

namespace GOSonic3D.Entity
{
    class Text3D
    {

        public float[] AnimationTransitions;
        int CurrentTransition;
        bool Moving;
        public float Scale { get; }
        vec3 StartPosition;

        public List<Model3D> Characters;
        public Text3D(string Text,vec3 StartPosition)
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            Text = Text.ToUpper();
            Characters = new List<Model3D>();
            this.StartPosition = StartPosition*Constants.AspectRatio;
            Scale = 3 * Constants.AspectRatio;
            for (int i = 0; i < Text.Length; i++)
            {
                Model3D NewChar = new Model3D();
                NewChar.LoadFile(projectPath + "\\ModelFiles\\Text", Text[i].ToString() + ".obj", 2);
                NewChar.Position = StartPosition;
                if (i != 0)
                {
                    NewChar.Position.x = Characters[i - 1].Position.x + 5;
                }
 
                NewChar.scalematrix = glm.scale(new mat4(1), new vec3(Scale, Scale, Scale));
                NewChar.ChangeVelocity(new vec3(0.03f, 0.03f, 0.03f));
                NewChar.IntialVelocity = NewChar.CurrentVelocity;
                NewChar.ChangeAcceleration(new vec3(0.001f, 0.01f, 0.01f));
                NewChar.Move();
                Characters.Add(NewChar);
            }

            AnimationTransitions = new float[2];
            AnimationTransitions[0] = -3.5f;
            AnimationTransitions[1] = 3.5f;
            CurrentTransition = 0;
            Moving = false;
            //Characters[0].MoveToX(Characters[0].Position.x + AnimationTransitions[CurrentTransition]);

        }
        public void Draw(int ModelID)
        {
            for (int i = 0; i < Characters.Count; i++)
            {
                Characters[i].Draw(ModelID);
            }
        }
        public  void UpdateMovement()
        {
            if (!Moving)
            {
                for (int i = 0; i < Characters.Count; i++)
                {
                    //Characters[i].TranslateBy(new vec3(AnimationTransitions[CurrentTransition], 0, 0));
                    Characters[i].MoveToX(Characters[i].Position.x + AnimationTransitions[CurrentTransition]);
                }
                CurrentTransition = (CurrentTransition + 1) % AnimationTransitions.Length;
                Moving = true;
            }
            else if (Characters[0].ReachedTarget())
            {
                //Characters[0].MoveToX(Characters[0].Position.x + AnimationTransitions[CurrentTransition]*2);
                Moving = false;
            }
            else
            {
                for (int i = 0; i < Characters.Count; i++)
                {
                    Characters[i].CurrentVelocity.x += -(Characters[i].Acceleration.x * 2);
                }
            }

            for (int i = 0; i < Characters.Count; i++)
            {
                Characters[i].UpdatePositon();
                Characters[i].Move();
            }
        }
    }
}
