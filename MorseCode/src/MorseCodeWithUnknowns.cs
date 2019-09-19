// Authored by Nicholas Petersen, 2019

using System;
using System.Collections.Generic;
using System.Linq;

namespace MorseCodeNS
{
	public class MorseCodeWithUnknowns
	{
		bool?[] _morse;
		int _depth0;
		char[] _depthArr;
		int _lastAddedCount;

		public Action<char> AddToOutput { get; private set; }
		public List<char> Output { get; set; }

		public MorseCodeWithUnknowns(Action<char> addToOutput)
			=> AddToOutput = addToOutput ?? throw new ArgumentNullException(nameof(addToOutput));


		public static string GetPossibilities(string mcodeEOrs)
		{
			bool?[] boolQuery = MorseCode.BitsStringToBoolsWithUnknowns(mcodeEOrs);

			var outputSB = new System.Text.StringBuilder(20);

			var m = new MorseCodeWithUnknowns(c => outputSB.Append(c));

			int added = m.DecodeCharWithUnknowns(boolQuery);

			string possibilities = outputSB.ToString();
			return possibilities;
		}


		/// <summary>
		/// Returns the count of chars that were possible decodings.
		/// NOTE: This uses state, is NOT multi-threaded.
		/// </summary>
		/// <param name="morse">Input, which may include uncertain signals
		/// (represented as null inputs).</param>
		public int DecodeCharWithUnknowns(bool?[] morse)
		{
			_lastAddedCount = 0;

			int depth = morse?.Length ?? -1;
			if(depth < 1 || depth > MorseCode.MCodeTreeDepth)
				return 0;  // for later, signal deeper than we encoded (or just out of range)

			int _lastAdded = 0;
			_morse = morse;
			_depth0 = depth - 1; // make depth index of 0 based (simplifies calculations below, removes bunch of "-1s")
			_depthArr = MorseCode.BTrees[_depth0];

			// note this LINQ expression adds one perf hit, tho idea is this is still likely faster
			bool noUnknowns = !morse.Any(m => m == null);
			if(noUnknowns)
				_FindFastNoEitherOrs();
			else
				_FindWithEitherOrs(0, 0);

			return _lastAdded;
		}


		char AddOutput(int idx)
		{
			char v = _depthArr[idx];
			if(v != '~') {
				_lastAddedCount++;
				AddToOutput(v);
			}
			return v;
		}

		/// <summary>
		/// Recursive magic ;)
		/// </summary>
		/// <param name="depth"></param>
		/// <param name="finalIdx"></param>
		void _FindWithEitherOrs(int depth, int finalIdx)
		{
			bool? m = _morse[depth];

			// code is just clearer setting these named vars at the top
			bool left = m == false || m == null;
			bool right = m == true || m == null;

			if(depth == _depth0) {
				if(left) {
					AddOutput(finalIdx);
				}
				if(right) {
					// on "+ 1", same as PowerOf2 at this point, but is just 1, no need
					AddOutput(finalIdx + 1);
				}
			}
			else {
				if(left) {
					// no change in `depthArrIdx` when moved left
					_FindWithEitherOrs(depth + 1, finalIdx);
				}
				if(right) {
					// move `depthArrIdx` by 2^(_depth0 - depth)
					int addIdx = MorseCode.PowerOf2(_depth0 - depth);
					_FindWithEitherOrs(depth + 1, finalIdx + addIdx);
				}
			}
		}

		/// <summary>
		/// Is the heart of <see cref="MorseCode.DecodeChar(bool[])"/> above, just
		/// with setup / error-checking already out of the way, and importantly,
		/// inputing a nullable bool array, otherwise would have to alloc a new
		/// bool array every time.
		/// </summary>
		char _FindFastNoEitherOrs()
		{
			int finalIdx = 0;
			for(int i = 0; i <= _depth0; i++) {
				if(_morse[i] == true)
					finalIdx += MorseCode.PowerOf2(_depth0 - i);
			}
			return AddOutput(finalIdx);
		}
	}
}
