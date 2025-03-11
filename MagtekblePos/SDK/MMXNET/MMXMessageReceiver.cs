// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.MMXMessageReceiver
using System;
using System.Linq;


internal class MMXMessageReceiver : IMMXMessageReceiver
{
	protected IMMXService mService;

	protected MMXMessageComposer mComposer;

	private ActionQueue<int> receivingQueue = new ActionQueue<int>();

	private int index;

	public bool mDiscardExtraData { get; set; }

	public event EventHandler<byte[]> OnMessage;

	public event EventHandler OnCancel;

	public event EventHandler<string> OnError;

	public void initialize(IMMXService service)
	{
		mDiscardExtraData = true;
		mService = service;
		mComposer = new MMXMessageComposer(service);
		service.onData += Service_onData;
		mComposer.PayloadReceived += MComposer_PayloadReceived;
		mComposer.OnError += MComposer_OnError;
	}

	private void MComposer_OnError(object sender, string e)
	{
		this.OnError?.Invoke(this, e);
	}

	private void Service_onData_Queue(object sender, byte[] e)
	{
		receivingQueue.Queue(delegate
		{
			Service_onData(sender, e);
			return 0;
		});
	}

	private void Service_onData(object sender, byte[] e)
	{
		try
		{
			if (mDiscardExtraData)
			{
				mComposer.ParseReport(e, out var _);
				return;
			}
			byte[] array = new byte[e.Length];
			e.CopyTo(array, 0);
			while (array != null)
			{
				mComposer.ParseReport(array, out var Rest2);
				if (Rest2 != null && Rest2.Count() >= 3)
				{
					array = Rest2;
					continue;
				}
				break;
			}
		}
		catch (OperationCanceledException ex)
		{
			Debug.LogCommunication(ex.Message);
			this.OnCancel(this, null);
		}
		catch
		{
			Debug.LogError("Invalid data to parse");
		}
	}

	private void MComposer_PayloadReceived(object sender, byte[] e)
	{
		this.OnMessage?.Invoke(this, e);
	}
}
