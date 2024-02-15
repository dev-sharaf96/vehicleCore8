using System;
using System.Collections.Generic;
using System.Linq;
using Tameenk.Common.Utilities;
using Tameenk.Core.Caching;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
namespace Tameenk.Services.Policy.Components
{
    /// <summary>
    /// Task service
    /// </summary>
    public class ScheduleTaskService : IScheduleTaskService
    {
        #region Fields

        private readonly IRepository<ScheduleTask> _taskRepository;
        private readonly ICacheManager _cacheManger;

        #endregion

        #region Ctor

        public ScheduleTaskService(IRepository<ScheduleTask> taskRepository, ICacheManager cacheManger)
        {
            this._taskRepository = taskRepository;
            _cacheManger = cacheManger ?? throw new ArgumentNullException(nameof(ICacheManager));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a task
        /// </summary>
        /// <param name="task">Task</param>
        public virtual void DeleteTask(ScheduleTask task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            _taskRepository.Delete(task);
        }

        /// <summary>
        /// Gets a task
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <returns>Task</returns>
        public virtual ScheduleTask GetTaskById(int taskId)
        {
            if (taskId == 0)
                return null;

            return _taskRepository.Table.FirstOrDefault(t => t.Id == taskId);
        }

        /// <summary>
        /// Gets a task by its type
        /// </summary>
        /// <param name="type">Task type</param>
        /// <returns>Task</returns>
        public virtual ScheduleTask GetTaskByType(string type)
        {
            if (String.IsNullOrWhiteSpace(type))
                return null;

            var query = _taskRepository.Table;
            query = query.Where(st => st.Type == type);
            query = query.OrderByDescending(t => t.Id);

            var task = query.FirstOrDefault();
            return task;
        }

        /// <summary>
        /// Gets all tasks
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Tasks</returns>
        public virtual IList<ScheduleTask> GetAllTasks(bool showHidden = false)
        {
            string serverIP = Utilities.GetAppSetting("ServerIP");
            return _cacheManger.Get(string.Format("_Get__alL_TaSkS_CACHE_Key_"+ serverIP, 0, 1000, 300), () =>
            {
                var query = _taskRepository.Table;
                if (!showHidden)
                {
                    query = query.Where(t => t.Enabled);
                }
                if (!string.IsNullOrEmpty(serverIP))
                {
                    query = query.Where(t => t.ServerIP == serverIP);
                }
                query = query.OrderByDescending(t => t.Seconds);
                var tasks = query.ToList();
                return tasks;
            });
        }
           
        

        /// <summary>
        /// Inserts a task
        /// </summary>
        /// <param name="task">Task</param>
        public virtual void InsertTask(ScheduleTask task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            _taskRepository.Insert(task);
        }

        /// <summary>
        /// Updates the task
        /// </summary>
        /// <param name="task">Task</param>
        public virtual void UpdateTask(ScheduleTask task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            _taskRepository.Update(task);
        }
        public bool UpdateScheduleTask(int id, DateTime? lastStart, DateTime? lastEnd, DateTime? lastSuccess)        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "UpdateScheduleTask";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter idParam = new SqlParameter() { ParameterName = "Id", Value = id };
                SqlParameter lastStartParam = new SqlParameter("LastStart", SqlDbType.DateTime);
                SqlParameter lastEndParam = new SqlParameter("LastEnd", SqlDbType.DateTime);
                SqlParameter lastSuccessParam = new SqlParameter("LastSuccess", SqlDbType.DateTime);

                if (lastStart.HasValue)
                    lastStartParam.Value = lastStart.Value;
                else
                    lastStartParam.Value = (object)DBNull.Value;

                if (lastEnd.HasValue)
                    lastEndParam.Value = lastEnd.Value;
                else
                    lastEndParam.Value = (object)DBNull.Value;

                if (lastSuccess.HasValue)
                    lastSuccessParam.Value = lastSuccess.Value;
                else
                    lastSuccessParam.Value = (object)DBNull.Value;

                command.Parameters.Add(idParam);
                command.Parameters.Add(lastStartParam);
                command.Parameters.Add(lastEndParam);
                command.Parameters.Add(lastSuccessParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                if (result == 0)
                {
                    return true;
                }

                return false;
            }            catch (Exception exp)            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\UpdateScheduleTask_log_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + exp.ToString());
                return false;
            }            finally            {                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();            }        }
        public ScheduleTask GetFromScheduleTask(string type)        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            { 
                ScheduleTask task = null;
          
            dbContext.DatabaseInstance.CommandTimeout = 60;            var command = dbContext.DatabaseInstance.Connection.CreateCommand();            command.CommandText = "GetFromScheduleTask";            command.CommandType = CommandType.StoredProcedure;            SqlParameter typeParam = new SqlParameter() { ParameterName = "Type", Value = type };            command.Parameters.Add(typeParam);           
            dbContext.DatabaseInstance.Connection.Open();            var reader = command.ExecuteReader();            task = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ScheduleTask>(reader).FirstOrDefault();
            dbContext.DatabaseInstance.Connection.Close();
                if (task != null)
            {
                return task;
            }
          
            return null;
            }            catch (Exception exp)            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetFromScheduleTask_log_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + exp.ToString());
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }        }
        #endregion
    }
}
