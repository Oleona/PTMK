using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BaseForPTMK
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            UserRepository userRepository = new UserRepository(connectionString);

            if (!int.TryParse(args[0], out int taskNumber) || taskNumber < 1 || taskNumber > 6)
            {
                throw new ArgumentException($"Некорректное значение задания {taskNumber}");
            }

            Console.WriteLine($"Начинаем выполнение задания {taskNumber}");

            switch (taskNumber)
            {
                case 1:
                    await userRepository.DoTask1();
                    break;

                case 2:
                    if (args.Length != 4)
                    {
                        throw new ArgumentException(
                            "Некорректное количество параметров командной строки в случае, когда первый параметр равен 2");
                    }

                    if (!DateTime.TryParse(args[2], out DateTime dateOfBirth))
                    {
                        throw new ArgumentException($"Некорректный формат даты {args[2]}");
                    }

                    await userRepository.DoTask2(
                        fullName: args[1],
                        dateOfBirth: dateOfBirth,
                        gender: args[3]
                        );
                    break;

                case 3:
                    var requestedCollection = await userRepository.DoTask3();
                    foreach (var item in requestedCollection)
                    {
                        Console.WriteLine(item);
                    }
                    break;

                case 4:
                    await userRepository.DoTask4();
                    break;

                case 5:
                    var timer = new Stopwatch();
                    timer.Start();

                    await userRepository.DoTask5();

                    timer.Stop();          
                    Console.WriteLine(timer.Elapsed);
                    break;

                case 6:
                    await userRepository.DoTask6();
                    timer = new Stopwatch();
                    timer.Start();

                    await userRepository.DoTask5();

                    timer.Stop();
                    Console.WriteLine(timer.Elapsed);
                    break;
                default:
                    throw new ArgumentException($"Некорректное значение задания {taskNumber}");
            }

            Console.WriteLine($"Выполнение задания {taskNumber} успешно завершено");
        }
    }
}
