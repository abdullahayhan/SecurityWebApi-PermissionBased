using Admin.Extensions;
using Admin.Services.Endpoints;
using Admin.Services.Interfaces;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using System.Net.Http.Json;

namespace Admin.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public RoleService(HttpClient httpClient, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClient;
            _apiEndpoints = apiEndpoints;
        }

        public async Task<IResponseWrapper<string>> CreateAsync(CreateRoleRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(_apiEndpoints.RoleEndpoints.Create, request);
            return await response.WrapResponse<string>();
        }

        public async Task<IResponseWrapper<string>> DeleteAsync(string roleId)
        {
            var response = await _httpClient.DeleteAsync($"{_apiEndpoints.RoleEndpoints.Delete}{roleId}");
            return await response.WrapResponse<string>();
        }

        public async Task<IResponseWrapper<RoleClaimResponse>> GetPermissionsAsync(string roleId)
        {
            var response = await _httpClient.GetAsync($"{_apiEndpoints.RoleEndpoints.GetPermissions}{roleId}");
            return await response.WrapResponse<RoleClaimResponse>();
        }

        public async Task<IResponseWrapper<List<RoleResponse>>> GetRolesAsync()
        {
            var response = await _httpClient.GetAsync(_apiEndpoints.RoleEndpoints.GetAll);
            return await response.WrapResponse<List<RoleResponse>>();
        }

        public async Task<IResponseWrapper<string>> UpdateAsync(UpdateRoleRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync(_apiEndpoints.RoleEndpoints.Update, request);
            return await response.WrapResponse<string>();
        }

        public async Task<IResponseWrapper<string>> UpdatePermissionsAsync(UpdateRolePermissionsRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync(_apiEndpoints.RoleEndpoints.UpdatePermissions, request);
            return await response.WrapResponse<string>();
        }
    }
}
