namespace Serene.Common.Interfaces;

public interface ISceneNavigator
{
    void Change(string sceneKey);
    void PushScene(string sceneKey);
    void PopScene();
    void ExitGame();
}
