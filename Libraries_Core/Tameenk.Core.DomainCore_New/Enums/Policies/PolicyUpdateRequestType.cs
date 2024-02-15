namespace Tameenk.Core.Domain.Enums.Policies
{
    /// <summary>
    /// The policy update request type
    /// </summary>
    public enum PolicyUpdateRequestType
    {
        /// <summary>
        /// Fix policy error
        /// </summary>
        FixPolicyError = 1,
        /// <summary>
        /// Change license.
        /// </summary>
        ChangeLicense = 2,
        /// <summary>
        /// Create license.
        /// </summary>
        CreateLicense = 3,
        /// <summary>
        /// Add driver.
        /// </summary>
        AddDriver = 4
    }
}
