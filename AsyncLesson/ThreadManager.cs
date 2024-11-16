namespace AsyncLesson;

public static class ThreadManager
{
	public static void ContextSwitching()
	{
		Thread thread = new Thread(WriteNumbers);
		thread.Name = "Поток 1";
		
		thread.Start();

		Thread.CurrentThread.Name = "Главный поток выполнения";

		for (int i = 0; i < 1000; i++)
		{
			Console.WriteLine(" A " + i + $" {Thread.CurrentThread.Name}");
		}

	}

	private static void WriteNumbers(object? obj)
	{
		for (int i = 0; i < 1000; i++)
		{
			Console.WriteLine(" N " + i + $" {Thread.CurrentThread.Name}");
		}
	}

	private static bool _isCompleted;
	private static readonly object lockCompleted = new object();
	public static void ShareResource()
	{
		Thread thread = new Thread(HelloWorld);
		thread.Name = "Поток 1";
		thread.Start();

		ThreadPool.QueueUserWorkItem(HelloWorld2, DateTime.Now);
		
		Thread thread2 = Thread.CurrentThread;
		Thread.CurrentThread.Name = "Главный поток";
		Console.WriteLine(Thread.CurrentThread.Name);
		Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
		HelloWorld();
	}

	private static void HelloWorld2(object? state)
	{
		Console.WriteLine(state);
		Console.WriteLine("1 - Этот поток из пула потоков: " + Thread.CurrentThread.IsThreadPoolThread);
		Console.WriteLine("2 -  " + Thread.CurrentThread.Name);
	}

	private static void HelloWorld()
	{
		Console.WriteLine("1 - Этот поток из пула потоков: " + Thread.CurrentThread.IsThreadPoolThread);
		Console.WriteLine("2 -  " + Thread.CurrentThread.Name);
		
		try
		{
			Monitor.Enter(lockCompleted);
			if (!_isCompleted)
			{
				Console.WriteLine("HelloWorld should print only once");
				_isCompleted = true;
			}
		}
		finally
		{
			Monitor.Exit(lockCompleted);
		}
	}

	public static void WorkWithThreadPool()
	{
		Console.WriteLine("" + Thread.CurrentThread.IsThreadPoolThread);
		Employee employee = new();
		employee.Name = "Petr";
		employee.Company = "Ms";
		
		ThreadPool.QueueUserWorkItem(DisplayEmployeeInfo, employee);

		Console.WriteLine($"Thread pool count  = {ThreadPool.ThreadCount}");
		ThreadPool.SetMaxThreads(1000, 1000);
		ThreadPool.SetMinThreads(100, 100);
		ThreadPool.GetAvailableThreads(out var workerThreads, out var completionPortThreads);
		Console.WriteLine($" 1 Worker threads count : {workerThreads}, I/O threads count : {completionPortThreads},");
		ThreadPool.SetMaxThreads(workerThreads / 2, completionPortThreads / 2);
		
		
		ThreadPool.GetAvailableThreads(out var workerThreads2, out var completionPortThreads2);
		Console.WriteLine($"2 Worker threads count : {workerThreads2}, I/O threads count : {completionPortThreads2},");
	}

	private static void DisplayEmployeeInfo(object? state)
	{
		Console.WriteLine("" + Thread.CurrentThread.IsThreadPoolThread);
		Employee empl = state as Employee;
		Console.WriteLine($"Employee  = Name : {empl.Name}, Company : {empl.Company}");
	}

	public static void DemoDeadLock()
	{
		object lockObject = new object();
		object lockObject2 = new object();
		
		new Thread(() =>
		{
			lock (lockObject)
			{
				Console.WriteLine("===> Первый лок захвачен");
				Thread.Sleep(2000);
				lock (lockObject2)
				{
					Console.WriteLine("===> Второй лок захвачен");
				}
			}
		}).Start();

		lock (lockObject2)
		{
			Console.WriteLine("===> Второй лок захвачен главным потоком");
			Thread.Sleep(2000);
			lock (lockObject)
			{
				Console.WriteLine("===> Первый лок захвачен главным потоком");
			}
			
		}
	}


	public static void MutexDemo(string fileName)
	{
		using Mutex mutex = new Mutex(false, "Global\\Mutex1");
		
		Console.WriteLine("===> Ожидаем доступ");

		if (!mutex.WaitOne(TimeSpan.FromSeconds(10),false))
		{
			Console.WriteLine("===> Кто то другой удерживает доступ");
			return;
		}

		try
		{
			Console.WriteLine("===> Получен доступ к Mutex");
			
			for (int i = 0; i < 100; i++)
			{
				File.AppendAllText(fileName, $"{i} ");		
			}
			
			Thread.Sleep(3000);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
		finally
		{
			Console.WriteLine("===> Mutex освобожден");
			mutex.ReleaseMutex();
		}
	}

	public static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(2);
	public static Semaphore semaphore = new Semaphore(2,5);
	public static void DemoSemaphore()
	{
		for (int i = 0; i < 10; i++)
		{
			new Thread(MethodWithSemaphore).Start(i + 1);
		}
	}

	private static void MethodWithSemaphore(object id)
	{
		Console.WriteLine($"===> {id} Ожидает чтобы войти в клуб семафора");
		semaphoreSlim.WaitAsync();
		
		Console.WriteLine($"===> {id} вошел в клуб семафора");
		Thread.Sleep(2000);
		Console.WriteLine($"===> {id} вышел из клуба семафора");
	}


	public static void AutoResetEventDemo()
	{
		new Thread(WaitEvent).Start();
		new Thread(WaitEvent).Start();

		_event.Set();
		
		Thread.Sleep(3000);

		_event.Set();
		
	}

	public static AutoResetEvent _event = new AutoResetEvent(false);


	public static void WaitEvent()
	{
		Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}");
		_event.WaitOne();
		Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}");
	}
	
	//SpinLock

	private static SpinLock _spinLock = new SpinLock();

	public static void DemoSpinLock()
	{
		new Thread(UseSpinLock).Start();
		new Thread(UseSpinLock).Start();
	}

	public static void UseSpinLock(object state)
	{
		bool lockTaken = false;
		_spinLock.Enter(ref lockTaken);
		Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}");
		_spinLock.Exit();
	}

	private static Barrier barrier = new Barrier(2, x =>
	{
		Console.WriteLine("Все потоки завершили");
	});
	
	public static void DemoBarierLock()
	{
		new Thread(UseBarier).Start();
		new Thread(UseBarier).Start();
		
	}

	private static void UseBarier(object? obj)
	{
		
		Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}");
		Thread.Sleep(1000);
		barrier.SignalAndWait();
	}

	public static volatile bool stopSignal = false;
	
	public static void UseVolatile()
	{
		Thread w = new Thread(WorkerMethod);
		w.Start();
		Console.WriteLine("Введите символ");
		Console.ReadKey();
		stopSignal = true;

		w.Join();
		Console.WriteLine("Рабочий поток завершен");

	}

	private static void WorkerMethod(object? obj)
	{
		while (!stopSignal)
		{
			Console.WriteLine($"Поток выполняет работу {Thread.CurrentThread.ManagedThreadId}");
			Thread.Sleep(500);
		}

		Console.WriteLine("Рабочий поток получил флаг что нужно закончить работу");
	}
}

public class Employee
{
	public string Name { get; set; }
	public string Company { get; set; }
}