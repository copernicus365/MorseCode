using System;
using BenchmarkDotNet.Running;
using BenchmarkFun;

namespace MorseCodeBenchs
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			Run_MorseCode1();
		}
		
		static void Run_MorseCode1()
		{
			new MorseCodeDecodeBench().AssertTestIsValid();

			var summary = BenchmarkRunner.Run<MorseCodeDecodeBench>();
		}

	}
}
