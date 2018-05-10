/****************************** Module Header ******************************\
Module Name:    Program [static]
Project:        SCADAToMSSQL
Summary:        The entry point of the windows service.
Author[s]:      Ryan Cooper
Email[s]:       rcooper@edwardsaquifer.org
\***************************************************************************/

using System.ServiceProcess;

namespace SCADAToMSSQL
{
    internal static class Program
    {
        #region Private Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new ScadaToSql()
            };
            ServiceBase.Run(servicesToRun);
        }

        #endregion Private Methods
    }
}