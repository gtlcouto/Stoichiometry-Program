using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Text.RegularExpressions;

namespace StoichiometryLib
{
    public interface IMolecule
    {
        string Formula { set; get; }
        double Weight { get; }
        bool Normalize();
        bool Save();
        string[] FormulasList { get; }
    }

    public class Molecule : IMolecule
    {

        private DataSet _ds;
        private DataSet _ds2;
        private OleDbDataAdapter _adElements, _adMolecules;

        private string _formula;

        public string Formula
        {
            get { return _formula; }
            set 
            {
                _formula = value;
            }
        }
        

        public double Weight
        {
            get
            {
                Dictionary<string, int> elementCount = new Dictionary<string, int>();
                string elementRegex = "([A-Z][a-z]*[a-z]*)([0-9]*)";
                string validateRegex = "^(" + elementRegex + ")+$";

                if (!Regex.IsMatch(Formula, validateRegex))
                    throw new FormatException("Input string was in an incorrect format.");

                foreach (Match match in Regex.Matches(Formula, elementRegex))
                {

                    string name = match.Groups[1].Value;

                    int count = match.Groups[2].Value != "" ? int.Parse(match.Groups[2].Value) : 1;
                    if (elementCount.ContainsKey(name))
                    {
                        elementCount[name]++;
                    }
                    else
                    {
                        elementCount.Add(name, count);
                    }
                }

                OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Stoichiometry.mdb");

                // Create data adapter for Elements table
                // This table will be used in read-only mode 
                _adElements = new OleDbDataAdapter();
                _adElements.SelectCommand = new OleDbCommand("SELECT * FROM Elements", conn);

                // Create data adapter for Molecules table
                // This table will be used in both read and write modes
                _adMolecules = new OleDbDataAdapter();
                _adMolecules.SelectCommand = new OleDbCommand("SELECT Formula FROM Molecules", conn);
                _adMolecules.InsertCommand = new OleDbCommand("INSERT INTO Molecules (Formula) VALUES(@Formula)", conn);
                // Create a parameter variable for the insert command (note that this code could easily
                // be modified to initialize the table's MolecularWeight column as well)
                _adMolecules.InsertCommand.Parameters.Add("@Formula", OleDbType.VarChar, -1, "Formula");

                // Create and fill the data set and close the connection 
                _ds = new DataSet();
                conn.Open();
                _adElements.Fill(_ds, "Elements");
                conn.Close();

                //dictionary to hold symbo and weight
                Dictionary<string, double> snw= new Dictionary<string,double>();

                // populate the dictionary
                foreach (DataRow row in _ds.Tables["Elements"].Rows)
                {
                    snw.Add(row.Field<string>("Symbol"), row.Field<double>("AtomicWeight"));
                }

                //calculate weight
                double weight = 0;
                foreach (KeyValuePair<string, int> pair in elementCount)
                {
                    if (!snw.ContainsKey(pair.Key))
                    {
                        throw new ArgumentException("Invalid element");
                    }
                    else
                    {
                        weight += snw[pair.Key]*pair.Value;
                    }
                }
                


                return weight;
            }
        }

