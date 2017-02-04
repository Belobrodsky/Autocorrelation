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
            //ghgtfuftuft
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


                //Копия для корреляции по Спирману
                List<double> ForSpirman = new List<double>(inputVec);
                List<double> ForSpirman2 = new List<double>(inputVec2);



                List<double> prefinal = new List<double>();

                int half = inputVec.Count / 2;
                while (inputVec.Count>half)
                {
                    prefinal.Add(PirsonCorrelation(inputVec, inputVec2));
                    //sw.WriteLine(PirsonCorrelation(inputVec, inputVec2));
                    inputVec.RemoveAt(0);
                    inputVec2.RemoveAt(inputVec2.Count-1);
                }


                if (numericUpDown1.Value>0)
                {

                    if (((int)numericUpDown1.Value > prefinal.Count) || (textBox1.Text.Length < 1))
                    {
                        MessageBox.Show(String.Format("отсечку невозможно сделать, так как введенное значение {0} больше длины результирующего сигнала {1} или отсечка не задана", numericUpDown1.Value, prefinal.Count));
                    }
                    else
                    {
                        for (int i = (int)numericUpDown1.Value; i < prefinal.Count; i++)
                        {
                            double maxValue = double.Parse(textBox1.Text);
                            if (prefinal[i]>maxValue)
                            {
                                prefinal[i] = maxValue;
                            }
                        }
                    }
                }
               
                StreamWriter sw = new StreamWriter(openFileDialog1.FileName + "_autocorrPirson.csv");

                for (int i = 0; i < prefinal.Count; i++)
                {
                    sw.WriteLine(prefinal[i]);
                }

                sw.Close();






               
                int half2 = ForSpirman.Count / 2;


                List<double> prefinalSpir = new List<double>();

                while (ForSpirman.Count > half)
                {
                    // sw2.WriteLine(PirsonCorrelation(ForSpirman, ForSpirman2));

                    prefinalSpir.Add(SpirmanCorrelation(ForSpirman, ForSpirman2));
                    ForSpirman.RemoveAt(0);
                    ForSpirman2.RemoveAt(ForSpirman2.Count - 1);
                }




                if (numericUpDown1.Value > 0)
                {

                    if (((int)numericUpDown1.Value > prefinalSpir.Count) || (textBox1.Text.Length<1))
                    {
                        MessageBox.Show(String.Format("отсечку невозможно сделать, так как введенное значение {0} больше длины результирующего сигнала {1} или отсечка не задана", numericUpDown1.Value, prefinalSpir.Count));
                    }
                    else
                    {
                        for (int i = (int)numericUpDown1.Value; i < prefinalSpir.Count; i++)
                        {
                            double maxValue = double.Parse(textBox1.Text);
                            if (prefinalSpir[i] > maxValue)
                            {
                                prefinalSpir[i] = maxValue;
                            }
                        }
                    }
                }



                StreamWriter sw2 = new StreamWriter(openFileDialog1.FileName + "_autocorrSpirman.csv");
                for (int i = 0; i < prefinal.Count; i++)
                {
                    sw2.WriteLine(prefinalSpir[i]);
                }
                sw2.Close();






                //Туровский просит сделать очевидно как тут http://medstatistic.ru/theory/pirson.html


            }
        }



        /// <summary>
        /// Запрогаем формулу коэффициента ранговой корреляции Спирмена, например, отсюда http://www.uchimatchast.ru/teory/spirman_primer.php
        /// ВНИМАНИЕ подсовываем копию объектов LIst при вызове функции, чтобы не испортить
        /// </summary>
        /// <returns></returns>
        public double SpirmanCorrelation(List<double> X, List<double> Y)
        {

//№ X Y 
//1 500 5.4 
//2 790 4.2 
//3 870 4.0 
//4 1500 3.4 
//5 2300 2.5 
//6 5600 1.0 
//7 100 6.1 
//8 20 8.2 
//9 5 14.6 

            

//n X ранг, Rx Y ранг, Ry разность рангов D, Rx-Ry D2 
//1 500 4 5.4 6 -2 4 
//2 790 5 4.2 5 0 0 
//3 870 6 4.0 4 2 4 
//4 1500 7 3.4 3 4 16 
//5 2300 8 2.5 2 6 36 
//6 5600 9 1.0 1 8 64 
//7 100 3 6.1 7 -4 16 
//8 20 2 8.2 8 -6 36 
//9 5 1 14.6 9 -8 64 


            //X.Sort();
            //Y.Sort();
            //Сюда будем заносить ранги
            List<double> RangeX = new List<double>();
            List<double> RangeY = new List<double>();  


          List<double> sortX = new List<double>(X);
          List<double> sortY = new List<double>(Y);

          sortX.Sort();
          sortY.Sort();


            for (int i = 0; i < X.Count; i++)
            {
                RangeX.Add(sortX.FindIndex(n => n == X[i]));
                RangeY.Add(sortY.FindIndex(n => n == Y[i]));        
            }

            //Ранги якобы нашли


            double D2 = 0.0;

            for (int i = 0; i < RangeX.Count; i++)
            {
                D2 += (RangeX[i] - RangeY[i]) * (RangeX[i] - RangeY[i]);
            }



            return (1-(6*D2)/(X.Count*(X.Count*X.Count-1)));
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



        private void button1_Click(object sender, EventArgs e)
        {
          //  List<double> X = new List<double>() { 500,790,870,1500,2300,5600,100,20,5};
          //  List<double> Y = new List<double>() { 5.4, 4.2, 4.0, 3.4, 2.5, 1.0, 6.1, 8.2, 14.6};

            List<double> X = new List<double>() { 3,5,6,1,4,11,9,2,8,7,10};
            List<double> Y = new List<double>() { 2,7,8,3,4,6,11,1,10,5,9};

            MessageBox.Show(SpirmanCorrelation(X, Y).ToString());
        
        }



    }
}
