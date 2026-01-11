using UnityEngine;

public abstract class BaseState<T> where T : class
{
    // Init
    public abstract void EnterState(T monster);

    // Update
    public abstract BaseState<T> UpdateState(T monster);

    // Exit
    public abstract void ExitState(T monster);
}