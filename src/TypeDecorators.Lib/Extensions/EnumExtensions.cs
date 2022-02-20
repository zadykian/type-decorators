using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using TypeDecorators.Lib.Types;

namespace TypeDecorators.Lib.Extensions;

/// <summary>
/// Extension methods for enumerations.
/// </summary>
public static class EnumExtensions
{
	/// <summary>
	/// Get description of <paramref name="enumValue"/>
	/// based on value configured by <see cref="DescriptionAttribute"/>. 
	/// </summary>
	public static NonEmptyString GetDescription<
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]TEnum>(this TEnum enumValue)
		where TEnum : struct, Enum
	{
		if (enumValue.Equals(default(TEnum)))
		{
			throw new ArgumentException($"{nameof(enumValue)} equals to default.");
		}

		var description = typeof(TEnum)
			.GetField(enumValue.ToString())!
			.GetCustomAttribute<DescriptionAttribute>()?
			.Description;

		return string.IsNullOrWhiteSpace(description)
			? throw new InvalidOperationException(
				$"Enum item {enumValue} is not configured by {nameof(DescriptionAttribute)}")
			: description;
	}
}