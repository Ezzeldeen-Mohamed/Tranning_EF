namespace MeterManagement.Application.DTOs.MeterDtos
{
    /// <summary>
    /// Represents sorting, structural filtration and pagination parameters utilized during extensive search queries.
    /// </summary>
    public class MeterQueryParameters
    {
        /// <summary>
        /// Gets or sets the designated page number segment to pull. Defaults to 1.
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Gets or sets the maximum size slice of data records returned inside an item block. Defaults to 10.
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Gets or sets the partial or full serial number sequence filter string.
        /// </summary>
        public string? SerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the status string used to restrict the dataset. Supported values include: "Assigned", "Installed", "OnStock".
        /// </summary>
        public string? Status { get; set; }
    }

    /// <summary>
    /// Structural generic container to convey schema statistics and structural records back for consumer UI page rendering elements.
    /// </summary>
    /// <typeparam name="T">The underlying collection element type data structure container.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Gets or sets the list dataset elements matching current target parameters slice.
        /// </summary>
        public List<T> Items { get; set; }

        /// <summary>
        /// Gets or sets total items available count under global unfiltered dataset context scope.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the current relative index placement sequence counter page context block.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets calculated total pages block values length metric.
        /// </summary>
        public int TotalPages { get; set; }
    }
}