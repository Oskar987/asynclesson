namespace AsyncLesson;

public class Program
{
	public static async Task Main(string[] args)
	{
		/*var cts = new CancellationTokenSource();

		var task = Task.Run(() => CancellationTokenLesson.LongOperation(cts.Token));
		
		cts.CancelAfter(2000);

		try
		{
			await task;
		}
		catch (OperationCanceledException exception)
		{
			Console.WriteLine("Operation was canceled " + exception.Message);
		}*/
		
		//ContinueWithLesson.DoContinueWith();
		//ContinueWithLesson.DoContinueWith2(3);

		//await ExceptionLesson.DoAsync();
		//ExceptionLesson.DoAsyncAggregateExceptions();
		//ExceptionLesson.HandleNestedExceptions();
		//ExceptionLesson.HandleSpecificExceptions();
		//IProgressLesson.HandleProgress();
        
		//ThreadManager.ContextSwitching();
		//ThreadManager.ShareResource();
		
		//ThreadManager.WorkWithThreadPool();
		//ThreadManager.DemoDeadLock();
		
		//ThreadManager.AutoResetEventDemo();
		//ThreadManager.DemoBarierLock();
		ThreadManager.UseVolatile();
		
		Console.ReadKey();
	}
}