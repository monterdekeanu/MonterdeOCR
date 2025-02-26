using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MonterdeOCR.Helper
{
    public class PlateNumberValidator
    {
        public static bool IsValid(string plateNumber)
        {
            if (string.IsNullOrWhiteSpace(plateNumber))
                return false;
            // Normalize input: trim and convert to upper-case for uniformity
            plateNumber = plateNumber.Trim().ToUpperInvariant();

            // Define regex patterns for each vehicle type
            string fourWheelPattern = @"^[A-Z]{3}[-\s]?\d{4}$";   // Cars, SUVs, etc.
            string motorcyclePattern = @"^[A-Z]{2}[-\s]?\d{5}$";  // Motorcycles

            // Check if input matches either pattern
            bool matchFourWheel = Regex.IsMatch(plateNumber, fourWheelPattern);
            bool matchMotorcycle = Regex.IsMatch(plateNumber, motorcyclePattern);

            return matchFourWheel || matchMotorcycle;
        }
    }
}
