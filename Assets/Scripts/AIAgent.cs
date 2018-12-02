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

        AddVectorObs(new float[] { width, height });

        if (false)
        {
            float[,] heights = terrain.terrainData.GetHeights(0, 0, width, height);
            float[] flattenedHeights = heights.Cast<float>().ToArray();

            AddVectorObs(flattenedHeights);
        }


        // Enemy grid
        if (false) { 
            Vector3 position = terrain.GetPosition();
            Vector3 size = terrain.terrainData.size;
            position = position - size/2;

            float[] enemyGrid = new float[width * height];
            if (enemies != null)
            foreach(var enemy in enemies)
            {
                Vector3 enemyPosition = enemy.transform.position;
                enemyPosition -= position;
            
                Vector2Int enemySamplePosition = new Vector2Int((int)(enemyPosition.x / sampleScale.x), (int)(enemyPosition.z / sampleScale.z));

                enemyGrid[enemySamplePosition.x * width + enemySamplePosition.y] = 1;
            }

            AddVectorObs(enemyGrid);
        }



    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Time constraint
        AddReward(-1);

        // Killed all enemies
        if (enemies != null && enemies.Count == 0) {
            Done();
            AddReward(100);
        }

        if (bender.IsHit())
        {
            AddReward(-1f);
        }

        if (bender == null)
        {
            Done();
            AddReward(-100f);
        }


        // Perform actions
        if (bender.State != Bender.States.Idle && bender.State != Bender.States.Moving)
        {
            // Penalize if choosing action in wrong state
            AddReward(-1);
            return;
        }

        int selectedAction = MaxIndex(vectorAction);

        switch (selectedAction) {
            case 1:
                bender.StartAbility(Bender.States.Casting1, 0); break;
            case 2:
                bender.StartAbility(Bender.States.Casting2, 1); break;
            case 3:
                bender.StartAbility(Bender.States.Casting3, 2); break;
            case 4:
                bender.SpeedInput = 5f; break;
            case 5:
                bender.SpeedInput = -5f; break;
            case 6:
                bender.RotationSpeedInput = 5f; break;
            case 7:
                bender.RotationSpeedInput = -5f; break;
            default:
                break;
        }
    }



    private int MaxIndex(float[]  array) {
        float max = array[0];
        int maxIndex = 0;

        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] > max)
            {
                max = array[i];
                maxIndex = i;
            }
        }
        return maxIndex;
    }

    public override void AgentOnDone()
    {
        //throw new NotImplementedException();
    }

    public override void AgentReset()
    {
        //throw new NotImplementedException();
    }
}
