using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using de.nanofocus.NFEval;

namespace ExampleWinFormApp
{
    public partial class Form1 : Form
    {
        
        VariantEditorControl.VariantEditorControl vc = new VariantEditorControl.VariantEditorControl();
        NFParameterSetPointer data = NFParameterSet.New();
        NFParameterSetPointer dataMin = NFParameterSet.New();
        NFParameterSetPointer dataMax = NFParameterSet.New();
        NFParameterSetPointer dataDiscrete = NFParameterSet.New();

        public Dictionary<int, string> ComboList { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, int> MyList { get; set; } = new Dictionary<int, int>();
        
        
        private void toolTipCoordinates(object sender, ToolTipEventArgs e)
        {
            switch (e.HitTestResult.ChartElementType)
            {
                case ChartElementType.DataPoint:
                    DataPoint dPoint = e.HitTestResult.Series.Points[e.HitTestResult.PointIndex];
                    e.Text = string.Format("X:   {0}\nY:   {1}", dPoint.XValue, dPoint.YValues[0]);
                    break;
                default:
                    break;
            }
        }
        public Form1()
        {
            InitializeComponent();
            vc.IntListener += RefreshChart;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.GetToolTipText += toolTipCoordinates;

            MyList.Add(1, 5);
            MyList.Add(3, 2);
            MyList.Add(8, 9);
            MyList.Add(11, 8);
            MyList.Add(15, 9);

            ComboList[0] = "Zero";
            ComboList[1] = "One";
            ComboList[2] = "Two";
            ComboList[3] = "Three";
            ComboList[4] = "Four";
            ComboList[5] = "Five";
            comboBox1.DataSource = new BindingSource(ComboList, null);
            comboBox1.DisplayMember = "Key";
            comboBox1.ValueMember = "Value";


            data.setParameter("X", new NFVariant(19, NFUnitCls.Unit.NFUnitCustom));
            data.setParameter("Y", new NFVariant(16, NFUnitCls.Unit.NFUnitCustom));
            panel1.Controls.Add(vc);
            vc.SetDataList(data, dataMin, dataMax, dataDiscrete);
            RefreshChart();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label1.Text = comboBox1.SelectedValue.ToString();
            //label1.Text = data.getParameter("Param1").getInt().ToString();
        }

        private void RefreshChart()
        {
            chart1.Series.Clear();
            //chart1.Titles.Add("This is my Test Chart");
            chart1.ChartAreas[0].AxisY.Interval =   1;
            chart1.ChartAreas[0].AxisX.Interval =   1;
            chart1.ChartAreas[0].AxisY.Minimum =    0;
            chart1.ChartAreas[0].AxisY.Maximum =    20;
            chart1.ChartAreas[0].AxisX.Minimum =    0;
            chart1.ChartAreas[0].AxisX.Maximum =    20;

            Series series = chart1.Series.Add("Dictionary X , Y Values");
            series.ChartType = SeriesChartType.Spline;
            chart1.Series["Dictionary X , Y Values"].MarkerStyle = MarkerStyle.Circle;
            chart1.Series["Dictionary X , Y Values"].MarkerSize = 12;
            chart1.Series["Dictionary X , Y Values"].Color = Color.Green;
            series.Points.DataBindXY(MyList.Keys, MyList.Values);
            series.Points.AddXY(data.getParameter("X").getInt(), data.getParameter("Y").getInt());
        }

       
        private void button1_Click(object sender, EventArgs e)
        {
            RefreshChart();
        }


    }
}
