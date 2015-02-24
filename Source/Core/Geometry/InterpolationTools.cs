﻿using System;

namespace CodeImp.DoomBuilder.Geometry
{
	
	
	public static class InterpolationTools
	{
		public enum Mode
		{
			LINEAR,
			EASE_IN_OUT_SINE,
			EASE_IN_SINE,
			EASE_OUT_SINE,
		}

		public static int Interpolate(float val1, float val2, float delta, Mode mode)
		{
			switch (mode)
			{
				case Mode.LINEAR: return Linear(val1, val2, delta);
				case Mode.EASE_IN_SINE: return EaseInSine(val1, val2, delta);
				case Mode.EASE_OUT_SINE: return EaseOutSine(val1, val2, delta);
				case Mode.EASE_IN_OUT_SINE: return EaseInOutSine(val1, val2, delta);
				default: throw new NotImplementedException("InterpolationTools.Interpolate: '" + mode + "' mode is not supported!");
			}
		}
		
		//Based on Robert Penner's original easing equations (http://www.robertpenner.com/easing/)
		public static int Linear(float val1, float val2, float delta)
		{
			return (int)(delta * val2 + (1.0f - delta) * val1);
		}
		
		/**
		 * Easing equation function for a sinusoidal (sin(t)) easing in: accelerating from zero velocity.
		 */
		public static int EaseInSine(float val1, float val2, float delta) 
		{
			float f_val1 = val1;
			float f_val2 = val2 - f_val1;
			return (int)(-f_val2 * Math.Cos(delta * Angle2D.PIHALF) + f_val2 + f_val1);
		}

		/**
		 * Easing equation function for a sinusoidal (sin(t)) easing out: decelerating from zero velocity.
		 */
		public static int EaseOutSine(float val1, float val2, float delta) 
		{
			return (int)((val2 - val1) * Math.Sin(delta * Angle2D.PIHALF) + val1);
		}

		/**
		 * Easing equation function for a sinusoidal (sin(t)) easing in/out: acceleration until halfway, then deceleration.
		 */
		public static int EaseInOutSine(float val1, float val2, float delta)
		{
			return (int)Math.Round(-(val2 - val1) / 2 * (float)(Math.Cos(Math.PI * delta) - 1) + val1);
		}
	}
}