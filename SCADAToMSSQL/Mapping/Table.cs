/****************************** Module Header ******************************\
Module Name:    Table
Project:        SCADAToMSSQL
Summary:        Defines an MSSQL table and its respective ClearSCADA 
                stations.
Author[s]:      Ryan Cooper
Email[s]:       rcooper@edwardsaquifer.org
\***************************************************************************/

using System.Collections.Generic;

namespace SCADAToMSSQL.Mapping
{
    public class Table
    {
        #region Internal Constructors

        internal Table(string name, List<Station> stations)
        {
            Name = name;
            Stations = stations;
            Tables.TableList.Add(this);
        }

        #endregion Internal Constructors

        #region Public Properties

        /// <summary>
        /// The name of the MSSQL table
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The list of ClearSCADA stations that map to this table
        /// </summary>
        public List<Station> Stations { get; }

        #endregion Public Properties
    }
}