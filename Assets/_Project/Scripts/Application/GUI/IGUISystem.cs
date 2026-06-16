using Cysharp.Threading.Tasks;

namespace _Project.Application.GUI
{
    public interface IGUISystem
    {
        CanvasBehaviour Canvas { get; }
        UniTask EnsureInitialized();
    }
}
