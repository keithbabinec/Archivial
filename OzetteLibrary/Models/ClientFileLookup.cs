namespace OzetteLibrary.Models
{
    /// <summary>
    /// Contains the result of a Client file lookup.
    /// </summary>
    public class ClientFileLookup
    {
        /// <summary>
        /// The resulting client file, if found, otherwise null.
        /// </summary>
        public ClientFile File { get; set; }

        /// <summary>
        /// The result of the lookup operation.
        /// </summary>
        public ClientFileLookupResult Result { get; set; }
    }
}
