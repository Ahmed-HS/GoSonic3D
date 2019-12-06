using System;
using System.Collections.Generic;
using Tao.OpenGl;
using GlmNet;
using System.IO;
using GOSonic3D.Entity.Objects;
using GOSonic3D.Entity;

namespace GOSonic3D
{
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
        int EyePositionID;
        int AmbientLightID;
        int DataID;
        mat4 scaleMat;

        mat4 ProjectionMatrix;
        mat4 ViewMatrix;

        GameMap Map;

        public float Speed = 1;
        public Camera cam;
        public Md2Object Sonic;
        public Menu MainMenu;
        public bool PlayingGame;

        public void Initialize()
        {

            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");


            Sonic = new Md2Object(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Sonic.md2");
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
                1, 0, 0, //normal

                 0.0f, 0.0f, -1.0f,   //3
                0.0f, 1.0f, 0.0f, //G
                1, 0, 0, //normal

                 0.0f,  1.0f, -1.0f,  //4
                0.0f, 0.0f, 1.0f, //B
                1, 0, 0, //normal

                 0.0f,  1.0f, 0.0f,   //7
                0.0f, 1.0f, 0.0f, //G                          
                1, 0, 0, //normal
            };
            float[] gforward =
            {
                 0.0f, 0.0f, -1.0f,   //3
                0.0f, 1.0f, 0.0f, //G
                0, 1, 0, //normal

                 1.0f, 0.0f, -1.0f,   //2
                0.0f, 1.0f, 0.0f, //G
                0, 1, 0, //normal

                 1.0f,  1.0f, -1.0f,  //5
                0.0f, 0.0f, 1.0f, //R
                0, 1, 0, //normal

                 0.0f,  1.0f, -1.0f,  //4
                0.0f, 0.0f, 1.0f, //R
                0, 1, 0, //normal

            };
            float[] gright =
            {
                1.0f, 0.0f, -1.0f,   //2
                0.0f, 1.0f, 0.0f, //G   
                0, 1, 0, //normal

                1.0f, 0.0f, 0.0f,    //1
                0.0f, 1.0f, 0.0f, //G
                0, 1, 0, //normal

                1.0f,  1.0f, 0.0f,   //6
                0.0f, 0.0f, 1.0f, //R            
                0, 1, 0, //normal

                 1.0f,  1.0f, -1.0f,  //5
                0.0f, 0.0f, 1.0f, //R
                0, 1, 0, //normal
            };
            float[] gback =
            {
                 1.0f, 0.0f, 0.0f,    //1
                0.0f, 1.0f, 0.0f, //G
                0, 1, 0, //normal

                 0.0f, 0.0f,  0.0f,   //0
                0.0f, 1.0f, 0.0f, //G
                0, 1, 0, //normal

                 0.0f,  1.0f, 0.0f,   //7
                0.0f, 0.0f, 1.0f, //R
                0, 1, 0, //normal

                 1.0f,  1.0f, 0.0f,   //6
                0.0f, 0.0f, 1.0f, //R
                0, 1, 0, //normal

            };
            float[] gdown =
            {
                 0.0f, 0.0f,  0.0f,   //0
                0.0f, 1.0f, 0.0f, //G
                0, 1, 0, //normal

                 1.0f, 0.0f, 0.0f,    //1
                0.0f, 1.0f, 0.0f, //G
                0, 1, 0, //normal

                 1.0f, 0.0f, -1.0f,   //2
                0.0f, 1.0f, 0.0f, //G    
                0, 1, 0, //normal

                 0.0f, 0.0f, -1.0f,   //3
                0.0f, 1.0f, 0.0f, //G
                0, 1, 0, //normal

            };
            float[] gup =
            {
                   0.0f,  1.0f, -1.0f, //4
                0.0f, 0.0f, 1.0f, //R
                0, -1, 0, //normal

                 1.0f,  1.0f, -1.0f,  //5
                0.0f, 0.0f, 1.0f, //R
                0, -1, 0, //normal

                 1.0f,  1.0f, 0.0f,   //6
                0.0f, 0.0f, 1.0f, //R
                0, -1, 0, //normal

                 0.0f,  1.0f, 0.0f,   //7
                0.0f, 0.0f, 1.0f, //R                          
                0, -1, 0, //normal

            };


            List<float[]> vertCoord;
            vertCoord = new List<float[]>();

            vertCoord.Add(gleft);
            vertCoord.Add(gforward);
            vertCoord.Add(gright);
            vertCoord.Add(gback);
            vertCoord.Add(gup);
            vertCoord.Add(gdown);

            int scale = 75;
            for (int i = 0; i < NumOfSkyBoxFaces; i++)
            {
                for (int j = 0; j < vertCoord[i].Length; j++)
                {
                    if (j % 6 == 0)
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

            cam = new Camera();
            cam.Reset(0, 34, 55, 0, 0, 0, 1, 0, 0);

            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();

            scaleMat = glm.scale(new mat4(1), new vec3(2f, 2f, 2.0f));
            transID = Gl.glGetUniformLocation(sh.ID, "trans");
            projID = Gl.glGetUniformLocation(sh.ID, "projection");
            viewID = Gl.glGetUniformLocation(sh.ID, "view");

            sh.UseShader();

            //ambientLight
            //============
            vec3 ambientLight = new vec3(1f, 1f, 1f);
            AmbientLightID = Gl.glGetUniformLocation(sh.ID, "aL");
            Gl.glUniform3fv(AmbientLightID, 1, ambientLight.to_array());

            //LightPosition
            //==============
            int LightPositionID = Gl.glGetUniformLocation(sh.ID, "LightPosition_worldspace");
            vec3 lightPosition = new vec3(5.0f, 10f, 20.0f);
            //vec3 lightDirection = new vec3(1, 1, 1);
            Gl.glUniform3fv(LightPositionID, 1, lightPosition.to_array());

            //eye position.
            EyePositionID = Gl.glGetUniformLocation(sh.ID, "EyePosition_worldspace");

            //attenuation & specularExponent
            //==================================
            float attenuation = 100, specularExponent = 10;
            vec2 data = new vec2(attenuation, specularExponent);
            DataID = Gl.glGetUniformLocation(sh.ID, "data");
            Gl.glUniform2fv(DataID, 1, data.to_array());

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LESS);

            Map.SetScale(glm.scale(new mat4(1), new vec3(20, 20, 20)));
            Map.SetTranslat(glm.translate(new mat4(1), new vec3(0, 20, 0)));
        }

        public void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            sh.UseShader();

            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, scaleMat.to_array());
            Gl.glUniformMatrix4fv(projID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());
            Gl.glUniformMatrix4fv(viewID, 1, Gl.GL_FALSE, ViewMatrix.to_array());

