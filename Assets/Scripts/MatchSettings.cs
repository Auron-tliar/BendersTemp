using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MatchSettings
{
    public static List<List<Bender.BenderTypes>> Players = new List<List<Bender.BenderTypes>>()
    {
        new List<Bender.BenderTypes>()
        {
            Bender.BenderTypes.Air,
            Bender.BenderTypes.Water,
            Bender.BenderTypes.Water,
            Bender.BenderTypes.Air
        },
        new List<Bender.BenderTypes>()
        {
            Bender.BenderTypes.Water,
            Bender.BenderTypes.Air,
            Bender.BenderTypes.Water,
            Bender.BenderTypes.Air
        }
    };

    public static List<Color> PlayerColors = new List<Color>()
    {
        Color.blue,
        Color.red
    };

    public static List<PlayerController.PlayerTypes> PlayerTypes = new List<PlayerController.PlayerTypes>()
    {
        PlayerController.PlayerTypes.HumanMouse,
        PlayerController.PlayerTypes.AI
    };

    public static int? WinnerTeam;

}
