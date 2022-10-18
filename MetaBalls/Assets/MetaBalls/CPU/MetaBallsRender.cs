using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluidSimulation;


public class MetaBallsRender : MonoBehaviour
{
    public MarcingSquaresWithLinerInterpolation marcingSquares;
    public MetaBallsModel metaBallsModel;
    public bool drawBoundry;
    public bool drawBalls;

    public float[] map;
    public Vector2 spaceSize;
    public Vector2Int mapSize;
    public float squareSize = 1;
    public float linerValue;
    public MeshFilter meshFilter;

    private void Start()
    { 
        marcingSquares = new MarcingSquaresWithLinerInterpolation(0.5f);
    }

    private void Update()
    {
        MarcingSquaresWithLinerInterpolation.linerValue = linerValue;
        ApplayFunction();
    }

    public void ApplayFunction()
    {
        mapSize.x = Mathf.RoundToInt(spaceSize.x / squareSize);
        mapSize.y = Mathf.RoundToInt(spaceSize.y / squareSize);

        map = new float[mapSize.x * mapSize.y];
     
        for (int x = -mapSize.x / 2; x < mapSize.x / 2; x++)
        {
            for (int y = -mapSize.y / 2; y < mapSize.y / 2; y++)
            {
                var pos     = new Vector2(x * squareSize, y * squareSize);
                var gridPos = Hellper.Position2GridIndex(pos, spaceSize, squareSize);
                var index   = Hellper.From2To1Index(gridPos.x, gridPos.y, mapSize.x);
                var color   = MetaBallsFunction(pos, metaBallsModel.balls);
                
                map[index]  = color;
            }
        }

        meshFilter.mesh = marcingSquares.GenerateMesh(map, mapSize, squareSize);
    }


    float MetaBallsFunction(Vector2 point, TransForm[] balls)
    {
        var meteSum = .0f;
     
        foreach (var ball in balls)
        {
            var dist = (point - ball.position).sqrMagnitude;
            meteSum += metaBallsModel.radius * metaBallsModel.radius / dist ;
        }
        return meteSum - 1;
    }


    private void OnDrawGizmos()
    {
        DrawGizmosBalls(drawBalls);
        DrawGizmosBoundry(drawBoundry);
    }

    private void DrawGizmosBalls(bool draw)
    {
        if (draw)
        {
            foreach (var ball in metaBallsModel.balls)
            {
                Gizmos.DrawWireSphere(ball.position, metaBallsModel.radius);
            }
        }
    }

    private void DrawGizmosBoundry(bool draw)
    {
        if (draw)
        {
            var contaner = metaBallsModel.contaner;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, contaner);
        }
    }
}
