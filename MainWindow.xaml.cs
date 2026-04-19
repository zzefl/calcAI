using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace calcAI
{
    public partial class MainWindow : Window
    {
        private double firstNum = 0;
        private string operatorSymbol = "";
        private bool startNewInput = true;

        public MainWindow()
        {
            InitializeComponent();
            this.Focus(); // Забираем фокус на окно при старте
        }

        // Клик мышкой
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                ProcessCommand(btn.Content.ToString());
            }
        }

        // Нажатие клавиш
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            string key = "";

            // Цифры
            if (e.Key >= Key.D0 && e.Key <= Key.D9) key = (e.Key - Key.D0).ToString();
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) key = (e.Key - Key.NumPad0).ToString();

            // Проверка SHIFT
            bool isShift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            // Знаки
            if (e.Key == Key.Multiply || (e.Key == Key.D8 && isShift)) key = "*";
            else if (e.Key == Key.Add || (e.Key == Key.OemPlus && isShift)) key = "+";
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus || e.Key == Key.Subtract) key = "-";
            else if (e.Key == Key.Divide || e.Key == Key.OemQuestion || e.Key == Key.Oem2) key = "/";

            // Энтер, Бэкспейс, Точка
            else if (e.Key == Key.Enter || (e.Key == Key.OemPlus && !isShift)) key = "=";
            else if (e.Key == Key.Back || e.Key == Key.Escape || e.Key == Key.Delete) key = "C";
            else if (e.Key == Key.OemPeriod || e.Key == Key.Decimal || e.Key == Key.OemComma) key = ".";

            if (!string.IsNullOrEmpty(key))
            {
                ProcessCommand(key);
                e.Handled = true; // Блокируем стандартное поведение Windows
            }
        }

        private void ProcessCommand(string cmd)
        {
            try
            {
                if ("0123456789".Contains(cmd))
                {
                    if (startNewInput) { Display.Text = cmd; startNewInput = false; }
                    else { Display.Text += cmd; }
                }
                else if (cmd == ".")
                {
                    if (startNewInput) { Display.Text = "0."; startNewInput = false; }
                    else if (!Display.Text.Contains(".")) { Display.Text += "."; }
                }
                else if (cmd == "C")
                {
                    Display.Text = "0"; HistoryLabel.Text = "";
                    firstNum = 0; operatorSymbol = ""; startNewInput = true;
                }
                else if (cmd == "√")
                {
                    double val = double.Parse(Display.Text);
                    HistoryLabel.Text = $"√({val})";
                    Display.Text = (val >= 0) ? Math.Sqrt(val).ToString() : "Error";
                    startNewInput = true;
                }
                else if (cmd == "=")
                {
                    double secondNum = double.Parse(Display.Text);
                    double result = 0;
                    switch (operatorSymbol)
                    {
                        case "+": result = firstNum + secondNum; break;
                        case "-": result = firstNum - secondNum; break;
                        case "*": result = firstNum * secondNum; break;
                        case "/": result = secondNum != 0 ? firstNum / secondNum : 0; break;
                        default: result = secondNum; break;
                    }
                    HistoryLabel.Text = $"{firstNum} {operatorSymbol} {secondNum} =";
                    Display.Text = result.ToString();
                    startNewInput = true;
                }
                else // Операторы
                {
                    firstNum = double.Parse(Display.Text);
                    operatorSymbol = cmd;
                    HistoryLabel.Text = $"{firstNum} {operatorSymbol}";
                    startNewInput = true;
                }
            }
            catch { Display.Text = "Error"; startNewInput = true; }
        }
    }
}