namespace AsyncLesson;

public class ContinueWithLesson
{
	public static void DoContinueWith()
	{
		Task<int> task = Task.Run(() =>
		{
			Console.WriteLine("Началась длительная операция");
			Task.Delay(2000).Wait();
			return 42;
		});

		task.ContinueWith(t =>
		{
			if (t.IsFaulted)
			{
				Console.WriteLine("В задаче произошла ошибка");
			}
			else
			{
				Console.WriteLine("Задача выполнилась успешно");
			}
		});
	}
	
	public static void DoContinueWith2(int num)
	{
		Task.Run(() =>
			{

				Task.Delay(2000).Wait();
				return num + 5;
			})
			.ContinueWith(t => t.Result * 2)
			.ContinueWith(t => t.Result + 5, TaskContinuationOptions.NotOnCanceled)
			.ContinueWith(t =>
			{
				if (t.Result % 5 == 0)
				{
					//throw new InvalidOperationException("task is faulted because divided to 5!!!");	
				}
				
				return t.Result + 5;
			}, TaskContinuationOptions.NotOnFaulted)
			.ContinueWith(t => t.Result + 20, TaskContinuationOptions.OnlyOnFaulted)
			.ContinueWith(t =>
			{
				if (t.IsFaulted)
				{
					Console.WriteLine($"Задача завершилась с ошибкой");
				}
				else
				{
					Console.WriteLine($"Результат нашей работы {t.Result}");	
				}
			});
	}
}