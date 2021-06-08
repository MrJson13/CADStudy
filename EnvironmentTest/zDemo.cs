using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentTest
{
    public class zDemo
    {
        #region Day1
        /// <summary>
        /// 测试环境
        /// </summary>
        [CommandMethod("TestEnv")] // 添加命令标识符
        public void TestEnv()
        {
            // 声明命令行对象
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            // 向命令行输出一段文字
            ed.WriteMessage("CAD二次开发笔记（1）：环境测试！");
        }
        #endregion
        #region Day2
        /// <summary>
        /// 绘线
        /// </summary>
        [CommandMethod("DrawLines")]
        public void DrawLines()
        {
            //声明一个直线对象
            Line lien1 = new Line();
            //声明两个坐标点对象
            Point3d startPoint = new Point3d(100, 100, 0);
            Point3d endPoint = new Point3d(200, 200, 0);
            //设置属性
            lien1.StartPoint = startPoint;
            lien1.EndPoint = endPoint;
            //声明数据库对象
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            //开启事务处理
            using(Transaction trans = db.TransactionManager.StartTransaction())
            {
                //以读的方式打开块表
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                //以写打开块表记录
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                //加直线到块表记录
                btr.AppendEntity(lien1);
                //更新数据
                trans.AddNewlyCreatedDBObject(lien1, true);
                //事务提交
                trans.Commit();
            }
        }
        #endregion
        #region Day3
        [CommandMethod("TestLine")]
        public void TestLine()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Line line1 = new Line(new Point3d(100, 100, 0), new Point3d(200, 100, 0));
            //调用封装函数,添加图形文件
            //db.AddEntityToModeSpace(line1);
            Line line2 = new Line(new Point3d(200, 100, 0), new Point3d(200, 200, 0));
            Line line3 = new Line(new Point3d(200, 200, 0), new Point3d(100, 100, 0));
            //调用封装函数,添加多个图形文件
            db.AddEntityToModeSpace(line1,line2,line3);
        }
        [CommandMethod("TestLine2")]
        public void TestLine2()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            //db.AddLineToModeSpace(new Point3d(100, 100, 0), new Point3d(200, 100, 0));
            //Otherwise
            db.AddLineToModeSpace(new Point3d(200, 100, 0), 30, 35);
            db.AddLineToModeSpace(new Point3d(200, 200, 0), 30, 35);
        }
        #endregion
        #region Day4
        /// <summary>
        /// 画圆弧
        /// </summary>
        [CommandMethod("TestArc")]
        public void TestArc()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Point3d startPoint = new Point3d(100, 100, 0);
            Point3d arcOnPoint = new Point3d(200, 200, 0);
            Point3d endPoint = new Point3d(150, 100, 0);
            CircularArc3d cArc = new CircularArc3d(startPoint, arcOnPoint, endPoint);

            double radius = cArc.Radius;
            Point3d center = cArc.Center;
            Vector3d cs = center.GetVectorTo(startPoint);
            Vector3d ce = center.GetVectorTo(endPoint);
            Vector3d xVector = new Vector3d(1, 0, 0);
            double startAngle = cs.Y > 0 ? cs.GetAngleTo(xVector) : -xVector.GetAngleTo(cs);
            double endAngle =ce.Y > 0 ? xVector.GetAngleTo(ce) : -xVector.GetAngleTo(ce);
            Arc arc = new Arc(center, radius, startAngle, endAngle);

            db.AddEntityToModeSpace(arc);
        }
        /// <summary>
        /// 画圆
        /// </summary>
        [CommandMethod("TestCircle")]
        public void TestCircle()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            //圆心画圆
            db.AddCircleToModeSpace(new Point3d(100, 100, 0), 100);
            //两点画圆
            db.AddCircleToModeSpace(new Point3d(200, 100, 0), new Point3d(300, 100, 0));
            //三点画圆
            db.AddCircleToModeSpace(new Point3d(400, 100, 0), new Point3d(600, 100, 0), new Point3d(600, 100, 0));
        }
        /// <summary>
        /// 多线段
        /// </summary>
        [CommandMethod("TestPolyLine")]
        public void TestPolyLine()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            //绘制多线段
            db.AddPolyLineToModeSpace(false, 0, new Point2d(10, 10), new Point2d(50, 10), new Point2d(100, 20)); //参数： 是否闭合 线宽 两个平面点
            db.AddPolyLineToModeSpace(true, 0, new Point2d(10, 50), new Point2d(50, 50), new Point2d(100, 70));
            //绘制矩形
            db.AddRectToModeSpace(new Point2d(0, 0), new Point2d(100, 100));
            db.AddRectToModeSpace(new Point2d(200, 200), new Point2d(100, 100));
            db.AddRectToModeSpace(new Point2d(500, 500), new Point2d(100, 100));

            //绘制正多边形
            db.AddPolygonToModeSpace(new Point2d(100, 100), 50, 3, 90);
            db.AddPolygonToModeSpace(new Point2d(200, 100), 50, 4, 45);
            db.AddPolygonToModeSpace(new Point2d(300, 100), 50, 5, 90);
            db.AddPolygonToModeSpace(new Point2d(400, 100), 50, 6, 0);
            db.AddPolygonToModeSpace(new Point2d(500, 100), 50, 12, 0);
        }
        /// <summary>
        /// 椭圆
        /// </summary>
        [CommandMethod("TestEllipse")]
        public void TestEllipse()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            //绘制椭圆
            db.AddEllipseToModeSpace(new Point3d(20, 20, 0), new Point3d(200, 200, 0),60);
            db.AddEllipseToModeSpace(new Point3d(100, 100, 0), new Point3d(500, 500, 0));
        }
        #endregion
    }
}
