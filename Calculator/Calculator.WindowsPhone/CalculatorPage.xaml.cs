using System;
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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Calculator
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CalculatorPage : Page
    {
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

        //设备旋转感应器
        private SimpleOrientationSensor sensor;

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
            operNumClicking = false;
            equalClicked = false;
            isErrorInput = false;
            basicDouble = 0;
            ResultTextBlockStr = "";
            ProgressTextBlockStr = "";
            itsFraction = 0;
            fractionClickCnt = 0;
            operSymbol = "";

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
                    case SimpleOrientation.Rotated90DegreesCounterclockwise:
                    case SimpleOrientation.Rotated270DegreesCounterclockwise:
                        Frame.Navigate(typeof(CalculatorRotatedPage));
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
            operNumClicking = false;
            equalClicked = false;
            isErrorInput = false;
            basicDouble = 0;
            ResultTextBlockStr = "";
            ProgressTextBlockStr = "";
            itsFraction = 0;
            fractionClickCnt = 0;
            ResultTextBlock.FontSize = 80;//在进行分数计算时，如果数为0时，不能计算。此时的FontSize位50，这里重新将其设置为初始值
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

                //运算符已经按过,之后继续按数字键
                if (basicSymbolClicked || equalClicked)
                {
                    basicSymbolClicked = false;
                    equalClicked = false;
                    ResultTextBlock.Text = buttonContent;
                }
                else
                {
                    if (ResultTextBlock.Text == "0" && ProgressTextBlock.Text == "")
                        ResultTextBlock.Text = buttonContent;
                    else
                    {
                        //按了根号或者分号后，按了数字键
                        if (sqrtClickCnt != 0 || fractionClickCnt != 0)
                        {
                            ResultTextBlock.Text = buttonContent;
                            ProgressTextBlock.Text = "";
                        }
                        else
                            ResultTextBlock.Text += buttonContent;
                    }
                }

                sqrtClickCnt = 0;
                fractionClickCnt = 0;
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
                char[] basicSymbolArray = new char[] { '+', '-', '*', '/', '%' };

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

        //单运算符按钮事件，包括根号、正负号、分号
        private void ButtonRoot_Click(object sender, RoutedEventArgs e)
        {
            if (!isErrorInput)
            {
                operNumClicking = false;
                sqrtClickCnt++;
                ResultTextBlockStr = ResultTextBlock.Text;
                ProgressTextBlockStr = ProgressTextBlock.Text;

                if (ResultTextBlockStr[0] != '-' && ResultTextBlockStr != "无效输入")
                {
                    //第一次按根号按钮得到该数
                    if (sqrtClickCnt == 1)
                    {
                        basicDouble = double.Parse(ResultTextBlockStr);

                        if (ProgressTextBlock.Text == "")
                            ProgressTextBlockStr = basicDouble.ToString();
                        else//按了sqrt后按1/x按钮
                            ProgressTextBlockStr = ProgressTextBlock.Text;
                    }

                    char[] basicSymbolArray = new char[] { '+', '-', '*', '/', '%' };

                    //加减乘除Mod基本运算按钮还没有按，只用处理在字符串前面加sqrt
                    if (ProgressTextBlock.Text.IndexOf("s") == 0 || ProgressTextBlock.Text.IndexOf("r") == 0 || (!basicSymbolClicked && sqrtClickCnt == 1 && fractionClickCnt == 0))
                    {
                        //显示过程框的内容
                        if (ProgressTextBlock.Text.IndexOfAny(basicSymbolArray) != -1)//过程框中有运算过程且有运算符号
                            ProgressTextBlock.Text += " sqrt(" + ResultTextBlockStr + ")";
                        else
                            ProgressTextBlock.Text = "sqrt(" + ProgressTextBlockStr + ")";
                    }
                    else
                    {
                        ProgressTextBlockStr = ProgressTextBlock.Text;
                        int lastBasicSymbolIndex = ProgressTextBlockStr.LastIndexOfAny(basicSymbolArray);

                        //获取之前的过程字符串和基数字符串
                        //处理这样的操作:98 + sqrt(98)，当再触发该分号按钮后，变成98 + sqrt(sqrt(98))
                        //处理1+2+和1+后按根号的情况
                        string baseNumberStr = "";
                        int firstBasicSymbolIndex = ProgressTextBlockStr.IndexOfAny(basicSymbolArray);

                        if(firstBasicSymbolIndex == lastBasicSymbolIndex)//1+
                            baseNumberStr = ProgressTextBlockStr.Substring(0, lastBasicSymbolIndex - 1);
                        else//1+2+
                            baseNumberStr = ResultTextBlock.Text;

                        string previousProgressStr = ProgressTextBlockStr.Substring(0, lastBasicSymbolIndex + 1);
                        string sqrtStr = "";

                        if (lastBasicSymbolIndex + 2 < ProgressTextBlockStr.Length)//第一次按根号，如字符串ProgressTextBlockStr为"98 +"
                            sqrtStr = " sqrt(" + ProgressTextBlockStr.Substring(lastBasicSymbolIndex + 2) + ")";
                        else
                            sqrtStr = " sqrt(" + baseNumberStr + ")";

                        ProgressTextBlock.Text = previousProgressStr + sqrtStr;
                    }

                    //显示当前结果框的内容
                    basicDouble = double.Parse(ResultTextBlockStr);
                    itsSqrt = System.Math.Sqrt(basicDouble);
                    ResultTextBlock.Text = itsSqrt.ToString();
                    basicSymbolClicked = false;
                }
                else
                {
                    ResultTextBlock.FontSize = 50;
                    ResultTextBlock.Text = "无效输入";
                    isErrorInput = true;
                }

                fractionClickCnt = 0;
            }
        }

        private void ButtonFraction_Click(object sender, RoutedEventArgs e)
        {
            if (!isErrorInput)
            {
                operNumClicking = false;
                fractionClickCnt++;
                ResultTextBlockStr = ResultTextBlock.Text;
                ProgressTextBlockStr = ProgressTextBlock.Text;

                if (ResultTextBlockStr != "0" && ResultTextBlockStr != "除数不能为0")
                {
                    //第一次按分数按钮得到该数的分数
                    if (fractionClickCnt == 1)
                    {
                        basicDouble = double.Parse(ResultTextBlockStr);
                        ProgressTextBlockStr = basicDouble.ToString();

                        if (ProgressTextBlock.Text == "")
                            ProgressTextBlockStr = basicDouble.ToString();
                        else//按了1/x后按sqrt按钮
                            ProgressTextBlockStr = ProgressTextBlock.Text;
                    }

                    char[] basicSymbolArray = new char[] { '+', '-', '*', '/', '%' };

                    //加减乘除Mod基本运算按钮还没有按，只用处理在字符串前面加reciproc
                    if (ProgressTextBlock.Text.IndexOf("r") == 0 || ProgressTextBlock.Text.IndexOf("s") == 0 || (!basicSymbolClicked && fractionClickCnt == 1 && sqrtClickCnt == 0))
                    {
                        //显示过程框的内容
                        if (ProgressTextBlock.Text.IndexOfAny(basicSymbolArray) != -1)//过程框中有运算过程且有运算符号
                            ProgressTextBlock.Text += "reciproc(" + ResultTextBlockStr + ")";
                        else
                            ProgressTextBlock.Text = "reciproc(" + ProgressTextBlockStr + ")";
                    }
                    else
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
                        string reciprocStr = "";

                        if (lastBasicSymbolIndex + 2 < ProgressTextBlockStr.Length)//第一次按分号，如字符串ProgressTextBlockStr为"98 +"
                            reciprocStr = " reciproc(" + ProgressTextBlockStr.Substring(lastBasicSymbolIndex + 2) + ")";
                        else
                            reciprocStr = " reciproc(" + baseNumberStr + ")";

                        ProgressTextBlock.Text = previousProgressStr + reciprocStr;
                    }

                    //显示当前结果框的内容
                    itsFraction = 1.0 / basicDouble;

                    if (fractionClickCnt % 2 == 1)
                        ResultTextBlock.Text = itsFraction.ToString();
                    else
                        ResultTextBlock.Text = basicDouble.ToString();

                    basicSymbolClicked = false;
                }
                else
                {
                    ResultTextBlock.FontSize = 50;
                    ResultTextBlock.Text = "除数不能为0";
                    isErrorInput = true;
                }

                sqrtClickCnt = 0;
            }
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

        //等号按钮
        private void ButtonEqual_Click(object sender, RoutedEventArgs e)
        {
            if (!isErrorInput && operSymbol !="")
            {
                equalClicked = true;//等号已经按下
                operNum2 = double.Parse(ResultTextBlock.Text);
                 
                ResultTextBlock.Text = Equal(operNum1, operSymbol, operNum2);

                //等号的话过程框可以清空
                ProgressTextBlock.Text = "";

                //初始化一些变量
                sqrtClickCnt = 0;
                basicSymbolClicked = false;
                fractionClickCnt = 0;
            }
        }

        //输出运算数1和运算数2在运算符下的结果
        private string Equal(double num1,string symbol,double num2)
        {
            double result=0;
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
    }
}
