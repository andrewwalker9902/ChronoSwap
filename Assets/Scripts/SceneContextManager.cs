using UnityEngine;

public static class SceneContextManager
{
    public enum SourceScene
    {
        None,
        MainMenu,
        Gameplay
    }

    public static SourceScene CameFrom = SourceScene.None;
}
