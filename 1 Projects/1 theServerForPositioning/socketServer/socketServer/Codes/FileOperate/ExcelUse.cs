using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace socketServer.Codes.FileOperate
{
    //这个类专门用于导出生excel数据
    class ExcelUse
    {
        //用的是最土鳖的方法，IO流方法
        //注意用"\t"分隔

        public DataTable DataGrid2Table(DataGrid dataGrid)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                if (dataGrid.Columns[i].Visibility == System.Windows.Visibility.Visible)//只导出可见列  
                {
                    dt.Columns.Add(dataGrid.Columns[i].Header.ToString());//构建表头  
                }
            }

            for (int i = 0; i < dataGrid.Items.Count; i++)
            {
                int columnsIndex = 0;
                System.Data.DataRow row = dt.NewRow();
                for (int j = 0; j < dataGrid.Columns.Count; j++)
                {
                    if (dataGrid.Columns[j].Visibility == System.Windows.Visibility.Visible)
                    {
                        if (dataGrid.Items[i] != null && (dataGrid.Columns[j].GetCellContent(dataGrid.Items[i]) as TextBlock) != null)//填充可见列数据  
                        {
                            row[columnsIndex] = (dataGrid.Columns[j].GetCellContent(dataGrid.Items[i]) as TextBlock).Text.ToString();
                        }
                        else row[columnsIndex] = "";

                        columnsIndex++;
                    }
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        //针对dataGrid的导出方法
        public string getExcelStringFromDataGrid (System.Windows.Controls.DataGrid dataGridIn)
        {
            string result = string.Empty;
            DataTable dt = new DataTable();
            dt = DataGrid2Table(dataGridIn);

               //try
               //{
               // 实例化流对象，以特定的编码向流中写入字符。 
               StringBuilder sb = new StringBuilder();
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    // 添加列名称  
                    sb.Append(dt.Columns[k].ColumnName.ToString() + "\t");
                }
                sb.Append(Environment.NewLine);
                // 添加行数据  
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    System.Data.DataRow row = dt.Rows[i];
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        // 根据列数追加行数据  
                        sb.Append(row[j].ToString() + "\t");
                    }
                    sb.Append(Environment.NewLine);
                }
                result = sb.ToString();
            //}
            //catch(Exception E)
            //{
            //    result = "";
            //    Console.WriteLine("导出DataGrid数据成为Excel表格失败");
            //    Console.WriteLine("失败原因：\n"+E.Message);
            //}
             return result;
        }



        //获得informationController中所有的数据
        //这个最好是打上保存用的时间戳，每一次保存都是一个文件
        //数据表项可能就会有重复，但是这是故意的保护
        public string  getExcelString(information theInformationController)
        {
            StringBuilder strbu = new StringBuilder();
            //写入标题
            strbu.Append("accelerometerX" + "\t");
            strbu.Append("accelerometerY" + "\t");
            strbu.Append("accelerometerZ" + "\t");
            strbu.Append("gyroX" + "\t");
            strbu.Append("gyroY" + "\t");
            strbu.Append("gyroZ" + "\t");
            strbu.Append("magnetometerX" + "\t");
            strbu.Append("magnetometerY" + "\t");
            strbu.Append("magnetometerZ" + "\t");
            strbu.Append("GPS(AxisX)" + "\t");
            strbu.Append("GPS(AxisY)" + "\t");
            strbu.Append("timeStamp" + "\t");
            //加入换行字符串
            strbu.Append(Environment.NewLine);

            //写入内容
            for(int i = 0; i < theInformationController.accelerometerX.Count; i++)
            {
                string dataClip = "";
                dataClip += theInformationController.accelerometerX[i] + "\t";
                dataClip += theInformationController.accelerometerY[i] + "\t";
                dataClip += theInformationController.accelerometerZ[i] + "\t";
                dataClip += theInformationController.gyroX[i] + "\t";
                dataClip += theInformationController.gyroY[i] + "\t";
                dataClip += theInformationController.gyroZ[i] + "\t";
                dataClip += theInformationController.magnetometerX[i] + "\t";
                dataClip += theInformationController.magnetometerY[i] + "\t";
                dataClip += theInformationController.magnetometerZ[i] + "\t";
                dataClip += theInformationController.GPSPositionX[i] + "\t";
                dataClip += theInformationController.GPSPositionY[i] + "\t";
                dataClip += theInformationController.timeStep[i] + "\t";
                strbu.Append(dataClip);
                strbu.Append(Environment.NewLine);
            }
            return strbu.ToString();
        }
    }
}
