// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MagTek.Equalizer<T>
using System;
using System.Collections.Generic;

internal class Equalizer<T> : IEqualityComparer<T> where T : IComparable<T>
{
	public bool Equals(T x, T y)
	{
		return x.CompareTo(y) == 0;
	}

	public int GetHashCode(T obj)
	{
		return obj.ToString().GetHashCode();
	}
}
