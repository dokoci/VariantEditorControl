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
namespace DataTypes
{
    using de.nanofocus.NFEval;
    using System.Collections.Generic;
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
            set
            {
                data.setInt(value);
            }
            get
            {
                return data.getInt();
            }
        }

        public double asDouble
        {
            set
            {
                data.setDouble(value);
            }
            get
            {
                return data.getDouble();
            }
        }

        public string asString
        {
            set
            {
                data.setString(value);
            }
            get
            {
                return data.getString();
            }
        }

        public bool asBool
        {
            set
            {
                data.setBool(value);
            }
            get
            {
                return data.getBool();
            }
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
                List<int> list = new List<int>();

                return new List<long>(intList);
            }
        }
        public List<double> asDoubleList
        {
            set
            {
                double[] intList = new double[value.Count];
                //data.setIntList(value);

                int i = 0;
                foreach (var item in value)
                {

                    intList[i] = item;
                    i++;
                }


                data.setDoubleVector(intList, (uint)value.Count);

            }
            get
            {
                uint count = data.getNumberOfElements();
                double[] intList = new double[count];
                data.getDoubleVector(intList, count);
                List<int> list = new List<int>();

                return new List<double>(intList);
            }
        }
        private NFVariant data;
    }
}