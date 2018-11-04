using System;

namespace OzetteLibrary.ServiceCore
{
    /// <summary>
    /// An option name and value pair.
    /// </summary>
    public class ApplicationOption
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ApplicationOption()
        {
        }

        /// <summary>
        /// A constructor that accepts and option name and value.
        /// </summary>
        /// <param name="OptionName"></param>
        /// <param name="OptionValue"></param>
        public ApplicationOption(string OptionName, string OptionValue)
        {
            Name = OptionName;
            Value = OptionValue;
        }

        /// <summary>
        /// An ID used for database tracking.
        /// </summary>
        public Guid ID { get; set; }

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
