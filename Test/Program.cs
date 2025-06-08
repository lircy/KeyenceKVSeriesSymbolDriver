using KEYENCE_KV_ETHERNET_TAG;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Test
{
    class Program
    {
        static KeyenceKVSeriesDriver commDriver = new KeyenceKVSeriesDriver();
        static void Main(string[] args)
        {
            uint status = commDriver.Connect("192.168.1.201");//此时连接的是基恩士KV8000系列PLC
            if (status == 0)
            {
                Console.WriteLine("连接PLC成功");
                string s = File.ReadAllText("varText.txt");
                List<string> variableNames = JsonConvert.DeserializeObject<List<string>>(s);
                List<string> variableNames_ = variableNames.GetRange(0, 1000);//读取1000个无序变量
                List<PValue> values = new List<PValue>();
                //在线获取变量的数据类型
                List<KeyenceDataType> dataTypes = commDriver.ReadType(variableNames_);
                //foreach (KeyenceDataType dataType in dataTypes)
                //{
                //    PValue value = null;
                //    switch (dataType)
                //    {
                //        case KeyenceDataType.BOOL:
                //            {
                //                value = new ValueBool(true);
                //                break;
                //            }
                //        case KeyenceDataType.INT:
                //            {
                //                value = new ValueInt(23);
                //                break;
                //            }
                //        case KeyenceDataType.UINT:
                //            {
                //                value = new ValueUInt(200);
                //                break;
                //            }
                //        case KeyenceDataType.DINT:
                //            {
                //                value = new ValueDInt(9999);
                //                break;
                //            }
                //        case KeyenceDataType.UDINT:
                //            {
                //                value = new ValueUDInt(30000);
                //                break;
                //            }
                //        case KeyenceDataType.REAL:
                //            {
                //                value = new ValueReal(3.14159f);
                //                break;
                //            }
                //        case KeyenceDataType.LREAL:
                //            {
                //                value = new ValueLReal(6.28);
                //                break;
                //            }
                //        case KeyenceDataType.STRING:
                //            {
                //                value = new ValueString("Hello World!");
                //                break;
                //            }
                //        case KeyenceDataType.TIMER:
                //            {
                //                //暂不支持
                //                break;
                //            }
                //        case KeyenceDataType.COUNTER:
                //            {
                //                //暂不支持
                //                break;
                //            }
                //    }
                //    values.Add(value);
                //}
                //写入值
                //status = commDriver.Write(variableNames, values);
                while (true)
                {
                    //读取值
                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();
                    status = commDriver.Read(variableNames_, out values);
                    stopwatch.Stop();
                    long ms = stopwatch.ElapsedMilliseconds;
                    if (status == 0)
                    {
                        for (int i = 0; i < values.Count; i++)
                            Console.WriteLine($"读取{values.Count}个变量耗时:{ms}ms {variableNames_[i]} = {values[i].ToString()}");
                    }
                }
            }
            else
            {
                Console.WriteLine("无法连接到PLC");
            }
            Console.ReadKey();
        }
    }
}
