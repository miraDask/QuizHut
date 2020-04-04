namespace QuizHut.Common.Hubs
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.SignalR;

    public class QuizHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            if (this.Context.User.IsInRole(GlobalConstants.AdministratorRoleName))
            {
                this.Groups.AddToGroupAsync(this.Context.ConnectionId, GlobalConstants.AdministratorRoleName);
            }
            else
            {
                this.Groups.AddToGroupAsync(this.Context.ConnectionId, this.Context.User.Identity.Name);
            }

            return base.OnConnectedAsync();
        }
    }
}
