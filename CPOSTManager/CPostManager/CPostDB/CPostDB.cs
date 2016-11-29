using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Data;
using Dapper;
using CPostManager.BusinessObjects;

namespace CPostManager.CPostDB
{
    public class CPostDBLayer
    {
        private DataTable _dt;

        public DataTable DoTask(Task task) 
        {
            //_dt = SqlHelper.QuerySP<SupplierSync>(task.SpName, null, null, null, true, null, task.DataConnStr).ToDataTable<SupplierSync>();
            switch (task.TaskCode) 
            {
                case "suppliersnyc":
                    _dt = SqlHelper.QuerySP<SupplierSync>("p_syncSup", new { lnum = "" }, null, null, true, null, task.DataConnStr).ToDataTable<SupplierSync>();
                    break;
            }
            return _dt;
        } // End DoTask

        public string WriteXMLFile(DataTable dt, string LocalDir, string DbName) 
        {
            string _xmlfile = "";
            try
            {
                dt.TableName = DbName;
                dt.WriteXml(LocalDir + "CPostTest.xml", XmlWriteMode.WriteSchema, false);
                _xmlfile = LocalDir + "CPostTest.xml";
            }
            catch (Exception e) 
            {
                _xmlfile = "Error: "+e.Message;
            }
            return _xmlfile;
        } // End WriteXMLFile

        public int WriteToLogHFile(TaskLog tasklog, string LogConnStr)
        {
            string sql = @"
                begin try
                    begin transaction
                        insert into epsloghd (
                            timeg
                            , notiftime
                            , task
                            , qid
                            , filecreated
                            , fileprocessed
                            , noofrecords
                            , noofretries
                            , systemerror
                            , taskerror
                            , acknowledged
                            , dateacknowledged
                            , acknowledgedfilerecs
                            , allerterror
                            , success)
                        values (
                            @timeg
                            , @notiftime
                            , @task
                            , @qid
                            , @filecreated
                            , @fileprocessed
                            , @noofrecords
                            , @noofretries
                            , @systemerror
                            , @taskerror
                            , @acknowledged
                            , @dateacknowledged
                            , @acknowledgedfilerecs
                            , @allerterror
                            , @success)
                    commit transaction
                    select cast(scope_identity() as int)
                end try
                begin catch
                    rollback transaction
                    select -1
                end catch";

            int logid = SqlHelper.ExecuteScalarSQL<int>(sql,
                new { timeg = tasklog.Timeg, notiftime = tasklog.NotifTime, task = tasklog.Task,
                    qid = tasklog.QId, filecreated = tasklog.FileCreated, fileprocessed = tasklog.FileProcessed,
                    noofrecords = tasklog.NoOfRecords, noofretries = tasklog.NoOfRetries, systemerror = tasklog.SystemError,
                    taskerror = tasklog.TaskError, acknowledged = tasklog.Acknowledged, dateacknowledged = tasklog.DateAcknowledged,
                    acknowledgedfilerecs = tasklog.AcknowledgedFileRecs, allerterror = tasklog.AlertError, success = tasklog.Success }
                    , null, null, null, LogConnStr);
            return logid;
        } // End WriteToLogHFile

        public int WriteToLogDFile(TaskLog tasklog, string LogConnStr)
        {
            string sql = @"
                begin try
                    begin transaction
                        insert into epslogln (
                            logid
                            , timeg
                            , notiftime
                            , task
                            , qid
                            , filecreated
                            , fileprocessed
                            , noofrecords
                            , noofretries
                            , systemerror
                            , taskerror
                            , acknowledged
                            , dateacknowledged
                            , acknowledgedfilerecs
                            , allerterror
                            , success)
                        values (
                            @logid
                            , @timeg
                            , @notiftime
                            , @task
                            , @qid
                            , @filecreated
                            , @fileprocessed
                            , @noofrecords
                            , @noofretries
                            , @systemerror
                            , @taskerror
                            , @acknowledged
                            , @dateacknowledged
                            , @acknowledgedfilerecs
                            , @allerterror
                            , @success)
                    commit transaction
                    select cast(scope_identity() as int)
                end try
                begin catch
                    rollback transaction
                    select -1
                end catch";

            int newid = SqlHelper.ExecuteScalarSQL<int>(sql,
                new
                {
                    logid = tasklog.LogId,
                    timeg = tasklog.Timeg,
                    notiftime = tasklog.NotifTime,
                    task = tasklog.Task,
                    qid = tasklog.QId,
                    filecreated = tasklog.FileCreated,
                    fileprocessed = tasklog.FileProcessed,
                    noofrecords = tasklog.NoOfRecords,
                    noofretries = tasklog.NoOfRetries,
                    systemerror = tasklog.SystemError,
                    taskerror = tasklog.TaskError,
                    acknowledged = tasklog.Acknowledged,
                    dateacknowledged = tasklog.DateAcknowledged,
                    acknowledgedfilerecs = tasklog.AcknowledgedFileRecs,
                    allerterror = tasklog.AlertError,
                    success = tasklog.Success
                }
                    , null, null, null, LogConnStr);
            return newid;
        } // End WriteToLogDFile
    }
}