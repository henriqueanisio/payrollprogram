using System.Text.Json.Serialization;

namespace Payroll.Models
{
    public class ResultModel
    {
        [JsonPropertyName("Departamento")]
        public string Department { get; set; }

        [JsonPropertyName("MesVigencia")]
        public string EffectiveMonth { get; set; }

        [JsonPropertyName("AnoVigencia")]
        public int EffectiveYear { get; set; }

        [JsonPropertyName("TotalPagar")]
        public double TotalPay { get; set; }

        [JsonPropertyName("TotalDescontos")]
        public double? TotalDiscounts { get; set; }

        [JsonPropertyName("TotalExtras")]
        public double? TotalExtras { get; set; }

        [JsonPropertyName("Funcionarios")]
        public List<SheetEmployeeModel> Employees { get; set; }
    }
}
