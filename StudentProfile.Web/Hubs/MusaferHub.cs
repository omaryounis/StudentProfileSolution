using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace StudentProfile.Web.Hubs
{
    public class MusaferHub : Hub
    {
        public static void UpdateFinancialTravelOrders()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MusaferHub>();
            context.Clients.All.refreshFinancialGrid();
        }
    }
}