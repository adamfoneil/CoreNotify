using CoreNotify.Shared;

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
}
