// Authored by Nicholas Petersen, 2019

using System;
using System.Collections.Generic;
using System.Linq;

namespace MorseCodeNS
{
	public class MorseCode
	{
		// Morse-code tree image: https://upload.wikimedia.org/wikipedia/commons/c/ca/Morse_code_tree3.png

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
		/// We never ended up using this, but not to have / to visualize with,
		/// and it still might well be utilized.
		/// </summary>
		public static readonly (int rowLen, int flatIdx)[] BinaryRows;

		static MorseCode()
		{
			(char[] btree, char[][] btrees, (int rowLen, int flatIdx)[] binaryRows) = GetMorseCodeDataStructures();

			BTree = btree;
			BTrees = btrees;
			BinaryRows = binaryRows;

			if(BTrees.Length != MCodeTreeDepth) // Depth = BTrees.Length;
				throw new ArgumentException();
		}

		/// <summary>
		/// Beautiful. So our plan of attack is to represent in data-structures the
		/// binary-tree of morse-code. We'll then be able to traverse this tree
		/// using some standard binary-search methods, or if not exactly that,
		/// still any solution will ultimately correlate in some ways to binary searching.
		/// This is most fitting, as morse-code is indeed a pure binary representation, with
		/// bits of 0s (., dots / 'dits') and 1s ('-', dashes / 'dahs'). 
		/// </summary>
		/// <returns></returns>
		public static (char[] btree, char[][] btrees, (int rowLen, int flatIdx)[] binaryRows)
			GetMorseCodeDataStructures()
		{
			const int numRows = MCodeTreeDepth;

			char[] btree = BTreeString.Where(c => c != ' ' && c != '|' && c != '\r' && c != '\n').ToArray();

			char[][] btrees = new char[numRows][];

			var binRowsL = new List<(int idx, int len)>();

			for(int i = 0; i < numRows; i++) {

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

			(int idx, int len)[] binRows = binRowsL.ToArray();

			for(int i = 0; i < numRows; i++) {

				(int flatIdx, int rowLen) = binRows[i];

				char[] row = btrees[i] = new char[rowLen];

				for(int j = 0; j < rowLen; j++) {
					int rowIdx = flatIdx + j;
					row[j] = btree[rowIdx];
				}
			}

			return (btree, btrees, binRows);
		}


		/// <summary>
		/// readonly means this can never be set to bool, or not to size of 6,
		/// though vals can change
		/// </summary>
		readonly bool[] _buffer = new bool[MCodeTreeDepth];

		/// <summary>
		/// Full text morse-code decoder, extremely HIGH-PERFORMANCE! Internally
		/// uses a single (boolean) buffer to decode the (string) morse values to, so each
		/// input letter does NOT stupidly alloc hugely wasteful arrays per hit.
		/// In fact ZERO heap allocs are made in this. Of course the decoding then is
		/// the big thing, and that I believe is extremely fast. But compared to a string-
		/// based dictionary lookup approach, it's just going to have to be profiled to
		/// know for sure.
		/// </summary>
		public IEnumerable<char> DecodeFull(string morse)
		{
			if(morse == null) throw new ArgumentNullException(nameof(morse));

			int len = morse.Length;
			int j = 0;
			char val = char.MinValue;

			bool handleBreak()
			{
				if(j > 0) {
					val = Find(_buffer, j);
					j = -1;
					return true;
				}
				j = -1;
				return false;
			}

			for(int i = 0; i < len; i++, j++) {

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
					case '.': _buffer[j] = false; break;
					case '-': _buffer[j] = true; break;
					case '0': _buffer[j] = false; break;
					case '1': _buffer[j] = true; break;
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

		public string DecodeFullToString(string morse)
		{
			char[] result = DecodeFull(morse).ToArray();

			string resultStr = new string(result);

			return resultStr;
		}

		public bool[] SetBufferFromBitsString(string bitsStr)
		{
			int len = bitsStr.Length;
			for(int i = 0; i < len; i++)
				_buffer[i] = BitsCharToBool(bitsStr[i]);
			return _buffer;
		}

		public char Find(string morse)
			=> Find(SetBufferFromBitsString(morse), morse.Length);


		public char Find(bool[] morse)
			=> Find(morse, morse?.Length ?? 0);

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
		public char Find(bool[] morse, int len)
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
