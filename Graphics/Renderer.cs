using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public static System.Media.SoundPlayer GameSound;
        public static bool PlayingGame = false;
        public static Menu MainMenu;
    }

    class Renderer
    {
        Shader shader;
        Shader skyboxShader;
        uint cubeVBO, skyboxVBO;
        Texture cubeTexture;
        Texture cubemapTexture;
        vec3 lightPos;
        bool forward = true;

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

        GameMap []Map;
        Enemy []Enemies;
        public void Initialize()
        {

            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");

            Sonic = new Character(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Sonic.md2");

            Enemies = new Enemy[3];
            for (int i = 0; i < Enemies.Length; i++)
            {
                Enemies[i] = new Enemy(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Shadow.md2", Sonic);
            }

            Constants.GameSound = new System.Media.SoundPlayer(projectPath + "\\Audio\\2.wav");
            Constants.GameSound.PlayLooping();

            Constants.MainMenu = new Menu();
            Map = new GameMap[2];
            Map[0] = new GameMap(new vec3(45, 588, -350));
            Map[0].IsMoving = true;
            Map[1] = new GameMap(new vec3(45, 588, Map[0].Position.z -700));
            Map[1].IsMoving = false;




            Gl.glEnable(Gl.GL_DEPTH_TEST);
            lightPos = new vec3(1.2f, 1.0f, -5.0f);
            lightPos = new vec3(0, 0, 0);

            shader = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader",
                                projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");

            modelID = Gl.glGetUniformLocation(shader.ID, "model");

            cam = new Camera();
            scaleMat = glm.scale(new mat4(1), new vec3(2f, 2f, 2.0f));
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();

            scaleMat = glm.scale(new mat4(1), new vec3(2f, 2f, 2.0f));
            transID = Gl.glGetUniformLocation(shader.ID, "model");
            projID = Gl.glGetUniformLocation(shader.ID, "projection");
            viewID = Gl.glGetUniformLocation(shader.ID, "view");


            skyboxShader = new Shader(projectPath + "\\Shaders\\6.2.skybox.vs",
                                      projectPath + "\\Shaders\\6.2.skybox.fs");

            float[] cubeVertices =
            {
                // positions          // normals
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 0.0f, 0.0f,
                 0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 1.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 1.0f, 1.0f,
                -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 0.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 0.0f, 0.0f,

                -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,  0.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,  1.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,  1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,  1.0f, 1.0f,
                -0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,  0.0f, 1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,  0.0f, 0.0f,

                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f, 1.0f, 0.0f,
                -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f, 1.0f, 1.0f,
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f, 0.0f, 1.0f,
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f, 0.0f, 1.0f,
                -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f, 0.0f, 0.0f,
                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f, 1.0f, 0.0f,

                 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f, 1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f, 1.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f, 0.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f, 0.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f, 0.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f, 1.0f, 0.0f,

                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, 0.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, 1.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f, 1.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f, 1.0f, 0.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f, 0.0f, 0.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, 0.0f, 1.0f,

                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, 0.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, 1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f, 1.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f, 1.0f, 0.0f,
                -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f, 0.0f, 0.0f,
                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, 0.0f, 1.0f
            };
            float[] skyboxVertices =
            {
                // positions          
                -1.0f,  1.0f, -1.0f,
                -1.0f, -1.0f, -1.0f,
                 1.0f, -1.0f, -1.0f,
                 1.0f, -1.0f, -1.0f,
                 1.0f,  1.0f, -1.0f,
                -1.0f,  1.0f, -1.0f,

                -1.0f, -1.0f,  1.0f,
                -1.0f, -1.0f, -1.0f,
                -1.0f,  1.0f, -1.0f,
                -1.0f,  1.0f, -1.0f,
                -1.0f,  1.0f,  1.0f,
                -1.0f, -1.0f,  1.0f,

                 1.0f, -1.0f, -1.0f,
                 1.0f, -1.0f,  1.0f,
                 1.0f,  1.0f,  1.0f,
                 1.0f,  1.0f,  1.0f,
                 1.0f,  1.0f, -1.0f,
                 1.0f, -1.0f, -1.0f,

                -1.0f, -1.0f,  1.0f,
                -1.0f,  1.0f,  1.0f,
                 1.0f,  1.0f,  1.0f,
                 1.0f,  1.0f,  1.0f,
                 1.0f, -1.0f,  1.0f,
                -1.0f, -1.0f,  1.0f,

                -1.0f,  1.0f, -1.0f,
                 1.0f,  1.0f, -1.0f,
                 1.0f,  1.0f,  1.0f,
                 1.0f,  1.0f,  1.0f,
                -1.0f,  1.0f,  1.0f,
                -1.0f,  1.0f, -1.0f,

                -1.0f, -1.0f, -1.0f,
                -1.0f, -1.0f,  1.0f,
                 1.0f, -1.0f, -1.0f,
                 1.0f, -1.0f, -1.0f,
                -1.0f, -1.0f,  1.0f,
                 1.0f, -1.0f,  1.0f
            };

            cubeVBO = GPU.GenerateBuffer(cubeVertices);
            skyboxVBO = GPU.GenerateBuffer(skyboxVertices);

            cubeTexture = new Texture(projectPath + "\\Textures\\wood.png", 2);

            List<string> faces = new List<string>();
            faces.Add(projectPath + "\\Textures\\right.jpg");
            faces.Add(projectPath + "\\Textures\\left.jpg");
            faces.Add(projectPath + "\\Textures\\top.jpg");
            faces.Add(projectPath + "\\Textures\\bottom.jpg");
            faces.Add(projectPath + "\\Textures\\front.jpg");
            faces.Add(projectPath + "\\Textures\\back.jpg");

            cubemapTexture = new Texture(faces, 3);

            shader.UseShader();
            shader.setInt("texture1", 0);

            skyboxShader.UseShader();
            skyboxShader.setInt("skybox", 0);
        }

        public void Draw()
        {
            Gl.glClearColor(0, 0, 0.4f, 1);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            shader.UseShader();

            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, scaleMat.to_array());
            Gl.glUniformMatrix4fv(projID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());
            Gl.glUniformMatrix4fv(viewID, 1, Gl.GL_FALSE, ViewMatrix.to_array());

            shader.setVec3("viewPos", cam.GetCameraPosition());

            if (lightPos.z < -20)
                forward = false;
            else
            if (lightPos.z > 20)
                forward = true;

            if (forward)
                lightPos = new vec3(0, 0, lightPos.z - .01f);
            else
                lightPos = new vec3(0, 0, lightPos.z + .01f);


            shader.setVec3("lightPos", lightPos);
            shader.setInt("blinn", 1);

            Sonic.Draw(modelID);
            Constants.MainMenu.Draw(modelID);
            for (int i = 0; i < Enemies.Length; i++)
            {
                Enemies[i].Draw(modelID);
            }
            Map[0].Draw(modelID);
            Map[1].Draw(modelID);



            Gl.glDepthFunc(Gl.GL_LEQUAL);

            skyboxShader.UseShader();
            float[] v3 = cam.GetViewMatrix().to_mat3().to_array();

            mat4 v4 = new mat4(0);
            int k = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    v4[i, j] = v3[k++];
                }
            }

            ViewMatrix = v4; // remove translation from the view matrix
            skyboxShader.setMat4("view", ViewMatrix);
            skyboxShader.setMat4("projection", ProjectionMatrix);

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, skyboxVBO);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 3 * sizeof(float), IntPtr.Zero);

            cubemapTexture.Bind(true);
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 36);
            Gl.glDepthFunc(Gl.GL_LESS);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(2);
            Gl.glDisableVertexAttribArray(3);
        }

        void MoveMap()
        {

            if (Map[0].FinishedMap(Map[0].StartZ == Map[1].StartZ ? 2: 1))
            {
                Map[0].SetPositionZ(Map[1].StartZ);
                Map[0].StartZ = Map[1].StartZ;
                Console.WriteLine("Map0");
            }

            if (Map[1].FinishedMap(2))
            {
                Map[1].SetPositionZ(Map[1].StartZ);
                Map[1].StartZ = Map[0].StartZ;
                Console.WriteLine("Map1");
            }

            Task.Run(() => Map[0].Move());
            Task.Run(() => Map[1].Move());

        }

        public void Update(float deltaTime)
        {
            cam.UpdateViewMatrix();
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();
            Sonic.UpdateMovement();
            for (int i = 0; i < Enemies.Length; i++)
            {
                Enemies[i].UpdateMovement();
            }
            Task.Run(() => MoveMap());

            Task.Run(() => Constants.MainMenu.UpdateMenu());
        }

        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
