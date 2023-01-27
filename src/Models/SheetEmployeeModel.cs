using System.Text.Json.Serialization;

namespace Payroll.Models
{
    public class SheetEmployeeModel
    {
        [JsonPropertyName("Código")]
        public int Code { get; set; }

        [JsonPropertyName("Nome")]
        public string Name { get; set; }

        [JsonPropertyName("TotalReceber")]
        public double TotalReceive { get; set; }

        [JsonPropertyName("HorasExtras")]
        public int Overtime { get; set; }

        [JsonPropertyName("HorasDebito")]
        public int DebitHours { get; set; }

        [JsonPropertyName("DiasFalta")]
        public int MissingDays { get; set; }

        [JsonPropertyName("DiasExtras")]
        public int ExtraDays { get; set; }

        [JsonPropertyName("DiasTrabalhados")]
        public int WorkedDays { get; set; }

        [JsonIgnore]
        public int WorkingDays { get; set; }

        [JsonIgnore]
        public double? TotalDiscounts { get; set; }

        [JsonIgnore]
        public double? TotalExtras { get; set; }

        [JsonIgnore]
        public double HourValue { get; set; }

    }
}
