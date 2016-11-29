using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Data;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using CPostManager.CPostDB;
using CPostManager.BusinessObjects;

namespace CPostManager.CPostManager
{
    public class CPost
    {
                // Singleton instance
        private readonly static Lazy<CPost> _instance = new Lazy<CPost>(
            () => new CPost(GlobalHost.ConnectionManager.GetHubContext<CPostHub>().Clients));

        public int _taskcnt = 0, _msgcnt = 0, _usercnt = 0, _taskdone = 0, _usersout = 0, _taskfailed = 0;
        private ConcurrentDictionary<string, Task> _tasks = new ConcurrentDictionary<string, Task>();
        private ConcurrentDictionary<string, Message> _messages = new ConcurrentDictionary<string, Message>();
        private ConcurrentDictionary<string, TaskLog> _tasklogs = new ConcurrentDictionary<string, TaskLog>();
        //private Timer _tasktimer;
        private TimeSpan _updateInterval = TimeSpan.FromMilliseconds(5000);
        CPostDBLayer _dblayer = new CPostDBLayer();

        private CPost(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
        }

        public static CPost Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        // Add to task class
        public void RequestTask(string ConnId, string TaskCode, string DataConnStr, string LogConnStr, string Param1, string Param2, string Param3,
            string Param4, string ConType, string LocalDir, string FtpAddr, string FtpUserId, string FtpPassw)
        {
            Task task = new Task
            {
                ConnId = ConnId,
                TaskCode = TaskCode,
                DataConnStr = DataConnStr,
                LogConnStr = LogConnStr,
                Param1 = Param1,
                Param2 = Param2,
                Param3 = Param3,
                Param4 = Param4,
                SpName = "",
                ConType = ConType,
                LocalDir = LocalDir,
                FtpAddr = FtpAddr,
                FtpUsrId = FtpUserId,
                FtpUsrPassw = FtpPassw,
                Status = (int)TaskStatus.ForRunnning,
            };
            _tasks.TryAdd(ConnId, task);
            Message MsgOrig = _messages[ConnId];
            Message message = new Message
            {
                ConnId = ConnId,
                Msg = "Task Code: << " + TaskCode + " >>   Status: << Task Started ..... >>"
            };
            _messages.TryUpdate(ConnId, message, MsgOrig);
            Interlocked.Increment(ref _msgcnt);
            Interlocked.Increment(ref _taskcnt);
            UpdateMessages(message.ConnId, message.Msg, _msgcnt, _taskcnt, _taskdone, _taskfailed);
            RunTask(ConnId);
            Task taskd = _tasks[ConnId];
            string errmsg = (taskd.Status == (int)TaskStatus.Done) ? "" : _messages[ConnId].Msg;
            String msg = "";
            switch (taskd.Status)
            {
                case (int)TaskStatus.Done:
                    msg = "Task Code: << " + TaskCode + " >>   Status: << Successfully Done ..... >>";
                    break;
                case (int)TaskStatus.Failed:
                    msg = "Task Code: << " + TaskCode + " >>   Status: << Task Failed ..... >>   Exception : " + errmsg;
                    break;
                default:
                    msg = _messages[ConnId].Msg;
                    break;
            };
            Message message2 = new Message
            {
                ConnId = ConnId,
                Msg = msg
            };
            MsgOrig = _messages[ConnId];
            _messages.TryUpdate(ConnId, message2, MsgOrig);
            Interlocked.Increment(ref _msgcnt);
            if (task.Status == (int)TaskStatus.Done) Interlocked.Increment(ref _taskdone);
            if (task.Status == (int)TaskStatus.Failed) Interlocked.Increment(ref _taskfailed);
            UpdateMessages(_messages[ConnId].ConnId, _messages[ConnId].Msg, _msgcnt, _taskcnt, _taskdone, _taskfailed);
        }

        public IEnumerable<Task> GetAllTasks() 
        {
            return _tasks.Values;
        }

        public IEnumerable<Message> GetAllMessages()
        {
            return _messages.Values;
        }

