using System;

namespace _Project.Application.GUI
{
    public interface IGUISystem : IDisposable
    {
        CanvasBehaviour Canvas { get; }
    }
}
