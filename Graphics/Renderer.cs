using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using GlmNet;
using System.IO;
using GOSonic3D._3D_Models;
using GOSonic3D.Entity.Objects;
using GOSonic3D.Entity;

namespace GOSonic3D
{
    static class Constants
    {
        public static float AspectRatio;
    }
    class Renderer
    {
        //0.1
        uint[] vertexBufferIDs;
        //1.1
        uint[] SkyBoxBufferID;
        //2.1
        Texture[] SkyBox;
        int NumOfSkyBoxFaces;

        public static float x = 0;
        Shader sh;
        int transID;
        int modelID;
        int viewID;
        int projID;
        mat4 scaleMat;
        int EyePositionID;

        mat4 ProjectionMatrix;
        mat4 ViewMatrix;

        public float Speed = 1;
        public Camera cam;
        public Character Sonic;
        public Menu MainMenu;
        GameMap Map;
        
        public bool PlayingGame;
        public void Initialize()
        {

            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");


            Sonic = new Character(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Sonic.md2");

            MainMenu = new Menu();
            Map = new GameMap();

            PlayingGame = false;

            //3.1
            NumOfSkyBoxFaces = 6;
            SkyBox = new Texture[NumOfSkyBoxFaces];
            for (int i = 0; i < NumOfSkyBoxFaces; i++)
            {
                SkyBox[i] = new Texture(projectPath + "\\Textures\\" + (i + 1).ToString() + ".jpg", i + 1, false);
            }
        

            float[] vertTexCoord =
            {
                0,1,
                1,1,
                1,0,
                0,0,
            };

            //4.2
            float[] gleft = {
                 0.0f, 0.0f,  0.0f,   //0
                0.0f, 0.0f, 1.0f, //R
                 0.0f, 0.0f, -1.0f,   //3
                0.0f, 1.0f, 0.0f, //G
                 0.0f,  1.0f, -1.0f,  //4
                0.0f, 0.0f, 1.0f, //B
                 0.0f,  1.0f, 0.0f,   //7
                0.0f, 1.0f, 0.0f, //G                          
            };
            float[] gforward =
            {
                 0.0f, 0.0f, -1.0f,   //3
                0.0f, 1.0f, 0.0f, //G
                 1.0f, 0.0f, -1.0f,   //2
                0.0f, 1.0f, 0.0f, //G
                 1.0f,  1.0f, -1.0f,  //5
                0.0f, 0.0f, 1.0f, //R
                 0.0f,  1.0f, -1.0f,  //4
                0.0f, 0.0f, 1.0f, //R
            };
            float[] gright =
            {
                1.0f, 0.0f, -1.0f,   //2
                0.0f, 1.0f, 0.0f, //G   
                1.0f, 0.0f, 0.0f,    //1
                0.0f, 1.0f, 0.0f, //G
                1.0f,  1.0f, 0.0f,   //6
                0.0f, 0.0f, 1.0f, //R                             
                 1.0f,  1.0f, -1.0f,  //5
                0.0f, 0.0f, 1.0f, //R
            };
            float[] gback =
            {
                 1.0f, 0.0f, 0.0f,    //1
                0.0f, 1.0f, 0.0f, //G
                 0.0f, 0.0f,  0.0f,   //0
                0.0f, 1.0f, 0.0f, //G
                 0.0f,  1.0f, 0.0f,   //7
                0.0f, 0.0f, 1.0f, //R
                 1.0f,  1.0f, 0.0f,   //6
                0.0f, 0.0f, 1.0f, //R
            };
            float[] gdown =
            {
                 0.0f, 0.0f,  0.0f,   //0
                0.0f, 1.0f, 0.0f, //G
                 1.0f, 0.0f, 0.0f,    //1
                0.0f, 1.0f, 0.0f, //G
                 1.0f, 0.0f, -1.0f,   //2
                0.0f, 1.0f, 0.0f, //G    
                 0.0f, 0.0f, -1.0f,   //3
                0.0f, 1.0f, 0.0f, //G
            };
            float[] gup =
            {
                   0.0f,  1.0f, -1.0f, //4
                0.0f, 0.0f, 1.0f, //R
                 1.0f,  1.0f, -1.0f,  //5
                0.0f, 0.0f, 1.0f, //R

                 1.0f,  1.0f, 0.0f,   //6
                0.0f, 0.0f, 1.0f, //R
                 0.0f,  1.0f, 0.0f,   //7
                0.0f, 0.0f, 1.0f, //R                          
            };

            List<float[]> vertCoord;
            vertCoord = new List<float[]>();

            vertCoord.Add(gleft);
            vertCoord.Add(gforward);
            vertCoord.Add(gright);
            vertCoord.Add(gback);
            vertCoord.Add(gup);
            vertCoord.Add(gdown);

            float scale = 1000*Constants.AspectRatio;
            for(int i = 0; i < NumOfSkyBoxFaces; i++)
            {
                for(int j = 0; j < vertCoord[i].Length; j++)
                {
                    if(j % 6 == 0)
                    {
                        vertCoord[i][j] -= .5f;                   
                    }

                    if ((j - 2) % 6 == 0)
                    {
                        vertCoord[i][j] += .5f;
                    }

                    vertCoord[i][j] *= scale;
                }
            }

            vertexBufferIDs = new uint[NumOfSkyBoxFaces];
            for (int i = 0; i < NumOfSkyBoxFaces; i++)
            {
                vertexBufferIDs[i] = GPU.GenerateBuffer(vertCoord[i]);
            }

            //5.1
            SkyBoxBufferID = new uint[NumOfSkyBoxFaces];
            for (int i = 0; i < NumOfSkyBoxFaces; i++)
            {
                SkyBoxBufferID[i] = GPU.GenerateBuffer(vertTexCoord);
            }


            Gl.glClearColor(0, 0, 0.4f, 1);

            modelID = Gl.glGetUniformLocation(sh.ID, "model");
          
            cam = new Camera();

            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();

            scaleMat = glm.scale(new mat4(1), new vec3(2f, 2f, 2.0f));
            transID = Gl.glGetUniformLocation(sh.ID, "model");
            projID = Gl.glGetUniformLocation(sh.ID, "projection");
            viewID = Gl.glGetUniformLocation(sh.ID, "view");

            sh.UseShader();

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LESS);
        }

