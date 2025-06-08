﻿using KEYENCE_KV_ETHERNET_TAG;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyenceGUIBrowser
{
    public partial class Form1 : Form
    {
        private KeyenceKVSeriesDriver controller = new KeyenceKVSeriesDriver();
        public Form1()
        {
            InitializeComponent();
        }

        private void setStatus(string status)
        {
            lbStatus.Text = $"：{status}";
            lbStatus.Refresh();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string ip = tbIpAddress.Text;
            setStatus("connecting...");
            uint res = controller.Connect(ip);
            if (res == 0)
            {
                setStatus("connected");
                //DeviceInfo info = controller.ControllerInfo;
                //lbInfo.Text = $"PLC type：{info.DeviceName}|{info.Version}";
            }
            else
                setStatus("error");
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            setStatus("disconnecting...");
            controller.Disconnect();
            treeView1.Nodes.Clear();
            lbInfo.Text = $"PLC type：";
            tbValue.Text = string.Empty;
            tbSymbolicDatatype.Text = string.Empty;
            tbSymbol.Text = string.Empty;
            setStatus("disconnected");
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null) return;
            ItemAddress item = (ItemAddress)e.Node.Tag;
            if (item != null && item.GetSymbol(out string symbol) == 0)
            {
                tbSymbol.Text = symbol;
                tbSymbolicDatatype.Text = item.Datatype.ToString();
            }
        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Nodes.Count < 0 || e.Node.Nodes[0].Text != "Loading...") return;
            setStatus("loading...");
            e.Node.Nodes.Clear();
            ItemAddress item = (ItemAddress)e.Node.Tag;
            TreeNode tn;
            List<ItemAddress> itemInfoList = controller.Browse(item.Handle);
            foreach (ItemAddress itemAddress in itemInfoList)
            {
                tn = e.Node.Nodes.Add($"{itemAddress.Name}");
                if (itemAddress.GetChildCount(out int childcount) == 0)
                {
                    if (childcount > 0)
                        tn.Nodes.Add("Loading...");
                }
                tn.Tag = itemAddress;
                setStatus("connected");
            }
            setStatus("connected");
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            if (!controller.IsConnected)
            {
                MessageBox.Show("PLC未连接");
                return;
            }
            string symbol = tbSymbol.Text;
            if (!string.IsNullOrEmpty(symbol))
            {
                if (controller.Read(new List<string>() { symbol }, out List<PValue> values) == 0 && values.Count > 0)
                    tbValue.Text = values[0].ToString();
            }
        }

        private void btn_LoadXml_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "选择 TXT 文件";
                openFileDialog.Filter = "TXT 文件|*.txt";
                openFileDialog.FilterIndex = 1;
                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    treeView1.Nodes.Clear();
                    setStatus("loading...");
                    IntPtr rootHandle = controller.LoadSymbolsFromFile(openFileDialog.FileName);
                    TreeNode tn;
                    List<ItemAddress> itemInfoList = controller.Browse(rootHandle);
                    foreach (ItemAddress itemAddress in itemInfoList)
                    {
                        tn = treeView1.Nodes.Add(itemAddress.Name);
                        if (itemAddress.GetChildCount(out int childcount) == 0)
                        {
                            if (childcount > 0)
                                tn.Nodes.Add("Loading...");
                        }
                        tn.Tag = itemAddress;
                    }
                    setStatus("load finished");
                }
            }
        }
    }
}
