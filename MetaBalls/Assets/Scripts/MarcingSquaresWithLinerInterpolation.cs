using System.Collections.Generic;
using UnityEngine;
using FluidSimulation;
using Newtonsoft.Json.Linq;
using System;

public class MarcingSquaresWithLinerInterpolation 
{
    public static float linerValue = 0.1f;
  
   
    private Vector2Int gridDimension;
    private float squareSize;

    public List<Vector3> vertices;
    public List<Vector2> uvs;
    public List<int> triangles;
    public List<int> boundryTriangles;
    
    public SquareGrid squareGrid;

    public MarcingSquaresWithLinerInterpolation(float value)
    {
        linerValue = value;
    }

    public void OnDrawGizmos()
    {
        if (squareGrid == null) return;

      
        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                var square = squareGrid.squares[x, y];

                Gizmos.color = square.topLeft.active > linerValue ? Color.green : Color.red;
                Gizmos.DrawCube(square.topLeft.position, Vector2.one * .1f);

                Gizmos.color = square.topRight.active > linerValue ? Color.green : Color.red;
                Gizmos.DrawCube(square.topRight.position, Vector2.one * .1f);

                Gizmos.color = square.bottomRight.active > linerValue ? Color.green : Color.red;
                Gizmos.DrawCube(square.bottomRight.position, Vector2.one * .1f);

                Gizmos.color = square.bottomLeft.active > linerValue ? Color.green : Color.red;
                Gizmos.DrawCube(square.bottomLeft.position, Vector2.one * .1f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(square.centreTop.position, .02f);
                Gizmos.DrawSphere(square.centreRight.position, .02f);
                Gizmos.DrawSphere(square.centreBottom.position, .02f);
                Gizmos.DrawSphere(square.centreLeft.position, .02f);
            }
        }
    }

    public Mesh GenerateMesh(float[] map, Vector2Int dimension, float squareSize)
    {
        this.gridDimension = dimension;
        this.squareSize = squareSize;

        squareGrid = new SquareGrid(map, dimension, squareSize);

        vertices = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<int>();
        boundryTriangles = new List<int>();


        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }

        Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    public Mesh GenerateMesh(Texture2D map, Vector2Int mapSize, float squareSize)
    {
        this.gridDimension = mapSize;
        this.squareSize = squareSize;

        squareGrid = new SquareGrid(map, mapSize, squareSize);

        vertices = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<int>();
        boundryTriangles = new List<int>();


        TriangulateGrid();


        Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

   
    void TriangulateGrid()
    {
        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }
    }

    void TriangulateSquare(Square square)
    {
        switch (square.configuration)
        {
            case 0: break;

            // 1 points:
            case 1: MeshFromPoints(true, square.centreLeft, square.centreBottom, square.bottomLeft); break;
            case 2: MeshFromPoints(true, square.bottomRight, square.centreBottom, square.centreRight); break;
            case 4: MeshFromPoints(true, square.topRight, square.centreRight, square.centreTop); break;
            case 8: MeshFromPoints(true, square.topLeft, square.centreTop, square.centreLeft); break;

            // 2 points:
            case 3: MeshFromPoints(true, square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft); break;
            case 6: MeshFromPoints(true, square.centreTop, square.topRight, square.bottomRight, square.centreBottom); break;
            case 9: MeshFromPoints(true, square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft); break;
            case 12: MeshFromPoints(true, square.topLeft, square.topRight, square.centreRight, square.centreLeft); break;

            case 5: MeshFromPoints(true, square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft); break;
            case 10: MeshFromPoints(true, square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft); break;

            // 3 point:
            case 7: MeshFromPoints(true, square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft); break;
            case 11: MeshFromPoints(true, square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft); break;
            case 13: MeshFromPoints(true, square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft); break;
            case 14: MeshFromPoints(true, square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft); break;

            // 4 point:
            case 15: MeshFromPoints(false, square.topLeft, square.topRight, square.bottomRight, square.bottomLeft); break;
        }
    }

    void AssignVertices(Node[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].vertexIndex == -1)
            {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);

                float percentX = Mathf.InverseLerp(-gridDimension.x / 2 * squareSize, gridDimension.x / 2 * squareSize, points[i].position.x);
                float percentY = Mathf.InverseLerp(-gridDimension.y / 2 * squareSize, gridDimension.y / 2 * squareSize, points[i].position.y);
                uvs.Add(new Vector2(percentX, percentY));
            }
        }
    }

    void MeshFromPoints(bool isBoundry, params Node[] points)
    {
        AssignVertices(points);

        if (points.Length >= 3) { CreateTriangle(isBoundry, points[0], points[1], points[2]); }
        if (points.Length >= 4) { CreateTriangle(isBoundry, points[0], points[2], points[3]); }
        if (points.Length >= 5) { CreateTriangle(isBoundry, points[0], points[3], points[4]); }
        if (points.Length >= 6) { CreateTriangle(isBoundry, points[0], points[4], points[5]); }
    }

    void CreateTriangle(bool isBoundry, Node a, Node b, Node c)
    {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);

        if (isBoundry)
        {
            boundryTriangles.Add(a.vertexIndex);
            boundryTriangles.Add(b.vertexIndex);
            boundryTriangles.Add(c.vertexIndex);
        }
    }

    public class SquareGrid
    {
        public ControlNode[,] controlNodes;
        public Square[,] squares;


        public SquareGrid(float[] map, Vector2Int dimension, float squareSize)
        {
            int nodeCountX = dimension.x;
            int nodeCountY = dimension.y;
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    var xPos = -mapWidth  / 2 + x * squareSize + squareSize / 2;
                    var yPos = -mapHeight / 2 + y * squareSize + squareSize / 2;
                    Vector3 pos = new Vector3(xPos, yPos, 0);

                    var index = Hellper.From2To1Index(x, y, nodeCountX);
                  
                    controlNodes[x, y] = new ControlNode(pos, map[index]);
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY - 1];

            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    var topLeft     = controlNodes[x    , y + 1];
                    var topRight    = controlNodes[x + 1, y + 1];
                    var bottomRight = controlNodes[x + 1, y    ];
                    var bottomLeft  = controlNodes[x    , y    ];


                    var centreTop    = CalculateCentreNode(topLeft    , topRight   , topLeft.active     , topRight.active   , EdgeType.Horizontal);
                    var centreRight  = CalculateCentreNode(topRight   , bottomRight, topRight.active    , bottomRight.active, EdgeType.Vertical);
                    var centreBottom = CalculateCentreNode(bottomRight, bottomLeft , bottomRight.active , bottomLeft.active , EdgeType.Horizontal);
                    var centreLeft   = CalculateCentreNode(bottomLeft , topLeft    , bottomLeft.active  , topLeft.active    , EdgeType.Vertical);

                    var square = new Square(topLeft, topRight, bottomRight, bottomLeft,centreTop, centreRight,centreBottom,centreLeft);

                    squares[x, y] = square;
                }
            }

        }

        public SquareGrid(Texture2D map, Vector2Int dimension, float squareSize)
        {
            float mapWidth = dimension.x * squareSize;
            float mapHeight = dimension.y * squareSize;

            controlNodes = new ControlNode[dimension.x, dimension.y];

            for (int x = 0; x < dimension.x; x++)
            {
                for (int y = 0; y < dimension.y; y++)
                {
                    var xPos = -mapWidth  / 2 + x * squareSize + squareSize / 2;
                    var yPos = -mapHeight / 2 + y * squareSize + squareSize / 2;
                    Vector3 pos = new Vector3(xPos, yPos, 0);

                    controlNodes[x, y] = new ControlNode(pos, map.GetPixel(x,y).r);
                }
            }

            squares = new Square[dimension.x - 1, dimension.y - 1];

            for (int x = 0; x < dimension.x - 1; x++)
            {
                for (int y = 0; y < dimension.y - 1; y++)
                {
                    var topLeft     = controlNodes[x    , y + 1];
                    var topRight    = controlNodes[x + 1, y + 1];
                    var bottomRight = controlNodes[x + 1, y    ];
                    var bottomLeft  = controlNodes[x    , y    ];

                    var centreTop    = CalculateCentreNode(topLeft, topRight, topLeft.active, topRight.active, EdgeType.Horizontal);
                    var centreRight  = CalculateCentreNode(topRight, bottomRight, topRight.active, bottomRight.active, EdgeType.Vertical);
                    var centreBottom = CalculateCentreNode(bottomRight, bottomLeft, bottomRight.active, bottomLeft.active, EdgeType.Horizontal);
                    var centreLeft   = CalculateCentreNode(bottomLeft, topLeft, bottomLeft.active, topLeft.active, EdgeType.Vertical);

                    var square = new Square(topLeft, topRight, bottomRight, bottomLeft, centreTop, centreRight, centreBottom, centreLeft);

                    squares[x, y] = square;
                }
            }

        }
        
        Node CalculateCentreNode(ControlNode node0, ControlNode node1,float val0,float val1, EdgeType type)
        {
            if (val0 == val1) return new Node(node0.position);

            var t = (linerValue - val0) / (val1 - val0); // InverseLerp

            if (type == EdgeType.Horizontal)
            {
                var y = node0.position.y;
                var x = Mathf.Lerp(node0.position.x, node1.position.x, t);
                var newNodePos = new Vector2(x, y);
                return new Node(newNodePos);
            }
            if (type == EdgeType.Vertical)
            {
                var x = node0.position.x;
                var y = Mathf.Lerp(node0.position.y, node1.position.y, t);
                var newNodePos = new Vector2(x, y);
                return new Node(newNodePos);
            }

            return null;
        }
    }

    public class Square
    {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centreTop, centreRight, centreBottom, centreLeft;
        public float configuration;

        public Square(ControlNode _topLeft  , ControlNode _topRight   , ControlNode _bottomRight , ControlNode _bottomLeft,
                      Node        _centreTop, Node        _centreRight, Node        _centreBottom, Node        _centreLeft )
        {
            topLeft     = _topLeft;
            topRight    = _topRight;
            bottomRight = _bottomRight;
            bottomLeft  = _bottomLeft;

            centreTop    = _centreTop;
            centreRight  = _centreRight;
            centreBottom = _centreBottom;
            centreLeft   = _centreLeft;

            configuration = 0;

            if (topLeft.active     > linerValue) configuration += 8;
            if (topRight.active    > linerValue) configuration += 4;
            if (bottomRight.active > linerValue) configuration += 2;
            if (bottomLeft.active  > linerValue) configuration += 1;
        }
    }

    public class Node
    {
        public Vector3 position;
        public int vertexIndex;

        public Node(Vector3 position)
        {
            this.position = position;
            vertexIndex = -1;
        }
    }

    public class ControlNode : Node
    {
        public float active;

        public ControlNode(Vector3 position, float active) : base(position)
        {
            this.active = active;
        }
    }

    enum EdgeType
    {
        Vertical, Horizontal
    }
}
