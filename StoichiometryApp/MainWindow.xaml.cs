using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StoichiometryLib;

namespace StoichiometryApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IMolecule molecule;

        public MainWindow()
        {
            InitializeComponent();
            IMolecule molecule = new Molecule();
            ComboBoxFormula.ItemsSource = molecule.FormulasList;
            
        }

        private void Button_Close(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Calculate(object sender, RoutedEventArgs e)
        {
            try
            {
                molecule = new Molecule();
                molecule.Formula = ComboBoxFormula.Text;
                molecule.Normalize();
                TextboxWeight.Text = molecule.Weight.ToString();
                if (!molecule.FormulasList.Contains(molecule.Formula))
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to save this result?", "Save?", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        molecule.Save();
                        ComboBoxFormula.Text = "";
                        TextboxWeight.Text = "";
                        ComboBoxFormula.ItemsSource = molecule.FormulasList;
                    }
                    else
                    {
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Normalize(object sender, RoutedEventArgs e)
        {
            try
                {
            molecule = new Molecule();
            molecule.Formula = ComboBoxFormula.Text;
            molecule.Normalize();
            ComboBoxFormula.Text = molecule.Formula;
                }
                catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
