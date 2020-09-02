using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace FitLinq
{
    public static class DataLinq
    {
        #region MainData
        public static List<Data> items = new List<Data>();

        /// <summary>
        /// Loads all data into List 'Items' for using in app
        /// </summary>
        /// <returns></returns>
        public static bool LoadData()
        {
            //Always use root path + data folder for Main Data
            string datafilePath = @"../../Data/MainData.txt";

            //Initialise Streamreader to open datafile
            using (StreamReader sr = new StreamReader(datafilePath))
            {
                try
                {
                    //Reads as long as there is data to be read
                    //Set delimiter
                    //Add values from delimited string into new list item
                    //Return True
                    string line = null;
                    while (null != (line = sr.ReadLine()))
                    {
                        var data = line.Split(',');
                        items.Add(new Data() { Date = data[0].ToString(), Steps = int.Parse(data[1].ToString()), Miles = double.Parse(data[2].ToString()), Calories = int.Parse(data[3].ToString()) });
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }

        }

        /// <summary>
        /// Writes to MainData Text File
        /// </summary>
        /// <param name="Date"></param>
        /// <param name="Steps"></param>
        /// <param name="Miles"></param>
        /// <param name="Calories"></param>
        /// <returns></returns>
        public static bool SaveData(string Date, string Steps, string Miles, string Calories)
        {
            try
            {
                File.AppendAllText(@"../../Data/MainData.txt", Date + "," + Steps + "," + Miles + "," + Calories + Environment.NewLine);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region MET Data
        public static List<MET> MetData = new List<MET>();

        /// <summary>
        /// Loads MET Data into METData List
        /// </summary>
        /// <returns></returns>
        public static bool LoadMET()
        {
            string METDataFile = @"../../Data/MET.txt";

            using (StreamReader sr = new StreamReader(METDataFile))
            {
                try
                {
                    //Reads as long as there is data to be read
                    //Set delimiter
                    //Add values from delimited string into new list item
                    //Return True
                    string line = null;
                    while (null != (line = sr.ReadLine()))
                    {
                        var data = line.Split(',');
                        MetData.Add(new MET() { Category = data[0].ToString(), Description = data[1].ToString(), METvalue = double.Parse(data[2].ToString()) });
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"LoadMET - DataLinq");
                    return false;
                }
            }
        }
        #endregion

        public static bool LoadProfile()
        {
            string ProfileDataFile = @"../../Data/Profile.txt";
            var lines = File.ReadAllLines(ProfileDataFile);
            foreach (var line in lines)
            {
                if (line.Contains("weight"))
                {
                    var text = line.Split('=');
                    Profile.weight = int.Parse(text[1].ToString());
                }

                if (line.Contains("height ft"))
                {
                    var text = line.Split('=');
                    Profile.fHeight = int.Parse(text[1].ToString());
                }

                if (line.Contains("height in"))
                {
                    var text = line.Split('=');
                    Profile.iHeight = int.Parse(text[1].ToString());
                }

                if (line.Contains("target"))
                {
                    var text = line.Split('=');
                    Profile.Target = int.Parse(text[1].ToString());
                }
            }
            return true;

            //using (StreamReader sr = new StreamReader(ProfileDataFile))
            //{
            //    try
            //    {
            //        //Reads as long as there is data to be read
            //        //Set delimiter
            //        //Add values from delimited string into new list item
            //        //Return True
            //        string line = null;
            //        while (null != (line = sr.ReadLine()))
            //        {
            //            var data = line.Split(',');
            //            MetData.Add(new MET() { Category = data[0].ToString(), Description = data[1].ToString(), METvalue = double.Parse(data[2].ToString()) });
            //        }
            //        return true;
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message, "LoadMET - DataLinq");
            //        return false;
            //    }
            //}
        }
    }

    public class Data
    {

        public int Steps { get; set; }

        public double Miles { get; set; }

        public string Date { get; set; }

        public int Calories { get; set; }
    }

    public class MET
    {
        public string Category { get; set; }

        public string Description { get; set; }

        public double METvalue { get; set; }
    }

    public static class Profile
    {
        public static int weight { get; set; }

        public static int fHeight { get; set; }

        public static int iHeight { get; set; }

        public static int Target { get; set; }
    }
}
