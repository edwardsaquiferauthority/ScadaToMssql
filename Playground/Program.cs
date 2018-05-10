namespace Playground
{
    internal class Program
    {
        #region Private Methods

        // ReSharper disable once InconsistentNaming
        private static void Main()
        {
            new SCADAToMSSQL.ScadaToSql().Start();//artificially start the service for debugging purposes
        }

        #endregion Private Methods
    }
}