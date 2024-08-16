using Npgsql;
using SixtyNames.Entities;
using Newtonsoft.Json;

namespace SixtyNames.Data
{
    public class ReportGenerator
    {
        public void GenerateReport(NpgsqlDataReader dataReader)
        {
            if (dataReader.HasRows)
            {
                List<PhysicalPerson> physicalPersons = new List<PhysicalPerson>();

                while (dataReader.Read())
                {
                    physicalPersons.Add(new PhysicalPerson
                    {
                        Name = dataReader.GetString(0),
                        Surname = dataReader.GetString(1),
                        MiddleName = dataReader.GetString(2),
                        Email = dataReader.GetString(3),
                        PhoneNumber = dataReader.GetString(4),
                        BirthDate = dataReader.GetDateTime(5)
                    });
                }

                string json = JsonConvert.SerializeObject(physicalPersons, Formatting.Indented);
                System.IO.File.WriteAllText("report.json", json);
            }

            dataReader.Dispose();
        }
    }
}
