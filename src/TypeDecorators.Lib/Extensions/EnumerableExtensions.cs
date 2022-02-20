using TypeDecorators.Lib.Types;

namespace TypeDecorators.Lib.Extensions;

/// <summary>
/// Extension methods for <see cref="IEnumerable{T}"/> type.
/// </summary>
public static class EnumerableExtensions
{
	/// <summary>
	/// Perform action <paramref name="action"/> for each item in <paramref name="enumerable"/>. 
	/// </summary>
	public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
	{
		foreach (var item in enumerable)
		{
			action(item);
		}
	}

	/// <summary>
	/// Join string values <paramref name="stringValues"/> with separator <paramref name="separator"/>. 
	/// </summary>
	public static string JoinBy(this IEnumerable<string> stringValues, string separator)
		=> string.Join(separator, stringValues);

	/// <summary>
	/// Join string values <paramref name="stringValues"/> with separator <paramref name="separator"/>. 
	/// </summary>
	public static NonEmptyString JoinBy(this IEnumerable<NonEmptyString> stringValues, NonEmptyString separator)
		=> string.Join(separator, stringValues);

	/// <summary>
	/// Append item <paramref name="itemToAppend"/> to enumerable <paramref name="enumerable"/>. 
	/// </summary>
	public static IEnumerable<T> Add<T>(this IEnumerable<T> enumerable, T itemToAppend)
	{
		foreach (var item in enumerable)
		{
			yield return item;
		}

		yield return itemToAppend;
	}

	/// <summary>
	/// Transform <paramref name="enumerable"/> into sequence
	/// of adjacent element pairs.
	/// </summary>
	/// <remarks>
	/// Examples:
	/// <para> [a, b, c, d] -> [(a, b), (b, c), (c, d)] </para>
	/// <para> [a] -> [ ] </para>
	/// <para> [ ] -> [ ] </para>
	/// </remarks>
	public static IEnumerable<(T Left, T Right)> Pairwise<T>(this IEnumerable<T> enumerable)
	{
		using var enumerator = enumerable.GetEnumerator();
		var previous = default(T);
		if (enumerator.MoveNext()) previous = enumerator.Current;
		while (enumerator.MoveNext()) yield return (Left: previous!, Right: previous = enumerator.Current);
	}

	/// <summary>
	/// Check if <paramref name="enumerable"/> is ordered. 
	/// </summary>
	public static bool IsOrdered<T>(this IEnumerable<T> enumerable)
		where T : IComparable<T>
		=> enumerable
			.Pairwise()
			.All(pair => pair.Left.CompareTo(pair.Right) <= 0);
}