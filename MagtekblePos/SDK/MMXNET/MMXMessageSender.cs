// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.MMXMessageSender
using System;


internal class MMXMessageSender : IMMXMessageSender
{
	protected IMMXService mService;

	protected MMXMessageComposer mComposer;

	public void initialize(IMMXService service)
	{
		mService = service;
		mComposer = new MMXMessageComposer(service);
	}

	public void send(MMXMessage message, Action<float> ProgressUpdate = null)
	{
		mComposer.SendPayload(message.getData(), ProgressUpdate);
	}

	public void cancel()
	{
		mComposer.SendCancel();
	}
}
