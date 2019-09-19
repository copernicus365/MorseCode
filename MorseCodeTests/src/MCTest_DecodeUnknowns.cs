using MorseCodeNS;
using Xunit;

namespace MorseCodeTests
{
	public class MCTest_DecodeUnknowns
	{
		void TestEitherOr(string mcodeEOrs, string expected, bool pass = true)
		{
			string vals = MorseCodeWithUnknowns.GetPossibilities(mcodeEOrs);

			bool equals = vals == expected;
			Assert.True(equals == pass);
		}


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

		[Fact]
		public void EOR_Row5() => TestEitherOr("??-??", "ÉÐ2ÞÀĴ1ÇĤ890");

		[Fact]
		public void EOR_Row6() => TestEitherOr("??-???", "?_@';!(:");

	}
}
