using System;

namespace OzetteProtocol.Models
{
    /// <summary>
    /// Describes the contents of a single transfer payload server response. 
    /// </summary>
    /// <remarks>
    /// This payload is used for transferring the file contents from a client to a target.
    /// </remarks>
    [Serializable]
    public class TransferPayloadResponse : BaseResponse
    {
    }
}
