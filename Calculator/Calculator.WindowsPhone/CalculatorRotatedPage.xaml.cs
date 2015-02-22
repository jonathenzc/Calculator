﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Calculator
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CalculatorRotatedPage : Page
    {
        //设备旋转感应器
        private SimpleOrientationSensor sensor;

        private double basicDouble;//如果面板上的数是小数

        private bool basicSymbolClicked; //加减乘除MOD等基本运算按键是否按下
        private bool operNumClicking;//操作数正在输入
        private bool equalClicked;//等号已经按下
        private bool isErrorInput;//分母为0、根号内为负数时，计算器必须按C才能重新开始

        private string ResultTextBlockStr;//计算框中的字符串

        private string ProgressTextBlockStr;//过程框中的字符串

        private double operNum1;//第一个操作数

        private string operSymbol;//操作符

        private double operNum2;//第二个操作数

        char[] basicSymbolArray = new char[] { '+', '-', '*', '/', '%' };

        //分号辅助变量
        private double itsFraction;
        private int fractionClickCnt;//分数按钮点击次数

        //根号辅助变量
        private double itsSqrt;
        private int sqrtClickCnt;//根号按钮点击次数

        public CalculatorRotatedPage()
        {
            this.InitializeComponent();

            sensor = SimpleOrientationSensor.GetDefault();
            // Assign an event handler for the sensor orientation-changed event
            if (sensor != null)
            {
                sensor.OrientationChanged += new TypedEventHandler<SimpleOrientationSensor, SimpleOrientationSensorOrientationChangedEventArgs>(OrientationChanged);
            }
        }

        private async void OrientationChanged(object sender, SimpleOrientationSensorOrientationChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SimpleOrientation orientation = e.Orientation;
                switch (orientation)
                {
                    case SimpleOrientation.NotRotated:
                        Frame.Navigate(typeof(CalculatorPage));
                        break;
                }
            });
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }


        //删除按钮
        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            if (!isErrorInput)
            {
                //当操作数正在输入时，delete按钮才能按
                if (operNumClicking)
                {
                    ResultTextBlockStr = ResultTextBlock.Text;

                    //当结果框的字符串只有一位时，将其设为0
                    if (ResultTextBlockStr.Length == 1)
                    {
                        ResultTextBlock.Text = "0";
                        operNumClicking = false;
                    }
                    else
                    {
                        ResultTextBlock.Text = ResultTextBlockStr.Substring(0, ResultTextBlockStr.Length - 1);
                    }
                }
            }
        }

        //计算器功能按钮事件，包括后退、清理回车、清空、等号
        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            ResultTextBlock.Text = "0";
            ProgressTextBlock.Text = "";

            basicSymbolClicked = false;
            operNumClicking = false;
            equalClicked = false;
            isErrorInput = false;
            basicDouble = 0;
            ResultTextBlockStr = "";
            ProgressTextBlockStr = "";
            itsFraction = 0;
            fractionClickCnt = 0;
            ResultTextBlock.FontSize = 50;//在进行分数计算时，如果数为0时，不能计算。此时的FontSize位50，这里重新将其设置为初始值
            sqrtClickCnt = 0;
            itsSqrt = 0;
            operSymbol = "";
        }

        //数字按钮事件（包括小数点）
        //NOTE：这个地方的实现值得改进，可以根据只通过一个函数来实现所有功能。目前不知道如何在xaml控件中传参数或者辨识哪个按钮触发事件
        //choice表示第几个按钮
        private void ButtonNumberClickHelper(string buttonContent)
        {
            if (!isErrorInput)
            {
                operNumClicking = true;
                sqrtClickCnt = 0;
                fractionClickCnt = 0;

                //运算符已经按过,之后继续按数字键
                if (basicSymbolClicked || equalClicked)
                {
                    basicSymbolClicked = false;
                    equalClicked = false;
                    ResultTextBlock.Text = buttonContent;
                }
                else
                {
                    if (ResultTextBlock.Text != "0")
                        ResultTextBlock.Text += buttonContent;
                    else
                        ResultTextBlock.Text = buttonContent;
                }
            }
        }

        private void Button0_Click(object sender, RoutedEventArgs e)
        {
            ButtonNumberClickHelper("0");
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            ButtonNumberClickHelper("1");
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            ButtonNumberClickHelper("2");
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            ButtonNumberClickHelper("3");
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            ButtonNumberClickHelper("4");
        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            ButtonNumberClickHelper("5");
        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            ButtonNumberClickHelper("6");
        }


        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            ButtonNumberClickHelper("7");
        }

        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            ButtonNumberClickHelper("8");
        }

        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            ButtonNumberClickHelper("9");
        }

        private void ButtonPoint_Click(object sender, RoutedEventArgs e)
        {
            if (!isErrorInput)
            {
                operNumClicking = true;
                sqrtClickCnt = 0;
                fractionClickCnt = 0;

                //运算符已经按过,之后继续按数字键
                if (basicSymbolClicked)
                {
                    basicSymbolClicked = false;
                    ResultTextBlock.Text = "0.";
                }
                else
                {
                    String resultStr = ResultTextBlock.Text;
                    if (!resultStr.Contains("."))
                        ResultTextBlock.Text += ".";
                }
            }
        }

        //基本运算按钮事件，包括加减乘除、mod
        private void ButtonSymbolClickHelper(string symbol)
        {
            if (!isErrorInput)
            {
                operNumClicking = false;
                sqrtClickCnt = 0;
                fractionClickCnt = 0;

                //处理按完运算符按钮后继续按其他的运算符按纽
                if (!basicSymbolClicked)
                {
                    ResultTextBlockStr = ResultTextBlock.Text;

                    //处理连按事件，比如1+2+3，但是sqrt、根号没用考虑在内
                    if (ProgressTextBlock.Text != "" && ProgressTextBlock.Text.IndexOfAny(basicSymbolArray) != -1)
                    {
                        operNum2 = double.Parse(ResultTextBlock.Text); ;
                        ResultTextBlock.Text = Equal(operNum1, operSymbol, operNum2);
                    }
                    else
                        operNum1 = double.Parse(ResultTextBlockStr);

                    operSymbol = symbol;

                    //sqrt、根号不考虑在内
                    int lastSymbolIndex = ProgressTextBlock.Text.LastIndexOfAny(basicSymbolArray);

                    if (lastSymbolIndex != -1)//基本运算符索引在过程框字符串倒数第二个，处理"3 * "这样的情况
                    {
                        if (lastSymbolIndex + 2 < ProgressTextBlock.Text.Length) // 3 * sqrt(3)
                            ProgressTextBlock.Text += symbol + " ";
                        else
                            ProgressTextBlock.Text += ResultTextBlockStr + " " + symbol + " ";
                    }
                    else
                    {
                        if (ProgressTextBlock.Text == "")//处理按了数字键后按运算符键
                            ProgressTextBlock.Text = ResultTextBlockStr + " " + symbol + " ";
                        else
                            ProgressTextBlock.Text += symbol + " ";
                    }
                }
                else
                {
                    int lastSymbolIndex = ProgressTextBlock.Text.Length - 2;
                    string lastSymbol = ProgressTextBlock.Text.Substring(lastSymbolIndex, 1);
                    if (lastSymbol != symbol)//处理3 * 之后按了其他运算符
                    {
                        ProgressTextBlock.Text = ProgressTextBlock.Text.Substring(0, lastSymbolIndex) + symbol + " ";
                    }

                    operSymbol = symbol;
                }

                basicSymbolClicked = true;
            }
        }

        private void ButtonPlus_Click(object sender, RoutedEventArgs e)
        {
            ButtonSymbolClickHelper("+");
        }

        private void ButtonMinus_Click(object sender, RoutedEventArgs e)
        {
            ButtonSymbolClickHelper("-");
        }

        private void ButtonMultiply_Click(object sender, RoutedEventArgs e)
        {
            ButtonSymbolClickHelper("*");
        }

        private void ButtonDivide_Click(object sender, RoutedEventArgs e)
        {
            ButtonSymbolClickHelper("/");
        }

        private void ButtonMod_Click(object sender, RoutedEventArgs e)
        {
            ButtonSymbolClickHelper("%");
        }

        //输出运算数1和运算数2在运算符下的结果
        private string Equal(double num1, string symbol, double num2)
        {
            double result = 0;
            string resultStr = "";

            if (symbol == "+")
            {
                result = num1 + num2;
                resultStr = result.ToString();
                operNum1 = result;
            }
            else if (symbol == "-")
            {
                result = num1 - num2;
                resultStr = result.ToString();
                operNum1 = result;
            }
            else if (symbol == "*")
            {
                result = num1 * num2;
                resultStr = result.ToString();
                operNum1 = result;
            }
            else if (symbol == "/")
            {
                if (num2 == 0)
                {
                    resultStr = "除数不能为0";
                    isErrorInput = true;
                }
                else
                {
                    result = num1 / num2;
                    resultStr = result.ToString();
                    operNum1 = result;
                }
            }
            else if (symbol == "%")
            {
                result = num1 % num2;
                resultStr = result.ToString();
                operNum1 = result;
            }
            else
                resultStr = "结果未定义";

            return resultStr;
        }

        //单运算符按钮事件，包括根号、正负号、分号
        private void ButtonSqrt_Click(object sender, RoutedEventArgs e)
        {
            UniOperSymbolHelper("s");
        }

        private void ButtonFraction_Click(object sender, RoutedEventArgs e)
        {
            UniOperSymbolHelper("f");
        }

        private void ButtonPosiNege_Click(object sender, RoutedEventArgs e)
        {
            if (!isErrorInput)
            {
                operNumClicking = false;

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

        //等号按钮
        private void ButtonEqual_Click(object sender, RoutedEventArgs e)
        {
            if (!isErrorInput && operSymbol != "")
            {
                equalClicked = true;//等号已经按下
                operNum2 = double.Parse(ResultTextBlock.Text);

                ResultTextBlock.Text = Equal(operNum1, operSymbol, operNum2);

                //等号的话过程框可以清空
                ProgressTextBlock.Text = "";

                //初始化一些变量
                sqrtClickCnt = 0;
                basicSymbolClicked = false;
            }
        }

        //单元运算符助手，针对不同的uniOperSymbol处理按钮事件，比如根号、分号
        private void UniOperSymbolHelper(String uniOperSymbol)
        {
            if (!isErrorInput)
            {
                //初始化一些变量
                InitiationHelper(uniOperSymbol);

                //处理不同按键、组合按键的情况
                JudgementHelper(uniOperSymbol);
            }
        }

        //单元运算符的初始化助手
        private void InitiationHelper(String uniOperSymbol)
        {
            operNumClicking = false;

            if (uniOperSymbol == "s")
            {
                sqrtClickCnt++;
                fractionClickCnt = 0;
            }
            else if (uniOperSymbol == "f")
                fractionClickCnt++;

            ResultTextBlockStr = ResultTextBlock.Text;
            ProgressTextBlockStr = ProgressTextBlock.Text;
        }

        //单源运算符的判断分支助手，由过程框助手、结果框助手、其他分支助手
        private void JudgementHelper(String uniOperSymbol)
        {
            if (uniOperSymbol == "s")
            {
                if (ResultTextBlockStr[0] != '-' && ResultTextBlockStr != "无效输入")
                {
                    //第一次按根号按钮得到该数
                    if (sqrtClickCnt == 1)
                    {
                        basicDouble = double.Parse(ResultTextBlockStr);
                        ProgressTextBlockStr = basicDouble.ToString();
                    }
                    //加减乘除Mod基本运算按钮还没有按，只用处理在字符串前面加sqrt
                    if (ProgressTextBlock.Text.IndexOf("s") == 0 || (!basicSymbolClicked && sqrtClickCnt == 1))
                    {
                        //显示过程框内容
                        ShowProgressBlockText(uniOperSymbol);
                    }
                    else
                        ElseHelper(uniOperSymbol);

                    //显示结果框内容
                    ShowResultBlockText(uniOperSymbol);
                }
                else
                {
                    ResultTextBlock.FontSize = 50;
                    ResultTextBlock.Text = "无效输入";
                    isErrorInput = true;
                }

            }
            else if (uniOperSymbol == "f")
            {
                if (ResultTextBlockStr != "0" && ResultTextBlockStr != "除数不能为0")
                {
                    //第一次按分数按钮得到该数的分数
                    if (fractionClickCnt == 1)
                    {
                        basicDouble = double.Parse(ResultTextBlockStr);
                        ProgressTextBlockStr = basicDouble.ToString();
                    }
                    //加减乘除Mod基本运算按钮还没有按，只用处理在字符串前面加reciproc
                    if (ProgressTextBlock.Text.IndexOf("r") == 0 || (!basicSymbolClicked && fractionClickCnt == 1))
                    {
                        //显示过程框内容
                        ShowProgressBlockText(uniOperSymbol);
                    }
                    else
                        ElseHelper(uniOperSymbol);

                    //显示结果框内容
                    ShowResultBlockText(uniOperSymbol);
                }
                else
                {
                    ResultTextBlock.FontSize = 50;
                    ResultTextBlock.Text = "除数不能为0";
                    isErrorInput = true;
                }
            }
        }

        //显示过程框的内容
        private void ShowProgressBlockText(string uniOperSymbol)
        {
            if (ProgressTextBlock.Text.IndexOfAny(basicSymbolArray) != -1)//过程框中有运算过程且有运算符号
            {
                if (uniOperSymbol == "s")
                    ProgressTextBlock.Text += "sqrt(" + ResultTextBlockStr + ")";
                else if (uniOperSymbol == "f")
                    ProgressTextBlock.Text += "reciproc(" + ResultTextBlockStr + ")";
            }
            else
            {
                if (uniOperSymbol == "s")
                    ProgressTextBlock.Text = "sqrt(" + ProgressTextBlockStr + ")";
                else if (uniOperSymbol == "f")
                    ProgressTextBlock.Text = "reciproc(" + ProgressTextBlockStr + ")";
            }
        }

        //显示当前结果框内容
        private void ShowResultBlockText(String uniOperSymbol)
        {
            if (uniOperSymbol == "s")
            {
                basicDouble = double.Parse(ResultTextBlockStr);
                itsSqrt = System.Math.Sqrt(basicDouble);
                ResultTextBlock.Text = itsSqrt.ToString();
            }
            else if (uniOperSymbol == "f")
            {
                itsFraction = 1.0 / basicDouble;

                if (fractionClickCnt % 2 == 1)
                    ResultTextBlock.Text = itsFraction.ToString();
                else
                    ResultTextBlock.Text = basicDouble.ToString();
            }

            basicSymbolClicked = false;
        }

        //单源运算符的判断其他分支助手
        private void ElseHelper(string uniOperSymbol)
        {
            ProgressTextBlockStr = ProgressTextBlock.Text;
            int lastBasicSymbolIndex = ProgressTextBlockStr.LastIndexOfAny(basicSymbolArray);

            //获取之前的过程字符串和基数字符串
            //处理这样的操作:98 + reciproc(98)，当再触发该分号按钮后，变成98 + reciproc(reciproc(98))
            string baseNumberStr = "";
            int firstBasicSymbolIndex = ProgressTextBlockStr.IndexOfAny(basicSymbolArray);

            if (firstBasicSymbolIndex == lastBasicSymbolIndex)//1+
                baseNumberStr = ProgressTextBlockStr.Substring(0, lastBasicSymbolIndex - 1);
            else//1+2+
                baseNumberStr = ResultTextBlock.Text;

            string previousProgressStr = ProgressTextBlockStr.Substring(0, lastBasicSymbolIndex + 1);
            string uniStr = "";

            if (lastBasicSymbolIndex + 2 < ProgressTextBlockStr.Length)//第一次按分号，如字符串ProgressTextBlockStr为"98 +"
            {
                if (uniOperSymbol == "s")
                    uniStr = " sqrt(" + ProgressTextBlockStr.Substring(lastBasicSymbolIndex + 2) + ")";
                else if (uniOperSymbol == "f")
                    uniStr = " reciproc(" + ProgressTextBlockStr.Substring(lastBasicSymbolIndex + 2) + ")";
            }
            else
            {
                if(uniOperSymbol == "s")
                    uniStr = " sqrt(" + baseNumberStr + ")";
                else if(uniOperSymbol == "f")
                    uniStr = " reciproc(" + baseNumberStr + ")";
            }

            ProgressTextBlock.Text = previousProgressStr + uniStr;
        }

        private void ButtonPi_Click(object sender, RoutedEventArgs e)
        {
            ResultTextBlock.Text = "3.14159265358979323846";
        }

        private void ButtonLn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonLog_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