        public bool Normalize()
        {
            Dictionary<string, int> elementCount = new Dictionary<string, int>();
            string elementRegex = "([A-Z][a-z]*[a-z]*)([0-9]*)";
            string validateRegex = "^(" + elementRegex + ")+$";

            if (!Regex.IsMatch(Formula, validateRegex))
                throw new FormatException("Input string was in an incorrect format.");

            OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Stoichiometry.mdb");

            // Create data adapter for Elements table
            // This table will be used in read-only mode 
            _adElements = new OleDbDataAdapter();
            _adElements.SelectCommand = new OleDbCommand("SELECT * FROM Elements", conn);

            // Create data adapter for Molecules table
            // This table will be used in both read and write modes
            _adMolecules = new OleDbDataAdapter();
            _adMolecules.SelectCommand = new OleDbCommand("SELECT Formula FROM Molecules", conn);
            _adMolecules.InsertCommand = new OleDbCommand("INSERT INTO Molecules (Formula) VALUES(@Formula)", conn);
            // Create a parameter variable for the insert command (note that this code could easily
            // be modified to initialize the table's MolecularWeight column as well)
            _adMolecules.InsertCommand.Parameters.Add("@Formula", OleDbType.VarChar, -1, "Formula");

            // Create and fill the data set and close the connection 
            _ds = new DataSet();
            conn.Open();
            _adElements.Fill(_ds, "Elements");
            conn.Close();

            //dictionary to hold symbo and weight
            Dictionary<string, double> snw = new Dictionary<string, double>();

            // populate the dictionary
            foreach (DataRow row in _ds.Tables["Elements"].Rows)
            {
                snw.Add(row.Field<string>("Symbol"), row.Field<double>("AtomicWeight"));
            }


            foreach (Match match in Regex.Matches(Formula, elementRegex))
            {

                string name = match.Groups[1].Value;

                int count = match.Groups[2].Value != "" ? int.Parse(match.Groups[2].Value) : 1;
                if (elementCount.ContainsKey(name))
                {
                    elementCount[name]++;
                }
                else
                {
                    elementCount.Add(name, count);
                }
            }

            Formula = "";

            foreach (KeyValuePair<string, int> pair in elementCount)
            {
                if (!snw.ContainsKey(pair.Key))
                {
                    throw new ArgumentException("Invalid element");
                }
            }
            foreach (KeyValuePair<string, int> pair in elementCount)
            {
                if (pair.Value > 1)
                    Formula += pair.Key + pair.Value;
                else if (pair.Value == 1)
                    Formula += pair.Key;
            }
            return true;
        }

        public bool Save()
        {

            OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Stoichiometry.mdb");
                    // Data-related member variables 

            // Create data adapter for Molecules table
            // This table will be used in both read and write modes
            _adMolecules = new OleDbDataAdapter();
            _adMolecules.SelectCommand = new OleDbCommand("SELECT Formula, MolecularWeight FROM Molecules", conn);
            _adMolecules.InsertCommand = new OleDbCommand("INSERT INTO Molecules (Formula, MolecularWeight) VALUES(@Formula , @MolecularWeight)", conn);
            // Create a parameter variable for the insert command (note that this code could easily
            // be modified to initialize the table's MolecularWeight column as well)
            _adMolecules.InsertCommand.Parameters.Add("@Formula", OleDbType.VarChar, -1, "Formula");
            _adMolecules.InsertCommand.Parameters.Add("@MolecularWeight", OleDbType.VarChar, -1, "MolecularWeight");



            // Add molecule to the dataset 

            _ds2 = new DataSet();
            conn.Open();
            _adMolecules.Fill(_ds2, "Molecules");
            conn.Close();

            DataRow molecule = _ds2.Tables["Molecules"].NewRow();
            molecule["Formula"] = Formula;
            molecule["MolecularWeight"] = Weight.ToString();
            _ds2.Tables["Molecules"].Rows.Add(molecule);

            // Update the physical database with changes made to the Molecules
            // table in the dataset
            _adMolecules.Update(_ds2, "Molecules");

            return true;

        }

        public string[] FormulasList
        {
            get
            {
                List<string> listMolecules = new List<string>();
                OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Stoichiometry.mdb");

                // Create data adapter for Molecules table
                // This table will be used in both read and write modes
                _adMolecules = new OleDbDataAdapter();
                _adMolecules.SelectCommand = new OleDbCommand("SELECT Formula FROM Molecules", conn);

                _ds = new DataSet();
                conn.Open();
                _adMolecules.Fill(_ds, "Molecules");
                conn.Close();

                // Populate the Molecules list
                foreach (DataRow row in _ds.Tables["Molecules"].Rows)
                {
                    listMolecules.Add(row.Field<String>("Formula"));
                }

                return listMolecules.ToArray();
            }
        }
    }
}
