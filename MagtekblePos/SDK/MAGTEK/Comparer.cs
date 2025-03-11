// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MagTek.Comparer<T>
using System;
using System.Collections.Generic;

public class Comparer<T> : IEqualityComparer<T>
{
	protected Func<T, T, int> comparer;

	public Comparer(Func<T, T, int> comparer)
	{
		this.comparer = comparer;
	}

	public bool Equals(T x, T y)
	{
		return comparer(x, y) == 0;
	}

	public int GetHashCode(T obj)
	{
		return obj.ToString().GetHashCode();
	}

	public static implicit operator Comparer<T>(Func<T, T, int> f)
	{
		return new Comparer<T>(f);
	}
}
