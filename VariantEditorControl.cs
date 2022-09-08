﻿//MIT License

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
using System.Collections.Generic;
using System.Windows.Forms;

namespace VariantEditorControl
{


    // using VariantList = Dictionary<string, Variant>; // NFParameterSet


    public partial class VariantEditorControl : UserControl
    {
        Dictionary<NFUnitCls.Unit, string> UnitToString = new Dictionary<NFUnitCls.Unit, string>() {
            { NFUnitCls.Unit.NFUnitNone, "" },
            { NFUnitCls.Unit.NFUnitInch, "Inch" },
            { NFUnitCls.Unit.NFUnitM, "Meter" },
            { NFUnitCls.Unit.NFUnitA, "Amper" },
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
            mNumberOfRows = 1;
            InitializeComponent();

            this.Load += (s, e) =>
            {
                CheckAutoScroll(s);

            };

            this.Resize += (s, e) =>
            {
                CheckAutoScroll(s);

            };

        }


        private void CheckAutoScroll(object s)
        {
            int totalHeight = CalculateRowHeights();
            if ((s as Control).Height <= totalHeight)
            {
                this.AutoScroll = true;
                this.AutoScrollMinSize = new System.Drawing.Size(0, totalHeight);
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

            var size = new System.Drawing.Size(85, 20);

            foreach (var parameterName in data.getParameterNames())
            {
                NFVariant value = data.getParameter(parameterName);
                NFVariant valueMin = dataMin.getParameter(parameterName);
                NFVariant valueMax = dataMax.getParameter(parameterName);

                switch (data.getParameter(parameterName).getType())
                {
                    case NFVariant.DataType.INT_TYPE: // NFVariant.Type


                        Label lIntegerName = new Label();
                        lIntegerName.Text = (parameterName);
                        mainTable.Controls.Add(lIntegerName, 0, rowIndex);


                        if (dataDiscrete.containsParameter(parameterName))
                        {
                            ComboBox cbxInteger = new ComboBox();
                            cbxInteger.Size = size;
                            cbxInteger.DataSource = dataDiscrete.getParameter(parameterName).getIntVector();
                            cbxInteger.SelectedIndexChanged += (s, e) =>
                            {
                                data.setParameter(parameterName, new NFVariant(System.Convert.ToInt32((s as ComboBox).SelectedItem)));
                            };


                            mainTable.Controls.Add(cbxInteger, 1, rowIndex);
                        }
                        else
                        {
                            NumericUpDown updwInteger = new NumericUpDown();
                            updwInteger.Size = size;
                            updwInteger.Minimum = dataMin.containsParameter(parameterName) ? dataMin.getParameter(parameterName).getInt() : 0;
                            updwInteger.Maximum = dataMax.containsParameter(parameterName) ? dataMax.getParameter(parameterName).getInt() : data.getParameter(parameterName).getInt() * 2; ;

                            updwInteger.DataBindings.Add("Value", new VariantBindingProperties(data.getParameter(parameterName)), "asInteger");
                            mainTable.Controls.Add(updwInteger, 1, rowIndex);



                        }
                        Label lIntegerUnit = new Label();

                        //value.getUnit();

                        lIntegerUnit.Text = UnitToString[value.getUnitType()];

                        mainTable.Controls.Add(lIntegerUnit, 2, rowIndex);



                        ++rowIndex;


                        break;


                    case NFVariant.DataType.DOUBLE_TYPE:


                        Label lDoubleName = new Label();
                        lDoubleName.Text = (parameterName);
                        mainTable.Controls.Add(lDoubleName, 0, rowIndex);


                        NumericUpDown updwnDouble = new NumericUpDown();
                        updwnDouble.Size = size;
                        updwnDouble.DecimalPlaces = 2;
                        updwnDouble.Increment = 0.1M;
                        updwnDouble.Minimum = dataMin.containsParameter(parameterName) ? (int)valueMin.getDouble() : 0;
                        updwnDouble.Maximum = dataMax.containsParameter(parameterName) ? (int)valueMax.getDouble() : (int)value.getDouble() * 2; ;


                        updwnDouble.DataBindings.Add("Value", new VariantBindingProperties(value), "asDouble");
                        mainTable.Controls.Add(updwnDouble, 1, rowIndex);

                        Label lDoubleUnit = new Label();
                        lDoubleUnit.Text = UnitToString[value.getUnitType()];


                        mainTable.Controls.Add(lDoubleUnit, 2, rowIndex);

                        ++rowIndex;

                        break;



                    case NFVariant.DataType.STRING_TYPE:


                        Label lStringName = new Label();
                        lStringName.Text = (parameterName);
                        mainTable.Controls.Add(lStringName, 0, rowIndex);

                        if (dataDiscrete.containsParameter(parameterName))
                        {
                            ComboBox cbxString = new ComboBox();
                            cbxString.Size = size;
                            cbxString.DataSource = dataDiscrete.getParameter(parameterName).getStdStringVector();
                            cbxString.SelectedIndexChanged += (s, e) =>
                            {
                                value.setString((string)(s as ComboBox).SelectedItem);
                            };


                            mainTable.Controls.Add(cbxString, 1, rowIndex);
                        }
                        else
                        {
                            TextBox txtString = new TextBox();
                            txtString.Size = size;
                            txtString.DataBindings.Add("Text", new VariantBindingProperties(value), "asString");

                            mainTable.Controls.Add(txtString, 1, rowIndex);
                        }




                        Label lStringUnit = new Label();
                        lStringUnit.Text = UnitToString[value.getUnitType()];


                        mainTable.Controls.Add(lStringUnit, 2, rowIndex);


                        ++rowIndex;
                        break;


                    case NFVariant.DataType.BOOL_TYPE:
                        Label lBoolName = new Label();
                        lBoolName.Text = (parameterName);
                        mainTable.Controls.Add(lBoolName, 0, rowIndex);

                        CheckBox cbxBool = new CheckBox();
                        cbxBool.Size = size;
                        cbxBool.DataBindings.Add("Checked", new VariantBindingProperties(value), "asBool");

                        mainTable.Controls.Add(cbxBool, 1, rowIndex);

                        Label lBoolUnit = new Label();
                        lBoolUnit.Text = UnitToString[value.getUnitType()];


                        mainTable.Controls.Add(lBoolUnit, 2, rowIndex);

                        ++rowIndex;

                        break;

                    case NFVariant.DataType.STRING_VECTOR_TYPE:
                        Label lStringListName = new Label();
                        lStringListName.Text = (parameterName);
                        mainTable.Controls.Add(lStringListName, 0, rowIndex);

                        ComboBox cbxStringList = new ComboBox();
                        cbxStringList.Size = size;
                        cbxStringList.DataSource = new VariantBindingProperties(value).asStringList;
                        mainTable.Controls.Add(cbxStringList, 1, rowIndex);

                        Label lStringListUnit = new Label();
                        lStringListUnit.Text = UnitToString[value.getUnitType()];
                        mainTable.Controls.Add(lStringListUnit, 2, rowIndex);

                        ++rowIndex;
                        break;

                    default:
                        break;

                }

            }
            mNumberOfRows = rowIndex - 1;
            this.Invalidate();

            return;
        }

        private void CreateTableHeader()
        {
            const int rowIndex = 0;
            Label l00 = new Label();
            l00.Text = ("Name");

            l00.Font = new System.Drawing.Font(Label.DefaultFont, System.Drawing.FontStyle.Bold);

            Label l10 = new Label();
            l10.Text = ("Value");
            l10.Font = new System.Drawing.Font(Label.DefaultFont, System.Drawing.FontStyle.Bold);

            Label l20 = new Label();
            l20.Anchor = AnchorStyles.Left;
            l20.Text = ("Unit");

            l20.Font = new System.Drawing.Font(Label.DefaultFont, System.Drawing.FontStyle.Bold);

            mainTable.Controls.Add(l00, 0, rowIndex);
            mainTable.Controls.Add(l10, 1, rowIndex);
            mainTable.Controls.Add(l20, 2, rowIndex);
            mainTable.RowStyles[rowIndex].SizeType = SizeType.AutoSize;
            return;
        }
    }
}
