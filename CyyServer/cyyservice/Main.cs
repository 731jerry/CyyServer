using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using LitJson;
using OCAPIDemo;
using System.Threading;

namespace CyyService
{
    public partial class Main : Form
    {
        System.Threading.Timer updateCPDataTTimerSD, updateCPDataTTimerGD, updateCPDataTTimerJX, updateCPDataTTimerCQ;
        int updateCPDataTTimerInterval = 20 * 1000;

        public static Main _form = null;
        public Main()
        {
            InitializeComponent();
            //updateCPDataTTimerSD = new System.Threading.Timer(new System.Threading.TimerCallback(UpdateCPWebDataThreadSD), null, 0, updateCPDataTTimerInterval);
            //updateCPDataTTimerGD = new System.Threading.Timer(new System.Threading.TimerCallback(UpdateCPWebDataThreadGD), null, 0, updateCPDataTTimerInterval);
            //updateCPDataTTimerJX = new System.Threading.Timer(new System.Threading.TimerCallback(UpdateCPWebDataThreadJX), null, 0, updateCPDataTTimerInterval);
            //updateCPDataTTimerCQ = new System.Threading.Timer(new System.Threading.TimerCallback(UpdateCPWebDataThreadCQ), null, 0, updateCPDataTTimerInterval);
        }

        //private const string sqlConnectionCommand = @"server=120.27.30.10; user id=admin; password=admin; database=cyydb;Charset=utf8";
        private const string sqlConnectionCommand = @"server=127.0.0.1; user id=root; password=; database=cyydb;Charset=utf8";
        private MySqlConnection sqlConnection = new MySqlConnection(sqlConnectionCommand);

        private bool isServerStarted = false;
        private int updateOnlineUserErrorCount = 0;
        private int getOnTimeCpDataErrorCount = 0;
        #region 自动暂停
        /*
        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public const int WM_CLOSE = 0x10;

        private void StartKiller()
        {
            Timer timer = new Timer();
            timer.Interval = 3000;//10秒启动
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            KillMessageBox();
            //停止计时器
            ((Timer)sender).Stop();
        }

        private void KillMessageBox()
        {
            //查找MessageBox的弹出窗口,注意对应标题
            IntPtr ptr = FindWindow(null, "MessageBox");
            if (ptr != IntPtr.Zero)
            {
                //查找到窗口则关闭
                PostMessage(ptr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
        }

        private void KillAutoRestartErrorMessageBox(string title)
        {
            IntPtr ptr = FindWindow(null, title);
            if (ptr != IntPtr.Zero)
            {
                //查找到窗口则关闭
                PostMessage(ptr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
        }
        */
        #endregion

