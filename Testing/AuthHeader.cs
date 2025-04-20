using CoreNotify.Shared;
using System.Diagnostics;

namespace Testing;

[TestClass]
public class AuthHeader
{
	[TestMethod]
	public void EncodeDecode()
	{
		var value = AuthorizationHeader.Encode("myemail@nowhere.org", "1234567");
		var (email, apiKey) = AuthorizationHeader.Decode(value);
		Assert.AreEqual("myemail@nowhere.org", email);
		Assert.AreEqual("1234567", apiKey);
	}

	[TestMethod]
	[DataRow("adamfoneil@proton.me", "zdHNSW0P0WipkrKuhvcBPsYpSV3bqwi7")]
	public void Encode(string email, string apiKey)
	{
		var value = AuthorizationHeader.Encode(email, apiKey);
		Debug.Print(value);
	}
}
