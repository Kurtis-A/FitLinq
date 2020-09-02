using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace FitLinq
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class win_Home : Window
    {
        #region Global Definitions
        public int TotalSteps { get; set; }
        public double TotalMiles { get; set; }
        public int TotalKcals { get; set; }
        public int CalorieTarget { get; set; }

        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
        #endregion

        public win_Home()
        {
            InitializeComponent();
            Load();
        }

        #region Events
        /// <summary>
        /// Allows user to drag anywhere on the window to move it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) => DragMove();

        /// <summary>
        /// Opens add new entry dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddNew_Click(object sender, RoutedEventArgs e)
        {
            dialog_Addnew.IsOpen = true;
        }

        /// <summary>
        /// Validation to ensure user only inputs numbers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_InputSteps_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = !IsTextAllowed(e.Text);

        private void btn_Accept_Click(object sender, RoutedEventArgs e)
        {
            CalculateActivityCalorie();
        }

        private void btn_ProfileAccept_Click(object sender, RoutedEventArgs e)
        {
            CalorieTarget = int.Parse(txt_TargetCal.Text);
            btn_ProfileAccept.Visibility = Visibility.Hidden;
            btn_ProfileReject.Visibility = Visibility.Hidden;
            txt_TargetCal.IsEnabled = false;

            TargetProgress();
        }

        private void btn_ProfileReject_Click(object sender, RoutedEventArgs e)
        {
            txt_TargetCal.Text = CalorieTarget.ToString();
            btn_ProfileAccept.Visibility = Visibility.Hidden;
            btn_ProfileReject.Visibility = Visibility.Hidden;
            txt_TargetCal.IsEnabled = false;
        }

        private void btn_EditProfile_Click(object sender, RoutedEventArgs e)
        {
            btn_ProfileAccept.Visibility = Visibility.Visible;
            btn_ProfileReject.Visibility = Visibility.Visible;
            txt_TargetCal.IsEnabled = true;
            CalorieTarget = int.Parse(txt_TargetCal.Text);
        }

        private void win_Close_Click(object sender, RoutedEventArgs e) => Close();

        private void win_Maximise_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void win_Minimise_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Calls required methods for correct start up and data load
        /// Data insertion on Load up
        /// </summary>
        private void Load()
        {
            try
            {
                //Set Today's Date for UI element
                txb_Date.Text = DateTime.Today.ToString("D");
                //==

                //Load Main Data set
                bool LoadDataSuccessful = DataLinq.LoadData();
                if (!LoadDataSuccessful)
                {
                    MessageBox.Show("Something went wrong while trying to load the Main Data", "Critical Error");
                }
                lst_StepCount.ItemsSource = DataLinq.items;
                //==

                //Load MET Data set
                bool LoadMETSuccessful = DataLinq.LoadMET();
                if (!LoadMETSuccessful)
                {
                    MessageBox.Show("Something went wrong when trying to load the MET Data", "Critical Error");
                }
                //==

                //Load Profile Data
                bool LoadProfileSuccessful = DataLinq.LoadProfile();
                if (!LoadProfileSuccessful)
                {
                    MessageBox.Show("Something went wrong when trying to load the Profile Data", "Critical Error");
                }
                else
                {
                    txt_Weight.Text = Profile.weight.ToString();
                    txt_ftHeight.Text = Profile.fHeight.ToString();
                    txt_inHeight.Text = Profile.iHeight.ToString();
                    txt_TargetCal.Text = Profile.Target.ToString();
                }
                //==

                //[TEMP LOAD CODE]
                //Set Calorie Target
                //Set Period Data to first item (INOP)
                //txt_TargetCal.Text = "840";
                cmb_ShowPeriod.SelectedIndex = 0;
                //==

                //Configures Main List with Group View Rules (Ordering)
                ConfigureList("Load");
                //==

                //Call Calculation on Display item totals
                //Call Target Progress to Show Progress state (UI)
                CalculateTotals();
                TargetProgress();
                //==
            }
            catch (Exception ex)
            {
                //[DO WORK]
                //Do something usefull
            }

        }

        /// <summary>
        /// Is called by the preview text event
        /// </summary>
        /// <param name="text"></param>
        /// <returns>Allowed / Disallowed Text Input</returns>
        private static bool IsTextAllowed(string text) => !_regex.IsMatch(text);

        /// <summary>
        /// Loads itemsource for main list data with view group rules
        /// </summary>
        /// <param name="Configuration"></param>
        private void ConfigureList(string Configuration)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lst_StepCount.ItemsSource);
            switch (Configuration)
            {
                case "Refresh":
                    view.Refresh();
                    break;
                case "Load":

                    PropertyGroupDescription groupDescription = new PropertyGroupDescription("Date");
                    view.GroupDescriptions.Add(groupDescription);
                    view.SortDescriptions.Add(new System.ComponentModel.SortDescription("Date", System.ComponentModel.ListSortDirection.Descending));
                    break;
            }

        }

        /// <summary>
        /// Calculates and shows Progress based on current data input in the form of a radial progress bar
        /// </summary>
        private void TargetProgress()
        {
            int TargetCal = int.Parse(txt_TargetCal.Text);
            string PeriodType = cmb_ShowPeriod.Text;

            int PeriodMultiplier = 1;
            switch (PeriodType)
            {
                case "Weekly":
                    PeriodMultiplier = 7;
                    break;

                case "Monthly":
                    PeriodMultiplier = 30;
                    break;

                case "Yearly":
                    PeriodMultiplier = 365;
                    break;

                case "LifeTime":
                    break;
            }

            int PeriodTarget = TargetCal * PeriodMultiplier;

            //Progress / Goal
            //Multiply by 100

            int Progress = (TotalKcals * 100 / PeriodTarget);

            if (Progress > 100)
            {
                prog_TgtProgress.Value = 100;
            }
            else if (Progress > 1)
            {
                prog_TgtProgress.Value = Progress;
            }
            else if (Progress <= 1)
            {
                prog_TgtProgress.Value = 1;
            }

            lbl_progressNumber.Content = Progress + "%";
            lbl_progressPeriod.Content = PeriodType + " Target: " + PeriodTarget.ToString();
        }

        /// <summary>
        /// Calculates totals from currently displayed data in MainData List
        /// </summary>
        private void CalculateTotals()
        {
            for (int i = 0; i < DataLinq.items.Count; i++)
            {
                foreach (var step in DataLinq.items)
                {
                    TotalSteps += step.Steps;
                }

                foreach (var mile in DataLinq.items)
                {
                    TotalMiles += mile.Miles;
                }

                foreach (var cal in DataLinq.items)
                {
                    TotalKcals += cal.Calories;
                }
            }

            lbl_TotalSteps.Content = TotalSteps;
            lbl_TotalMiles.Content = TotalMiles;
            lbl_TotalCal.Content = TotalKcals;
        }

        /// <summary>
        /// Calls Calculator to calculate values
        /// </summary>
        private void CalculateActivityCalorie()
        {
            int steps = int.Parse(txt_InputSteps.Text);
            double miles = steps / 2000;

            int Calorie = CBPM.CalculateCBT(5.0,86,60);
            //int Calorie = 0;

            if (DataLinq.SaveData(DateTime.Now.ToString("D"), steps.ToString(), miles.ToString(), Calorie.ToString()))
            {
                DataLinq.items.Add(new Data() { Date = DateTime.Now.ToString("D"), Steps = steps, Miles = miles, Calories = Calorie});
                dialog_Addnew.IsOpen = false;
                ConfigureList("Refresh");
                CalculateTotals();
                TargetProgress();
            }
        }
        #endregion
    }
}
