namespace OzetteLibrary.CommandLine.Arguments
{
    /// <summary>
    /// A set of Ozette provider removal arguments.
    /// </summary>
    public class RemoveProviderArguments : ArgumentBase
    {
        /// <summary>
        /// The ID of the provider to remove.
        /// </summary>
        public int ProviderID { get; set; }
    }
}
