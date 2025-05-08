namespace Frontend.States;

public class AuthStateNotifier
{
    // Lista di componenti che ascoltano i cambiamenti
    private readonly List<Action> _subscribers = new();

    // Metodo per iscriversi ai cambiamenti
    public void Subscribe(Action callback)
    {
        _subscribers.Add(callback);
    }

    // Metodo per annullare l'iscrizione
    public void Unsubscribe(Action callback)
    {
        _subscribers.Remove(callback);
    }

    // Metodo per inviare una notifica a tutti gli iscritti
    public void Notify()
    {
        foreach (var subscriber in _subscribers)
        {
            subscriber.Invoke();  // Esegui il callback per ogni componente
        }
    }
}
