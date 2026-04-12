using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Customers.Vehicles;

public static class VehicleErrors
{
    public static Error MakeRequired => Error.Validation(code:"Vehicle_Make_Required", description: "Vehicle make is required");
    public static Error ModelRequired => Error.Validation(code:"Vehicle_Model_Required", description: "Vehicle model is required");
    public static Error LicensePlateRequired => Error.Validation(code:"Vehicle_LicensePlate_Required", description: "Vehicle license plate is required");
    public static Error YearInvalid => Error.Validation(code:"Vehicle_Year_Invalid", description: "Year must be between 1886 and the current year");
}
