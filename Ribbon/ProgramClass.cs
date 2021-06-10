using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ribbon
{
    public class ProgramClass
    {
        [CommandMethod("RibbonOld")]
        public void RibbonOld()
        {
            RibbonControl ribbonCtrl = ComponentManager.Ribbon;
            RibbonTab tab = ribbonCtrl.AddTab("测试选项卡", "Acad.RibbonId1", true);

            tab.AddPanel("绘图");
            tab.AddPanel("修改");
            tab.AddPanel("注释");
            tab.AddPanel("图层");
        }
        [CommandMethod("RibbonNew")]
        public void RibbonNew()
        {
            RibbonControl ribbonCtrl = ComponentManager.Ribbon; //获取cad的Ribbon界面
            RibbonTab tab = ribbonCtrl.AddTab("New测试选项卡", "Acad.RibbonId1", true); //给Ribbon界面添加一个选项卡
            CurPaht.curPaht = Path.GetDirectoryName(this.GetType().Assembly.Location) + "\\"; //获取程序集的加载路径
            RibbonPanelSource panelSource = tab.AddPanel("绘图"); //给选项卡添加面板
            panelSource.Items.Add(RibbonButtonInfos.LineBtn); //添加直线命令按钮
            panelSource.Items.Add(RibbonButtonInfos.PolylineBtn); //添加多段线命令按钮

        }
    }
}
