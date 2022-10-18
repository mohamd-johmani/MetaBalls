using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluidSimulation;



public class MetaBallsModel : MonoBehaviour
{
    public PhysicsOpject[] balls;

    public float radius;
    public Vector2 contaner = new Vector2(10, 20);

    private void FixedUpdate()
    {
        foreach (var ball in balls)
        {
            ball.position += ball.velocity * Time.deltaTime;
        }

        Hellper.CheckBoundary(contaner / 2, balls, radius, -1);
    } 
}
