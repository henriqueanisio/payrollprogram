using Payroll.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

Console.WriteLine("Informe o caminho da pasta dos documentos.");
var searchPath = Console.ReadLine();

Console.WriteLine("Informe o caminho onde deseja salvar o resultado");
var pathSave = Console.ReadLine();

var dir = new DirectoryInfo(searchPath);
var resultModelList = new List<ResultModel>();
var timer = new Stopwatch();

Console.WriteLine($"Início da execução");
timer.Start();

var department = "";
var effectiveMonth = "";
var effectiveYear = 0;

foreach (var file in dir.GetFiles("*.*", SearchOption.AllDirectories))
{
    var lines = File.ReadAllLinesAsync(file.FullName, Encoding.UTF8);

    var employeesModel = new List<EmployeeModel>();

    foreach (var line in lines.Result)
    {
        var splitLine = line.Split(";");

        if (!int.TryParse(splitLine[0], out int value))
            continue;

        var employeeModel = CsvToDto(splitLine);
        department = file.Name.Substring(0, file.Name.IndexOf("-"));
        var monthAux = file.Name.Substring(file.Name.IndexOf("-") + 1);
        effectiveMonth = monthAux.Substring(0, monthAux.IndexOf("-"));
        var yearAux = file.Name.Substring(file.Name.IndexOf("-") + 1);
        effectiveYear = int.Parse(monthAux.Substring(monthAux.IndexOf("-") + 1).Replace(".csv", ""));
        employeesModel.Add(employeeModel);
    }

    var processMass = await ProcessMassOfDate(employeesModel,department, effectiveMonth, effectiveYear);

    resultModelList.AddRange(processMass);
}

await SaveDocumentAsync(resultModelList);

timer.Stop();
Console.WriteLine($"Tempo: {timer.Elapsed:m\\:ss\\.fff}");


async Task<List<ResultModel>> ProcessMassOfDate(List<EmployeeModel> employeesModel, string department, string effectiveMonth, int effectiveYear)
{
    var sheetEmployesModelList = new List<SheetEmployeeModel>();
    var resultModelList = new List<ResultModel>();

    var employeesByCode = employeesModel.GroupBy(x => new { x.Code }).ToList();

    Parallel.ForEach(employeesByCode, employees =>
    {
        var sheetEmployeeModel = new SheetEmployeeModel();

        foreach (var employee in employees)
        {

            Console.WriteLine($"Funcionario: {employee.Name}, Date: {employee.Date}, Thread:{Thread.CurrentThread.ManagedThreadId}");

            sheetEmployeeModel.Name = employee.Name;
            sheetEmployeeModel.HourValue = employee.HourValue;

            var weekend = employee.ItsWeekend();
            var overTimeTime = employee.GetOverTime();

            if (weekend)
            {
                sheetEmployeeModel.TotalReceive = sheetEmployeeModel.TotalReceive + (overTimeTime * employee.HourValue);
                sheetEmployeeModel.ExtraDays++;
                sheetEmployeeModel.Overtime = sheetEmployeeModel.Overtime + overTimeTime;
                sheetEmployeeModel.WorkedDays++;
            }
            else
            {
                if (employee.ValidateTimeWorked())
                {
                    sheetEmployeeModel.TotalReceive = sheetEmployeeModel.TotalReceive + (overTimeTime * employee.HourValue) + (8 * employee.HourValue);
                    sheetEmployeeModel.WorkedDays++;
                    sheetEmployeeModel.WorkingDays++;

                    if (overTimeTime > 0)
                        sheetEmployeeModel.Overtime = sheetEmployeeModel.Overtime + overTimeTime;
                }
                else
                {
                    sheetEmployeeModel.TotalReceive = sheetEmployeeModel.TotalReceive - employee.GetHoursNotWorked();
                    sheetEmployeeModel.DebitHours = sheetEmployeeModel.DebitHours + employee.GetHoursNotWorked();
                }
            }
        }

        sheetEmployeeModel.MissingDays = GetWorkingDays(effectiveYear, employeesModel.Select(x => x.Date.Month).First()) - sheetEmployeeModel.WorkingDays;
        if (sheetEmployeeModel.MissingDays > 0)
        {
            sheetEmployeeModel.TotalReceive = sheetEmployeeModel.TotalReceive - (sheetEmployeeModel.MissingDays * 8);
            sheetEmployeeModel.DebitHours = sheetEmployeeModel.DebitHours + (sheetEmployeeModel.MissingDays * 8);
        }


        sheetEmployeeModel.DebitHours = -sheetEmployeeModel.DebitHours;
        sheetEmployeeModel.MissingDays = -sheetEmployeeModel.MissingDays;
        sheetEmployeeModel.TotalReceive = Convert.ToDouble(sheetEmployeeModel.TotalReceive.ToString("F"));
        sheetEmployeeModel.TotalExtras = Convert.ToDouble((sheetEmployeeModel.Overtime * sheetEmployeeModel.HourValue + ((sheetEmployeeModel.ExtraDays * 8) * sheetEmployeeModel.HourValue)).ToString("F"));
        sheetEmployeeModel.TotalDiscounts = -Convert.ToDouble((sheetEmployeeModel.DebitHours * sheetEmployeeModel.HourValue + ((sheetEmployeeModel.MissingDays * 8) * sheetEmployeeModel.HourValue)).ToString("F"));

        sheetEmployesModelList.Add(sheetEmployeeModel);
    });

    var resultModel = new ResultModel();
    resultModel.Department = department;
    resultModel.EffectiveMonth = effectiveMonth;
    resultModel.EffectiveYear = effectiveYear;
    resultModel.TotalPay = Convert.ToDouble(sheetEmployesModelList.Sum(y => y.TotalReceive).ToString("F"));
    resultModel.TotalDiscounts = sheetEmployesModelList.Sum(y => y.TotalDiscounts);
    resultModel.TotalExtras = sheetEmployesModelList.Sum(y => y.TotalExtras);
    resultModel.Employees = sheetEmployesModelList;

    resultModelList.Add(resultModel);


    return resultModelList;

}

EmployeeModel CsvToDto(string[] splitLine)
{
    return new EmployeeModel(int.Parse(splitLine[0]),
        splitLine[1],
        Convert.ToDouble(splitLine[2].Replace("R$", "").Replace(" ", "")),
        Convert.ToDateTime(splitLine[3]),
        Convert.ToDateTime(splitLine[4]).ToString("HH:mm:ss"),
        Convert.ToDateTime(splitLine[5]).ToString("HH:mm:ss"),
        splitLine[6]);
}

int GetWorkingDays(int year, int month)
{
    var beginDate = new DateTime(year, month, 1);
    var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
    var dateAux = beginDate;
    var workingDays = 0;

    while(dateAux < endDate)
    {
        if(dateAux.DayOfWeek == DayOfWeek.Saturday || dateAux.DayOfWeek == DayOfWeek.Sunday)
        {
            dateAux = dateAux.AddDays(1);
            continue;
        }
        dateAux = dateAux.AddDays(1);
        workingDays++;
    }

    return workingDays;
}

async Task SaveDocumentAsync(List<ResultModel> result)
{
    JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
    jsonOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

    string fileName = $"{pathSave}\\{DateTime.Now.Ticks}.json";
    using FileStream createStream = File.Create(fileName);
    await JsonSerializer.SerializeAsync(createStream, result, jsonOptions);
    await createStream.DisposeAsync();
}