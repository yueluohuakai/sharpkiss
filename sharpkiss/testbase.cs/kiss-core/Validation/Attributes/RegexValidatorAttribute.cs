using System.Reflection;
using Kiss.Validation.Validators;

namespace Kiss.Validation
{
    /// <summary>
    /// Implements a RegexValidator for the property.
    /// </summary>
    public class RegexAttribute : ValidatorAttribute
    {

        private string regex;

        /// <summary>
        /// Gets the regular expression.
        /// </summary>
        /// <value>The regular expression.</value>
        public string Regex
        {
            get { return this.regex; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexAttribute"/> class.
        /// </summary>
        /// <param name="regex">The regular expression.</param>
        public RegexAttribute(string regex)
        {
            this.regex = regex;
        }

        /// <summary>
        /// Gets the validator.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        public override Validator GetValidator(PropertyInfo propertyInfo)
        {
            return new RegexValidator(this.ErrorMessage, propertyInfo, this.regex);
        }

    }
}