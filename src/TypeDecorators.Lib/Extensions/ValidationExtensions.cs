using System.ComponentModel.DataAnnotations;

namespace TypeDecorators.Lib.Extensions;

/// <summary>
/// Extension methods for <see cref="ValidationResult"/> type.
/// </summary>
public static class ValidationExtensions
{
	/// <summary>
	/// Throw <see cref="ValidationException"/>
	/// if <paramref name="validationResult"/> != <see cref="ValidationResult.Success"/>.
	/// </summary>
	/// <param name="validationResult">
	/// Validation result.
	/// </param>
	public static void ThrowIfInvalid(this ValidationResult? validationResult)
	{
		if (validationResult == ValidationResult.Success)
		{
			return;
		}

		throw new ValidationException(validationResult!.ErrorMessage);
	}

	/// <summary>
	/// Check if <paramref name="validationResult"/> representing successful validation.
	/// </summary>
	public static bool IsValid(this ValidationResult? validationResult)
		=> validationResult == ValidationResult.Success;

	/// <summary>
	/// Run all passed validation functions and return <see cref="ValidationResult.Success"/>
	/// if all validators returned <c>true</c>.
	/// </summary>
	/// <param name="validators">
	/// Validation functions paired with error messages.
	/// </param>
	/// <returns>
	/// Validation result.
	/// </returns>
	public static ValidationResult? RunAll(this IEnumerable<(Func<bool> Validator, string ErrorMessage)> validators)
	{
		var combinedErrorMessage = validators
			.Where(tuple => !tuple.Validator())
			.Select(tuple => tuple.ErrorMessage)
			.JoinBy(Environment.NewLine);

		return string.IsNullOrWhiteSpace(combinedErrorMessage)
			? ValidationResult.Success
			: new ValidationResult(combinedErrorMessage);
	}
}