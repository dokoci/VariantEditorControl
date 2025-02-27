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
namespace DataTypes
{
    using de.nanofocus.NFEval;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class FilePath
    {
        public string path;
        public string filename;
    }

    class VariantBindingProperties
    {
        public VariantBindingProperties(NFVariant v)
        {
            data = v;
        }

        public int asInteger
        {
            set => data.setInt(value);
            get => data.getInt();
        }

        public double asDouble
        {
            set => data.setDouble(value);
            get => data.getDouble();
        }

        public float asFloat
        {
            set => data.setFloat(value);
            get => data.getFloat();
        }
        public string asFloatString
        {
            set => data.setFloat((float)Convert.ToDecimal(value));
            //get => data.getFloat().ToString("0.##########"); 
            get => data.getFloat().ToString("G");
        }
        public string asString
        {
            set => data.setString(value);
            get => data.getString();
        }

        public bool asBool
        {
            set => data.setBool(value);
            get => data.getBool();
        }

        public List<string> asStringList
        {
            set
            {
                NFParameterNameListType list = new NFParameterNameListType();

                foreach (var element in value)
                {
                    list.Add(element);
                }

                data.setStringVector(list);
            }
            get
            {
                List<string> list = new List<string>();
                foreach (var element in data.getStdStringVector())
                {
                    list.Add(element);
                }
                return list;
            }
        }

        public List<long> asIntList
        {
            set
            {
                long[] intList = new long[value.Count];
                //data.setIntList(value);

                int i = 0;
                foreach (var item in value)
                {

                    intList[i] = item;
                    i++;
                }

                data.setIntVector(intList, (uint)value.Count);
            }
            get
            {
                uint count = data.getNumberOfElements();
                long[] intList = new long[count];
                data.getIntVector(intList, count);

                return new List<long>(intList);
            }
        }

        public List<double> asDoubleList
        {
            set
            {
                double[] doubleList = new double[value.Count];
                //data.setIntList(value);

                int i = 0;
                foreach (var item in value)
                {

                    doubleList[i] = item;
                    i++;
                }

                data.setDoubleVector(doubleList, (uint)value.Count);
            }
            get
            {
                uint count = data.getNumberOfElements();
                double[] doubleList = new double[count];
                data.getDoubleVector(doubleList, count);
                //List<int> list = new List<int>();

                return new List<double>(doubleList);
            }
        }
        private NFVariant data;
    }
}