        public void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT|Gl.GL_DEPTH_BUFFER_BIT);
            sh.UseShader();

            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, scaleMat.to_array());
            Gl.glUniformMatrix4fv(projID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());
            Gl.glUniformMatrix4fv(viewID, 1, Gl.GL_FALSE, ViewMatrix.to_array());

            //Gl.glUniform3fv(EyePositionID, 1, cam.GetCameraPosition().to_array());


            for (int i = 0; i < NumOfSkyBoxFaces; i++)
            {
                GPU.BindBuffer(vertexBufferIDs[i]);
                Gl.glEnableVertexAttribArray(0);
                Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), IntPtr.Zero);
                Gl.glEnableVertexAttribArray(1);
                Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));
                //6
                GPU.BindBuffer(SkyBoxBufferID[i]);
                Gl.glEnableVertexAttribArray(2);
                Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);
                ////7
                SkyBox[i].Bind();
                //Gl.glDrawElements(Gl.GL_QUADS, 8 * 3, Gl.GL_UNSIGNED_SHORT, IntPtr.Zero);
                //Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
                Gl.glDrawArrays(Gl.GL_QUADS, 0, 4);
            }

            Sonic.Draw(modelID);
            MainMenu.Draw(modelID);
            Map.Draw(modelID);

        }
        public void Update(float deltaTime)
        {
            cam.UpdateViewMatrix();
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();
            Sonic.UpdateMovement();
            Map.Move();
            MainMenu.UpdateMenu();
        }
        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
