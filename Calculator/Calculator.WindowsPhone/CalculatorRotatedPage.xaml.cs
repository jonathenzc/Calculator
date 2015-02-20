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
            //if (!isErrorInput)
            //{
            //    //当操作数正在输入时，delete按钮才能按
            //    if (operNumClicking)
            //    {
            //        ResultTextBlockStr = ResultTextBlock.Text;

            //        //当结果框的字符串只有一位时，将其设为0
            //        if (ResultTextBlockStr.Length == 1)
            //        {
            //            ResultTextBlock.Text = "0";
            //            operNumClicking = false;
            //        }
            //        else
            //        {
            //            ResultTextBlock.Text = ResultTextBlockStr.Substring(0, ResultTextBlockStr.Length - 1);
            //        }
            //    }
            //}
        }
    }
}
