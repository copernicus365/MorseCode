using MorseCodeNS;
using Xunit;

namespace MorseCodeTests
{
	public class MCTest_DecodeChar
	{
		void DecodeChar(string mcode, char expected, bool pass = true)
		{
			var morseCode = new MorseCode();
			char val = morseCode.DecodeChar(mcode);
			bool equals = val == expected;
			Assert.True(equals == pass);
		}

		[Fact]
		public void Norm_E() => DecodeChar(".", 'E');

		[Fact]
		public void Norm_T() => DecodeChar("-", 'T');

		[Fact]
		public void Norm_I() => DecodeChar("..", 'I');

		[Fact]
		public void Norm_S() => DecodeChar("...", 'S');

		[Fact]
		public void Norm_A() => DecodeChar("01", 'A');

		[Fact]
		public void Norm_N() => DecodeChar("-.", 'N');

		[Fact]
		public void Norm_M() => DecodeChar("11", 'M');

		[Fact]
		public void Norm_K() => DecodeChar("101", 'K');

		[Fact]
		public void Norm_O() => DecodeChar("---", 'O');

		[Fact]
		public void Norm_P() => DecodeChar(".--.", 'P');

		[Fact]
		public void Norm_Zero_FiveTrues() => DecodeChar("-----", '0');

		[Fact]
		public void Norm_Row5_ExclMrk() => DecodeChar("-.-.--", '!');

		[Fact]
		public void Norm_Row5_Semicolon() => DecodeChar("-.-.-.", ';');

		[Fact]
		public void Norm_Row5_ExclMrk_Fail()
			// input is for excl mark, semi-colon is one to the left
			=> DecodeChar("-.-.--", ';', false);


	}
}
