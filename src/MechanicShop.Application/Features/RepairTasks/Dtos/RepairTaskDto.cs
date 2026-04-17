namespace MechanicShop.Application.Features.RepairTasks.Dtos
{
    public class RepairTaskDto
    {
        public Guid RepairTaskId { get; set; }
        public string? Name { get; set; }
        public decimal LaborCost { get; set; }
        public int RepairDurationInMinutes { get; set; }
        public decimal TotalCost { get; set; }
        public List<PartDto> Parts { get; set; } = [];
    }
}
