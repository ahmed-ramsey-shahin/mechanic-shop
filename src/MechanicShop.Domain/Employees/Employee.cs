using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Identities;

namespace MechanicShop.Domain.Employees;

public class Employee : AuditableEntity
{
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public Role Role { get; private set; }
    public string FullName => $"{FirstName} {LastName}";

    private Employee()
    {
    }

    private Employee(Guid id, string firstName, string lastName, Role role) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Role = role;
    }

    private static Result<bool> Validate(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return EmployeeErrors.FirstNameRequired;
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return EmployeeErrors.LastNameRequired;
        }

        return true;
    }

    public static Result<Employee> Create(Guid id, string firstName, string lastName, Role role)
    {
        var validationResult = Validate(firstName, lastName);
        if (validationResult.IsError)
        {
            return validationResult.Errors;
        }
        return new Employee(id, firstName, lastName, role);
    }
}
