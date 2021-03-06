namespace TypeDecorators.Lib.Types;

/// <summary>
/// Range represented as left and right bounds. 
/// </summary>
public readonly struct Range<T> where T : IComparable<T>
{
	/// <param name="leftBound">
	/// Left bound of range.
	/// </param>
	/// <param name="rightBound">
	/// Right bound of range.
	/// </param>
	/// <exception cref="ArgumentException">
	/// <paramref name="leftBound"/> larger then <paramref name="rightBound"/>.
	/// </exception>
	public Range(T leftBound, T rightBound)
	{
		if (leftBound.CompareTo(rightBound) > 0)
		{
			throw new ArgumentException(
				$"{nameof(leftBound)} ({leftBound}) must be " +
				$"less or equal to {nameof(rightBound)} ({rightBound}).");
		}

		LeftBound = leftBound;
		RightBound = rightBound;
	}

	/// <summary>
	/// Left bound of the range.
	/// </summary>
	public T LeftBound { get; }

	/// <summary>
	/// Right bound of the range.
	/// </summary>
	public T RightBound { get; }

	/// <inheritdoc />
	public override string ToString() => $"[{LeftBound}, {RightBound}]";

	/// <summary>
	/// Implicit cast operator '<see cref="ValueTuple{T,T}"/> -> <see cref="Range{T}"/>'.
	/// </summary>
	public static implicit operator Range<T>(ValueTuple<T, T> valueTuple) => new(valueTuple.Item1, valueTuple.Item2);
}