using System.Reflection;
using Kiss.Validation.Validators;

namespace Kiss.Validation
{
    /// <summary>
    /// Implements a <see cref="CustomValidator"/> for the property.
    /// </summary>
    public class CustomAttribute : ValidatorAttribute
    {

        private string methodName;

        /// <summary>
        /// Gets or sets the name of the method.
        /// </summary>
        /// <value>The name of the method.</value>
        public string MethodName
        {
            get { return this.methodName; }
            set { this.methodName = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAttribute"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="methodName">Name of the method.</param>
        public CustomAttribute(string errorMessage, string methodName)
        {
            this.ErrorMessage = errorMessage;
            this.methodName = methodName;
        }

        /// <summary>
        /// Gets the validator.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        public override Validator GetValidator(PropertyInfo propertyInfo)
        {
            return new CustomValidator(this.ErrorMessage, propertyInfo, this.methodName);
        }

    }
}