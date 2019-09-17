using System;
using System.Collections.Generic;
using System.Linq;
using MorseCodeNS;
using Xunit;

namespace MorseCodeTests
{
	public class MorseCodeTests
	{
		void Find(string mcode, char expected, bool pass = true)
		{
			var morseCode = new MorseCode();
			char val = morseCode.Find(mcode);
			bool equals = val == expected;
			Assert.True(equals == pass);
		}

		void FindMany(string expected, string mcode, bool pass = true)
		{
			var morseCode = new MorseCode();

			string result = morseCode.DecodeFullToString(mcode);

			bool equals = result.Equals(expected, StringComparison.OrdinalIgnoreCase);
			Assert.True(equals == pass);
		}

		void TestEitherOr(string mcodeEOrs, string expected, bool pass = true)
		{
			bool?[] boolQuery = MorseCode.BitsStringToBoolsWithUnknowns(mcodeEOrs);

			var m = new MorseCodeWithUnknowns();

			int added = m.FindWithEitherOrs(boolQuery);

			string vals = new string(m.GetLastAdded().ToArray());

			bool equals = vals == expected;
			Assert.True(equals == pass);
		}

		[Fact]
		public void FullTest_HelloWorld()
			=> FindMany("Hello World 123!",
				// this has multiple spaces, and at least one tab separators
				".... . .-.. .-.. --- / .-- --- .-. .-.. -.. / .---- ..--- ...-- -.-.--");

		[Fact]
		public void FullText_HiX()
			=> FindMany("hi!", ".... .. -.-.--");

		[Fact]
		public void FullText_E()
			=> FindMany("E", ".");

		[Fact]
		public void FullText_E_WSpaceMess()
			=> FindMany("E", "  \t . ");

		[Fact]
		public void FullText_X()
			=> FindMany("!", "-.-.--");

		[Fact]
		public void FullText_X_WSpaceMess()
			=> FindMany("!", "  \t -.-.-- ");

		[Fact]
		public void FullText_HiThere_WSpaceMess()
			=> FindMany("Hi there!", "  \r\n			....  .. /    - ....\t\t  . .-. .     -.-.--	\t ");

		[Fact]
		public void FullText_HiThereX()
			=> FindMany("Hi there!", "  \r\n			....  .. /    - ....\t\t  . .-. .     -.-.--	\t ");

		[Fact]
		public void FullText_WSpaceMess()
			=> FindMany("Hello World 123!",
				// this has multiple spaces, and at least one tab separators
				"  \r\n .... . .-.. .-.. --- / .-- --- .-. .-.. -.. / .---- ..--- ...-- -.-.--   ");


		[Fact]
		public void Norm_E() => Find(".", 'E');

		[Fact]
		public void Norm_T() => Find("-", 'T');

		[Fact]
		public void Norm_I() => Find("..", 'I');

		[Fact]
		public void Norm_S() => Find("...", 'S');

		[Fact]
		public void Norm_A() => Find("01", 'A');

		[Fact]
		public void Norm_N() => Find("-.", 'N');

		[Fact]
		public void Norm_M() => Find("11", 'M');

		[Fact]
		public void Norm_K() => Find("101", 'K');

		[Fact]
		public void Norm_O() => Find("---", 'O');

		[Fact]
		public void Norm_P() => Find(".--.", 'P');

		[Fact]
		public void Norm_Zero_FiveTrues() => Find("-----", '0');

		[Fact]
		public void Norm_Row5_ExclMrk() => Find("-.-.--", '!');

		[Fact]
		public void Norm_Row5_Semicolon() => Find("-.-.-.", ';');

		[Fact]
		public void Norm_Row5_ExclMrk_Fail()
			=> Find("-.-.--", ';', false); // input is for excl mark, semi-colon is one to the left




		[Fact]
		public void EOR_TQFT() => TestEitherOr("1?01", "XQ");

		[Fact]
		public void EOR_FQQF() => TestEitherOr("0~~0", "HFLP");

		[Fact]
		public void EOR_K() => TestEitherOr("101", "K");

		[Fact]
		public void EOR_QFQ() => TestEitherOr("?-?", "RWGO");

		[Fact]
		public void EOR_QFQ_Fail() => TestEitherOr("?-?", "ZWGO", false);

		[Fact]
		public void EOR_QQT() => TestEitherOr("??-", "UWKO");
		[Fact]
		public void EOR_QQQ() => TestEitherOr("???", "SURWDKGO");

		[Fact]
		public void EOR_QQQ_Fail() => TestEitherOr("???", "SURWDIGO", false);

		[Fact]
		public void EOR_TFQ() => TestEitherOr("-.?", "DK");

	}
}
