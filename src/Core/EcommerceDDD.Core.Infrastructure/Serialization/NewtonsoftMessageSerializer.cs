namespace EcommerceDDD.Core.Infrastructure.Serialization;

public sealed class NewtonsoftMessageSerializer(JsonSerializerSettings settings) : IMessageSerializer
{
	private readonly JsonSerializerSettings _settings = settings;

	public NewtonsoftMessageSerializer()
		: this(new JsonSerializerSettings { ContractResolver = new ImmutableFriendlyResolver() }) { }

	public string ContentType => "application/json";

	public byte[] Write(Envelope envelope) =>
		Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(envelope.Message, _settings));

	public object ReadFromData(Type messageType, Envelope envelope) =>
		JsonConvert.DeserializeObject(Encoding.UTF8.GetString(envelope.Data!), messageType, _settings)!;

	// Wolverine's own STJ serializer throws here too: a known type is required.
	public object ReadFromData(byte[] data) =>
		throw new NotSupportedException("A known message type is required to deserialize.");

	public byte[] WriteMessage(object message) =>
		Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, _settings));

	// Wolverine serializes a message only when it must be persisted (durable inbox / dead letter).
	// Domain value objects are immutable (get-only auto-properties, non-public ctors) and command
	// records expose no parameterless ctor, so neither STJ nor stock Newtonsoft can rehydrate them.
	// This resolver fills get-only properties through their backing field and constructs via a
	// non-public parameterless ctor when one exists, or the widest ctor otherwise.
	private sealed class ImmutableFriendlyResolver : DefaultContractResolver
	{
		protected override JsonObjectContract CreateObjectContract(Type objectType)
		{
			var contract = base.CreateObjectContract(objectType);
			if (contract.OverrideCreator is not null || contract.DefaultCreator is not null
				|| contract.CreatorParameters.Count > 0)
				return contract;

			var parameterless = objectType.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
			if (parameterless is not null)
			{
				contract.DefaultCreator = () => parameterless.Invoke(null);
				return contract;
			}

			var ctor = objectType
				.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.OrderByDescending(c => c.GetParameters().Length)
				.FirstOrDefault();
			if (ctor is not null && ctor.GetParameters().Length > 0)
			{
				contract.OverrideCreator = args => ctor.Invoke(args);
				foreach (var parameter in CreateConstructorParameters(ctor, contract.Properties))
					contract.CreatorParameters.Add(parameter);
			}

			return contract;
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var property = base.CreateProperty(member, memberSerialization);
			if (!property.Writable && member is PropertyInfo propertyInfo)
			{
				var backingField = propertyInfo.DeclaringType!.GetField(
					$"<{propertyInfo.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
				if (backingField is not null)
				{
					property.Writable = true;
					property.ValueProvider = new BackingFieldValueProvider(backingField);
				}
			}
			return property;
		}

		private sealed class BackingFieldValueProvider(FieldInfo field) : IValueProvider
		{
			public object? GetValue(object target) => field.GetValue(target);
			public void SetValue(object target, object? value) => field.SetValue(target, value);
		}
	}
}
