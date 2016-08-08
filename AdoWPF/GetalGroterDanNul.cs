using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OefeningenADO
{
    public class GetalGroterDanNul : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            decimal getal;
            NumberStyles style = NumberStyles.Currency;
            if (value == null || value.ToString() == string.Empty)
            {
                return new ValidationResult(false, "Getal moet ingevuld zijn");
            }
            if (!decimal.TryParse(value.ToString(), style, cultureInfo, out getal))
            {
                return new ValidationResult(false, "Waarde moet een getal zijn");
            }
            if (getal <= 0)
            {
                return new ValidationResult(false, "Getal moet groter zijn dan nul");
            }
            return ValidationResult.ValidResult;
        }
    }
}
