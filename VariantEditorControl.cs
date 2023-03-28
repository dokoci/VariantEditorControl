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
using System.Drawing;
using NFVectorComboBox;
using NFNumericTextBox;
namespace VariantEditorControl
{

    public delegate void Notify();
    // using VariantList = Dictionary<string, Variant>; // NFParameterSet

    public partial class VariantEditorControl : UserControl
    {
        public event Notify IntListener;

        Dictionary<NFUnitCls.Unit, string> UnitToString = new Dictionary<NFUnitCls.Unit, string>() {
            { NFUnitCls.Unit.NFUnitNone, "None" },
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

        Dictionary<NFVariant.DataType, string> TypeToString = new Dictionary<NFVariant.DataType, string>()
        {
            {NFVariant.DataType.BOOL_TYPE, "Bool" },
            {NFVariant.DataType.DOUBLE_TYPE, "Double" },
            {NFVariant.DataType.DOUBLE_VECTOR_TYPE, "Double Vector" },
            {NFVariant.DataType.FLOAT_TYPE, "Float" },
            {NFVariant.DataType.INT_TYPE, "Int" },
            {NFVariant.DataType.INT_VECTOR_TYPE, "Int Vector" },
            {NFVariant.DataType.STRING_TYPE, "String" },
            {NFVariant.DataType.STRING_VECTOR_TYPE, "String Vector" },
            {NFVariant.DataType.UNDEFINED_TYPE, "Undefined" }
        };
        private int mNumberOfRows;
        private NFParameterSetPointer p = NFParameterSet.New();

        private NFParameterSetPointer parameterSet;
        private NFParameterSetReaderPointer reader = NFParameterSetReader.New();
        private NFParameterSetWriterPointer writer = NFParameterSetWriter.New();

       public  string hexStringOriginal = null;
       public  string hexStringEditet = null;

        private BindingSource bindingSource = new BindingSource();
        public void LoadData(string path)
        {
            mainTable.Controls.Clear();

            CreateTableHeader();
            string tempStr = path;
            //reader.setSource("C:\\Users\\koci\\Desktop\\NFMsurfControl.npsx");
            reader.setSource(tempStr);
            bool success = reader.read();
            //System.Drawing.Size size = new System.Drawing.Size(150, 20);
            if (success)
            {
                parameterSet = reader.getParameterSet();
                hexStringOriginal = parameterSet.toProtoBufHexString();
                ComboBox cb = new ComboBox()
                {
                    Size = new Size(170, 50),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                List<string> ls = new List<string>(parameterSet.getParameterNames());
                cb.DataSource = ls;
                //Console.WriteLine(parameterSet.getParameter("MPD_CONTROL_TIMEOUT").getInt());
                mainTable.Controls.Add(cb, 1, 1);
                string assign = "";

                Label type = new Label()
                {
                    Font = new Font("Arial", 10)
                };
                Label unit = new Label()
                {
                    Font = new Font("Arial", 10)
                };
                Label Multiplicator = new Label()
                {
                    Font = new Font("Arial", 10)
                };
                Label Exponent = new Label()
                {
                    Font = new Font("Arial", 10)
                };
                TextBox StringValueText = new TextBox()
                {
                    Size = new Size(200, 30),
                    Font = new Font("Arial", 10)
                };
                NumericUpDown NumVal = new NumericUpDown()
                {
                    Enabled = false
                };
                ComboBox cbVectorInt = new ComboBox
                {
                    Size = new Size(150, 20)
                };
                ComboBox cbVectorDouble = new ComboBox
                {
                    Size = new Size(150, 20)
                };

                NumericTextBox numericTextBox = new NumericTextBox()
                {
                    Size = new Size(200, 30),
                    Font = new Font("Arial", 10)
                };

                VectorComboBox nfCombo = new VectorComboBox();

                cb.SelectedIndexChanged += (s, e) =>
                    {
                        mainTable.Controls.Remove(StringValueText);
                        mainTable.Controls.Remove(numericTextBox);
                        mainTable.Controls.Remove(NumVal);
                        mainTable.Controls.Remove(cbVectorInt);
                        mainTable.Controls.Remove(cbVectorDouble);
                        mainTable.Controls.Remove(nfCombo);
                        NumVal.Refresh();
                        numericTextBox.Refresh();
                        StringValueText.Refresh();

                        NumVal.DataBindings.Clear();
                        StringValueText.DataBindings.Clear();
                        numericTextBox.DataBindings.Clear();
                        nfCombo.bindingSource.Clear();
                        bindingSource.Clear();

                        StringValueText.Text = "";
                        numericTextBox.Text = "";

                        string str = (string)(s as ComboBox).SelectedValue;
                        NFVariant val = parameterSet.getParameter(str);

                            switch (parameterSet.getParameter(str).getType())
                            {
                            case NFVariant.DataType.INT_TYPE:
                                assign = "asInteger";
                                NumVal.Enabled = true;
                                NumVal.Maximum = int.MaxValue;
                                NumVal.Minimum = int.MinValue;
                                NumVal.DataBindings.Add("Value", new VariantBindingProperties(val), assign);
                                mainTable.Controls.Add(NumVal, 6, 6);
                                break;

                            case NFVariant.DataType.INT_VECTOR_TYPE:
                                bindingSource.Clear();
                                nfCombo.MyProperty = val;
                                mainTable.Controls.Add(nfCombo, 6, 6);

                                uint count = val.getNumberOfElements();
                                long[] intList = new long[count];
                                val.getIntVector(intList, count);
                              
                                bindingSource.DataSource = new VariantBindingProperties(val).asIntList;
                                nfCombo.bindingSource.DataSource = bindingSource;
                                
                                break;

                            case NFVariant.DataType.DOUBLE_VECTOR_TYPE:
                                bindingSource.Clear();
                                nfCombo.MyProperty = val;
                                mainTable.Controls.Add(nfCombo, 6, 6);

                                uint doubleCount = val.getNumberOfElements();
                                double[] doubleList = new double[doubleCount];
                                val.getDoubleVector(doubleList, doubleCount);

                                bindingSource.DataSource = new VariantBindingProperties(val).asDoubleList;
                                nfCombo.bindingSource.DataSource = bindingSource;
                                break;

                            case NFVariant.DataType.FLOAT_TYPE:
                                assign = "asFloat";
                                mainTable.Controls.Add(numericTextBox, 6, 6);
                                numericTextBox.DataBindings.Add("Text", new VariantBindingProperties(val), assign, true, DataSourceUpdateMode.OnPropertyChanged);
                                break;
                                
                            case NFVariant.DataType.STRING_TYPE:
                                assign = "asString";
                                StringValueText.Text = val.valueToString();
                                mainTable.Controls.Add(StringValueText, 6, 6);
                               
                                StringValueText.DataBindings.Add("Text", new VariantBindingProperties(val), assign, true, DataSourceUpdateMode.OnPropertyChanged);
                                break;

                            case NFVariant.DataType.BOOL_TYPE:
                                break;

                                default:
                                    break;
                            }

                        type.Text = TypeToString[val.getType()];
                        unit.Text = UnitToString[val.getUnitType()];
                        Multiplicator.Text = val.getUnitMultiplicator().ToString();
                        Exponent.Text = val.getUnitExponent().ToString();
                    };

                mainTable.Controls.Add(type,1,6);
                mainTable.Controls.Add(unit,3,6);
                mainTable.Controls.Add(Multiplicator,4, 6);
                mainTable.Controls.Add(Exponent, 5, 6);
            }
            return;
        }

        public void SaveData(string location)
        {
            if (location != null)
            {
                string dest = location;
                //writer.setDestination("C:\\Users\\koci\\Desktop\\NFMsurfControl_Edited.npsx");
                writer.setDestination(dest);
                //hexStringEditet = parameterSet.toProtoBufHexString();
                writer.setParameterSet(parameterSet);
                writer.write();
            }
        }
        public VariantEditorControl()
        {
            mNumberOfRows = 1;
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
                AutoScroll = false;
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
            Size size = new Size(110, 30);
            const int rowIndex = 0;
            Label LType = new Label()
            {
                Size = size,
                Text = "Type",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.Black
            };
            Label LUnit = new Label()
            {
                Size = size,
                Text = "Unit",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.Black
            };
            Label LUnitMultiplicator = new Label()
            {
                Size = size,
                Text = "Multiplicator",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.Black
            };
            Label LUnitExponent = new Label()
            {
                Size = size,
                Text = "Exponent",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.Black
            };
            
            Label LValue = new Label()
            {
                Size = size,
                Text = "Value",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.Black
            };
           
            mainTable.Controls.Add(LValue, 6, 5);
            mainTable.Controls.Add(LUnitExponent, 5, 5);
            mainTable.Controls.Add(LUnitMultiplicator, 4, 5);
            mainTable.Controls.Add(LUnit, 3, 5);
            mainTable.Controls.Add(LType, 1, 5);
           
            mainTable.RowStyles[rowIndex].SizeType = SizeType.AutoSize;
            return;
        }
    }
}
