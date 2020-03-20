using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace PodcastStaticWebsiteBuilder
{
    public static class PodcastWebsiteBuilder
    {
        static void Main(string[] args)
        {
            string rootDir = Directory.GetCurrentDirectory();
            Console.WriteLine("Hello please type in the name of the input directory " + Environment.NewLine + "(note this must be in the same directory as this program):");
            string inputDir = Console.ReadLine();
            Console.WriteLine("Thank you! Now please type in the name of the output directory "+Environment.NewLine+"(note this must be in the same directory as this program):");
            string outputDir = Console.ReadLine();
            inputDir = MakeValidFileName(inputDir);
            outputDir = MakeValidFileName(outputDir);
            HtmlDocument mainDoc = new HtmlDocument();
            mainDoc.Load(@"template.html");
            HtmlNode postList = mainDoc.GetElementbyId("postList");
            HtmlNode pagesList = mainDoc.GetElementbyId("numberPages");

            if (Directory.Exists(inputDir))
            {
                string[] inDirs = Directory.GetDirectories(inputDir, "*", SearchOption.AllDirectories);
                List<string> dirSort = new List<string>();
                foreach (string dir in inDirs)
                {
                    dirSort.Add(dir);
                    Console.WriteLine("Checking: "+dir);
                }
                dirSort = OrderByAlphaNumeric(dirSort, b => b).ToList();
                dirSort.Reverse();
                int postNum = 1;
                int pageNum = 1;
                // read the input markdown files in each folder and write the html post page to disk and append list item to main list
                foreach (string tim in dirSort)
                {
                    string headerPath = tim+@"\header.MD";
                    string discPath = tim + @"\disc.MD";
                    string bodyPath = tim + @"\body.MD";
                    string linkPath = tim + @"\link.txt";
                    
                    List<string> headerList = File.ReadAllLines(headerPath).ToList();
                    List<string> discList = File.ReadAllLines(discPath).ToList();
                    List<string> bodyList = File.ReadAllLines(bodyPath).ToList();
                    string link = File.ReadAllText(linkPath);
                    string mdHeader = string.Join(System.Environment.NewLine, headerList);
                    string mdDisc = string.Join(System.Environment.NewLine, discList);
                    string mdBody = string.Join(System.Environment.NewLine, bodyList);
                    string htmlHeader = @"<div>" + Markdig.Markdown.ToHtml(mdHeader) + @"</div>";
                    string htmlDisc = @"<div>" + Markdig.Markdown.ToHtml(mdDisc) + @"</div>";
                    string htmlBody = @"<div>" + Markdig.Markdown.ToHtml(mdBody) + @"</div>";

                    HtmlDocument tempDoc = new HtmlDocument();
                    tempDoc.Load(@"postTemplate.html");
                    HtmlNode headerNode = tempDoc.GetElementbyId("postHeader");
                    headerNode.AppendChild(HtmlNode.CreateNode(htmlHeader));
                    HtmlNode bodyNode = tempDoc.GetElementbyId("postContent");
                    bodyNode.AppendChild(HtmlNode.CreateNode(htmlBody));
                    HtmlNode audioNode = tempDoc.GetElementbyId("postAudio");
                    HtmlNode downloadNode = tempDoc.GetElementbyId("audioDownload");
                    audioNode.SetAttributeValue("src", link);
                    downloadNode.SetAttributeValue("href", link);

                    string htmlName = "post" + postNum + ".html";

                    if(postNum %20 == 0 && pageNum != (postNum/20)+1)
                    {
                        pageNum++;
                    }


                    if (postNum % 20 == 0 || postNum == 1)
                    {
                        HtmlNode pageListItem = HtmlNode.CreateNode(@"<li></li>");
                        HtmlNode tempLink = HtmlNode.CreateNode("<a>" + pageNum + @"</a>");
                        tempLink.SetAttributeValue("id", "p" + pageNum);
                        tempLink.SetAttributeValue("href", "#");
                        tempLink.SetAttributeValue("class", "numbered");
                        pageListItem.AppendChild(tempLink);
                        pagesList.AppendChild(pageListItem);
                    }

                    HtmlNode listItem = HtmlNode.CreateNode(@"<li></li>");
                    listItem.SetAttributeValue("id", "post"+postNum);
                    listItem.SetAttributeValue("class", "page"+pageNum);

                    HtmlNode tempLink2 = HtmlNode.CreateNode("<a>" + htmlHeader + @"</a>");
                    tempLink2.SetAttributeValue("id", "link" + postNum);
                    tempLink2.SetAttributeValue("href", htmlName);
                    listItem.AppendChild(tempLink2);

                    HtmlNode tempDisc = HtmlNode.CreateNode("<div>" + htmlDisc + @"</div>");
                    tempDisc.SetAttributeValue("class", "discription");

                    listItem.AppendChild(tempDisc);
                    listItem.AppendChild(HtmlNode.CreateNode(@"<br></br>"));
                   
                    postList.AppendChild(listItem);



                    tempDoc.Save(outputDir + @"\" + htmlName);

                    Console.WriteLine("Reading: "+tim);
                    Console.WriteLine("Writing: "+outputDir+@"\"+htmlName);
                    postNum++;
                }
                Console.WriteLine("Writing: " + outputDir + @"\" + "index.html");
                mainDoc.Save(outputDir + @"\" + "index.html");
            }
            
            Console.ReadLine();
        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        public static IEnumerable<T> OrderByAlphaNumeric<T>(this IEnumerable<T> source, Func<T, string> selector)
        {
            int max = source.SelectMany(i => Regex.Matches(selector(i), @"\d+").Cast<Match>().Select(m => (int?)m.Value.Length)).Max() ?? 0;
            return source.OrderBy(i => Regex.Replace(selector(i), @"\d+", m => m.Value.PadLeft(max, '0')));
        }
    }

}

