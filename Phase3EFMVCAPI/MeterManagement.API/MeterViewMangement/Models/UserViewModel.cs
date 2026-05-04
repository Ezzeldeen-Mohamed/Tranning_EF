namespace MeterViewMangement.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }
    public class ChangeRoleViewModel
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string CurrentRole { get; set; }
        public string NewRole { get; set; }
        public List<string> AllRoles { get; set; } = new List<string> { "Admin", "Agent" };
    }
    public class AssignMeterViewModel
    {
        public int MeterId { get; set; }
        public string SerialNumber { get; set; } // للعرض فقط عشان نعرف إحنا بنوزع أنهي عداد
        public string Email { get; set; } // إيميل الموظف (Agent) اللي هنربطه
    }

    public class GetAssignedMetersViewModel
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string Status { get; set; }


    }
}
