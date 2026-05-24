namespace MeterManagement.Application.DTOs.MeterDtos
{
    /// <summary>
    /// Data transfer object used for creating or updating a meter's basic details.
    /// </summary>
    public class MeterDto
    {
        /// <summary>
        /// Gets or sets the unique hardware serial number assigned to the physical meter.
        /// </summary>
        public string SerialNumber { get; set; }
    }

    /// <summary>
    /// Data transfer object used for processing multiple meter items concurrently.
    /// </summary>
    public class BulkMeterDto
    {
        /// <summary>
        /// Gets or sets the collection of meters to be processed collectively.
        /// </summary>
        public List<MeterDto> Meters { get; set; }
    }

    /// <summary>
    /// Data transfer object capturing parameters necessary to install a specific meter.
    /// </summary>
    public class InstallMeterDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the meter targeted for installation.
        /// </summary>
        public int MeterId { get; set; }
    }

    /// <summary>
    /// Data transfer object representation of a meter intended for read operations and API outputs.
    /// </summary>
    public class GetMeterDto
    {
        /// <summary>
        /// Gets or sets the database unique internal identifier for the meter.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer serial number of the meter.
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the current textual representation of the meter's status (e.g., "OnStock", "Assigned").
        /// </summary>
        public string Status { get; set; }
    }

    /// <summary>
    /// Data transfer object representing the operational outcome metrics of an excel batch import process.
    /// </summary>
    public class ImportResultDto
    {
        /// <summary>
        /// Gets or sets the total count of successfully parsed and imported meter entries.
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// Gets or sets the total count of failed entries encountered during processing.
        /// </summary>
        public int FailedCount { get; set; }

        /// <summary>
        /// Gets or sets individual error descriptions mapped out chronologically or contextually from failed lines.
        /// </summary>
        public List<string> Errors { get; set; } = new();
    }

    /// <summary>
    /// Data transfer object containing details necessary to link a specific meter to an external user account email.
    /// </summary>
    public class AssignMeterDto
    {
        /// <summary>
        /// Gets or sets the identifier of the targeted system meter.
        /// </summary>
        public int MeterId { get; set; }

        /// <summary>
        /// Gets or sets the target user's email address context for ownership association.
        /// </summary>
        public string Email { get; set; }
    }
}