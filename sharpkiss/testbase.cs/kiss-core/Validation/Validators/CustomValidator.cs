using System;
using System.Reflection;

namespace Kiss.Validation.Validators
{
    /// <summary>
    /// Performs custom validation based on a custom method.
    /// </summary>
    public class CustomValidator : Validator
    {
        private Type methodType;

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        /// <value>The name of the method.</value>
        public string MethodName { get; private set; }

        #region Protected Properties

        /// <summary>
        /// Gets the valid property types.
        /// </summary>
        /// <value>The valid property types.</value>
        protected override Type[] ValidPropertyTypes
        {
            get { return null; }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValidator"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="methodName">
        /// Name of the method.  This method must conform to the Predicate[of T]
        /// signature where T is the property type this attribute has been placed on.
        /// </param>
        public CustomValidator(string errorMessage, PropertyInfo propertyInfo, string methodName)
            : base(errorMessage, propertyInfo)
        {
            this.MethodName = methodName;
            Type predType = typeof(Predicate<>);
            this.methodType = predType.MakeGenericType(this.PropertyInfo.PropertyType);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Does the validation.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected override bool DoIsValid(object instance, object value)
        {
            Delegate d = Delegate.CreateDelegate(this.methodType, instance, this.MethodName);
            return (bool)d.DynamicInvoke(value);
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValidator"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="methodName">
        /// Name of the method.  This method must conform to the Predicate[of T]
        /// signature where T is the property type this attribute has been placed on.
        /// </param>
        /// <returns></returns>
        public static CustomValidator CreateValidator<T>(string errorMessage, string propertyName, string methodName)
        {
            return CreateValidator(typeof(T), errorMessage, propertyName, methodName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValidator"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="methodName">
        /// Name of the method.  This method must conform to the Predicate[of T]
        /// signature where T is the property type this attribute has been placed on.
        /// </param>
        /// <returns></returns>
        public static CustomValidator CreateValidator(Type type, string errorMessage, string propertyName, string methodName)
        {
            return new CustomValidator(errorMessage, Validator.GetPropertyInfo(type, propertyName), methodName);
        }

        #endregion
    }
}
