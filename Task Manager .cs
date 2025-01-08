using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace TaskManagerApp
{
	class Program
	{
		static void Main()
		{
			while (true)
			{
				Console.Clear();
				Console.WriteLine("=== Task Manager ===");
				Console.WriteLine("1. List Running Processes");
				Console.WriteLine("2. Kill a Process");
				Console.WriteLine("3. Monitor CPU and Memory Usage");
				Console.WriteLine("4. List Top 5 Memory Consuming Processes");
				Console.WriteLine("5. Exit");

				Console.Write("Enter your choice (1-5): ");
				string choice = Console.ReadLine();

				switch (choice)
				{
					case "1":
						ListProcesses();
						break;
					case "2":
						KillProcess();
						break;
					case "3":
						MonitorSystemUsage();
						break;
					case "4":
						ListTopMemoryConsumingProcesses();
						break;
					case "5":
						Console.WriteLine("Exiting... Goodbye!");
						return;
					default:
						Console.WriteLine("Invalid choice. Press Enter to try again.");
						break;
				}

				Console.WriteLine("\nPress Enter to continue.");
				Console.ReadLine();
			}
		}

		static void ListProcesses()
		{
			Console.Clear();
			Console.WriteLine("=== Running Processes ===");
			Process[] processes = Process.GetProcesses();

			foreach (var process in processes.OrderBy(p => p.ProcessName))
			{
				try
				{
					
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.Write($"PID: {process.Id}");

					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write($", Name: {process.ProcessName}");

					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.Write($", Memory: {process.WorkingSet64 / 1024} KB");

					Console.ResetColor();
					Console.WriteLine();
				}
				catch (Exception)
				{
					// ignore processes that needs admin
				}
			}
		}

		static void KillProcess()
		{
			Console.Clear();
			Console.Write("Enter the PID of the process to kill: ");
			if (int.TryParse(Console.ReadLine(), out int pid))
			{
				try
				{
					Process process = Process.GetProcessById(pid);
					process.Kill();
					Console.WriteLine($"Process with PID {pid} has been terminated.");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error: {ex.Message}");
				}
			}
			else
			{
				Console.WriteLine("Invalid PID. Press Enter to try again.");
			}
		}

		static void MonitorSystemUsage()
		{
			Console.Clear();
			Console.WriteLine("=== Monitoring CPU and Memory Usage ===");

			// perf counter for mem
			PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
			PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");

			while (true)
			{
				try
				{
					//gets cpu and ram values 
					float cpuUsage = cpuCounter.NextValue();
					float availableRam = ramCounter.NextValue();

					Console.Clear();
					Console.WriteLine($"CPU Usage: {cpuUsage}%");
					Console.WriteLine($"Available RAM: {availableRam} MB");

					// updates every sec
					Thread.Sleep(1000);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error: {ex.Message}");
					break;
				}
			}
		}

		static void ListTopMemoryConsumingProcesses()
		{
			Console.Clear();
			Console.WriteLine("=== Top 5 Memory Consuming Processes ===");

			// Get the list of processes
			var processes = Process.GetProcesses()
				.Where(p => p.WorkingSet64 > 0) // Filters out processes wth 0 memory usage
				.OrderByDescending(p => p.WorkingSet64) // Sorts by lower mem useage)
				.Take(5) // top 5 mem uses
				.ToList();

			// top 5 process
			foreach (var process in processes)
			{
				try
				{
					
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.Write($"PID: {process.Id}");

					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write($", Name: {process.ProcessName}");

					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.Write($", Memory: {process.WorkingSet64 / 1024 / 1024} MB");

					Console.ResetColor();
					Console.WriteLine();
				}
				catch (Exception)
				{
					
				}
			}
		}
	}
}
