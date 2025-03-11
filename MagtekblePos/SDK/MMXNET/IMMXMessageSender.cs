// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.IMMXMessageSender
using System;
internal interface IMMXMessageSender
{
	void initialize(IMMXService service);

	void send(MMXMessage message, Action<float> ProgressUpdate = null);

	void cancel();
}
