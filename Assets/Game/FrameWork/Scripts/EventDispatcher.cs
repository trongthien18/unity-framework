using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum EventID
{
    None = 0,
    OnSpaceShipShoot,
    OnBulletHit,
    OnEnemyDie,
    OnAudioDBValueHit,
    OnAudioDBValueChange,
    OnTriangleEnemyHit,
    OnEarthHit,  
    OnMultiplierChange, 
    OnGameOver 
}

public class EventDispatcher : Singleton<EventDispatcher>
{
    Dictionary<EventID, List<Action<Component, object>>> listenerDict = new Dictionary<EventID, List<Action<Component, object>>>();

    // Register to listen for eventID
    public void RegisterListener(EventID eventID, Action<Component, object> callback)
    {
        MyLog.Assert(callback == null, "[RegisterListener]: Event {0} Callback is null!!!", eventID.ToString());
        
        if (listenerDict.ContainsKey(eventID))
        {
            listenerDict[eventID].Add(callback);
        }
        else
        {
            var listCallback = new List<Action<Component, object>>();
            listCallback.Add(callback);
            listenerDict.Add(eventID, listCallback);
        }
    }

    // Posts the event. This will notify all listener that register for this event
    public void PostEvent(EventID eventID, Component sender, object param = null)
    {
        MyLog.Assert(sender == null, "[PostEvent]: Event {0} Sender is null!!!", eventID.ToString());

        List<Action<Component, object>> actionList;
        if (listenerDict.TryGetValue(eventID, out actionList))
        {
            for (int i = 0, amount = actionList.Count; i < amount; i++)
            {
                try
                {
                    actionList[i](sender, param);
                }
                catch (Exception e)
                {
                    MyLog.LogWarning(this, "[PostEvent] Error when post event {0}, message: {1}", eventID.ToString(), e.ToString());
                    actionList.RemoveAt(i);
                    if (actionList.Count == 0)
                    {
                        listenerDict.Remove(eventID);
                    }
                    amount--;
                    i--;
                }
            }
        }
        else
        {
            MyLog.LogWarning(this, "[PostEvent] No listener for event {0}", eventID.ToString());
        }
    }

    // Removes the listener. Use to Unregister listener
    public void RemoveListener(EventID eventID, Action<Component, object> callback)
    {
        MyLog.Assert(callback == null, "[RemoveListener] Event {0} Callback is null!!!", eventID.ToString());

        List<Action<Component, object>> actionList;
        if (listenerDict.TryGetValue(eventID, out actionList))
        {
            if (actionList.Contains(callback))
            {
                actionList.Remove(callback);
                if (actionList.Count == 0)
                {
                    listenerDict.Remove(eventID);
                }
            }
        }
        else
        {
            MyLog.LogWarning(this, "[RemoveListener] No listener for event {0}", eventID.ToString());
        }
    }

    // Clean the ListenerList, remove the listener that have a null target. This happen when an object that
    // already be "delete" in Hirachy, but still have a callback remain in listenerList
    public void RemoveRedundancies()
    {
        foreach (var keyPairs in listenerDict)
        {
            var listenerList = keyPairs.Value;
            for (int amount = listenerList.Count, i = amount - 1; i >= 0; i--)
            {
                var listener = listenerList[i];
                // Use Target.Equal(null) instead of Target == null, it won't work
                if (listener == null || listener.Target.Equals(null))
                {
                    listenerList.RemoveAt(i);
                    if (listenerList.Count == 0)
                    {
                        // no listener remain, then delete this key
                        listenerDict.Remove(keyPairs.Key);
                    }
                    i--;
                }
            }
        }
    }

    // Clear all listener
    public void ClearAllListener()
    {
        listenerDict.Clear();
    }
}

#region Extension Class
public static class EventDispatcherExtension
{
    // Use for registering with EventsManager
    public static void RegisterListener(this MonoBehaviour sender, EventID eventID, Action<Component, object> callback)
    {
        EventDispatcher.Instance.RegisterListener(eventID, callback);
    }

    // Post event with param
    public static void PostEvent(this MonoBehaviour sender, EventID eventID, object param)
    {
        EventDispatcher.Instance.PostEvent(eventID, sender, param);
    }


    // Post event with no param (param = null)
    public static void PostEvent(this MonoBehaviour sender, EventID eventID)
    {
        EventDispatcher.Instance.PostEvent(eventID, sender, null);
    }
} 
#endregion