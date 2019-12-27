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
using System.Media;

namespace GOSonic3D
{
    enum MenuType
    {
        main = 1,
        choosep1 = 2,
        choosep2 = 3,
        play = 4
    }
    enum PlayType
    {
        single = 1,
        multi = 2,
        no = 3
    }
    static class Constants
    {
        public static float AspectRatio;
        public static System.Media.SoundPlayer GameSound;
        public static bool SelectScreen = false;
        public static bool PlayingGame = false;
        public static Menu MainMenu;
        public static ChoosePlayerMenu choosep1, choosep2;
        public static GAMEOVER gameover;
        public static Renderer renderer;
        public static PlayType playType = PlayType.no;
        public static MenuType currentMen = MenuType.main;
        public static bool isGameOver = false;
        public static Kernel.Sound s;
        public static SoundPlayer SoundEffects { get; internal set; }
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

        Model3D[] Chracters;
        Shader selectCharacter;
        float rotate = 0;
        public int selectedCharacter = 0;
        public int numOfCharacters = 4;
        public int characterSelected = 0;
        public Character[] charcters;
        vec3 StartSelectPosition;

        public static float x = 0;
        Shader sh;
        int transID;
        int modelID;
        int viewID;
        int projID;

        mat4 scaleMat;
        mat4 ProjectionMatrix;
        mat4 ViewMatrix;

        public float Speed = 1;
        public Camera cam;
        public Character charcter;
        public Character charcter2;

        GameMap[] Map;
        public GroundedObject[] Enemies;
        public GroundedObject[] Rings;
        public void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");

            cam = new Camera();

