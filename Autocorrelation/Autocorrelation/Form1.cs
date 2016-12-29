using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Autocorrelation
{
    public partial class Form1 : Form
    {
        List<double> inputVec;

        //Ну ИМХО так удобнее сам на себе множить
        List<double> inputVec2;

        public Form1()
        {
            InitializeComponent();
            inputVec = new List<double>();

            inputVec2 = new List<double>();
        }

        private void открытьИПосчитатьАКToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                StreamReader sr = new StreamReader(openFileDialog1.FileName);
                string line = "123";
                while (line != null)
                {
                    line = sr.ReadLine();

                    if (line != null)
                    {
                        //   MessageBox.Show(vecA.Count.ToString());
                        inputVec.Add(Double.Parse(line));
                        inputVec2.Add(Double.Parse(line));
                    }
                }
                sr.Close();


                StreamWriter sw = new StreamWriter(openFileDialog1.FileName + "_autocorr.csv");
                while (inputVec.Count>0)
                {
                    sw.WriteLine(PirsonCorrelation(inputVec, inputVec2));
                    inputVec.RemoveAt(0);
                    inputVec2.RemoveAt(inputVec2.Count-1);
                }

                sw.Close();

                //Туровский просит сделать очевидно как тут http://medstatistic.ru/theory/pirson.html


            }
        }


        /// <summary>
        /// Запрогаем формулу Пирсона от сюда https://ru.wikipedia.org/wiki/Корреляция
        /// </summary>
        /// <returns></returns>
        public double PirsonCorrelation(List<double> X, List<double> Y)
        {

            double Xsr = X.Average();
            double Ysr = Y.Average();

            double sum_chislit = 0.0;

            double kv_X_Xsr = 0.0;
            double kv_Y_Ysr = 0.0;

            for (int i = 0; i < X.Count; i++)
            {

                sum_chislit+=((X[i]-Xsr)*(Y[i]-Ysr));

                kv_X_Xsr += (X[i] - Xsr) * (X[i] - Xsr);
                kv_Y_Ysr += (Y[i] - Ysr) * (Y[i] - Ysr);

            }

            return sum_chislit/(Math.Sqrt(kv_X_Xsr)*Math.Sqrt(kv_Y_Ysr));
        }



        public double Correlation(List<double> a, List<double> b)
        {

            double sum = 0.0;
            //пожразумевая что буду подсовывать сигнал один и тотже
            for (int i = 0; i < a.Count; i++)
            {
                sum += a[i] * b[i];
            }
            return sum;
        }



    }
}
