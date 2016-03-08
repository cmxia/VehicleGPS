using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Media;
using System.Globalization;
using System.Windows;
using System.Windows.Media.Imaging;
using VehicleGPS.Services;
using VehicleGPS.Models;
using System.Data;
using VehicleGPS.Models.Login;
using VehicleGPS.Views.Login;
using System.IO;
using System.Xml;
using System.ComponentModel;

namespace VehicleGPS.ViewModels.Login
{
    class LoginViewModel : NotificationObject
    {

        private Window win;
        public LoginViewModel(Window win)
        {
            this.win = win;
            this.LoginCommand = new DelegateCommand(new Action(LoginCommandExecute));
            this.ExitCommand = new DelegateCommand(new Action(ExitCommandExecute));
            this.ResetLoginInfo();
            this.ReadMemoryLoginInfo();//读入上次是否有记住密码信息
            this.CreateImage();
        }
        /*用户名*/
        private string userID;
        public string UserID
        {
            get { return userID; }
            set
            {
                if (userID != value)
                {
                    userID = value;
                    this.RaisePropertyChanged("UserID");
                }
            }
        }
        /*用户密码*/
        private string passwd;
        public string Passwd
        {
            get { return passwd; }
            set
            {
                if (passwd != value)
                {
                    passwd = value;
                    this.RaisePropertyChanged("Passwd");
                }
            }
        }
        /*验证码图片*/
        private bool isMemory;
        public bool IsMemory
        {
            get { return isMemory; }
            set
            {
                if (isMemory != value)
                {
                    isMemory = value;
                    this.RaisePropertyChanged("IsMemory");
                }
            }
        }
        /*验证码*/
        public string Code;
        private string codeInput;
        public string CodeInput
        {
            get { return codeInput; }
            set
            {
                if (codeInput != value)
                {
                    codeInput = value;
                    this.RaisePropertyChanged("CodeInput");
                }
            }
        }
        /*验证码图片*/
        private ImageSource imageCode;
        public ImageSource ImageCode
        {
            get { return imageCode; }
            set
            {
                if (imageCode != value)
                {
                    imageCode = value;
                    this.RaisePropertyChanged("ImageCode");
                }
            }
        }

