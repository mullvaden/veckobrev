namespace DownloadWeekly.Dtos
{
    internal class FileToSend
    {
        public int Id { get; set; }
        public int DocumentToDownloadId { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public int WeekNumber { get; set; }
        public string Name { get; set; }
    }
}