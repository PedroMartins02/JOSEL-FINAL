using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager Instance { get; private set; }

    // We can use List because we cant change rules mid game, but this is scuffed i guess?
    private List<GameModel.GameRule> gameRules;

}
