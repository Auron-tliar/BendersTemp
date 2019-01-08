using MLAgents;
using System.Linq;
using UnityEngine;

public class AIAgent : Agent {
    public static float downsampleFactor = 0.08f;
    

    private Bender bender;
    private Bender[] enemies;
    private AIController enemyController;
    private Terrain terrain;

    private Bender template;

    private Vector3 initialPosition;
    private bool useTerrainHeights = false;

    public bool isInTrainingCamp = false;

    private float downsampleFactorCorrection = downsampleFactor; //*1.2f;

    public override void InitializeAgent()
    {
        bender = GetComponentInChildren<Bender>();

        bender.gameObject.SetActive(false);
        template = Instantiate(bender, transform.parent);
        //template.NavAgent.enabled = false;

        // Find enemy controller
        AIController[] controllers = Component.FindObjectsOfType<AIController>();
        var controller = controllers.First((x) => x != bender.Owner);
        enemies = controller.GetComponentsInChildren<Bender>();

        terrain = Terrain.activeTerrain;

        initialPosition = transform.position;

        AgentReset();
    }


    public override void AgentReset()
    {
        Debug.Log("Resetting agent " + name);

        Vector3 newPosition;
        Quaternion newAngle;

        if (isInTrainingCamp && Terrain.activeTerrain != null)
        {
            newPosition = Terrain.activeTerrain.transform.position + (new Vector3(Random.Range(0.0f, 40.0f), 0, Random.Range(00.0f, 50.0f)));
            newAngle = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.up);
        }
        else
        {
            newPosition = initialPosition;
            newAngle = transform.rotation;
        }

        newPosition.y += terrain.SampleHeight(newPosition);

        var controller = bender.Owner;

        // Recreate the bender
        // TODO: Wasteful, should reuse bender if possible
        if (bender != null)
        {
            Destroy(bender.gameObject);
        }

        bender = Instantiate(template, newPosition, newAngle, transform);
        bender.gameObject.SetActive(true);
        bender.NavAgent.enabled = false;
        bender.Owner = controller;

        //bender.transform.position = newPosition;
        //bender.transform.rotation = Quaternion.identity;

        // TODO: Dependent on how agent reset is called
        // Find enemy controller
        AIController[] enemyControllers = FindObjectsOfType<AIController>();
        enemyController = enemyControllers.First((x) => x != controller);

