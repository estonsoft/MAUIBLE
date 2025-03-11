// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.EventPublisher
using System;
using System.Collections.Generic;


public class EventPublisher : IEventPublisher
{
	private List<IEventSubscriber> mEventSubscriberList = new List<IEventSubscriber>();

	public bool addSubscriber(IEventSubscriber eventSubscriber)
	{
		mEventSubscriberList.Add(eventSubscriber);
		return true;
	}

	public bool removeSubscriber(IEventSubscriber eventSubscriber)
	{
		mEventSubscriberList.Remove(eventSubscriber);
		return true;
	}

	protected virtual void sendEvent(EventType eventType, IData data)
	{
		foreach (IEventSubscriber mEventSubscriber in mEventSubscriberList)
		{
			try
			{
				mEventSubscriber.OnEvent(eventType, data);
			}
			catch (Exception)
			{
			}
		}
	}
}
