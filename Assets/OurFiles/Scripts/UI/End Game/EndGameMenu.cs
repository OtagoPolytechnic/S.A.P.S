using UnityEngine;

public class EndGameMenu : MainMenuManager
{
    [SerializeField] private string sceneOnMenu;
    
    public void MainMenu() => SceneLoader.Instance.LoadScene(sceneOnMenu);
}
