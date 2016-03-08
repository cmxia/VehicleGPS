using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Models.Dialog;
using System.Windows;

namespace VehicleGPS.Services.Dialog
{
    public sealed class MessageBoxHelper
    {
        #region 提示消息

        /// <summary>
        /// 弹出提示消息标题为提示，按钮为确定 居中显示
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowMessage(string msg)
        {

            PopupBorder poupB = new PopupBorder(msg, false);
            poupB.Show();
            poupB.poupClosed();

        }
        /// <summary>
        /// 弹出提示消息标题为提示，按钮为确定 
        /// </summary>
        /// <param name="msg">提示消息的内容</param>
        /// <param name="isRDorNot">false表示居中，true表示右下角弹出</param>
        public static void ShowMessage(string msg, bool isRDorNot)
        {

            PopupBorder poupB = new PopupBorder(msg, isRDorNot);
            poupB.Show();
            poupB.poupClosed();

        }
        public static void ShowMessage(string msg, int width, int height)
        {

            PopupBorder poupB = new PopupBorder(msg, width, height, true);
            poupB.Show();
            poupB.poupClosed();

        }
        /// <summary>
        /// 弹出提示消息按钮为确定
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowMessage(string msg, string title)
        {
            ShowMessage(msg, title, MessageBoxButton.OK);
        }

        /// <summary>
        /// 弹出提示消息
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowMessage(string msg, string title, MessageBoxButton buttons)
        {
            MessageBox.Show(msg, title, buttons);
        }

        #endregion
    }
}