        public void RunTask(string ConnId) 
        {
            Task task = _tasks[ConnId];
            try
            {
                TaskLog tasklog = new TaskLog 
                {
                    LogId = 0,
                    Timeg = DateTime.Now,
                    NotifTime = DateTime.Now,
                    Task = task.TaskCode,
                    QId = task.ConnId,
                    FileCreated = "",
                    FileProcessed = "",
                    NoOfRecords = 0,
                    NoOfRetries = 0,
                    SystemError = "",
                    TaskError = _messages[ConnId].Msg,
                    Acknowledged = "",
                    DateAcknowledged = null,
                    AcknowledgedFileRecs = 0,
                    AlertError = "",
                    Success = true
                };
                _tasklogs.TryAdd(ConnId, tasklog);
                /*
                int logid = _dblayer.WriteToLogHFile(tasklog, task.LogConnStr);
                if (logid == -1)
                {
                    _tasks[ConnId].Status = (int)TaskStatus.Failed;
                    _messages[ConnId].Msg = "Can't write to header log file";
                    UpdateMessages(_messages[ConnId].ConnId, _messages[ConnId].Msg, _msgcnt, _taskcnt, _taskdone, _taskfailed);
                }
                else 
                {
                    _tasklogs[ConnId].LogId = logid;
                    tasklog = _tasklogs[ConnId];
                    logid = _dblayer.WriteToLogDFile(tasklog, task.LogConnStr);
                    if (logid == -1)
                    {
                        _tasks[ConnId].Status = (int)TaskStatus.Failed;
                        _messages[ConnId].Msg = "Can't write to detail log file";
                        UpdateMessages(_messages[ConnId].ConnId, _messages[ConnId].Msg, _msgcnt, _taskcnt, _taskdone, _taskfailed);
                    }
                    else 
                    {*/
                        DataTable dt = _dblayer.DoTask(task);

                        if (task.ConType != null || task.ConType != string.Empty)
                        {
                            if (task.LocalDir != null || task.LocalDir != string.Empty)
                            {
                                _tasks[ConnId].Status = (int)TaskStatus.ToFile;
                                _messages[ConnId].Msg = "Please wait... Task now generating output file";
                                UpdateMessages(_messages[ConnId].ConnId, _messages[ConnId].Msg, _msgcnt, _taskcnt, _taskdone, _taskfailed);
                                string outputfile = _dblayer.WriteXMLFile(dt, task.LocalDir, task.TaskCode + "_TableName");
                                _messages[ConnId].Msg = "Task have generated output file... FileName : " + outputfile;
                            }
                        }
                    //}
                //}
            }
            catch (Exception e)
            {
                Task taskd = _tasks[ConnId];
                taskd.Status = (int)TaskStatus.Failed;
                _tasks.TryUpdate(ConnId, taskd, task);
                Message MsgOrig = _messages[ConnId];
                Message Msg = new Message { ConnId = ConnId, Msg = e.Message };
                _messages.TryUpdate(ConnId, Msg, MsgOrig);
                TaskLog tasklogOrig = _tasklogs[ConnId];
                //int logid = _dblayer.WriteToLogDFile(tasklog, task.LogConnStr);
            }

        }

        public void Start(dynamic msg) 
        {
            //_tasktimer = new Timer(CheckMessages, null, _updateInterval, _updateInterval);
            Interlocked.Increment(ref _msgcnt);
            Interlocked.Increment(ref _usercnt);
            Message Msg = new Message { ConnId = msg.ConnId, Msg = msg.message };
            _messages.TryAdd(msg.ConnId, Msg);
            UpdateMessages(_messages[msg.ConnId].ConnId, _messages[msg.ConnId].Msg, _msgcnt, _taskcnt, _taskdone, _taskfailed);
        }

        public void End(dynamic msg) 
        {
            Interlocked.Decrement(ref _msgcnt);
            Interlocked.Decrement(ref _usercnt);
            Message Msg = _messages[msg.ConnId];
            Msg.Msg = msg.message;
            //_messages.TryRemove(msg.ConnId, out Msg);
            Task task = _tasks[msg.ConnId];
            if (task.Status == (int)TaskStatus.Done) Interlocked.Decrement(ref _taskdone);
            if (task.Status == (int)TaskStatus.Failed) Interlocked.Decrement(ref _taskfailed);
            UpdateMessages(Msg.ConnId, Msg.Msg, _msgcnt, _taskcnt, _taskdone, _taskfailed);
        }

        private void CheckMessages(object state) 
        {
            _msgcnt = 0;
            string msg;
            IEnumerable<Message> messages = GetAllMessages();
            foreach (Message message in messages) 
            {
                _msgcnt += 1;
                UpdateMessages(message.ConnId, message.Msg, _msgcnt, _taskcnt, _taskdone, _taskfailed);
            }
        }

        public void UpdateMessages(string ConnId, string msg, int msgcnt, int taskcnt, int taskdone, int taskfailed) 
        {
            Clients.All.updateMessages(ConnId, msg, msgcnt, taskcnt, taskdone, taskfailed);
        }
    }
}