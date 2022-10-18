using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FluidSimulation
{
    public class Hellper
    {
        static public int From3DTo1D(int x, int y, int z, int dim)
        {
            return (z * dim * dim) + (y * dim) + x;
        }

        static public int[] From1DTo3D(int i, int dim)
        {
            int z = i / (dim * dim);
            i -= (z * dim * dim);
            int y = i / dim;
            int x = i % dim;
            return new int[] { x, y, z };
        }

        static public float getRandomArbitrary(float min, float max)
        {
            return Random.value * (max - min) + min;
        }

        static public Vector2 MousePositionInWorldSpace()
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 v = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
            return new Vector2(v.x, v.y);
        }

#if UNITY_EDITOR
        public static Vector2 getMousPositionOnScene(Event guiEvent)
        {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
            float drawPlaneHeight = 0;
            float dstToDrawPlane = (drawPlaneHeight - mouseRay.origin.z) / mouseRay.direction.z;

            return mouseRay.GetPoint(dstToDrawPlane);
        }


        public static Vector2 getMousPositionOnScene()
        {
            return HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
        }
#endif



        static public int From2To1Index(int x, int y, int xDim)
        {
            return y * xDim + x;
        }

        public static Vector2Int From1To2Index(int i, int xLength)
        {
            var x = i % xLength;
            var y = i / xLength;

            return new Vector2Int(x, y);
        }


        static public Vector2Int Position2GridIndex(Vector2 position, Vector2 spaceSize, float size)
        {
            var x = (int)((position.x + spaceSize.x / 2) / size);
            var y = (int)((position.y + spaceSize.y / 2) / size);

            return new Vector2Int(x, y);
        }

        static public Vector2Int Position2GridIndex1(Vector2 position, float width, float height, float size)
        {
            var x = Mathf.FloorToInt((position.x + width / 2) / size);
            var y = Mathf.FloorToInt((position.y + height / 2) / size);

            return new Vector2Int(x, y);
        }


        static public void Bin(float[,] map, Vector2Int pos, int radius, int type)
        {
            for (int neighbourX = pos.x - radius; neighbourX <= pos.x + radius; neighbourX++)
            {
                for (int neighbourY = pos.y - radius; neighbourY <= pos.y + radius; neighbourY++)
                {
                    if (neighbourX >= 0 && neighbourX < map.GetLength(0) && neighbourY >= 0 && neighbourY < map.GetLength(1))
                    {
                        map[neighbourX, neighbourY] = type;
                    }
                }
            }
        }


        static public void Bin(float[] map, Vector2Int dim, Vector2Int pos, int radius, int type)
        {
            for (int neighbourX = pos.x - radius; neighbourX <= pos.x + radius; neighbourX++)
            {
                for (int neighbourY = pos.y - radius; neighbourY <= pos.y + radius; neighbourY++)
                {
                    if (neighbourX >= 0 && neighbourX < dim.x && neighbourY >= 0 && neighbourY < dim.y)
                    {
                        var index = From2To1Index(neighbourX, neighbourY, dim.x);
                        map[index] = type;
                    }
                }
            }
        }



        static public List<Vector2Int> GetIndexesFromArray(Vector2Int mapSize, Vector2Int gridPos, int radius)
        {
            List<Vector2Int> posIndexes = new List<Vector2Int>();
            for (int x = gridPos.x - radius; x <= gridPos.x + radius; x++)
            {
                for (int y = gridPos.y - radius; y <= gridPos.y + radius; y++)
                {
                    if (x >= 0 && x < mapSize.x && y >= 0 && y < mapSize.y)
                    {
                        posIndexes.Add(new Vector2Int(x, y));
                    }
                }
            }

            return posIndexes;
        }

        static public Vector2Int[,] GetIndexesFrom2DArray(Vector2Int pixelPos, int radius)
        {
            Vector2Int[,] posIndexes = new Vector2Int[radius*2+1, radius * 2 + 1];

            int xCount = 0;
            for (int x = pixelPos.x - radius; x <= pixelPos.x + radius; x++)
            {
                int yCount = 0;
                for (int y = pixelPos.y - radius; y <= pixelPos.y + radius; y++)
                {
                    posIndexes[xCount, yCount] = new Vector2Int(x, y);
                    yCount++;
                }
                xCount++;
            }

            return posIndexes;
        }




       

        public static object[] Convert2DArrayTo1(object[,] map)
        {
            object[] arr = new object[map.Length];


            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    var index = From2To1Index(x, y, map.GetLength(0));
                    arr[index] = map[x, y];
                }
            }

            return arr;
        }


        public static object[,] Convert1DArrayTo2(object[] array, Vector2Int dim)
        {
            object[,] arr = new object[dim.x, dim.y];

            for (int i = 0; i < array.Length; i++)
            {
                var index = From1To2Index(i, dim.x);
                arr[index.x, index.y] = array[i];
            }

            return arr;
        }

        public static void CheckBoundary(Vector2 boundry, PhysicsOpject[] balls, float radius, float damping)
        {
            foreach (var ball in balls)
            {
                if (ball.position.x > boundry.x - radius)
                {
                    ball.velocity.x *= damping;
                    ball.position = new Vector2((boundry.x - radius), ball.position.y);
                }
                if (ball.position.x < -(boundry.x - radius))
                {
                    ball.velocity.x *= damping;
                    ball.position = new Vector2(-(boundry.x - radius), ball.position.y);
                }


                if (ball.position.y > boundry.y - radius)
                {
                    ball.velocity.y *= damping;
                    ball.position = new Vector2(ball.position.x, (boundry.y - radius));
                }
                if (ball.position.y < -(boundry.y - radius))
                {
                    ball.velocity.y *= damping;
                    ball.position = new Vector2(ball.position.x, -(boundry.y - radius));
                }
            }
        }
    }
}