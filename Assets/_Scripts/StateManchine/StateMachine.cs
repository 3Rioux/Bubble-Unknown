using System;

public class StateMachine<T> where T : Enum
{
    private T _currentState;

    public T CurrentState => _currentState;

    // Action to trigger when the state changes
    public event Action<T> OnStateChanged;

    public StateMachine(T initialState)
    {
        _currentState = initialState;
    }

    public void ChangeState(T newState)
    {
        if (!_currentState.Equals(newState))
        {
            _currentState = newState;
            OnStateChanged?.Invoke(newState); // Notify listeners
            UnityEngine.Debug.Log($"Game state changed to: {newState}");
        }
    }//end ChangeState


}//end StateMachine
