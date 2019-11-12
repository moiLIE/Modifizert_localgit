
using System;
using FHT59N3;
using NUnit.Framework;

namespace FHT59N3UnitTest
{
    [TestFixture, RequiresSTA]
    public class UnitTest_TimeCalc
    {
        [SetUp]
        public void TestSetup()
        {
            frmMain main = new frmMain();
            main.tmrStart_Tick(null, null);
            
        }

        /// <summary>
        /// Tests the next filter time.
        /// </summary>
        [Test]
        public void TestNextFilterTime()
        {
            AnalyzeConfig analyzeConfig = new AnalyzeConfig {FilterPeriodHours = 4, AnalyzePeriodHours = 2, StartHourPerDay = 6};
            //Test des Anlaufs (Synchronisierung zur StartHour...
            TestFilterTime(analyzeConfig, "04:00", "06:00");
            TestFilterTime(analyzeConfig, "06:59", "10:00");
            TestFilterTime(analyzeConfig, "09:59", "14:00");
            //Während des Tages
            TestFilterTime(analyzeConfig, "10:00", "14:00");
            TestFilterTime(analyzeConfig, "10:01", "14:00");
            TestFilterTime(analyzeConfig, "11:00", "14:00");
            TestFilterTime(analyzeConfig, "12:00", "14:00");
            TestFilterTime(analyzeConfig, "13:00", "18:00");

            TestFilterTime(analyzeConfig, "22:00", "02:00", 1);

            analyzeConfig = new AnalyzeConfig { FilterPeriodHours = 12, AnalyzePeriodHours = 2, StartHourPerDay = 6 };
            TestFilterTime(analyzeConfig, "07:00", "18:00");

        }

        /// <summary>
        /// Tests the analyze times.
        /// </summary>
        [Test]
        public void TestAnalyzeTimes()
        {
            AnalyzeConfig analyzeConfig = new AnalyzeConfig { FilterPeriodHours = 4, AnalyzePeriodHours = 2, StartHourPerDay = 6 };
            //Test des Anlaufs (Synchronisierung zur StartHour...
            TestAnalyzeTime(analyzeConfig, "05:00", "06:00");
            TestAnalyzeTime(analyzeConfig, "05:30", "08:00");
            TestAnalyzeTime(analyzeConfig, "06:01", "08:00");
            TestAnalyzeTime(analyzeConfig, "08:00", "10:00");
        }



        /// <summary>
        /// Tests the filter time.
        /// </summary>
        /// <param name="analyzeConfig">The analyze configuration.</param>
        /// <param name="currentDateStr">The current date.</param>
        /// <param name="expectedDateStr">The expected date string.</param>
        /// <param name="nextDays">The next days.</param>
        /// <exception cref="AssertionException">Didnt match expected: " + expectedDate + " != " + nextFilterDateByCalc</exception>
        private void TestFilterTime(AnalyzeConfig analyzeConfig, string currentDateStr, string expectedDateStr, int nextDays = 0)
        {
            int filterPeriod = analyzeConfig.FilterPeriodHours;
            int analyzePeriod = analyzeConfig.AnalyzePeriodHours * 60;

            FHT59N3_SystemProperties._MyFHT59N3Par.DayStartTime = analyzeConfig.StartHourPerDay;
            FHT59N3_SystemProperties._MyFHT59N3Par.FilterTimeh = filterPeriod;
            FHT59N3_SystemProperties._MyFHT59N3Par.MeasurementTimemin = analyzePeriod;

            DateTime currentDate = DateTime.Parse(currentDateStr);
            DateTime expectedDate = DateTime.Parse(expectedDateStr);
            expectedDate = expectedDate.AddDays(nextDays);

            FHT59N3_DataFunctions.SYS_SetDerivedWorkParamsFromConfig();
            FHT59N3_ControlFunctions.SYS_SynchronizeNextFilterStepTime(
                currentDate.Hour * 60 + currentDate.Minute, "unit test");
            
            DateTime nextFilterDateByCalc = FHT59N3_SystemProperties._NextFilterStepMinuteDate;

            if (!expectedDate.Equals(nextFilterDateByCalc))
            {
                throw new AssertionException("Didnt match expected: " + expectedDate + " != " + nextFilterDateByCalc);
            }
        }


        /// <summary>
        /// Tests the analyze time.
        /// </summary>
        /// <param name="analyzeConfig">The analyze configuration.</param>
        /// <param name="currentDateStr">The current date string.</param>
        /// <param name="expectedDateStr">The expected date string.</param>
        /// <param name="nextDays">The next days.</param>
        /// <exception cref="AssertionException">Next analyze time didnt match expected: " + expectedDate + " != " + nextAnalyzeDatebyCalc</exception>
        private void TestAnalyzeTime(AnalyzeConfig analyzeConfig, string currentDateStr, string expectedDateStr, int nextDays = 0)
        {
            int filterPeriod = analyzeConfig.FilterPeriodHours;
            int analyzePeriod = analyzeConfig.AnalyzePeriodHours * 60;

            FHT59N3_SystemProperties._MyFHT59N3Par.DayStartTime = analyzeConfig.StartHourPerDay;
            FHT59N3_SystemProperties._MyFHT59N3Par.FilterTimeh = filterPeriod;
            FHT59N3_SystemProperties._MyFHT59N3Par.MeasurementTimemin = analyzePeriod;

            DateTime currentDate = DateTime.Parse(currentDateStr);
            DateTime expectedDate = DateTime.Parse(expectedDateStr);
            expectedDate = expectedDate.AddDays(nextDays);

            FHT59N3_DataFunctions.SYS_SetDerivedWorkParamsFromConfig();
            FHT59N3_ControlFunctions.SYS_SynchronizeNextAnalyzationTime(currentDate.Hour * 60 + currentDate.Minute, "unit test");

            DateTime nextAnalyzeDatebyCalc = FHT59N3_SystemProperties._AnalyzeMinuteDate;

            if (!expectedDate.Equals(nextAnalyzeDatebyCalc))
            {
                throw new AssertionException("Next analyze time didnt match expected: " + expectedDate + " != " + nextAnalyzeDatebyCalc);
            }
        }

    }

    /// <summary>
    /// Configuration of analyzation parameters
    /// </summary>
    public class AnalyzeConfig
    {
        public int FilterPeriodHours;
        public int AnalyzePeriodHours;
        public int StartHourPerDay;
    }
}
