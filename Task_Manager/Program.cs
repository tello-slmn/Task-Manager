using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Task_Manager
{
    internal class Program
    {
        private static readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "tasks.csv");

        static void Main(string[] args)
        {
            Menu();
            Console.WriteLine("\n\tPress any key to exit...");
            Console.ReadKey();
        }

        /* FEATURES
         * Add, edit, delete tasks.
         * Mark tasks as done.
         * Save/load tasks to a local file (JSON or XML).
         * Clean UI.*/
        #region Menu
        private static void Menu()
        {
            TaskService service = new TaskService();
            service.tasks = LoadTasksFromCSV();
            int choice_;
            
            do
            {
                Console.Clear();
                Console.WriteLine("\t==== Task Manager ===\n");
                Console.WriteLine("\t1. Add\n\t2. Edit\n\t3. Delete\n\t4. Mark task as done\n\t5. View all tasks\n\t6. Exit");
                Console.Write("\n\tEnter your choice: ");
                int choice;
                while (!int.TryParse(Console.ReadLine(), out choice) || !(choice >= 0 && choice < 7))
                    Console.Write("\tERROR!\n\tEnter a valid choice: ");
                choice_ = choice;

                switch (choice)
                {
                    case 1:
                        AddTask(service);
                        break;
                    case 2:
                        EditTask(service);
                        break;
                    case 3:
                        DeleteTask(service);
                        break;
                    case 4:
                        MarkTaskAsDone(service);
                        break;
                    case 5:
                        ViewAllTask(service);
                        break;
                }
                SaveTasksToCSV(service.tasks);
                if (choice != 6)
                {
                    Console.WriteLine("\n\tPress any key to continue...");
                    Console.ReadKey();
                }
            }
            while (choice_ != 6);
        }
        #endregion
        #region Add
        private static void AddTask(TaskService service)
        {
            Console.Clear();
            Console.WriteLine("\t=== Adding Task ===\n");

            Console.Write("\tEnter a task name: ");
            string taskName = Console.ReadLine();

            if (service.GetTask(taskName) == null)
            {
                Console.Write("\tEnter a description of a task: ");
                string taskDescription = Console.ReadLine();
                service.AddTask(taskName, taskDescription);
                Console.WriteLine($"\tTask '{taskName}' added successfully.");
            }
            else
                Console.WriteLine($"\tTask '{taskName}' cannot be added it already exists.");
        }
        #endregion
        #region Edit
        private static void EditTask(TaskService service)
        {
            Console.Clear();
            if (service.tasks.Count == 0)
                Console.WriteLine("\tERROR\n\tNo tasks found... add them first.");
            else
            {
                Console.Clear();
                Console.WriteLine("\t=== Editing Task ===\n");

                Console.Write("\tEnter a task to edit: ");
                string taskName = Console.ReadLine();


                if (service.GetTask(taskName) != null)
                {
                    Console.Write($"\tUpdate name of the task named '{taskName}': ");
                    string newTaskName = Console.ReadLine();
                    Console.Write($"\tUpdate desciption of the task named '{newTaskName}': ");
                    string taskDescription = Console.ReadLine();
                    service.Update(taskName, newTaskName, taskDescription);
                    Console.WriteLine("\tUpdated successfully.");
                }
                else
                    Console.WriteLine($"\tTask '{taskName}' not found.");
            }
        }
        #endregion
        #region Delete
        private static void DeleteTask(TaskService service)
        {
            Console.Clear();
            if (service.tasks.Count == 0)
                Console.WriteLine("\tERROR\n\tNo tasks found... add them first.");
            else
            {
                Console.Clear();
                Console.WriteLine("\t=== Deleting Task ===\n");

                Console.Write("\tEnter a task to delete: ");
                string taskName = Console.ReadLine();
                var task = service.GetTask(taskName);

                if (task == null)
                    Console.WriteLine($"\tTask '{taskName}' is not found.");
                else
                {
                    service.Delete(taskName);
                    Console.WriteLine($"\tTask '{taskName}' deleted successfully.");
                }
            }
        }
        #endregion
        #region Mark Task as Done
        private static void MarkTaskAsDone(TaskService service)
        {
            Console.Clear();
            if (service.tasks.Count == 0)
                Console.WriteLine("\tERROR\n\tNo tasks found... add them first.");
            else
            {
                Console.Clear();
                Console.WriteLine("\t=== Marking Task as Done ===\n");
                Console.Write("\tEnter a task to mark as done: ");
                string taskName = Console.ReadLine();

                var task = service.GetTask(taskName);

                if (task != null)
                {
                    service.MarkAsDone(taskName);
                    Console.WriteLine($"\tTask '{taskName}' marked as done successfully.");
                }
                else
                    Console.WriteLine($"\tTask '{taskName}' is not found.");
            }
        }
        #endregion
        #region View All Tasks
        private static void ViewAllTask(TaskService service)
        {
            Console.Clear();
            if (service.tasks.Count == 0)
                Console.WriteLine("\tERROR\n\tNo tasks found... add them first.");
            else
            {
                Console.WriteLine("\t=== All Tasks ===\n");
                foreach (var task in service.tasks)
                    Console.WriteLine($"\t{task}");
            }
        }
        #endregion
        #region Load Tasks from CSV
        private static List<Task> LoadTasksFromCSV()
        {
            List<Task> tasks = new List<Task>();

            if (!File.Exists(filePath))
            {
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.WriteLine("Task Name,Description,Status");
                }
            }

            string[] lines = File.ReadAllLines(filePath);
            for (int i = 1; i < lines.Length; i++)//skip header
            {
                string[] parts = lines[i].Split(',');
                if (parts.Length == 3)
                {
                    string taskName = parts[0].Trim();
                    string description = parts[1].Trim();
                    bool status = bool.Parse(parts[2].Trim());
                    Task task = new Task(taskName, description, status);
                    tasks.Add(task);
                    Console.WriteLine("\tTask added successfully.");
                }
                else
                    Console.WriteLine($"\tInvalid line format at line {i + 1}: {lines[i]}");
            }
            return tasks;
        }
        #endregion
        #region Save
        // Note: You may want to implement saving tasks back to the CSV file when exiting the application.
        // This can be done by iterating through the tasks list and writing each task to the file.
        private static void SaveTasksToCSV(List<Task> tasks)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("Task Name,Description,Status");
                foreach (var task in tasks)
                {
                    sw.WriteLine($"{task.TaskName},{task.Description},{task.Status}");
                }
            }
        }
        #endregion
    }
}
