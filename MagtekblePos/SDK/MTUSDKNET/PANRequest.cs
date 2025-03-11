// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.PANRequest
using System.Collections.Generic;


public class PANRequest
{
	public byte Timeout { get; set; }

	public List<PaymentMethod> PaymentMethods { get; set; }

	public PANRequest()
	{
		Timeout = 60;
		PaymentMethods = null;
	}

	public PANRequest(byte timeout, List<PaymentMethod> paymentMethods)
	{
		Timeout = timeout;
		PaymentMethods = paymentMethods;
	}
}
