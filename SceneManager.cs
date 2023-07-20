using SFML.Graphics;
using TeleBox.Engine.Utility;
using TeleBox.Scenes;
using TeleBox.UI;

namespace TeleBox;

public class SceneManager
{
    private RenderWindow _window;
    private Dictionary<string, Scene> _knownScenes;
    private List<BaseGame> _games;

    public List<BaseGame> Games => _games;
    
    public Scene CurrentScene { get; private set; }
    
    public static SceneManager Instance { get; private set; }
    
    public SceneManager(RenderWindow window)
    {
        Instance = this;
        _window = window;
        _knownScenes = new Dictionary<string, Scene>();
        _games = new List<BaseGame>();

        //Generate Scenes
        foreach (var subclass in typeof(Scene).AllSubclasses())
        {
            if (Activator.CreateInstance(subclass) is Scene scene)
            {
                _knownScenes.Add(scene.Name, scene);
                if (scene is BaseGame game)
                {
                    _games.Add(game);
                }
            }
        }
        
        //
        Switch("MainMenu");
    }
    
    public void Switch(string sceneName)
    {
        if (_knownScenes.TryGetValue(sceneName, out var scene))
        {
            CurrentScene?.SceneClosed();
            CurrentScene = scene;
            CurrentScene.SceneOpened();
        }
    }
    
    public void HandleEvents(TeleEventArgs args)
    {
        CurrentScene.HandleEvents(args);
    }
    
    public void Update(float delta)
    {
        CurrentScene.Update(delta);
    }
    
    public void Render()
    {
        _window.Draw(CurrentScene);
    }
}