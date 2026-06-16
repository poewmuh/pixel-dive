using Cysharp.Threading.Tasks;

namespace _Project.Application.GUI
{
    public interface IOverlaySystem
    {
        UniTask CreateOverlay(OverlayType type);
    }
}
