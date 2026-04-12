using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Employees;

public static class EmployeeErrors
{
    public static Error FirstNameRequired => Error.Validation("Employee_FirstName_Required", "First name is required.");
    public static Error LastNameRequired => Error.Validation("Employee_LastName_Requiored", "Last name is required.");
}
