namespace Payroll.Models
{
    public class EmployeeModel
    {
        public string Department { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public double HourValue { get; set; }
        public DateTime Date { get; set; }
        public string Entry { get; set; }
        public string Exit { get; set; }
        public string Lunch { get; set; }

        public EmployeeModel(int code, string name, double hourValue, DateTime date, string entry, string exit, string lunch)
        {
            Code = code;
            Name = name;
            HourValue = hourValue;
            Date = date;
            Entry = entry;
            Exit = exit;
            Lunch = lunch;
        }

        public bool ValidateTimeWorked()
        {
            return Convert.ToDateTime(Exit).Subtract(Convert.ToDateTime(Entry)).Hours >= 9;
        }

        public int GetOverTime()
        {
            if (ItsWeekend())
                return Convert.ToDateTime(Exit).Subtract(Convert.ToDateTime(Entry)).Hours - 1;

            if (Convert.ToDateTime(Exit).Subtract(Convert.ToDateTime(Entry)).Hours > 9)
                return Convert.ToDateTime(Exit).Subtract(Convert.ToDateTime(Entry)).Hours - 9;

            return 0;
        }

        public int GetHoursNotWorked()
        {
            if (int.Parse(Exit) - int.Parse(Entry) < 9)
                return (int.Parse(Exit) - int.Parse(Entry)) - 9;

            return 0;
        }

        public bool ItsWeekend()
        {
            return Date.DayOfWeek == DayOfWeek.Saturday || Date.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}
