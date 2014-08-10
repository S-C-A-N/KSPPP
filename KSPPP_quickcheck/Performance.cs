using System;
using System.Diagnostics;

using System.Runtime.InteropServices;

namespace KSPPP_quickcheck
{
	public class Performance
	{
		public Performance ()
		{
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct mes64 {
			[FieldOffset(52)]	public UInt64 mantissa;	// 52
			[FieldOffset(11)]	public UInt64 exponent;	// 11
			[FieldOffset(1)]	public UInt64 sign;		// 1
		}
		[StructLayout(LayoutKind.Explicit)]
		public struct mes32 {
			[FieldOffset(23)]	public UInt32 mantissa; //= 23;
			[FieldOffset(11)]	public UInt32 exponent;	//=  8;
			[FieldOffset(1)]	public UInt32 sign;		//=  1;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct Double_t {
			[FieldOffset(0)]	public Int64	i; 
			[FieldOffset(0)]	public double	f;
			[FieldOffset(0)]	public mes64	parts;

			public Double_t(double d0) {
				parts.exponent = 0;
				parts.mantissa = 0;
				parts.sign = 0;
				i = 0;
				f = d0;
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct Float_t {

			public Float_t(float f0) {
				parts.exponent = 0;
				parts.mantissa = 0;
				parts.sign = 0;
				i = 0;
				f = f0;
			}
			[FieldOffset(0)]	public Int32 	i;
			[FieldOffset(0)]	public float 	f;
			[FieldOffset(0)]	public mes64 	parts;
		}

		public struct TwoFloats {
			public float away;
			public float toward;

			public TwoFloats(float a, float b) {
				away = a;
				toward = b;
			}
		}

		public struct TwoDoubles {
			public double away;
			public double toward;

			public TwoDoubles(double a, double b) {
				away = a;
				toward = b;
			}
		}

		void IterateAllPositiveFloats()  {
			Float_t allFloats = new Float_t(0.0f);
			allFloats.f = 0.0f;

			Console.WriteLine(String.Format("{0,1:e8}",allFloats.f));
			while (allFloats.parts.exponent < 255) { 
				allFloats.i += 1;
				Console.WriteLine(String.Format("{0,1:e8}",allFloats.f));
			} 
		}

		[ConditionalAttribute("DEBUG")]
		public static void Assert(bool b) { }

		public static float TestFloatPrecisionAwayFromZero(float input) 
		{
			Float_t num = new Float_t(input);
			num.f = input;
			Assert(num.parts.exponent < 255); 
			num.i += 1;
			float delta = num.f - input; 
			return delta; 
		} 

		public static float TestFloatPrecisionTowardZero(float input) 
		{
			Float_t num = new Float_t (input);
			num.f = input;
			Assert(num.parts.exponent != 0 || num.parts.mantissa != 0);
			Assert(num.parts.exponent != 255 || num.parts.mantissa == 0);
			num.i += 1;
			float delta = num.f - input; 
			return delta; 
		} 

		public static double TestDoublePrecisionAwayFromZero(double input) 
		{
			Double_t num = new Double_t(input);
			num.f = input;
			Assert(num.parts.exponent < 2047); 
			num.i += 1;
			double delta = num.f - input; 
			return delta; 
		} 

		public static double TestDoublePrecisionTowardZero(double input) 
		{
			Double_t num = new Double_t(input);
			num.f = input;
			Assert(num.parts.exponent != 0 || num.parts.mantissa != 0);
			Assert(num.parts.exponent != 2047 || num.parts.mantissa == 0);
			num.i += 1;
			double delta = num.f - input; 
			return delta; 
		} 

		public static TwoFloats TestFloatPrecision(float input) {
			TwoFloats result;
			result.away = TestFloatPrecisionAwayFromZero(input);
			result.toward = TestFloatPrecisionTowardZero(input);
			return result;
		}

		public static TwoDoubles TestDoublePrecision(double input) {
			TwoDoubles result;
			result.away = TestDoublePrecisionAwayFromZero(input);
			result.toward = TestDoublePrecisionTowardZero(input);
			return result;
		}
		public static void Main() {
			TestFloatPrecision(65536);



		}
	}
}