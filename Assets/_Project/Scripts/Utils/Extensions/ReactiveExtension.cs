using System.Threading;
using R3;
using UnityEngine.InputSystem;

namespace Utils._Project.Utils.Extensions
{
    public static class ReactiveExtension
    {
        public static Observable<InputAction.CallbackContext> PerformedAsObservable(this InputAction inputAction, CancellationToken cancellationToken = default)
        {
            return Observable.FromEvent<InputAction.CallbackContext>(
                h => inputAction.performed += h,
                h => inputAction.performed -= h,
                cancellationToken
            );
        }

        public static Observable<InputAction.CallbackContext> CanceledAsObservable(this InputAction inputAction, CancellationToken cancellationToken = default)
        {
            return Observable.FromEvent<InputAction.CallbackContext>(
                h => inputAction.canceled += h,
                h => inputAction.canceled -= h,
                cancellationToken
            );
        }

        public static Observable<InputAction.CallbackContext> StartedAsObservable(this InputAction inputAction, CancellationToken cancellationToken = default)
        {
            return Observable.FromEvent<InputAction.CallbackContext>(
                h => inputAction.started += h,
                h => inputAction.started -= h,
                cancellationToken
            );
        }
    }
}
