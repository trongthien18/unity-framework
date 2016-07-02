using UnityEngine;
using System.Collections;
using System;

public class SimpleEvent
{
    private Action m_Delegate = () => { };
    public void Add(Action aDelegate)
    {
        m_Delegate += aDelegate;
    }
    public void Remove(Action aDelegate)
    {
        m_Delegate -= aDelegate;
    }
    public void Run()
    {
        m_Delegate();
    }
}
