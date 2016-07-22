using System.Reflection;
using Kiss.Validation.Validators;

namespace Kiss.Validation
{
    /// <summary>
    /// Implements a <see cref="RequiredValidator"/> for the property.
    /// </summary>
    public class RequiredAttribute : ValidatorAttribute
    {
        /// <summary>
        /// Gets the validator.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        public override Validator GetValidator(PropertyInfo propertyInfo)
        {
            return new RequiredValidator(this.ErrorMessage, propertyInfo);
        }
    }
}