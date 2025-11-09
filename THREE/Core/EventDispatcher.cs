namespace THREE
{
    public class Event : EventArgs
    {
        public string Type { get; }
        public object Target { get; set; }

        public Event(string type, object target = null)
        {
            Type = type;
            Target = target;
        }
    }

    public class EventDispatcher
    {
        private readonly Dictionary<string, List<EventHandler<Event>>> _listeners = new();

        public void AddEventListener(string type, EventHandler<Event> handler)
        {
            if (string.IsNullOrEmpty(type) || handler == null) return;

            if (!_listeners.TryGetValue(type, out var list))
            {
                list = new List<EventHandler<Event>>();
                _listeners[type] = list;
            }

            if (!list.Contains(handler))
                list.Add(handler);
        }

        public bool HasEventListener(string type, EventHandler<Event> handler)
        {
            if (string.IsNullOrEmpty(type) || handler == null) return false;
            return _listeners.TryGetValue(type, out var list) && list.Contains(handler);
        }

        public void RemoveEventListener(string type, EventHandler<Event> handler)
        {
            if (string.IsNullOrEmpty(type) || handler == null) return;

            if (_listeners.TryGetValue(type, out var list))
            {
                list.Remove(handler);
                if (list.Count == 0)
                    _listeners.Remove(type);
            }
        }

        public void DispatchEvent(Event _event)
        {
            if (string.IsNullOrEmpty(_event.Type)) return;

            if (!_listeners.TryGetValue(_event.Type, out var list)) return;

            var snapshot = list.ToArray();
            var ev = new Event(_event.Type, _event.Target);

            foreach (var handler in snapshot)
            {
                try
                {
                    handler?.Invoke(_event.Target, ev);
                }
                catch
                {
                    // 필요하면 로깅. 예외가 전체 디스패치를 중단하지 않도록 흡수.
                }
            }
        }
    }
}