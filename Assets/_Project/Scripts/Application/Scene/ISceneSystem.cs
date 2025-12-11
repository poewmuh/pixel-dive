using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace _Project.Application.Scene
{
    public interface ISceneSystem
    {
        UniTask LoadScene(SceneType scene, LoadSceneMode sceneMode);
    }
}
