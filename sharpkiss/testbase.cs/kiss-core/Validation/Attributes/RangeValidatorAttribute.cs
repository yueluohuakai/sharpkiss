using System.Reflection;
using Kiss.Validation.Validators;

namespace Kiss.Validation
{
    /// <summary>
    /// Implements a <see cref="RangeValidator"/> for the property.
    /// </summary>
    public class RangeAttribute : ValidatorAttribute
    {

        #region Private Fields

        private object maxValue;
        private object minValue;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        /// <value>The maximum value.</value>
        public object MaxValue
        {
            get { return this.maxValue; }
        }

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        /// <value>The minimum value.</value>
        public object MinValue
        {
            get { return this.minValue; }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeAttribute"/> class.
        /// </summary>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        public RangeAttribute(object minValue, object maxValue)
        {
            this.maxValue = maxValue;
            this.minValue = minValue;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the validator.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        public override Validator GetValidator(PropertyInfo propertyInfo)
        {
            return new RangeValidator(this.ErrorMessage, propertyInfo, this.minValue, this.maxValue);
        }

        #endregion

    }
}