using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tao.OpenGl;
using GlmNet;
using Graphics._3D_Models;

namespace Graphics
{
    class GameMap
    {

        Model3D[] MapComponents;
        

        string currPath;
        int assestsCount;

        mat4 translateMatrix;
        mat4 rotationMatrix;
        mat4 scaleMatrix;


        public GameMap()
        {
            assestsCount = 10;
            MapComponents = new Model3D[assestsCount];
            /* 
             1: road
             2: road tiles
             3: small islands
             4: high rocks
             5: low rocks
             6: mountain fall
             7: water fall
             8: palms
             9: GroundSides
             */
            translateMatrix = glm.translate(new mat4(1), new vec3(0, 0, 0));
            rotationMatrix = glm.rotate((0f / 180f), new vec3(0, 0, 0));
            scaleMatrix = glm.scale(new mat4(1), new vec3(1, 1, 1));

            for(int i = 0;i < assestsCount;++i)
            {


                MapComponents[i] = new Model3D();
            }

            currPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\ModelFiles\\Models\\";

            Intialize();
        }
        void Intialize()
        {
            MapComponents[0].LoadFile(currPath, 3, "road.obj");
            MapComponents[1].LoadFile(currPath, 3, "GroundTiles.obj");
            MapComponents[2].LoadFile(currPath, 3, "Islands.obj");
            MapComponents[3].LoadFile(currPath, 3, "HighRocks.obj");
            MapComponents[4].LoadFile(currPath, 3, "LowRocks.obj");
            MapComponents[5].LoadFile(currPath, 3, "MountainFall.obj");
            MapComponents[6].LoadFile(currPath, 3, "WaterFall.obj");
            MapComponents[7].LoadFile(currPath, 3, "Palms.obj");
            MapComponents[8].LoadFile(currPath, 3, "RoadSides.obj");
            MapComponents[9].LoadFile(currPath, 3, "HeadStarts.obj");

        }

        public void SetTranslat(mat4 translation)
        {
            translateMatrix = translation;
            ApplyTransformation();
        }
        public void SetRoation(mat4 rotation)
        {
            rotationMatrix = rotation;
            ApplyTransformation();
        }
        public void SetScale(mat4 scale)
        {
            scaleMatrix = scale;
            ApplyTransformation();
        }

        void ApplyTransformation()
        {
            foreach(Model3D Component in MapComponents)
            {
                Component.rotmatrix = rotationMatrix;
                Component.scalematrix = scaleMatrix;
                Component.transmatrix = translateMatrix;
            }
        }

        public void Draw(int ID)
        {
            foreach(Model3D Component in MapComponents)
            {
                Component.Draw(ID);
            }
        }

    }
}
