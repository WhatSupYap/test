using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace WebUtil
{
    /// <summary>
    /// 기본 적인 함수를 포함한다.
    /// </summary>
    public class Files
    {
        /// <summary>
        /// 
        /// </summary>
        public Files()
        {
        }

        /// <summary>
        /// 디렉토리 존재여부 
        /// </summary>
        /// <param name="Path">경로</param>
        /// <returns></returns>
        public bool IsDir(string Path)
        {
            return (System.IO.Directory.Exists(Path)) ? true : false;
        }

        /// <summary>
        /// 디렉토리 생성 [1]
        /// </summary>
        /// <param name="Path">경로</param>
        public void MkDirs(string Path)
        {
            if (IsDir(Path) == false)
            {
                System.IO.Directory.CreateDirectory(Path);
            }
        }

        /// <summary>
        /// 디렉토리 생성 [2]
        /// </summary>
        /// <param name="Root">루트</param>
        /// <param name="Path">경로</param>
        public void MkDirs(string Root, string Path)
        {
            string[] Arrs = Path.Replace("\\", "/").Split('/');
            string Dirs = Root;

            foreach (string Val in Arrs)
            {
                if (Val.Length > 0)
                {
                    Dirs += "/" + Val;

                    MkDirs(Dirs);
                }
            }
        }

        /// <summary>
        /// 디렉토리 삭제
        /// </summary>
        /// <param name="Path">경로</param>
        public void RmDirs(string Path)
        {
            if (IsDir(Path) == true)
            {
                System.IO.Directory.Delete(Path, true);
            }
        }

        /// <summary>
        /// 디렉토리 내용
        /// </summary>
        /// <param name="Path">경로</param>
        /// <returns></returns>
        public string[] Dirs(string Path)
        {
            if (IsDir(Path) == true)
            {
                return System.IO.Directory.GetDirectories(Path);
            }

            return new string[0];
        }

        /// <summary>
        /// 파일명 구하기
        /// </summary>
        /// <param name="Path">파일경로</param>
        /// <returns></returns>
        public string GetName(string Path)
        {
            string Real = "";
            string Name = "";

            if (Path.Length > 0)
            {
                Path = Path.Replace("\\", "/");
                Real = Path.Substring(Path.LastIndexOf("/") + 1, Path.Length - (Path.LastIndexOf("/") + 1));
                if (Real.IndexOf(".") > 0)
                    Name = Real.Substring(0, Real.LastIndexOf("."));
                else
                    Name = Real;
            }

            return Name;
        }

        /// <summary>
        /// 파일명 구하기
        /// </summary>
        /// <param name="Path">파일경로</param>
        /// <returns></returns>
        public string GetPathName(string Path)
        {
            string Real = "";
            string Name = "";
            Path = Path.Replace("\\", "/");

            if (Path.Length > 0 && Path.LastIndexOf("/") > 0)
            {
                Real = Path.Substring(0, Path.LastIndexOf("/"));
                Name = Real;
            }

            return Name;
        }

        /// <summary>
        /// 파일 확장자 구하기
        /// </summary>
        /// <param name="Path">파일경로</param>
        /// <returns></returns>
        public string GetExtention(string Path)
        {
            string Extention = "";

            if (Path.Length > 0)
            {
                Extention = Path.Substring(Path.LastIndexOf(".") + 1, Path.Length - (Path.LastIndexOf(".") + 1));
            }

            return Extention;
        }

        /// <summary>
        /// 이미지 확장자 구하기
        /// </summary>
        /// <param name="Img"></param>
        /// <returns></returns>
        public string GetExtention(System.Drawing.Image Img)
        {
            string Extention = "";

            if (System.Drawing.Imaging.ImageFormat.Bmp.Equals(Img.RawFormat))
            {
                Extention = "bmp";
            }
            else if (System.Drawing.Imaging.ImageFormat.Gif.Equals(Img.RawFormat))
            {
                Extention = "gif";
            }
            else if (System.Drawing.Imaging.ImageFormat.Icon.Equals(Img.RawFormat))
            {
                Extention = "ico";
            }
            else if (System.Drawing.Imaging.ImageFormat.Jpeg.Equals(Img.RawFormat))
            {
                Extention = "jpg";
            }
            else if (System.Drawing.Imaging.ImageFormat.Png.Equals(Img.RawFormat))
            {
                Extention = "png";
            }
            else if (System.Drawing.Imaging.ImageFormat.Tiff.Equals(Img.RawFormat))
            {
                Extention = "tif";
            }
            else if (System.Drawing.Imaging.ImageFormat.Wmf.Equals(Img.RawFormat))
            {
                Extention = "wmf";
            }

            return Extention;
        }

        /// <summary>
        /// 파일 존재여부
        /// </summary>
        /// <param name="Path">경로</param>
        /// <returns></returns>
        public bool IsFile(string Path)
        {
            return (System.IO.File.Exists(Path)) ? true : false;
        }

        public string CheckFileName(string Upload_FilePath)
        {
            string Path = GetPathName(Upload_FilePath);
            string FileName = GetName(Upload_FilePath);
            string FileExt = GetExtention(Upload_FilePath);
            int i = 1;
            string tmpFilePath = Path + "\\" + FileName + "." + FileExt;

            do
            {
                if (IsFile(tmpFilePath))
                    tmpFilePath = Path + "\\"+ FileName + "(" + i.ToString() + ")." + FileExt;

                i += 1;

            } while (IsFile(tmpFilePath));


            return tmpFilePath;
        }

        /// <summary>
        /// 파일 생성
        /// </summary>
        /// <param name="Path">경로</param>
        public void MkFile(string Path)
        {
            if (IsFile(Path) == false)
            {
                System.IO.File.Create(Path);
            }
        }

        /// <summary>
        /// 파일 삭제
        /// </summary>
        /// <param name="Path">경로</param>
        public void RmFile(string Path)
        {
            if (IsFile(Path) == true)
            {
                try
                {
                    System.IO.File.Delete(Path);
                }
                catch
                {
                }
            }
        }


        /// <summary>
        /// 파일 카피
        /// </summary>
        /// <param name="From">원본</param>
        /// <param name="To">복사</param>
        public void Copy(string From, string To)
        {
            if (IsFile(From) == true)
            {
                System.IO.File.Copy(From, To, true);
            }
        }

        /// <summary>
        /// 파일 이동
        /// </summary>
        /// <param name="From">원본</param>
        /// <param name="To">이동</param>
        public void Move(string From, string To)
        {
            if (IsFile(From) == true)
            {
                // 2012-11-20 파일 삭제가 문제가 되어 카피로 변경
                //System.IO.File.Move(From, To);

                Copy(From, To);

                // 카피이후 삭제 처리
                RmFile(From);
            }

        }

        /// <summary>
        /// 파일 읽기
        /// </summary>
        /// <param name="Path">경로</param>
        /// <param name="encoding">인코딩 ANSI인 파일은 Encoding.Default 로 넘겨야 됨</param>
        /// <returns></returns>
        public string Read(string Path, System.Text.Encoding encoding = null)
        {
            string Str = "";
            string Rtn = "";

            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;

            if (IsFile(Path) == true)
            {
                using (System.IO.StreamReader Strm = new StreamReader(Path, encoding, true))
                {
                    while ((Str = Strm.ReadLine()) != null)
                    {
                        Rtn += Str + System.Environment.NewLine;
                    }
                }
            }

            return Rtn;
        }

        /// <summary>
        /// 파일 읽기
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public byte[] ReadByte(string Path)
        {
            byte[] fileBytes = null;
            fileBytes = File.ReadAllBytes(Path);

            return fileBytes;
        }

        /// <summary>
        /// 파일 쓰기
        /// </summary>
        /// <param name="Path">경로</param>
        /// <param name="Body">내용</param>
        /// <param name="encoding">인코딩 ANSI로 저장할경우는 Encoding.Default 로 넘겨야 됨</param>
        public void Write(string Path, string Body, System.Text.Encoding encoding = null)
        {
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;

            using (System.IO.FileStream Fs = System.IO.File.OpenWrite(Path))
            {
                System.Text.Encoding Enc = encoding;

                byte[] Info = Enc.GetBytes(Body);

                Fs.Write(Info, 0, Info.Length);
            }

			
		}

        public void Write_BOM(string Path, string Body, System.Text.Encoding encoding = null)
        {
            try
            {
				if (encoding == null) encoding = System.Text.Encoding.UTF8;

				using (var fs = new FileStream(Path, FileMode.Create))
				{
					// BOM 바이트를 포함하는 바이트 배열 생성
					var bomBytes = new byte[] { 0xEF, 0xBB, 0xBF };
					var bodyBytes = encoding.GetBytes(Body);

					// BOM 바이트와 본문 바이트를 연결
					var allBytes = bomBytes.Concat(bodyBytes).ToArray();

					fs.Write(allBytes, 0, allBytes.Length);
				}
			}
            catch (Exception ex)
            {
                throw ex;
            }
		}


			/// <summary>
			/// 파일 내용 추가
			/// </summary>
			/// <param name="Path"></param>
			/// <param name="Body"></param>
			public void WriteAppend(string Path, string Body)
        {
            using (StreamWriter sw = File.AppendText(Path))
            {
                sw.WriteLine(Body);
            }
        }

        /// <summary>
        /// 파일 내용 삭제
        /// </summary>
        /// <param name="Path"></param>
        public void Clear (string Path)
        {
            using (StreamWriter writer = new StreamWriter(Path))
            {
                writer.Write("");
            }
        }

        /// <summary>
        /// 폴더 규칙
        /// </summary>
        /// <param name="idx">index</param>
        /// <returns></returns>
        public string GetDir(int idx)
        {
            string ReturnDir = "";
            string RtnVal = "";
            string Frm = "0";
            string Str = "";

            Str = (idx % 1000).ToString();
            RtnVal = Str;

            for (int ii = 1; ii < 4 - Str.Length; ii++)
            {
                RtnVal = Frm + RtnVal;
            }

            ReturnDir = RtnVal + "\\" + idx.ToString();

            return ReturnDir;
        }

        /// <summary>
        /// 이미지 사이즈 구하기
        /// </summary>
        /// <param name="Path">경로</param>
        /// <returns></returns>
        public int[] GetSize(string Path)
        {
            int[] Size = { 0, 0 };

            System.Drawing.Image Img = System.Drawing.Image.FromFile(Path);

            Size[0] = Img.Width;
            Size[1] = Img.Height;

            return Size;
        }

        /// <summary>
        /// 보정된 사이즈 구하기
        /// </summary>
        /// <param name="Width">가로</param>
        /// <param name="Height">세로</param>
        /// <param name="MaxWidth">최대가로</param>
        /// <param name="MaxHeight">최대세로</param>
        /// <returns></returns>
        public int[] ADJSIZE(double Width, double Height, double MaxWidth, double MaxHeight)
        {
            int[] RtnVal = { 0, 0 };

            double RatioWidth = 1;
            double RatioHeight = 1;

            if (Width == 0 || Height == 0)
            {
                RtnVal[0] = (int)MaxWidth;
                RtnVal[1] = (int)MaxHeight;
            }
            else if (MaxWidth > 0 && MaxHeight > 0)
            {
                if (Width > MaxWidth || Height > MaxHeight)
                {
                    RatioWidth = Width / MaxWidth;
                    RatioHeight = Height / MaxHeight;

                    if (RatioWidth > RatioHeight)
                    {
                        RtnVal[1] = (int)(Height * (MaxWidth / Width));
                        RtnVal[0] = (int)MaxWidth;
                    }
                    else
                    {
                        RtnVal[0] = (int)(Width * (MaxHeight / Height));
                        RtnVal[1] = (int)MaxHeight;
                    }
                }
                else
                {
                    RtnVal[0] = (int)Width;
                    RtnVal[1] = (int)Height;
                }
            }
            else if (MaxWidth > 0)
            {
                if (Width > MaxWidth)
                {
                    RtnVal[0] = (int)MaxWidth;
                    RtnVal[1] = (int)(Height * MaxWidth / Width);
                }
                else
                {
                    RtnVal[0] = (int)Width;
                    RtnVal[1] = (int)Height;
                }
            }
            else if (MaxHeight > 0)
            {
                if (Height > MaxHeight)
                {
                    RtnVal[0] = (int)(Width * MaxHeight / Height);
                    RtnVal[1] = (int)MaxHeight;
                }
                else
                {
                    RtnVal[0] = (int)Width;
                    RtnVal[1] = (int)Height;
                }
            }
            else
            {
                RtnVal[0] = (int)Width;
                RtnVal[1] = (int)Height;
            }

            return RtnVal;
        }

        /// <summary>
        /// 리사이징
        /// </summary>
        /// <param name="OrgPath">원본</param>
        /// <param name="RePath">수정</param>
        /// <param name="ReSize">사이즈</param>
        public void RESIZE(string OrgPath, string RePath, int[] ReSize)
        {
            int[] OrgSize = GetSize(OrgPath);
            int[] AdjSize = ADJSIZE(System.Double.Parse(OrgSize[0].ToString()), System.Double.Parse(OrgSize[1].ToString()), System.Double.Parse(ReSize[0].ToString()), System.Double.Parse(ReSize[1].ToString()));

            System.Drawing.Image Img = System.Drawing.Image.FromFile(OrgPath).GetThumbnailImage(AdjSize[0], AdjSize[1], null, System.IntPtr.Zero);
            Img.Save(RePath, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        /// <summary>
        /// 이미지.사이즈 줄이기.(50K 이하로 맞췄다.)
        /// </summary>
        /// <param name="ori_img"></param>
        /// <param name="Percent"></param>
        /// <returns></returns>
        public System.Drawing.Image ResizeFile(Image ori_img, int Percent = -1)
        {
            try
            {
                if (Percent == -1)
                {
                    // 업로드된 이미지를 객체로 생성
                    Bitmap img = new Bitmap(ori_img);

                    //새로운 크기로 만들 이미지 생성
                    int width = 800;
                    int height = (width * img.Height) / img.Width;//종횡비 유지한 웹에 출력될 높이

                    if (height > 800)
                    {
                        height = 800;
                        width = (height * img.Width) / img.Height;
                    }

                    Bitmap resizeImg = new Bitmap(width, height);

                    //GDI+를 이용해서 리사이즈된 이미지 생성
                    Graphics g = Graphics.FromImage(resizeImg);
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(img, new Rectangle(0, 0, width, height));

                    Bitmap Ret_img = new Bitmap(resizeImg);

                    if (resizeImg != null)
                        resizeImg.Dispose(); resizeImg = null;

                    if (g != null)
                        g.Dispose(); g = null;

                    if (img != null)
                        img.Dispose(); img = null;

                    return Ret_img;
                }
                else
                {
                    float nPercent = ((float)Percent / 100);

                    int OriginalWidth = ori_img.Width;
                    int OriginalHeight = ori_img.Height;

                    //소스의 처음 위치
                    int OriginalX = 0;
                    int OriginalY = 0;

                    int adjustX = 0;
                    int adjustY = 0;

                    //조절될 퍼센트 계산
                    int adjustWidth = (int)(OriginalWidth * nPercent);
                    int adjustHeight = (int)(OriginalHeight * nPercent);

                    //비어있는 비트맵 객체 생성
                    Bitmap bmPhoto = new Bitmap(adjustWidth, adjustHeight, PixelFormat.Format24bppRgb);

                    //이미지를 그래픽 객체로 만든다.
                    Graphics grPhoto = Graphics.FromImage(bmPhoto);

                    grPhoto.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    grPhoto.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    grPhoto.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                    //그릴 이미지객체 크기, 그려질 이미지객체 크기
                    grPhoto.DrawImage(ori_img,
                    new Rectangle(adjustX, adjustY, adjustWidth, adjustHeight),
                    new Rectangle(OriginalX, OriginalY, OriginalWidth, OriginalHeight),
                    GraphicsUnit.Pixel);
                    grPhoto.Dispose();

                    return bmPhoto;
                }
            }
            catch
            {
                return null;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 파일 용량 가지고 오기
        /// </summary>
        /// <param Img="Image"></param>
        /// <returns></returns>
        public long GetFileSize(string filePath)
        {
            FileInfo info = null;

            try
            {
                info = new FileInfo(filePath);
                return info.Length;
            }
            catch
            {
                return 0;
            }
            finally
            {
                if (info != null)
                {
                    info = null;
                }
            }
        }

        /// <summary>
        /// MineType 리턴
        /// </summary>
        /// <param name="fileName">파일명</param>
        /// <returns></returns>
        public string MimeTypeByFileExtension(string fileName)
        {
            string ext = String.Empty;
            string contentType = "application/octet-stream";
            if (fileName.IndexOf(".") != -1) ext = fileName.Substring(fileName.LastIndexOf(".") + 1);
            switch (ext.ToLower())
            {
                case "mdb": contentType = "application/x-msaccess"; break;
                case "xls": contentType = "application/vnd.ms-excel"; break;
                case "xlsx": contentType = "application/vnd.ms-excel"; break;
                case "doc": contentType = "application/msword"; break;
                case "ppt": contentType = "application/vnd.ms-powerpoint"; break;
                case "pptx": contentType = "application/vnd.ms-powerpoint"; break;
                case "asf": contentType = "video/x-ms-asf"; break;
                case "avi": contentType = "video/avi"; break;
                case "gif": contentType = "image/gif"; break;
                case "jpg": contentType = "image/jpeg"; break;
                case "jpeg": contentType = "image/jpeg"; break;
                case "png": contentType = "image/png"; break;
                case "tif": contentType = "image/tiff"; break;
                case "tiff": contentType = "image/tiff"; break;
            }
            return contentType;
        }

        /// <summary>
        /// ByteArray를 file로 write
        /// 예) ByteArrayToFileSave(base.FileServerDir + "Temp\\" + AttFileNm, (byte[]) ds.Tables[0].Rows[0]["AttFile"]);
        /// </summary>
        /// <param name="path"></param>
        /// <param name="byteObj"></param>
        /// <returns></returns>
        public bool ByteArrayToFileSave(string path, byte[] byteObj)
        {
            FileStream fs = null;
            BinaryWriter w = null;

            try
            {
                fs = new FileStream(path, FileMode.OpenOrCreate);
                w = new BinaryWriter(fs);

                w.Write(byteObj);
                byteObj.Initialize();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (w != null) { w.Close(); w = null; }
                if (fs != null) { fs.Close(); fs = null; }
            }
        }

        /// <summary>
        /// CRC 계산
        /// </summary>
        /// <param name="strPacket"></param>
        /// <returns></returns>
        public ushort CalcCRC16(byte[] strPacket)
        {
            ushort[] CRC16_TABLE = { 0x0000, 0xCC01, 0xD801, 0x1400, 0xF001, 0x3C00, 0x2800, 0xE401, 0xA001, 0x6C00, 0x7800, 0xB401, 0x5000, 0x9C01, 0x8801, 0x4400 };
            ushort usCRC = 0xFFFF;
            ushort usTemp = 0;

            foreach (char cCurrent in strPacket)
            {
                byte bytCurrent = Convert.ToByte(cCurrent);// lower 4 bits 
                usTemp = CRC16_TABLE[usCRC & 0x000F];
                usCRC = (ushort)((usCRC >> 4) & 0x0FFF);
                usCRC = (ushort)(usCRC ^ usTemp ^ CRC16_TABLE[bytCurrent & 0x000F]); // Upper 4 Bits 
                usTemp = CRC16_TABLE[usCRC & 0x000F];
                usCRC = (ushort)((usCRC >> 4) & 0x0FFF);
                usCRC = (ushort)(usCRC ^ usTemp ^ CRC16_TABLE[(bytCurrent >> 4) & 0x000F]);
            }
            return usCRC;
        }

        /// <summary>
        /// CRC 값
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public string GetCRC16(string Path)
        {
            return CalcCRC16(ReadByte(Path)).ToString();
        }


        /// <summary>
        /// 파일크기
        /// </summary>
        /// <param name="Path">경로</param>
        /// <returns></returns>
        public long GetLength(string Path)
        {
            FileInfo fInfo = new FileInfo(Path);
            return fInfo.Length;
        }

        /// <summary>
        /// 파일 업로드
        /// </summary>
        /// <param name="sSavePath"></param>
        /// <param name="HNFleAttachFile"></param>
        /// <param name="SaveFileName"></param>
        /// <returns></returns>
        public Hashtable FileUPload(string sSavePath, System.Web.HttpPostedFileBase HNFleAttachFile, string SaveFileName = "")
        {
            Hashtable result = new Hashtable();

            try
            {
                //파일 업로드 경로
                string sUpFileDir = sSavePath;
                string FilePath = "";
                //파일 업로드
                if (HNFleAttachFile.FileName.Trim() != "")
                {
                    //디렉터리가 없으면 생성
                    MkDirs(sUpFileDir);

                    if (SaveFileName == "")
                    {
                        FilePath = sUpFileDir.Trim() + "\\" + GetName(HNFleAttachFile.FileName) + '.' + GetExtention(HNFleAttachFile.FileName);
                    }else
                    {
                        FilePath = sUpFileDir.Trim() + "\\" + SaveFileName + '.' + GetExtention(HNFleAttachFile.FileName);
                    }

                    //파일명이 중복될 경우 처리
                    string tmpFilePath = CheckFileName(FilePath);

                    HNFleAttachFile.SaveAs(tmpFilePath);

                    result.Add("FileName", GetName(tmpFilePath) + '.' + GetExtention(tmpFilePath));
                    result.Add("FileSize", GetLength(tmpFilePath));
                    result.Add("FilePath", tmpFilePath);
                    result.Add("FileType",HNFleAttachFile.ContentType);
                }

            }
            catch (Exception E)
            {
                throw new Exception(E.Message.ToString());
            }

            return result;
        }

        /// <summary>
        /// 파일삭제
        /// </summary>
        /// <param name="sSavePath"></param>
        /// <param name="sFileName"></param>
        /// <returns></returns>
        public string FileDelete(string sSavePath, string sFileName)
        {
            string sReturnMsg = "";

            try
            {
                if (sFileName.Trim() != "")
                {
                    string sFilePath = sSavePath + "\\" + sFileName;

                    if (File.Exists(sFilePath))
                    {
                        File.Delete(sFilePath);
                    }
                }

                sReturnMsg = "OK";
            }
            catch (Exception E)
            {
                sReturnMsg = "ERROR:" + E.Message.Trim();
            }

            return sReturnMsg;
        }

		//하루 이상 지난 파일은 삭제
		public void DeleteOldFile(string strPath)
		{
			string[] TempFiles = Directory.GetFiles(strPath);

			DateTime Dt = DateTime.Now.AddDays(-1);

			foreach (string OldPath in TempFiles)
			{
				DateTime WriteTime = Directory.GetLastWriteTime(OldPath);

				if (WriteTime < Dt)
				{
					Files f = new Files();
					f.RmFile(OldPath);
				}
			}
		}
	}
}
