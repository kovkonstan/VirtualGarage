using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VirtualGarage
{
	public class CheckValueNumericOnSideClient : ModelValidatorProvider
	{
		private static Dictionary<Type, string> _messageForTypes;

		private const string DefaultMessage = "Введите корректное значение в поле \"{0}\"";

		private static readonly HashSet<Type> NumericTypes = new HashSet<Type>(new[] {
            typeof(byte), typeof(sbyte), typeof(short), typeof(ushort),
            typeof(int), typeof(uint), typeof(long), typeof(ulong),
            typeof(float), typeof(double),typeof(decimal)
        });

		public CheckValueNumericOnSideClient()
		{
			_messageForTypes = new Dictionary<Type, string>();

			foreach (var prov in ModelValidatorProviders.Providers)
			{
				if (prov.GetType() == typeof(ClientDataTypeModelValidatorProvider))
				{
					ModelValidatorProviders.Providers.Remove(prov);
					break;
				}
			}

			ModelValidatorProviders.Providers.Add(this);
		}

		public CheckValueNumericOnSideClient SetMessageForType(Type type, string message)
		{
			if (IsNumericType(type)) _messageForTypes.Add(type, message);
			else throw new ArgumentException("This type is not supported or is not numeric.");
			return this;
		}

		public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
		{
			if (metadata == null)
			{
				throw new ArgumentNullException("metadata");
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}

			return GetValidatorsImpl(metadata, context);
		}

		private static IEnumerable<ModelValidator> GetValidatorsImpl(ModelMetadata metadata, ControllerContext context)
		{
			var type = metadata.ModelType;
			if (type == typeof(Decimal?))
			{
				yield return null;		 
			}		

			if (IsNumericType(type))
			{
				yield return new NumericModelValidator(metadata, context);
			}
		}

		private static bool IsNumericType(Type type)
		{
			Type underlyingType = Nullable.GetUnderlyingType(type);
			return NumericTypes.Contains(underlyingType ?? type);
		}

		internal sealed class NumericModelValidator : ModelValidator
		{
			public NumericModelValidator(ModelMetadata metadata, ControllerContext controllerContext)
				: base(metadata, controllerContext)
			{
			}

			public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
			{
				var rule = new ModelClientValidationRule
				{
					ValidationType = "number",
					ErrorMessage = MakeErrorString(Metadata)
				};

				return new[] { rule };
			}

			private static string MakeErrorString(ModelMetadata metadata)
			{
				var message = _messageForTypes.FirstOrDefault(x => x.Key.Name == metadata.ModelType.Name).Value ?? DefaultMessage;
				return String.Format(message, metadata.GetDisplayName());
			}

			public override IEnumerable<ModelValidationResult> Validate(object container)
			{
				return Enumerable.Empty<ModelValidationResult>();
			}
		}
	}
}