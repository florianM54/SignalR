using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace CPostManager.CPostManager
{
    public class CPostHub : Hub
    {
        private readonly CPost _cpost;

        public CPostHub() :
            this(CPost.Instance)
        {

        }

        public CPostHub(CPost cpost)
        {
            _cpost = cpost;
        }

        public override Task OnConnected() 
        {
            var msg = new
            {
                ConnId = Context.ConnectionId,
                message = "<< Id : " +Context.ConnectionId + " >> Connection Request Granted"
            };
            _cpost.Start(msg);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var msg = new
            {
                ConnId = Context.ConnectionId,
                message = "<< Id : " + Context.ConnectionId + " >> User Disconnected"
            };
            _cpost.End(msg);
            return base.OnDisconnected(stopCalled);
        }

        // Request to run stored procedure on a database with a given connection and save the result to local drive or ftp
        public void RequestTask(string TaskCode, string DataConnStr, string LogConnStr, string Param1, string Param2, string Param3,
            string Param4, string ConType, string LocalDir, string FtpAddr, string FtpUserId, string FtpPassw)
        {
            var connId = Context.ConnectionId;
            _cpost.RequestTask(connId, TaskCode, DataConnStr, LogConnStr, Param1, Param2, Param3, Param4, ConType, LocalDir, FtpAddr,
                FtpUserId, FtpPassw);
        }
    }
}