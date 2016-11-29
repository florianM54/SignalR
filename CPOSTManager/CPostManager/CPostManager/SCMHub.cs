using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace CPostManager.CPostManager
{
    public class SCMHub : Hub
    {
        private readonly SCM _scm;

        public SCMHub() :
            this(SCM.Instance)
        {

        }

        public SCMHub(SCM scm)
        {
            _scm = scm;
        }
    }
}