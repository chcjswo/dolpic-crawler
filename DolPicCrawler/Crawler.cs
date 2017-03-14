using DolPicCrawler.HashTag;
using DolPicCrawler.Image;
using DolPicCrawler.Utils;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DolPicCrawler
{
    public partial class Crawler : Form
    {
        protected ILog ErrorLog = LogManager.GetLogger("ErrorLog");
        protected ILog ImageInsertLog = LogManager.GetLogger("ImageInsertLog");

        private int CHECK_TIME = 0;
        private ErrFrm errfrm;
        private List<string[]> _arrTxt;

        private string[] arrSiteName = { "트위터", "인스타그램", "페이스북" };

        /// <summary>
        /// 서버에 전송될 이미지 경로가 담길 Dictionary
        /// </summary>
        private Dictionary<string, List<string>> _dImage;
        /// <summary>
        /// 서버에 전송될 내용 경로가 담길 Dictionary
        /// </summary>
        private Dictionary<string, List<string>> _dCaption;
        /// <summary>
        /// 카운트다운 변수
        /// </summary>
        private double dDay, dMod, dHour, dMin, dSec;

        /// <summary>
        /// 해쉬태그 리스트
        /// </summary>
        private List<HashTagData> _listHashTags;

        /// <summary>
        /// json 가져오는 api
        /// </summary>
        protected readonly string hahsTagJsonApi = ConfigurationManager.AppSettings["hashTagJsonApiUrl"].ToString();

        /// <summary>
        /// 생성자
        /// </summary>
        public Crawler()
        {
            InitializeComponent();
            FormInit();

            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            txtLog.AppendText(version.ToString() + Environment.NewLine);
            txtLog.AppendText(Application.StartupPath + Environment.NewLine);

            txtLog.AppendText(CommonUtils.getUnicodeToString("\uba38\ub9ac\ud588\uae14. \uace0\ub3d9 \ud68c\uc0c9 \ubcf4\ub77c \ub290\ub08c \uc624\ubb18 \ud55c\ub370 \ud68c\uba74\uc5d0 \uc798 \uc548\uc7a1\ud788\ub124 \uc5b4\ub461\uac8c \ucc0d\uc74c \ub108\ubb34 \uc5b4\ub461\uac8c \ucc0d\ud788\uace0 \ubc1d\uc740\ub370\uc11c \ucc0d\uc74c \ub108\ubb34 \ubc1d\uc544\ubd48\uace0 \uc6d4\ub798 \ud558\ub824\ub358 \uba38\ub9ac\ub294 \ud1f4\uc9dc \ub9de\uc558\uc9c0\ub9cc \ub9d8\uc5d0 \ub9cc\uc871\uc2a4\ub7fd\uac8c \ub098\uc654\uc73c\ub2c8 \uc140\uce74 \ub9ce\uc774 \ub0a8\uaca8\uc57c\uaca0\uad70 \ud83d\ude3d\ud83d\udc83\ud83c\udffc\u2b50\ud83c\udf19\n#\uc77c\uc0c1#\uc778\uc2a4\ud0c0\uc77c\uc0c1#\uc140\uce74#\uc140\uc2a4\ud0c0\uadf8\ub7a8#\uc140\uae30\uafbc#\uba38\ub9ac\uc2a4\ud0c0\uc77c #\uc5fc\uc0c9#selfie"));
        }

        #region Init

        /// <summary>
        /// Grid 초기화
        /// </summary>
        private void GridInit()
        {
            dataGridView1.Rows.Clear();

            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "이미지경로";
            dataGridView1.Columns[1].Name = "해쉬태그";
            dataGridView1.Columns[2].Name = "사이트";

            dataGridView1.Columns[0].Width = 400;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[1].Width = 100;
            dataGridView1.Columns[2].Width = 100;
        }

        /// <summary>
        /// Form 초기화
        /// </summary>
        private void FormInit()
        {
            _arrTxt = new List<string[]>();
            errfrm = new ErrFrm();
            btnImageLoad.Enabled = false;

            if (timer2.Enabled)
                btnImageLoad.Enabled = false;
            else
                btnImageLoad.Enabled = true;

            // 그리드 초기화
            GridInit();
        }

        /// <summary>
        /// 서버로 부터 json 정보 읽어 오기
        /// </summary>
        private void JsonDataLoadByServer()
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.PostAsync(hahsTagJsonApi,
                new
                {
                }, new JsonMediaTypeFormatter()).Result;

                var contents = response.Content.ReadAsStringAsync().Result;
                txtLog.AppendText(contents);
                _listHashTags = JsonConvert.DeserializeObject<List<HashTagData>>(contents);
            }
        }

        #endregion

        /// <summary>
        /// 이미지 정보 가져오기
        /// </summary>
        private async void ImageGet()
        {
            txtLog.Clear();

            // 일단 json 로딩
            JsonDataLoadByServer();

            // 시간 측정 시작
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (var nTagUrlType = 1; nTagUrlType < 3; nTagUrlType++)
            {
                var task = Task<int>.Run(() => ImageProc(nTagUrlType));

                await task.ContinueWith(x => {
                    txtLog.AppendText(Environment.NewLine);
                    txtLog.AppendText(arrSiteName[nTagUrlType] + " 크롤링 완료");
                    txtLog.AppendText(Environment.NewLine);
                    txtLog.AppendText(arrSiteName[nTagUrlType] + " 이미지 전송중....");
                    txtLog.AppendText(Environment.NewLine);

                    // 그리드 그리기
                    SetGridInfo(nTagUrlType);

                    // 해당 사이트로 부터 이미지정보를 가져오고 이미지 저장
                    ImageService.getInstance.ImageSend(_dImage, nTagUrlType);
                }, TaskScheduler.FromCurrentSynchronizationContext());

                txtLog.AppendText(arrSiteName[nTagUrlType] + " 이미지 전송 완료");
                txtLog.AppendText(Environment.NewLine);
            }

            // 시간 측정 끝
            sw.Stop();
            toolStripLabel3.Text = "총 걸린 시간 : ";
            toolStripLabel4.Text = (sw.ElapsedMilliseconds / 1000.0F).ToString() + " 초 걸림";
        }

        /// <summary>
        /// 각 사이트별 처리
        /// </summary>
        /// <param name="a_nTagUrlType">사이트 타입 1:트위터 2:인스타그램 3:페이스북</param>
        private void ImageProc(int a_nTagUrlType)
        {
            // Dictionary 초기화
            _dImage = new Dictionary<string, List<string>>();

            switch (a_nTagUrlType)
            {
                case (int)OriginSiteType.twitter:
                    // 트위터 이미지 긁어 오기
                    OriginHashTag.JsonFactory(OriginSiteType.twitter).ImageSrcSearch(_listHashTags, ref _dImage, ref _dCaption);
                    break;

                case (int)OriginSiteType.instagram:
                    // 인스타그램 이미지 긁어 오기
                    OriginHashTag.JsonFactory(OriginSiteType.instagram).ImageSrcSearch(_listHashTags, ref _dImage, ref _dCaption);
                    break;

                case (int)OriginSiteType.facebook:
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 그리드 셋팅
        /// </summary>
        /// <param name="a_nTagUrlType">사이트 타입 1:트위터 2:인스타그램 3:페이스북</param>
        private void SetGridInfo(int a_nTagUrlType)
        {
            foreach (KeyValuePair<string, List<string>> kvp in _dImage)
            {
                var tag = _listHashTags.Where(c => c.hashTagIndex.Equals(kvp.Key))
                                        .Select(m => new { m.twitterHashTag, m.instaHashTag });

                var tagName = a_nTagUrlType == (int)OriginSiteType.twitter
                                            ? tag.First().twitterHashTag
                                            : tag.First().instaHashTag;


                txtLog.AppendText(string.Format("태그 index == {0} / 태그 이름 == {2} / count == {1}",
                                kvp.Key, kvp.Value.Count, tagName) + Environment.NewLine);

                foreach (var item in kvp.Value)
                {
                    string[] arrApp = new string[3];

                    arrApp[0] = item;
                    arrApp[1] = tagName;

                    switch (a_nTagUrlType)
                    {
                        case (int)OriginSiteType.twitter:
                            arrApp[2] = "트위터";
                            break;

                        case (int)OriginSiteType.instagram:
                            arrApp[2] = "인스타그램";
                            break;

                        case (int)OriginSiteType.facebook:
                            arrApp[2] = "페이스북";
                            break;

                        default:
                            arrApp[3] = "어디야?";
                            break;
                    }

                    dataGridView1.Rows.Add(arrApp);
                    _arrTxt.Add(arrApp);
                }
            }
        }

        /// <summary>
        /// 에러 보여주기
        /// </summary>
        /// <param name="ex">에러 정보</param>
        private void ShowError(Exception ex)
        {
            errfrm.Owner = this;
            errfrm.textBox1.Text = ex.ToString();
            errfrm.Show();

            txtLog.AppendText(ex.ToString() + Environment.NewLine);

            ErrorLog.ErrorFormat("에러 발생 ==  {0}", ex.ToString());
        }

        #region  자동체크

        /// <summary>
        /// 시간 입력 체크
        /// </summary>
        /// <returns>체크값</returns>
        private bool IsTimeCheck()
        {
            if (string.IsNullOrEmpty(txtTime.Text))
            {
                MessageBox.Show("체크시간을 입력해야지~~~", "에러 발생", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 돈 좀 벌러 가볼까~~~
        /// </summary>
        private void GoShowMeTheMoney()
        {
            CHECK_TIME = Convert.ToInt32(txtTime.Text) * 60;

            ShowMeTheMoney();
        }

        /// <summary>
        /// 돈 좀 벌어 볼까~~~
        /// </summary>
        private void ShowMeTheMoney()
        {
            // 폼은 숨기고
            errfrm.Hide();

            // 초기화하고 시작하자
            FormInit();

            try
            {
                // 이미지 가져오기
                ImageGet();

                toolStripLabel5.Text = string.Format("{0} 에 체크 완료", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                // 파일 만들기
                MakeFile();
            }
            catch (Exception ex)
            {
                // 에러 보여주기
                ShowError(ex);
            }
        }

        /// <summary>
        /// 파일 만들기
        /// </summary>
        private void MakeFile()
        {
            foreach (string[] arrStr in _arrTxt)
            {
                ImageInsertLog.InfoFormat("{0}\t\t{1}\t\t{2}", arrStr[0], arrStr[1], arrStr[2]);
            }
        }

        #endregion

        #region 남은시간 카운트다운

        /// <summary>
        /// 남은시간 카운트다운
        /// </summary>
        private void CountDown()
        {
            toolStripLabel1.Text = "남은 시간 : ";
            string text = "";

            // 남은일수
            dDay = Math.Floor(Convert.ToDouble(CHECK_TIME) / (3600 * 24));
            dMod = CHECK_TIME % (24 * 3600);

            // 남은시간
            dHour = Math.Floor(dMod / 3600);
            dMod = dMod % 3600;

            // 남은분
            dMin = Math.Floor(dMod / 60);

            // 남은초
            dSec = dMod % 60;

            text = (dDay > 0) ? dDay + "일 " : "";
            text = (dHour > 0) ? text + dHour + "시간 " : (text.Length > 0) ? text + dHour + "시간 " : text;
            text = (dMin > 0) ? text + dMin + "분 " : (text.Length > 0) ? text + dMin + "분 " : text;
            text = text + dSec + "초";

            CHECK_TIME--;
            toolStripLabel2.Text = text.ToString();
        }

        #endregion

        #region 이벤트 모음

        /// <summary>
        /// 이미지 가져오기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImageLoad_Click(object sender, EventArgs e)
        {
            txtLog.AppendText(string.Empty);
            dataGridView1.Rows.Clear();
            btnImageLoad.Enabled = false;

            try
            {
                // 이미지 가져오기
                ImageGet();
            }
            catch (Exception ex)
            {
                // 에러 보여주기
                ShowError(ex);
            }
        }

        /// <summary>
        /// json 정보 읽기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnXmlLoad_Click(object sender, EventArgs e)
        {
            btnXmlLoad.Enabled = false;

            btnImageLoad.Enabled = true;
            btnAuthCheck.Enabled = true;

            try
            {
                JsonDataLoadByServer();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GoShowMeTheMoney();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            CountDown();
        }

        /// <summary>
        /// 자동 체크 시작
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAuthCheck_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                btnAuthCheck.Text = "자동 체크 시작";
                btnImageLoad.Enabled = true;
                label2.Visible = false;
                txtTime.ReadOnly = false;
                btnXmlLoad.Enabled = true;

                timer1.Stop();
                timer2.Stop();
            }
            else
            {
                if (IsTimeCheck())
                {
                    timer1.Interval = Convert.ToInt32(txtTime.Text) * 60000;

                    btnAuthCheck.Text = "자동 체크 멈춤";
                    btnImageLoad.Enabled = false;
                    label2.Visible = true;
                    txtTime.ReadOnly = true;

                    timer1.Start();
                    timer2.Start();
                    timer2.Enabled = true;

                    GoShowMeTheMoney();
                }
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            // 아이콘을 더블클릭하면 폼 화면을 보여줌
            ShowForm();
        }

        private void ShowForm()
        {
            this.Visible = true; // 폼의 표시
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal; // 최소화를 멈춘다
            this.Activate(); // 폼을 활성화 시킨다
            this.notifyIcon1.Visible = false;
        }

        private void Crawler_Load(object sender, EventArgs e)
        {
            this.notifyIcon1.Visible = true;
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            this.Dispose();
            Application.Exit();
        }

        private void Crawler_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("프로그램을 종료 하시겠습니까?\n\n'아니요'를 선택하시면 백그라운드로 실행 합니다.",
                "돌픽이야", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Dispose(true);
                Application.Exit();
                Process.GetCurrentProcess().Kill();
                this.Close();
            }
            else
            {
                // 이벤트 취소
                e.Cancel = true;
                this.Hide();
                notifyIcon1.Visible = true;
            }
        }

        #endregion
    }
}
