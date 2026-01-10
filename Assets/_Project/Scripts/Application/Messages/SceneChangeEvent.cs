using _Project.Application.Scene;

namespace _Project.Application.Messages
{
    public class SceneChangeEvent
    {
        public SceneType SceneType;

        public SceneChangeEvent(SceneType sceneType)
        {
            SceneType = sceneType;
        }
    }
}
