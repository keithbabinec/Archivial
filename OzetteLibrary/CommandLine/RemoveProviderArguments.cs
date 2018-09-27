namespace OzetteLibrary.CommandLine
{
    /// <summary>
    /// A set of Ozette provider removal arguments.
    /// </summary>
    public class RemoveProviderArguments : Arguments
    {
        /// <summary>
        /// The ID of the provider to remove.
        /// </summary>
        public int ProviderID { get; set; }
    }
}
