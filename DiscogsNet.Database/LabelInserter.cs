using MySql.Data.MySqlClient;
using DiscogsNet.Model;

namespace DiscogsNet.Database
{
    public class LabelInserter
    {
        private MySqlConnection conn;
        private InsertBuilder cmdInsertLabel, cmdInsertLabelImage, cmdInsertLabelUrl;

        public LabelInserter(string connectionString)
        {
            conn = new MySqlConnection(connectionString);
            conn.Open();
            Init();
        }

        public LabelInserter(MySqlConnection conn)
        {
            this.conn = conn;
            Init();
        }

        private void Init()
        {
            cmdInsertLabel = new InsertBuilder(conn, "labels")
                .AddString("name")
                .AddString("contactinfo")
                .AddString("profile")
                .AddString("parentlabel")
                .Build();

            cmdInsertLabelImage = new InsertBuilder(conn, "labels_images")
                .AddInt32("label")
                .AddInt32("number")
                .AddString("type")
                .AddInt32("width")
                .AddInt32("height")
                .AddString("uri")
                .AddString("uri150")
                .Build();

            cmdInsertLabelUrl = new InsertBuilder(conn, "labels_urls")
                .AddInt32("label")
                .AddInt32("number")
                .AddString("url")
                .Build();
        }

        private void InsertLabelImages(Label label, long labelId)
        {
            int number = 0;
            foreach (var image in label.Images)
            {
                cmdInsertLabelImage["label"] = labelId;
                cmdInsertLabelImage["number"] = number++;
                cmdInsertLabelImage["type"] = image.Type.ToString();
                cmdInsertLabelImage["width"] = image.Width;
                cmdInsertLabelImage["height"] = image.Width;
                cmdInsertLabelImage["uri"] = image.Uri;
                cmdInsertLabelImage["uri150"] = image.Uri150;
                cmdInsertLabelImage.Execute();
            }
        }

        private void InsertLabelUrls(Label label, long labelId)
        {
            int number = 0;
            foreach (var url in label.Urls)
            {
                cmdInsertLabelUrl["label"] = labelId;
                cmdInsertLabelUrl["number"] = number++;
                cmdInsertLabelUrl["url"] = url;
                cmdInsertLabelUrl.Execute();
            }
        }

        public void Insert(Label label)
        {
            cmdInsertLabel["name"] = label.Name;
            cmdInsertLabel["contactinfo"] = label.ContactInfo ?? "";
            cmdInsertLabel["profile"] = label.Profile ?? "";
            cmdInsertLabel["parentlabel"] = label.ParentLabel ?? "";
            long labelId = cmdInsertLabel.Execute();

            if (label.Images != null)
            {
                InsertLabelImages(label, labelId);
            }
            if (label.Urls != null)
            {
                InsertLabelUrls(label, labelId);
            }
        }
    }
}
