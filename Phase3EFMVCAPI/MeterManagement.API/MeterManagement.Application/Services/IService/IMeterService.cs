using MeterManagement.Application.Common;
using MeterManagement.Application.DTOs.MeterDtos;
using MeterManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace MeterManagement.Application.Services.IService
{
    /// <summary>
    /// Defines operations for managing meters, including creation, retrieval, updates, bulk imports, and status tracking.
    /// </summary>
    public interface IMeterService
    {
        /// <summary>
        /// Retrieves a paginated and filtered list of meters based on query parameters.
        /// </summary>
        /// <param name="query">The query parameters containing pagination and filter criteria.</param>
        /// <returns>A base response wrapping a paged result of meter data transfer objects.</returns>
        Task<BaseResponse<PagedResult<GetMeterDto>>> GetAll(MeterQueryParameters query);

        /// <summary>
        /// Retrieves a specific meter by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the meter.</param>
        /// <returns>A base response containing the details of the requested meter.</returns>
        Task<BaseResponse<GetMeterDto>> GetById(int id);

        /// <summary>
        /// Creates a new meter record.
        /// </summary>
        /// <param name="dto">The data transfer object containing the details of the meter to create.</param>
        /// <returns>A base response indicating whether the creation was successful.</returns>
        Task<BaseResponse<bool>> Create(MeterDto dto);

        /// <summary>
        /// Creates multiple meter records in bulk.
        /// </summary>
        /// <param name="dtos">The list of meter data transfer objects to create.</param>
        /// <returns>A base response containing a list of strings, such as created IDs or potential individual errors.</returns>
        Task<BaseResponse<List<string>>> CreateBulk(List<MeterDto> dtos);

        /// <summary>
        /// Imports meter data from an uploaded Excel file.
        /// </summary>
        /// <param name="file">The uploaded Excel file containing meter records.</param>
        /// <returns>A base response containing the processing summary, including success and failure counts.</returns>
        Task<BaseResponse<ImportResultDto>> ImportFromExcel(IFormFile file);

        /// <summary>
        /// Updates an existing meter record.
        /// </summary>
        /// <param name="id">The unique identifier of the meter to update.</param>
        /// <param name="dto">The updated data transfer object details.</param>
        /// <returns>A base response indicating whether the update was successful.</returns>
        Task<BaseResponse<bool>> Update(int id, MeterDto dto);

        /// <summary>
        /// Assigns a meter to a user using their email address.
        /// </summary>
        /// <param name="dto">The data transfer object containing assignment details.</param>
        /// <returns>A base response indicating whether the assignment was successful.</returns>
        Task<BaseResponse<bool>> AssignMeterByEmail(AssignMeterDto dto);

        /// <summary>
        /// Retrieves all meters assigned to or associated with a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A base response containing a list of meters associated with the user.</returns>
        Task<BaseResponse<List<GetMeterDto>>> GetByUser(string userId);

        /// <summary>
        /// Retrieves all meters filtered by their operational or administrative status.
        /// </summary>
        /// <param name="status">The operational status enum to filter by.</param>
        /// <returns>A base response containing a list of meters matching the specified status.</returns>
        Task<BaseResponse<List<GetMeterDto>>> GetByStatus(MeterStatus status);

        /// <summary>
        /// Installs a specific meter for a designated user.
        /// </summary>
        /// <param name="meterId">The unique identifier of the meter to install.</param>
        /// <param name="userId">The unique identifier of the user receiving the installation.</param>
        /// <returns>A base response indicating whether the installation setup succeeded.</returns>
        Task<BaseResponse<bool>> InstallMeter(int meterId, string userId);

        /// <summary>
        /// Permanently deletes a meter record from the system.
        /// </summary>
        /// <param name="id">The unique identifier of the meter to delete.</param>
        /// <returns>A base response indicating whether the deletion was successful.</returns>
        Task<BaseResponse<bool>> Delete(int id);

        /// <summary>
        /// Soft-deletes a meter record, flagging it as inactive without permanently purging it from the database.
        /// </summary>
        /// <param name="id">The unique identifier of the meter to soft-delete.</param>
        /// <returns>A base response indicating whether the soft-deletion was successful.</returns>
        Task<BaseResponse<bool>> SoftDelete(int id);

        /// <summary>
        /// Restores a previously soft-deleted meter record.
        /// </summary>
        /// <param name="id">The unique identifier of the meter to restore.</param>
        /// <returns>A base response indicating whether the restoration was successful.</returns>
        Task<BaseResponse<bool>> Restore(int id);
    }
}