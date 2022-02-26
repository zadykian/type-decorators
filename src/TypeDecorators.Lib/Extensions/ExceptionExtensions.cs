using System.Diagnostics.CodeAnalysis;

namespace TypeDecorators.Lib.Extensions;

/// <summary>
/// Extension methods for <see cref="Exception"/> type.
/// </summary>
public static class ExceptionExtensions
{
	/// <summary>
	/// Try to get <paramref name="innerException"/> of type <typeparamref name="TException"/>
	/// from all inner exceptions of <paramref name="exception"/>.
	/// </summary>
	public static bool TryUnwrap<TException>(
		this Exception exception,
		[NotNullWhen(returnValue: true)] out TException? innerException)
		where TException : Exception
	{
		var current = exception;

		while (current is not null)
		{
			if (current is TException inner)
			{
				innerException = inner;
				return true;
			}

			current = current.InnerException;
		}

		innerException = default;
		return false;
	}
}