            //Gl.glUniform3fv(EyePositionID, 1, cam.GetCameraPosition().to_array());


            for (int i = 0; i < 1; i++)
            {
                GPU.BindBuffer(vertexBufferIDs[4]);
                Gl.glEnableVertexAttribArray(0);
                Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 9 * sizeof(float), IntPtr.Zero);

                Gl.glEnableVertexAttribArray(1);
                Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 9 * sizeof(float), (IntPtr)(3 * sizeof(float)));

                Gl.glEnableVertexAttribArray(3);
                Gl.glVertexAttribPointer(2, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 9 * sizeof(float), (IntPtr)(6 * sizeof(float)));

                //6
                GPU.BindBuffer(SkyBoxBufferID[4]);
                Gl.glEnableVertexAttribArray(2);
                Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);
                
                SkyBox[4].Bind();
                ////7

                //Gl.glDrawElements(Gl.GL_QUADS, 8 * 3, Gl.GL_UNSIGNED_SHORT, IntPtr.Zero);
                //Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 3);
                //Gl.glDrawArrays(Gl.GL_TRIANGLES, 3, 3);

               Gl.glDrawArrays(Gl.GL_QUADS, 0, 4);
            }

            Sonic.Draw(transID);
            MainMenu.Draw(transID);
            //Map.Draw(transID);

        }
        public void Update(float deltaTime)
        {
            cam.UpdateViewMatrix();
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();
            Sonic.UpdateMovement();
            MainMenu.UpdateMenu();
        }
        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
