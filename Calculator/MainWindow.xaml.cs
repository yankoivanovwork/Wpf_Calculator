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
        //*
        private bool withoutCurrentNumber = false;
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
            { "a%b", "%" },
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
                        AddCurrentOperation(elementsDictionary[btnPercent.Content.ToString()]);
                        return;
                    }

                    if (e.Key == Key.D6)
                    {
                        AddCurrentOperation(elementsDictionary[btnPow.Content.ToString()]);
                        return;
                    }

                    if (e.Key == Key.D8)
                    {
                        AddCurrentOperation(elementsDictionary[btnMultiply.Content.ToString()]);
                        return;
                    }
                }
            }
            else
            {
                if (KeyPressedIsNumber(e.Key))
                {
                    if (e.Key.ToString().LastOrDefault() == Key.D0.ToString().LastOrDefault())
                    {
                        if (AddZerosCondition() != false)
                            lblCurrentNumber.Content += Key.D0.ToString().LastOrDefault().ToString();

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
                CeDelete();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            SetFocusToEqualsButton();

            if (button.Content.ToString() == btnBackspace.Content.ToString())
            {
                BackspaceDelete();
                return;
            }

            if (button.Content.ToString() == btnCE.Content.ToString())
            {
                CeDelete();
                return;
            }

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

            if (button.Content.ToString() == Key.D0.ToString().LastOrDefault().ToString())
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

            if (button.Content.ToString() == btnPow.Content.ToString()
                || button.Content.ToString() == btnSqrt.Content.ToString())
            {
                AddCurrentOperation(elementsDictionary[button.Content.ToString()]);
                return;
            }

            AddCurrentOperation(button.Content.ToString());
        }

        private void AddCurrentNumber()
        {
            string currentNumber = lblCurrentNumber.Content.ToString();

            if (currentNumber.Contains('.'))
                currentNumber = currentNumber.Replace(".", string.Empty);

            if (currentNumber.Contains('-'))
                currentNumber = currentNumber.Replace("-", string.Empty);

            //if (currentNumber.All(cn => cn == Key.D0.ToString().LastOrDefault()) || sqrtActivated || numberOperation.Values.LastOrDefault() == "%")
            if (currentNumber.All(cn => cn == Key.D0.ToString().LastOrDefault()) 
                || numberOperation.Values.LastOrDefault() == "sqrt" 
                || numberOperation.Values.LastOrDefault() == "%")
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
            if (operation == btnSubtract.Content.ToString() && lblCurrentNumber.Content.ToString() == string.Empty)
            {
                lblCurrentNumber.Content += btnSubtract.Content.ToString();
                return;
            }

            if (operation == btnPercent.Content.ToString() && entryNumbers.Count < 1)
            {
                return;
            }

            if ((lblCurrentNumber.Content.ToString() != string.Empty
                || numberOperation.Values.LastOrDefault() == btnPercent.Content.ToString()
                || numberOperation.Values.LastOrDefault() == elementsDictionary[btnSqrt.Content.ToString()])
                && lblCurrentNumber.Content.ToString().LastOrDefault() != btnSubtract.Content.ToString().FirstOrDefault())
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
            if (numberOperation.Values.LastOrDefault() == elementsDictionary[btnSqrt.Content.ToString()])
            {
                double calculateSqrt = CalculateSqrt(entryNumbers.LastOrDefault());
                lblCalculation.Content += numberOperation.Values.LastOrDefault() + "(" + entryNumbers.LastOrDefault() + ")->" + calculateSqrt;
                lblCurrentNumber.Content = calculateSqrt;
            }
            else if (numberOperation.Values.LastOrDefault() == btnPercent.Content.ToString())
            {
                double calculatePercent = (entryNumbers.ElementAtOrDefault(entryNumbers.Count - 2) * (entryNumbers.LastOrDefault() / 100.0d));
                lblCalculation.Content += entryNumbers.LastOrDefault() + numberOperation.Values.LastOrDefault() + "->" + calculatePercent;
                lblCurrentNumber.Content = calculatePercent;
            }
            else
            {
                if (numberOperation.Values.ElementAtOrDefault(numberOperation.Count - 2) != elementsDictionary[btnSqrt.Content.ToString()]
                    && numberOperation.Values.ElementAtOrDefault(numberOperation.Count - 2) != btnPercent.Content.ToString())
                {
                    lblCalculation.Content += entryNumbers.LastOrDefault() + numberOperation.Values.LastOrDefault();
                    lblCurrentNumber.Content = string.Empty;
                }
                else
                {
                    lblCalculation.Content += numberOperation.Values.LastOrDefault();
                }
            }
        }

        private void CalculateResult()
        {
            if (lblCalculation.Content.ToString() != string.Empty)
            {
                double currentResult = 0;
                string dbCalculationString = string.Empty;

                if (lblCurrentNumber.Content.ToString() != string.Empty)
                    AddCurrentNumber();

                for (int i = 0; i < numberOperation.Count; i++)
                {
                    if (numberOperation[numberOperation.Keys.ElementAtOrDefault(i)] == "*" && i < entryNumbers.Count - 1)
                    {
                        entryNumbers[i] = entryNumbers.ElementAtOrDefault(i) * entryNumbers.ElementAtOrDefault(i + 1);
                        withoutCurrentNumber = false;
                    } 
                    else if (numberOperation[numberOperation.Keys.ElementAtOrDefault(i)] == "/" && i < entryNumbers.Count - 1)
                    {
                        entryNumbers[i] = entryNumbers.ElementAtOrDefault(i) / entryNumbers.ElementAtOrDefault(i + 1);
                        withoutCurrentNumber = false;
                    }  
                    else if (numberOperation[numberOperation.Keys.ElementAtOrDefault(i)] == "^" && i < entryNumbers.Count - 1)
                    {
                        entryNumbers[i] = Math.Pow(entryNumbers.ElementAtOrDefault(i), entryNumbers.ElementAtOrDefault(i + 1));
                        withoutCurrentNumber = false;
                    }  
                    else if (numberOperation[numberOperation.Keys.ElementAtOrDefault(i)] == "sqrt" && i < entryNumbers.Count - 1)
                    {
                        entryNumbers[i] = CalculateSqrt(entryNumbers.ElementAtOrDefault(i));
                        withoutCurrentNumber = true;
                    }  
                    else if (numberOperation[numberOperation.Keys.ElementAtOrDefault(i)] == "%")
                    {
                        entryNumbers[i] = (entryNumbers.ElementAtOrDefault(i - 1) * (entryNumbers.ElementAtOrDefault(i) / 100.0d));
                        withoutCurrentNumber = true;
                        continue;
                    }  
                    else
                        continue;
                    
                    entryNumbers.RemoveAt(i + 1);
                    numberOperation.Remove(numberOperation.Keys.ElementAtOrDefault(i));
                    i = -1;
                }

                for (int i = 0; i < entryNumbers.Count; i++)
                {
                    if (numberOperation.ContainsKey(numberOperation.Keys.ElementAtOrDefault(i))
                        && numberOperation[numberOperation.Keys.ElementAtOrDefault(i)] == btnSubtract.Content.ToString())
                    {
                        currentResult += entryNumbers.ElementAtOrDefault(i) - entryNumbers.ElementAtOrDefault(i + 1);
                        entryNumbers.RemoveAt(i + 1);
                    }
                    else
                        currentResult += entryNumbers.ElementAtOrDefault(i);
                }

                if (withoutCurrentNumber)
                    dbCalculationString = lblCalculation.Content.ToString();
                else
                    dbCalculationString = lblCalculation.Content.ToString() + lblCurrentNumber.Content.ToString();

                if (dbConnection.DatabaseAdd(dbCalculationString, currentResult.ToString()))
                {
                    ClearTemporaryData();
                    lblCurrentNumber.Content = currentResult;
                }
                else
                    MessageBox.Show("Oops, something went wrong!");
            }
        }

        private void ClearTemporaryData()
        {
            lblCalculation.Content = string.Empty;
            CeDelete();
            entryNumbers.Clear();
            numberOperation.Clear();
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

        private void BtnEquals_Click(object sender, RoutedEventArgs e)
        {
            CalculateResult();
        }

        private void SetFocusToEqualsButton()
        {
            Keyboard.Focus(btnEquals);
        }

        private double CalculateSqrt(double entryNumber)
        {
            return Math.Sqrt(entryNumber);
        }

        private bool KeyPressedIsNumber(Key pressedKey)
        {
            if (char.IsDigit(pressedKey.ToString().LastOrDefault()))
                return true;

            return false;
        }

        private bool AddZerosCondition()
        {
            if (lblCurrentNumber.Content.ToString().LastOrDefault() != Key.D0.ToString().LastOrDefault()
                || lblCurrentNumber.Content.ToString().Length > 1
                || lblCurrentNumber.Content.ToString().Contains('.'))
                return true;

            return false;
        }
    }
}