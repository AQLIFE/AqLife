namespace AqLife.Shared.Options
{
    public struct APIStatus
    {
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }

    }


    public record ServiceStatus(string Message, bool IsValid = true);
}
