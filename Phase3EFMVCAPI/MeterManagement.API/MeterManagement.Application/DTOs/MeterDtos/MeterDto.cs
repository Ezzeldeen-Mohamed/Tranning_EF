namespace MeterManagement.Application.DTOs.MeterDtos
{
    public class MeterDto
    {
        public string SerialNumber { get; set; }
    }
    public class BulkMeterDto
    {
        public List<MeterDto> Meters { get; set; }
    }
    public class InstallMeterDto
    {
        public int MeterId { get; set; }
    }
    public class GetMeterDto
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string Status { get; set; }
    }
    public class ImportResultDto
    {
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }
    public class AssignMeterDto
    {
        public int MeterId { get; set; }
        public string Email { get; set; }
    }
}
