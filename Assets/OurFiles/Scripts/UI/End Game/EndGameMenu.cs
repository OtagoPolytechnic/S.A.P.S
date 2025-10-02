using UnityEngine;

/// <summary>
/// Controls the end game menu behaviour.  
/// Extends <see cref="MainMenuManager"/> for button handling,  
/// and adds functionality to return to the main menu.
/// </summary>
public class EndGameMenu : MainMenuManager
{
    public void MainMenu() => SceneLoader.Instance.LoadMenuScene();
}
