using System.Globalization;

namespace CS.Toolkit.Automapper.Contracts.Converters
{
    public class CompoundStringToDoubleConverter : IConverter
    {
        public object Convert(object value)
        {
            if(value is string v)
            {
                return ConvertValue(v);
            }
            return value;
        }

        private double ConvertValue(string value)
        {
            var arr = (value ?? string.Empty).Split();
            if (arr.Length > 0 && double.TryParse(arr[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var res))
            {
                return res;
            }
            return default;
        }
    }
}
