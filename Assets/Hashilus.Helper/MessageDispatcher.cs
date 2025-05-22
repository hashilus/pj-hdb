using System;
using System.Collections.Generic;

public class MessageDispatcher<MessageBaseT>
{
    interface IHandlerWrapper
    {
        void Dispatch(MessageBaseT message);
        Delegate WrappedHandler { get; }
    }

    readonly Dictionary<Type, List<IHandlerWrapper>> handlerDict = new Dictionary<Type, List<IHandlerWrapper>>();

    public void Dispatch(MessageBaseT message)
    {
        var handlers = handlerDict.GetOrDefault(message.GetType());
        if (handlers == null) return;
        handlers.ForEach(h => h.Dispatch(message));
    }

    public void AddListener<MessageT>(Action<MessageT> handler) where MessageT : MessageBaseT
    {
        var wrappedHandler = new HandlerWrapper<MessageT>(handler);

        var handlers = handlerDict.GetOrDefault(typeof(MessageT));
        if (handlers == null)
        {
            handlerDict[typeof(MessageT)] = new List<IHandlerWrapper>() { wrappedHandler };
        }
        else
        {
            handlerDict[typeof(MessageT)].Add(wrappedHandler);
        }
    }

    public void RemoveListener<MessageT>(Action<MessageT> handler) where MessageT : MessageBaseT
    {
        var handlers = handlerDict.GetOrDefault(typeof(MessageT));
        if (handlers == null)
        {
            return;
        }
        else
        {
            handlerDict[typeof(MessageT)].RemoveAll(h => ReferenceEquals(h.WrappedHandler, handler));
        }
    }

    class HandlerWrapper<MessageT> : IHandlerWrapper where MessageT : MessageBaseT
    {
        public readonly Action<MessageT> handler;

        public Delegate WrappedHandler { get { return handler; } }

        public HandlerWrapper(Action<MessageT> handler)
        {
            this.handler = handler;
        }

        public void Dispatch(MessageBaseT message)
        {
            handler((MessageT)message);
        }
    }
}


public class MessageDispatcher<MessageBaseT, Arg1T>
{
    interface IHandlerWrapper
    {
        void Dispatch(MessageBaseT message, Arg1T arg1);
        Delegate WrappedHandler { get; }
    }

    readonly Dictionary<Type, List<IHandlerWrapper>> handlerDict = new Dictionary<Type, List<IHandlerWrapper>>();

    public void Dispatch(MessageBaseT message, Arg1T arg1)
    {
        var handlers = handlerDict.GetOrDefault(message.GetType());
        if (handlers == null) return;
        handlers.ForEach(h => h.Dispatch(message, arg1));
    }

    public void AddListener<MessageT>(Action<MessageT, Arg1T> handler) where MessageT : MessageBaseT
    {
        var wrappedHandler = new HandlerWrapper<MessageT>(handler);

        var handlers = handlerDict.GetOrDefault(typeof(MessageT));
        if (handlers == null)
        {
            handlerDict[typeof(MessageT)] = new List<IHandlerWrapper>() { wrappedHandler };
        }
        else
        {
            handlerDict[typeof(MessageT)].Add(wrappedHandler);
        }
    }

    public void RemoveListener<MessageT>(Action<MessageT, Arg1T> handler) where MessageT : MessageBaseT
    {
        var handlers = handlerDict.GetOrDefault(typeof(MessageT));
        if (handlers == null)
        {
            return;
        }
        else
        {
            handlerDict[typeof(MessageT)].RemoveAll(h => ReferenceEquals(h.WrappedHandler, handler));
        }
    }

    class HandlerWrapper<MessageT> : IHandlerWrapper where MessageT : MessageBaseT
    {
        public readonly Action<MessageT, Arg1T> handler;

        public Delegate WrappedHandler { get { return handler; } }

        public HandlerWrapper(Action<MessageT, Arg1T> handler)
        {
            this.handler = handler;
        }

        public void Dispatch(MessageBaseT message, Arg1T arg1)
        {
            handler((MessageT)message, arg1);
        }
    }
}

public class MessageDispatcher<MessageBaseT, Arg1T, Arg2T>
{
    interface IHandlerWrapper
    {
        void Dispatch(MessageBaseT message, Arg1T arg1, Arg2T arg2);
        Delegate WrappedHandler { get; }
    }

    readonly Dictionary<Type, List<IHandlerWrapper>> handlerDict = new Dictionary<Type, List<IHandlerWrapper>>();

    public void Dispatch(MessageBaseT message, Arg1T arg1, Arg2T arg2)
    {
        var handlers = handlerDict.GetOrDefault(message.GetType());
        if (handlers == null) return;
        handlers.ForEach(h => h.Dispatch(message, arg1, arg2));
    }

    public void AddListener<MessageT>(Action<MessageT, Arg1T, Arg2T> handler) where MessageT : MessageBaseT
    {
        var wrappedHandler = new HandlerWrapper<MessageT>(handler);

        var handlers = handlerDict.GetOrDefault(typeof(MessageT));
        if (handlers == null)
        {
            handlerDict[typeof(MessageT)] = new List<IHandlerWrapper>() { wrappedHandler };
        }
        else
        {
            handlerDict[typeof(MessageT)].Add(wrappedHandler);
        }
    }

    public void RemoveListener<MessageT>(Action<MessageT, Arg1T, Arg2T> handler) where MessageT : MessageBaseT
    {
        var handlers = handlerDict.GetOrDefault(typeof(MessageT));
        if (handlers == null)
        {
            return;
        }
        else
        {
            handlerDict[typeof(MessageT)].RemoveAll(h => ReferenceEquals(h.WrappedHandler, handler));
        }
    }

    class HandlerWrapper<MessageT> : IHandlerWrapper where MessageT : MessageBaseT
    {
        public readonly Action<MessageT, Arg1T, Arg2T> handler;

        public Delegate WrappedHandler { get { return handler; } }

        public HandlerWrapper(Action<MessageT, Arg1T, Arg2T> handler)
        {
            this.handler = handler;
        }

        public void Dispatch(MessageBaseT message, Arg1T arg1, Arg2T arg2)
        {
            handler((MessageT)message, arg1, arg2);
        }
    }
}