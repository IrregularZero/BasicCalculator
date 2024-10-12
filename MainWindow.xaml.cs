using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool equals_Pressed; // If equals is pressed display should be cleared with next press of the button

        private string display;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Display 
        {
            get
            {
                return display;
            }
            set
            {
                display = value;

                OnPropertyChanged("Display");
            }
        }

        public MainWindow()
        {
            DataContext = this; // Setting this script as data context for the window so that PropertyChanged actually works
            InitializeComponent();

            display = " "; // Just an empty form of string needed for code
            equals_Pressed = false;
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /*
         Return values:
            "isOperator" - if is + - * /
            "isDot" - if is .
            "isNumber" - if is 1, 2, 3, 4...
            "nothing" - if is unknown
         */
        private string CheckWhatKindOfElement(string element)
        {
            string output = string.Empty;

            switch (element)
            {
                case "/":
                case "*":
                case "-":
                case "+": output = "isOperator"; break;

                case ".": output = "isDot"; break;

                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9": output = "isNumber"; break;
                default: output = "nothing"; break;
            }

            return output;
        }

        public void Numpad_OnClick(object sender, RoutedEventArgs e)
        {
            // After Equals was pressed this part works out and clears display
            if (equals_Pressed)
            {
                Display = " ";
                equals_Pressed = false;
            }

            // Element is equals to a content in a button pressed
            string element = string.Empty;
            if (e is not null)
            {
                element = (e.Source as Button).Content.ToString();
            }
            else
            {
                return;
            }

            string elementType = CheckWhatKindOfElement(element);

            // Depending on Element it's placement conditions differ
            /*
             Operator can be placed if:
                    not the first symbol
                    not placed before dot or other operator
             Dot can be placed if:
                    not placed before Operator and another Dot
                Also if placed first 0 will be placed before it
             Number can be placed without any conditions.
             */
            switch (elementType)
            {
                case "isOperator": 
                    if (display.Length <= 1)
                    {
                        return;
                    }
                    else if (display.Length == 2) // Needed extra check, becouse there was bug if there wasn't
                    {
                        if (CheckWhatKindOfElement(display[1].ToString()) != "isOperator" 
                            && CheckWhatKindOfElement(display[1].ToString()) != "isDot")
                        {
                            Display += ' ';
                            Display += element;
                            Display += ' ';
                        }
                    }
                    else
                    {
                        if (CheckWhatKindOfElement(display[display.Length - 1].ToString()) != "isOperator"
                            && CheckWhatKindOfElement(display[display.Length - 1].ToString()) != "isDot")
                        {
                            Display += ' ';
                            Display += element;
                            Display += ' ';
                        }
                    }
                    break;
                case "isDot":
                    if (display.Length <= 1)
                    {
                        Display += '0' + element;
                    }
                    else if (display.Length == 2)
                    {
                        if (CheckWhatKindOfElement(display[0].ToString()) != "isOperator"
                            && CheckWhatKindOfElement(display[0].ToString()) != "isDot")
                        {
                            Display += element;
                        }
                    }
                    else
                    {
                        if (CheckWhatKindOfElement(display[display.Length - 1].ToString()) != "isOperator"
                            && CheckWhatKindOfElement(display[display.Length - 1].ToString()) != "isDot")
                        {
                            Display += element;
                        }
                    }
                    break;
                case "isNumber":
                    Display += element;
                    break;
            }
        }
        public void Clear_OnClick(object sender, RoutedEventArgs e)
        {
            Display = " ";
            equals_Pressed = false; // Since we already cleared no need in it even after equal sign was pressed
        }
        public void Equals_OnClick(object sender, RoutedEventArgs e)
        {
            /*
             The idia behind code:
                    First it searches for the operator in a string divided by spaces,
                    then it takes number before it and after it and makes operation,
                    according to opperator they were found under.
                    Sets result in first value's place and deletes operator along with second value.
                    Operation continues for the amount of operators in equation.
             Example:
                    1. 2 + 2 * 2
                    2. 2 + 4
                    3. 6
             Output: 6
             */

            // Dividing symbols by space
            string[] symbolsBad = display.Split(' ');
            List<string> symbols = new List<string>(symbolsBad);
            symbols.RemoveAt(0);

            try
            {
                for (int i = 0; i < symbols.Count; i++)
                {
                    if (CheckWhatKindOfElement(symbols[i]) == "isOperator")
                    {
                        double result;
                        if (symbols[i] == "*")
                        {
                            result = double.Parse(symbols[i - 1]) * double.Parse(symbols[i + 1]);
                            symbols[i - 1] = result.ToString();

                            symbols.RemoveAt(i + 1);
                            symbols.RemoveAt(i);
                        }
                        else if (symbols[i] == "/")
                        {
                            if (double.Parse(symbols[i + 1]) == 0.0)
                                throw new DivideByZeroException();

                            result = double.Parse(symbols[i - 1]) / double.Parse(symbols[i + 1]);
                            symbols[i - 1] = result.ToString();

                            symbols.RemoveAt(i + 1);
                            symbols.RemoveAt(i);
                        }
                    }
                }
                for (int i = 0; i < symbols.Count; i++)
                {
                    if (CheckWhatKindOfElement(symbols[i]) == "isOperator")
                    {
                        double result;
                        if (symbols[i] == "+")
                        {
                            result = double.Parse(symbols[i - 1]) + double.Parse(symbols[i + 1]);
                            symbols[i - 1] = result.ToString();

                            symbols.RemoveAt(i + 1);
                            symbols.RemoveAt(i);
                        }
                        else if (symbols[i] == "-")
                        {
                            result = double.Parse(symbols[i - 1]) - double.Parse(symbols[i + 1]);
                            symbols[i - 1] = result.ToString();

                            symbols.RemoveAt(i + 1);
                            symbols.RemoveAt(i);
                        }
                    }
                }

                Display = symbols[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                equals_Pressed = true;
            }
        }
    }
}