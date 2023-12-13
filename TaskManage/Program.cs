using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskManage
{
    class Program
    {
        static void Main(string[] args)
        {
            ScheduledTask<string> scheduledTask = new ScheduledTask<string>();

            var task1 = scheduledTask.AddTask("Task 1", () =>
            {
                string message = "Task 1 executed at " + DateTime.Now;
                Console.WriteLine(message);
                return message;
            });
            task1.SetTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
            task1.Start();

            var task2 = scheduledTask.AddTask("Task 2", () =>
            {
                string message = "Task 2 executed at " + DateTime.Now;
                Console.WriteLine(message);
                return message;
            });
            task2.SetTime(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10));
            task2.Start();

            PrintTaskCountAndStatus(scheduledTask);

            // 添加对 GetTaskStatus 方法的测试
            Console.WriteLine("Press any key to get task 1 status...");
            Console.ReadKey();
            string task1Status = scheduledTask.GetTaskStatus("Task 1");
            Console.WriteLine("Task 1 status: " + task1Status);

            Console.WriteLine("Press any key to get task 2 status...");
            Console.ReadKey();
            string task2Status = scheduledTask.GetTaskStatus("Task 2");
            Console.WriteLine("Task 2 status: " + task2Status);

            Console.WriteLine("Press any key to get non-existent task status...");
            Console.ReadKey();
            string nonExistentTaskStatus = scheduledTask.GetTaskStatus("Non-existent task");
            Console.WriteLine("Non-existent task status: " + nonExistentTaskStatus);

            Console.WriteLine("Press any key to stop task 1...");
            Console.ReadKey();
            task1.Stop();
            Console.WriteLine("Task 1 status: " + task1.Status);

            PrintTaskCountAndStatus(scheduledTask);

            Console.WriteLine("Press any key to continue task 1...");
            Console.ReadKey();
            task1.Continue();
            Console.WriteLine("Task 1 status: " + task1.Status);

            PrintTaskCountAndStatus(scheduledTask);

            Console.WriteLine("Press any key to remove task 1...");
            Console.ReadKey();
            scheduledTask.RemoveTask("Task 1");

            PrintTaskCountAndStatus(scheduledTask);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            scheduledTask.StopAll();
        }

        static void PrintTaskCountAndStatus(ScheduledTask<string> scheduledTask)
        {
            Console.WriteLine("Task count: " + scheduledTask.GetTaskCount());
            foreach (var taskStatus in scheduledTask.GetTaskStatuses())
            {
                Console.WriteLine("Task name: " + taskStatus.Key + ", status: " + taskStatus.Value);
            }
        }
    }
}