using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool sqrtActivated;
        List<double> entryNumbers = new List<double>();
        Dictionary<int, string> numberOperation = new Dictionary<int, string>();
        private DbConnection dbConnection = new DbConnection();

        Dictionary<string, string> elementsDictionary = new Dictionary<string, string>()
        {
            { "oemPlus", "+" },
            { "Add", "+" },
            { "Subtract", "-" },
            { "OemMinus", "-" },
            { "Divide", "/" },
            { "OemQuestion", "/" },
            { "Multiply", "*" },
            { "OemPeriod", "." },
            { "Decimal", "." },
            { "Power", "^" },
            { "Sqrt", "sqrt" }
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                if (lblCurrentNumber.Content.ToString() != string.Empty)
                {
                    if (e.Key == Key.D1)
                    {
                        AddCurrentOperation(elementsDictionary["Sqrt"]);
                        return;
                    }

                    if (e.Key == Key.D2)
                    {
                        AddCurrentOperation(elementsDictionary["Power"]);
                        return;
                    }

                    if (e.Key == Key.D8)
                    {
                        AddCurrentOperation(elementsDictionary[elementsDictionary[Key.Multiply.ToString()]]);
                        return;
                    }
                }
            }
            else
            {
                if (KeyPressedIsNumber(e.Key))
                {
                    if (e.Key.ToString().LastOrDefault() == '0')
                    {
                        if (AddZerosCondition() != false)
                            lblCurrentNumber.Content += "0";

                        return;
                    }

                    lblCurrentNumber.Content += e.Key.ToString().LastOrDefault().ToString();
                    return;
                }

                if ((e.Key != Key.Enter || e.Key != Key.Back) && elementsDictionary.ContainsKey(e.Key.ToString()))
                {
                    AddCurrentOperation(elementsDictionary[e.Key.ToString()]);
                    return;
                }
            }

            if (e.Key == Key.Enter)
            {
                EnterKeyPressed();
                return;
            }

            if (e.Key == Key.Back)
            {
                if (lblCurrentNumber.Content.ToString() != string.Empty)
                {
                    string currentNumber = lblCurrentNumber.Content.ToString();
                    lblCurrentNumber.Content = currentNumber.Remove(currentNumber.Length - 1);
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            ClearCurrentNumberAndCalculationLabel();
            ClearTemporaryData();
        }

        private void BtnDB_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            DbPreviewWindow dbPreview = new DbPreviewWindow();
            dbPreview.ShowDialog();
        }

        private void ButtonNumbers_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            SetFocusToEqualsButton();
            if (button.Content.ToString() == "0")
            {
                if (AddZerosCondition() != false)
                    lblCurrentNumber.Content += button.Content.ToString();

                return;
            }

            lblCurrentNumber.Content += button.Content.ToString();
        }

        private void BtnPow_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();

            AddCurrentOperation(elementsDictionary["Power"]);
        }

        private void BtnSqrt_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();

            AddCurrentOperation(elementsDictionary["Sqrt"]);
        }

        private void OperationsButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            SetFocusToEqualsButton();

            if (button.Content.ToString() == "-")
            {
                if (lblCurrentNumber.Content.ToString() == string.Empty)
                    lblCurrentNumber.Content += button.Content.ToString();
                else
                    AddCurrentOperation(elementsDictionary[Key.Subtract.ToString()]);

                return;
            }
            
            AddCurrentOperation(button.Content.ToString());
        }

        private void BtnEquals_Click(object sender, RoutedEventArgs e)
        {
            EnterKeyPressed();
        }

        private void SetFocusToEqualsButton()
        {
            Keyboard.Focus(btnEquals);
        }
        
        private void AddCurrentNumber()
        {
            string currentNumber = lblCurrentNumber.Content.ToString();

            if (currentNumber.Contains('.'))
                currentNumber = currentNumber.Replace(".", string.Empty);

            if (currentNumber.Contains('-'))
                currentNumber = currentNumber.Replace("-", string.Empty);
            
            if (currentNumber.All(zc => zc == '0') || sqrtActivated)
                entryNumbers.Add(0);
            else
            {
                #pragma warning disable IDE0018 // Inline variable declaration
                double numberToAdd;
                #pragma warning restore IDE0018 // Inline variable declaration
                if (double.TryParse(lblCurrentNumber.Content.ToString().Replace(".", ","), out numberToAdd))
                    entryNumbers.Add(numberToAdd);
            }   
        }

        private void AddCurrentOperation(string operation)
        {
            if (operation == "-")
            {
                if (lblCurrentNumber.Content.ToString() == string.Empty)
                {
                    lblCurrentNumber.Content += "-";
                    return;
                }
            }

            if (lblCurrentNumber.Content.ToString() != string.Empty && lblCurrentNumber.Content.ToString().LastOrDefault() != '-')
            {
                if (operation == ".")
                {
                    if (lblCurrentNumber.Content.ToString() != string.Empty 
                        && !lblCurrentNumber.Content.ToString().Contains('.') 
                        && lblCurrentNumber.Content.ToString().LastOrDefault() != '.')
                        lblCurrentNumber.Content += ".";

                    return;
                }

                AddCurrentNumber();
                numberOperation.Add(entryNumbers.Count - 1, operation);
                AddForPreview();
            }
        }

        private void AddForPreview()
        {
            if (numberOperation[entryNumbers.Count - 1] == "sqrt")
            {
                lblCalculation.Content += numberOperation[entryNumbers.Count - 1] + "(" + entryNumbers.LastOrDefault() + ")";
                lblCurrentNumber.Content = CalculateSqrt(entryNumbers.LastOrDefault());
                sqrtActivated = true;
            }
            else    
            {
                if (!sqrtActivated)
                {
                    lblCalculation.Content += entryNumbers.LastOrDefault() + numberOperation[numberOperation.Keys.LastOrDefault()];
                    lblCurrentNumber.Content = string.Empty;
                }
                else
                {
                    lblCalculation.Content += numberOperation[numberOperation.Keys.LastOrDefault()];
                    sqrtActivated = false;
                } 
            }
        }

        private double CalculateSqrt(double entryNumber)
        {
            return Math.Sqrt(entryNumber);
        }

        private void EnterKeyPressed()
        {
            //neraboti!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            ///add cylce to do * and / and sqrt() and a^b, lastly do + and -, return result
            ///10000,0000 is bugged only one 0 before decimal separator

            if (lblCalculation.Content.ToString() != string.Empty && lblCalculation.Content.ToString().LastOrDefault() != '^')
            {
                double currentResult = 0;

                if (lblCurrentNumber.Content.ToString() != string.Empty)
                    AddCurrentNumber();

                for (int i = 0, j = 0; i < entryNumbers.Count; i++, j++)
                {
                    switch (numberOperation[numberOperation.Keys.ElementAtOrDefault(j)])
                    {
                        case "+":
                            currentResult += entryNumbers.ElementAtOrDefault(i);
                            break;
                        case "-":
                            currentResult -= entryNumbers.ElementAtOrDefault(i);
                            break;
                        case "*":
                            currentResult += entryNumbers.ElementAtOrDefault(i) * entryNumbers.ElementAtOrDefault(i + 1);
                            i++;
                            break;
                        case "/":
                            currentResult += entryNumbers.ElementAtOrDefault(i) / entryNumbers.ElementAtOrDefault(i + 1);
                            i++;
                            break;
                        case "sqrt":
                            currentResult += CalculateSqrt(entryNumbers.ElementAtOrDefault(i)) + entryNumbers.ElementAtOrDefault(i + 1);
                            i++;
                            break;
                        case "^":
                            currentResult += Math.Pow(entryNumbers.ElementAtOrDefault(i), entryNumbers.ElementAtOrDefault(i + 1));
                            i++;
                            break;
                        default:
                            break;
                    }
                }

                /*if (dbConnection.DatabaseAdd(lblCalculation.Content.ToString() + lblCurrentNumber.Content.ToString(), currentResult.ToString()))
                {
                    lblCalculation.Content = string.Empty;
                    lblCurrentNumber.Content = currentResult;
                    currentResult = 0;
                    ClearTemporaryData();
                }
                else
                    MessageBox.Show("Oops, something went wrong!");*/
                lblCalculation.Content = string.Empty;
                lblCurrentNumber.Content = currentResult;
                currentResult = 0;
                ClearTemporaryData();
            }
        }

        private void ClearTemporaryData()
        {
            sqrtActivated = false;
            entryNumbers.Clear();
            numberOperation.Clear();
        }

        private void ClearCurrentNumberAndCalculationLabel()
        {
            lblCalculation.Content = string.Empty;
            lblCurrentNumber.Content = string.Empty;
        }

        private bool KeyPressedIsNumber(Key pressedKey)
        {
            if (char.IsDigit(pressedKey.ToString().LastOrDefault()))
                return true;

            return false;
        }

        private bool AddZerosCondition()
        {
            if (lblCurrentNumber.Content.ToString().LastOrDefault() != '0' || lblCurrentNumber.Content.ToString().Contains('.'))
                return true;

            return false;
        }
    }
}