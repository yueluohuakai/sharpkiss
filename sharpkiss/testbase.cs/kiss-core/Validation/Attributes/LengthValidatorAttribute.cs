using System.Reflection;
using Kiss.Validation.Validators;

namespace Kiss.Validation
{
    /// <summary>
    /// Implements a <see cref="LengthValidator"/> for the property.
    /// </summary>
    public class LengthAttribute : ValidatorAttribute
    {

        #region Private Fields

        private int _maxLength;
        private int _minLength;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the maximum length.
        /// </summary>
        /// <value>The maximum length.</value>
        public int MaxLength
        {
            get { return this._maxLength; }
        }

        /// <summary>
        /// Gets the minimum length.
        /// </summary>
        /// <value>The minimum length</value>
        public int MinLength
        {
            get { return this._minLength; }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="LengthAttribute"/> class.
        /// </summary>
        /// <param name="maxLength">The maximum length.</param>
        public LengthAttribute(int maxLength)
            : this(0, maxLength)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LengthAttribute"/> class.
        /// </summary>
        /// <param name="minLength">The minimum length..</param>
        /// <param name="maxLength">The maximum length.</param>
        public LengthAttribute(int minLength, int maxLength)
        {
            this._maxLength = maxLength;
            this._minLength = minLength;
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
            return new LengthValidator(this.ErrorMessage, propertyInfo, this._minLength, this._maxLength);
        }

        #endregion

    }
}