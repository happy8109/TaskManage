using System;
using System.Collections.Generic;
using System.Threading;

namespace TaskManage
{
    /// <summary>
    /// 表示一个定时任务项。
    /// </summary>
    public class TaskItem<T>
    {
        /// <summary>
        /// 获取任务的名称。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取任务的状态。
        /// </summary>
        public string Status { get; private set; }

        /// <summary>
        /// 获取任务的返回值。
        /// </summary>
        public T Result { get; private set; }

        private Timer timer;
        private TimeSpan dueTime;
        private TimeSpan period;
        private Func<T> action;

        /// <summary>
        /// 当任务发生错误时发生。
        /// </summary>
        public event Action<Exception> ErrorOccurred;

        /// <summary>
        /// 当任务的进度改变时发生。
        /// </summary>
        public event Action<int> ProgressChanged;

        /// <summary>
        /// 当任务完成时发生。
        /// </summary>
        public event Action<T> TaskCompleted;

        /// <summary>
        /// 创建一个新的任务项。
        /// </summary>
        /// <param name="name">任务的名称。</param>
        /// <param name="action">任务要执行的操作。</param>
        public TaskItem(string name, Func<T> action)
        {
            Name = name;
            this.action = action;
            timer = new Timer(_ => ExecuteAction(), null, Timeout.Infinite, Timeout.Infinite);
            Status = "Created";
        }

        private void ExecuteAction()
        {
            try
            {
                Result = action();
                if (TaskCompleted != null)
                {
                    TaskCompleted(Result);
                }
                Status = "Completed";
            }
            catch (Exception ex)
            {
                if (ErrorOccurred != null)
                {
                    ErrorOccurred(ex);
                }
                Status = "Error";
            }
        }

        /// <summary>
        /// 报告任务的进度。
        /// </summary>
        /// <param name="progress">任务的进度。</param>
        public void ReportProgress(int progress)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(progress);
            }
        }

        /// <summary>
        /// 设置任务的启动时间和执行周期。
        /// </summary>
        /// <param name="dueTime">任务的启动时间。</param>
        /// <param name="period">任务的执行周期。</param>
        public void SetTime(TimeSpan dueTime, TimeSpan period)
        {
            this.dueTime = dueTime;
            this.period = period;
        }

        /// <summary>
        /// 启动任务。
        /// </summary>
        public void Start()
        {
            timer.Change(dueTime, period);
            Status = "Running";
        }

        /// <summary>
        /// 停止任务。
        /// </summary>
        public void Stop()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            Status = "Stopped";
        }

        /// <summary>
        /// 继续任务。
        /// </summary>
        public void Continue()
        {
            timer.Change(dueTime, period);
            Status = "Running";
        }

        /// <summary>
        /// 移除任务，释放资源。
        /// </summary>
        public void Remove()
        {
            timer.Dispose();
            Status = "Removed";
        }
    }

    /// <summary>
    /// 表示一个定时任务的调度器。
    /// </summary>
    public class ScheduledTask<T>
    {
        private Dictionary<string, TaskItem<T>> tasks = new Dictionary<string, TaskItem<T>>();
        private readonly object lockObj = new object();

        /// <summary>
        /// 添加一个新的任务。
        /// </summary>
        /// <param name="name">任务的名称。</param>
        /// <param name="action">任务要执行的操作。</param>
        /// <returns>新添加的任务。</returns>
        public TaskItem<T> AddTask(string name, Func<T> action)
        {
            var task = new TaskItem<T>(name, action);
            lock (lockObj)
            {
                tasks.Add(name, task);
            }
            return task;
        }

        /// <summary>
        /// 移除一个任务。
        /// </summary>
        /// <param name="name">要移除的任务的名称。</param>
        public void RemoveTask(string name)
        {
            lock (lockObj)
            {
                tasks[name].Remove();
                tasks.Remove(name);
            }
        }

        /// <summary>
        /// 停止所有的任务。
        /// </summary>
        public void StopAll()
        {
            lock (lockObj)
            {
                foreach (var task in tasks.Values)
                {
                    task.Stop();
                }
            }
        }

        /// <summary>
        /// 获取任务的数量。
        /// </summary>
        /// <returns>任务的数量。</returns>
        public int GetTaskCount()
        {
            lock (lockObj)
            {
                return tasks.Count;
            }
        }

        /// <summary>
        /// 获取所有任务的状态。
        /// </summary>
        /// <returns>一个字典，其中的键是任务的名称，值是任务的状态。</returns>
        public Dictionary<string, string> GetTaskStatuses()
        {
            var statuses = new Dictionary<string, string>();
            lock (lockObj)
            {
                foreach (var task in tasks)
                {
                    statuses.Add(task.Key, task.Value.Status);
                }
            }
            return statuses;
        }

        /// <summary>
        /// 获取指定任务的状态。
        /// </summary>
        /// <param name="name">任务的名称。</param>
        /// <returns>任务的状态，如果任务不存在，则返回空字符串。</returns>
        public string GetTaskStatus(string name)
        {
            lock (lockObj)
            {
                if (tasks.ContainsKey(name))
                {
                    return tasks[name].Status;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}