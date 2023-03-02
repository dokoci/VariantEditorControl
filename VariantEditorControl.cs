//MIT License

//Copyright (c) 2022 Markus Leitz MLeitz at boptics.de

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using DataTypes;
using de.nanofocus.NFEval;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace VariantEditorControl
{

    public delegate void Notify();
    // using VariantList = Dictionary<string, Variant>; // NFParameterSet

    //delegate OnParameterChange;
    //event
    public partial class VariantEditorControl : UserControl
    {
        //public event EventHandler IntListener; // testing..
        public event Notify IntListener;

        Dictionary<NFUnitCls.Unit, string> UnitToString = new Dictionary<NFUnitCls.Unit, string>() {
            { NFUnitCls.Unit.NFUnitNone, "" },
            { NFUnitCls.Unit.NFUnitInch, "Inch" },
            { NFUnitCls.Unit.NFUnitM, "Meter" },
            { NFUnitCls.Unit.NFUnitA, "Ampere" },
            { NFUnitCls.Unit.NFUnitV, "Volt" },
            { NFUnitCls.Unit.NFUnitN, "Newton" },
            { NFUnitCls.Unit.NFUnitDeg, "Degree" },
            { NFUnitCls.Unit.NFUnitRad, "Radian" },
            { NFUnitCls.Unit.NFUnitK, "Kelvin" },
            { NFUnitCls.Unit.NFUnitDegC, "Celsius" },
            { NFUnitCls.Unit.NFUnitDegF, "Farenheit" },
            { NFUnitCls.Unit.NFUnitPerCent, "Percent" },
            { NFUnitCls.Unit.NFUnitSec, "Second" },
            { NFUnitCls.Unit.NFUnitHz, "Hertz" },
            { NFUnitCls.Unit.NFUnitDigit, "Digit" },
            { NFUnitCls.Unit.NFUnitInputFileName, "InputFileName" },
            { NFUnitCls.Unit.NFUnitInputDirectoryName, "InputDirectoryName" },
            { NFUnitCls.Unit.NFUnitBitMask, "BitMask" },
            { NFUnitCls.Unit.NFUnitOutputFileName, "OutputFileName" },
            { NFUnitCls.Unit.NFUnitOutputDirectoryName, "OutputDirectoryName" },
            { NFUnitCls.Unit.NFUnitCustom, "Custom" },
            { NFUnitCls.Unit.NFUnitInvalidUnit, "Invalid Unit" }
        };

        private int mNumberOfRows;
        private NFParameterSetPointer p = NFParameterSet.New();
        public VariantEditorControl()
        {
            mNumberOfRows = 4;
            InitializeComponent();

            Load += (s, e) =>
            {
                CheckAutoScroll(s);

            };

            Resize += (s, e) =>
            {
                CheckAutoScroll(s);

            };

        }


        private void CheckAutoScroll(object s)
        {
            int totalHeight = CalculateRowHeights();
            if ((s as Control).Height <= totalHeight)
            {
                AutoScroll = true;
                AutoScrollMinSize = new System.Drawing.Size(0, totalHeight);
            }
            else
            {
                this.AutoScroll = false;
            }
        }

        private int CalculateRowHeights()
        {

            Dictionary<int, int> RowHeights = new Dictionary<int, int>();

            var list = mainTable.Controls;

            foreach (Control element in list)
            {
                int key = mainTable.GetRow(element);
                var margin = element.Margin.Top + element.Margin.Bottom;

                if (RowHeights.ContainsKey(key))
                    RowHeights[key] = element.Height > RowHeights[key] ? element.Height + margin : RowHeights[key];
                else
                    RowHeights.Add(key, element.Height + margin);

            }

            int height = 0;
            foreach (var row in RowHeights)
            {
                height += row.Value;
            }
            return height;
        }

        public void SetDataList(/*VariantList*/ NFParameterSetPointer data, NFParameterSetPointer dataMin, NFParameterSetPointer dataMax, NFParameterSetPointer dataDiscrete)
        {

            mainTable.Controls.Clear();

            CreateTableHeader();

            int rowIndex = 1;
            System.Drawing.Size size = new System.Drawing.Size(85, 20);

            foreach (var parameterName in data.getParameterNames())
            {
                NFVariant value = data.getParameter(parameterName);
                NFVariant valueMin = dataMin.getParameter(parameterName);
                NFVariant valueMax = dataMax.getParameter(parameterName);
                NFVariant valueDisc = dataDiscrete.getParameter(parameterName);

                switch (data.getParameter(parameterName).getType())
                {
                    case NFVariant.DataType.INT_TYPE: // NFVariant.Type

                        Label lIntegerName = new Label
                        {
                            Text = parameterName
                        };
                        mainTable.Controls.Add(lIntegerName, 0, rowIndex);

                        if (dataDiscrete.containsParameter(parameterName))
                        {
                            ComboBox cbxInteger = new ComboBox
                            {
                                Size = size
                            };

                            uint count = valueDisc.getNumberOfElements();
                            long[] intList = new long[count];
                            valueDisc.getIntVector(intList, count);
                            List<long> list = new List<long>(intList);
                            cbxInteger.DataSource = list;

                            cbxInteger.SelectedIndexChanged += (s, e) =>
                            {
                                data.setParameter(parameterName, new NFVariant(Convert.ToInt32((s as ComboBox).SelectedItem)));
                                IntListener?.Invoke();
                            };

                            mainTable.Controls.Add(cbxInteger, 1, rowIndex);
                        }
                        else
                        {
                            NumericUpDown updwInteger = new NumericUpDown()
                            {
                                Size = size,
                                Minimum = dataMin.containsParameter(parameterName) ? dataMin.getParameter(parameterName).getInt() : 0,
                                Maximum = 20/*dataMax.containsParameter(parameterName) ? dataMax.getParameter(parameterName).getInt() : data.getParameter(parameterName).getInt() * 2; */
                            };

                            updwInteger.DataBindings.Add("Value", new VariantBindingProperties(data.getParameter(parameterName)), "asInteger");
                            mainTable.Controls.Add(updwInteger, 1, rowIndex);

                            updwInteger.ValueChanged += (s, e) =>
                            {
                                //System.Console.WriteLine(" Parameter " + parameterName + " " + s.ToString() + e.ToString());
                                //OnParameterChange(parameterName);
                                data.setParameter(parameterName, new NFVariant(Convert.ToInt32(updwInteger.Value)));
                                IntListener?.Invoke();
                            };

                        }
                        Label lIntegerUnit = new Label
                        {

                            //value.getUnit();

                            Text = UnitToString[value.getUnitType()]
                        };

                        mainTable.Controls.Add(lIntegerUnit, 2, rowIndex);
                        
                        ++rowIndex;


                        break;


                    case NFVariant.DataType.DOUBLE_TYPE:


                        Label lDoubleName = new Label
                        {
                            Text = parameterName
                        };
                        mainTable.Controls.Add(lDoubleName, 0, rowIndex);


                        NumericUpDown updwnDouble = new NumericUpDown
                        {
                            Size = size,
                            DecimalPlaces = 2,
                            Increment = 0.1M,
                            Minimum = dataMin.containsParameter(parameterName) ? (int)valueMin.getDouble() : 0
                        };
                        //updwnDouble.Maximum = dataMax.containsParameter(parameterName) ? (int)valueMax.getDouble() : (int)value.getDouble() * 2; ;


                        updwnDouble.DataBindings.Add("Value", new VariantBindingProperties(value), "asDouble");
                        mainTable.Controls.Add(updwnDouble, 1, rowIndex);

                        Label lDoubleUnit = new Label
                        {
                            Text = UnitToString[value.getUnitType()]
                        };


                        mainTable.Controls.Add(lDoubleUnit, 2, rowIndex);

                        ++rowIndex;

                        break;



                    case NFVariant.DataType.STRING_TYPE:


                        Label lStringName = new Label
                        {
                            Text = parameterName
                        };
                        mainTable.Controls.Add(lStringName, 0, rowIndex);

                        if (dataDiscrete.containsParameter(parameterName))
                        {
                            ComboBox cbxString = new ComboBox
                            {
                                Size = size,
                                DataSource = dataDiscrete.getParameter(parameterName).getStdStringVector()
                            };
                            cbxString.SelectedIndexChanged += (s, e) =>
                            {
                                value.setString((string)(s as ComboBox).SelectedItem);
                            };


                            mainTable.Controls.Add(cbxString, 1, rowIndex);
                        }
                        else
                        {
                            TextBox txtString = new TextBox
                            {
                                Size = size
                            };
                            txtString.DataBindings.Add("Text", new VariantBindingProperties(value), "asString");

                            mainTable.Controls.Add(txtString, 1, rowIndex);
                        }




                        Label lStringUnit = new Label
                        {
                            Text = UnitToString[value.getUnitType()]
                        };


                        mainTable.Controls.Add(lStringUnit, 2, rowIndex);


                        ++rowIndex;
                        break;


                    case NFVariant.DataType.BOOL_TYPE:
                        Label lBoolName = new Label
                        {
                            Text = (parameterName)
                        };
                        mainTable.Controls.Add(lBoolName, 0, rowIndex);

                        CheckBox cbxBool = new CheckBox
                        {
                            Size = size
                        };
                        cbxBool.DataBindings.Add("Checked", new VariantBindingProperties(value), "asBool");

                        mainTable.Controls.Add(cbxBool, 1, rowIndex);

                        Label lBoolUnit = new Label
                        {
                            Text = UnitToString[value.getUnitType()]
                        };


                        mainTable.Controls.Add(lBoolUnit, 2, rowIndex);

                        ++rowIndex;

                        break;

                    case NFVariant.DataType.STRING_VECTOR_TYPE:
                        Label lStringListName = new Label
                        {
                            Text = (parameterName)
                        };
                        mainTable.Controls.Add(lStringListName, 0, rowIndex);

                        ComboBox cbxStringList = new ComboBox
                        {
                            Size = size,
                            DataSource = new VariantBindingProperties(value).asStringList
                        };
                        mainTable.Controls.Add(cbxStringList, 1, rowIndex);

                        Label lStringListUnit = new Label
                        {
                            Text = UnitToString[value.getUnitType()]
                        };
                        mainTable.Controls.Add(lStringListUnit, 2, rowIndex);

                        ++rowIndex;
                        break;

                    default:
                        break;

                }

            }
            mNumberOfRows = rowIndex - 1;
            Invalidate();

            return;
        }

        private void CreateTableHeader()
        {
            const int rowIndex = 0;
            Label l00 = new Label
            {
                Text = "Name",

                Font = new System.Drawing.Font(DefaultFont, System.Drawing.FontStyle.Bold)
            };

            Label l10 = new Label
            {
                Text = "Value",
                Font = new System.Drawing.Font(DefaultFont, System.Drawing.FontStyle.Bold)
            };

            Label l20 = new Label
            {
                Anchor = AnchorStyles.Left,
                Text = "Unit",

                Font = new System.Drawing.Font(DefaultFont, System.Drawing.FontStyle.Bold)
            };

            Label lTest = new Label { Text = "test" };
            mainTable.Controls.Add(l00, 0, rowIndex);
            mainTable.Controls.Add(l10, 1, rowIndex);
            mainTable.Controls.Add(l20, 2, rowIndex);
            mainTable.Controls.Add(lTest, 3, rowIndex);
            mainTable.RowStyles[rowIndex].SizeType = SizeType.AutoSize;
            return;
        }
    }
}
