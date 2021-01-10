using EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventBus.Sample
{
    public class SampleEventBus : IEventBus
    {
        private static Dictionary<string, List<object>> eventAndEventHandlers = new Dictionary<string, List<object>>(); //事件源与事件处理的映射字典

        public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
        {
            var eventTypeName = typeof(TEvent).FullName;
            if (eventAndEventHandlers.ContainsKey(eventTypeName))
            {
                var handlers = eventAndEventHandlers[eventTypeName];
                try
                {
                    foreach (var handler in handlers)
                    {
                        MethodInfo meth = handler.GetType().GetMethod("Handle");
                        meth.Invoke(handler, new object[] { @event });
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : IEvent
        {
            var eventTypeName = typeof(TEvent).FullName;
            if (eventAndEventHandlers.ContainsKey(eventTypeName))
            {
                var handlers = eventAndEventHandlers[eventTypeName];
                handlers.Add(eventHandler);
            }
            else
            {
                eventAndEventHandlers.Add(eventTypeName, new List<object> { eventHandler });
            }
        }

        public void Unsubscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : IEvent
        {
            var eventTypeName = typeof(TEvent).FullName;
            if (eventAndEventHandlers.ContainsKey(eventTypeName))
            {
                var handlers = eventAndEventHandlers[eventTypeName];
                if (handlers != null && handlers.Exists(s => s.GetType() == eventHandler.GetType()))
                {
                    var handlerToRemove = handlers.First(s => s.GetType() == eventHandler.GetType());
                    handlers.Remove(handlerToRemove);
                }
            }
        }
    }
}
