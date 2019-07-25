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
using System.Windows.Shapes;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double currentResult = 0;
        private string currentOperation = string.Empty;
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
            { "Decimal", "." }
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                if (e.Key == Key.D1)
                {
                    CalctulateFromOperation("sqrt");
                    return;
                }

                if (e.Key == Key.D2)
                {
                    CalctulateFromOperation("power2");
                    return;
                }

                if (e.Key == Key.D8)
                {
                    if (lblCurrentNumber.Content.ToString() != string.Empty)
                    {
                        currentOperation = "*";

                        FillCurrentResult();

                        lblCalculation.Content += currentResult.ToString() + currentOperation;
                        ClearCurrentNumber();
                    }
                    return;
                }
            }
            else
            {
                //digit and subtract check
                if (KeyPressedIsNumber(e.Key) || ((e.Key==Key.Subtract || e.Key == Key.OemMinus) && !char.IsDigit(lblCurrentNumber.Content.ToString().LastOrDefault())))
                {
                    if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
                    {
                        if (lblCurrentNumber.Content.ToString().LastOrDefault() != char.Parse(elementsDictionary[e.Key.ToString()]))
                            lblCurrentNumber.Content += elementsDictionary[e.Key.ToString()];
                    }
                    else
                        lblCurrentNumber.Content += e.Key.ToString().LastOrDefault().ToString();

                    return;
                }

                //calculate result and fill label calculation, from given operation that is not =
                if (lblCurrentNumber.Content.ToString().LastOrDefault() != '-')
                    CalculateResultByGivenOperation(e.Key);
            }

            if (e.Key == Key.Enter)
                EnterKeyPressed(e.Key);

            if (e.Key == Key.Back)
            {
                if (lblCurrentNumber.Content.ToString() != string.Empty)
                {
                    string currentNumber = lblCurrentNumber.Content.ToString();
                    lblCurrentNumber.Content = currentNumber.Remove(currentNumber.Length - 1);
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            currentResult = 0;
            ClearCurrentNumber();
            ClearCurrentOperationAndCalculationLabel();
        }

        private void btnDB_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            DbPreviewWindow dbPreview = new DbPreviewWindow();
            dbPreview.ShowDialog();
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            lblCurrentNumber.Content += "1";
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            lblCurrentNumber.Content += "2";
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            lblCurrentNumber.Content += "3";
        }

        private void btn4_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            lblCurrentNumber.Content += "4";
        }

        private void btn5_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            lblCurrentNumber.Content += "5";
        }

        private void btn6_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            lblCurrentNumber.Content += "6";
        }

        private void btn7_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            lblCurrentNumber.Content += "7";
        }

        private void btn8_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            lblCurrentNumber.Content += "8";
        }

        private void btn9_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            lblCurrentNumber.Content += "9";
        }

        private void btn0_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            if (lblCurrentNumber.Content.ToString()!=string.Empty
                && lblCurrentNumber.Content.ToString().LastOrDefault() != '0')
            {
                lblCurrentNumber.Content += "0";
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            CalculateResultByGivenOperation(Key.Add);
        }

        private void btnSubtract_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            CalculateResultByGivenOperation(Key.Subtract);
        }

        private void btnPow2_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            CalctulateFromOperation("power2");
        }

        private void btnSqrt_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            CalctulateFromOperation("sqrt");
        }

        private void btnMultiply_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            CalculateResultByGivenOperation(Key.Multiply);
        }

        private void btnDivide_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            CalculateResultByGivenOperation(Key.Divide);
        }

        private void btnDecimal_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            PlaceDecimalPointIntoCurrentNumberLabel();
        }

        private void btnEquals_Click(object sender, RoutedEventArgs e)
        {
            SetFocusToEqualsButton();
            EnterKeyPressed(Key.Enter);
        }

        private void CalculateResultByGivenOperation(Key pressedKey)
        {
            if (lblCurrentNumber.Content.ToString() != string.Empty && elementsDictionary.ContainsKey(pressedKey.ToString()))
            {
                if (elementsDictionary[pressedKey.ToString()] == ".")
                {
                    PlaceDecimalPointIntoCurrentNumberLabel();
                    return;
                }
                else
                {
                    FillCurrentOperation(pressedKey);

                    FillCurrentResult();

                    lblCalculation.Content += lblCurrentNumber.Content.ToString() + elementsDictionary[pressedKey.ToString()];
                    ClearCurrentNumber();
                    return;
                }
            }
            else
            {
                //smqna na operaciqta -> 12+ -> - -> 12-
                if (lblCalculation.Content.ToString() != string.Empty && elementsDictionary.ContainsKey(pressedKey.ToString()))
                {
                    string calculationString = lblCalculation.Content.ToString();
                    lblCalculation.Content = calculationString.Remove(calculationString.Length - 1);
                    lblCalculation.Content += elementsDictionary[pressedKey.ToString()];
                    FillCurrentOperation(pressedKey);
                    return;
                }
            }
        }

        private void EnterKeyPressed(Key pressedKey)
        {
            if (lblCalculation.Content.ToString() != string.Empty)
            {
                string tempCurrentNumber = string.Empty;

                if (lblCurrentNumber.Content.ToString() == string.Empty)
                    lblCurrentNumber.Content = currentResult.ToString();
                else
                {
                    currentResult = MakeCalculation(currentResult, double.Parse(lblCurrentNumber.Content.ToString()), currentOperation);
                    tempCurrentNumber = lblCurrentNumber.Content.ToString();
                    lblCurrentNumber.Content = currentResult;
                }

                if (dbConnection.DatabaseAdd(lblCalculation.Content.ToString() + tempCurrentNumber, lblCurrentNumber.Content.ToString()))
                    ClearCurrentOperationAndCalculationLabel();
                else
                    MessageBox.Show("Oops, something went wrong!");

                return;
            }
        }

        private void PlaceDecimalPointIntoCurrentNumberLabel()
        {
            if (lblCurrentNumber.Content.ToString().LastOrDefault() != ',')
                lblCurrentNumber.Content += ",";
        }

        private void SetFocusToEqualsButton()
        {
            Keyboard.Focus(btnEquals);
        }

        private void ClearCurrentOperationAndCalculationLabel()
        {
            currentOperation = string.Empty;
            lblCalculation.Content = string.Empty;
        }

        private void ClearCurrentNumber()
        {
            lblCurrentNumber.Content = string.Empty;
        }

        private void FillCurrentOperation(Key pressedKey)
        {
            currentOperation = elementsDictionary[pressedKey.ToString()];
        }

        private bool KeyPressedIsNumber(Key pressedKey)
        {
            if (char.IsDigit(pressedKey.ToString().LastOrDefault()))
                return true;

            return false;
        }

        private double MakeCalculation(double firstNumber, double secondNumber, string operation)
        {
            double result = 0;
            switch (operation)
            {
                case "+":
                    result = firstNumber + secondNumber;
                    break;
                case "-":
                    result = firstNumber - secondNumber;
                    break;
                case "*":
                    result = firstNumber * secondNumber;
                    break;
                case "/":
                    result = firstNumber / secondNumber;
                    break;
                case "sqrt":
                    result = Math.Sqrt(secondNumber);
                    break;
                default:
                    //power2
                    result = Math.Pow(secondNumber, 2);
                    break;
            }

            return result;
        }

        private void FillCurrentResult()
        {
            if (lblCurrentNumber.Content.ToString().LastOrDefault()!='-')
            {
                currentResult = lblCalculation.Content.ToString() == string.Empty ? double.Parse(lblCurrentNumber.Content.ToString())
                : MakeCalculation(currentResult, double.Parse(lblCurrentNumber.Content.ToString()), currentOperation);
            }
        }

        private void CalctulateFromOperation(string operation)
        {
            if (lblCurrentNumber.Content.ToString() != string.Empty)
            {
                currentOperation = operation;
                currentResult = MakeCalculation(0, double.Parse(lblCurrentNumber.Content.ToString()), operation);
                lblCurrentNumber.Content = currentResult;
            }
        }
    }
}







/*if (lblCalculation.Content.ToString() == string.Empty)
                currentResult = double.Parse(lblCurrentNumber.Content.ToString());
            else
                currentResult = MakeCalculation(currentResult, double.Parse(lblCurrentNumber.Content.ToString()), currentOperation);*/

/*if (operation == "sqrt")
{
    lblCalculation.Content += "sqrt("+ lblCurrentNumber.Content.ToString()+")";
}
if (operation == "power2")
{
    lblCalculation.Content += lblCurrentNumber.Content.ToString() + "^2";
}*/
