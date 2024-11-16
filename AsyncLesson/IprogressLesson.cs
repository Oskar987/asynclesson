namespace AsyncLesson;

public static class IProgressLesson
{
	public static async Task HandleProgress()
	{
		/*var progress = new Progress<int>(val =>
		{
		   Console.WriteLine($"Progress: {val} %");
		});*/

		await LongOperation(new Progress());
		Console.WriteLine("Operation completed!!!");
	}

	private static async Task LongOperation(IProgress<int> progress)
	{
		for (int i = 0; i < 100; i++)
		{
			await Task.Delay(100);
			progress.Report(i);
		}
	}
	
	public class Progress : IProgress<int>
	{
		public void Report(int value)
		{
			Console.WriteLine($"Progress: {value} %");
		}
	}
}