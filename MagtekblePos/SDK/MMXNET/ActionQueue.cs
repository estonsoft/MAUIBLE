// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.ActionQueue<T>
using System;
using System.Threading;
using System.Threading.Tasks;

internal class ActionQueue<T>
{
	private Task<T> actionTask;

	private Semaphore accessSemaphore = new Semaphore(1, 1);

	public Task<T> Queue(Func<T> action)
	{
		accessSemaphore.WaitOne();
		if (actionTask == null || actionTask.IsCompleted)
		{
			actionTask = Task.Run(() => action());
		}
		else
		{
			actionTask = actionTask.ContinueWith((Task<T> x) => action());
		}
		Task<T> result = actionTask;
		accessSemaphore.Release();
		return result;
	}

	public Task<T> Queue<P1>(Func<P1, T> action, P1 p)
	{
		accessSemaphore.WaitOne();
		if (actionTask == null || actionTask.IsCompleted)
		{
			actionTask = Task.Run(() => action(p));
		}
		else
		{
			actionTask = actionTask.ContinueWith((Task<T> x) => action(p));
		}
		Task<T> result = actionTask;
		accessSemaphore.Release();
		return result;
	}

	public Task<T> Queue<P1, P2>(Func<P1, P2, T> action, P1 p1, P2 p2)
	{
		accessSemaphore.WaitOne();
		if (actionTask == null || actionTask.IsCompleted)
		{
			actionTask = Task.Run(() => action(p1, p2));
		}
		else
		{
			actionTask = actionTask.ContinueWith((Task<T> x) => action(p1, p2));
		}
		Task<T> result = actionTask;
		accessSemaphore.Release();
		return result;
	}

	public Task<T> Queue(Task<T> action)
	{
		accessSemaphore.WaitOne();
		if (actionTask == null || actionTask.IsCompleted)
		{
			actionTask = action;
		}
		else
		{
			actionTask = actionTask.ContinueWith(delegate
			{
				action.Wait();
				return action.Result;
			});
		}
		Task<T> result = actionTask;
		accessSemaphore.Release();
		return result;
	}
}
