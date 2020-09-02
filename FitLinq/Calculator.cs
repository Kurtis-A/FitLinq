using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitLinq
{
    /// <summary>
    /// Calories Burned Per Minute
    /// </summary>
    public static class CBPM
    {
        //[FORMULA]
        //(MET x weight (KG) x 3.5) / 200

        public static double CalculateCBPM(double METvalue, double Weight)
        {
            //OPM = Assumption of 3.5ml of Oxygen brunt per minute
            const double OPM = 3.5;
            double calPM1 = (METvalue * Weight * OPM);
            double calPM2 = calPM1 / 200;

            return calPM2;
        }

        public static int CalculateCBT(double METvalue, double Weight, int MinDuration)
        {
            //Calculate Calorie per minute
            double CBPM = CalculateCBPM(METvalue, Weight);

            //Calculate Calories burnt total
            //CBPM * Duration(Minutes)
            double CBT = (CBPM * MinDuration);
            int returnValue = Convert.ToInt32(CBT);

            return returnValue;
        }
    }

    
}
