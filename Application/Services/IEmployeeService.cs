using Domain;

namespace Application.Services;

public interface IEmployeeService
{
    Task<Employee> CreateEmployeeAsync(Employee employee);
    Task<Employee> UpdateEmployeeAsync(Employee employee);
    Task<int> DeleteEmployeeAsync(Employee employee);
    Task<Employee> GetEmployeeByIdAsync(int id);
    Task<List<Employee>> GetEmployeeListAsync();
}