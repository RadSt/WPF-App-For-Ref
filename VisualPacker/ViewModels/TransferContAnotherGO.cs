using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using VisualPacker.Models;
using VisualPacker.Views;

namespace VisualPacker.ViewModels
{
    public class TransferContAnotherGO
    {
        //private readonly ObservableCollection<Container> containers = new ObservableCollection<Container>();

        //private void TransferContainer()
        //{
        //    dataGrid1.CommitEdit();
        //    var list = new List<string>();
        //    foreach (var c in containers)
        //    {
        //        if (c.IsChecked) list.Add(c.Name);
        //    }
        //    var inputDialog = new InputDialog();
        //    if (inputDialog.ShowDialog() == true)
        //    {
        //        var loadId = inputDialog.Answer;
        //        textBox.Text = "Протокол переноса контейнеров...";
        //        SendRequestToService(list, loadId);
        //    }
        //    WriteLog("Перенос контейнеров в существующее ГО");
        //}

        //private void SendRequestToService(List<string> list, string loadId)
        //{
        //    var reqString =
        //        "http://localhost/ILSIntegrationServices/ShipmentContainerTransferResource/MoveContainers?containerId=" +
        //        string.Join(",", list.ToArray()) + "&load=" + loadId;
        //    var request = WebRequest.Create(reqString) as HttpWebRequest;
        //    request.Method = "GET";
        //    request.Accept = "application/json";
        //    request.ContentType = "application/json";
        //    request.Headers.Add("UserName:user199");
        //    try
        //    {
        //        var response = request.GetResponse() as HttpWebResponse;
        //        var responseBody = "";
        //        using (var rspStm = response.GetResponseStream())
        //        {
        //            using (var reader = new StreamReader(rspStm))
        //            {
        //                responseBody = reader.ReadToEnd();
        //            }
        //        }
        //        textBox.Text = textBox.Text + "\r\n";
        //        textBox.Text = textBox.Text + "Статус выполнения запроса: " + response.StatusCode;
        //        textBox.Text = textBox.Text + "\r\n";
        //        textBox.Text = textBox.Text + responseBody;
        //    }
        //    catch (WebException ex)
        //    {
        //        textBox.Text = textBox.Text + "Ошибка переноса контейнеров: " + ex.Message;
        //        textBox.Text = textBox.Text + "\n";
        //        textBox.Text = textBox.Text + "Текст запроса: " + reqString;
        //        textBox.Text = textBox.Text + "\r\n";
        //        var reader = new StreamReader(ex.Response.GetResponseStream());
        //        textBox.Text = textBox.Text + reader.ReadToEnd();
        //    }
        //}
    }
}