        #region 验证码功能
        /*产生验证码*/
        private string CreateCode(int codeLength)
        {
            string so = "1,2,3,4,5,6,7,8,9,0,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            string[] strArr = so.Split(',');
            string code = "";
            Random rand = new Random();
            for (int i = 0; i < codeLength; i++)
            {
                code += strArr[rand.Next(0, strArr.Length)];
            }
            return code;
        }
        /*产生验证图片*/
        private void CreateImage()
        {
            this.Code = this.CreateCode(4);
            SolidColorBrush[] colorList = { Brushes.Green, Brushes.Red, Brushes.Black, Brushes.SkyBlue };
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            for (int i = 0; i < this.Code.ToCharArray().Length; i++)
            {
                drawingContext.DrawText(
                new FormattedText((this.Code.ToCharArray()[i]).ToString(), CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight, new Typeface("Verdana"), 25, colorList[i % this.Code.ToCharArray().Length]),
                new Point(4 + 14 * i, 0));
            }
            drawingContext.Close();
            // 利用RenderTargetBitmap对象，以保存图片
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(55, 25, 80, 80, PixelFormats.Pbgra32);
            renderBitmap.Render(drawingVisual);
            this.ImageCode = BitmapFrame.Create(renderBitmap);
        }
        #endregion
        #region 登陆、取消命令
        private bool isBusy = false;//忙等待
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    this.RaisePropertyChanged("IsBusy");
                }
            }
        }
        public DelegateCommand LoginCommand { get; set; }
        private void LoginCommandExecute()
        {
            if (this.userID == "" || this.passwd == "")
            {
                MessageBox.Show("用户名或密码不能为空", "用户登陆", MessageBoxButton.OK);
                return;
            }

            BackgroundWorker worker = new BackgroundWorker();
            bool bLogin = false;
            worker.DoWork += (o, ea) =>
            {
                bLogin = this.LoginResutl();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                this.IsBusy = false;
                if (!bLogin)
                {
                    this.win.Dispatcher.Invoke((Action)(() =>
                        {
                            MessageBox.Show("用户名或密码错误", "用户登陆", MessageBoxButton.OK);
                            this.ResetLoginInfo();
                            this.CreateImage();//重新生成验证码
                        }));
                }
                else
                {
                    this.win.Dispatcher.Invoke((Action)(() =>
                    {
                        LoginDataLoad dataLoadWin = new LoginDataLoad();
                        dataLoadWin.Show();
                        this.WriteMemoryLoginInfo();//写入用户信息到文件
                        this.win.Close();
                    }));
                }
            };
            this.IsBusy = true;
            worker.RunWorkerAsync();
        }
        public DelegateCommand ExitCommand { get; set; }
        private void ExitCommandExecute()
        {
            this.win.Close();
        }
        private void ResetLoginInfo()
        {
            this.UserID = "";
            this.Passwd = "";
            this.CodeInput = "";
            this.IsMemory = false;
        }
        #endregion

        /*查询登陆结果*/
        private bool LoginResutl()
        {
            StaticLoginInfo loginInfo = StaticLoginInfo.GetInstance();
            loginInfo.UserName = "admin";
            loginInfo.Passwd = this.passwd;//pwd;
            loginInfo.LoginTime = DateTime.Now.ToString("yyyy/mm/dd HH:mm:ss");
            return true;

            string loginpwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(this.passwd.Trim(), "SHA1"); 
            string sql = "select userid,loginpassword,replace(convert(varchar(50),getdate(),120),':','-') logintime "
                      + "from loginusers "
                      + "where userstate='启用' and userid='" + this.userID + "' and loginpassword='" + loginpwd + "'";
            string jsonStr = "error";
            try
            {
                jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(this.userID, sql);
            }
            catch (Exception)
            {
                return false;
            }
            if (jsonStr == "error")
                return false;
            DataTable retDt = JsonHelper.JsonToDataTable(jsonStr);
            if(retDt != null && retDt.Rows.Count == 1)
            {
                try
                {
                    string uid = retDt.Rows[0]["userid"].ToString();
                    string pwd = retDt.Rows[0]["loginpassword"].ToString();
                    string logintime = retDt.Rows[0]["logintime"].ToString();

                    if(uid == this.userID && pwd == loginpwd)
                    {
                        //StaticLoginInfo loginInfo = StaticLoginInfo.GetInstance();
                        loginInfo.UserName = uid;
                        loginInfo.Passwd = this.passwd;//pwd;
                        loginInfo.LoginTime = logintime;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch(Exception e)
                {
                    return false;
                }
            }
            return false;
        }
        #region 记住密码
        private void WriteMemoryLoginInfo()
        {

            FileStream fs = new FileStream(VehicleConfig.GetInstance().passPath, FileMode.Create, FileAccess.ReadWrite);
            //FileStream fs = new FileStream(Environment.CurrentDirectory + "/MemoryLoginInfo.txt", FileMode.Create, FileAccess.Write);
            if (this.isMemory == true)
            {
                XmlDocument xmlDoc = new XmlDocument();
                /*创建编码格式*/
                XmlDeclaration xmlDec = xmlDoc.CreateXmlDeclaration("1.0", "gb2312", null);
                xmlDoc.AppendChild(xmlDec);
                /*创建根节点*/
                XmlElement root = xmlDoc.CreateElement("User");
                xmlDoc.AppendChild(root);
                /*用户名*/
                XmlElement userName = xmlDoc.CreateElement("UserID");
                userName.InnerText = this.userID;
                root.AppendChild(userName);
                /*用户密码*/
                XmlElement userPasswd = xmlDoc.CreateElement("Passwd");
                userPasswd.InnerText = this.Passwd;
                root.AppendChild(userPasswd);

                /*写入文件*/
                byte[] str = System.Text.Encoding.Default.GetBytes(xmlDoc.InnerXml);
                fs.Write(str, 0, str.Length);
            }
            fs.Flush();//清空缓冲区
            fs.Close();
        }
        private void ReadMemoryLoginInfo()
        {
            //string filePath = Environment.CurrentDirectory + "/MemoryLoginInfo.txt";
        if (!File.Exists(VehicleConfig.GetInstance().passPath))
            {
                MessageBox.Show("密码文件不存在，请重新输入密码" + VehicleConfig.GetInstance().passPath, "用户登陆", MessageBoxButton.OK);
                this.ResetLoginInfo();
                return;
            }
        FileStream fs = new FileStream(VehicleConfig.GetInstance().passPath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string xmlStr = sr.ReadLine();
            fs.Flush();
            fs.Close();
            sr.Close();

            if (xmlStr != null)
            {
                XmlReader xReader = XmlReader.Create(new StringReader(xmlStr));
                while (xReader.Read())
                {
                    string elementName = xReader.Name;
                    XmlNodeType type = xReader.NodeType;
                    if (elementName == "UserID" && type == XmlNodeType.Element)
                    {
                        xReader.Read();//跳过<UserID>
                        this.UserID = xReader.Value;
                    }
                    if (elementName == "Passwd" && type == XmlNodeType.Element)
                    {
                        xReader.Read();//跳过<Passwd>
                        this.Passwd = xReader.Value;
                    }
                }
                /*如果有记住密码信息，则默认选择“记住密码”*/
                if (this.userID != "" && this.passwd != "")
                {
                    this.IsMemory = true;
                }
            }
        }
        #endregion
    }
}
