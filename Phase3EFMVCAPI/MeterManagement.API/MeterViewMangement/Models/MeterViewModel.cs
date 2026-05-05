using System.ComponentModel.DataAnnotations;

namespace MeterViewMangement.Models
{
    public class MeterViewModel
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string Status { get; set; }
    }
    public class MeterCreateViewModel
    {
        [Required(ErrorMessage = "Serial Number is required")]
        public string SerialNumber { get; set; }
    }

    public class PagedMetersViewModel
    {
        public List<DeleteMeterViewModel> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }

        public string? SerialFilter { get; set; }
        public string? StatusFilter { get; set; }
    }
    public class ImportResultDto
    {
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    public class DeleteMeterViewModel
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string Status { get; set; }
        public bool IsDeleted { get; set; }
    }


}
