namespace AsyncLesson;

public static class ExceptionLesson
{
	public static async Task DoAsync()
	{
		var task1 = Task.Run(() => throw new NotImplementedException("NotImplementedException is expected!"));
		var task2 = Task.Run(() => throw new InvalidOperationException("InvalidOperationException is expected!"));

		Task allTasks = Task.WhenAll(task1, task2);

		try
		{
			await allTasks;
		}
		catch (Exception e)
		{
			var exc = allTasks.Exception;

			foreach (var exception in exc.InnerExceptions)
			{
				Console.WriteLine(exception.Message);
			}
		}
	}
	
	public static void DoAsyncAggregateExceptions()
	{
		var task1 = Task.Run(() => throw new NotImplementedException("NotImplementedException is expected!"));
		var task2 = Task.Run(() => throw new InvalidOperationException("InvalidOperationException is expected!"));

		try
		{
			Task.WaitAll(task1, task2);
		}
		catch (AggregateException ae)
		{
			foreach (var exception in ae.InnerExceptions)
			{
				Console.WriteLine(exception.Message);
			}
		}
	}

	public static void HandleNestedExceptions()
	{
		var task = Task.Factory.StartNew(() =>
		{
			var child1 = Task.Factory.StartNew(() =>
			{
					var child2 = Task.Factory.StartNew(() =>
					{
						var child3 = Task.Factory.StartNew(() =>
						{
							var child4 = Task.Factory.StartNew(() =>
							{
								var child5 = Task.Factory.StartNew(() =>
								{
									throw new InvalidOperationException("Child 5 exception");
								}, TaskCreationOptions.AttachedToParent);
								throw new InvalidOperationException("Child 4 exception");
							}, TaskCreationOptions.AttachedToParent);
					
							throw new InvalidOperationException("Child 3 exception");
						}, TaskCreationOptions.AttachedToParent);
						throw new InvalidOperationException("Child 2 exception");
					}, TaskCreationOptions.AttachedToParent);
					
					throw new InvalidOperationException("Child 1 exception");
				}, TaskCreationOptions.AttachedToParent);

				throw new InvalidOperationException("task  exception");
			}, TaskCreationOptions.AttachedToParent);

		try
		{
			task.Wait();
		}
		catch (AggregateException ae)
		{
			var nestedExceptions = task.Exception;
			var flattenExceptions = nestedExceptions.Flatten();
			
			foreach (var exception in flattenExceptions.InnerExceptions)
			{
				Console.WriteLine(exception.Message);
			}
		}
	}

	public static void HandleSpecificExceptions()
	{
		var task1 = Task.Run(() => throw new NotImplementedException("NotImplementedException is expected!"));
		var task2 = Task.Run(() => throw new InvalidOperationException("InvalidOperationException is expected!"));

		try
		{
			Task.WaitAll(task1, task2);
		}
		catch (AggregateException ae)
		{
			ae.Handle(ex =>
			{
				if (ex is InvalidOperationException)
				{
					Console.WriteLine("InvalidOperationException");
				}
				
				if (ex is NotImplementedException)
				{
					Console.WriteLine("NotImplementedException");
				}

				return true;
			});
		}
	}

	public static void HandleVoid()
	{
		try
		{
			   MethodWithException();
			   Task.Run(() => MethodWithException2());
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	private static async void MethodWithException()
	{
		try
		{
			await Task.Delay(1000);
			throw new NotImplementedException("NotImplementedException is expected!");
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}
	
	private static async void MethodWithException2()
	{
		await Task.Delay(1000);
		throw new NotImplementedException("NotImplementedException is expected!");
	}
	
}