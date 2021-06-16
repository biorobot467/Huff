using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace HaffArch
{
    public partial class Form1 : Form
    {
        string PATH= @"G:\С#\Haffman\";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try {
                Archive.ToArchive(PATH + textBox1.Text, PATH + textBox3.Text);
                textBox2.Text = "Архивирован"; }
            catch
            {
                textBox2.Text = "Error 666";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Archive.UnArchive(PATH + textBox1.Text, PATH + textBox3.Text);
                textBox2.Text = "Рахархивирован";
            }
            catch
            {
                textBox2.Text = "Error 999";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = "";

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }
    }
    class Translate
    {
        public static string TranslateToBinary(string strn)
        {
            int c = 0;
            int decN = IntToTen(strn);
            int bufD = decN;
            while (bufD > 0)
            {
                bufD /= 2;
                c++;
            }

            char[] arr = new char[c];
            char[] rStr = new char[100];
            int ix = 0;
            while (decN > 0)
            {
                arr[ix] = IntToChar(decN % 2);
                decN /= 2;
                ix++;
            }

            ix = 0;
            for (int i = 0; i < c; i++)
            {
                rStr[i] = arr[c - 1 - i];
                ix++;
            }
            string res = null, buf = null;
            int cBit = 0;
            ix = 0;
            while (rStr[ix] != '\0')
            {
                res += rStr[ix];
                cBit++;
                ix++;
            }
            while (cBit < 8)
            {
                buf += "0";
                cBit++;
            }
            buf += res;
            res = buf;
            return res;
        }
        private static int IntToTen(string n)
        {
            int decN = 0,
                ix = 1;
            int c = n.Length;

            for (int i = c - 1; i >= 0; i--)
            {
                int buf = CharToInt(n[i]);
                decN += (buf * ix);
                ix *= 10;
            }
            return decN;
        }
        public static int CharToInt(char s)
        {
            if (s >= '0' && s <= '9') return s - '0';
            else
            {
                if (s >= 'A' && s <= 'Z') return s - 'A' + 10;
                else return 100000;
            }
        }
        private static char IntToChar(int n)
        {
            if (n <= 9) return (char)(n + '0');
            else return (char)(n + 55);
        }
    }
    class List : IComparable<List>//сортировка листов для бинарного дерева
    {
        private char v = '\0';
        private int w = 0;
        private List l = null;
        private List r = null;
        private string cod = null;
        public List L//конструктор - спец.метод, вызывается автоматически в процессе создания объекта
        {
            get => l;
            set => l = value;
        }
        public List R//используется для инициализации элементов данных нового объекта в целом
        {
            get => r;
            set => r = value;
        }
        public string Cod
        {
            get => cod;
            set => cod = value;
        }
        public int W { get => w; }
        public char V { get => v; set => v = value; }
        public List() { }
        public List(in char v, in int w)
        {
            this.v = v;
            this.w = w;
        }
        public List(in int w)
        {
            this.w = w;
        }
        public List(in char v)
        {
            this.v = v;
        }
        public int CompareTo(List obj)//сравнивает экземпляр с аналогичным и возвращает целое число,
        {                             //кодирующее его положение относительно данного
            return this.w.CompareTo(obj.w);//this указывает на объект
        }
    }
    public static class Archive
    {
        public delegate void ShowMessage(string mes);
        public static event ShowMessage Notify;
        private static Dictionary<char, int> cChars = new Dictionary<char, int> { };
        private static Dictionary<char, string> binCod = new Dictionary<char, string> { };
        private static Dictionary<string, char> codToSym = new Dictionary<string, char> { };
        private static List<List> listOfTree = new List<List> { };
        private static string cod = null;
        private static string Start = null;
        private static string Archiv = null;
        private static string Result = null;
        private static string bufp = @"G:\С#\Haffman\bufFile.txt";
        private static int cbitsres = 0;
        public static void ToArchive(string pStart, string pOut)
        {
            Start = pStart;
            Archiv = pOut;
            char s = '\0';
            string cod = null,
                eCod = null;
            int Bit = 0;
            CountChars();
            CreateTree();
            CreateBinaryCodes(listOfTree[0]);
            try
            {
                using (StreamWriter stWr = new StreamWriter(Archiv, false))
                {
                    WriteTreeToFile(listOfTree[0], stWr);
                    stWr.WriteLine();
                    stWr.WriteLine("---");
                }
            }
            catch (Exception)
            {
                Notify?.Invoke("Error when writing data to a file!");
            }
            using (StreamReader stRead = File.OpenText(Start))
            {
                using (FileStream stWrite = new FileStream(bufp, FileMode.Create)) //FileMode - операции с файлами
                {
                    while (stRead.Peek() != -1)
                    {
                        s = Convert.ToChar(stRead.Read());
                        cod += binCod[s];
                        Bit = cod.Length;
                        if (Bit == 8)
                        {
                            List<Byte> bList = new List<Byte>();
                            bList.Add(Convert.ToByte(cod, 2));
                                stWrite.Write(bList.ToArray(),0,1);
                            cbitsres += cod.Length;
                            cod = null;
                            Bit = 0;
                        }
                        else if (Bit > 8)
                        {
                            eCod = cod.Substring(8);
                            cod = cod.Substring(0, cod.Length - eCod.Length);
                            List<Byte> byteList = new List<Byte>();
                            byteList.Add(Convert.ToByte(cod, 2));
                            stWrite.Write(byteList.ToArray(),0,1);
                            cbitsres += cod.Length;
                            cod = eCod;
                            Bit = eCod.Length;
                        }
                    }
                    if (cod != null)
                    {
                        cbitsres += cod.Length;
                        while (cod.Length != 8)
                            cod += "0";
                        List<Byte> byteList = new List<Byte>();
                        byteList.Add(Convert.ToByte(cod, 2));
                        stWrite.Write(byteList.ToArray(),0,1);
                    }
                }
                using (StreamWriter strWr = new StreamWriter(Archiv, true))
                {
                    strWr.WriteLine(Convert.ToString(cbitsres));
                }
                using (FileStream read = File.OpenRead(bufp))
                {
                    using (FileStream write = new FileStream(Archiv, FileMode.Append))
                    {
                        while (read.Position != read.Length)
                        {
                            byte[] ar = new byte[1];
                            read.Read(ar, 0, 1);
                            string textFromFile = System.Text.Encoding.Default.GetString(ar);
                            byte[] arr = new byte[1];
                            arr = System.Text.Encoding.Default.GetBytes(textFromFile);
                            write.Write(ar, 0, ar.Length);
                        }
                    }
                }
                File.Delete(bufp);
                cbitsres = 0;
            }
            binCod.Clear();
            codToSym.Clear();
            Notify?.Invoke("Archived!");//оповещение о событии, конкретно - архивация окончена
            Start = null;
            Archiv = null;
        }
        public static void UnArchive(string pArchiv, string pResult)
        {
            Archiv = pArchiv;
            Result = pResult;
            try
            {
                using (StreamReader strRead = File.OpenText(Archiv))
                {
                    listOfTree.Add(new List());
                    listOfTree[0] = RecoverTree(listOfTree[0], strRead);
                    CreateBinaryCodes(listOfTree[0]);
                }
                Notify?.Invoke("Please, wait...");//событие - процесс идёт
                using (BinaryReader readBin = new BinaryReader(File.Open(pArchiv, FileMode.Open)))
                {
                    using (StreamWriter strWrite = new StreamWriter(Result, false))
                    {
                        int c = 0;
                        string readCod = null;
                        byte byteFile = 0;
                        char charFile = '\0';
                        int cLine = 0;
                        while (cLine != 3)
                        {
                            if ((charFile = readBin.ReadChar()) == '-')
                                cLine++;
                            else
                                cLine = 0;

                        }
                        charFile = readBin.ReadChar();
                        charFile = readBin.ReadChar();
                        while ((charFile = readBin.ReadChar()) != '\r')
                        {
                            cbitsres += Translate.CharToInt(charFile);
                            cbitsres *= 10;
                        }
                        cbitsres /= 10;
                        charFile = readBin.ReadChar();
                        while (true)
                        {
                            try
                            {
                                byteFile = readBin.ReadByte();
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    strWrite.Write(codToSym[readCod]);
                                    break;
                                }
                                catch (Exception) { break; }
                            }
                            string strBits = Translate.TranslateToBinary(byteFile.ToString());
                            for (int i = 0; i < strBits.Length; i++)
                            {
                                if (c <= cbitsres)
                                    try
                                    {
                                        strWrite.Write(codToSym[readCod]);
                                        readCod = Convert.ToString(strBits[i]);
                                        c++;
                                    }
                                    catch (Exception)
                                    {
                                        readCod += strBits[i];
                                        c++;
                                    }
                            }
                        }
                    }
                }
                Notify?.Invoke("File was unarchived!");//событие - разархивация окончена
            }
            catch (Exception)
            {
                Notify?.Invoke("Error when reading data from a file!"); //событие - ничего не получилось
            }
            Start = null;
            Result = null;
            File.Delete(bufp);
        }
        private static List RecoverTree(List head, StreamReader strRead)
        {
            List newLeftList = new List();
            List newRightList = new List();

            if ((Convert.ToChar(strRead.Read())) == '0')
            {
                head.L = newLeftList;
                RecoverTree(head.L, strRead);
                head.R = newRightList;
                RecoverTree(head.R, strRead);
            }
            else
            {
                head.V = Convert.ToChar(strRead.Read());
            }
            return head;
        }

        private static void CountChars()
        {
            char s = '\0';
            try
            {
                using (StreamReader strRead = File.OpenText(Start))
                {
                    while (strRead.Peek() != -1)
                    {
                        s = Convert.ToChar(strRead.Read());
                        try
                        {
                            cChars[s]++;
                        }
                        catch (Exception)
                        {
                            cChars.Add(s, 1);
                        }
                    }
                }
            }
            catch (Exception)
            {
                Notify?.Invoke("File isn't found!");//не нашли файл
            }
        }
        private static void CreateTree()
        {
            foreach (char s in cChars.Keys)//Оператор foreach выполняет оператор или блок операторов для каждого элемента в экземпляре типа
            {
                listOfTree.Add(new List(s, cChars[s]));
            }
            while (listOfTree.Count != 1)
            {
                listOfTree.Sort();
                List newList = new List(listOfTree[0].W + listOfTree[1].W);
                newList.L = listOfTree[0];
                newList.R = listOfTree[1];
                listOfTree.RemoveAt(0);
                listOfTree.RemoveAt(0);
                listOfTree.Add(newList);
            }
        }
        private static void CreateBinaryCodes(List list)
        {
            if (list.L != null)
            {
                cod += "0";
                CreateBinaryCodes(list.L);
            }
            if (list.R != null)
            {
                cod += "1";
                CreateBinaryCodes(list.R);
            }
            if (list.L == null && list.R == null)
            {
                binCod.Add(list.V, cod);
                codToSym.Add(cod, list.V);
            }

            list.Cod = cod;
            cod = null;
            if (list.Cod != null)
                for (int i = 0; i < list.Cod.Length - 1; i++)
                {
                    cod += list.Cod[i];
                }
        }
        private static void WriteTreeToFile(List list, StreamWriter strWrite)
        {
            if (list.L != null || list.R != null)
            {
                strWrite.Write("0");
                WriteTreeToFile(list.L, strWrite);
                WriteTreeToFile(list.R, strWrite);
            }
            if (list.L == null && list.R == null)
            {
                strWrite.Write("1");
                strWrite.Write(list.V);
            }
        }
        private static void CreateFile(ref string pFile, string nFile)
        {
            DirectoryInfo dirInfo;
            try
            {
                dirInfo = new DirectoryInfo(Start);
            }
            catch (Exception)
            {
                dirInfo = new DirectoryInfo(Archiv);
            }
            string pathBuf = Convert.ToString(dirInfo.Parent) + nFile;
            FileInfo file = new FileInfo(pathBuf);
            pFile = Convert.ToString(file.FullName);
        }
    }


}
