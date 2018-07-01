namespace OzetteLibrary.ServiceCore
{
    /// <summary>
    /// An option name and value pair.
    /// </summary>
    public class ServiceOption
    {
        /// <summary>
        /// The setting ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The setting name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The setting value.
        /// </summary>
        public string Value { get; set; }
    }
}
