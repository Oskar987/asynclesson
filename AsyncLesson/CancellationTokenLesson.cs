namespace AsyncLesson;

public static class CancellationTokenLesson
{
	public static void LongOperation(CancellationToken cancellationToken = default)
	{
		for (int i = 0; i < 10; i++)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Console.WriteLine($"Работаю с {i}");
			Thread.Sleep(1000);
		}
	}
}