        // 打开连接
        private void DBOpen()
        {
            try
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }
            }
            catch (Exception ex)
            {
                StopService();
                //KillAutoRestartErrorMessageBox("无法打开连接！");
                RecordLog("无法打开连接!\r\nTargetSite: " + ex.TargetSite + "\r\n" + ex.ToString());
                MessageBox.Show(ex.Message, "无法打开连接！");
                RecordLog(BasicFeature.SendMail("[错误]彩盈盈后台服务管理系统", "", "无法打开连接 已暂停后台服务！<br>DBOpen()" + ex.ToString(), true));
                return;
            }
        }

        // 关闭连接
        public void DBClose()
        {
            sqlConnection.Close();
        }


        private void Main_Load(object sender, EventArgs e)
        {
            StartService(); // 开启服务
        }

        // 开启服务
        private void StartService()
        {
            manProgressLabel.Text = "服务已开启";
            isServerStarted = true;

            updatePasswordCheckBox.Enabled = false;
            UpdateUserPasswordHour.Enabled = false;
            duplicateOnlineUserCheckBox.Enabled = false;
            duplicateOnlineUserTextBox.Enabled = false;
            cpDataOntimeCheckBox.Enabled = false;
            GetDataFromWebCheckBox.Enabled = false;

            UpdateUserPasswordButton.Enabled = true;
            ClearUserOnlineInfoButton.Enabled = true;
            GetCPDataManualButton.Enabled = true;
            GetBigCPDataManualButton.Enabled = true;

            controlButton.Text = "暂 停";

            if (updatePasswordCheckBox.Checked)
            {
                updateUserPasswordTimer.Enabled = true;
            }
            if (duplicateOnlineUserCheckBox.Checked)
            {
                UpdateUserOnlineInfoTimer.Enabled = true;
            }
            if (cpDataOntimeCheckBox.Checked)
            {
                updateCPDataTimer.Enabled = true;
                updateCPDataBigTimer.Enabled = true;
            }
        }

        // 关闭服务
        private void StopService()
        {
            DBClose();
            //初始化
            updateOnlineUserErrorCount = 0;
            getOnTimeCpDataErrorCount = 0;

            manProgressLabel.Text = "服务已暂停";
            isServerStarted = false;

            updatePasswordCheckBox.Enabled = true;
            UpdateUserPasswordHour.Enabled = true;
            duplicateOnlineUserCheckBox.Enabled = true;
            duplicateOnlineUserTextBox.Enabled = true;
            cpDataOntimeCheckBox.Enabled = true;
            GetDataFromWebCheckBox.Enabled = true;

            UpdateUserPasswordButton.Enabled = false;
            ClearUserOnlineInfoButton.Enabled = false;
            GetCPDataManualButton.Enabled = false;
            GetBigCPDataManualButton.Enabled = false;

            controlButton.Text = "开 启";

            updateUserPasswordTimer.Enabled = false;
            UpdateUserOnlineInfoTimer.Enabled = false;
            updateCPDataTimer.Enabled = false;
            updateCPDataBigTimer.Enabled = false;
        }

        private void UpdateUserPasswordButton_Click(object sender, EventArgs e)
        {
            updateUserPasswordFun();
            manProgressLabel.Text = "更新成功";
        }

        private void updateUserPasswordTimer_Tick(object sender, EventArgs e)
        {
            //now.ToString("HH:mm:ss");
            // 1000 * 60 = 1min 每10分钟检测 每天12点运行一次
            if ((DateTime.Now.Hour == int.Parse(UpdateUserPasswordHour.Text)) && DateTime.Now.Minute == 0)
            {
                updateUserPasswordTimer.Enabled = false;
                updateUserPasswordFun();
            }
        }

        private void updateUserPasswordFun()
        {
            try
            {
                RecordLog("开始更新普通会员密码\n");
                string randompassword = generateRandomPassword();

                string SQLforGeneral = @"UPDATE cyy_lngpack SET langstr = '" + randompassword + @"' WHERE  lpid= '857';";

                MD5 md5Hash = MD5.Create();
                string hashRandompassword = GetMd5Hash(md5Hash, randompassword).ToLower();

                string SQLforNormalUsers = @"UPDATE ecs_users SET password = '" + hashRandompassword + @"' WHERE  degreeid= '1';";

                string SQLcommand = SQLforGeneral + "\r\n" + SQLforNormalUsers;

                DBOpen();
                MySqlCommand cmd = new MySqlCommand(SQLcommand, sqlConnection);
                cmd.ExecuteNonQuery();

                manProgressLabel.Text = "基础版用户动态密码更新成功!";
                //MessageBox.Show("基础版用户动态密码更新成功!", "提示");
                RecordLog("***更新普通用户密码 updateUserPasswordTimer_Tick\n");
                RecordLog(BasicFeature.SendMail("[恭喜]彩盈盈后台服务管理系统", "", "更新普通用户密码！<br>当前密码:" + randompassword, true));
                DBClose();
            }
            catch (Exception e)
            {
                DBClose();

                //StopService();
                //lanuchRestartService();
                RecordLog("更新普通会员密码出错\r\nTargetSite: " + e.TargetSite + "\r\n" + e.ToString());
                MessageBox.Show(e.Message, "更新普通会员密码出错");
                RecordLog(BasicFeature.SendMail("[错误]彩盈盈后台服务管理系统", "", "更新普通会员密码出错！<br>updateUserPasswordFun()<br>" + e.ToString(), true));

            }
        }

        // 产生随机密码 
        // 格式： C 6位数字 YY
        private string generateRandomPassword()
        {
            int randomN = generateRandomIntBybit(6);
            return "C" + randomN.ToString() + "YY";
        }

        private int generateRandomIntBybit(int bit)
        {
            Random rNumber = new Random();
            int randomN = rNumber.Next((int)Math.Pow(10, (bit - 1)), (int)(Math.Pow(10, bit) - 1));
            return randomN;
        }
        public string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // 每
        private void UpdateUserOnlineInfoTimer_Tick(object sender, EventArgs e)
        {
            updateOnlineUserInfo(int.Parse(duplicateOnlineUserTextBox.Text));
        }

        private void ClearUserOnlineInfoButton_Click(object sender, EventArgs e)
        {
            updateOnlineUserInfo(int.Parse(duplicateOnlineUserTextBox.Text));
            manProgressLabel.Text = "清理成功";
        }

        private void updateOnlineUserInfo(int delaySeconds)
        {
            try
            {
                /*
                string SQL = @"SELECT
                            CASE
                                WHEN ISNULL(timestampdiff(min, UserLogined_DaySave,now())) = 1 THEN 0
                                ELSE 1
                            END as diff, mid FROM cyy_OnlinesUsers";
                 */
                DBOpen();
                string SQL = @"SELECT TIMESTAMPDIFF( 
                        SECOND , UserLogined_DaySave, NOW() ) AS diff, mid
                        FROM cyy_OnlinesUsers";
                MySqlCommand cmdDiffAll = new MySqlCommand(SQL, sqlConnection);
                MySqlDataReader dataReaderDiffAll = cmdDiffAll.ExecuteReader();

                List<int> midList = new List<int>();
                while (dataReaderDiffAll.Read())
                {
                    int aa = int.Parse(dataReaderDiffAll["diff"].ToString());
                    if (int.Parse(dataReaderDiffAll["diff"].ToString()) > delaySeconds)
                    {
                        midList.Add(int.Parse(dataReaderDiffAll["mid"].ToString()));
                    }
                }

                dataReaderDiffAll.Close();

                foreach (int i in midList)
                {
                    string SQL2 = @"DELETE FROM cyy_OnlinesUsers WHERE mid = " + @"'" + i.ToString() + @"'";

                    MySqlCommand cmd3 = new MySqlCommand(SQL2, sqlConnection);
                    cmd3.ExecuteNonQuery();

                }

                DBClose();
            }

            catch (Exception e)
            {
                DBClose();
                updateOnlineUserErrorCount++;
                string title = "更新会员数据时出错";
                //KillAutoRestartErrorMessageBox(title);

                //Application.Exit();
                //StopService();
                //lanuchRestartService();
                RecordLog(title + "\r\nTargetSite: " + e.TargetSite + "\r\n" + e.ToString());
                MessageBox.Show(e.Message, title);
                if (updateOnlineUserErrorCount == 4)
                {
                    StopService();
                    RecordLog(BasicFeature.SendMail("[错误]彩盈盈后台服务管理系统", "", "多次尝试更新会员数据时出错 已暂停后台服务！<br>updateOnlineUserInfo()<br>" + e.ToString(), true));
                }
                //MessageBox.Show("", "更新会员数据时出错, 已关闭服务");
            }
        }

        public void SoundPlay(string filename)
        {
            System.Media.SoundPlayer media = new System.Media.SoundPlayer(filename);
            media.Play();
        }

        private int restartTime;
        private void lanuchRestartService()
        {
            restartTime = 6; // 5秒
            RestartServiceTimer.Enabled = true;
        }

        private void RestartServiceTimer_Tick(object sender, EventArgs e)
        {
            /*
            if (isWindowLoaded)
            {
                RestartServiceTimer.Enabled = false;
                DBOpen();
            }
            else
            {
             */
            this.Text = "服务已关闭 将于[" + (restartTime - 1) + "秒]后重启";
            restartTime--;
            if (restartTime == 0)
            {
                this.Text = "彩盈盈管理系统";
                RestartServiceTimer.Enabled = false;

                StartService();
                DBOpen();
            }
            //}
        }

        public void RecordLog(string log)
        {
            string fileName = System.Environment.CurrentDirectory + @"\config\log.txt";
            string fileString = "";
            if (System.IO.File.Exists(fileName))
            {
                using (System.IO.FileStream fszz = System.IO.File.OpenRead(fileName))
                {
                    byte[] bytes = new byte[fszz.Length];
                    fszz.Read(bytes, 0, bytes.Length);

                    fileString = Encoding.UTF8.GetString(bytes);
                }
            }
            using (System.IO.FileStream fs = System.IO.File.Create(fileName))
            {
                StringBuilder sb = new StringBuilder();
                byte[] info = new UTF8Encoding().GetBytes(DateTime.Now + "   " + log + "\r\n" + fileString);
                fs.Write(info, 0, info.Length);
            }
        }

        private void controlButton_Click(object sender, EventArgs e)
        {
            if (isServerStarted)
            {
                StopService();
            }
            else
            {
                StartService();
            }
        }

        #region 彩票
        // 彩票数据
        /*
         山东1
         广东2
         江西3
         重庆4
         江苏5
         浙江6
         上海7
         */
        private void GetCPDataWebTimerCallback(object sender)
        {
            int min = DateTime.Now.Minute % 10;
            if ((min == 5) || (min == 0))
            {
                Console.Out.WriteLine(DateTime.Now + " " + DateTime.Now.Millisecond.ToString() + "timer in:");
                //GetCPDataWeb();
            }
            //updateCPDataTTimer.Change(System.Threading.Timeout.Infinite, updateCPDataTTimerInterval);
        }
        private void GetCPDataOnlineOpencai()
        {
            UpdateCPOnlineOpencaiData("http://f.opencai.net/sd11x5-1.json", 1, "山东");
            //UpdateCPOnlineData("http://c.opencai.net/Y45eEPnwqUgaRRgKk1r/index.json", 2, "广东");
            UpdateCPOnlineOpencaiData("http://f.opencai.net/gd11x5-1.json", 2, "广东");
            UpdateCPOnlineOpencaiData("http://f.opencai.net/jx11x5-1.json", 3, "江西");
            UpdateCPOnlineOpencaiData("http://f.opencai.net/cq11x5-1.json", 4, "重庆");
            UpdateCPOnlineOpencaiData("http://f.opencai.net/js11x5-1.json", 5, "江苏");
            UpdateCPOnlineOpencaiData("http://f.opencai.net/zj11x5-1.json", 6, "浙江");
            UpdateCPOnlineOpencaiData("http://f.opencai.net/sh11x5-1.json", 7, "上海");
            manProgressLabel.Text = "获取成功";
        }

        private void GetBigCPDataOnlineOpencai()
        {
            UpdateCPOnlineOpencaiData("http://f.opencai.net/sd11x5-25.json", 1, "山东");
            //UpdateCPOnlineData("http://c.opencai.net/Y45eEPnwqUgaRRgKk1r/index.json", 2, "广东");
            UpdateCPOnlineOpencaiData("http://f.opencai.net/gd11x5-25.json", 2, "广东");
            UpdateCPOnlineOpencaiData("http://f.opencai.net/jx11x5-25.json", 3, "江西");
            UpdateCPOnlineOpencaiData("http://f.opencai.net/cq11x5-25.json", 4, "重庆");
            UpdateCPOnlineOpencaiData("http://f.opencai.net/js11x5-25.json", 5, "江苏");
            UpdateCPOnlineOpencaiData("http://f.opencai.net/zj11x5-25.json", 6, "浙江");
            UpdateCPOnlineOpencaiData("http://f.opencai.net/sh11x5-25.json", 7, "上海");
            manProgressLabel.Text = "大量数据补救成功";
        }

        // 爱彩人
        private void GetCPDataWebAicairen()
        {
            /*
            ThreadStart threadStartSD = new ThreadStart(UpdateCPWebDataThreadSD);
            ThreadStart threadStartGD = new ThreadStart(UpdateCPWebDataThreadGD);
            ThreadStart threadStartJX = new ThreadStart(UpdateCPWebDataThreadJX);
            ThreadStart threadStartCQ = new ThreadStart(UpdateCPWebDataThreadCQ);

            Thread threadSD = new Thread(threadStartSD);
            Thread threadGD = new Thread(threadStartSD);
            Thread threadJX = new Thread(threadStartSD);
            Thread threadCQ = new Thread(threadStartSD);

            threadSD.Start();
            threadGD.Start();
            threadJX.Start();
            threadCQ.Start();
             */
            RecordLog("GetCPDataWeb " + DateTime.Now.ToString());
            UpdateCPWebDataAicairen("http://www.aicairen.com/openinfo.aspx?lottype=gd11x5", 2, "广东");
            RecordLog("GetCPDataWeb 广东 " + DateTime.Now.ToString());
            UpdateCPWebDataAicairen("http://www.aicairen.com/openinfo.aspx?lottype=sd11x5", 1, "山东");
            RecordLog("GetCPDataWeb 山东 " + DateTime.Now.ToString());
            UpdateCPWebDataAicairen("http://www.aicairen.com/openinfo.aspx?lottype=jx11x5", 3, "江西");
            RecordLog("GetCPDataWeb 江西" + DateTime.Now.ToString());
            UpdateCPWebDataAicairen("http://www.aicairen.com/openinfo.aspx?lottype=cq11x5", 4, "重庆");
            RecordLog("GetCPDataWeb 重庆 " + DateTime.Now.ToString());
        }

        // 淘宝彩票
        private void GetCPDataWebTaobaoAicairen()
        {
            RecordLog("GetCPDataWeb " + DateTime.Now.ToString());
            UpdateCPWebDataTaobao("http://caipiao.taobao.com/lottery/awardresult/lottery_syxw.htm", 2, "广东");
            RecordLog("GetCPDataWeb 广东 " + DateTime.Now.ToString());
            UpdateCPWebDataTaobao("http://caipiao.taobao.com/lottery/awardresult/lottery_dj.htm", 1, "山东");
            RecordLog("GetCPDataWeb 山东 " + DateTime.Now.ToString());
            UpdateCPWebDataAicairen("http://www.aicairen.com/openinfo.aspx?lottype=jx11x5", 3, "江西");
            RecordLog("GetCPDataWeb 江西" + DateTime.Now.ToString());
            UpdateCPWebDataAicairen("http://www.aicairen.com/openinfo.aspx?lottype=cq11x5", 4, "重庆");
            RecordLog("GetCPDataWeb 重庆 " + DateTime.Now.ToString());
        }

        // caipiaokong
        private void GetCPDataOnlineCaipiaokong()
        {
            RecordLog("GetCPDataWeb " + DateTime.Now.ToString());
            UpdateCPOnlineCaipiaokongData("http://api.caipiaokong.com/lottery/?name=gdsyxw&format=json&uid=53821&token=E5ED6AE6DE20EDFB83F34984DD88A995", 2, "广东");

            /*
            RecordLog("GetCPDataWeb 广东 " + DateTime.Now.ToString());
            UpdateCPWebDataTaobao("http://caipiao.taobao.com/lottery/awardresult/lottery_dj.htm", 1, "山东");
            RecordLog("GetCPDataWeb 山东 " + DateTime.Now.ToString());
            UpdateCPWebDataAicairen("http://www.aicairen.com/openinfo.aspx?lottype=jx11x5", 3, "江西");
            RecordLog("GetCPDataWeb 江西" + DateTime.Now.ToString());
            UpdateCPWebDataAicairen("http://www.aicairen.com/openinfo.aspx?lottype=cq11x5", 4, "重庆");
            RecordLog("GetCPDataWeb 重庆 " + DateTime.Now.ToString());
             */
        }

        // 混合
        private void GetCPData()
        {
            RecordLog("GetCPDataWeb " + DateTime.Now.ToString());
            UpdateCPOnlineOpencaiData("http://f.opencai.net/sd11x5-1.json", 1, "山东");
            //UpdateCPWebDataBaidu("http://hao123.lecai.com/lottery/draw/view/23", 2, "广东");
            RecordLog("GetCPDataWeb 广东 " + DateTime.Now.ToString());
            UpdateCPOnlineOpencaiData("http://f.opencai.net/gd11x5-1.json", 2, "广东");
            //UpdateCPWebDataBaidu("http://hao123.lecai.com/lottery/draw/view/20", 1, "山东");
            RecordLog("GetCPDataWeb 山东 " + DateTime.Now.ToString());
            UpdateCPOnlineOpencaiData("http://f.opencai.net/jx11x5-1.json", 3, "江西");
            RecordLog("GetCPDataWeb 江西" + DateTime.Now.ToString());
            UpdateCPOnlineOpencaiData("http://f.opencai.net/cq11x5-1.json", 4, "重庆");
            RecordLog("GetCPDataWeb 重庆 " + DateTime.Now.ToString());
            UpdateCPOnlineOpencaiData("http://f.opencai.net/js11x5-1.json", 5, "江苏");
            RecordLog("GetCPDataWeb 江苏 " + DateTime.Now.ToString());
            UpdateCPOnlineOpencaiData("http://f.opencai.net/zj11x5-1.json", 6, "浙江");
            RecordLog("GetCPDataWeb 浙江 " + DateTime.Now.ToString());
            UpdateCPOnlineOpencaiData("http://f.opencai.net/sh11x5-1.json", 7, "上海");
            RecordLog("GetCPDataWeb 上海 " + DateTime.Now.ToString());
        }
        /*
        private void UpdateCPWebDataThreadSD(object sender)
        {
            int min = DateTime.Now.Minute % 10;
            if ((min == 5))
            {
                //RecordLog("UpdateCPWebDataThreadSD " + DateTime.Now.ToString());
                UpdateCPWebDataAicairen("http://www.aicairen.com/openinfo.aspx?lottype=sd11x5", 1, "山东");
            }
        }
        private void UpdateCPWebDataThreadGD(object sender)
        {
            int min = DateTime.Now.Minute % 10;
            if ((min == 0))
            {
                //RecordLog("UpdateCPWebDataThreadGD " + DateTime.Now.ToString());
                UpdateCPWebDataAicairen("http://www.aicairen.com/openinfo.aspx?lottype=gd11x5", 2, "广东");
            }
        }
        private void UpdateCPWebDataThreadJX(object sender)
        {
            int min = DateTime.Now.Minute % 10;
            if ((min == 0))
            {
                //RecordLog("UpdateCPWebDataThreadJX " + DateTime.Now.ToString());
                UpdateCPWebDataAicairen("http://www.aicairen.com/openinfo.aspx?lottype=jx11x5", 3, "江西");
            }
        }
        private void UpdateCPWebDataThreadCQ(object sender)
        {
            int min = DateTime.Now.Minute % 10;
            if ((min == 0))
            {
                //RecordLog("UpdateCPWebDataThreadCQ " + DateTime.Now.ToString());
                UpdateCPWebDataAicairen("http://www.aicairen.com/openinfo.aspx?lottype=cq11x5", 4, "重庆");
            }
        }
        */

        //  抓取
        private void updateCPDataTimer_Tick(object sender, EventArgs e)
        {
            //int min = DateTime.Now.Minute % 5;
            //if (DateTime.Now.Minute % 2 == 0)
            //if ((min == 3) || ((min > 7) && (min < 8)))
            if ((DateTime.Now.Hour <= 23) && (DateTime.Now.Hour >= 9))
            {
                // if ((min == 0) || (min == 1) || (min == 2))
                // {
                try
                {
                    if (GetDataFromWebCheckBox.Checked)
                    {
                        RecordLog("updateCPDataTimer_Tick " + DateTime.Now.ToString());
                        //GetCPDataWebAicairen();
                        GetCPData();
                    }
                    else
                    {
                        GetCPDataOnlineOpencai();
                    }
                }
                catch (WebException wex)
                {
                    RecordLog("获取实时彩票数据超时" + "\r\nWebException: " + wex.ToString() + "\r\nStatus" + wex.Status);
                }
                catch (Exception ex)
                {
                    getOnTimeCpDataErrorCount++;
                    if (getOnTimeCpDataErrorCount == 9)
                    {
                        StopService();
                        RecordLog(BasicFeature.SendMail("[错误]彩盈盈后台服务管理系统", "", "多次获取实时彩票数据错误 已暂停后台服务！<br>updateCPDataTimer_Tick()<br>" + ex.ToString(), true));
                    }
                    RecordLog("获取实时彩票数据错误" + "\r\nTargetSite: " + ex.TargetSite + "\r\n" + ex.ToString());
                }
                //}
            }
        }

        // 大彩票数据
        private void updateCPDataBigTimer_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.Hour % 3 == 0)
            {
                GetBigCPDataOnlineOpencai();
            }
        }


        // opencai
        private void UpdateCPOnlineOpencaiData(string link, int cpType, string cpName)
        {
            string html = api.HttpGet(link);
            JsonData json = JsonMapper.ToObject(html);
            foreach (JsonData row in json["data"])
            {
                UpdateCPOnlineDataSQL(row, cpType, cpName);
            }
        }


        // caipiaokong
        private void UpdateCPOnlineCaipiaokongData(string link, int cpType, string cpName)
        {
            //string html = api.HttpGet(link);
            string html = api.LinkDataGrab(link);
            JsonData json = JsonMapper.ToObject(html);
            foreach (JsonData row in json["data"])
            {
                UpdateCPOnlineDataSQL(row, cpType, cpName);
            }
        }
        //爱彩人
        private void UpdateCPWebDataAicairen(string link, int cpType, string cpName)
        {
            string html = api.LinkDataGrab(link);
            string cpDataPre = "<span class=\"\">开奖号码：</span>\r\n\t\t\t\t\t\t\t\t\t\t<i class=\"red_ball\">";
            string cpDayPre = "<li><span class=\"\">开奖期号：</span>第";
            string cpOpenTimePre = "<li><span class=\"\">开奖时间：</span>";

            int cpDataFirst = html.IndexOf(cpDataPre) + cpDataPre.Length;
            int cpDataLast = html.LastIndexOf("</i>\r\n\t\t\t\t\t\t\t\t\t\t\r\n\t\t\t\t\t\t\t\t\t\r\n\t\t\t\t\t\t\t\t\t</li>\r\n\t\t\t\t\t\t\t\t\t<li><span class=\"\">开奖期号：</span>第");

            int cpDayFirst = html.IndexOf(cpDayPre) + cpDayPre.Length;
            int cpDayLast = html.LastIndexOf("期 </li>\r\n\t\t\t\t\t\t\t\t\t<li><span class=\"\">开奖时间：");

            int cpOpenFirst = html.IndexOf(cpOpenTimePre) + cpOpenTimePre.Length;
            int cpOpenTimeLast = html.LastIndexOf(" </li>\r\n\t\t\t\t\t\t\t\t</ul>\r\n\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t<div class=\"kj_ljtz t_10\">");

            string[] stringSeparators = new string[] { "</i>\r\n\t\t\t\t\t\t\t\t\t\t<i class=\"red_ball\">" };
            string[] cpDataArray = html.Substring(cpDataFirst, cpDataLast - cpDataFirst).Split(stringSeparators, StringSplitOptions.None);
            string cpData = cpDataArray[0] + cpDataArray[1] + cpDataArray[2] + cpDataArray[3] + cpDataArray[4];
            string cpDay = html.Substring(cpDayFirst, cpDayLast - cpDayFirst);

            Random rNumber = new Random();
            int randomN = rNumber.Next(0, 59);
            string randomString = randomN.ToString();
            if (randomN < 10)
            {
                randomString = "0" + randomN.ToString();
            }
            string cpOpenTime = html.Substring(cpOpenFirst, cpOpenTimeLast - cpOpenFirst).Replace("年", "-").Replace("月", "-").Replace("日", "") + ":" + randomString;
            UpdateCPWebDataSQL(cpDay, cpData, cpOpenTime, cpType, cpName);
            //RecordLog(cpName + " 获取成功 " + DateTime.Now.ToString());
        }

        //淘宝彩票
        private void UpdateCPWebDataTaobao(string link, int cpType, string cpName)
        {
            string html = api.LinkDataGrab(link);
            string cpDataPre = "开奖结果：</span>\n\t\t\t\t\t\t\t\t\t\t<span class=\"cb\">";
            string cpDayPre = "\"  selected=\"selected\" >";
            string cpOpenTimePre = "<li><span>开奖时间：</span><span>";

            int cpDataFirst = html.IndexOf(cpDataPre) + cpDataPre.Length;
            int cpDataLast = html.LastIndexOf("</span>\t\t\t\t\t\t\t\t\t\t\n\t\t\t\t\t\t\t\t\t</li>\n\t\t\t\t\t\t\t\t    <li><span>");

            int cpDayFirst = html.IndexOf(cpDayPre) + cpDayPre.Length;
            int cpDayLast = html.LastIndexOf("</option>\n                                                                        \t<option value=\"?issue_id=");

            int cpOpenFirst = html.IndexOf(cpOpenTimePre) + cpOpenTimePre.Length;
            int cpOpenTimeLast = html.LastIndexOf("</span></li>\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</ul>\n\t\t\t\t\t\t\t");

            string[] stringSeparators = new string[] { "</span>\n\t\t\t\t\t\t\t\t\t\t<span class=\"cb\">" };
            string[] cpDataArray = html.Substring(cpDataFirst, cpDataLast - cpDataFirst).Split(stringSeparators, StringSplitOptions.None);
            string cpData = cpDataArray[0] + cpDataArray[1] + cpDataArray[2] + cpDataArray[3] + cpDataArray[4];
            string cpDay = html.Substring(cpDayFirst, cpDayLast - cpDayFirst).Substring(0, 8);

            /*
            Random rNumber = new Random();
            int randomN = rNumber.Next(0, 59);
            string randomString = randomN.ToString();
            if (randomN < 10)
            {
                randomString = "0" + randomN.ToString();
            }
             */
            string cpOpenTime = html.Substring(cpOpenFirst, cpOpenTimeLast - cpOpenFirst).Replace("年", "-").Replace("月", "-").Replace("日", "") + ":00";
            UpdateCPWebDataSQL(cpDay, cpData, cpOpenTime, cpType, cpName);
            //RecordLog(cpName + " 获取成功 " + DateTime.Now.ToString());
        }

        //百度彩票
        private void UpdateCPWebDataBaidu(string link, int cpType, string cpName)
        {
            string html = api.HttpGet(link);
            if (!html.Equals(""))
            {
                string cpDataPre = "var latest_draw_result = {\"red\":[\"";
                string cpDayPre = "var latest_draw_phase = '";
                string cpOpenTimePre = "var latest_draw_time = '";

                int cpDataFirst = html.IndexOf(cpDataPre) + cpDataPre.Length;
                int cpDataLast = html.LastIndexOf("\"],\"blue\":[],\"310\":[],\"extra\":[],\"normal\":[]};\n    var latest_draw_phase =");

                int cpDayFirst = html.IndexOf(cpDayPre) + cpDayPre.Length;
                int cpDayLast = html.LastIndexOf("';\n    var latest_draw_time");

                int cpOpenFirst = html.IndexOf(cpOpenTimePre) + cpOpenTimePre.Length;
                int cpOpenTimeLast = html.LastIndexOf("';\n\n    var phaseData = {\"");

                string[] stringSeparators = new string[] { "\",\"" };
                string[] cpDataArray = html.Substring(cpDataFirst, cpDataLast - cpDataFirst).Split(stringSeparators, StringSplitOptions.None);
                string cpData = cpDataArray[0] + cpDataArray[1] + cpDataArray[2] + cpDataArray[3] + cpDataArray[4];
                string cpDay = html.Substring(cpDayFirst, cpDayLast - cpDayFirst);
                string cpOpenTime = html.Substring(cpOpenFirst, cpOpenTimeLast - cpOpenFirst);
                UpdateCPWebDataSQL(cpDay, cpData, cpOpenTime, cpType, cpName);
                //RecordLog(cpName + " 获取成功 " + DateTime.Now.ToString());
            }
        }

        private void UpdateCPWebDataSQL(string cpday, string cpdata, string opentime, int cpType, string cpName)
        {
            RecordLog(cpName + " 开始加入到数据库 " + DateTime.Now.ToString());
            string SQL = @"INSERT INTO cyycpdata (CDay, CType, CData, AddDate, InputMan, CName, OpenTime) SELECT " + "'"
                + cpday + "',"
                + cpType + ",'"
                + cpdata + "', NOW(),'Server'," + "'"
                //+ cpdata + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','Server'," + "'"
                + cpName + "', '"
                + opentime + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM cyycpdata WHERE CDay = '"
                + cpday + "' AND CType = "
                + cpType + ");";
            MySqlCommand cmd = new MySqlCommand(SQL, sqlConnection);
            DBOpen();
            cmd.ExecuteNonQuery();
            DBClose();
            RecordLog(cpName + " 加入数据库成功 " + DateTime.Now.ToString() + " " + SQL);
            /*
             string sqlConnectionCommandTemp = @"server=120.27.30.10; user id=admin; password=admin; database=cyydb;Charset=utf8";
            MySqlConnection sqlConnectionTemp = new MySqlConnection(sqlConnectionCommandTemp);
            sqlConnectionTemp.Open();
            MySqlCommand cmd = new MySqlCommand(SQL, sqlConnectionTemp);
            cmd.ExecuteNonQuery();
            sqlConnectionTemp.Close();
             */
        }


        private void UpdateCPOnlineDataSQL(JsonData joData, int cpType, string cpName)
        {
            DBOpen();
            string cpday = joData["expect"].ToString().Substring(2, 8);
            string cpdata = joData["opencode"].ToString().Replace(",", "");
            string opentime = joData["opentime"].ToString();
            string SQL = @"INSERT INTO cyycpdata (CDay, CType, CData, AddDate, InputMan, CName, OpenTime) SELECT " + "'"
                + cpday + "',"
                + cpType + ",'"
                + cpdata + "', NOW(),'Server'," + "'"
                + cpName + "', '"
                + opentime + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM cyycpdata WHERE CDay = '"
                + cpday + "' AND CType = "
                + cpType + ");";
            MySqlCommand cmd = new MySqlCommand(SQL, sqlConnection);
            cmd.ExecuteNonQuery();
            DBClose();
        }

        private void GetCPDataManualButton_Click(object sender, EventArgs e)
        {
            if (GetDataFromWebCheckBox.Checked)
            {
                GetCPData();
            }
            else
            {
                GetCPDataOnlineOpencai();
            }
        }

        private void GetBigCPDataManualButton_Click(object sender, EventArgs e)
        {
            GetBigCPDataOnlineOpencai();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(generateRandomIntBybit(2).ToString());
        }

        /*
        private JObject GetCpJsonDataFromlink(string link)
        {
            string cpData;

            WebRequest request = WebRequest.Create(link);
            WebResponse response = request.GetResponse();
            StreamReader stream = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);
            cpData = stream.ReadToEnd();//
            request.Abort();
            response.Close();
            stream.Close();

            JObject joData = (JObject)JsonConvert.DeserializeObject(cpData);
            return joData;
        }
        private void UpdateCPOnlineData(JObject jo, int cpType, string cName)
        {
            DBOpen();
            string cpday = jo["data"][0]["expect"].ToString().Substring(2, 8);
            string cpdata = jo["data"][0]["opencode"].ToString().Replace(",", "");
            string opentime = jo["data"][0]["opentime"].ToString();
            string SQL = @"INSERT INTO cyycpdata (CDay, CType, CData, AddDate, InputMan, CName, OpenTime) SELECT " + "'"
                + cpday + "',"
                + cpType + ",'"
                + cpdata + "', NOW(),'Server'," + "'"
                + cName + "', '"
                + opentime + "' FROM DUAL WHERE NOT EXISTS (SELECT * FROM cyycpdata WHERE CDay = '"
                + cpday + "' AND CType = "
                + cpType + ");";
            MySqlCommand cmd = new MySqlCommand(SQL, sqlConnection);
            cmd.ExecuteNonQuery();
            DBClose();
        }
        */
        #endregion

        // 测试

    }
}
