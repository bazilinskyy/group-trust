using System;
using System.Collections.Generic;


namespace Barmetler
{
    public abstract class InValidatable
    {
	    /// <summary>
	    ///     These objects will also be invalidated if this one is. Looping children are not a problem.
	    /// </summary>
	    public readonly List<InValidatable> children = new();


        public void Invalidate(Stack<InValidatable> callStack = null)
        {
            callStack ??= new Stack<InValidatable>();

            if (callStack.Contains(this))
            {
                return;
            }

            OnInvalidate();
            callStack.Push(this);

            foreach (var child in children)
            {
                child.Invalidate(callStack);
            }

            callStack.Pop();
        }


        public abstract void OnInvalidate();
    }


    /// <summary>
    ///     Caches data depending on some context. Good for preventing expensive computations, if the data it depends on has
    ///     not been changed.
    /// </summary>
    public class ContextDataCache<DataType, ContextType> : InValidatable
    {
        private readonly Dictionary<int, DataType> data = new();


        public void SetData(DataType data, ContextType context)
        {
            this.data[context.GetHashCode()] = data;
        }


        public DataType GetData(ContextType context)
        {
            if (!IsValid(context))
            {
                throw new Exception("Cache is invalid");
            }

            return data[context.GetHashCode()];
        }


        public override void OnInvalidate()
        {
            data.Clear();
        }


        public bool IsValid(ContextType context)
        {
            return data.ContainsKey(context.GetHashCode());
        }
    }
}