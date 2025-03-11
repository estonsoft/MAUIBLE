// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.IMMXMessageReceiver
using System;


internal interface IMMXMessageReceiver
{
	bool mDiscardExtraData { get; set; }

	event EventHandler<byte[]> OnMessage;

	event EventHandler OnCancel;

	event EventHandler<string> OnError;

	void initialize(IMMXService service);
}
