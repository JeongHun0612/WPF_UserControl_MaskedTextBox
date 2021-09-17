using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Media;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net;

namespace MaskedTextBoxProject
{
    public partial class MaskedTextBox : UserControl
    {
        //public TextBox FirstBox { get { return firstBox; } }
        //public TextBox SecondBox { get { return secondBox; } }
        //public TextBox ThirdBox { get { return thirdBox; } }
        //public TextBox FourthBox { get { return fourthBox; } }
        private string firstBoxText = string.Empty;

        public MaskedTextBox()
        {
            InitializeComponent();
        }

        public string GetIPAddress()
        {
            return firstBox.Text + "." + secondBox.Text + "." + thirdBox.Text + "." + fourthBox.Text;
        }

        private void jumpRight(TextBox rightNeighborBox, KeyEventArgs e)
        {
            rightNeighborBox.CaretIndex = 0;
            rightNeighborBox.Focus();

            e.Handled = true;
        }

        private void jumpLeft(TextBox leftNeighborBox, KeyEventArgs e)
        {
            leftNeighborBox.Focus();
            if (leftNeighborBox.Text != "")
            {
                leftNeighborBox.CaretIndex = leftNeighborBox.Text.Length;
            }
            e.Handled = true;
        }

        private bool checkJumpRight(TextBox currentBox, TextBox rightNeighborBox, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    if (currentBox.CaretIndex == currentBox.Text.Length || currentBox.Text == "")
                    {
                        jumpRight(rightNeighborBox, e);
                    }
                    return true;
                case Key.Space:
                    jumpRight(rightNeighborBox, e);
                    rightNeighborBox.SelectAll();
                    return true;

                default:
                    return false;
            }
        }

        private bool checkJumpLeft(TextBox currentBox, TextBox leftNeighborBox, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (currentBox.CaretIndex == 0 || currentBox.Text == "")
                    {
                        jumpLeft(leftNeighborBox, e);
                    }
                    return true;
                case Key.Back:
                    if ((currentBox.CaretIndex == 0 || currentBox.Text == "") && currentBox.SelectionLength == 0)
                    {
                        jumpLeft(leftNeighborBox, e);
                    }
                    return true;
                default:
                    return false;
            }
        }

        private void HandleTextChange(TextBox currentBox, TextBox rightNeighborBox)
        {
            if (currentBox.Text.Length == 3)
            {
                if (!byte.TryParse(currentBox.Text, out _))
                {
                    currentBox.Text = "255";
                }

                if (currentBox != fourthBox)
                {
                    rightNeighborBox.SelectAll();
                    rightNeighborBox.Focus();
                }
            }
        }

        // PreviewKeyDown Event
        private void FirstByte_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            checkJumpRight(firstBox, secondBox, e);
        }

        private void SecondByte_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (checkJumpRight(secondBox, thirdBox, e))
                return;

            checkJumpLeft(secondBox, firstBox, e);
        }

        private void ThirdByte_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (checkJumpRight(thirdBox, fourthBox, e))
                return;

            checkJumpLeft(thirdBox, secondBox, e);
        }

        private void FourthByte_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            checkJumpLeft(fourthBox, thirdBox, e);
        }

        // TextChanged Event
        private void FirstByte_TextChanged(object sender, TextChangedEventArgs e)
        {
            HandleTextChange(firstBox, secondBox);
        }

        private void SecondByte_TextChanged(object sender, TextChangedEventArgs e)
        {
            HandleTextChange(secondBox, thirdBox);
        }

        private void ThirdByte_TextChanged(object sender, TextChangedEventArgs e)
        {
            HandleTextChange(thirdBox, fourthBox);
        }

        private void FourthByte_TextChanged(object sender, TextChangedEventArgs e)
        {
            HandleTextChange(fourthBox, fourthBox);
        }

        // TextInput Event
        private void OnlyNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
