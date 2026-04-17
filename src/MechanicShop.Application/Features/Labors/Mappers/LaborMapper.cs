using MechanicShop.Application.Features.Labors.Dtos;
using MechanicShop.Domain.Employees;

namespace MechanicShop.Application.Features.Labors.Mappers
{
    public static class LaborMapper
    {
        public static LaborDto ToDto(this Employee employee)
        {
            return new LaborDto
            {
                LaborId = employee.Id,
                Name = employee.FullName
            };
        }

        public static List<LaborDto> ToDto(this IEnumerable<Employee> employees)
        {
            return [.. employees.Select(emp => emp.ToDto())];
        }
    }
}
