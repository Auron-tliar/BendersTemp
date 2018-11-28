using MLAgents;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIAgent : Agent {
    private Bender bender;
    private List<Bender> enemies;
    private Terrain terrain;

    public override void InitializeAgent()
    {
        bender = GetComponentInChildren<Bender>();
        terrain = Terrain.activeTerrain;
    }

    public override void CollectObservations()
    {
        // Height grid
        int width = terrain.terrainData.heightmapWidth;
        int height = terrain.terrainData.heightmapHeight;
        Vector3 sampleScale = terrain.terrainData.heightmapScale;
    
        float[,] heights = terrain.terrainData.GetHeights(0, 0, width, height);
        float[] flattenedHeights = heights.Cast<float>().ToArray();

        AddVectorObs(flattenedHeights);


        // Enemy grid
        Vector3 position = terrain.GetPosition();
        Vector3 size = terrain.terrainData.size;
        position = position - size/2;

        float[] enemyGrid = new float[width * height];
        foreach(var enemy in enemies)
        {
            Vector3 enemyPosition = enemy.transform.position;
            enemyPosition -= position;
            
            Vector2Int enemySamplePosition = new Vector2Int((int)(enemyPosition.x / sampleScale.x), (int)(enemyPosition.z / sampleScale.z));

            enemyGrid[enemySamplePosition.x * width + enemySamplePosition.y] = 1;
        }

        AddVectorObs(enemyGrid);



    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Time constraint
        AddReward(-1);

        // Killed all enemies
        if (enemies.Count == 0) {
            Done();
            AddReward(100);
        }

        //if(bender.GotHit())
        //{
        //    AddReward(-1f);
        //}

        //if(bender.GotDefeated())
        //{
        //    Done();
        //    AddReward(-100f);
        //}


        // Perform actions
        if (bender.State != Bender.States.Idle && bender.State != Bender.States.Moving)
        {
            return;
        }

        if (vectorAction[0] == 1)
        {
            bender.StartAbility(Bender.States.Casting1, 0);
        }
        else if (vectorAction[1] == 1)
        {
            bender.StartAbility(Bender.States.Casting2, 1);
        }
        else if (vectorAction[2] == 1)
        {
            bender.StartAbility(Bender.States.Casting3, 2);
        }

    }

    public override void AgentOnDone()
    {
        throw new NotImplementedException();
    }

    public override void AgentReset()
    {
        throw new NotImplementedException();
    }
}
