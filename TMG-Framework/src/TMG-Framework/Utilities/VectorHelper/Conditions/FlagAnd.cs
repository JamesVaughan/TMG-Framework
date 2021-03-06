﻿/*
    Copyright 2015-2016 Travel Modelling Group, Department of Civil Engineering, University of Toronto

    This file is part of XTMF.

    XTMF is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    XTMF is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with XTMF.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Numerics;
using System.Threading.Tasks;
using static System.Numerics.Vector;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace TMG.Utilities
{
    public static partial class VectorHelper
    {
        /// <summary>
        /// Set the value to one if the condition is met.
        /// </summary>
        public static void FlagAnd(float[] dest, float value, float[] data)
        {
            // check if we are supposed to just clear everything and use a faster function for that
            if (value == 0.0f)
            {
                Array.Clear(dest, 0, dest.Length);
            }
            else
            {
                var length = dest.Length;
                var vectorLength = length / Vector<float>.Count;
                var remainder = length % Vector<float>.Count;
                var destSpan = (new Span<float>(dest, 0, length - remainder)).NonPortableCast<float, Vector<float>>();
                var dataSpan = (new Span<float>(data, 0, length - remainder)).NonPortableCast<float, Vector<float>>();
                var valueV = new Vector<float>(value);
                Vector<float> zero = Vector<float>.Zero;
                int i = 0;
                for (; i < vectorLength - 1; i += 2)
                {
                    destSpan[i] = ConditionalSelect(System.Numerics.Vector.Equals(dataSpan[i], zero), zero, valueV);
                    destSpan[i + 1] = ConditionalSelect(System.Numerics.Vector.Equals(dataSpan[i + 1], zero), zero, valueV);
                }
                i *= Vector<float>.Count;
                for (; i < data.Length; i++)
                {
                    dest[i] = data[i] == 0 ? 0.0f : value;
                }
            }
        }

        /// <summary>
        /// Set the value to one if the condition is met.
        /// </summary>
        public static void FlagAnd(float[] dest, int destIndex, float value, float[] data, int dataIndex, int length)
        {
            // check if we are supposed to just clear everything and use a faster function for that
            if (value == 0.0f)
            {
                Array.Clear(dest, destIndex, length);
            }
            else
            {
                var vectorLength = length / Vector<float>.Count;
                var remainder = length % Vector<float>.Count;
                var destSpan = (new Span<float>(dest, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
                var dataSpan = (new Span<float>(data, dataIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
                var valueV = new Vector<float>(value);
                Vector<float> zero = Vector<float>.Zero;
                int i = 0;
                for (; i < vectorLength - 1; i += 2)
                {
                    destSpan[i] = ConditionalSelect(System.Numerics.Vector.Equals(dataSpan[i], zero), zero, valueV);
                    destSpan[i + 1] = ConditionalSelect(System.Numerics.Vector.Equals(dataSpan[i + 1], zero), zero, valueV);
                }
                i *= Vector<float>.Count;
                for (; i < length; i++)
                {
                    dest[destIndex + i] = data[dataIndex + i] == 0 ? 0.0f : value;
                }
            }
        }

        /// <summary>
        /// Set the value to one if the condition is met.
        /// </summary>
        public static void FlagAnd(float[] dest, int destIndex, float[] lhs, int lhsIndex, float[] rhs, int rhsIndex, int length)
        {
            var vectorLength = length / Vector<float>.Count;
            var remainder = length % Vector<float>.Count;
            var destSpan = (new Span<float>(dest, destIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var lhsSpan = (new Span<float>(lhs, lhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            var rhsSpan = (new Span<float>(rhs, rhsIndex, length - remainder)).NonPortableCast<float, Vector<float>>();
            Vector<float> zero = Vector<float>.Zero;
            int i = 0;
            for (; i < vectorLength - 1; i += 2)
            {
                destSpan[i] = ConditionalSelect(System.Numerics.Vector.Equals(lhsSpan[i], zero), zero, rhsSpan[i]);
                destSpan[i + 1] = ConditionalSelect(System.Numerics.Vector.Equals(lhsSpan[i + 1], zero), zero, rhsSpan[i + 1]);
            }
            i *= Vector<float>.Count;
            for (; i < length; i++)
            {
                dest[destIndex + i] = lhs[lhsIndex + i] == 0 ? 0.0f : rhs[rhsIndex + i];
            }
        }

        /// <summary>
        /// Set the value to one if the condition is met.
        /// </summary>
        public static void FlagAnd(float[][] dest, float[][] data, float literalValue)
        {
            Parallel.For(0, dest.Length, i =>
            {
                FlagAnd(dest[i], data[i], literalValue);
            });
        }

        /// <summary>
        /// Set the value to one if the condition is met.
        /// </summary>
        public static void FlagAnd(float[] dest, float[] data, float literalValue)
        {
            FlagAnd(dest, literalValue, data);
        }

        /// <summary>
        /// Set the value to one if the condition is met.
        /// </summary>
        public static void FlagAnd(float[][] dest, float[][] lhs, float[][] rhs)
        {
            Parallel.For(0, dest.Length, i =>
            {
                FlagAnd(dest[i], 0, lhs[i], 0, rhs[i], 0, dest.Length);
            });
        }

        /// <summary>
        /// Set the value to one if the condition is met.
        /// </summary>
        public static void FlagAnd(float[][] v1, float literalValue, float[][] v2)
        {
            Parallel.For(0, v1.Length, i =>
            {
                FlagAnd(v1[i], literalValue, v2[i]);
            });
        }
    }
}
