﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Utilities
{
    public static class DynamicEqualityComparerLinqIntegration
    {
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> func) where T : class
        {
            return source.Distinct(new DynamicEqualityComparer<T>(func));
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IEnumerable<IGrouping<TSource, TSource>> GroupBy<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, bool> func) where TSource : class
        {
            return source.GroupBy(t => t, new DynamicEqualityComparer<TSource>(func));
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IEnumerable<T> Intersect<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> func) where T : class
        {
            return first.Intersect(second, new DynamicEqualityComparer<T>(func));
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IEnumerable<T> Except<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> func) where T : class
        {
            return first.Except(second, new DynamicEqualityComparer<T>(func));
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool Contains<T>(this IEnumerable<T> source, T value, Func<T, T, bool> func) where T : class
        {
            return source.Contains(value, new DynamicEqualityComparer<T>(func));
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> other, Func<TSource, TSource, bool> func) where TSource : class
        {
            return source.SequenceEqual(other, new DynamicEqualityComparer<TSource>(func));
        }
    }
}
