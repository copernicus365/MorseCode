using MorseCodeNS;
using Xunit;

namespace MorseCodeTests
{
	public class MCTest_EncodeFullText
	{
		void EncodeFullText(string input, string expectedMorse, bool pass = true)
		{
			var morseCode = new MorseCode();

			string morseCodeResult = morseCode.EncodeFullText(input);

			bool equals = morseCodeResult == expectedMorse;
			Assert.True(equals == pass);
		}

		[Fact]
		public void HelloWorld()
			=> EncodeFullText("Hello World!", ".... . .-.. .-.. --- / .-- --- .-. .-.. -.. -.-.--");

		[Fact]
		public void LotsOfMessedWS()
			=> EncodeFullText("\t Hi there\r\nFriend! ", "\t .... .. / - .... . .-. . /\r\n..-. .-. .. . -. -.. -.-.-- / ");

		[Fact]
		public void SpecialChars()
			=> EncodeFullText("Ich möchten oder träumen, nicht español", ".. -.-. .... / -- ---. -.-. .... - . -. / --- -.. . .-. / - .-. .-.- ..- -- . -. --..-- / -. .. -.-. .... - / . ... .--. .- --.-- --- .-..");


		[Fact]
		public void HandleInvalidMorseChars1()
			=> EncodeFullText("Hello Αλφα sir!", ".... . .-.. .-.. --- / # # # # / ... .. .-. -.-.--");


	}
}
