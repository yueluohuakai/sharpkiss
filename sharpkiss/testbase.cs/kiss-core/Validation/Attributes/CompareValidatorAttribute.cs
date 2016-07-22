using System.Reflection;
using Kiss.Validation.Validators;

namespace Kiss.Validation
{
    /// <summary>
    /// Implements a <see cref="CompareValidator"/> for the property.
    /// </summary>
    public class CompareAttribute : ValidatorAttribute
    {

        #region Private Fields

        private ComparisonOperator comparisonOperator;
        private object valueToCompare;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the comparison operator.
        /// </summary>
        /// <value>The comparison operator.</value>
        public ComparisonOperator ComparisonOperator
        {
            get { return this.comparisonOperator; }
        }

        /// <summary>
        /// Gets the value to compare.
        /// </summary>
        /// <value>The value to compare.</value>
        public object ValueToCompare
        {
            get { return this.valueToCompare; }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareAttribute"/> class.
        /// </summary>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="valueToCompare">The value to compare.</param>
        public CompareAttribute(ComparisonOperator comparisonOperator, object valueToCompare)
        {
            this.comparisonOperator = comparisonOperator;
            this.valueToCompare = valueToCompare;
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
            return new CompareValidator(this.ErrorMessage, propertyInfo, this.comparisonOperator, this.valueToCompare);
        }

        #endregion

    }
}