        //enemies = enemyController.GetComponentsInChildren<Bender>();
        enemies = null; // don't set it here because the other controller will delete its benders when resetting the scene

    }



    public override void CollectObservations()
    {
        // Height grid
        int width = (int)(terrain.terrainData.heightmapWidth * downsampleFactor);
        int height = (int)(terrain.terrainData.heightmapHeight * downsampleFactor);
        Vector3 sampleScale = terrain.terrainData.heightmapScale;


        // Set observation memory
        int observationSize = width * height + 3;

        // add general information header
        AddVectorObs(new float[] { (float)bender.BenderType, width, height });

        if (useTerrainHeights)
        {
            observationSize += width * height;
            float[,] heights = terrain.terrainData.GetHeights(0, 0, width, height);
            float[] flattenedHeights = heights.Cast<float>().ToArray();

            AddVectorObs(flattenedHeights);
        }

        brain.brainParameters.vectorObservationSize = observationSize;
        //Debug.Log("observationSize:" + observationSize);
        //Debug.Log("BenderType:" + (float)bender.BenderType);
        //Debug.Log("width:" + width);
        //Debug.Log("height:" + height);


        // Enemy grid
        float[] enemyGrid = new float[width * height];
        enemyGrid.Fill(-1);

        try
        {
            if (bender.transform == null)
            {
                AddVectorObs(enemyGrid);
                return;
            }
        }
        catch
        {
            AddVectorObs(enemyGrid);
            return;
        }


        // Subtract the position of the bender to have relative enemy positions
        Vector3 position = bender.transform.position;
        Vector3 terrainPosition = terrain.transform.position;
        float y_rotation = bender.transform.rotation.eulerAngles.y;
        Quaternion rotation = bender.transform.rotation;

        //Debug.Log("y_rotation:" + y_rotation);

        Vector3 p = new Vector3(0, 0, 0);

        for (int y = 0; y < terrain.terrainData.size.z; y++)
        {
            for (int x = 0; x < terrain.terrainData.size.x; x++)
            {
                p.Set(x, 0, y);
                p = p + terrainPosition - position;
                p = (rotation * p);

                float enemyX = p.x / sampleScale.x * downsampleFactorCorrection;
                float enemyY = p.z / sampleScale.z * downsampleFactorCorrection;

                Vector2Int enemySamplePosition = new Vector2Int((int)((enemyX + width) / 2), (int)(enemyY + height) / 2);

                //Debug.Log(enemySamplePosition);

                int grid_pos = enemySamplePosition.x * height + enemySamplePosition.y;

                if (grid_pos >= 0 && grid_pos < width * height)
                    enemyGrid[grid_pos] = 0;
            }
        }


        if (enemies == null) {
            enemies = enemyController.GetComponentsInChildren<Bender>();
        }

        if (enemies != null)
        {
            foreach (var enemy in enemies)
            {
                if (enemy != null && !enemy.IsDefeated())
                {
                    Vector3 enemyPosition = enemy.transform.position;
                    enemyPosition -= position;

                    enemyPosition = rotation * enemyPosition;

                    float enemyX = enemyPosition.x / sampleScale.x * downsampleFactorCorrection;
                    float enemyY = enemyPosition.z / sampleScale.z * downsampleFactorCorrection;
                    Vector2Int enemySamplePosition = new Vector2Int((int)((enemyX+width)/2), (int)(enemyY+height)/2);

                    //Debug.Log(enemySamplePosition);

                    int grid_pos = enemySamplePosition.x * height + enemySamplePosition.y;

                    if (grid_pos >= 0 && grid_pos < width * height)
                    {
                        if (enemyGrid[grid_pos] < 0)
                        {
                            enemyGrid[grid_pos] = 1;
                        }
                        else
                        {
                            enemyGrid[grid_pos] += 1;
                        }
                    }
                }
            }
        }

        AddVectorObs(enemyGrid);
    }



    public override void AgentAction(float[] vectorAction, string textAction)
    {
        SetReward(0);

        // Time constraint
        // AddReward(-1);

        // Killed all enemies
        /*if (enemies.Length == 0) {
            Done();
            AddReward(100);
        }*/

        foreach (var enemy in enemies)
        {
            if (!enemy.IsDefeated() && enemy.IsHit())
            {
                AddReward(2);
            }
            if (enemy.IsDefeated())
            {
                AddReward(10);
            }

            if (bender != null && enemy != null && !bender.IsDefeated())
            {
                float angle = Vector3.Angle((enemy.transform.position - bender.transform.position), bender.transform.forward);
                if (System.Math.Abs(angle) < 3f)
                {
                    //Debug.Log("looking at enemy!");
                    AddReward(1);
                }
            }
        }

        if (bender == null || bender.IsDefeated())
        {
            //Done();
            AddReward(-10f);
        }


        /*if (!bender.IsDefeated() && bender.IsHit())
        {
            AddReward(-1f);
        }*/


        // Perform actions
        if (bender.State != Bender.States.Idle && bender.State != Bender.States.Moving)
        {
            // Penalize if choosing action in wrong state
            //AddReward(-1);
            return;
        }

        int selectedAction = MaxIndex(vectorAction);

        if (selectedAction == 0)
            AddReward(-1);

        //Debug.Log("vectorAction:" + string.Join(", ", vectorAction));

        try
        {
            switch (selectedAction)
            {
                case 1:
                    bender.StartAbility(Bender.States.Casting1, 0); break;
                case 2:
                    bender.StartAbility(Bender.States.Casting2, 1); break;
                case 3:
                    bender.StartAbility(Bender.States.Casting3, 2); break;
                case 4:
                    bender.SpeedInput = 2f; break;
                case 5:
                    bender.SpeedInput = -2f; break;
                case 6:
                    bender.RotationSpeedInput = 5f; break;
                case 7:
                    bender.RotationSpeedInput = -5f; break;
                default:
                    break;
            }
        }
        catch
        {
            Debug.Log("could not execute action: "+selectedAction);
        }

    }

    private int MaxIndex(float[] array)
    {
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
    }
}
