using MLAgents;
using System.Linq;
using UnityEngine;

public class AIAgent : Agent {
    public float downsampleFactor = 0.2f;

    private Bender bender;
    private Bender[] enemies;
    private Terrain terrain;

    private Bender template;

    public override void InitializeAgent()
    {
        bender = GetComponentInChildren<Bender>();

        template = Instantiate(bender);
        template.gameObject.SetActive(false);

        // Find enemy controller
        AIController[] controllers = GetComponents<AIController>();
        var controller = controllers.First((x) => x != bender.Owner);
        enemies = controller.GetComponentsInChildren<Bender>();
         
        terrain = Terrain.activeTerrain;
    }


    public override void AgentReset()
    {
        Debug.Log("Resetting agent " + name);
        Vector3 newPosition = terrain.terrainData.bounds.center + Vector3.Scale(Random.insideUnitCircle, terrain.terrainData.bounds.extents);
        newPosition.y = terrain.SampleHeight(newPosition);
       
        // Recreate the bender
        // TODO: Wasteful, should reuse bender if possible
        if (bender != null) Destroy(bender.gameObject);
        bender = Instantiate(template, newPosition, Quaternion.identity, transform);

        // TODO: Dependent on how agent reset is called
        // Find enemy controller
        AIController[] controllers = GetComponents<AIController>();
        var controller = controllers.First((x) => x != bender.Owner);
        enemies = controller.GetComponentsInChildren<Bender>();
    }


    public override void CollectObservations()
    {
        // Height grid
        int width = (int) (terrain.terrainData.heightmapWidth * downsampleFactor);
        int height = (int) (terrain.terrainData.heightmapHeight * downsampleFactor);
        Vector3 sampleScale = terrain.terrainData.heightmapScale;

        float[,] heights = terrain.terrainData.GetHeights(0, 0, width, height);
        float[] flattenedHeights = heights.Cast<float>().ToArray();

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
            
            Vector2Int enemySamplePosition = new Vector2Int((int)(enemyPosition.x / sampleScale.x * downsampleFactor), (int)(enemyPosition.z / sampleScale.z * downsampleFactor));

            enemyGrid[enemySamplePosition.x * width + enemySamplePosition.y] += 1;
        }

        AddVectorObs(enemyGrid);
    }



    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Time constraint
        AddReward(-1);

        // Killed all enemies
        if (enemies.Length == 0) {
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

        switch (selectedAction)
        {
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
