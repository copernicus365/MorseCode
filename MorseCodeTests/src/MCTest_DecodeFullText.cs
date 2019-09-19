using System;
using MorseCodeNS;
using Xunit;

namespace MorseCodeTests
{
	public class MCTest_DecodeFullText
	{
		void DecodeFullText(string expected, string mcode, bool pass = true)
		{
			var morseCode = new MorseCode();

			string result = morseCode.DecodeFullTextToString(mcode);

			bool equals = result.Equals(expected, StringComparison.OrdinalIgnoreCase);
			Assert.True(equals == pass);
		}

		[Fact]
		public void HelloWorld()
			=> DecodeFullText("Hello World 123!",
			// this has multiple spaces, and at least one tab separators
			".... . .-.. .-.. --- / .-- --- .-. .-.. -.. / .---- ..--- ...-- -.-.--");

		[Fact]
		public void HiX()
			=> DecodeFullText("hi!", ".... .. -.-.--");

		[Fact]
		public void E()
			=> DecodeFullText("E", ".");

		[Fact]
		public void E_WSpaceMess()
			=> DecodeFullText("E", "  \t . ");

		[Fact]
		public void X()
			=> DecodeFullText("!", "-.-.--");

		[Fact]
		public void X_WSpaceMess()
			=> DecodeFullText("!", "  \t -.-.-- ");

		[Fact]
		public void HiThere_WSpaceMess()
			=> DecodeFullText("Hi there!", "  \r\n			....  .. /    - ....\t\t  . .-. .     -.-.--	\t ");

		[Fact]
		public void HiThereX()
			=> DecodeFullText("Hi there!", "  \r\n			....  .. /    - ....\t\t  . .-. .     -.-.--	\t ");

		[Fact]
		public void WSpaceMess()
			=> DecodeFullText("Hello World 123!",
				// this has multiple spaces, and at least one tab separators
				"  \r\n .... . .-.. .-.. --- / .-- --- .-. .-.. -.. / .---- ..--- ...-- -.-.--   ");

	}
}
