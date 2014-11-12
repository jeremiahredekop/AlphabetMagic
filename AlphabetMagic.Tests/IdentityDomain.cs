using System;

namespace AlphabetMagic.Tests
{

	public class Identity: AggregateRoot<IdentityState>
	{
		public Identity ()
		{
			Behavior<RegisterIdentity>(c=> new IdentityRegistered{
				CreatedOn = DateTime.Now,
				DateOfBirth = c.DateOfBirth,
				Identity = Id,
				Name = c.Name
			});

			Behavior<ChangeIdentityName>(c=> {

				if(State.NameChangeCount > 5) throw new Exception("You can't change the name more than 5 times");

				return new  IdentityNameChanged
				{
					ChangeCount = State.NameChangeCount,
					Identity = Id,
					Name = c.Name,
					PriorName = State.CurrentName,
					UpdatedOn = DateTime.Now,
					Reason = c.Reason
				};
			});
		}
	}

	public class RegisterIdentity : ICommand
	{
		public string Name {get;set;} 
		public DateTime DateOfBirth {get;set;}
		public Guid Identity { get; set;}
	}

	public class IdentityRegistered : IEvent
	{
		public string Name {get;set;} 
		public DateTime DateOfBirth {get;set;}
		public Guid Identity { get; set;}
		public DateTime CreatedOn {get;set;}
	}

	public class ChangeIdentityName : ICommand
	{
		public string Name {get;set;} 
		public Guid Identity { get; set;}
		public string Reason {get;set;}
	}

	public class IdentityNameChanged : IEvent
	{
		public string Name {get;set;} 
		public string PriorName {get;set;}
		public Guid Identity { get; set;}
		public DateTime UpdatedOn {get;set;}
		public int ChangeCount {get;set;}
		public string Reason {get;set;}
	}
		
	public class IdentityState : State
	{
		public string CurrentName {	get;set; }

		public IdentityState ()
		{
			OnEvent<IdentityNameChanged>(e=> 
				{
					NameChangeCount ++;
					CurrentName = e.Name;
				});

			OnEvent<IdentityRegistered>(e=> CurrentName = e.Name);
		}

		public int NameChangeCount {get;set;}

	}
}

