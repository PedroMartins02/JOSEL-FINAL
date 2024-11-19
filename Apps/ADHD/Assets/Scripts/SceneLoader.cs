using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    public enum Scene
    {
        Authentication,
        NavigationScene, //Main menu of the game
        DeckCosmeticsScene, //Change cosmetics of a deck
        //Game, //Scene for the game
        Lobby, //Scene for the pre-game lobby
        DeckBuilder,
        LoadingScene,
        ExitNetworkLoadingScene
    }

    private static Scene targetScene;


    public static void Load(string scene)
    {
        if (scene != null && scene != "")
        {
            Scene targetScene = (Scene)Enum.Parse(typeof(Scene), scene);
            Load(targetScene);
        }
    }

    public static void Load(Scene targetScene)
    {
        // Load the scene we want to access
        SceneLoader.targetScene = targetScene;

        // While the target Scene is Loading, show LoadingScene (need to create)
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void ExitNetworkLoad(Scene targetScene)
    {
        // Load the scene we want to access
        SceneLoader.targetScene = targetScene;

        // While the target Scene is Loading, show LoadingScene (need to create)
        SceneManager.LoadScene(Scene.ExitNetworkLoadingScene.ToString());
    }

    public static void LoadNetwork(Scene targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
