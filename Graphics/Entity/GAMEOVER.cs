using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSonic3D.Entity
{
    class GAMEOVER
    {
        public int Selected;
        public Text3D[] Buttons;
        public float ButtonSize;
        public GAMEOVER()
        {
            Buttons = new Text3D[1];
            Buttons[0] = new Text3D("GAMEOVER", new vec3(-20, 1060, 0));
            Selected = 0;
            ButtonSize = Buttons[0].Scale;
        }

        public void HideMenu()
        {
            for (int i = 0; i < Buttons.Length; i++)
            {
                for (int j = 0; j < Buttons[i].Characters.Count; j++)
                {
                    Buttons[i].Characters[j].scalematrix = glm.scale(new mat4(1), new vec3(0, 0, 0));
                }
            }
        }

        public void ShowMenu()
        {
            for (int i = 0; i < Buttons.Length; i++)
            {
                for (int j = 0; j < Buttons[i].Characters.Count; j++)
                {
                    Buttons[i].Characters[j].scalematrix = glm.scale(new mat4(1), new vec3(ButtonSize, ButtonSize, ButtonSize));
                }
            }
        }

        public void Draw(int ModelID)
        {
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].Draw(ModelID);
            }
        }
        public void MoveUp()
        {
            Selected = (Selected - 1 + Buttons.Length) % Buttons.Length;
        }

        public void MoveDown()
        {
            Selected = (Selected + 1) % Buttons.Length;
        }

        public void UpdateMenu()
        {
            Buttons[Selected].UpdateMovement();
        }

        public void SetPositionZ(float PositionZ)
        {
            for (int i = 0; i < Buttons.Length; i++)
            {
                for (int j = 0; j < Buttons[i].Characters.Count; j++)
                {
                    Buttons[i].Characters[j].SetPostionZ(PositionZ);
                    Buttons[i].Characters[j].Move();
                }
            }
        }
    }
}
