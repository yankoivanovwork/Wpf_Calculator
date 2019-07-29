using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private bool sqrtActivated;
        List<double> entryNumbers = new List<double>();
        Dictionary<int, string> numberOperation = new Dictionary<int, string>();
        private DbConnection dbConnection = new DbConnection();

        Dictionary<string, string> elementsDictionary = new Dictionary<string, string>()
        {
            { "OemPlus", "+" },
            { "Add", "+" },
            { "Subtract", "-" },
            { "OemMinus", "-" },
            { "Divide", "/" },
            { "OemQuestion", "/" },
            { "Multiply", "*" },
            { "OemPeriod", "." },
            { "Decimal", "." },
            { "Pow", "^" },
            { "a^b", "^" },
            { "Sqrt", "sqrt" },
            { "√", "sqrt" }
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
                        AddCurrentOperation(elementsDictionary[elementsDictionary.Keys.LastOrDefault()]);
                        return;
                    }

                    if (e.Key == Key.D5)
                    {
                        //AddCurrentOperation(elementsDictionary[elementsDictionary[Key.Multiply.ToString()]]);
                        AddCurrentOperation("%");
                        return;
                    }

                    if (e.Key == Key.D6)
                    {
                        AddCurrentOperation(elementsDictionary[elementsDictionary.Keys.ElementAtOrDefault(elementsDictionary.Count - 3)]);
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
                    if (e.Key.ToString().LastOrDefault() == Key.D0.ToString().LastOrDefault()) //'0')
                    {
                        if (AddZerosCondition() != false)
                            lblCurrentNumber.Content += Key.D0.ToString().LastOrDefault().ToString(); //"0";

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
                CalculateResult();
                return;
            }

            if (e.Key == Key.Back)
            {
                BackspaceDelete();
                return;
            }

            if (e.Key == Key.Delete)
            {
                CeDelete();
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
            if (button.Content.ToString() == Key.D0.ToString().LastOrDefault().ToString()) //"0")
            {
                if (AddZerosCondition() != false)
                    lblCurrentNumber.Content += button.Content.ToString();

                return;
            }

            lblCurrentNumber.Content += button.Content.ToString();
        }

        private void OperationsButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            SetFocusToEqualsButton();

            if (numberOperation.Values.LastOrDefault() == "%")
            {
                if (button.Content.ToString() == "-")
                {
                    return;
                }
            }

            if (button.Content.ToString() == "bksp")
            {
                BackspaceDelete();
                return;
            }

            if (button.Content.ToString() == "CE")
            {
                CeDelete();
                return;
            }

            if (button.Content.ToString() == "a%b")
            {
                AddCurrentOperation("%");
            }

            if (button.Content.ToString() == elementsDictionary[Key.Subtract.ToString()]) //"-")
            {
                if (lblCurrentNumber.Content.ToString() == string.Empty)
                    lblCurrentNumber.Content += button.Content.ToString();
                else
                    AddCurrentOperation(elementsDictionary[Key.Subtract.ToString()]);

                return;
            }

            //if (button.Content.ToString()=="a^b" || button.Content.ToString()== "√")
            if (button.Content.ToString() == elementsDictionary.Keys.LastOrDefault() 
                || button.Content.ToString() == elementsDictionary.Keys.ElementAtOrDefault(elementsDictionary.Count - 3))
            {
                AddCurrentOperation(elementsDictionary[button.Content.ToString()]);
                return;
            }

            AddCurrentOperation(button.Content.ToString());
        }

        private void BtnEquals_Click(object sender, RoutedEventArgs e)
        {
            CalculateResult();
        }

        private void SetFocusToEqualsButton()
        {
            Keyboard.Focus(btnEquals);
        }
        
        private void BackspaceDelete()
        {
            if (lblCurrentNumber.Content.ToString() != string.Empty)
            {
                string currentNumber = lblCurrentNumber.Content.ToString();
                lblCurrentNumber.Content = currentNumber.Remove(currentNumber.Length - 1);
            }
        }

        private void CeDelete()
        {
            lblCurrentNumber.Content = string.Empty;
        }

        private void AddCurrentNumber()
        {
            string currentNumber = lblCurrentNumber.Content.ToString();

            if (currentNumber.Contains('.'))
                currentNumber = currentNumber.Replace(".", string.Empty);

            if (currentNumber.Contains('-'))
                currentNumber = currentNumber.Replace("-", string.Empty);
            
            if (currentNumber.All(zc => zc == Key.D0.ToString().LastOrDefault()) || sqrtActivated)
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
            if (operation == elementsDictionary[Key.Subtract.ToString()]) //"-")
            {
                if (lblCurrentNumber.Content.ToString() == string.Empty)
                {
                    lblCurrentNumber.Content += elementsDictionary[Key.Subtract.ToString()]; //"-";
                    return;
                }
            }

            if (lblCurrentNumber.Content.ToString()=="%" && entryNumbers.Count < 1)
            {
                return;
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
            if (numberOperation[entryNumbers.Count - 1] == elementsDictionary[elementsDictionary.Keys.LastOrDefault()]) //"sqrt")
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

        private void CalculateResult()
        {
            if (lblCalculation.Content.ToString() != string.Empty)
            {
                double currentResult = 0;

                if (lblCurrentNumber.Content.ToString() != string.Empty)
                    AddCurrentNumber();

                for (int i = 0; i < numberOperation.Count; i++)
                {
                    if (numberOperation[numberOperation.Keys.ElementAtOrDefault(i)] == "*" && i < entryNumbers.Count - 1)
                        entryNumbers[i] = entryNumbers.ElementAtOrDefault(i) * entryNumbers.ElementAtOrDefault(i + 1);
                    else if (numberOperation[numberOperation.Keys.ElementAtOrDefault(i)] == "/" && i < entryNumbers.Count - 1)
                        entryNumbers[i] = entryNumbers.ElementAtOrDefault(i) / entryNumbers.ElementAtOrDefault(i + 1);
                    else if (numberOperation[numberOperation.Keys.ElementAtOrDefault(i)] == "^" && i < entryNumbers.Count - 1)
                        entryNumbers[i] = Math.Pow(entryNumbers.ElementAtOrDefault(i), entryNumbers.ElementAtOrDefault(i + 1));  
                    else if (numberOperation[numberOperation.Keys.ElementAtOrDefault(i)] == "sqrt" && i < entryNumbers.Count - 1)
                        entryNumbers[i] = CalculateSqrt(entryNumbers.ElementAtOrDefault(i));
                    else if (numberOperation[numberOperation.Keys.ElementAtOrDefault(i)] == "%" && i < entryNumbers.Count - 1)
                        entryNumbers[i] = (entryNumbers.ElementAtOrDefault(i) * (entryNumbers.ElementAtOrDefault(i + 1) / 100.0d));
                    else
                        continue;

                    entryNumbers.RemoveAt(i + 1);
                    numberOperation.Remove(numberOperation.Keys.ElementAtOrDefault(i));
                    i = -1;
                }

                for (int i = 0; i < entryNumbers.Count; i++)
                {
                    if (numberOperation.ContainsKey(numberOperation.Keys.ElementAtOrDefault(i)) 
                        && numberOperation[numberOperation.Keys.ElementAtOrDefault(i)] == elementsDictionary[Key.Subtract.ToString()]) //"-")    
                    {
                        currentResult += entryNumbers.ElementAtOrDefault(i) - entryNumbers.ElementAtOrDefault(i + 1);
                        entryNumbers.RemoveAt(i + 1);
                    }
                    else
                        currentResult += entryNumbers.ElementAtOrDefault(i);
                }

                if (dbConnection.DatabaseAdd(lblCalculation.Content.ToString() + lblCurrentNumber.Content.ToString(), currentResult.ToString()))
                {
                    lblCalculation.Content = string.Empty;
                    lblCurrentNumber.Content = currentResult;
                    currentResult = 0;
                    ClearTemporaryData();
                }
                else
                    MessageBox.Show("Oops, something went wrong!");
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
            if (lblCurrentNumber.Content.ToString().LastOrDefault() != Key.D0.ToString().LastOrDefault() //'0' 
                || lblCurrentNumber.Content.ToString().Length > 1 
                || lblCurrentNumber.Content.ToString().Contains('.'))
                return true;

            return false;
        }
    }
}