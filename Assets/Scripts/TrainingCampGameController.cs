using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingCampGameController: MonoBehaviour
{
    [HideInInspector]
    public List<PlayerController> PlayerControllers;

    public Text Minutes;
    public Text Seconds;

    public Transform PlayersContainer;
    public List<Transform> BenderIconPanels;
    public GameObject AbilityPanels;
    public List<Transform> SpawnPoints;

    public GameObject HumanMousePlayerPrefab;
    public GameObject HumanKeyboardPlayerPrefab;
    public GameObject AIPlayerPrefab;

    [Header("Bender Prefabs")]
    public GameObject AirBenderPrefab;
    public GameObject EarthBenderPrefab;
    public GameObject FireBenderPrefab;
    public GameObject WaterBenderPrefab;

    public MLAgents.Brain brain;

    public static class TrainingMatchSettings
    {
        public static List<List<Bender.BenderTypes>> Players = new List<List<Bender.BenderTypes>>()
        {
            new List<Bender.BenderTypes>()
            {
                Bender.BenderTypes.Water
            },
            new List<Bender.BenderTypes>()
            {
                Bender.BenderTypes.Water
            }
        };

        public static List<Color> PlayerColors = new List<Color>()
        {
            Color.blue,
            Color.red
        };

        public static List<PlayerController.PlayerTypes> PlayerTypes = new List<PlayerController.PlayerTypes>()
        {
            PlayerController.PlayerTypes.AI,
            PlayerController.PlayerTypes.AI
        };

    }

    private void Start()
    {

        for (int i = 0; i < TrainingMatchSettings.Players.Count; i++)
        {
            switch (TrainingMatchSettings.PlayerTypes[i])
            {
                case PlayerController.PlayerTypes.HumanMouse:
                    PlayerControllers.Add(Instantiate(HumanMousePlayerPrefab, 
                        PlayersContainer).GetComponent<PlayerController>());
                    ((HumanControllerMouse)PlayerControllers[i]).Panels = AbilityPanels;
                    break;
                case PlayerController.PlayerTypes.HumanKeyBoard:
                    PlayerControllers.Add(Instantiate(HumanKeyboardPlayerPrefab,
                        PlayersContainer).GetComponent<PlayerController>());
                    ((HumanControllerKeyBoard)PlayerControllers[i]).Panels = AbilityPanels;
                    break;
                case PlayerController.PlayerTypes.AI:
                    PlayerControllers.Add(Instantiate(AIPlayerPrefab,
                        PlayersContainer).GetComponent<PlayerController>());
                    break;
                default:
                    break;
            }

            for (int j = 0; j < TrainingMatchSettings.Players[i].Count; j++)
            {
                GameObject temp;
                switch (TrainingMatchSettings.Players[i][j])
                {
                    case Bender.BenderTypes.Air:
                        temp = AirBenderPrefab;
                        break;
                    case Bender.BenderTypes.Earth:
                        temp = EarthBenderPrefab;
                        break;
                    case Bender.BenderTypes.Fire:
                        temp = FireBenderPrefab;
                        break;
                    case Bender.BenderTypes.Water:
                        temp = WaterBenderPrefab;
                        break;
                    default:
                        temp = null;
                        break;
                }


                if (TrainingMatchSettings.PlayerTypes[i] == PlayerController.PlayerTypes.AI)
                {
                    GameObject aiAgentTemplate = new GameObject("AIAgent");
                    aiAgentTemplate.SetActive(false);

                    aiAgentTemplate.tag = "Bender";
                    aiAgentTemplate.AddComponent<AIAgent>();
                    aiAgentTemplate.GetComponent<AIAgent>().brain = brain;
                    aiAgentTemplate.GetComponent<AIAgent>().isInTrainingCamp = true;

                    GameObject aiAgent = Instantiate(aiAgentTemplate, SpawnPoints[i].GetChild(j).position, SpawnPoints[i].GetChild(j).rotation, PlayerControllers[i].transform);
                   
                    Instantiate(temp, SpawnPoints[i].GetChild(j).position, SpawnPoints[i].GetChild(j).rotation, aiAgent.transform);

                    aiAgent.SetActive(true);

                    Destroy(aiAgentTemplate);
                }
                else
                {
                    Instantiate(temp, SpawnPoints[i].GetChild(j).position,
                        SpawnPoints[i].GetChild(j).rotation, PlayerControllers[i].transform);
                }
                
                
            }
            PlayerControllers[i].PlayerColor = TrainingMatchSettings.PlayerColors[i];
        }
    }

    private void Update()
    {

    }

}
