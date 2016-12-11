using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using XMindAPI.LIB;
namespace outputxmind
{
    class Program
    {

        public class Xmind
        {
            public string title { get; set; }
            public List<string> keyword { get; set; }
            public List<Xmind> content { get; set; }
        }

        public class Movie
        {
            public string Name { get; set; }
            public int Year { get; set; }
        }

        public void XmindOutput(XMindWorkBook XObj,string ParentID,List<Xmind> Xmind_json)
        {
            foreach (Xmind item in Xmind_json)
            {
                string TopicID = XObj.AddTopic(ParentID, item.title);  //创建节点

                if(item.keyword.Count != 0)     //处理标签
                {
                    string LableStr = "";
                    foreach (string str in item.keyword)
                    {
                        LableStr += ",";
                        LableStr += str;
                    }
                    LableStr = LableStr.Remove(0, 1);   //删除第一个逗号
                    XObj.AddLabel(TopicID, LableStr);
                }

                //递归
               if(item.content != null)
               {
                    XmindOutput(XObj, TopicID, item.content);
                }
            }

            return;
        }

        public List<Xmind> Read(string path)
        {
            List<Xmind> myJson = JsonConvert.DeserializeObject<List<Xmind>>(File.ReadAllText(@path));

            //Xmind myJson = JsonConvert.DeserializeObject<Xmind>(data);

            return myJson;
        }

        static void Main(string[] args)
        {
            if (args.Length == 0 )
            {
                Console.WriteLine("File is Empty");
                return;
            }

            string FilePath = args[0];
            string FileMind = FilePath.Replace(".txt", ".xmind");
            Program Obj = new Program();
            XMindWorkBook XObj = new XMindWorkBook(@FileMind);

            if (!File.Exists(FilePath))
            {
                Console.WriteLine("Json is Empty");
                return;
            }
            List<Xmind> Xmind_json = Obj.Read(FilePath);
            
            string sheetID = XObj.AddSheet(Xmind_json[0].title);        //表
            string centralID = XObj.AddCentralTopic(sheetID, Xmind_json[0].title, XMindStructure.Map);  //主题(唯一)

            Obj.XmindOutput(XObj,centralID,Xmind_json[0].content);       //递归添加节点

            XObj.Save();
        }
    }
}
