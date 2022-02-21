using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;

namespace TypeDecorators.Lib.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> type.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Add service <typeparamref name="TForwarding"/> as implementation of <typeparamref name="TForwardTo"/>. 
	/// </summary>
	public static IServiceCollection Forward<TForwarding, TForwardTo>(this IServiceCollection services)
		where TForwarding : class, TForwardTo
		where TForwardTo : class
		=> services
			.AddTransient<TForwardTo>(provider => provider.GetRequiredService<TForwarding>());

	/// <summary>
	/// Register <typeparamref name="T"/> with multiple contract types.
	/// </summary>
	/// <param name="services">
	/// Collection of services.
	/// </param>
	/// <param name="config">
	/// Configuration action.
	/// </param>
	/// <param name="lifetime">
	/// Lifetime of services being registered.
	/// </param>
	public static IServiceCollection Multiple<T>(
		this IServiceCollection services,
		Action<MultipleContractsOptions<T>> config,
		ServiceLifetime lifetime = ServiceLifetime.Singleton)
		where T : notnull
	{
		var options = new MultipleContractsOptions<T>();
		config(options);

		if (services.All(descriptor => descriptor.ServiceType != typeof(T)))
		{
			services.Add(new ServiceDescriptor(typeof(T), typeof(T), lifetime));
		}

		options
			.ContactTypes
			.Select(type =>
			{
				static object Factory(IServiceProvider provider) => provider.GetRequiredService<T>();
				return new ServiceDescriptor(type, Factory, lifetime);
			})
			.ForEach(services.Add);

		return services;
	}

	/// <summary>
	/// Options for <see cref="ServiceCollectionExtensions.Multiple{T}"/> configuration action.
	/// </summary>
	/// <typeparam name="T">
	/// Implementation type.
	/// </typeparam>
	public readonly struct MultipleContractsOptions<T>
	{
		private readonly Collection<Type> contractTypes = new();

		/// <summary>
		/// All configured contract types.
		/// </summary>
		public IEnumerable<Type> ContactTypes => contractTypes;

		/// <summary>
		/// Add <typeparamref name="TBase"/> as one of services of type <typeparamref name="T"/>.
		/// </summary>
		public MultipleContractsOptions<T> As<TBase>()
		{
			if (!typeof(T).IsAssignableTo(typeof(TBase)))
			{
				throw new ArgumentException($"Type '{typeof(TBase)}' must be one of the base types of '{typeof(T)}'.");
			}

			contractTypes.Add(typeof(TBase));
			return this;
		}
	}
}