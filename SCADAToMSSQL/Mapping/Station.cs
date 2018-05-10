/****************************** Module Header ******************************\
Module Name:    Station
Project:        SCADAToMSSQL
Summary:        Defines a ClearSCADA station and where its data is located
                within ClearSCADA
Author[s]:      Ryan Cooper
Email[s]:       rcooper@edwardsaquifer.org
\***************************************************************************/

namespace SCADAToMSSQL.Mapping
{
    public class Station
    {
        #region Public Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">The name of the station as it appears in ClearSCADA</param>
        /// <param name="field">The name of the target field as it appears in ClearSCADA</param>
        /// <param name="table">The name of the parent table as it appears in ClearSCADA</param>
        public Station(string name, string field, string table)
        {
            Name = name;
            Field = field;
            Table = table;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// The name of the station as it appears in ClearSCADA
        /// </summary>
        public string Field { get; }

        /// <summary>
        /// The name of the target field as it appears in ClearSCADA
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The name of the parent table as it appears in ClearSCADA
        /// </summary>
        public string Table { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Returns an ClearSCADA query te get all the data >0
        /// </summary>
        /// <returns></returns>
        public string GetValuesSql()
        {
            return "SELECT \"Time\", \"" + Field + "\" FROM " + Table + " WHERE \"" + Field + "\" > 0";
        }

        #endregion Public Methods
    }
}