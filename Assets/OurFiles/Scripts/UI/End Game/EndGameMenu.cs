using UnityEngine;

public class EndGameMenu : MainMenuManager
{
    [SerializeField] private string sceneOnMenu;
    
    public void MainMenu() => loader.LoadScene(sceneOnMenu);
}
