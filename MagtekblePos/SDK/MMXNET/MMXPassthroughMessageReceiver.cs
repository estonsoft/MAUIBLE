// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.MMXPassthroughMessageReceiver
using System;


internal class MMXPassthroughMessageReceiver : IMMXMessageReceiver
{
	protected IMMXService mService;

	public bool mDiscardExtraData { get; set; }

	public event EventHandler<byte[]> OnMessage;

	public event EventHandler OnCancel;

	public event EventHandler<string> OnError;

	public void initialize(IMMXService service)
	{
		mService = service;
		service.onData += Service_onData;
	}

	private void Service_onData(object sender, byte[] e)
	{
		this.OnMessage?.Invoke(this, e);
	}
}
