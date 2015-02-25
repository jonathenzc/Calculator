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

        //ln辅助变量
        private int lnClickCnt; //ln按钮点击次数

        //log辅助变量
        private int logClickCnt;//log按钮点击次数

        //powten辅助变量
        private int powtenClickCnt;//10^x按钮点击次数

        //sin辅助变量
        private int sinClickCnt;//sind按钮点击次数

        //cos辅助变量
        private int cosClickCnt;//cosd按钮点击次数

        //tan辅助变量
        private int tanClickCnt;//tand按钮点击次数

        //asind辅助变量
        private int asindClickCnt;//asind按钮点击次数

        //acosd辅助变量
        private int acosdClickCnt;//acosd按钮点击次数

        //atand辅助变量
        private int atandClickCnt;//atand按钮点击次数

        //sqr辅助变量
        private int sqrClickCnt;//sqr按钮点击次数

        //cube辅助变量
        private int cubeClickCnt;//cube按钮点击次数

        //cuberoot辅助变量
        private int cubeRootClickCnt;//cubeRoot按钮点击次数

        public CalculatorRotatedPage()
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
            operSymbol = "";
            
            //单元运算符辅助变量
            fractionClickCnt = 0;
            lnClickCnt = 0;
            logClickCnt = 0;
            powtenClickCnt = 0;
            sinClickCnt = 0;
            cosClickCnt = 0;
            tanClickCnt = 0;
            asindClickCnt = 0;
            acosdClickCnt = 0;
            atandClickCnt = 0;
            sqrClickCnt = 0;
            cubeClickCnt = 0;
            cubeRootClickCnt = 0;

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
            ResultTextBlock.FontSize = 60;//在进行分数计算时，如果数为0时，不能计算。此时的FontSize位60，这里重新将其设置为初始值
            sqrtClickCnt = 0;
            itsSqrt = 0;
            operSymbol = "";

            //单元运算符辅助变量
            fractionClickCnt = 0;
            lnClickCnt = 0;
            logClickCnt = 0;
            powtenClickCnt = 0;
            sinClickCnt = 0;
            cosClickCnt = 0;
            tanClickCnt = 0;
            asindClickCnt = 0;
            acosdClickCnt = 0;
            atandClickCnt = 0;
            sqrClickCnt = 0;
            cubeClickCnt = 0;
            cubeRootClickCnt = 0;
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

        //基本运算按钮事件，包括加减乘除、mod、x^y、y√x
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

        //二元运算符
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

        //x^y次方
        private void ButtonXYSquare_Click(object sender, RoutedEventArgs e)
        {
            ButtonSymbolClickHelper("^");
        }

        //x的y次根
        private void ButtonXYRoot_Click(object sender, RoutedEventArgs e)
        {
            ButtonSymbolClickHelper("yroot");
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
            else if (symbol == "^")//x^y
            {
                if (num1 == 0 && num2 < 0)
                {
                    resultStr = "除数不能为0";
                    isErrorInput = true;
                }
                //else if(num1<0 && )//当num1为负数，num2开根的条件不知道如何判断
                //{
                //    resultStr = "无效输入";
                //    isErrorInput = true;
                //}
                else//合法输入
                {
                    result = Math.Pow(num1,num2);
                    resultStr = result.ToString();
                    operNum1 = result;
                }
            }
            else if (symbol == "yroot")//x的y次根
            {
                if (num2 == 0 || (num1 == 0 && num2 < 0))
                {
                    resultStr = "除数不能为0";
                    isErrorInput = true;
                }
                //else if(num1<0 && )//当num1为负数，num2开根的条件不知道如何判断
                //{
                //    resultStr = "无效输入";
                //    isErrorInput = true;
                //}
                else//合法输入
                {
                    result = Math.Pow(num1, 1.0/num2);
                    resultStr = result.ToString();
                    operNum1 = result;
                }
            }
            else
                resultStr = "结果未定义";

            return resultStr;
        }

        //单运算符按钮事件，包括根号、正负号、分号
        private void ButtonSqrt_Click(object sender, RoutedEventArgs e)
        {
            UniOperSymbolHelper("sqrt");
        }

        private void ButtonFraction_Click(object sender, RoutedEventArgs e)
        {
            UniOperSymbolHelper("reciproc");
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

            if (uniOperSymbol == "sqrt")//sqrt
            {
                sqrtClickCnt++;
                fractionClickCnt = 0;
            }
            else if (uniOperSymbol[0] == 'r')//reciproc
                fractionClickCnt++;
            else if (uniOperSymbol[1] == 'n')//ln
                lnClickCnt++;
            else if (uniOperSymbol == "log")//log
                logClickCnt++;
            else if (uniOperSymbol[0] == 'p')//powten
                powtenClickCnt++;
            else if (uniOperSymbol[1] == 'i')//sin
                sinClickCnt++;
            else if (uniOperSymbol == "cos")//cos
                cosClickCnt++;
            else if (uniOperSymbol[0] == 't')//tan
                tanClickCnt++;
            else if (uniOperSymbol == "asind")//asind
                asindClickCnt++;
            else if (uniOperSymbol == "acosd")//acosd
                acosdClickCnt++;
            else if (uniOperSymbol == "atand")//atand
                atandClickCnt++;
            else if (uniOperSymbol == "sqr")//sqr
                sqrClickCnt++;
            else if (uniOperSymbol == "cube")//cube
                cubeClickCnt++;
            else if (uniOperSymbol == "cuberoot")//cubeRoot
                cubeRootClickCnt++;

            ResultTextBlockStr = ResultTextBlock.Text;
            ProgressTextBlockStr = ProgressTextBlock.Text;
        }

        //单源运算符的判断分支助手，由过程框助手、结果框助手、其他分支助手
        private void JudgementHelper(String uniOperSymbol)
        {
            if (uniOperSymbol == "sqrt")//sqrt
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
                    ResultTextBlock.FontSize = 60;
                    ResultTextBlock.Text = "无效输入";
                    isErrorInput = true;
                }

            }
            else if (uniOperSymbol[0] == 'r')//reciproc
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
                    ResultTextBlock.FontSize = 60;
                    ResultTextBlock.Text = "除数不能为0";
                    isErrorInput = true;
                }
            }
            else if (uniOperSymbol[0] == 'l')//ln或者log
            {
                if (ResultTextBlockStr[0] != '-' && ResultTextBlockStr != "0" && ResultTextBlockStr != "无效输入")
                {
                    //第一次按ln或者log按钮得到该数的分数
                    if (lnClickCnt == 1 || logClickCnt == 1)
                    {
                        basicDouble = double.Parse(ResultTextBlockStr);
                        ProgressTextBlockStr = basicDouble.ToString();
                    }
                    //加减乘除Mod基本运算按钮还没有按，只用处理在字符串前面加ln或log
                    if (ProgressTextBlock.Text.IndexOf("l") == 0 || (!basicSymbolClicked && (lnClickCnt == 1 || logClickCnt == 1)))
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
                    ResultTextBlock.FontSize = 60;
                    ResultTextBlock.Text = "无效输入";
                    isErrorInput = true;
                }
            }
            else if (uniOperSymbol[0] == 'p')//powten
            {
                if ( ResultTextBlockStr != "正无穷大")
                {
                    //第一次按powten按钮得到该数的分数
                    if (powtenClickCnt == 1)
                    {
                        basicDouble = double.Parse(ResultTextBlockStr);
                        ProgressTextBlockStr = basicDouble.ToString();
                    }
                    //加减乘除Mod基本运算按钮还没有按，只用处理在字符串前面加powten
                    if (ProgressTextBlock.Text.IndexOf("p") == 0 || (!basicSymbolClicked && powtenClickCnt == 1))
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
                    ResultTextBlock.FontSize = 60;
                    ResultTextBlock.Text = "正无穷大";
                    isErrorInput = true;
                }
            }
            else if (uniOperSymbol[1] == 'i')//sin
            {
                //第一次按sin按钮得到该数的分数
                if (sinClickCnt == 1)
                {
                    basicDouble = double.Parse(ResultTextBlockStr);
                    ProgressTextBlockStr = basicDouble.ToString();
                }
                //加减乘除Mod基本运算按钮还没有按，只用处理在字符串前面加sind
                if (ProgressTextBlock.Text.IndexOf("s") == 0 || (!basicSymbolClicked && sinClickCnt == 1))
                {
                    //显示过程框内容
                    ShowProgressBlockText(uniOperSymbol);
                }
                else
                    ElseHelper(uniOperSymbol);

                //显示结果框内容
                ShowResultBlockText(uniOperSymbol);
            }
            else if (uniOperSymbol == "cos")//cos
            {
                //第一次按sin按钮得到该数的分数
                if (cosClickCnt == 1)
                {
                    basicDouble = double.Parse(ResultTextBlockStr);
                    ProgressTextBlockStr = basicDouble.ToString();
                }
                //加减乘除Mod基本运算按钮还没有按，只用处理在字符串前面加cosd
                if (ProgressTextBlock.Text.IndexOf("c") == 0 || (!basicSymbolClicked && cosClickCnt == 1))
                {
                    //显示过程框内容
                    ShowProgressBlockText(uniOperSymbol);
                }
                else
                    ElseHelper(uniOperSymbol);

                //显示结果框内容
                ShowResultBlockText(uniOperSymbol);
            }
            else if (uniOperSymbol[0] == 't')//tan
            {
                if (isInTanDomain(double.Parse(ResultTextBlockStr)) && ResultTextBlockStr != "无效输入")
                {
                    //第一次按tan按钮得到该数的分数
                    if (tanClickCnt == 1)
                    {
                        basicDouble = double.Parse(ResultTextBlockStr);
                        ProgressTextBlockStr = basicDouble.ToString();
                    }
                    //加减乘除Mod基本运算按钮还没有按，只用处理在字符串前面加tand
                    if (ProgressTextBlock.Text.IndexOf("t") == 0 || (!basicSymbolClicked && tanClickCnt == 1))
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
                    ResultTextBlock.FontSize = 60;
                    ResultTextBlock.Text = "无效输入";
                    isErrorInput = true;
                }
            }
            else if (uniOperSymbol == "asind" || uniOperSymbol == "acosd")//asind或者acosd
            {
                if (isInArcSinOrCosDomain(double.Parse(ResultTextBlockStr)) && ResultTextBlockStr != "无效输入")
                {
                    //第一次按asin或者acos按钮得到该数的分数
                    if (asindClickCnt == 1 || acosdClickCnt == 1)
                    {
                        basicDouble = double.Parse(ResultTextBlockStr);
                        ProgressTextBlockStr = basicDouble.ToString();
                    }
                    //加减乘除Mod基本运算按钮还没有按，只用处理在字符串前面加asind或者acosd
                    if (ProgressTextBlock.Text.IndexOf("a") == 0 || (!basicSymbolClicked && (asindClickCnt == 1 || acosdClickCnt == 1)))
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
                    ResultTextBlock.FontSize = 60;
                    ResultTextBlock.Text = "无效输入";
                    isErrorInput = true;
                }
            }
            else if (uniOperSymbol == "atand")//atand
            {
                //第一次按atan按钮得到该数的分数
                if (atandClickCnt == 1)
                {
                    basicDouble = double.Parse(ResultTextBlockStr);
                    ProgressTextBlockStr = basicDouble.ToString();
                }
                //加减乘除Mod基本运算按钮还没有按，只用处理在字符串前面加atand
                if (ProgressTextBlock.Text.IndexOf("a") == 0 || (!basicSymbolClicked && atandClickCnt == 1))
                {
                    //显示过程框内容
                    ShowProgressBlockText(uniOperSymbol);
                }
                else
                    ElseHelper(uniOperSymbol);

                //显示结果框内容
                ShowResultBlockText(uniOperSymbol);
            }
            else if (uniOperSymbol == "sqr")//sqr
            {
                //第一次按sqr按钮得到该数的分数
                if (sqrClickCnt == 1)
                {
                    basicDouble = double.Parse(ResultTextBlockStr);
                    ProgressTextBlockStr = basicDouble.ToString();
                }
                //加减乘除Mod基本运算按钮还没有按，只用处理在字符串前面加sqr
                if (ProgressTextBlock.Text.IndexOf("s") == 0 || (!basicSymbolClicked && sqrClickCnt == 1))
                {
                    //显示过程框内容
                    ShowProgressBlockText(uniOperSymbol);
                }
                else
                    ElseHelper(uniOperSymbol);

                //显示结果框内容
                ShowResultBlockText(uniOperSymbol);
            }
            else if (uniOperSymbol == "cube")//cube
            {
                //第一次按cube按钮得到该数的分数
                if (cubeClickCnt == 1)
                {
                    basicDouble = double.Parse(ResultTextBlockStr);
                    ProgressTextBlockStr = basicDouble.ToString();
                }
                //加减乘除Mod基本运算按钮还没有按，只用处理在字符串前面加cube
                if (ProgressTextBlock.Text.IndexOf("c") == 0 || (!basicSymbolClicked && cubeClickCnt == 1))
                {
                    //显示过程框内容
                    ShowProgressBlockText(uniOperSymbol);
                }
                else
                    ElseHelper(uniOperSymbol);

                //显示结果框内容
                ShowResultBlockText(uniOperSymbol);
            }
            else if (uniOperSymbol == "cuberoot")//cuberoot
            {
                //第一次按cuberoot按钮得到该数的分数
                if (cubeRootClickCnt == 1)
                {
                    basicDouble = double.Parse(ResultTextBlockStr);
                    ProgressTextBlockStr = basicDouble.ToString();
                }
                //加减乘除Mod基本运算按钮还没有按，只用处理在字符串前面加cuberoot
                if (ProgressTextBlock.Text.IndexOf("c") == 0 || (!basicSymbolClicked && cubeRootClickCnt == 1))
                {
                    //显示过程框内容
                    ShowProgressBlockText(uniOperSymbol);
                }
                else
                    ElseHelper(uniOperSymbol);

                //显示结果框内容
                ShowResultBlockText(uniOperSymbol);
            }
        }

        //显示过程框的内容
        private void ShowProgressBlockText(string uniOperSymbol)
        {
            if (ProgressTextBlock.Text.IndexOfAny(basicSymbolArray) != -1)//过程框中有运算过程且有运算符号
                ProgressTextBlock.Text += uniOperSymbol+ "(" + ResultTextBlockStr + ")";
            else
                ProgressTextBlock.Text = uniOperSymbol+"(" + ProgressTextBlockStr + ")";
        }

        //显示当前结果框内容
        private void ShowResultBlockText(String uniOperSymbol)
        {
            if (uniOperSymbol == "sqrt")//sqrt
            {
                basicDouble = double.Parse(ResultTextBlockStr);
                itsSqrt = System.Math.Sqrt(basicDouble);
                ResultTextBlock.Text = itsSqrt.ToString();
            }
            else if (uniOperSymbol[0] == 'r')//reciproc
            {
                itsFraction = 1.0 / basicDouble;

                if (fractionClickCnt % 2 == 1)
                    ResultTextBlock.Text = itsFraction.ToString();
                else
                    ResultTextBlock.Text = basicDouble.ToString();
            }
            else if(uniOperSymbol[1] == 'n')//ln
            {
                basicDouble = double.Parse(ResultTextBlockStr);
                ResultTextBlock.Text = System.Math.Log(basicDouble).ToString();
            }
            else if (uniOperSymbol == "log")//log
            {
                basicDouble = double.Parse(ResultTextBlockStr);
                ResultTextBlock.Text = System.Math.Log(basicDouble,10).ToString();
            }
            else if (uniOperSymbol[0] == 'p')//powten
            {
                basicDouble = double.Parse(ResultTextBlockStr);
                ResultTextBlock.Text = System.Math.Pow(10,basicDouble).ToString();
            }
            else if (uniOperSymbol[1] == 'i')//sin
            {
                basicDouble = double.Parse(ResultTextBlockStr);
                ResultTextBlock.Text = Math.Round(Math.Sin(Math.PI*(basicDouble/180.0)),9).ToString();
            }
            else if (uniOperSymbol == "cos")//cos
            {
                basicDouble = double.Parse(ResultTextBlockStr);
                ResultTextBlock.Text = Math.Round(Math.Cos(Math.PI * (basicDouble / 180.0)), 9).ToString();
            }
            else if (uniOperSymbol[0] == 't')//tan
            {
                basicDouble = double.Parse(ResultTextBlockStr);
                ResultTextBlock.Text = Math.Round(Math.Tan(Math.PI * (basicDouble / 180.0)), 9).ToString();
            }
            else if (uniOperSymbol == "asind")//asind
            {
                basicDouble = double.Parse(ResultTextBlockStr);
                ResultTextBlock.Text = Math.Round(180*Math.Asin(basicDouble)/Math.PI,9).ToString();
            }
            else if (uniOperSymbol == "acosd")//acosd
            {
                basicDouble = double.Parse(ResultTextBlockStr);
                ResultTextBlock.Text = Math.Round(180 * Math.Acos(basicDouble) / Math.PI,9).ToString();
            }
            else if (uniOperSymbol == "atand")//atand
            {
                basicDouble = double.Parse(ResultTextBlockStr);
                ResultTextBlock.Text = Math.Round(180 * Math.Atan(basicDouble) / Math.PI,9).ToString();
            }
            else if (uniOperSymbol == "sqr")//sqr
            {
                basicDouble = double.Parse(ResultTextBlockStr);
                ResultTextBlock.Text = Math.Pow(basicDouble,2).ToString();
            }
            else if (uniOperSymbol == "cube")//cube
            {
                basicDouble = double.Parse(ResultTextBlockStr);
                ResultTextBlock.Text = Math.Pow(basicDouble, 3).ToString();
            }
            else if (uniOperSymbol == "cuberoot")//cubeRoot
            {
                basicDouble = double.Parse(ResultTextBlockStr);
                ResultTextBlock.Text = Math.Pow(basicDouble, 1.0/3.0).ToString();
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
                uniStr = " " + uniOperSymbol + "(" + ProgressTextBlockStr.Substring(lastBasicSymbolIndex + 2) + ")";
            else
                uniStr = " " + uniOperSymbol + "(" + baseNumberStr + ")";

            ProgressTextBlock.Text = previousProgressStr + uniStr;
        }

        //tan判断助手,tan的定义域为x≠kπ+π/2
        private bool isInTanDomain(double num)
        {
            if ((num - 90) % 180 != 0)
                return true;
            else
                return false;
        }

        //反三角函数判断助手，asind的定义域为[-1,1]
        private bool isInArcSinOrCosDomain(double num)
        {
            if (num >= -1 && num <= 1)
                return true;
            else
                return false;
        }

        private void ButtonLn_Click(object sender, RoutedEventArgs e)
        {
            UniOperSymbolHelper("ln");
        }

        private void ButtonLog_Click(object sender, RoutedEventArgs e)
        {
            UniOperSymbolHelper("log");
        }

        private void ButtonPowTen_Click(object sender, RoutedEventArgs e)
        {
            UniOperSymbolHelper("powten");
        }

        private void ButtonSin_Click(object sender, RoutedEventArgs e)
        {
            UniOperSymbolHelper("sind");
        }

        private void ButtonCos_Click(object sender, RoutedEventArgs e)
        {
            UniOperSymbolHelper("cosd");
        }

        private void ButtonTan_Click(object sender, RoutedEventArgs e)
        {
            UniOperSymbolHelper("tand");
        }

        private void ButtonSinh_Click(object sender, RoutedEventArgs e)
        {
            UniOperSymbolHelper("asind");
        }

        private void ButtonCosh_Click(object sender, RoutedEventArgs e)
        {
            UniOperSymbolHelper("acosd");
        }

        private void ButtonTanh_Click(object sender, RoutedEventArgs e)
        {
            UniOperSymbolHelper("atand");
        }

        private void ButtonSquare_Click(object sender, RoutedEventArgs e)
        {
            UniOperSymbolHelper("sqr");
        }

        private void ButtonCube_Click(object sender, RoutedEventArgs e)
        {
            UniOperSymbolHelper("cube");
        }

        private void ButtonCubeRoot_Click(object sender, RoutedEventArgs e)
        {
            UniOperSymbolHelper("cuberoot");
        }

        //常数按钮
        private void ButtonE_Click(object sender, RoutedEventArgs e)
        {
            ResultTextBlock.Text = System.Math.E.ToString();//"2.71828182845905"
        }

        private void ButtonPi_Click(object sender, RoutedEventArgs e)
        {
            ResultTextBlock.Text = System.Math.PI.ToString();//"3.14159265358979323846"
        }
    }
}
