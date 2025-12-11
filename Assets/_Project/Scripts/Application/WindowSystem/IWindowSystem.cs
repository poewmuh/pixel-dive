using R3;

namespace _Project.Application.WindowSystem
{
    public interface IWindowSystem
    {
        Subject<IWindow> OnWindowOpened { get; }
        Subject<IWindow> OnWindowClosed { get; }
        
        T Show<T>() where T : IWindow;
        void Close(IWindow window);
        void Close<T>() where T : IWindow;
        void CloseAll();
        void CloseLast();
    }
}
