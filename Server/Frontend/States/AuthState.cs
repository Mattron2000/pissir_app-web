using Shared.DTOs.User;

namespace Frontend.States;

public class AuthState(AuthStateNotifier notifier)
{
    private readonly AuthStateNotifier _notifier = notifier;
    public UserEntityDTO? User { get; private set; }

    public void SetUser(UserEntityDTO? user)
    {
        User = user;
        try {
            _notifier.Notify();
        } catch (Exception ex) {
            Console.WriteLine(ex.GetType().Name + ": " + ex.Message);
        }
    }
}
