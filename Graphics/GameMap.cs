using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tao.OpenGl;
using GlmNet;
using GOSonic3D._3D_Models;
using GOSonic3D.Entity.Objects;

namespace GOSonic3D
{
    class GameMap : MoveableObject
    {

        Model3D[] MapComponents;
        

        string currPath;
        int assestsCount;

        mat4 translateMatrix;
        mat4 rotationMatrix;
        mat4 scaleMatrix;

        public const float MapLength = 1760;
        public float StartZ;
        public bool IsMoving;
        public GameMap(vec3 MapPosition)
        {
            assestsCount = 9;
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
            Position = MapPosition * Constants.AspectRatio;
            translateMatrix = glm.translate(new mat4(1), Position);
            rotationMatrix = glm.rotate((-90f / 180f) * (float)Math.PI, new vec3(0, 1, 0));
            scaleMatrix = glm.scale(new mat4(1), new vec3(200, 200, 200) * Constants.AspectRatio);

            for(int i = 0;i < assestsCount;++i)
            {
                MapComponents[i] = new Model3D();
            }

            currPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\ModelFiles\\Models\\";

            Intialize();
            ApplyTransformation();
            StartZ = Position.z;
        }
        public void Move()
        {
            UpdatePositon();
            TranslateByZ(20,2);
            translateMatrix = glm.translate(new mat4(1), Position);
            ApplyTransformation();
            Console.WriteLine("Length: " + (Position.z - StartZ));
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
            //MapComponents[7].LoadFile(currPath, "Palms.obj", 3);
            MapComponents[7].LoadFile(currPath, "RoadSides.obj", 3);
            MapComponents[8].LoadFile(currPath, "HeadStarts.obj", 3);

        }
        public bool FinishedMap(int Count = 1)
        {
            return Position.z - StartZ >= MapLength*Count;
        }
        public void SetPositionZ(float NewZ)
        {
            Position.z = NewZ;
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
