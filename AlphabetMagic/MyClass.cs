using System;
using System.Collections.Generic;
using System.Linq;

namespace AlphabetMagic
{
	public interface IReactive
	{
		IEnumerable<Type> GetSupportedMessageTypes();
	}

	public interface IEvent	{}

	public interface ICommand	{}

	public interface IState
	{
		void Apply(IEvent @event);
	}

	public interface IAggregateRoot
	{
		void Do(ICommand command);
		IEnumerable<IEvent> GetUncommittedEvents();
		void Apply (IEvent @event);
	}

	public class AggregateRoot<TState> : 
		IReactive,
		IAggregateRoot where TState : IState, new()
	{
		IEnumerable<Type> IReactive.GetSupportedMessageTypes ()
		{
			return _handlers.Keys;
		}
			
		protected readonly TState State = new TState();
		readonly List<IEvent> _uncommittedEvents = new List<IEvent>();

		readonly Dictionary<Type,Func<ICommand, IEnumerable<IEvent>>> _handlers = new Dictionary<Type, Func<ICommand,IEnumerable<IEvent>>> ();

		public Guid Id {get; protected set;}

		protected void Behaviors<T>(Func<T, IEnumerable<IEvent>> action) where T: ICommand
		{
			_handlers [typeof(T)] = e => action((T)e);
		}

		protected void Behavior<T>(Func<T, IEvent> action) where T: ICommand
		{
			Behaviors<T>(e => {
			var @event = action((T)e);
				return new IEvent[] {@event};
			});
		}
			
		protected void Raise(IEvent @event)
		{
			_uncommittedEvents.Add (@event);
		}

		public void Do (ICommand command)
		{
			_handlers [command.GetType ()] (command);
		}

		IEnumerable<IEvent> IAggregateRoot.GetUncommittedEvents ()
		{
			return _uncommittedEvents.ToArray ();
		}

		void IAggregateRoot.Apply (IEvent @event)
		{
			State.Apply (@event);
		}
	}

	public class State: IState, IReactive
	{
		readonly Dictionary<Type,Action<IEvent>> _handlers 	= new Dictionary<Type, Action<IEvent>> ();

		void IState.Apply (IEvent @event)
		{
			_handlers [@event.GetType ()] (@event);
		}
			
		protected void ProjectFrom<T>(Action <T> handler) where T: IEvent
		{
			_handlers [typeof(T)] = e => handler((T)e);
		}

		IEnumerable<Type> IReactive.GetSupportedMessageTypes ()
		{
			return _handlers.Keys;
		}
	}
}

