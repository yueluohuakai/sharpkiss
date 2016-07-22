using Kiss.Validation.Validators;
using System.Reflection;

namespace Kiss.Validation
{
    /// <summary>
    /// Implements a <see cref="NotEmptyValidator"/> for the property.
    /// </summary>
    public class NotNullAttribute : ValidatorAttribute
    {
        public object DefaultValue { get; private set; }

        public NotNullAttribute()
        {
        }

        public NotNullAttribute(object defaultValue)
        {
            this.DefaultValue = defaultValue;
        }

        /// <summary>
        /// Gets the validator.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        public override Validator GetValidator(PropertyInfo propertyInfo)
        {
            return new NotNullValidator(this.ErrorMessage, propertyInfo);
        }

    }
}