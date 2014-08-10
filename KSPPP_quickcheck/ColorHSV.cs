using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using KSP;
using FsCheck;
using FsCheck.Fluent;

namespace KSPPP_quickcheck {
	public static class Extensions
	{
		public static IEnumerable<int> Insert(this IEnumerable<int> cs, int x)
		{
			var result = new List<int>(cs);
			foreach (var c in cs)
			{
				if (x <= c)
				{
					result.Insert(result.IndexOf(c), x);
					return result;
				}
			}
			result.Add(x);
			return result;
		}

		public static bool IsOrdered<T>(this IEnumerable<T> source)
		{
			//by Jon Skeet
			//I was too lazy to write it myself, and wondered whether a prettier 
			//solution might exist in C# than the one I had in mind.
			//Here's your answer...
			var comparer = Comparer<T>.Default;
			T previous = default(T);
			bool first = true;

			foreach (T element in source)
			{
				if (!first && comparer.Compare(previous, element) > 0)
				{
					return false;
				}
				first = false;
				previous = element;
			}
			return true;
		}
	}

	class Program
	{
		public static void Main(/*string[] args*/) {
			Spec.ForAny<int[]>(xs => xs.Reverse().Reverse().SequenceEqual( xs ))
				.QuickCheck("RevRev");

			Spec.ForAny <Color> (c => ((Color) ((Color32) c)).Equals(c)).QuickCheck("Color -> Color32 -> Color");

			Spec.ForAny<int[]>(xs => xs.Reverse().SequenceEqual(xs))
				.QuickCheck("RevId");
		}

		public class ArbitraryColor : Arbitrary<Color> {
			public override Gen<Color> Generator {
				get {
					return (Gen<Color>)null;
				}
			}
		}


		public class ArbitraryLong : Arbitrary<long> {
			public override Gen<long> Generator {
				get { 
					return (Gen<long>)Any.OfSize (s => Any.IntBetween (-s, s)).Select (i => Convert.ToInt64 (i)); 
				}
			}
		}

		// Analysis disable once ConvertToStaticType
		public class MyArbitraries
		{
			public static Arbitrary<long>			Long() 			{ return new ArbitraryLong(); }
			public static Arbitrary<IEnumerable<T>>	Enumerable<T>() { return Arb.Default.Array<T>().Convert(x => (IEnumerable<T>)x, x => (T[])x); }
			public static Arbitrary<StringBuilder>	StringBuilder() { return Any.OfType<string>().Select(x => new StringBuilder(x)).ToArbitrary(); }
		}
	}
}
