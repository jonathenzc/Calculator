﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Calculator
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CalculatorPage : Page
    {
        private int basicInt;//如果面板上的数是整数

        private double basicDouble;//如果面板上的数是小数

        private bool basicSymbolClicked; //加减乘除MOD等基本运算按键是否按下

        private string ResultTextBlockStr;//计算框中的字符串

        private string ProgressTextBlockStr;//过程框中的字符串


        //分号辅助变量
        private double itsFraction;
        private int fractionClickCnt;//分数按钮点击次数

        //根号辅助变量
        private double itsSqrt;
        private int sqrtClickCnt;//根号按钮点击次数

        public CalculatorPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            basicSymbolClicked = false;
            basicInt = 0;
            basicDouble = 0;
            ResultTextBlockStr = "";
            ProgressTextBlockStr = "";
            itsFraction = 0;
            fractionClickCnt = 0;
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: 准备此处显示的页面。

            // TODO: 如果您的应用程序包含多个页面，请确保
            // 通过注册以下事件来处理硬件“后退”按钮:
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed 事件。
            // 如果使用由某些模板提供的 NavigationHelper，
            // 则系统会为您处理该事件。
        }

        //计算器功能按钮事件，包括后退、清理回车、清空、等号
        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            ResultTextBlock.Text = "0";
            ProgressTextBlock.Text = "";

            basicSymbolClicked = false;
            basicInt = 0;
            basicDouble = 0;
            ResultTextBlockStr = "";
            ProgressTextBlockStr = "";
            itsFraction = 0;
            fractionClickCnt = 0;
            ResultTextBlock.FontSize = 80;//在进行分数计算时，如果数为0时，不能计算。此时的FontSize位50，这里重新将其设置为初始值
            sqrtClickCnt = 0;
            itsSqrt = 0;
        }

        //数字按钮事件（包括小数点）
        //NOTE：这个地方的实现值得改进，可以根据只通过一个函数来实现所有功能。目前不知道如何在xaml控件中传参数或者辨识哪个按钮触发事件
        private void Button0_Click(object sender, RoutedEventArgs e)
        {
            if (ResultTextBlock.Text != "0")
                ResultTextBlock.Text += "0";
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (ResultTextBlock.Text != "0")
                ResultTextBlock.Text += "1";
            else
                ResultTextBlock.Text = "1";
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            if (ResultTextBlock.Text != "0")
                ResultTextBlock.Text += "2";
            else
                ResultTextBlock.Text = "2";
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            if (ResultTextBlock.Text != "0")
                ResultTextBlock.Text += "3";
            else
                ResultTextBlock.Text = "3";
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            if (ResultTextBlock.Text != "0")
                ResultTextBlock.Text += "4";
            else
                ResultTextBlock.Text = "4";
        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            if (ResultTextBlock.Text != "0")
                ResultTextBlock.Text += "5";
            else
                ResultTextBlock.Text = "5";
        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            if (ResultTextBlock.Text != "0")
                ResultTextBlock.Text += "6";
            else
                ResultTextBlock.Text = "6";
        }


        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            if (ResultTextBlock.Text != "0")
                ResultTextBlock.Text += "7";
            else
                ResultTextBlock.Text = "7";
        }

        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            if (ResultTextBlock.Text != "0")
                ResultTextBlock.Text += "8";
            else
                ResultTextBlock.Text = "8";
        }

        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            if (ResultTextBlock.Text != "0")
                ResultTextBlock.Text += "9";
            else
                ResultTextBlock.Text = "9";
        }

        private void ButtonPoint_Click(object sender, RoutedEventArgs e)
        {
            String resultStr = ResultTextBlock.Text;
            if (!resultStr.Contains("."))
                ResultTextBlock.Text += ".";
        }

        //基本运算按钮事件，包括加减乘除、mod
        private void ButtonPlus_Click(object sender, RoutedEventArgs e)
        {
            if (!basicSymbolClicked)
                ResultTextBlockStr = ResultTextBlock.Text;

            ProgressTextBlock.Text = ResultTextBlockStr + " +";
            basicSymbolClicked = true;
        }

        private void ButtonMinus_Click(object sender, RoutedEventArgs e)
        {
            if (!basicSymbolClicked)
                ResultTextBlockStr = ResultTextBlock.Text;

            ProgressTextBlock.Text = ResultTextBlockStr + " -";
            basicSymbolClicked = true;
        }

        private void ButtonMultiply_Click(object sender, RoutedEventArgs e)
        {
            if (!basicSymbolClicked)
                ResultTextBlockStr = ResultTextBlock.Text;

            ProgressTextBlock.Text = ResultTextBlockStr + " *";
            basicSymbolClicked = true;
        }

        private void ButtonDivide_Click(object sender, RoutedEventArgs e)
        {
            if (!basicSymbolClicked)
                ResultTextBlockStr = ResultTextBlock.Text;

            ProgressTextBlock.Text = ResultTextBlockStr + " /";
            basicSymbolClicked = true;
        }

        private void ButtonMod_Click(object sender, RoutedEventArgs e)
        {
            if (!basicSymbolClicked)
                ResultTextBlockStr = ResultTextBlock.Text;

            ProgressTextBlock.Text = ResultTextBlockStr + " Mod";
            basicSymbolClicked = true;
        }

        //单运算符按钮事件，包括根号、正负号、分号
        private void ButtonRoot_Click(object sender, RoutedEventArgs e)
        {
            sqrtClickCnt++;
            ResultTextBlockStr = ResultTextBlock.Text;
            ProgressTextBlockStr = ProgressTextBlock.Text;

            if (ResultTextBlockStr[0] != '-' && ResultTextBlockStr != "无效输入")
            {
                //第一次按根号按钮得到该数
                if (sqrtClickCnt == 1)
                {
                    basicDouble = double.Parse(ResultTextBlockStr);
                    ProgressTextBlockStr = basicDouble.ToString();
                }

                //加减乘除Mod基本运算按钮还没有按，只用处理在字符串前面加sqrt
                if (!basicSymbolClicked)
                {
                    //显示过程框的内容
                    ProgressTextBlock.Text = "sqrt(" + ProgressTextBlockStr + ")";
                }
                else
                {
                    ProgressTextBlockStr = ProgressTextBlock.Text;
                    char[] basicSymbolArray = new char[] { '+', '-', '*', '/', 'M' };
                    int basicSymbolIndex = ProgressTextBlockStr.IndexOfAny(basicSymbolArray);

                    //获取之前的过程字符串和基数字符串
                    //处理这样的操作:98 + sqrt(98)，当再触发该分号按钮后，变成98 + sqrt(sqrt(98))
                    string baseNumberStr = ProgressTextBlockStr.Substring(0, basicSymbolIndex - 1);
                    string previousProgressStr = ProgressTextBlockStr.Substring(0, basicSymbolIndex + 1);
                    string reciprocStr = "";

                    if (basicSymbolIndex + 2 < ProgressTextBlockStr.Length)//第一次按分号，如字符串ProgressTextBlockStr为"98 +"
                        reciprocStr = " sqrt(" + ProgressTextBlockStr.Substring(basicSymbolIndex + 2) + ")";
                    else
                        reciprocStr = " sqrt(" + baseNumberStr + ")";

                    ProgressTextBlock.Text = previousProgressStr + reciprocStr;
                }

                //显示当前结果框的内容
                basicDouble = double.Parse(ResultTextBlockStr);
                itsSqrt = System.Math.Sqrt(basicDouble);
                ResultTextBlock.Text = itsSqrt.ToString();
            }
            else
            {
                ResultTextBlock.FontSize = 50;
                ResultTextBlock.Text = "无效输入";
            }
        }

        private void ButtonFraction_Click(object sender, RoutedEventArgs e)
        {
            fractionClickCnt++;
            ResultTextBlockStr = ResultTextBlock.Text;
            ProgressTextBlockStr = ProgressTextBlock.Text;

            if (ResultTextBlockStr != "0" && ResultTextBlockStr != "除数不能为0")
            {
                //第一次按分数按钮得到该数的分数
                if (fractionClickCnt == 1)
                {
                    basicDouble = double.Parse(ResultTextBlockStr);
                    itsFraction = 1.0 / basicDouble;
                    ProgressTextBlockStr = basicDouble.ToString();
                }

                //加减乘除Mod基本运算按钮还没有按，只用处理在字符串前面加reciproc
                if (!basicSymbolClicked)
                {
                    //显示过程框的内容
                    ProgressTextBlock.Text = "reciproc(" + ProgressTextBlockStr + ")";
                }
                else
                {
                    ProgressTextBlockStr = ProgressTextBlock.Text;
                    char[] basicSymbolArray = new char[] { '+', '-', '*', '/', 'M' };
                    int basicSymbolIndex = ProgressTextBlockStr.IndexOfAny(basicSymbolArray);

                    //获取之前的过程字符串和基数字符串
                    //处理这样的操作:98 + reciproc(98)，当再触发该分号按钮后，变成98 + reciproc(reciproc(98))
                    string baseNumberStr = ProgressTextBlockStr.Substring(0, basicSymbolIndex - 1);
                    string previousProgressStr = ProgressTextBlockStr.Substring(0, basicSymbolIndex + 1);
                    string reciprocStr = "";

                    if (basicSymbolIndex + 2 < ProgressTextBlockStr.Length)//第一次按分号，如字符串ProgressTextBlockStr为"98 +"
                        reciprocStr = " reciproc(" + ProgressTextBlockStr.Substring(basicSymbolIndex + 2) + ")";
                    else
                        reciprocStr = " reciproc(" + baseNumberStr + ")";

                    ProgressTextBlock.Text = previousProgressStr + reciprocStr;
                }

                //显示当前结果框的内容
                if (fractionClickCnt % 2 == 1)
                    ResultTextBlock.Text = itsFraction.ToString();
                else
                    ResultTextBlock.Text = basicDouble.ToString();
            }
            else
            {
                ResultTextBlock.FontSize = 50;
                ResultTextBlock.Text = "除数不能为0";
            }
        }

        private void ButtonPosiNege_Click(object sender, RoutedEventArgs e)
        {
            ResultTextBlockStr = ResultTextBlock.Text;
            ProgressTextBlockStr = ProgressTextBlock.Text;

            //结果框若为0，没有正负号
            if (ResultTextBlockStr != "0")
            {
                //加减乘除Mod基本运算按钮触发后，需要处理过程框中的运算过程
                if (basicSymbolClicked)
                {
                    char[] basicSymbolArray = new char[] { '+', '-', '*', '/', 'M' };
                    int basicSymbolIndex = ProgressTextBlockStr.IndexOfAny(basicSymbolArray);

                    //获取之前的过程字符串和基数字符串
                    //处理这样的操作:98 + -(98)，当再触发该正负号按钮后，变成98 + -(-(98))
                    string baseNumberStr = ProgressTextBlockStr.Substring(0, basicSymbolIndex - 1);
                    string previousProgressStr = ProgressTextBlockStr.Substring(0, basicSymbolIndex + 1);
                    string negativeStr = "";

                    if (basicSymbolIndex + 2 < ProgressTextBlockStr.Length)//第一次按正负号，如字符串ProgressTextBlockStr为"98 +"
                        negativeStr = " -(" + ProgressTextBlockStr.Substring(basicSymbolIndex + 2) + ")";
                    else
                        negativeStr = " -(" + baseNumberStr + ")";

                    ProgressTextBlock.Text = previousProgressStr + negativeStr;
                }

                //对结果框的结果添加正负号
                if (ResultTextBlockStr.Contains("-"))
                    ResultTextBlock.Text = ResultTextBlockStr.Substring(1);
                else
                    ResultTextBlock.Text = "-" + ResultTextBlockStr;
            }
        }
    }
}