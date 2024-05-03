using System;
using System.Collections.Concurrent;

public class EventCenter
{
    private static ConcurrentDictionary<string, Action> m_EventTable = new ConcurrentDictionary<string, Action>();

    public static void RegisterListener(string eventName, Action listener)
    {
        m_EventTable.AddOrUpdate(eventName, listener, (key, existingValue) => existingValue += listener);
    }

    public static void RemoveListener(string eventName, Action listener)
    {
        if (!m_EventTable.ContainsKey(eventName)) return;
        m_EventTable[eventName] -= listener;
    }

    public static void TriggerEvent(string eventName)
    {
        if (!m_EventTable.TryGetValue(eventName, out Action listener)) return;
        listener?.Invoke();
    }
}
