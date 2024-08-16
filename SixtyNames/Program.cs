using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SixtyNames.Data;
using Microsoft.Extensions.Hosting;
using SixtyNames.Migrations;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var config = builder.Build();
var connectionString = config.GetConnectionString("DefaultConnection");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<ApplicationContext>(options =>
            options.UseNpgsql(connectionString));
    })
    .Build();

host.Run();

SqlQueryHandler handler = new SqlQueryHandler(connectionString);

Console.WriteLine($"Что вы хотите сделать? {Environment.NewLine} 1 - Вывести сумму всех заключенных договоров за текущий год. {Environment.NewLine} 2 - Вывести сумму заключенных договоров по каждому контрагенту из России. {Environment.NewLine} 3 - Вывести список e-mail уполномоченных лиц, заключивших договора за последние 30 дней, на сумму больше 40000 {Environment.NewLine} 4 - Изменить статус договора на \"Расторгнут\" для физических лиц, у которых есть действующий договор, и возраст которых старше 60 лет включительно. {Environment.NewLine} 5 - Создать отчет (текстовый файл, например, в формате xml, xlsx, json) содержащий ФИО, e-mail, моб. телефон, дату рождения физ. лиц, у которых есть действующие договора по компаниям, расположенных в городе Москва.");
int variable = 0;
while (true)
{
    if (variable > 0 && variable <= 5)
    try
    {
        variable = int.Parse(Console.ReadLine());
        break;
    }
    catch (FormatException)
    {
        Console.WriteLine("Ошибка: Введите корректное целое число.");
    }
}
if (variable > 0)
{
    switch (variable)
    {
        case 1:
            var result1 = handler.GetDataString("SELECT SUM(\"ContractSum\") AS total_contract_sum FROM \"Contracts\" WHERE EXTRACT(YEAR FROM \"DateOfSigning\") = EXTRACT(YEAR FROM CURRENT_DATE)");
            Console.WriteLine(result1);
            break;
        case 2:
            var result2 = handler.GetDataString("SELECT SUM(c.\"ContractSum\") AS TotalContractSum FROM \"Contracts\" c LEFT JOIN \"PhysicalPersons\" pp ON c.\"PhysicalPersonId\" = pp.\"Id\" LEFT JOIN  \"LegalEntities\" le ON c.\"LegalEntityId\" = le.\"Id\" WHERE (pp.\"Country\" = 'Russia' OR le.\"Country\" = 'Russia') ORDER BY TotalContractSum DESC;");
            Console.WriteLine(result2);
            break;
        case 3:
            var result3 = handler.GetDataString("SELECT pp.\"Email\" FROM \"Contracts\" c JOIN    \"PhysicalPersons\" pp ON c.\"PhysicalPersonId\" = pp.\"Id\" WHERE c.\"ContractSum\" > 40000 AND c.\"DateOfSigning\" >= (CURRENT_DATE - INTERVAL '30 days');");
            Console.WriteLine(result3);
            break;
        case 4:
            var result4 = handler.GetDataString("UPDATE \"Contracts\" c SET \"Status\" = 'Расторгнут' WHERE c.\"Status\" = 'Заключен' AND EXISTS (SELECT 1 FROM \"PhysicalPersons\" pp WHERE pp.\"Id\" = c.\"PhysicalPersonId\" AND EXTRACT(YEAR FROM AGE(CURRENT_DATE, pp.\"BirthDate\")) >= 60);");
            Console.WriteLine(result4);
            break;
        case 5:
            handler.GetDataReader("SELECT pp.\"Name\", pp.\"Surname\", pp.\"MiddleName\", pp.\"Email\", pp.\"PhoneNumber\", pp.\"BirthDate\" FROM \"PhysicalPersons\" pp JOIN \"Contracts\" c ON pp.\"Id\" = c.\"PhysicalPersonId\" JOIN \"LegalEntities\" le ON c.\"LegalEntityId\" = le.\"Id\" WHERE c.\"Status\" = 'Заключен' AND le.\"City\" = 'Москва';");
            break;
    }
}

