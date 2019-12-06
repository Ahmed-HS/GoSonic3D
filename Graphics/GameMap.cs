using System;
using System.IO;
using GlmNet;
using GOSonic3D;

namespace GOSonic3D
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
            translateMatrix = glm.translate(new mat4(1), new vec3(0));

            rotationMatrix = glm.rotate((0f / 180f), new vec3(0, 1, 0));
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
            MapComponents[0].LoadFile(currPath, "road.obj", 3);
            MapComponents[1].LoadFile(currPath, "GroundTiles.obj", 3);
            MapComponents[2].LoadFile(currPath, "Islands.obj", 3);
            MapComponents[3].LoadFile(currPath, "HighRocks.obj", 3);
            MapComponents[4].LoadFile(currPath, "LowRocks.obj", 3);
            MapComponents[5].LoadFile(currPath, "MountainFall.obj", 3);
            MapComponents[6].LoadFile(currPath, "WaterFall.obj", 3);
            MapComponents[7].LoadFile(currPath,"Palms.obj", 3);
            MapComponents[8].LoadFile(currPath, "RoadSides.obj", 3);
            MapComponents[9].LoadFile(currPath, "HeadStarts.obj", 3);

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
