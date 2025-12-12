using R3;

namespace _Project.Application.GUI.WindowsSystem
{
    public interface IWindowSystem
    {
        Subject<IWindow> OnWindowOpened { get; }
        Subject<IWindow> OnWindowClosed { get; }
        
        int WindowsCount { get; }
        
        T Show<T>() where T : WindowBase;
        void Close(IWindow window);
        void Close<T>() where T : IWindow;
        void CloseAll();
        void CloseLast();
    }
}
