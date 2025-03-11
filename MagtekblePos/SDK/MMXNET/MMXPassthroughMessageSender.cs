// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.MMXPassthroughMessageSender
using System;


internal class MMXPassthroughMessageSender : IMMXMessageSender
{
	protected IMMXService mService;

	public void cancel()
	{
	}

	public void initialize(IMMXService service)
	{
		mService = service;
	}

	public void send(MMXMessage message, Action<float> ProgressUpdate = null)
	{
		mService.sendData(message.getData());
		ProgressUpdate?.Invoke(1f);
	}
}