            Constants.s = new Kernel.Sound(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\Audio\\Ring.wav"); ;

            charcters = new Character[numOfCharacters * 2];
            charcters[0] = new Character(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Sonic.md2");
            charcters[1] = new Character(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Tails.md2");
            charcters[2] = new Character(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Knuckles.md2");
            charcters[3] = new Character(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Shadow.md2");

            charcters[0 + numOfCharacters] = new Character(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Sonic.md2");
            charcters[1 + numOfCharacters] = new Character(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Tails.md2");
            charcters[2 + numOfCharacters] = new Character(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Knuckles.md2");
            charcters[3 + numOfCharacters] = new Character(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Shadow.md2");

            charcter = charcters[0];
            charcter2 = charcters[0 + numOfCharacters];

            Enemies = new GroundedObject[6];
            Rings = new GroundedObject[20];
            for (int i = 0; i < Enemies.Length; i++)
            {
                Enemies[i] = new GroundedObject(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Shadow.md2", new Character[] { charcter, charcter2 }, cam, GroundedObject.Type.Enemy);
            }

            for (int i = 0; i < Rings.Length; i++)
            {
                Rings[i] = new GroundedObject(projectPath + "\\ModelFiles\\animated\\md2\\Sonic\\Ring.md2", new Character[] { charcter, charcter2 }, cam, GroundedObject.Type.Ring);
            }

            Kernel.Sound s = new Kernel.Sound(projectPath + "\\Audio\\2.wav");
            s.Play(true);

            Constants.MainMenu = new Menu();
            Constants.choosep1 = new ChoosePlayerMenu();
            Constants.choosep2 = new ChoosePlayerMenu();
            Constants.gameover = new GAMEOVER();

            Map = new GameMap[2];
            Map[0] = new GameMap(new vec3(45, 588, -350), cam);
            Map[0].IsMoving = true;
            Map[1] = new GameMap(new vec3(45, 588, Map[0].Position.z - 700), cam);
            Map[1].IsMoving = false;

            selectCharacter = new Shader(projectPath + "\\Shaders\\SimpleVertexShader2.vertexshader",
                                         projectPath + "\\Shaders\\SimpleFragmentShader2.fragmentshader");


            Chracters = new Model3D[4];
            for (int i = 0; i < numOfCharacters; i++)
            {
                Chracters[i] = new Model3D();
            }

            Chracters[0].LoadFile(projectPath + "\\ModelFiles\\static\\SelectCharacter", "Sonic1.obj", 4);
            Chracters[1].LoadFile(projectPath + "\\ModelFiles\\static\\SelectCharacter", "Tails1.obj", 4);
            Chracters[2].LoadFile(projectPath + "\\ModelFiles\\static\\SelectCharacter", "Knuckles1.obj", 4);
            Chracters[3].LoadFile(projectPath + "\\ModelFiles\\static\\SelectCharacter", "Shadow1.obj", 4);

            StartSelectPosition = new vec3(0, 0, 5);
            StartSelectPosition = charcter.Position;
            for (int i = 0; i < 4; i++)
            {
                Chracters[i].transmatrix = glm.translate(new mat4(1), StartSelectPosition);
                Chracters[i].rotmatrix = glm.rotate((0f / 180f) * (float)Math.PI, new vec3(0, 1, 0));
                Chracters[i].scalematrix = glm.scale(new mat4(1), new vec3(1, 1, 1));
            }


            Gl.glEnable(Gl.GL_DEPTH_TEST);
            lightPos = new vec3(1.2f, 1.0f, -5.0f);
            lightPos = new vec3(0, 0, 0);

            shader = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader",
                                projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");

            modelID = Gl.glGetUniformLocation(shader.ID, "model");


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

        public void SetCharactersPosition()
        {
            StartSelectPosition.z = charcter.Position.z;
            charcter.Position = StartSelectPosition;
            charcter2.Position = StartSelectPosition;

            for (int i = 0; i < 4; i++)
            {
                Chracters[i].transmatrix = glm.translate(new mat4(1), StartSelectPosition);
                Chracters[i].rotmatrix = glm.rotate((0f / 180f) * (float)Math.PI, new vec3(0, 1, 0));
                Chracters[i].scalematrix = glm.scale(new mat4(1), new vec3(1, 1, 1));
            }
        }

        public void SwitchCharacter()
        {
            if (characterSelected == 0)
            {
                charcters[selectedCharacter].Position = charcter.Position;
                charcter = charcters[selectedCharacter];
            }

            if (characterSelected == 1)
            {
                charcters[selectedCharacter + numOfCharacters].Position = charcter2.Position;
                charcter2 = charcters[selectedCharacter + numOfCharacters];
            }
        }

        public void Draw()
        {
            Gl.glClearColor(0, 0, 0.4f, 1);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            if (Constants.SelectScreen && characterSelected < 2)
            {
                selectCharacter.UseShader();
                Gl.glUniformMatrix4fv(modelID, 1, Gl.GL_FALSE, scaleMat.to_array());
                Gl.glUniformMatrix4fv(projID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());
                Gl.glUniformMatrix4fv(viewID, 1, Gl.GL_FALSE, ViewMatrix.to_array());

                for (int i = 0; i < 4; i++)
                {
                    Chracters[i].rotmatrix = glm.rotate((rotate += .05f / 180f) * (float)Math.PI, new vec3(0, 1, 0));
                }

                Chracters[selectedCharacter].Draw(modelID);
            }

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


            charcter.Draw(modelID);
            //if (Constants.playType == PlayType.multi)
            charcter2.Draw(modelID);

            Constants.MainMenu.Draw(modelID);
            Constants.choosep1.Draw(modelID);
            Constants.choosep2.Draw(modelID);
            Constants.gameover.Draw(modelID);


            if (Constants.currentMen == MenuType.main)
            {
                Constants.MainMenu.ShowMenu();
                Constants.choosep1.HideMenu();
                Constants.choosep2.HideMenu();
                if (Constants.isGameOver)
                    Constants.gameover.ShowMenu();
                else
                    Constants.gameover.HideMenu();
            }
            else if (Constants.currentMen == MenuType.choosep1)
            {
                Constants.MainMenu.HideMenu();
                Constants.choosep1.ShowMenu();
                Constants.choosep2.HideMenu();
                if (Constants.isGameOver)
                    Constants.gameover.ShowMenu();
                else
                    Constants.gameover.HideMenu();
            }
            else if (Constants.currentMen == MenuType.choosep2)
            {
                Constants.MainMenu.HideMenu();
                Constants.choosep1.HideMenu();
                Constants.choosep2.ShowMenu();
                if (Constants.isGameOver)
                    Constants.gameover.ShowMenu();
                else
                    Constants.gameover.HideMenu();
            }

            for (int i = 0; i < Enemies.Length; i++)
            {
                Enemies[i].Draw(modelID);
            }
            Map[0].Draw(modelID);
            Map[1].Draw(modelID);

            for (int i = 0; i < Rings.Length; i++)
            {
                Rings[i].Draw(modelID);
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
            if (Map[0].FinishedMap())
            {
                Map[0].ResesMap();
            }

            if (Map[1].FinishedMap())
            {
                Map[1].ResesMap();
            }
        }

        public void Update(float deltaTime)
        {
            cam.UpdateViewMatrix();
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();

            if (Constants.PlayingGame)
            {
                charcter.UpdateMovement();
                if (Constants.playType == PlayType.multi)
                    charcter2.UpdateMovement();
                cam.UpdateMovement();
                for (int i = 0; i < Enemies.Length; i++)
                {
                    Enemies[i].UpdateMovement();
                }

                for (int i = 0; i < Rings.Length; i++)
                {
                    Rings[i].UpdateMovement();
                }

                //MoveMap();
                Task.Run(() => MoveMap());
            }
            else
            {
                //Constants.MainMenu.UpdateMenu();
                if (Constants.currentMen == MenuType.main)
                    Task.Run(() => Constants.MainMenu.UpdateMenu());
                else if (Constants.currentMen == MenuType.choosep1)
                    Task.Run(() => Constants.choosep1.UpdateMenu());
                else if (Constants.currentMen == MenuType.choosep2)
                    Task.Run(() => Constants.choosep2.UpdateMenu());
            }
        }

        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
