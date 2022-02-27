using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using TypeDecorators.Lib.Extensions;

namespace TypeDecorators.Lib.Types;

/// <summary>
/// Memory volume.
/// </summary>
public readonly struct Memory : IEquatable<Memory>, IComparable<Memory>, IComparable
{
	/// <param name="totalBytes">
	/// Memory value in bytes.
	/// </param>
	public Memory(ulong totalBytes) => TotalBytes = totalBytes;

	/// <summary>
	/// Memory value in bytes.
	/// </summary>
	public ulong TotalBytes { get; }

	/// <inheritdoc />
	/// <remarks>
	/// Value less than 10kB represented in bytes, value less then 10MB - in kilobytes, and so on.
	/// </remarks>
	public override string ToString()
	{
		var (value, unitString) = TotalBytes switch
		{
			< 10 * (1UL << 10) => (TotalBytes,                       "B" ),
			< 10 * (1UL << 20) => (TotalBytes / Kilobyte.TotalBytes, "KB"),
			< 10 * (1UL << 30) => (TotalBytes / Megabyte.TotalBytes, "MB"),
			< 10 * (1UL << 40) => (TotalBytes / Gigabyte.TotalBytes, "GB"),
			_                  => (TotalBytes / Terabyte.TotalBytes, "TB")
		};

		return $"{value}{unitString}";
	}

	#region EqualityMembers

	/// <inheritdoc />
	public bool Equals(Memory other) => TotalBytes == other.TotalBytes;

	/// <inheritdoc />
	public override bool Equals(object? obj) => obj is Memory other && Equals(other);

	/// <inheritdoc />
	public override int GetHashCode() => TotalBytes.GetHashCode();

	/// <inheritdoc />
	public int CompareTo(Memory other) => TotalBytes.CompareTo(other.TotalBytes);

	/// <inheritdoc />
	int IComparable.CompareTo(object? obj)
	{
		if (ReferenceEquals(null, obj)) return 1;
		return obj is Memory other
			? CompareTo(other)
			: throw new ArgumentException($"Object must be of type {nameof(Memory)}");
	}

	#endregion

	#region Constants

	/// <summary>
	/// Zero memory size - 0B.
	/// </summary>
	public static readonly Memory Zero = new(0);

	/// <summary>
	/// One byte - 1B.
	/// </summary>
	public static readonly Memory Byte = new(1);

	/// <summary>
	/// One kilobyte - 1024B.
	/// </summary>
	public static readonly Memory Kilobyte = 1024 * Byte;

	/// <summary>
	/// One megabyte - 1024KB.
	/// </summary>
	public static readonly Memory Megabyte = 1024 * Kilobyte;

	/// <summary>
	/// One gigabyte - 1024MB.
	/// </summary>
	public static readonly Memory Gigabyte = 1024 * Megabyte;

	/// <summary>
	/// One terabyte - 1024GB.
	/// </summary>
	public static readonly Memory Terabyte = 1024 * Gigabyte;

	#endregion

	/// <summary>
	/// Parse string <paramref name="stringToParse"/> to memory value.
	/// </summary>
	/// <returns>
	/// <c>true</c> if <paramref name="stringToParse"/> has valid format, otherwise - <c>false</c>.
	/// </returns>
	public static bool TryParse(NonEmptyString stringToParse, [NotNullWhen(returnValue: true)] out Memory? memory)
	{
		if (!Regex.IsMatch(stringToParse, @"^[0-9]+\s*(B|KB|MB|GB|TB)$"))
		{
			memory = default;
			return false;
		}

		var (numericValue, unit) = stringToParse.ParseToTokens();

#pragma warning disable CS8509
		var multiplier = unit switch
#pragma warning restore CS8509
		{
			"B"  => Byte,
			"KB" => Kilobyte,
			"MB" => Megabyte,
			"GB" => Gigabyte,
			"TB" => Terabyte
		};

		memory = numericValue * multiplier;
		return true;
	}
	
	/// <summary>
	/// Parse string <paramref name="stringToParse"/> to memory value.
	/// </summary>
	/// <exception cref="ArgumentException">
	/// Occurs when <paramref name="stringToParse"/> has invalid format.
	/// </exception>
	public static Memory Parse(NonEmptyString stringToParse)
		=> TryParse(stringToParse, out var memory)
			? memory.Value
			: throw new ArgumentException($"Input string '{stringToParse}' has invalid format.", nameof(stringToParse));

	#region Operators

	/// <summary>
	/// Equality operator. 
	/// </summary>
	public static bool operator ==(Memory left, Memory right) => left.Equals(right);

	/// <summary>
	/// Inequality operator. 
	/// </summary>
	public static bool operator !=(Memory left, Memory right) => !(left == right);

	/// <summary>
	/// Comparison operator.
	/// </summary>
	public static bool operator <(Memory left, Memory right) => left.CompareTo(right) < 0;

	/// <inheritdoc cref="op_LessThan"/>
	public static bool operator >(Memory left, Memory right) => left.CompareTo(right) > 0;

	/// <inheritdoc cref="op_LessThan"/>
	public static bool operator <=(Memory left, Memory right) => left.CompareTo(right) <= 0;

	/// <inheritdoc cref="op_LessThan"/>
	public static bool operator >=(Memory left, Memory right) => left.CompareTo(right) >= 0;

	/// <summary>
	/// Multiplication operator.
	/// </summary>
	public static Memory operator *(Memory memory, ulong coefficient) => new(memory.TotalBytes * coefficient);

	/// <inheritdoc cref="op_Multiply(Memory,ulong)"/>
	public static Memory operator *(ulong coefficient, Memory memory) => memory * coefficient;

	/// <inheritdoc cref="op_Multiply(Memory,ulong)"/>
	public static Memory operator *(Memory memory, int coefficient) => new(memory.TotalBytes * (ulong) coefficient);

	/// <inheritdoc cref="op_Multiply(Memory,ulong)"/>
	public static Memory operator *(int coefficient, Memory memory) => memory * coefficient;

	/// <inheritdoc cref="op_Multiply(Memory,ulong)"/>
	public static Memory operator *(Memory memory, double coefficient) => new((ulong) (memory.TotalBytes * coefficient));

	/// <inheritdoc cref="op_Multiply(Memory,ulong)"/>
	public static Memory operator *(double coefficient, Memory memory) => memory * coefficient;

	/// <inheritdoc cref="op_Multiply(Memory,ulong)"/>
	public static Memory operator *(Memory memory, decimal coefficient) => new((ulong) (memory.TotalBytes * coefficient));

	/// <inheritdoc cref="op_Multiply(Memory,ulong)"/>
	public static Memory operator *(decimal coefficient, Memory memory) => memory * coefficient;

	/// <summary>
	/// Division operator.
	/// </summary>
	public static Memory operator /(Memory memory, ulong coefficient) => new(memory.TotalBytes / coefficient);

	/// <inheritdoc cref="op_Division(Memory,ulong)"/>
	public static Memory operator /(Memory memory, double coefficient) => new((ulong) (memory.TotalBytes / coefficient));

	/// <inheritdoc cref="op_Division(Memory,ulong)"/>
	public static Memory operator /(Memory memory, decimal coefficient) => new((ulong) (memory.TotalBytes / coefficient));

	/// <inheritdoc cref="op_Division(Memory,ulong)"/>
	public static double operator /(Memory left, Memory right) => (double) left.TotalBytes / right.TotalBytes;

	#endregion

	/// <summary>
	/// Memory unit.
	/// </summary>
	public enum Unit : byte
	{
		/// <summary>
		/// Bytes.
		/// </summary>
		Bytes,

		/// <summary>
		/// Kilobytes.
		/// </summary>
		Kilobytes,

		/// <summary>
		/// Megabytes.
		/// </summary>
		Megabytes,

		/// <summary>
		/// Gigabytes.
		/// </summary>
		Gigabytes,

		/// <summary>
		/// Terabytes.
		/// </summary>
		Terabytes
	}
}