using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleActionEvent
{
    private event Action action;
    public static SimpleActionEvent operator +(SimpleActionEvent action1, Action action2)
    {
        action1.action += action2;
        return action1;
    }
    public static SimpleActionEvent operator -(SimpleActionEvent action1, Action action2)
    {
        action1.action -= action2;
        return action1;
    }

    public void Invoke()
    {

        if (action != null)
        {
            action.Invoke();
        }
    }

    public static void Add(SimpleActionEvent action1, Action action2)
    {

    }
}
[System.Serializable]
public class SimpleActionEvent<T>
{
    private event Action<T> action;
    public static SimpleActionEvent<T> operator +(SimpleActionEvent<T> action1, Action<T> action2)
    {
        action1.action += action2;
        return action1;
    }
    public static SimpleActionEvent<T> operator -(SimpleActionEvent<T> action1, Action<T> action2)
    {
        action1.action -= action2;
        return action1;
    }
    public void Invoke(T arg)
    {

        if (action != null)
        {
            action.Invoke(arg);
        }
    }
}
public class SimpleActionEvent<T,U> {
    private event Action<T, U> action;
    public static SimpleActionEvent<T, U> operator +(SimpleActionEvent<T, U> action1,  Action<T, U> action2) {
        action1.action += action2;
        return action1;
    }
    public static SimpleActionEvent<T, U> operator -(SimpleActionEvent<T, U> action1, Action<T, U> action2)
    {
        action1.action -= action2;
        return action1;
    }
    public void Invoke(T arg, U arg2)
    {

        if (action != null) {
            action.Invoke(arg, arg2);
        }
    }
}
public class SimpleActionEvent<T, U, O>
{
    private event Action<T, U, O> action;
    public static SimpleActionEvent<T, U, O> operator +(SimpleActionEvent<T, U, O> action1, Action<T, U, O> action2)
    {
        action1.action += action2;
        return action1;
    }
    public static SimpleActionEvent<T, U, O> operator -(SimpleActionEvent<T, U, O> action1, Action<T, U, O> action2)
    {
        action1.action -= action2;
        return action1;
    }
    public void Invoke(T arg, U arg2,O arg3)
    {

        if (action != null)
        {
            action.Invoke(arg, arg2, arg3);
        }
    }
}
