// Authored by Nicholas Petersen, 2019

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MorseCodeNS
{
	public class MorseCode
	{
		// Morse-code binary tree image: https://upload.wikimedia.org/wikipedia/commons/c/ca/Morse_code_tree3.png

		/// <summary>
		/// All the morse code data drives from this singular input.
		/// </summary>
		public const string BTreeString =
@"ET
IANM
SURWDKGO
HVFÜLÄPJ BXCYZQÖŠ
54Ŝ3É~Ð2 ~È+~ÞÀĴ1 6=/~Ç~Ĥ~ 7~ĜÑ8~90
~~~~~~~~ ~~~~?_~~ ~~""~~.~~ ~~@~~~'~ ~~-~~~~~ ~~;!~(~~ ~~~,~~~~ :~~~~~~~";

		public const int MCodeTreeDepth = 6;

		public static readonly char[] BTree;
		public static readonly char[][] BTrees;

		/// <summary>
		/// ! This will contain an EXTREMELY fast reverse lookup table,
		/// allowing lookup conversions from text to morse code. It's
		/// extremely doubtful there could be a faster way of doing this.
		/// Also one huge benefit is this also handles upper and lower case
		/// so one does not have to upper or lower case input text first.
		/// One cost is there is a little waste in this table, as it will
		/// be 354 long, but only 107 of the (jagged) arrays will be set,
		/// so there will be 247 null values in the 0 level array here.
		/// But super small beans, and if we were not trying to allow the full
		/// Morse Code set (i.e. excluding the special characters), this
		/// would have been much shorter still, but that small cost is worth it.
		/// <para />
		/// To use this, one simply has to
		/// 1) get the integer value for a given char,
		/// 2) validate that it is less than <see cref="ReverseLookupTableLength"/>, and
		/// 3) index into this table with that value, thereby
		/// 4) retrieving and using the boolean array which tells the morse values for
		/// that char.
		/// </summary>
		static readonly MorseCodeVal[] ReverseLookupTable;

		static readonly Dictionary<string, char> EncRevDict;

		public const int ReverseLookupTableLength = 354;

		/// <summary>
		/// We never ended up using this, but nice to have / to visualize with,
		/// and it still might well be utilized.
		/// </summary>
		public static readonly (int rowLen, int flatIdx)[] BinaryRows;

		/* --- Visual Printout Of Reverse Table (generate with: GetDisplayOfReverseLookupValues) ---
(omitting the many null ones)

[33] ! ("-.-.--")
[34] " (".-..-.")
[39] ' (".----.")
[40] ( ("-.--.-")
[43] + (".-.-.")
[44] , ("--..--")
[45] - ("-...-.")
[46] . (".-.-.-")
[47] / ("-..-.")
[48] 0 ("-----")
[49] 1 (".----")
[50] 2 ("..---")
[51] 3 ("...--")
[52] 4 ("....-")
[53] 5 (".....")
[54] 6 ("-....")
[55] 7 ("--...")
[56] 8 ("---..")
[57] 9 ("----.")
[58] : ("---...")
[59] ; ("-.-.-.")
[61] = ("-...-")
[63] ? ("..--..")
[64] @ (".--.-.")
[65] A (".-")
[66] B ("-...")
[67] C ("-.-.")
[68] D ("-..")
[69] E (".")
[70] F ("..-.")
[71] G ("--.")
[72] H ("....")
[73] I ("..")
[74] J (".---")
[75] K ("-.-")
[76] L (".-..")
[77] M ("--")
[78] N ("-.")
[79] O ("---")
[80] P (".--.")
[81] Q ("--.-")
[82] R (".-.")
[83] S ("...")
[84] T ("-")
[85] U ("..-")
[86] V ("...-")
[87] W (".--")
[88] X ("-..-")
[89] Y ("-.--")
[90] Z ("--..")
[95] _ ("..--.-")
[97] a (".-")
[98] b ("-...")
[99] c ("-.-.")
[100] d ("-..")
[101] e (".")
[102] f ("..-.")
[103] g ("--.")
[104] h ("....")
[105] i ("..")
[106] j (".---")
[107] k ("-.-")
[108] l (".-..")
[109] m ("--")
[110] n ("-.")
[111] o ("---")
[112] p (".--.")
[113] q ("--.-")
[114] r (".-.")
[115] s ("...")
[116] t ("-")
[117] u ("..-")
[118] v ("...-")
[119] w (".--")
[120] x ("-..-")
[121] y ("-.--")
[122] z ("--..")
[192] À (".--.-")
[196] Ä (".-.-")
[199] Ç ("-.-..")
[200] È (".-..-")
[201] É ("..-..")
[208] Ð ("..--.")
[209] Ñ ("--.--")
[214] Ö ("---.")
[220] Ü ("..--")
[222] Þ (".--..")
[224] à (".--.-")
[228] ä (".-.-")
[231] ç ("-.-..")
[232] è (".-..-")
[233] é ("..-..")
[240] ð ("..--.")
[241] ñ ("--.--")
[246] ö ("---.")
[252] ü ("..--")
[254] þ (".--..")
[284] Ĝ ("--.-.")
[285] ĝ ("--.-.")
[292] Ĥ ("-.--.")
[293] ĥ ("-.--.")
[308] Ĵ (".---.")
[309] ĵ (".---.")
[348] Ŝ ("...-.")
[349] ŝ ("...-.")
[352] Š ("----")
[353] š ("----")
*/

		class MorseCodeVal
		{
			public char Val;
			public string MCode;
			public bool[] Bits;
		}

		/// <summary>
		/// Beautiful. So our plan of attack is to represent in data-structures the
		/// binary-tree of morse-code. We'll then be able to traverse this tree
		/// using some standard binary-search methods, or if not exactly that,
		/// still any solution will ultimately correlate in some ways to binary searching.
		/// This is most fitting, as morse-code is indeed a pure binary representation, with
		/// bits of 0s (., dots / 'dits') and 1s ('-', dashes / 'dahs').
		/// </summary>
		static MorseCode()
		{
			BTree = BTreeString
				.Where(c => c != ' ' && c != '|' && c != '\r' && c != '\n')
				.ToArray();

			BTrees = new char[MCodeTreeDepth][];

			var binRowsL = new List<(int idx, int len)>();

			for(int i = 0; i < MCodeTreeDepth; i++) {
				if(i == 0) {
					binRowsL.Add((0, 2));
				}
				else {
					(int lastIdx, int lastLen) = binRowsL[i - 1];

					int idx = lastIdx + lastLen;
					int len = lastLen * 2;

					binRowsL.Add((idx, len));
				}
			}

			BinaryRows = binRowsL.ToArray();

			for(int i = 0; i < MCodeTreeDepth; i++) {

				(int flatIdx, int rowLen) = BinaryRows[i];

				char[] row = BTrees[i] = new char[rowLen];

				for(int j = 0; j < rowLen; j++) {
					int rowIdx = flatIdx + j;
					row[j] = BTree[rowIdx];
				}
			}

			if(BTrees.Length != MCodeTreeDepth)
				throw new ArgumentException();

			ReverseLookupTable = new MorseCodeVal[ReverseLookupTableLength];
			var mc = new MorseCode();

			for(int i = 0; i < 2; i++)
				SetReverseLookupTable(0, i, new MorseCodeVal() { Bits = new bool[0] }, ReverseLookupTable, mc);

			EncRevDict = new Dictionary<string, char>(ReverseLookupTableLength);

			foreach(MorseCodeVal rvt in ReverseLookupTable)
				if(rvt != null)
					EncRevDict[rvt.MCode] = rvt.Val;

			EncRevDict["\\"] = ' ';
			EncRevDict["\n"] = '\n';
			EncRevDict["\r"] = char.MinValue;
		}

		public static string GetDisplayOfReverseLookupValues()
		{
			var sb = new StringBuilder(2048);

			for(int i = 0; i < ReverseLookupTableLength; i++) {

				MorseCodeVal revTbl = ReverseLookupTable[i];

				if(revTbl != null) {

					bool[] morseBits = revTbl.Bits;

					sb.Append("[")
						.Append(i)
						.Append("] ")
						.Append((char)i)
						.Append(" (\"")
						.Append(revTbl.MCode)
						.Append("\")")
						.AppendLine();

					//for(int j = 0; j < morseBits.Length; j++) {
					//	bool bVal = morseBits[j];
					//	sb.Append(bVal ? '-' : '.');
					//}
				}
			}

			string disp = sb.ToString();
			return disp;
		}

		/// <summary>
		/// Recursive function that singularly builds the ReverseLookupTable
		/// via the already set morse binary trees (nicely shares raw data input
		/// without having to repeat).
		/// </summary>
		/// <param name="depth0"></param>
		/// <param name="idx"></param>
		/// <param name="parentVal"></param>
		/// <param name="revLkpTable"></param>
		static void SetReverseLookupTable(int depth0, int idx, MorseCodeVal parentVal, MorseCodeVal[] revLkpTable, MorseCode mc)
		{
			if(depth0 >= MCodeTreeDepth)
				return;

			char val = BTrees[depth0][idx];

			// EVEN when val is not used (== '~'), some morse table values have a null parent
			bool isOdd_ThusTrue = idx % 2 != 0;

			bool[] newBits = parentVal.Bits.Concat(new bool[] { isOdd_ThusTrue }).ToArray();

			MorseCodeVal newRTVal = new MorseCodeVal() {
				Val = mc.DecodeChar(newBits),
				Bits = newBits,
				MCode = new string(newBits.Select(b => b ? '-' : '.').ToArray()),		
			};

			if(val != '~') {
				int valNm = val;
				if(valNm >= revLkpTable.Length)
					throw new ArgumentOutOfRangeException();

				char valLower = char.ToLower(val);
				int valLwrNm = valLower;
				if(valLwrNm >= revLkpTable.Length)
					throw new ArgumentOutOfRangeException();

				revLkpTable[valNm] = newRTVal;
				revLkpTable[valLwrNm] = newRTVal;
			}

			int childDepth0 = depth0 + 1;

			if(childDepth0 < MCodeTreeDepth) {
				int childLeftIdx = idx * 2;
				int thisDepthLen = PowerOf2(depth0 + 1);

				// set left
				SetReverseLookupTable(childDepth0, childLeftIdx, newRTVal, revLkpTable, mc);
				// set right
				SetReverseLookupTable(childDepth0, childLeftIdx + 1, newRTVal, revLkpTable, mc);
			}
		}



		#region --- ENCODE ---

		public string EncodeFullText(string text)
		{
			var sb = new StringBuilder(capacity: text.Length * 5);

			EncodeFullText(text, c => sb.Append(c), str => sb.Append(str));

			string val = sb.ToString();
			return val;
		}

		public void EncodeFullText(string text, Action<char> writeChar, Action<string> writeStr)
		{
			if(string.IsNullOrEmpty(text))
				return;

			int len = text.Length;
			int lastI = len - 1;
			int lastWS = -1;

			for(int i = 0; i < len; i++) {
				char c = text[i];

				void writeWordBreakIfLastWasNotWS()
				{
					if(lastWS != i - 1)
						writeChar('/');
					lastWS = i;
				}

				if(c < ReverseLookupTableLength) {
					MorseCodeVal rvTbl = ReverseLookupTable[c];

					if(rvTbl != null) {

						bool[] mbits = rvTbl.Bits;

						if(mbits.Length < 3) {
							for(int j = 0; j < mbits.Length; j++) {
								bool bit = mbits[j];
								writeChar(bit ? '-' : '.');
							}
						}
						else {
							writeStr(rvTbl.MCode);
						}

						if(i != lastI)
							writeChar(' ');

						continue;
					}
					else {
						switch(c) {
							case ' ':
								writeWordBreakIfLastWasNotWS();
								writeChar(' ');
								break;
							case '\t':
								writeWordBreakIfLastWasNotWS();
								writeChar('\t');
								break;
							case '\n':
								writeWordBreakIfLastWasNotWS();
								writeChar('\r');
								writeChar('\n');
								break;
							case '\r':
								break;
								// ignore, \r must ALWAYS be followed by \n anyways, just pick \n up next
						}
					}
				}
				else {
					writeChar('#');
					if(i != lastI)
						writeChar(' ');
				}

				// can reach here two ways:
				// 1) char was out of range
				// 2) in range but still had no morse value
				// what to do?!

				// throw???
			}
		}

		#endregion



		#region --- DECODE ---

		public char DecodeChar(string morse)
			=> DecodeChar(SetBufferFromBitsString(morse), morse.Length);

		public char DecodeChar(bool[] morse)
			=> DecodeChar(morse, morse?.Length ?? 0);

		/// <summary>
		/// *visually* meditating on the tree-chart helped me to realize how this can be
		/// done this way. Basically it's about getting down to the proper depth
		/// (so if 4 morse code points / 4 true/falses, then dealing with 4th row / array
		/// i.e. starting with index-depth (3 instead of 4) power of and working down, thus:
		/// 'K':[1,0,1] (depth 3), == 2^2 [4] + [0: false stays 0] + 2^0 [1] = index 5
		/// 'O':[1,1,1] (depth 3), == 2^2 + 2^1 + 2^0 = index 7
		/// visually, if you have all falses, then no calculations ever are even needed,
		/// so '...' will be arrayRow3[0], or: BTrees["...".Length - 1][0]
		/// </summary>
		/// <param name="morse">Bit code points, false is a dot, true a dash.
		/// For high-perf concerns, set the length below allowing this array
		/// to be a buffer that gets reused for every letter.</param>
		/// <param name="len">Send in a length if the input array is being reused
		/// as a buffer input (MUCH preferred if decoding more than just a few letters).</param>
		public char DecodeChar(bool[] morse, int len)
		{
			if(morse == null) throw new ArgumentNullException(nameof(morse));
			if(len < 1 || len > MCodeTreeDepth)
				throw new ArgumentOutOfRangeException(nameof(morse));

			int depth0 = len - 1;
			int finalIdx = 0;

			for(int i = 0; i <= depth0; i++) {
				if(morse[i] == true) {
					int addPowerOf = PowerOf2Arr[depth0 - i];
					finalIdx += addPowerOf;
				}
			}

			char val = BTrees[depth0][finalIdx];
			return val;
		}

		public string DecodeFullTextToString(string morse)
		{
			char[] result = DecodeFullText(morse).ToArray();

			string resultStr = new string(result);

			return resultStr;
		}

		public bool DecodeWithDictionary;

		//static char[] DictSplitMCForDecode = { ' ', '\t', '\r', '\n' };
		//public IEnumerable<char> DecodeFullTextWithDict(string morse)
		//{
		//	if(morse == null) throw new ArgumentNullException(nameof(morse));
		//	//string[] vals = morse.Split(DictSplitMCForDecode,);
		//	//for(int i = 0; i < vals.Length; i++) {
		//	//}
		//}

		//static string GetMCFromBools(bool[] mc, int len)
		//{
		//	for(int i = 0; i < len; i++)
		//		;
		//}

		/// <summary>
		/// Full text morse-code decoder, extremely HIGH-PERFORMANCE! Internally
		/// uses a single (boolean) buffer to decode the (string) morse values to, so each
		/// input letter does NOT stupidly alloc hugely wasteful arrays per hit.
		/// In fact ZERO heap allocs are made in this. Of course the decoding then is
		/// the big thing, and that I believe is extremely fast. But compared to a string-
		/// based dictionary lookup approach, it's just going to have to be profiled to
		/// know for sure.
		/// </summary>
		public IEnumerable<char> DecodeFullText(string morse)
		{
			if(morse == null) throw new ArgumentNullException(nameof(morse));

			int len = morse.Length;
			int j = 0;
			int i = 0;
			char val = char.MinValue;

			bool handleBreak()
			{
				if(j > 0) {
					if(DecodeWithDictionary) {
						string ky = morse.Substring(i - j, j);
						j = -1;				
						val = EncRevDict[ky];
						return val != char.MinValue;
					}

					val = DecodeChar(_encBuffer, j);
					j = -1;
					return true;
				}
				j = -1;
				return false;
			}

			for(; i < len; i++, j++) {

				char c = morse[i];

				if(j > 5) {
					// explanation: if the last decoded char had the fullest possible
					// 6 dits/dahs, there MUST be a break following it if we've reached
					// this point (but that IS valid). We just have to make sure that
					// nothing else tries to ADD to the buffer now. No need though
					// handling those cases, still let below do the handling for DRY,
					// we only have to validate no buffer adds are next up
					switch(c) {
						case '.':
						case '-':
						case '0':
						case '1':
							throw new ArgumentOutOfRangeException("Found a morse code value that was greater than 6 chars");
					}
				}

				switch(c) {
					case '.': _encBuffer[j] = false; break;
					case '-': _encBuffer[j] = true; break;
					case '0': _encBuffer[j] = false; break;
					case '1': _encBuffer[j] = true; break;
					case ' ':
					case '\t':
					case '\r':
					case '\n':
						if(handleBreak())
							yield return val;
						break;
					case '/':
					case '|':
						if(handleBreak())
							yield return val;
						yield return ' ';
						break;
					default:
						throw new ArgumentOutOfRangeException("blaH!");
				}
			}

			if(handleBreak())
				yield return val;
		}

		/// <summary>
		/// readonly means this can never be set to bool, or not to size of 6,
		/// though vals can change
		/// </summary>
		readonly bool[] _encBuffer = new bool[MCodeTreeDepth];

		#endregion



		public bool[] SetBufferFromBitsString(string bitsStr)
		{
			int len = bitsStr.Length;
			for(int i = 0; i < len; i++)
				_encBuffer[i] = BitsCharToBool(bitsStr[i]);
			return _encBuffer;
		}



		#region --- Helpers ---

		public static int PowerOf2(int pow)
			=> PowerOf2Arr[pow];

		/// <summary>
		/// When looking up power of 2, a lot faster / easier to lookup this way
		/// instead of calculating each time, which takes a loop adding up the values
		/// per the power of (see <see cref="IntPow"/> below). Math.Pow also works
		/// natively with doubles, ugly.
		/// </summary>
		static readonly int[] PowerOf2Arr = { 1, 2, 4, 8, 16, 32, 64, 128, 256 };

		/// <summary>
		/// https://stackoverflow.com/a/383596/264031
		/// </summary>
		/// <param name="num"></param>
		/// <param name="pow"></param>
		/// <returns></returns>
		public static int IntPow(int num, int pow)
		{
			int ret = 1;
			while(pow != 0) {
				if((pow & 1) == 1)
					ret *= num;
				num *= num;
				pow >>= 1;
			}
			return ret;
		}

		public static bool BitsCharToBool(char c)
		{
			switch(c) {
				case '.': return false;
				case '-': return true;
				case '0': return false;
				case '1': return true;
				default: throw new ArgumentOutOfRangeException();
			}
		}

		public static bool? BitsCharToBoolWithUnknown(char c)
		{
			switch(c) {
				case '0':
				case '.':
					return false;
				case '1':
				case '-':
					return true;
				case '?':
				case '~':
					return (bool?)null;
				default: throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Converts bits (0 or 1s) or morse code (dots / dashses) string
		/// to bool array. Both can be mixed, e.g. "0-.1" returns
		/// `{ false, true, false, true }`.
		/// </summary>
		public static bool[] BitsStringToBools(string bitsStr)
			=> bitsStr?.Select(c => BitsCharToBool(c)).ToArray();

		/// <summary>
		/// A nullable input version of overload above, allowing input of '?' or '~'
		/// which are interpretted as nulls.
		/// </summary>
		public static bool?[] BitsStringToBoolsWithUnknowns(string bitsStr)
			=> bitsStr?.Select(c => BitsCharToBoolWithUnknown(c)).ToArray();

		#endregion

	}
}
