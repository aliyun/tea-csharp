namespace Darabonba.Exceptions
{
    public class DaraResponseException : DaraException
    {
        public long? RetryAfter { get; set; }
    }
}
