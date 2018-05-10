/****************************** Module Header ******************************\
Module Name:    ScadaToSql inherits ServiceBase [slealed] [partial]
Project:        SCADAToMSSQL
Summary:        Control point of the windows service
Author[s]:      Ryan Cooper
Email[s]:       rcooper@edwardsaquifer.org
\***************************************************************************/

using SCADAToMSSQL.Mapping;
using System;
using System.ComponentModel;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace SCADAToMSSQL
{
    public sealed partial class ScadaToSql : ServiceBase
    {
        #region Private Fields

        private const string LONG_LOG = @"C:\SCADAToSQL\FULL_LOG.TXT";
        private const string SESSION_LOG = @"C:\SCADAToSQL\SESSION_LOG.TXT";
        private readonly System.Timers.Timer timer;

        #endregion Private Fields

        #region Public Constructors

        /// <inheritdoc />
        /// <summary>
        /// Default constructor
        /// </summary>
        public ScadaToSql()
        {
            InitializeComponent();
            timer = new System.Timers.Timer();

            ((ISupportInitialize)EventLog).BeginInit();
            if (!EventLog.SourceExists(ServiceName)) EventLog.CreateEventSource(ServiceName, @"Application");
            ((ISupportInitialize)EventLog).EndInit();

            EventLog.Source = ServiceName;
            EventLog.Log = @"Application";
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Used to artificially start the service via another project
        /// </summary>
        public void Start()
        {
            OnStart(null);
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry(@"Started.");
            timer.AutoReset = true;
            timer.Interval = (DateTime.Today.AddDays(1).AddHours(1) - DateTime.Now).TotalMilliseconds;
            timer.Start();

            timer.Elapsed += Timer_Elapsed;

            new Thread(() => { Timer_Elapsed(this, null); }).Start();//run on start
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry(@"Stopped");
        }

        #endregion Protected Methods

        #region Private Methods

        private void synchronize(Table tbl, OdbcConnection connection)
        {
            EventLog.WriteEntry($@"Started {tbl.Name} sync.");
            var str = string.Empty;
            var i = 0;

            foreach (var station in tbl.Stations)
                try
                {
                    Console.WriteLine($@"Running {station.Name}");
                    var dbCommand = connection.CreateCommand();
                    dbCommand.CommandText = station.GetValuesSql();
                    var dbReader = dbCommand.ExecuteReader();

                    var rainDbConnection = new SqlConnection(@"Server=localhost; Database=RainfallDB; Integrated Security=true;");//must run service as user with access to db
                    rainDbConnection.Open();
                    var cmd = rainDbConnection.CreateCommand();

                    var lastTimeStampFromSql = DateTime.MinValue;

                    try
                    {
                        cmd.CommandText = $@"SELECT TOP 1 [timestamp] FROM [{tbl.Name}] WHERE station_id = '{station.Name}' ORDER BY [timestamp] DESC";
                        var reader = cmd.ExecuteReader();
                        reader.Read();
                        lastTimeStampFromSql = DateTime.Parse(reader[@"timestamp"].ToString()).ToUniversalTime();
                        rainDbConnection.Close();
                        rainDbConnection.Dispose();
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(LONG_LOG, $@"No records found of {tbl.Name} starting from scratch. {ex}");
                        File.AppendAllText(SESSION_LOG, $@"No records found of {tbl.Name}, starting from scratch. {ex}");
                        EventLog.WriteEntry(tbl.Name + $@"No records found of {tbl.Name} starting from scratch. {ex}");
                    }

                    while (dbReader.Read())
                    {
                        var lastTimeStampFromScada = DateTime.Parse(dbReader[@"Time"].ToString()).ToUniversalTime();

                        if (lastTimeStampFromScada <= lastTimeStampFromSql)
                            continue;

                        var value = dbReader[station.Field];

                        str += $@"Timestmap: {lastTimeStampFromScada} Station: {station.Name} Value: {value}";

                        rainDbConnection = new SqlConnection(@"Server=localhost; Database=RainfallDB; Integrated Security=true;");
                        rainDbConnection.Open();

                        cmd = rainDbConnection.CreateCommand();
                        cmd.CommandText = $@"INSERT INTO [{tbl.Name}](station_id, timestamp, value) VALUES('{station.Name}', '{lastTimeStampFromScada}', '{value}')";
                        cmd.ExecuteNonQuery();

                        rainDbConnection.Close();

                        i++;
                    }
                }
                catch (Exception ex)
                {
                    File.AppendAllText(LONG_LOG, $@"{tbl.Name} sync error: {ex}");
                    File.AppendAllText(SESSION_LOG, $@"{tbl.Name} sync error: {ex}");
                    EventLog.WriteEntry($@"{tbl.Name} sync error: {ex}");
                    //Console.WriteLine($@"{tbl.Name} sync error: {ex}");
                }

            File.AppendAllText(LONG_LOG, str);
            File.AppendAllText(SESSION_LOG, str);
            EventLog.WriteEntry($@"{tbl.Name} sync complete, {i} rows affected.");
            //Console.WriteLine($@"{tbl.Name} sync complete, {i} rows affected.");
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (File.Exists(SESSION_LOG))
                File.Delete(SESSION_LOG);

            Emailer.Send(@"ClearSCADA To MSSQL", @"Database sync in progress. " + DateTime.UtcNow);

            using (var connection = new OdbcConnection(@"Driver=ClearSCADA Driver;Location=ClearScada;"))
            {
                connection.Open();

                synchronize(Tables.SixMinute, connection);
                synchronize(Tables.EighteenMinute, connection);
                synchronize(Tables.OneHourAccumulation, connection);
                synchronize(Tables.SixHourAccumulation, connection);
                synchronize(Tables.DailyAccumulation, connection);
            }

            timer.Interval = (DateTime.Today.AddDays(1).AddHours(1) - DateTime.Now).TotalMilliseconds;
            EventLog.WriteEntry($@"Next run set for {DateTime.Today.AddDays(1).AddHours(1)}");

            Emailer.Send(@"ClearSCADA To MSSQL", $@"Database sync finished. {DateTime.UtcNow} Next Run: {DateTime.Today.AddDays(1).AddHours(1)}", SESSION_LOG);
        }

        #endregion Private Methods
    }
}