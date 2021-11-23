using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;

using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BaseForPTMK
{
    public class UserRepository
    {

        private const string createUsersTableQuery =
                   "DROP TABLE Users \n" +
                   "CREATE TABLE Users" +
                    "(" +
                    "id INT NOT NULL PRIMARY KEY IDENTITY, " +
                    "nameLastNameePatronymic NVARCHAR(200), " +
                    "dateOfBirth DATETIME NULL, " +
                    "gender NVARCHAR(10) NULL  " +
                    ")";

        private const string createNameAndGenderIndexQuery =
                    "IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'NameAndGender' AND object_id = OBJECT_ID('Users'))" +
                    "       CREATE INDEX NameAndGender ON Users(nameLastNameePatronymic, gender)";

        private string connectionString;
        public UserRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task DoTask1()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var sqlCom = new SqlCommand(createUsersTableQuery, connection))
                {
                    try
                    {
                        await sqlCom.ExecuteNonQueryAsync();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Создание таблицы завершилось ошибкой {e.Message}");
                    }
                }
                connection.Close();
            }
        }

        public async Task DoTask2(string fullName, DateTime dateOfBirth, string gender)
        {
            using (var context = new PTMKBaseContext())
            {
                var newUser = new User
                {
                    FullName = fullName,
                    DateOfBirth = dateOfBirth,
                    Gender = gender,
                };

                context.Users.Add(newUser);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<string>> DoTask3()
        {
            using (var context = new PTMKBaseContext())
            {
                var currentDate = DateTime.Now;

                var collection = context
                    .Users
                    .Where(u => u.DateOfBirth.HasValue)
                    .DistinctBy(u => $"{u.FullName} {u.DateOfBirth}")
                    .OrderBy(u => u.FullName)
                    .Select(u =>
                    {
                        var age = currentDate.Year - u.DateOfBirth.Value.Year;
                        if (u.DateOfBirth.Value > DateTime.Today.AddYears(-age)) age--;

                        return $"{u.FullName} {u.DateOfBirth} {u.Gender} {age}";
                    });

                return await Task.FromResult(collection.ToList());
            }
        }

        public async Task DoTask4()
        {
            using (var context = new PTMKBaseContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                for (int i = 0; i < 1000000; i++)
                {
                    int yearOffset = i % 30 + 1;

                    context.Users.Add(new User
                    {
                        FullName = GenerateName(),
                        DateOfBirth = DateTime.Now.AddYears(-yearOffset),
                        Gender = i % 2 == 0 ? User.MaleGender : User.FemaleGedner,
                    });

                    if (i % 1000 == 0)
                    {
                        await context.SaveChangesAsync();
                    }
                }

                for (int i = 0; i < 100; i++)
                {
                    int yearOffset = i % 30 + 1;

                    context.Users.Add(new User
                    {
                        FullName = GenerateName('F'),
                        DateOfBirth = DateTime.Now.AddYears(-yearOffset),
                        Gender = User.MaleGender,
                    });
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task DoTask5()
        {
            using (var context = new PTMKBaseContext())
            {
                await context
                     .Users
                     .Where(u => u.FullName.StartsWith("F"))
                     .Where(u => u.Gender == User.MaleGender)
                     .ToListAsync();
            }
        }

        public async Task DoTask6()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var sqlCom = new SqlCommand(createNameAndGenderIndexQuery, connection))
                {
                    try
                    {
                        await sqlCom.ExecuteNonQueryAsync();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Создание индекса завершилось ошибкой {e.Message}");
                    }
                }
                connection.Close();
            }
        }

        private string GenerateName(char? startsWith = null)
        {
            var guidBasedName = Guid.NewGuid().ToString();
            var fullName = startsWith != null ? $"{startsWith.Value}{guidBasedName}" : guidBasedName;
            return Regex.Replace(fullName, @"[\d-]", string.Empty);
        }
    }
}
