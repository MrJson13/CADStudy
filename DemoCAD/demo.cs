using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AcDoNetTools;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;

namespace DemoCAD
{
    public class demo
    {
        #region Day6
        /// <summary>
        /// dll测试
        /// </summary>
        [CommandMethod("TestColor")]
        public void TestColor()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            //调用图形绘制
            db.AddCircleToModeSpace(new Point3d(100, 100, 0), 50);

            Circle c1 = new Circle(new Point3d(100, 100, 0), new Vector3d(0, 0, 1), 50);
            Circle c2 = new Circle(new Point3d(200, 100, 0), new Vector3d(0, 0, 1), 50);

            c1.ColorIndex = 1;
            c2.Color = Color.FromRgb(23, 156, 255);

            db.AddEntityToModeSpace(c1, c2);
        }
        /// <summary>
        /// 用户交互
        /// </summary>
        [CommandMethod("TestPrompt")]
        public void TestPrompt()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            // 获取命令行窗口
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;​
            Point3d p1 = new Point3d(0, 0, 0);
            Point3d p2 = new Point3d();
​
            PromptPointOptions ppo = new PromptPointOptions("请指定第一个点：");
            ppo.AllowNone = true;
            PromptPointResult ppr = ed.GetPoint(ppo);
​
            if (ppr.Status == PromptStatus.Cancel) return;
​
            if (ppr.Status == PromptStatus.OK) p1 = ppr.Value;
            ppo.Message = "请指定第二个点";
            ppo.BasePoint = p1;
            ppo.UseBasePoint = true;
            ppr = ed.GetPoint(ppo);
            if (ppr.Status == PromptStatus.Cancel) return;
            if (ppr.Status == PromptStatus.None) return;
            if (ppr.Status == PromptStatus.OK) p2 = ppr.Value;
            db.AddLineToModeSpace(p1, p2);
        }
        #endregion
    }
}
