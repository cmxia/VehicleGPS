using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.IO.Packaging;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Windows.Threading;

namespace VehicleGPS
{
    /// <summary>
    /// PrintPreview.xaml 的交互逻辑
    /// </summary>
    public partial class PrintPreview : Window
    {
        private delegate void LoadXpsMethod();
        private readonly Object m_data;
        private readonly FlowDocument m_doc;

        public static FlowDocument LoadDocumentAndRender(string strTmplName, Object data)
        {
            Uri uri = new Uri("./DispatchDocument.xaml", UriKind.RelativeOrAbsolute);
            FlowDocument doc = (FlowDocument)Application.LoadComponent(uri);
            doc.PagePadding = new Thickness(50);
            doc.DataContext = data;
            return doc;
        }

        public PrintPreview(string strTmplName, Object data)
        {
            InitializeComponent();
            m_data = data;
            m_doc = LoadDocumentAndRender(strTmplName, data);
            Dispatcher.BeginInvoke(new LoadXpsMethod(LoadXps), DispatcherPriority.ApplicationIdle);
        }

        public void LoadXps()
        {
            //构造一个基于内存的xps document
            MemoryStream ms = new MemoryStream();
            Package package = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite);
            Uri DocumentUri = new Uri("pack://InMemoryDocument.xps");
            PackageStore.RemovePackage(DocumentUri);
            PackageStore.AddPackage(DocumentUri, package);
            XpsDocument xpsDocument = new XpsDocument(package, CompressionOption.Fast, DocumentUri.AbsoluteUri);

            //将flow document写入基于内存的xps document中去
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
            writer.Write(((IDocumentPaginatorSource)m_doc).DocumentPaginator);

            //获取这个基于内存的xps document的fixed document
            docViewer.Document = xpsDocument.GetFixedDocumentSequence();

            //关闭基于内存的xps document
            xpsDocument.Close();
        }
    }
}
