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
            //获取命令行窗口 
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Point3d p1 = new Point3d(0, 0, 0);
            Point3d p2 = new Point3d();
            PromptPointOptions ppo = new PromptPointOptions("请指定第一个点");
            ppo.AllowNone = true;
            PromptPointResult ppr = ed.GetPoint(ppo);
            if (ppr.Status == PromptStatus.Cancel) return;
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
        #region Day7
        /// <summary>
        /// 单行文本
        /// </summary>
        [CommandMethod("TestDBText")]
        public void TestDBText()
        {
            Database db = HostApplicationServices.WorkingDatabase;

            // 绘制一些沿Y轴增长的直线
            Line[] line = new Line[10];
            Point3d[] pt1 = new Point3d[10];
            Point3d[] pt2 = new Point3d[10];
            for (int i = 0; i < line.Length; i++)
            {
                pt1[i] = new Point3d(50, 50 + 20 * i, 0);
                pt2[i] = new Point3d(150, 50 + 20 * i, 0);
                line[i] = new Line(pt1[i], pt2[i]);
            }
            db.AddEntityToModeSpace(line);


            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DBText text0 = new DBText(); // 新建单行文本对象
                text0.Position = pt1[0]; // 设置文本位置 
                text0.TextString = "你好 数据智能笔记！text0"; // 设置文本内容
                text0.Height = 5;  // 设置文本高度
                text0.Rotation = Math.PI * 0.5;  // 设置文本选择角度
                text0.IsMirroredInX = true; // 在X轴镜像
                text0.HorizontalMode = TextHorizontalMode.TextCenter; // 设置对齐方式
                text0.AlignmentPoint = text0.Position; //设置对齐点
                db.AddEntityToModeSpace(text0);

                DBText text1 = new DBText(); // 新建单行文本对象
                text1.Position = pt1[1];
                text1.TextString = "你好 数据智能笔记！text1";
                text1.Height = 10;
                text1.Rotation = Math.PI * 0.1;
                text1.IsMirroredInY = true; // 在Y轴镜像
                text1.HorizontalMode = TextHorizontalMode.TextLeft;
                text1.AlignmentPoint = text1.Position; // 设置对齐点
                db.AddEntityToModeSpace(text1);

                trans.Commit();
            }
        }
        /// <summary>
        /// 多行文本
        /// </summary>
        [CommandMethod("TestMText")]
        public void TestMText() {
            Database db = HostApplicationServices.WorkingDatabase;

            MText mtext = new MText();//声明多行文本对象
            mtext.Location = new Point3d(100, 100, 0);
            mtext.Contents = "多行文本文字内容blabla";
            mtext.TextHeight = 10;//文本高度
            mtext.Width = 3;//文本框宽
            mtext.Height = 5;//文本框高

            MText mtext2 = new MText();//声明多行文本对象
            mtext2.Location = new Point3d(200, 100, 0);
            mtext2.Contents = "MText word content 布拉布拉";
            mtext2.TextHeight = 10;//文本高度
            mtext2.Width = 3;//文本框宽
            mtext2.Height = 5;//文本框高

            db.AddEntityToModeSpace(mtext, mtext2);

        }
        /// <summary>
        /// 图层操作
        /// </summary>
        [CommandMethod("LayerDemo")]
        public void LayerDemo()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            AddLayerResult alr = db.AddLayer("文字");
            db.ChangeLayerColor("文字", 1);
            //db.LockLayer("文字");
            //db.UnLockLayer("文字");
            //db.ChangleLineWeight("文字", LineWeight.LineWeight050);
            //db.SetCurrentLayer("文字");
            //List<LayerTableRecord> list = db.GetAllLayers();
            //db.DeleteLayer("图层1",true);
            db.DeleteNotUsedLayer();
        }
        #endregion
    }
}
