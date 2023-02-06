using System;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Diagnostics;

/// <summary>
/// 部分参考文档 https://blog.csdn.net/qq_40841733/article/details/81938505
/// </summary>
namespace StringImage_F1
{
    /// <summary>
    /// 播放字符画到控制台
    /// </summary>
    class Program
    {
        /// <summary>
        /// 灰度图文件夹位置
        /// </summary>
        static DirectoryInfo GeryFolder = new DirectoryInfo(@"Resources\ikunGery");             
        /// <summary>
        /// 字符图存储位置
        /// </summary>
        static DirectoryInfo ikunTxtPath = new DirectoryInfo(@"Resources\ikunText\");       
        /// <summary>
        /// 播放速度
        /// </summary>
        private static int frameRate = 29;
        /// <summary>
        /// debug 等待时长/毫秒
        /// </summary>
        public static int debugTimer = 2000;

        /// <summary>
        /// 神秘代码ha
        /// </summary>
        private const  string passkey ="ikun";
        /// <summary>
        /// 打印字符设置
        /// </summary>
        private static readonly string cherSet = "MHYN&KI?7>!:_;. ";

        private static bool GetIkun(string code)
        {            
            return code == passkey;
        }
        /// <summary>
        /// 文件夹里否存有文件
        /// </summary>
        private static bool IsFiles(string moded, DirectoryInfo info)
        {
            FileInfo[] fileInfo = info.GetFiles(moded);
            return fileInfo.Length > 0;
        }
        /// <summary>
        /// 输出文件信息
        /// </summary>        
        private static int PrintFileInfos(string moded,DirectoryInfo infos,int chearTime)
        {
            if (!infos.Exists) return 0;
            FileInfo[] fileInfo = infos.GetFiles(moded);                        
            Thread.Sleep(chearTime);
            foreach (FileInfo item in fileInfo)
            {                
                Console.WriteLine(item.FullName);
            }
            Thread.Sleep(chearTime);Console.Clear();      
            return fileInfo.Length + 1;
        }
        /// <summary>
        /// 入口函数Main
        /// </summary>
        static void Main()
        {
            #region 凑数代码
            if (!GeryFolder.Exists) { GeryFolder.Create();Console.WriteLine("新建文件夹 {0}",GeryFolder.Name); }
            if (!ikunTxtPath.Exists) { ikunTxtPath.Create(); Console.WriteLine("新建文件夹 {0}",ikunTxtPath.Name); }

            Console.WriteLine("输入神秘代码");           
            string key = Console.ReadLine();
            Console.WriteLine(GetIkun(key) ? "真ikun" : "小黑子\r\n再见！");
            if (!GetIkun(key)) {Console.ReadKey(); return; }

            #region 删除文件，运行时刷新字符文件
            //Thread.Sleep(debugTimer);
            //DelFiles("*.txt", ikunTxtPath);
            //Thread.Sleep(debugTimer);
            #endregion

            //判断是否有 .txt 文件
            if (!IsFiles("*.txt",ikunTxtPath))
            {
                //创建
                Console.WriteLine("创建字符画");
                //判断是否有 .jpg 文件
                if (!IsFiles("*.jpg", GeryFolder))
                {
                    Console.WriteLine("灰度图文件为空，创建失败!");
                    Console.ReadKey();
                    return;
                }
                double implementTime = CreateTxt();
                Console.WriteLine("\n创建成功 >> 耗时 {0} 毫秒 约等于 {1} 秒\r", implementTime, (implementTime / 1000).ToString("00.00"));
                Thread.Sleep(debugTimer * 2);
            }                
            #region 开始
            Console.WriteLine("{0} 帧，已经就绪\n开始播放", PrintFileInfos("*.txt", ikunTxtPath, Math.Min(700, debugTimer)));
            Thread.Sleep(debugTimer);
            #endregion
            #endregion            
            double playTime = PlaySequence(frameRate);
            Console.WriteLine("\r\n播放完成 >> 耗时 {0} 毫秒 约等于 {1} 秒", playTime,(playTime/1000).ToString("00.00"));            
            Console.WriteLine("Main Completed!");
            Console.ReadLine();
        }

        #region 核心函数
        /// <summary>
        /// 播放序列帧 .txt
        /// </summary>
        /// <param name="rate">帧率/s</param>
        /// <returns>播放时间</returns>
        private static double PlaySequence(int rate)
        {
            if(!IsFiles("*.txt",ikunTxtPath))
            {
                Console.WriteLine("文件为空!"); 
                return 0.0d;
            }
            FileInfo[] fileInfo = ikunTxtPath.GetFiles("*.txt");
            //单线程
            Stopwatch oTime = new Stopwatch();
            oTime.Start();
            foreach (FileInfo item in fileInfo)
            {
                StreamReader sr = new StreamReader(item.FullName);
                string temp = sr.ReadToEnd();
                Thread.Sleep(rate);

                Console.SetCursorPosition(0, 0);
                Console.Write(temp);//打印
            }
            #region use for loop
            //for (int i = 0; i < fileInfo.Length; i++)
            //{
            //    StreamReader sr = new StreamReader(fileInfo[i].FullName);
            //    string temp = sr.ReadToEnd();
            //    Thread.Sleep(rate);

            //    Console.SetCursorPosition(0, 0);
            //    Console.Write(temp);
            //}
            #endregion
            oTime.Stop();           
            return oTime.Elapsed.TotalMilliseconds;
        }
        /// <summary>
        /// 创建字符图
        /// </summary>
        /// <returns></returns>
        private static double CreateTxt()
        {
            //单线程
            Stopwatch oTime = new Stopwatch();//计时器
                  
            oTime.Start();
            FileInfo[] fileCount = GeryFolder.GetFiles("*.jpg");                   
            for (int i = 0; i < fileCount.Length; i++)
            {                
                string newFileName = Path.GetFileNameWithoutExtension(fileCount[i].FullName);//获取图片名称
                Program p = new Program();

                #region Debug
                string txtSave = Path.Combine(ikunTxtPath.FullName, newFileName) + ".txt";
                Console.WriteLine("创建文件成功 >> {0}", txtSave);
                //Console.WriteLine(p.ImageToChar(fileCount[i].FullName, txtSave));
                #endregion
                p.ImageToChar(fileCount[i].FullName, txtSave);
            }
            oTime.Stop();
            return oTime.Elapsed.TotalMilliseconds;
        }       
        ///<summary>
        ///灰度图转字符
        /// </summary>
        private string ImageToChar(string ImgPath,string TxtPath)
        {
            string Write2Txt = "";
            try
            {               
                char[] CharArr = cherSet.ToArray<char>();
                //Bitmap bim = p.ToGery();//灰度图         
                Bitmap bim = new Bitmap(ImgPath);
                for (int i = 0; i < bim.Height; i += 4)                    
                {
                    for (int j = 0; j < bim.Width; j += 2)
                    {
                        Color origalColor = bim.GetPixel(j, i);
                        int index = (int)((origalColor.R + origalColor.G + origalColor.B) / 768.0f * CharArr.Length);//200 * 3 = 600 / 768 * 16

                        #region Debug
                        //int al = (int)(origalColor.R + origalColor.G + origalColor.B);
                        //Console.WriteLine("R = {0}  G = {1}  B = {2}  Index = {3}  al = {4}\n", origalColor.R, origalColor.G, origalColor.B, index, al);
                        //Console.WriteLine("al = {0}\n ", al);
                        #endregion

                        Write2Txt += CharArr[index];
                        //Console.WriteLine("转换中...");
                    }
                    Write2Txt += "\r\n";
                }
                //存盘
                StreamWriter writer = new StreamWriter(TxtPath);
                writer.Write(Write2Txt);
                writer.Close();
                writer.Dispose();           
            }
            catch (Exception e)
            {
                Console.Write("程序执行出现错误:" + e.ToString());
                Console.ReadKey();                
            }
            return Write2Txt;
        }
        #endregion

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="moded">文件类型 *.type</param>
        /// <param name="folder">文件夹位置</param>
        /// <returns></returns>
        public static int DelFiles(string moded,DirectoryInfo folder)
        {
            if(!folder.Exists||!IsFiles(moded,folder))
            {
                Console.WriteLine("不存在，删除失败");
                return 0;
            }
            FileInfo[] fileCount = folder.GetFiles(moded);
            foreach (FileInfo files in fileCount)
            {
                files.Delete();
                Console.WriteLine("删除文件成功 >> {0}", files.FullName);
            }
            Console.WriteLine("删除 {0} 个文件", fileCount.Length + 1);
            return fileCount.Length;
        }
        ///<summary>
        ///将彩图转成灰度图(不转也行)，或AE调成黑白序列帧(快)
        /// </summary>
        public Bitmap ToGery(string urlGery,string imgPath)
        {
            //单线程
            if (File.Exists(urlGery))
                File.Delete(urlGery);
            Bitmap bim = new Bitmap(imgPath);
            Bitmap bimG = new Bitmap(bim.Width, bim.Height);

            int px = 0;
            for (int i = 0; i < bim.Width; i++)
            {
                for (int j = 0; j < bim.Height; j++)
                {
                    px++;
                    Color c = bim.GetPixel(i, j);
                    int rgb = (int)(c.R * 0.299 + c.G * 0.587 + c.B * 0.114);
                    bimG.SetPixel(i, j, Color.FromArgb(rgb, rgb, rgb));
                    //Console.WriteLine("转换中...像素 {0}\n", px);
                }
            }
            bimG.Save(urlGery);
            return bimG;
        }
    }
}
