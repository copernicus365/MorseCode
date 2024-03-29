using System;
using System.Linq;

using BenchmarkDotNet.Attributes;

using MorseCodeNS;

namespace BenchmarkFun
{
	[MemoryDiagnoser]
	public class MorseCodeDecodeBench
	{
		public const string Value =

			// "Hello World 123!"
			//".... . .-.. .-.. --- / .-- --- .-. .-.. -.. / .---- ..--- ...-- -.-.--";

			// "In the beginning"
			".. -. / - .... . / -... . --. .. -. -. .. -. --. --..-- / --. --- -.. / -.-. .-. . .- - . -.. / - .... . / .... . .- ...- . -. ... / .- -. -.. / - .... . / . .- .-. - .... .-.-.- / - .... . / . .- .-. - .... / .-- .- ... / .-- .. - .... --- ..- - / ..-. --- .-. -- / .- -. -.. / ...- --- .. -.. --..-- / .- -. -.. / -.. .- .-. -.- -. . ... ... / .-- .- ... / --- ...- . .-. / - .... . / ..-. .- -.-. . / --- ..-. / - .... . / -.. . . .--. .-.-.- / .- -. -.. / - .... . / ... .--. .. .-. .. - / --- ..-. / --. --- -.. / .-- .- ... / .... --- ...- . .-. .. -. --. / --- ...- . .-. / - .... . / ..-. .- -.-. . / --- ..-. / - .... . / .-- .- - . .-. ... .-.-.- / .- -. -.. / --. --- -.. / ... .- .. -.. --..-- / .-..-. .-.. . - / - .... . .-. . / -... . / .-.. .. --. .... - --..-- .-..-. / .- -. -.. / - .... . .-. . / .-- .- ... / .-.. .. --. .... - .-.-.- / .- -. -.. / --. --- -.. / ... .- .-- / - .... .- - / - .... . / .-.. .. --. .... - / .-- .- ... / --. --- --- -.. .-.-.- / .- -. -.. / --. --- -.. / ... . .--. .- .-. .- - . -.. / - .... . / .-.. .. --. .... - / ..-. .-. --- -- / - .... . / -.. .- .-. -.- -. . ... ... .-.-.- / --. --- -.. / -.-. .- .-.. .-.. . -.. / - .... . / .-.. .. --. .... - / -.. .- -.-- --..-- / .- -. -.. / - .... . / -.. .- .-. -.- -. . ... ... / .... . / -.-. .- .-.. .-.. . -.. / -. .. --. .... - .-.-.- / .- -. -.. / - .... . .-. . / .-- .- ... / . ...- . -. .. -. --. / .- -. -.. / - .... . .-. . / .-- .- ... / -- --- .-. -. .. -. --. --..-- / - .... . / ..-. .. .-. ... - / -.. .- -.-- .-.-.-";

		[Benchmark]
		public char[] DecodeWDict()
		{
			MorseCode mc = new() {
				DecodeWithDictionary = true
			};
			char[] res = mc.DecodeFullText(Value).ToArray();
			return res;
		}

		[Benchmark]
		public char[] DecodeNormal()
		{
			MorseCode mc = new() {
				DecodeWithDictionary = false
			};
			char[] res = mc.DecodeFullText(Value).ToArray();
			return res;
		}

		public void AssertTestIsValid()
		{
			string v1 = new(DecodeNormal());
			string v2 = new(DecodeWDict());

			// "Hello World 123!"
			// "In the beginning"

			if(v1.Length < 1
				|| v1 != v2
				|| !(v1.StartsWith("In the beginning", StringComparison.OrdinalIgnoreCase)))
				throw new Exception("INVALID TEST!");
		}
	}
}
