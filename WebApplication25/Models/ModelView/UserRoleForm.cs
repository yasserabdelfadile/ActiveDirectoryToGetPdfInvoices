namespace WebApplication25.Models.ModelView
{
    public class UserRoleForm
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public List<RoleUForm>? roleUs { get; set; }
    }
}
