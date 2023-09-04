using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace AppointmentsApi
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class AppointmentApiException : Exception
    {
        public AppointmentApiException() { }
        public AppointmentApiException(string message) : base(message) { }
        public AppointmentApiException(string message, Exception innerException) : base(message, innerException) { }
        protected AppointmentApiException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
