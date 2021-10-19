using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractural.FastEvent
{
    public class EmptyEventArgs { }

	public class FastEvent : FastEvent<EmptyEventArgs>
    {
        public void Invoke()
        {
            Invoke(new EmptyEventArgs());
		}
	}
    
    public class FastEvent<TArgs>
    {
        public List<IFastEventListener<TArgs>> Listeners { get; private set; } = new List<IFastEventListener<TArgs>>();

        public void AddListener(IFastEventListener<TArgs> listener)
        {
            Listeners.Add(listener);
        }

        public bool RemoveListener(IFastEventListener<TArgs> listener)
        {
            return Listeners.Remove(listener);
        }

        public void Invoke(TArgs args)
        {
            foreach (IFastEventListener<TArgs> listener in Listeners)
                listener.Listen(args);
        }
    }

    public interface IFastEventListener : IFastEventListener<EmptyEventArgs> { }

    public interface IFastEventListener<TArgs>
    {
        void Listen(TArgs args);
    }
}
