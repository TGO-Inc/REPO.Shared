using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Returns a random element from the enumerable collection that matches the predicate.
    /// <para>Includes null check (Unity safe).</para>
    /// </summary>
    /// <param name="enumerable"><see cref="Enumerable"/>&lt;<see cref="T"/>&gt;</param>
    /// <param name="predicate"></param>
    /// <typeparam name="T"><see cref="T"/>?</typeparam>
    /// <returns>A single random element, or <see cref="null"/> if empty.</returns>
    public static T? OneRandom<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        => enumerable.OrderBy(_ => Guid.NewGuid()).FirstOrDefault<T?>(x => x != null && predicate(x));
}