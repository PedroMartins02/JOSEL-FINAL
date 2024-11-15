using ModestTree;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_CreateGameController : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameText;
    [SerializeField] private Transform ruleSingleTemplate;
    [SerializeField] private Transform container;

    private string lobbyName;
    private bool isPrivate = false;

    private void Awake()
    {
        // Hide the template for the rules
        ruleSingleTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        // Load the rules
        LoadRulesIntoContainer();
    }

    public void CreateGame()
    {
        lobbyName = nameText.text.IsEmpty() ? "Custom Game" : nameText.text;

        List<GameModel.GameRule> gameRules = new List<GameModel.GameRule>();

        foreach (Transform child in container)
        {
            if (child == ruleSingleTemplate) continue;

            gameRules.Add(child.GetComponent<UI_RuleSingleTemplate>().GetRule());
        }

        LobbyManager.Instance.CreateLobby(
                lobbyName,
                isPrivate,
                LobbyManager.LobbyType.CustomMatch,
                gameRules
            );
    }

    private void LoadRulesIntoContainer()
    {
        foreach (Transform child in container)
        {
            if (child == ruleSingleTemplate) continue;

            Destroy(child.gameObject);
        }

        foreach (GameModel.GameRule rule in GameModel.GameRule.GetDefaultRules())
        {
            Transform ruleSingleTransform = Instantiate(ruleSingleTemplate, container);
            ruleSingleTransform.gameObject.SetActive(true);

            UI_RuleSingleTemplate ruleListSingleUI = ruleSingleTransform.GetComponent<UI_RuleSingleTemplate>();
            ruleListSingleUI.SetRule(rule);
        }
    }
}
