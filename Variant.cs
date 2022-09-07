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
    using System;
    using System.Collections.Generic;
    using de.nanofocus.NFEval;
    public class FilePath
    {
        public string path;
        public string filename;

    }

    public class Variant
    {
        //public enum VariantDataType
        //{
        //    INTEGER = 0, DOUBLE = 1, STRING = 2, BOOL = 3, STRINGLIST = 5, INTLIST = 6, DOUBLELIST = 7, FILEPATH = 8, UNDEFINED = 9
        //}
        public Variant()
        {
            mData = 0;
            mDataType = 0;
            mUnit = "";

        }
        public Variant(int value, string unit)
        {
            mDataType = NFVariant.DataType.INT_TYPE;

            mData = value;
            mUnit = unit;
        }
        public Variant(double value, string unit)
        {
            mDataType = NFVariant.DataType.DOUBLE_TYPE;

            mData = value;
            mUnit = unit;
        }

        public Variant(string value)
        {
            mDataType = NFVariant.DataType.STRING_TYPE;

            mData = value;
            mUnit = "";
        }

        public Variant(bool value)
        {
            mDataType = NFVariant.DataType.BOOL_TYPE;

            mData = value;
            mUnit = "";
        }
        public Variant(List<string> value)
        {
            mDataType = NFVariant.DataType.STRING_VECTOR_TYPE;

            mData = value;
        }

        public Variant(List<int> value)
        {
            mDataType = NFVariant.DataType.INT_VECTOR_TYPE;

            mData = value;
        }
        public Variant(List<double> value)
        {
            mDataType = NFVariant.DataType.DOUBLE_VECTOR_TYPE;

            mData = value;
        }


        public int getInt()
        {
            return Convert.ToInt32(mData);
        }
        public double getDouble()
        {
            return Convert.ToDouble(mData);
        }
        public string getString()
        {
            return Convert.ToString(mData);
        }

        public bool getBool()
        {
            return Convert.ToBoolean(mData);
        }


        public List<string> getStringList()
        {
            if (mDataType == NFVariant.DataType.INT_TYPE) return (List<string>)mData;
            else return new List<string>() { Convert.ToString(mData) };
        }

        public List<int> getIntList()
        {
            if (mDataType == NFVariant.DataType.INT_VECTOR_TYPE) return (List<int>)mData;
            else return new List<int>() { Convert.ToInt32(mData) };
        }
        public List<double> getDoubleList()
        {
            if (mDataType == NFVariant.DataType.DOUBLE_VECTOR_TYPE) return (List<double>)mData;
            else return new List<double>() { Convert.ToDouble(mData) };
        }

        public void setInt(int value)
        {
            mData = value;
            mDataType = NFVariant.DataType.INT_TYPE;
        }
        public void setDouble(double value)
        {
            mData = value;
            mDataType = NFVariant.DataType.DOUBLE_TYPE;
        }
        public void setString(string value)
        {
            mData = value;
            mDataType = NFVariant.DataType.STRING_TYPE;
        }
        public void setBool(bool value)
        {
            mData = value;
            mDataType = NFVariant.DataType.BOOL_TYPE;
        }
        public void setStringList(List<string> value)
        {
            mData = value;
            mDataType = NFVariant.DataType.STRING_VECTOR_TYPE;
        }

        public void setIntList(List<int> value)
        {
            mData = value;
            mDataType = NFVariant.DataType.INT_VECTOR_TYPE;
        }

        public void setDoubleList(List<double> value)
        {
            mData = value;
            mDataType = NFVariant.DataType.DOUBLE_VECTOR_TYPE;
        }


        //public VariantDataType getDataType()
        //{
        //    return mDataType;
        //}


        public string getUnit()
        {
            return mUnit;
        }

        #region Private   Fields
        private object mData;
        //private VariantDataType mDataType;
        private NFVariant.DataType mDataType;
        private string mUnit;
        #endregion 



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
        public List<int> asIntList
        {
            set
            {
                //data.setIntList(value);
                Vector2DList intList = new Vector2DList();
                foreach (var item in value)
                {
                    intList.Add(item);
                }
                data.setVector2DList(intList);
                //data.setInt(intList);
            }
            get
            {
                return data.getIntList();
            }
        }
        public List<double> asDoubleList
        {
            set
            {
                data.setDoubleList(value);
            }
            get
            {
                return data.getDoubleList();
            }
        }
        private NFVariant data;
    }
}